/*
 *  FILE        : UserSettings.cs
 *  DESCRIPTION :
 *      UserSettings is 
 */

using UnityEngine;
using Lean.Gui;

namespace RemoteEducation.UI
{
    /// <summary>
    ///     Provides front-end access to the CORE Engine <see cref="PreferenceKeys"/>
    /// </summary>
    public class UserSettings : MonoBehaviour
    {
        // "Toggle" elements to present and toggle the settings
        [SerializeField] LeanToggle audioSwitch;
        [SerializeField] LeanToggle tutorialSwitch;
        [SerializeField] LeanToggle developerModeSwitch;
        [SerializeField] LeanToggle playbackControlsSwitch;

        /// <summary>
        ///     Set event hooks for each of the Preference toggles
        /// </summary>
        void Start()
        {
            // Audio switch
            audioSwitch.OnOn.AddListener(() => PreferenceKeys.AudioEnabled = true);
            audioSwitch.OnOff.AddListener(() => PreferenceKeys.AudioEnabled = false);

            // Tutoria switch
            tutorialSwitch.OnOn.AddListener(() => PreferenceKeys.TutorialEnabled = true);
            tutorialSwitch.OnOff.AddListener(() => PreferenceKeys.TutorialEnabled = false);

            // Developer mode
            developerModeSwitch.OnOn.AddListener(() => PreferenceKeys.DeveloperModeEnabled = true);
            developerModeSwitch.OnOff.AddListener(() => PreferenceKeys.DeveloperModeEnabled = false);

            // Playback controls
            playbackControlsSwitch.OnOn.AddListener(() => PreferenceKeys.PlaybackControlsEnabled = true);
            playbackControlsSwitch.OnOff.AddListener(() => PreferenceKeys.PlaybackControlsEnabled = false);
        }

        /// <summary>
        ///     Update the state of each UserSettings toggle
        /// </summary>
        private void OnEnable()
        {
            audioSwitch.On = PreferenceKeys.AudioEnabled;
            tutorialSwitch.On = PreferenceKeys.TutorialEnabled;
            developerModeSwitch.On = PreferenceKeys.DeveloperModeEnabled;
            playbackControlsSwitch.On = PreferenceKeys.PlaybackControlsEnabled;
        }
    }
}