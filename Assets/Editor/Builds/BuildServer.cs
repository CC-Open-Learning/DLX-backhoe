using UnityEditor;

namespace RemoteEducation.Builds
{
    public static class BuildServer
    {
        public static void BuildWebGL()
        {
            var settings = BuildPipelines.CreateDefaultBuildSettings();

            settings.DevelopmentBuild = false;
            settings.BuildTarget = (BuildTarget)20;
            settings.CreateSCORMPackage = true;
            settings.BuildAddressables = true;
            settings.LoadFromCCD = true;
            settings.loadSpecificScenario = false;
            settings.UploadToCCD = true;



            BuildPipelines.Build(settings);
        }
    }
}
