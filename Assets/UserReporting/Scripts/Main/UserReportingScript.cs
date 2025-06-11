using System;
using System.Collections;
using System.Reflection;
using System.Text;
using Unity.Cloud.UserReporting;
using Unity.Cloud.UserReporting.Client;
using Unity.Cloud.UserReporting.Plugin;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace RemoteEducation.UserReporting
{
    /// <summary>Represents a behavior for working with the user reporting client.</summary>
    /// <remarks>
    /// This script is provided as a sample and isn't necessarily the most optimal solution for your project.
    /// You may want to consider replacing with this script with your own script in the future.
    /// </remarks>
    public partial class UserReportingScript : MonoBehaviour
    {
        public UserReportingScript()
        {
            UserReportSubmitting = new UnityEvent();
            unityUserReportingUpdater = new UnityUserReportingUpdater();
        }

        public UserReport CurrentUserReport { get; private set; }

        private bool isShowingError;
        private bool isCreatingUserReport;
        private bool isSubmitting;
        private UnityUserReportingUpdater unityUserReportingUpdater;

        private static UserReportingScript singleton;

        public UserReportingState State
        {
            get
            {
                if (CurrentUserReport != null)
                {
                    if (IsInSilentMode)
                    {
                        return UserReportingState.Idle;
                    }
                    else if (isSubmitting)
                    {
                        return UserReportingState.SubmittingForm;
                    }
                    else
                    {
                        return UserReportingState.ShowingForm;
                    }
                }
                else
                {
                    if (isCreatingUserReport)
                    {
                        return UserReportingState.CreatingUserReport;
                    }
                    else
                    {
                        return UserReportingState.Idle;
                    }
                }
            }
        }

        public static void CreateUserReport()
        {
            if (singleton.isCreatingUserReport)
                return;

            singleton.isCreatingUserReport = true;

            // Take Main Screenshot
            UnityUserReporting.CurrentClient.TakeScreenshot(2048, 2048, s => { });

            // Take Thumbnail Screenshot
            UnityUserReporting.CurrentClient.TakeScreenshot(512, 512, s => { });

            // Create Report
            UnityUserReporting.CurrentClient.CreateUserReport((br) =>
            {
                // Ensure Project Identifier
                if (string.IsNullOrEmpty(br.ProjectIdentifier))
                {
                    Debug.LogWarning("The user report's project identifier is not set. Please setup cloud services using the Services tab or manually specify a project identifier when calling UnityUserReporting.Configure().");
                }

                // Attachments
                //StringBuilder sb = new StringBuilder();

                //sb.AppendLine("Add additional diagnostic info here.");
                //br.Attachments.Add(new UserReportAttachment("Sample Attachment.json", "SampleAttachment.json", "application/json", System.Text.Encoding.UTF8.GetBytes(sb.ToString())));

                // Dimensions
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

                br.Dimensions.Add(new UserReportNamedValue("Platform.Version", string.Format("{0}.{1}", platform, version)));

                singleton.CurrentUserReport = br;

                singleton.isCreatingUserReport = false;

                singleton.SetThumbnail(br);

                // Submit Immediately in Silent Mode
                if (singleton.IsInSilentMode)
                {
                    singleton.SubmitUserReport();
                }
            });
        }

        public void SubmitUserReport()
        {
            if (isSubmitting || CurrentUserReport == null)
                return;

            isSubmitting = true;

            if (SummaryInput != null)
                CurrentUserReport.Summary = SummaryInput.text;

            // Set Category
            if (CategoryDropdown != null)
            {
                TMP_Dropdown.OptionData optionData = CategoryDropdown.options[CategoryDropdown.value];
                string category = optionData.text;
                CurrentUserReport.Dimensions.Add(new UserReportNamedValue("Category", category));
                CurrentUserReport.Fields.Add(new UserReportNamedValue("Category", category));
            }

            // Set Description
            // This is how you would add additional fields.
            if (DescriptionInput != null)
            {
                UserReportNamedValue userReportField = new UserReportNamedValue();
                userReportField.Name = "Description";
                userReportField.Value = DescriptionInput.text;
                CurrentUserReport.Fields.Add(userReportField);
            }

            ClearForm();

            RaiseUserReportSubmitting();

            // Send Report
            UnityUserReporting.CurrentClient.SendUserReport(CurrentUserReport, (uploadProgress, downloadProgress) =>
            {
                if (ProgressText != null)
                {
                    string progressText = string.Format("{0:P}", uploadProgress);
                    ProgressText.text = progressText;
                }
            }, (success, br2) =>
            {
                if (!success)
                {
                    isShowingError = true;
                    StartCoroutine(ClearError());
                }

                CurrentUserReport = null;
                isSubmitting = false;
            });
        }

        public void CancelUserReport()
        {
            CurrentUserReport = null;
            ClearForm();
        }

        private IEnumerator ClearError()
        {
            yield return new WaitForSeconds(10);
            isShowingError = false;
        }

        private void ClearForm()
        {
            SummaryInput.text = null;
            DescriptionInput.text = null;
        }

        private UserReportingClientConfiguration GetConfiguration()
        {
            return new UserReportingClientConfiguration();
        }

        private void SetThumbnail(UserReport userReport)
        {
            if (userReport != null && ThumbnailViewer != null)
            {
                byte[] data = Convert.FromBase64String(userReport.Thumbnail.DataBase64);
                Texture2D texture = new Texture2D(1, 1);
                texture.LoadImage(data);
                ThumbnailViewer.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5F, 0.5F));
                ThumbnailViewer.preserveAspect = true;
            }
        }

        /// <summary>Occurs when a user report is submitting.</summary>
        protected virtual void RaiseUserReportSubmitting()
        {
            if (UserReportSubmitting != null)
            {
                UserReportSubmitting.Invoke();
            }
        }
    }
}