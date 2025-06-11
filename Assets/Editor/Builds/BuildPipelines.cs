using RemoteEducation.Scenarios;
using System.IO;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;
using VARLab.SCORM.Editor;
using VARLab.SCORM;

namespace RemoteEducation.Builds
{
    public partial class BuildPipelines : EditorWindow
    {
        private static ScenarioToLoad scenarioToLoad;

        private static BuildPipelinesSettings buildSettings;
        private static BuildPipelines window;

        private static int buildId;
        private static string buildLocation, scormLocation;

    //    private static GUIStyle richTextStyle;

        private static string originalDefineString;
        private static bool metadataFoldout = true;

        [MenuItem("Window/Build/CORE Builder")]
        private static void OpenBuildWindow()
        {
            window = (BuildPipelines)EditorWindow.GetWindow(typeof(BuildPipelines));
            window.titleContent.text = "CORE Builder";
            window.Show();
        }

        private void Awake()
        {
            InitBuildSettings();
            InitScenarioToLoad();
        }

        private void OnValidate()
        {
            InitBuildSettings();
            InitScenarioToLoad();
        }

        void OnGUI()
        {
            EditorGUILayout.LabelField(LABEL_BUILD_SETTINGS, EditorStyles.boldLabel);
            buildSettings.BuildTarget = (BuildTarget)EditorGUILayout.EnumPopup(LABEL_PLATFORM, buildSettings.BuildTarget);

            EditorGUILayout.Space(5);

            buildSettings.LoadFromCCD = CreateLeftToggle(LABEL_LOAD_FROM_CCD, TOOLTIP_LOAD_FROM_CCD, buildSettings.LoadFromCCD);
            buildSettings.BuildAddressables = CreateLeftToggle(LABEL_BUILD_ADDRESSABLES, TOOLTIP_BUILD_ADDRESSABLES, buildSettings.BuildAddressables);

            if (buildSettings.LoadFromCCD && buildSettings.BuildAddressables)
                buildSettings.UploadToCCD = CreateLeftToggle(LABEL_UPLOAD_TO_CCD, TOOLTIP_UPLOAD_TO_CCD, buildSettings.UploadToCCD);

            buildSettings.DevelopmentBuild = CreateLeftToggle(LABEL_DEVELOPMENT_BUILD, TOOLTIP_DEVELOPMENT_BUILD, buildSettings.DevelopmentBuild);

            buildSettings.loadSpecificScenario = CreateLeftToggle(LABEL_LOAD_SCENARIO, TOOLTIP_LOAD_SCENARIO, buildSettings.loadSpecificScenario);

            if (buildSettings.loadSpecificScenario)
            {
                scenarioToLoad.fileName = EditorGUILayout.TextField(LABEL_SCENARIO_TO_LOAD, scenarioToLoad.fileName);
            }

            if (buildSettings.BuildTarget == BuildTarget.WebGL)
            {
                buildSettings.CreateSCORMPackage = CreateLeftToggle(LABEL_CREATE_SCORM, TOOLTIP_CREATE_SCORM, buildSettings.CreateSCORMPackage);

                if (buildSettings.CreateSCORMPackage)
                {

                    metadataFoldout = EditorGUILayout.Foldout(metadataFoldout, LABEL_METADATA, EditorStyles.foldout);
                    if (metadataFoldout)
                    {
                        GUILayout.BeginVertical("TextArea");

                        buildSettings.SCORMExportData.ManifestIdentifier = EditorGUILayout.TextField(LABEL_MANIFEST_IDENTIFIER, buildSettings.SCORMExportData.ManifestIdentifier);
                        buildSettings.SCORMExportData.CourseTitle = EditorGUILayout.TextField(LABEL_COURSE_TITLE, buildSettings.SCORMExportData.CourseTitle);
                        buildSettings.SCORMExportData.CourseDescription = EditorGUILayout.TextField(LABEL_COURSE_DESCRIPTION, buildSettings.SCORMExportData.CourseDescription);
                        buildSettings.SCORMExportData.SCOTitle = EditorGUILayout.TextField(LABEL_SCO_TITLE, buildSettings.SCORMExportData.SCOTitle);
                        buildSettings.SCORMExportData.DataFromLMS = EditorGUILayout.TextField(LABEL_DATA_FROM_LMS, buildSettings.SCORMExportData.DataFromLMS);
                        buildSettings.SCORMExportData.CompletedByProgressAmount = CreateLeftToggle(LABEL_COMPLETED_BY_PROGRESS, TOOLTIP_COMPLETED_BY_PROGRESS, buildSettings.SCORMExportData.CompletedByProgressAmount);

                        if (buildSettings.SCORMExportData.CompletedByProgressAmount)
                        {
                            buildSettings.SCORMExportData.ProgressAmountForCompletion = EditorGUILayout.Slider(buildSettings.SCORMExportData.ProgressAmountForCompletion, 0f, 1f);
                        }

                        buildSettings.SCORMExportData.TimeLimitAction = (TimeLimitAction)EditorGUILayout.EnumPopup(LABEL_TIME_LIMIT_ACTION, buildSettings.SCORMExportData.TimeLimitAction);

                        if (buildSettings.SCORMExportData.TimeLimitAction != TimeLimitAction.None)
                        {
                            buildSettings.SCORMExportData.TimeLimit = EditorGUILayout.TextField(LABEL_TIME_LIMIT, buildSettings.SCORMExportData.TimeLimit);
                        }
                        GUILayout.EndVertical();

                    }
                }
            }

            buildSettings.RunAfterBuilding = CreateLeftToggle(LABEL_RUN_AFTER, TOOLTIP_RUN_AFTER, buildSettings.RunAfterBuilding);

            EditorGUILayout.Space(5);

            if (buildSettings.BuildTarget == EditorUserBuildSettings.activeBuildTarget)
            {
                var startBuild = GUILayout.Button("Build");

                if (startBuild)
                    Build(buildSettings);
            }
            else
            {
                var swapPlatform = GUILayout.Button("Swap Platform");

                if (swapPlatform)
                    SwapPlatform(buildSettings);
            }
            
            EditorGUILayout.Space(5);

            if (!string.IsNullOrEmpty(buildLocation))
                EditorGUILayout.LabelField($"Built to {buildLocation}");

            if (!string.IsNullOrEmpty(scormLocation))
                EditorGUILayout.LabelField($"SCORM Package exported to {scormLocation}");

            EditorUtility.SetDirty(buildSettings);
            EditorUtility.SetDirty(scenarioToLoad);
        }

