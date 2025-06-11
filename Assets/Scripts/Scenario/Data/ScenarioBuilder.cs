/*
 *  ScenarioBuilder imports and initializes Scenarios, Tasks, and their associated environments 
 *  based on data from a serialized configuration file. It also has the ability to export 
 *  the set-up of Scenarios and Tasks to the same configuration file.
 */

using System;
using System.Collections.Generic;
using UnityEngine;
using RemoteEducation.Networking.LTI;
using UnityEngine.AddressableAssets;
using System.Collections;
using UnityEngine.ResourceManagement.AsyncOperations;
using RemoteEducation.Helpers.Unity;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace RemoteEducation.Scenarios
{
    /// <summary>
    ///     Imports and initializes Scenarios, Tasks, and their associated environments 
    ///     based on data from a serialized configuration file. 
    /// </summary>
    /// 
    /// <remarks>
    ///     The ScenarioBuilder also has the ability to export the set-up of Scenarios and Tasks
    ///     to the same configuration file. See <see cref="Serialization"/> for lower-level implementation
    ///     of importing and exporting.
    /// </remarks>
    public static class ScenarioBuilder
    {
        /// <summary>Uses the name of the <see cref="ScenarioData"/> class to construct the GameObject whichwill store persistent Scenario objects</summary>
        public static readonly string ScenarioDataKey = nameof(ScenarioData);

        /// <summary>Flag updated when the EnvironmentLoaded callback is invoked</summary>
        public static bool IsEnvironmentLoaded { get; private set; }

        /// <summary>Callback invoked when the Environment of a Scenario loaded in the ScenarioScene</summary>
        public static Action EnvironmentLoaded;

        /// <summary>Handle reference for the scenario asset bundle being downloaded.</summary>
        /// <remarks><see cref="AssetBundleDownload"/>.IsValid() returns false if no download is in progress.</remarks>
        public static AsyncOperationHandle AssetBundleDownload { get; private set; }

        private static float lastLoadProgress;
        public static bool assetBundleDownloadFailed;
        private static float adjustmentAmount;

        /// <summary>Find the <see cref="ScenarioData"/> object in the scene. If one isn't found, create a new one.</summary>
        /// <returns>The <see cref="ScenarioData"/> object</returns>
        private static ScenarioData FindScenarioData()
        {
            // Attempts to find a ScenarioData MonoBehaviour somewhere in the Scene.
            // If one is not found, a new GameObject with an empty ScenarioData component is created
            ScenarioData data = UnityEngine.Object.FindObjectOfType<ScenarioData>();

            if (!data)
            {
                data = new GameObject(ScenarioDataKey).AddComponent<ScenarioData>();
            }

            return data;
        }

        /// <summary>Retrieves a collection of <see cref="Scenario"/> objects from a serialized configuration file, if one exists.</summary>
        /// <param name="fileType">Enumerator indicating the file type of the serialized configuration file</param>
        /// <returns>A collection of <see cref="Scenario"/> objects. This collection is empty if the configuration a </returns>
        public static List<Scenario> LoadFromFile()
        {
            return Serialization.GetScenarios(Serialization.FileType.XML);
        }




        /// <summary>Creates a <see cref="ScenarioData>"/> game object which persists across Scene changes, and populates it with a list of <see cref="Scenario>"/> objects.</summary>
        /// <param name="scenarios">A collection of <see cref="Scenario>"/> objects to store in the Scene-persistent area of memory</param>
        /// <returns>True when no issue occurs, false if the <paramref name="scenarios"/> collection is <c>null</c> or contains no objects</returns>
        public static bool CreatePersistentScenarioData(List<Scenario> scenarios)
        {
            if (scenarios == null || scenarios.Count < 1)
            {
                Debug.LogError("SceneBuilder : Cannot create ScenarioData with no Scenarios");
                return false;
            }

            ScenarioData scenarioData = FindScenarioData();

            scenarioData.scenarios = scenarios;
            UnityEngine.Object.DontDestroyOnLoad(scenarioData);

            return true;
        }


        /// <summary>Creates a <see cref="ScenarioData>"/> object which persists across scene changes, and populates it with a <see cref="Scenario>"/> object.</summary>
        /// <param name="scenario">A Scenario to store in the Scene-persistent area of memory</param>
        /// <returns><see langword="true"/> when no issue occurs, false if the specified <paramref name="scenario"/> is <see langword="null"/></returns>
        public static bool CreatePersistentScenarioData(Scenario scenario)
        {
            if (scenario == null)
            {
                Debug.LogError("SceneBuilder : Cannot create ScenarioData with an invalid Scenario");
                return false;
            }

            return CreatePersistentScenarioData(new List<Scenario> { scenario });
        }


        /// <summary>Directly retrieves a Collection of Scenarios using the private FindScenarioData() method of the ScenarioBuilder.</summary>
        /// <returns>The Collection of Scenarios that is contained in the <see cref="ScenarioData"/> object which persists across Unity Scene changes</returns>
        public static List<Scenario> GetScenarios()
        {
            return FindScenarioData().scenarios;
        }


        /// <summary>Checks against the command-line arguments to determine if a "scenarioid" argument has been provided. </summary>
        /// <param name="scenarioId">When this method returns, contains the value passed with the "scenarioid" command-line argument. If no value is found, a value of -1 is set.</param>
        /// <returns><see langword="true"/> if the command-line arguments contains a scenarioid; otherwise, <see langword="false"/></returns>
        public static bool TryGetDefaultScenario(out int scenarioId)
        {
            scenarioId = -1;

            return (CommandLineProcessing.GetArgument("scenarioid", out string id)
                && int.TryParse(id, out scenarioId));
        }

        public static void BeginScenarioDownload(Scenario scenario)
        {
            lastLoadProgress = 0f;
            adjustmentAmount = 0f;

            assetBundleDownloadFailed = false;

            if (!AssetBundleDownload.IsValid())
                AssetBundleDownload = Addressables.DownloadDependenciesAsync(scenario.Environment, false);
        }

        /// <summary>Loads the environment and begins interaction with the specified <paramref name="scenario"/></summary>
        /// <remarks>This method must be able to locate an <see cref="IExtensionModule"/> in the loaded environment prefab, 
        /// and it must also be able to load a prefab containing an <see cref="ITaskableGraphBuilder"/></remarks>
        /// <returns>Whether the specified <paramref name="scenario"/> loaded successfully</returns>
        public static IEnumerator Build(Scenario scenario)
        {
         

            if (scenario == null)
            {
                Debug.LogError("ScenarioBuilder : Invalid Scenario provided");
                yield break;
            }

            if (!ScenarioManager.Instance || !ScenarioManager.Instance.TaskManager)
            {
                Debug.LogError("ScenarioBuilder : The ScenarioManager singleton is not properly initialized. Scenario provided");
                yield break;
            }

            var tempCam = CreateLoadCamera(); // Add a camera just so the editor doesn't complain about not having a camera during the load period.

            // Download Scenario
            FadeSceneLoader.ShowLoadProgress(true, FadeSceneLoader.LoadMessage.Downloading);

            BeginScenarioDownload(scenario);

            yield return WaitForScenarioDownload();

            if (assetBundleDownloadFailed)
            {
                FadeSceneLoader.LoadSceneFailed();
                yield break;
            }

            // Load Scene
            FadeSceneLoader.ShowLoadProgress(true);

            var handle = Addressables.LoadSceneAsync(scenario.Environment, UnityEngine.SceneManagement.LoadSceneMode.Additive);

            yield return WaitForSceneLoad(handle);

            if (handle.Status == AsyncOperationStatus.Failed)
            {
                FadeSceneLoader.LoadSceneFailed();
                yield break;
            }

         

            // Clean up after scene has loaded
            FadeSceneLoader.FinishedLoadingScene();
            GameObject.Destroy(tempCam.gameObject);

            // Set up task module.
            if (!SetTaskModule(handle.Result))
                yield break;

            SceneManager.SetActiveScene(handle.Result.Scene);

            // After extension module is validated, listeners of the EnvironmentLoaded delegate are invoked
            IsEnvironmentLoaded = true;
            EnvironmentLoaded?.Invoke();

            // Set up task graph.
            yield return SetTaskGraph(scenario);

            // Attach to the task manager, even if there is no graph.
            yield return ScenarioManager.Instance.TaskManager.Attach(scenario);
            ScenarioManager.Instance.WindowManager.SetButtonGroupVisible(true);

            
        }

        private static IEnumerator WaitForSceneLoad(AsyncOperationHandle<SceneInstance> handle)
        {
            adjustmentAmount = 0f;
            lastLoadProgress = 0f;
            float progress;
            while (!handle.IsDone)
            {
                progress = handle.PercentComplete;
                FadeSceneLoader.UpdateLoadProgress(AdjustLoadProgressValue(progress));
                yield return null;
            }
        }

   
        private static IEnumerator WaitForScenarioDownload()
        {
            float progress;
            while (!AssetBundleDownload.IsDone)
            {
                progress = AssetBundleDownload.PercentComplete;
                FadeSceneLoader.UpdateLoadProgress(AdjustLoadProgressValue(progress));
                yield return null;
            }

            assetBundleDownloadFailed |= AssetBundleDownload.Status == AsyncOperationStatus.Failed;

            Addressables.Release(AssetBundleDownload);
        }

        private static float AdjustLoadProgressValue(float t)
        {
            if(lastLoadProgress == 0f && t != 0f)
            {
                adjustmentAmount = t;
            }

            lastLoadProgress = t;

            const float MINIMUM_PROGRESS_AMOUNT = 0.1f; // People don't like to see an empty loading bar, this is purely a UXD addition.

            return (t - adjustmentAmount + MINIMUM_PROGRESS_AMOUNT) / (1f + MINIMUM_PROGRESS_AMOUNT - adjustmentAmount);
        }

        private static IEnumerator SetTaskGraph(Scenario scenario)
        {
            if (!string.IsNullOrEmpty(scenario.TaskGraphResourcePath))
            {
                // This will cause an InvalidKeyException if the path specified in the scenario file does not match the Addressable path.
                var graphHandle = Addressables.InstantiateAsync("GraphScenarios/" + scenario.TaskGraphResourcePath);

                yield return graphHandle;

                scenario.TaskGraph = graphHandle.Result.GetComponent<ITaskableGraphBuilder>();
            }
        }

        private static bool SetTaskModule(SceneInstance sceneInstance)
        {
            var rootObjects = sceneInstance.Scene.GetRootGameObjects();

            IExtensionModule extensionModule = null;

            for (int i = 0; i < rootObjects.Length; i++)
            {
                extensionModule = rootObjects[i].GetComponentInChildren<IExtensionModule>();

                if (extensionModule != null)
                    break;
            }

            if (extensionModule == null)
            {
                Debug.LogError("ScenarioBuilder : The loaded environment does not contain an Extension Module. Scenario could not be loaded");
                return false;
            }

            ScenarioManager.Instance.TaskManager.TaskModule = extensionModule;

            return true;
        }

        private static Camera CreateLoadCamera()
        {
            var tempCam = (new GameObject()).AddComponent<Camera>();
            tempCam.clearFlags = CameraClearFlags.SolidColor;
            tempCam.backgroundColor = Color.black;

            return tempCam;
        }
    }
}