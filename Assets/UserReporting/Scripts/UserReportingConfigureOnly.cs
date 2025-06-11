using Unity.Cloud.UserReporting.Plugin;
using UnityEngine;

namespace RemoteEducation.UserReporting
{
    /// <summary>Represents a behavior that configures user reporting, but does not provide any additional functionality.</summary>
    public class UserReportingConfigureOnly : MonoBehaviour
    {
        private void Start()
        {
            if (UnityUserReporting.CurrentClient == null)
            {
                UnityUserReporting.Configure();
            }
        }
    }
}