        private static void SwapPlatform(BuildPipelinesSettings settings)
        {
            if (settings.BuildTarget == EditorUserBuildSettings.activeBuildTarget)
                return;

            var buildTargetGroup = GetBuildTargetGroup(settings);

            if (buildTargetGroup != BuildTargetGroup.Unknown)
            {
                EditorUserBuildSettings.SwitchActiveBuildTargetAsync(buildTargetGroup, settings.BuildTarget);
            }
        }

        public static void Build(BuildPipelinesSettings settings)
        { 
            scormLocation = null;

            if (!HandleAddressables(settings)) 
            {
                Debug.LogError("Error handling addressables before building executable. Build pipeline has stopped");
                return; 
            }

            AddressableAssetSettingsDefaultObject.Settings.BuildRemoteCatalog = true; // Reset this just in case someone does a cloud build later.

            buildLocation = GetBuildName(settings);

            if (string.IsNullOrEmpty(buildLocation))
                return;

            var buildOptions = GetBuildOptions(settings);

            originalDefineString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);

            if (settings.loadSpecificScenario)
                SetScriptingDefineSymbols(scenarioToLoad.fileName);

            BuildPipeline.BuildPlayer(DEFAULT_SCENES, buildLocation, settings.BuildTarget, buildOptions);

            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, originalDefineString); // Set our platform defines back to what they were.

