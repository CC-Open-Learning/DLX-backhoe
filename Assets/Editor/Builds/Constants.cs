namespace RemoteEducation.Builds
{
    partial class BuildPipelines
    {
        private const string EXCLUDE_MODULES_DEFINE = "EXCLUDE_MODULES";
        private const string EXCLUDE_PLAYBACK_DEFINE = "EXCLUDE_PLAYBACK";

        private const string WINDOWS64_DEV_BUCKET_ID = "c60c44da-9ecb-4097-b828-bd60779238dc";
        private const string WEBGL_DEV_BUCKET_ID = "a48aa2b9-f07e-43f1-8238-ef3740bd2c4a";

        private const string SETTINGS_PATH = "Assets/Editor/Builds/BuildPipelinesSettings.asset";
        private const string SCENARIO_TO_LOAD_PATH = "Assets/Scripts/ScenarioToLoad.asset";
        private const string BUILD_DIRECTORY = "./builds";
        private const string EDITOR_BUILD_ID_KEY = "EDITOR_BUILD_ID";
        private static readonly string[] DEFAULT_SCENES = { 
            "Assets/Scenes/StartScene.unity",
            "Assets/Scenes/ScenarioScene.unity",
            "Assets/Scenes/PersistentObjects.unity"
        };

        private const string LABEL_BUILD_SETTINGS = "Build Settings:";

        private const string LABEL_PLATFORM = "Platform:";
        private const string LABEL_LOAD_FROM_CCD = "Use Cloud Content Delivery to Load Assets";
        private const string LABEL_BUILD_ADDRESSABLES = "Build Addressables";
        private const string LABEL_UPLOAD_TO_CCD = "Upload to Cloud Content Delivery";
        private const string LABEL_DEVELOPMENT_BUILD = "Development Build";
        private const string LABEL_CREATE_SCORM = "Create SCORM Package";
        private const string LABEL_RUN_AFTER = "Run After Building";
        private const string LABEL_LOAD_SCENARIO = "Plays Only One Scenario";
        private const string LABEL_SCENARIO_TO_LOAD = "Scenario Name:";

        private const string TOOLTIP_LOAD_FROM_CCD = "If checked, this build will load its assets from CCD rather than your computer.";
        private const string TOOLTIP_BUILD_ADDRESSABLES = "If checked, addressable asset bundles will be built as well.\nUse this if assets have changed since the last time they were built for this platform.\nIf in doubt, leave checked.";
        private const string TOOLTIP_UPLOAD_TO_CCD = "If checked, all built addressable asset bundles will be uploaded to the Development bucket for this platform on CCD automatically.";
        private const string TOOLTIP_DEVELOPMENT_BUILD = "If checked, build the player with development mode enabled.";
        private const string TOOLTIP_CREATE_SCORM = "If checked, the finished WebGL build will be packaged with SCORM for eConestoga deployment.";
        private const string TOOLTIP_RUN_AFTER = "If checked, the build will automatically run itself when complete.";
        private const string TOOLTIP_LOAD_SCENARIO = "If checked, this build will only be able to play the scenario specified.";

        // SCORM
        private const string LABEL_METADATA = "Metadata:";

        private const string LABEL_MANIFEST_IDENTIFIER = "Identifier:";
        private const string LABEL_COURSE_TITLE = "Course Title:";
        private const string LABEL_COURSE_DESCRIPTION = "Course Description:";
        private const string LABEL_SCO_TITLE = "Module Title:";
        private const string LABEL_DATA_FROM_LMS = "Launch Args:";
        private const string LABEL_COMPLETED_BY_PROGRESS = "Completed by Progress Amount Reached";
        private const string LABEL_TIME_LIMIT_ACTION = "Time Limit Action:";
        private const string LABEL_TIME_LIMIT = "Time Limit:";

        private const string TOOLTIP_COMPLETED_BY_PROGRESS = "Check if this module should report as complete when the student reaches a set completion percentage.";
    }
}
