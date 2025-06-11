using RemoteEducation.Interactions;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace RemoteEducation
{
    /// <summary>Entry point for our application.</summary>
    /// <remarks>A poor excuse for main().</remarks>

    public class ApplicationEntry : MonoBehaviour
    {
        /// <summary>Denotes if this script has ran already.</summary>
        private static bool initialized = false;

        /// <summary><see cref="Scene"/> containing all that should persist across all scenes.</summary>
        private Scene scene;

        private void Awake()
        {
            if (initialized)
                return;

            scene = SceneManager.LoadScene("PersistentObjects", new LoadSceneParameters(LoadSceneMode.Additive));
        }

        private IEnumerator Start()
        {
            if (!initialized)
            {
                if (scene != null)
                {
                    SetObjectsAsPersistent(scene);
                    SceneManager.UnloadSceneAsync(scene);
                }

                RunUglyCoupledStaticFunctions();

#if UNITY_EDITOR
                yield return CreateBugReporter();
#else
                yield return null;
#endif

                initialized = true;
            }

            Destroy(gameObject);
        }

        /// <summary>Sets all root objects within a scene to DontDestroyOnLoad.</summary>
        /// <param name="scene">Scene containing all persistent objects to be marked with DontDestroyOnLoad.</param>
        private static void SetObjectsAsPersistent(Scene scene)
        {
            foreach (var obj in scene.GetRootGameObjects())
            {
                DontDestroyOnLoad(obj);
            }
        }

#if UNITY_EDITOR

        private IEnumerator CreateBugReporter()
        {
            var handle = Addressables.InstantiateAsync("Internal Bug Reporter");

            yield return handle;

            DontDestroyOnLoad(handle.Result);
        }

#endif

        /// <summary>Dumping ground for all static functions we wish to run right at application start.</summary>
        /// <remarks>This is bad code but at least we can aggregate all of our ugly class coupling into this one function rather than spreading it out.</remarks>
        private static void RunUglyCoupledStaticFunctions()
        {
            CursorIcons.Init();

#if UNITY_WEBGL && !UNITY_EDITOR
            Application.targetFrameRate = -1;
#else
            Application.targetFrameRate = Screen.currentResolution.refreshRate;
#endif

            VARConsole.VARConsole.OnConsoleOpen += OnConsoleOpen;
            RemoteEducation.Editor.BugReporting.BugReporter.OnBugReporterOpen += OnBugReporterOpen;
        }

        private static void OnBugReporterOpen(bool opened)
        {
            SetLock(opened, "bugreporter");

        }

        private static void OnConsoleOpen(bool opened)
        {
            SetLock(opened, "console");
        }

        private static void SetLock(bool opened, string id)
        {
            if (opened)
            {
                Hotkeys.AddLock(id);
                return;
            }

            Hotkeys.RemoveLock(id);
        }
    }
}