            PlayerPrefs.SetInt(EDITOR_BUILD_ID_KEY, buildId);

            if (settings.BuildTarget == BuildTarget.WebGL && settings.CreateSCORMPackage)
            {
                scormLocation = $"{buildLocation}/{settings.SCORMExportData.ManifestIdentifier}.zip";
                ScormExport.Publish(buildLocation, scormLocation, settings.SCORMExportData);
            }
        }

        public static BuildTargetGroup GetBuildTargetGroup(BuildPipelinesSettings settings)
        {
            var buildTargetGroup = BuildTargetGroup.Unknown;

            switch (settings.BuildTarget)
            {
                // Add more cases here as the platforms grow.
                case BuildTarget.WebGL:
                    buildTargetGroup = BuildTargetGroup.WebGL;
                    break;

                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneOSX:
                case BuildTarget.StandaloneLinux64:
                case BuildTarget.StandaloneWindows64:
                    buildTargetGroup = BuildTargetGroup.Standalone;
                    break;

                default:
                    Debug.LogError("Canceled Build: No Build Target Group set for this platform! If you wish to build for this platform, add support for it in BuildPipelines.cs!");
                    break;
            }

            return buildTargetGroup;
        }

        private static BuildOptions GetBuildOptions(BuildPipelinesSettings settings)
        {
            var buildOptions = BuildOptions.ShowBuiltPlayer;

            if (settings.DevelopmentBuild)
                buildOptions |= BuildOptions.Development;

            if (settings.RunAfterBuilding)
                buildOptions |= BuildOptions.AutoRunPlayer;

            return buildOptions;
        }

        private static string GetBuildName(BuildPipelinesSettings settings)
        {
            var path = $"{BUILD_DIRECTORY}/{settings.BuildTarget}";

            Directory.CreateDirectory(path);

            buildId = GetBuildId() + 1;
            var label = settings.LoadFromCCD ? "Remote" : "Local";
            var buildType = settings.DevelopmentBuild ? "Dev" : "Release";
            var module = settings.loadSpecificScenario ? scenarioToLoad.fileName : "All";
            
            var subPath = $"{path}/Build {buildId} ({module} {label} {buildType})";

            Directory.CreateDirectory(subPath);

            switch (settings.BuildTarget)
            {
                case BuildTarget.StandaloneWindows64:
                    return $"{subPath}/CORE.exe";

                case BuildTarget.WebGL:
                    return $"{subPath}/CORE";

                default:
                    Debug.LogError("BuildPipelines: No build location setting for this platform! If you wish to build for this platform, add support for it in BuildPipelines.cs!");
                    return null;
            }
        }

        public static void SetScriptingDefineSymbols(string defStr)
        {
            if (string.IsNullOrEmpty(defStr))
                return;

            var defines = GetModuleScriptingDefineSymbols(defStr);

            if (string.IsNullOrEmpty(defines))
                return;

            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, $"{originalDefineString};{EXCLUDE_PLAYBACK_DEFINE};{EXCLUDE_MODULES_DEFINE};{defines}");
        }

        private static string GetModuleScriptingDefineSymbols(string defStr)
        {
            var scenarios = Serialization.GetScenarios(Serialization.FileType.XML);

            foreach(var scenario in scenarios)
            {
                if(scenario.OriginalFileName.Equals(defStr))
                {
                    scenarioToLoad.fileName = defStr;
                    return scenario.Defines;
                }
            }

            return null;
        }

        private static int GetBuildId()
        {
            if (PlayerPrefs.HasKey(EDITOR_BUILD_ID_KEY))
                return PlayerPrefs.GetInt(EDITOR_BUILD_ID_KEY);

            return 0;
        }

        private static bool CreateLeftToggle(string label, string tooltip, bool value)
        {
            return EditorGUILayout.ToggleLeft(new GUIContent(label, tooltip), value);
        }
    }
}
