using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace RemoteEducation.Builds
{
    partial class BuildPipelines
    {
        private static bool HandleAddressables(BuildPipelinesSettings settings)
        {
            if (!settings.BuildAddressables) { return true; }


            var profileSettings = SetAddressablesProfile(settings);

            var activeProfileId = AddressableAssetSettingsDefaultObject.Settings.activeProfileId;

            var corePath = Path.Combine(Application.dataPath, @"..\");
            var fullRemoteBuildPath = corePath + profileSettings.GetValueByName(activeProfileId, "RemoteBuildPath");
            var assetBuildDirectory = Path.GetFullPath(fullRemoteBuildPath.Replace("[BuildTarget]", settings.BuildTarget.ToString()));


            var badgeName = Guid.NewGuid().ToString();

            if (settings.LoadFromCCD && settings.UploadToCCD)
                profileSettings.SetValue(activeProfileId, "BadgeName", badgeName);
            
            ClearAssetBuildDirectory(assetBuildDirectory);

            AddressableAssetSettings.BuildPlayerContent();

            if (settings.LoadFromCCD && settings.UploadToCCD)
            {
                string bucketId;

                switch (settings.BuildTarget)
                {
                    case BuildTarget.StandaloneWindows64:
                        bucketId = WINDOWS64_DEV_BUCKET_ID;
                        break;

                    case BuildTarget.WebGL:
                        bucketId = WEBGL_DEV_BUCKET_ID;
                        break;

                    default:
                        Debug.LogError("BuildPipelines: No Cloud Content Delivery Bucket ID defined for this platform! Add support for this in BuildPipelines.cs and make sure the bucket exists!");
                        return false;
                }

                var ucdPath = Path.Combine(corePath, @"Tools\");

                var releaseId = UploadAssetBundlesToCCD(assetBuildDirectory, ucdPath, bucketId, settings.SCORMExportData.ManifestIdentifier);

                if(string.IsNullOrEmpty(releaseId))
                {
                    Debug.LogError("Failed to create CCD Release!");
                    return false;
                }

                if (!AssignBadgeToRelease(badgeName, ucdPath, releaseId, bucketId))
                {
                    return false;
                }
            }

            return true;
        }

        private static AddressableAssetProfileSettings SetAddressablesProfile(BuildPipelinesSettings settings)
        {
            var profileName = "Local";

            if (settings.LoadFromCCD)
            {
                switch (settings.BuildTarget)
                {
                    case BuildTarget.StandaloneWindows64:
                        profileName = "Windows64";
                        break;

                    case BuildTarget.WebGL:
                        profileName = "WebGL";
                        break;

                    default:
                        Debug.LogError("BuildPipelines: No Remote Addressables Profile for this platform! If you wish to build for this platform, add support for it in Addressables Profiles!");
                        return null;
                }
            }

            AddressableAssetProfileSettings profileSettings = AddressableAssetSettingsDefaultObject.Settings.profileSettings;
            string profileId = profileSettings.GetProfileId(profileName);
            AddressableAssetSettingsDefaultObject.Settings.activeProfileId = profileId;

            AddressableAssetSettingsDefaultObject.Settings.BuildRemoteCatalog = settings.LoadFromCCD;

            return profileSettings;
        }

        private static void ClearAssetBuildDirectory(string assetBuildDirectory)
        {
            if (!Directory.Exists(assetBuildDirectory))
                return;

            var di = new DirectoryInfo(assetBuildDirectory);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }
        }

        private static string UploadAssetBundlesToCCD(string uploadDirectory, string ucdPath, string bucketId, string comment = "")
        {
            System.Diagnostics.Process proc;

            var releaseId = "";

            try
            {

                proc = new System.Diagnostics.Process();
                proc.StartInfo.FileName = "cmd.exe";
                proc.StartInfo.CreateNoWindow = false;
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.StandardOutputEncoding = Encoding.UTF8;
                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.StandardErrorEncoding = Encoding.UTF8;
                proc.EnableRaisingEvents = true;
                proc.StartInfo.Arguments = $"/c \"\"{ucdPath}/uploadContent.bat\" \"{uploadDirectory}\" {bucketId} \"{comment}\"\"";
                if (!proc.Start()) { Debug.LogError($"Process '{proc.ProcessName}' failed to start"); }

                string line;

                while((line = proc.StandardOutput.ReadLine()) != null)
                {
                    if (line.Contains("Id:"))
                    {
                        releaseId = line.Split(':')[1].Trim();
                    }
                }

                StringBuilder errLog = new StringBuilder();
                while ((line = proc.StandardError.ReadLine()) != null)
                {
                    errLog.AppendLine(line);
                }
                Debug.LogError(errLog.ToString());

                proc.WaitForExit();
            }
            catch (Exception ex)
            {
                Debug.Log(ex.ToString());
            }

            return releaseId;
        }

        private static bool AssignBadgeToRelease(string badgeName, string ucdPath, string releaseId, string bucketId)
        {
            System.Diagnostics.Process proc;

            try
            {
                proc = new System.Diagnostics.Process();
                proc.StartInfo.WorkingDirectory = ucdPath;
                proc.StartInfo.FileName = "createBadge.bat";
                proc.StartInfo.CreateNoWindow = false;
                proc.StartInfo.Arguments = $"{badgeName} {bucketId} {releaseId}";
                proc.Start();
                proc.WaitForExit();
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
                return false;
            }

            return true;
        }
    }
}
