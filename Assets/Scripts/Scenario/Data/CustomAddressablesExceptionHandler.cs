using RemoteEducation.Helpers.Unity;
using System;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace RemoteEducation.Scenarios
{
    internal static class CustomAddressablesExceptionHandler
    {
        internal static void Handler(AsyncOperationHandle handle, Exception exception)
        {
            if(handle.Equals(ScenarioBuilder.AssetBundleDownload))
            {
                ScenarioBuilder.assetBundleDownloadFailed = true;
            }

            Addressables.LogException(handle, exception);
        }
    }
}