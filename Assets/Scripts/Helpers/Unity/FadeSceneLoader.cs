using RemoteEducation.Localization;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RemoteEducation.Helpers.Unity
{
    /// <summary>Handles seamlessly moving from one scene to another with smooth visuals.</summary>
    public class FadeSceneLoader : MonoBehaviour
    {
        public delegate void OnLoadSceneStart(string sceneName);

        public static event OnLoadSceneStart OnLoadScene;

        public enum LoadMessage
        {
            Loading = 0,
            Downloading = 1
        }

        private const string TOKEN_LOAD_ERROR_MESSAGE = "Engine.LoadSceneErrorMessage";
        private const string TOKEN_CONTACT_EMAIL = "Engine.ContactEmail";
        private const string TOKEN_DOWNLOADING = "Engine.DownloadingContent";
        private const string TOKEN_LOADING = "Engine.LoadingScenario";

        /// <summary>Time spent fading into and out of the different loading <see cref="CanvasGroup"/> objects.</summary>
        private const float FADETIME = .75f;

        [Tooltip("Group containing elements to fade into and out of during the loading of a scenario.")]
        [SerializeField] private CanvasGroup fadeGroup;

        [Tooltip("Group containing UI elements showing the progress of the scenario download/load.")]
        [SerializeField] private CanvasGroup progressGroup;

        [Tooltip("Group for the main background image used for the main menu.")]
        [SerializeField] private CanvasGroup mainBackgroundGroup;

        /// <summary>Current instance of the <see cref="FadeSceneLoader"/>.</summary>
        private static FadeSceneLoader singleton;

        [Tooltip("Scenario download and loading progress bar image.")]
        [SerializeField] private Image progressBar;

        [Tooltip("Window to show if there is an error while loading a scenario.")]
        [SerializeField] private GameObject errorWindow;

        [Tooltip("Displays the error message when a scenario fails to load.")]
        [SerializeField] private TextMeshProUGUI errorMessage;

        [Tooltip("Displays the loading message alongside the progress bar.")]
        [SerializeField] private TextMeshProUGUI loadMessage;

        private void Awake()
        {
            var persistentObjects = new GameObject[] { gameObject, mainBackgroundGroup.transform.parent.gameObject };

            if (singleton != null)
            {
                DestroyDuplicatePersistentObjects(persistentObjects);
                Debug.LogWarning("FadeSceneLoader: Singleton already exists! Deleting this duplicate.");
                return;
            }

            singleton = this;

            ShowLoadProgress(false);
        }

        public static void LoadSceneWithFade(string sceneName)
        {
            OnLoadScene?.Invoke(sceneName);

            singleton.StartCoroutine(singleton._LoadSceneWithFade(sceneName));
        }

        private IEnumerator _LoadSceneWithFade(string sceneName)
        {
            ShowLoadProgress(false);

            if (CurrentSceneIsStartScene())
                yield return FadeCanvasGroupsIn(FADETIME, fadeGroup);
            else
                yield return FadeCanvasGroupsIn(FADETIME, fadeGroup, mainBackgroundGroup);

            var operation = SceneManager.LoadSceneAsync(sceneName);

            yield return operation;

            if (sceneName.Equals(Constants.START_SCENE_NAME))
                yield return FadeCanvasGroupsOut(FADETIME, fadeGroup);
        }

        public static void FinishedLoadingScene()
        {
            singleton.StartCoroutine(singleton.FinishSceneLoad());
        }

        private IEnumerator FinishSceneLoad()
        {
            ShowLoadProgress(false);

            yield return FadeCanvasGroupsOut(FADETIME, fadeGroup, mainBackgroundGroup);
        }

        public static void UpdateLoadProgress(float t)
        {
            t = Mathf.Clamp(t, 0f, 1f);

            if (t >= 1f)
                singleton.progressGroup.alpha = 0f;

            singleton.progressBar.fillAmount = t;
        }

        public static void ShowLoadProgress(bool enable, LoadMessage message = LoadMessage.Loading)
        {
            singleton.progressGroup.alpha = enable ? 1f : 0f;
            singleton.loadMessage.text = Localizer.Localize(message == LoadMessage.Loading ? TOKEN_LOADING : TOKEN_DOWNLOADING) + "...";
        }

        public void CloseErrorMessage()
        {
            errorWindow.SetActive(false);
        }

        public static void QuitApplication()
        {
            singleton.StartCoroutine(singleton.Quit());
        }

        private IEnumerator Quit()
        {
            yield return FadeCanvasGroupsIn(FADETIME, fadeGroup);

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private IEnumerator FadeCanvasGroupsIn(float fadeTime, params CanvasGroup[] canvasGroups)
        {
            for (float t = 0f; t < 1f; t += Time.unscaledDeltaTime / fadeTime)
            {
                SetGroupAlphas(canvasGroups, t);
                yield return null;
            }

            SetGroupAlphas(canvasGroups, 1f);
        }

        private IEnumerator FadeCanvasGroupsOut(float fadeTime, params CanvasGroup[] canvasGroups)
        {
            for (float t = 1f; t >= 0f; t -= Time.unscaledDeltaTime / FADETIME)
            {
                SetGroupAlphas(canvasGroups, t);
                yield return null;
            }

            SetGroupAlphas(canvasGroups, 0f);
        }

        private static void SetGroupAlphas(CanvasGroup[] canvasGroups, float alpha)
        {
            for (int i = 0; i < canvasGroups.Length; i++)
            {
                canvasGroups[i].alpha = alpha;
            }
        }

        public static void OpenErrorMessage()
        {
            singleton.errorWindow.SetActive(true);
            singleton.errorMessage.text = $"{Localizer.Localize(TOKEN_LOAD_ERROR_MESSAGE)}\n{Localizer.Localize(TOKEN_CONTACT_EMAIL)}";
        }

        public static void LoadSceneFailed()
        {
            OpenErrorMessage();
            LoadSceneWithFade(Constants.START_SCENE_NAME);
        }


        private static void DestroyDuplicatePersistentObjects(GameObject[] persistentObjects)
        {
            for (int i = 0; i < persistentObjects.Length; i++)
            {
                Destroy(persistentObjects[i]);
            }
        }

        private static bool CurrentSceneIsStartScene()
        {
            return SceneManager.GetActiveScene().buildIndex == 0;
        }
    }
}