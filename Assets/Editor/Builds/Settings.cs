using UnityEditor;
using VARLab.SCORM;

namespace RemoteEducation.Builds
{
    partial class BuildPipelines
    {
        private void InitBuildSettings()
        {
            if (buildSettings == null && !TryLoadSettings(SETTINGS_PATH, out buildSettings))
            {
                buildSettings = CreateDefaultBuildSettings();

                AssetDatabase.CreateAsset(buildSettings, SETTINGS_PATH);
                AssetDatabase.SaveAssets();
            }
        }

        public static BuildPipelinesSettings CreateDefaultBuildSettings()
        {
            var settings = CreateInstance<BuildPipelinesSettings>();

            settings.BuildTarget = BuildTarget.StandaloneWindows64;
            settings.BuildAddressables = true;
            settings.DevelopmentBuild = true;
            settings.CreateSCORMPackage = true;
            settings.RunAfterBuilding = true;
            settings.loadSpecificScenario = false;

            settings.SCORMExportData = new SCORMExportData
            {
                ManifestIdentifier = "CORE",
                CourseTitle = "CORE Engine",
                CourseDescription = "VARLab CORE Engine Module",
                SCOTitle = "CORE Engine",
                DataFromLMS = "",
                CompletedByProgressAmount = false,
                ProgressAmountForCompletion = 1f,
                TimeLimitAction = TimeLimitAction.None,
                TimeLimit = ""
            };

            return settings;
        }

        public static bool TryLoadSettings(string path, out BuildPipelinesSettings settings)
        {
            settings = (BuildPipelinesSettings)AssetDatabase.LoadAssetAtPath(path, typeof(BuildPipelinesSettings));

            return settings != null;
        }

        private void InitScenarioToLoad()
        {
            if (scenarioToLoad == null && !TryLoadScenarioToLoad(SCENARIO_TO_LOAD_PATH, out scenarioToLoad))
            {
                scenarioToLoad = CreateInstance<ScenarioToLoad>();

                scenarioToLoad.fileName = "";

                AssetDatabase.CreateAsset(scenarioToLoad, SCENARIO_TO_LOAD_PATH);
                AssetDatabase.SaveAssets();
            }
        }

        public static bool TryLoadScenarioToLoad(string path, out ScenarioToLoad settings)
        {
            settings = (ScenarioToLoad)AssetDatabase.LoadAssetAtPath(path, typeof(ScenarioToLoad));

            return settings != null;
        }
    }
}
