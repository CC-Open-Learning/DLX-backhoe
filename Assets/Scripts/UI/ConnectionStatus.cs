#region Resources

using TMPro;
using UnityEngine;
using RemoteEducation.Networking.Telemetry;

#endregion

namespace RemoteEducation.UI
{
    /// <summary>
    ///     Displays the status of the <see cref="LaunchEligibility"/> launch process in a <see cref="TextMeshProUGUI"/> text object
    /// </summary>
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class ConnectionStatus : MonoBehaviour
    {
        ///<summary>The UI text GameObject which will be updated with the telemetry connection status</summary>
        private TextMeshProUGUI connectionText;

        private readonly string StatusConnecting = "Connecting . . .";
        private readonly string StatusConnected = "Connection established";
        private readonly string StatusDisconnected = "Offline";

        void Awake()
        {
            connectionText = GetComponent<TextMeshProUGUI>();
            LaunchEligibility launcher = FindObjectOfType<LaunchEligibility>();
            launcher.VerificationCompleteAction += SetConnectionStatusString;
            SetConnectionStatusString(launcher.ConnectionStatus);
        }


        /// <summary>
        ///     Wraps the 'connectionText' TextMeshProUGUI field within a setter, and sets
        ///     the value of the text based on the status of <see cref="LaunchEligibility"/>
        /// </summary>
        /// <param name="status"></param>
        private void SetConnectionStatusString(LaunchEligibility.Status status)
        {
            if (!connectionText)
            {
                return;
            }

            switch (status)
            {
                case LaunchEligibility.Status.Connected:
                    connectionText.text = StatusConnected;
                    break;
                case LaunchEligibility.Status.Offline:
                    connectionText.text = StatusDisconnected;
                    break;
                case LaunchEligibility.Status.Connecting:
                    connectionText.text = StatusConnecting;
                    break;

            }
        }
    }
}
