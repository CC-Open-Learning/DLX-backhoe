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

namespace RemoteEducation.UserReporting
{
    partial class UserReportingScript
    {
        private void Awake()
        {
            if (singleton != null)
            {
                Debug.LogWarning("UserReportingScript: Singleton already exists! Deleting this duplicate.");
                Destroy(transform.root.gameObject);
                return;
            }

            singleton = this;
        }

        private void Start()
        {
            // Set Up Event System
            if (Application.isPlaying)
            {
                EventSystem sceneEventSystem = UnityEngine.Object.FindObjectOfType<EventSystem>();
                if (sceneEventSystem == null)
                {
                    GameObject eventSystem = new GameObject("EventSystem");
                    eventSystem.AddComponent<EventSystem>();
                    eventSystem.AddComponent<StandaloneInputModule>();
                }
            }

            // Configure Client
            bool configured = false;
            if (UserReportingPlatform == UserReportingPlatformType.Async)
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                Type asyncUnityUserReportingPlatformType = assembly.GetType("Unity.Cloud.UserReporting.Plugin.Version2018_3.AsyncUnityUserReportingPlatform");
                if (asyncUnityUserReportingPlatformType != null)
                {
                    object activatedObject = Activator.CreateInstance(asyncUnityUserReportingPlatformType);
                    IUserReportingPlatform asyncUnityUserReportingPlatform = activatedObject as IUserReportingPlatform;
                    if (asyncUnityUserReportingPlatform != null)
                    {
                        UnityUserReporting.Configure(asyncUnityUserReportingPlatform, GetConfiguration());
                        configured = true;
                    }
                }
            }

            if (!configured)
            {
                UnityUserReporting.Configure(GetConfiguration());
            }

            // Ping
            string url = string.Format("https://userreporting.cloud.unity3d.com/api/userreporting/projects/{0}/ping", UnityUserReporting.CurrentClient.ProjectIdentifier);
            UnityUserReporting.CurrentClient.Platform.Post(url, "application/json", Encoding.UTF8.GetBytes("\"Ping\""), (upload, download) => { }, (result, bytes) => { });
        }

        private void Update()
        {
            // Hotkey Support
            if (IsHotkeyEnabled)
            {
                if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftAlt))
                {
                    if (Input.GetKeyDown(KeyCode.B))
                    {
                        CreateUserReport();
                    }
                }
            }

            // Update Client
            UnityUserReporting.CurrentClient.IsSelfReporting = IsSelfReporting;
            UnityUserReporting.CurrentClient.SendEventsToAnalytics = SendEventsToAnalytics;

            if (UserReportForm != null)
            {
                UserReportForm.enabled = State == UserReportingState.ShowingForm;
            }

            if (SubmittingPopup != null)
            {
                SubmittingPopup.enabled = State == UserReportingState.SubmittingForm;
            }

            if (ErrorPopup != null)
            {
                ErrorPopup.enabled = isShowingError;
            }

            // Update Client
            // The UnityUserReportingUpdater updates the client at multiple points during the current frame.
            unityUserReportingUpdater.Reset();
            StartCoroutine(unityUserReportingUpdater);
        }
    }
}