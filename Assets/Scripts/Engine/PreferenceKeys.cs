using UnityEngine;
using RemoteEducation.Audio;
using System.Collections.Generic;

namespace RemoteEducation
{
    /// <summary>
    ///     Defines a single point of access for all keys used
    ///     by the UnityEngine <see cref="PlayerPrefs"/> class
    /// </summary>
    public static class PreferenceKeys
    {
        public delegate void OnPreferenceChangedEvent(string key, object value);

        public static event OnPreferenceChangedEvent OnPreferenceChanged;

        // User settings preference keys
        public const string AudioPreferenceKey = "mute";
        public const string EngineAudioVolumeKey = "EngineAudio";
        public const string SimulationAudioVolumeKey = "SimulationAudio";
        public const string TutorialPreferenceKey = "tutorial";
        public const string DevModePreferenceKey = "devmode";
        public const string PlaybackPreferenceKey = "playback";

        // Legacy preference keys
        public const string HostnameKey = "HostnameKey";
        public const string SMTPPortKey = "SMTPPortKey";
        public const string SenderKey = "SenderKey";
        public const string SendGridSenderKey = "SendGridSenderKey";
        public const string PasswordKey = "PasswordKey";
        public const string SendGridPasswordKey = "SendGridPasswordKey";
        public const string SendGridAccountKey = "SendGridAccountKey";
        public const string CCEmailKey = "CCEmailKey";
        public const string SendGridFlagKey = "SendGridFlagKey";
        public const string AddressKey = "AddressKey";
        public const string PortKey = "PortKey";
        public const string UserNameKey = "UserNameKey";

        public enum Key
        {
            MuteAudio = 0,
            EngineAudioVolume = 1,
            SimulationAudioVolume = 2,
            Tutorial = 3,
            DeveloperMode = 4,
            Playback = 5 
        }

        // Added this Dictionary so that we can refer to keys by an enum.
        // This allows the keys to be exposed to the Unity inspector which then reference the strings here.
        // Prevents someone from writing in a PlayerPref key in UIPlayerPrefWrapper in the inspector and having it change later.
        private static Dictionary<Key, string> preferenceNames = new Dictionary<Key, string>();

        static PreferenceKeys()
        {
            preferenceNames.Add(Key.MuteAudio, AudioPreferenceKey);
            preferenceNames.Add(Key.EngineAudioVolume, EngineAudioVolumeKey);
            preferenceNames.Add(Key.SimulationAudioVolume, SimulationAudioVolumeKey);
            preferenceNames.Add(Key.Tutorial, TutorialPreferenceKey);
            preferenceNames.Add(Key.DeveloperMode, DevModePreferenceKey);
            preferenceNames.Add(Key.Playback, PlaybackPreferenceKey);
        }

        public static string GetPreferenceName(Key key)
        {
            string s;

            if (preferenceNames.TryGetValue(key, out s))
                return s;

            Debug.LogWarning("Preference Key " + key.ToString() + " was not added to the dictionary in the static constructor.");
            return null;
        }

        /// <summary>Access and modify the state of CORE Engine audio</summary>
        public static bool AudioEnabled
        {
            get { return PlayerPrefs.GetInt(AudioPreferenceKey) == 0; }
            set
            {
                PlayerPrefs.SetInt(AudioPreferenceKey, value ? 0 : 1);
                Audio.AudioManager.MuteChannel(Audio.AudioManager.Channel.Name.Master, !value);
            }
        }

        /// <summary>Access and modify the presence of the intro tutorial</summary>
        public static bool TutorialEnabled
        {
            get { return PlayerPrefs.GetInt(TutorialPreferenceKey) == 1; }
            set { PlayerPrefs.SetInt(TutorialPreferenceKey, value ? 1 : 0); }
        }

        /// <summary>Access and modify the state of developer mode in CORE Engine</summary>
        public static bool DeveloperModeEnabled
        {
            get { return PlayerPrefs.GetInt(DevModePreferenceKey) == 1; }
            set 
            { 
                PlayerPrefs.SetInt(DevModePreferenceKey, value ? 1 : 0);  
                if(OnPreferenceChanged != null)
                {
                    OnPreferenceChanged(DevModePreferenceKey, value);
                }
            }
        }

        /// <summary>Access and modify the state of event playback controls in CORE Engine</summary>
        public static bool PlaybackControlsEnabled
        {
            get { return PlayerPrefs.GetInt(PlaybackPreferenceKey) == 1; }
            set { PlayerPrefs.SetInt(PlaybackPreferenceKey, value ? 1 : 0); }
        }

        /// <summary>Gets a value from <see cref="PlayerPrefs"/>, defaulting to <paramref name="failValue"/> if not found.</summary>
        public static float GetPreferenceSafe(string pref, float failValue)
        {
            if (PlayerPrefs.HasKey(pref))
                return PlayerPrefs.GetFloat(pref);

            return failValue;
        }

        /// <summary>Gets a value from <see cref="PlayerPrefs"/>, defaulting to <paramref name="failValue"/> if not found.</summary>
        public static string GetPreferenceSafe(string pref, string failValue)
        {
            if (PlayerPrefs.HasKey(pref))
                return PlayerPrefs.GetString(pref);

            return failValue;
        }

        /// <summary>Gets a value from <see cref="PlayerPrefs"/>, defaulting to <paramref name="failValue"/> if not found.</summary>
        public static int GetPreferenceSafe(string pref, int failValue)
        {
            if (PlayerPrefs.HasKey(pref))
                return PlayerPrefs.GetInt(pref);

            return failValue;
        }
    }
}
