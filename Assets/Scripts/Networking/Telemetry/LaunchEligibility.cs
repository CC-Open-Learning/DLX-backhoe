/*
 *  LaunchEligibility authorizes the launch of CORE Engine using 
 *  Unity.RemoteConfig services and the ConfigManager
 */

#region Resources

using System;
using Unity.RemoteConfig;
using UnityEngine;

#endregion

namespace RemoteEducation.Networking.Telemetry
{

    public struct UserAttributes
    {
        //any attributes you might want to use for segmentation, empty if nothing
    }
    public struct AppAttributes
    {
        //any attributes you might want to use for segmentation, empty if nothing
    }

    /// <summary>
    ///     Authorizes the launch of CORE Engine using Unity.RemoteConfig services and the <see cref="ConfigManager"/>.
    /// </summary>
    /// 
    /// <remarks>
    ///     With the added functionality of being able to load a Scenario directly on Engine launch, 
    ///     the initial invokation of LaunchEligibility.FetchEnvironmentSettings is now the 
    ///     responsibility of an external class. 
    ///     <para />
    ///     Currently it is invoked by <see cref="RemoteEducation.Scenarios.ContentModuleQuickAccess"/>
    /// </remarks>
    /// 
    public class LaunchEligibility : MonoBehaviour
    {
        /// <summary>
        ///     Indicates connection status with the Remote Config services for the purposes of 
        ///     authorizing the CORE Engine application to launch.
        /// </summary>
        public enum Status
        {
            Offline,
            Connecting,
            Connected,
        }

        /// <summary>
        ///     Key used by CORE Engine's Remote Config instance to specify the 
        ///     application's eligibility to launch</summary>
        private readonly string EligibilityConfigKey = "enabled";


        #region Properties

        [SerializeField]
        [Tooltip("The window to be shown to the user when CORE Engine is disabled")]
        private GameObject applicationDisabledSplashScreen;

        /// <summary>
        ///     Invoked on completion of the Launch Eligibility verification process. 
        ///     When invoked, the LaunchEligibility.ConnectionStatus property has a meaningful value.
        /// </summary>
        public Action<Status> VerificationCompleteAction;

        /// <summary>Status of the Launch Eligibility verification process</summary>
        public Status ConnectionStatus { get; private set; }

        #endregion


        #region MonoBehaviour Callbacks

        /// <summary>
        ///     Configure the LaunchEligibility controller
        /// </summary>
        private void Awake()
        {

            // Hide any splash screen on start-up
            if (applicationDisabledSplashScreen)
            {
                applicationDisabledSplashScreen.SetActive(false);
            }

            // Set connections status to in-progress
            ConnectionStatus = Status.Connecting;
        }

        #endregion


        #region Methods


        /// <summary>
        ///     Assigns the VerifyEnviromentSettings method as a listener
        ///     to the ConfigManager.FetchCompleted delegate, then initiates
        ///     the connection with the Remote Config web service to retrieve
        ///     the environment configuration.
        /// </summary>
        /// <remarks>
        ///     This environment configuration must have a boolean-type key 
        ///     with the same name as the value stored in the
        ///     EligibilityConfigKey field, where the value of this key determines
        ///     whether or not the CORE Engine should be allowed to launch
        /// </remarks>
        public void FetchEnvironmentSettings()
        {
#if UNITY_EDITOR

            // Bypass LaunchEligibility check while running in Unity Editor
            ConnectionStatus = Status.Connected;
            Debug.Log("LaunchEligibility : Bypassing Remote Config check in Editor mode");

            // Invoke and clear any delegate(s) assigned to VerificationComplete action
            VerificationCompleteAction?.Invoke(ConnectionStatus);
            VerificationCompleteAction = null;
            return;
#endif

#pragma warning disable CS0162 // Suppressing "Unreachable code detected" warning due to UNITY_EDITOR compiler flag
            ConnectionStatus = Status.Connecting;
            Debug.Log("LaunchEligibility : Retrieving Remote Config settings");

            ConfigManager.FetchCompleted += VerifyEnvironmentSettings;
            ConfigManager.FetchConfigs<UserAttributes, AppAttributes>(new UserAttributes(), new AppAttributes());
#pragma warning restore CS0162

        }


        /// <summary>
        ///     Assigns the ConnectionStatus property the value <c>Connected</c> only if:
        ///     <list type="bullet">
        ///         <item>A successful connection has been made to the Unity Remote Config web service</item>
        ///         <item>The configuration settings have been retrieved remotely, not from a cached copy</item>
        ///         <item>The Remote Config environment settings contain an ElibilityConfigKey field</item>
        ///         <item>The EligibilityConfigKey is set to <c>true</c></item>
        ///     </list>
        ///  </summary>
        /// <param name="response">Struct containing status information for the Remote Config request and response</param>
        private void VerifyEnvironmentSettings(ConfigResponse response)
        {
            /* 
             * This verification block ensures that a successful connection has 
             * been made to the Unity Remote Config web service, the configuration settings
             * have been retrieved remotely (not from a cached copy), and that the
             * Remote Config environment settings contain the ElibilityConfigKey field and
             * it is set to true
             * 
             * If any of these conditions are not met, CORE Engine will refuse to launch and will
             * present a splash screen with an informative message
             */
            if (response.status == ConfigRequestStatus.Success
                && response.requestOrigin == ConfigOrigin.Remote
                && ConfigManager.appConfig.HasKey(EligibilityConfigKey)
                && ConfigManager.appConfig.GetBool(EligibilityConfigKey) )
            {
                ConnectionStatus = Status.Connected;
                Debug.Log("LaunchEligibility : Access to CORE Engine v" + Application.version + " is enabled");
            }
            else
            {
                ConnectionStatus = Status.Offline;
                Debug.Log("LaunchEligibility : Access to CORE Engine v" + Application.version + " is disabled");
                applicationDisabledSplashScreen.SetActive(true);
            }

            // Invoke and clear any delegate(s) assigned to VerificationComplete action
            VerificationCompleteAction?.Invoke(ConnectionStatus);
            VerificationCompleteAction = null;

            // Sends the EngineLaunch event after assessing launch eligibility
            // TODO provide a field in the EngineLaunch event which indicates if CORE Engine
            // was launched successfully
            //AnalyticsCapture.SendEngineLaunchEvent(); killed for uplift
            ConfigManager.FetchCompleted -= VerifyEnvironmentSettings;
        }

        #endregion
    }
}
