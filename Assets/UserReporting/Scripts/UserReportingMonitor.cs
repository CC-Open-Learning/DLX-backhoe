using System;
using Unity.Cloud.UserReporting;
using Unity.Cloud.UserReporting.Plugin;
using UnityEngine;

namespace RemoteEducation.UserReporting
{
    /// <summary>Represents a behavior that monitors the application for issues and automatically submits a user report.</summary>
    public class UserReportingMonitor : MonoBehaviour
    {
        public UserReportingMonitor()
        {
            this.IsEnabled = true;
            this.IsHiddenWithoutDimension = true;
            Type type = this.GetType();
            this.MonitorName = type.Name;
        }

        public bool IsEnabled;
        public bool IsEnabledAfterTrigger;
        public bool IsHiddenWithoutDimension;
        public string MonitorName;
        public string Summary;

        private void Start()
        {
            if (UnityUserReporting.CurrentClient == null)
            {
                UnityUserReporting.Configure();
            }
        }

        public void Trigger()
        {
            if (!this.IsEnabledAfterTrigger)
            {
                this.IsEnabled = false;
            }

            UnityUserReporting.CurrentClient.TakeScreenshot(2048, 2048, s => { });
            UnityUserReporting.CurrentClient.TakeScreenshot(512, 512, s => { });
            UnityUserReporting.CurrentClient.CreateUserReport((br) =>
            {
                if (string.IsNullOrEmpty(br.ProjectIdentifier))
                {
                    Debug.LogWarning("The user report's project identifier is not set. Please setup cloud services using the Services tab or manually specify a project identifier when calling UnityUserReporting.Configure().");
                }

                br.Summary = this.Summary;
                br.DeviceMetadata.Add(new UserReportNamedValue("Monitor", this.MonitorName));
                string platform = "Unknown";
                string version = "0.0";
                foreach (UserReportNamedValue deviceMetadata in br.DeviceMetadata)
                {
                    if (deviceMetadata.Name == "Platform")
                    {
                        platform = deviceMetadata.Value;
                    }

                    if (deviceMetadata.Name == "Version")
                    {
                        version = deviceMetadata.Value;
                    }
                }

                br.Dimensions.Add(new UserReportNamedValue("Monitor.Platform.Version", string.Format("{0}.{1}.{2}", this.MonitorName, platform, version)));
                br.Dimensions.Add(new UserReportNamedValue("Monitor", this.MonitorName));
                br.IsHiddenWithoutDimension = this.IsHiddenWithoutDimension;
                UnityUserReporting.CurrentClient.SendUserReport(br, (success, br2) => { this.Triggered(); });
            });
        }

        protected virtual void Triggered()
        {
            // Empty
        }
    }
}