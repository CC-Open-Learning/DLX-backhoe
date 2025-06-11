using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Audio;

namespace RemoteEducation.Audio
{
    /// <summary>Provides control of the engine's main <see cref="UnityEngine.Audio.AudioMixer"/> channels.</summary>
    public partial class AudioManager : MonoBehaviour
    {
        private const float MUTE_FADE_TIME = .5f;
        private const float DEFAULT_VOLUME_DB = 0f; // The default volume of all channels in Decibels.
        private const float OFF_VOLUME_DB = -80f;   // The volume in Decibels that the AudioMixer is set to shut off at.
        public static readonly float OFF_VOLUME_LINEAR = DBVolumeToLinear(OFF_VOLUME_DB); // Generates linear minimum volume from the decibel value.

        public struct Channel
        {
            public enum Name
            {
                Master = 0,
                Engine = 1,
                Simulation = 2,
                SimulationForeground = 3,
                SimulationBackground = 4
            }

            public string exposedVolumeParameter;
            public string mixerName;
            public float dBVolume;
            public bool muted, fading;

            public Channel(string mixerName, string exposedVolumeParameter)
            {
                this.mixerName = mixerName;
                this.exposedVolumeParameter = exposedVolumeParameter;
                dBVolume = DEFAULT_VOLUME_DB; // Could be replaced by some PlayerPrefs value where we save the user's previous desired volume for each channel based on some sliders.
                muted = false;
                fading = false;
            }
        }

        // We can't read these programmatically, they have to be hardcoded like this if we want the enum to work as intended.
        private static readonly Channel[] channels = { 
            new Channel("Master", "MasterAudioVolume"),
            new Channel("Engine", "EngineAudioVolume"),
            new Channel("Simulation", "DLSGlobalVolume"),
            new Channel("Foreground", "DLSForegroundVolume"),
            new Channel("Background", "DLSBackgroundVolume") };

        /// <summary>Returns the <see cref="UnityEngine.Audio.AudioMixerGroup"/> for the given <see cref="Channel.Name"/>.</summary>
        public static AudioMixerGroup GetMixerGroup(Channel.Name channel)
        {
            return singleton.mixer.FindMatchingGroups(channels[(int)channel].mixerName)[0];
        }
    }
}
