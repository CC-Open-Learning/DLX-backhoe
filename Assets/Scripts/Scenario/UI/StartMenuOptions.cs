using UnityEngine;
using System.Linq;
using System;

namespace RemoteEducation.Scenarios
{
    [DisallowMultipleComponent]
    public class StartMenuOptions : MonoBehaviour
    {
        [SerializeField]
        private GameObject scenariosButton;

        [SerializeField]
        private GameObject playbackButton;

        [SerializeField]
        private GameObject startButton;

        [SerializeField]
        private GameObject quitButton;

        [SerializeField]
        private ScenarioToLoad scenarioToLoad;

        /// <summary>Persistent build data, used to load into the correct scenario and provide a build identifier for analytics.</summary>
        public static ScenarioToLoad ScenarioToLoad;

        private Scenario scenario;

        void Awake()
        {
            ScenarioToLoad = scenarioToLoad;

            scenariosButton.SetActive(false);
            startButton.SetActive(true);

            GetScenario();

            BeginModuleDownload();

            //scenariosButton.SetActive(true);
            //startButton.SetActive(false);

#if EXCLUDE_PLAYBACK
            playbackButton.SetActive(false);
#endif

#if UNITY_WEBGL
            quitButton.SetActive(false);
#endif
        }

        private void GetScenario()
        {
            scenario = ScenarioBuilder.LoadFromFile().FirstOrDefault(e => e.OriginalFileName.Equals(scenarioToLoad.fileName));
        }

        private void BeginModuleDownload()
        {
            ScenarioBuilder.BeginScenarioDownload(scenario);
        }

        public void StartScenario()
        {
            ScenarioBuilder.CreatePersistentScenarioData(scenario);

            StartCoroutine(ScenarioManager.LazyLoad());
        }
    }
}
