using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

namespace RemoteEducation.UserReporting
{
    partial class UserReportingScript
    {
        // Unity package had these all serialized with public.
        // Changing them to lower case first letter naming convention for private would just be a pain.

        [Tooltip("The category dropdown.")]
        [SerializeField] private TMP_Dropdown CategoryDropdown;

        [Tooltip("The description input on the user report form.")]
        [SerializeField] private TMP_InputField DescriptionInput;

        [Tooltip("The summary input on the user report form.")]
        [SerializeField] private TMP_InputField SummaryInput;

        [Tooltip("The display text for the progress text.")]
        [SerializeField] private TextMeshProUGUI ProgressText;

        [Tooltip("The thumbnail viewer on the user report form.")]
        [SerializeField] private Image ThumbnailViewer;

        [Tooltip("The UI shown when there's an error.")]
        [SerializeField] private Canvas ErrorPopup;

        [Tooltip("The UI shown while submitting.")]
        [SerializeField] private Canvas SubmittingPopup;

        [Tooltip("The UI for the user report form. Shown after a user report is created.")]
        [SerializeField] private Canvas UserReportForm;

        [Tooltip("The User Reporting platform. Different platforms have different features but may require certain Unity versions or target platforms. The Async platform adds async screenshotting and report creation, but requires Unity 2018.3 and above, the package manager version of Unity User Reporting, and a target platform that supports asynchronous GPU readback such as DirectX.")]
        [SerializeField] private UserReportingPlatformType UserReportingPlatform;

        [Tooltip("A value indicating whether the hotkey is enabled (Left Alt + Left Shift + B).")]
        [SerializeField] private bool IsHotkeyEnabled;

        [Tooltip("A value indicating whether the prefab is in silent mode. Silent mode does not show the user report form.")]
        [SerializeField] private bool IsInSilentMode;

        [Tooltip("A value indicating whether the user report client reports metrics about itself.")]
        [SerializeField] private bool IsSelfReporting;

        [Tooltip("A value indicating whether the user report client send events to analytics.")]
        [SerializeField] private bool SendEventsToAnalytics;

        [Tooltip("The event raised when a user report is submitting.")]
        [SerializeField] private UnityEvent UserReportSubmitting;
    }
}