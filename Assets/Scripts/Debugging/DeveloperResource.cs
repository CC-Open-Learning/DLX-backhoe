using UnityEngine;

namespace RemoteEducation.Debugging
{
    /// <summary>
    ///     Attaching this MonoBehaviour to a GameObject indicates that
    ///     the GameObject should only be active when the "developer mode"
    ///     <see cref="PlayerPrefs" /> is on.
    /// </summary>
    public class DeveloperResource : MonoBehaviour
    {
        private void OnEnable()
        {
            // Disable the GameObject if "devmode" is not turned on
            if (!PreferenceKeys.DeveloperModeEnabled)
            {
                gameObject.SetActive(false);
            }
        }
    }
}