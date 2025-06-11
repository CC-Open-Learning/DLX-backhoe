using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace RemoteEducation.Audio
{
    /// <summary>Provides control of the engine's main <see cref="UnityEngine.Audio.AudioMixer"/> channels.</summary>
    public partial class AudioManager : MonoBehaviour
    {
        private static AudioManager singleton;

        private AsyncOperationHandle<AudioMixer> mixerHandle;
        private AudioMixer mixer;

        public bool Initialized { get; private set; }

        private void Awake()
        {
            if (singleton != null)
            {
                Debug.LogWarning("<color=#0099ff>AudioManager</color> : Singleton already exists! Deleting replacement.");
                Destroy(gameObject);
                return;
            }

            singleton = this;

            mixerHandle = Addressables.LoadAssetAsync<AudioMixer>("CORE Audio Mixer");
        }

        private IEnumerator Start()
        {
            yield return mixerHandle;

            mixer = mixerHandle.Result;

            MuteChannel(Channel.Name.Master, !PreferenceKeys.AudioEnabled);

            // Set Engine and Simulation volume levels from PlayerPrefs
            // PlayerPrefs still need to be saved after being edited by user
            SetVolume(Channel.Name.Engine, PreferenceKeys.GetPreferenceSafe(PreferenceKeys.EngineAudioVolumeKey, 1f));
            SetVolume(Channel.Name.Simulation, PreferenceKeys.GetPreferenceSafe(PreferenceKeys.SimulationAudioVolumeKey, 1f));

            CreatePooledResources();

            Initialized = true;
        }

        /// <summary>Sets the volume of the <see cref="UnityEngine.Audio.AudioMixerGroup"/> for the given <see cref="Channel.Name"/>.</summary>
        /// <remarks>The <paramref name="volume"/> parameter is a normalized linear value. Pass in 0 for silent, 1 for max or anything in between.</remarks>
        public static void SetVolume(Channel.Name channel, float volume)
        {
            volume = Mathf.Clamp(volume, OFF_VOLUME_LINEAR, 1f);

            channels[(int)channel].dBVolume = LinearVolumeToDB(volume);

            // If we are muted we still store the value of the volume change for when we are unmuted.
            if (channels[(int)channel].muted || singleton == null || singleton.mixer == null)
                return;

            singleton.mixer.SetFloat(channels[(int)channel].exposedVolumeParameter, channels[(int)channel].dBVolume);
        }

        /// <summary>Gets the volume of the <see cref="UnityEngine.Audio.AudioMixerGroup"/> for the given <see cref="Channel.Name"/>.</summary>
        /// <returns>Volume as a normalized linear value. 0 is silent, 1 is max volume.</returns>
        public static float GetVolume(Channel.Name channel)
        {
            return DBVolumeToLinear(channels[(int)channel].dBVolume);
        }

        /// <summary>Mutes or Unmutes a specified channel with an optional fade.</summary>
        public static void MuteChannel(Channel.Name channel, bool mute, float fadeTime = MUTE_FADE_TIME)
        {
            if (channels[(int)channel].muted && mute)
            {
                Debug.LogWarning("<color=#0099ff>AudioManager</color> : Mute called on muted channel!");
                return;
            }

            if (!channels[(int)channel].muted && !mute)
            {
                Debug.LogWarning("<color=#0099ff>AudioManager</color> : Unmute called on unmuted channel!");
                return;
            }

            channels[(int)channel].muted = mute;

            if (channels[(int)channel].fading || singleton == null || singleton.mixer == null)
                return;

            if (fadeTime > 0f)
                singleton.StartCoroutine(singleton.FadeChannel(channel, fadeTime));
            else
                singleton.mixer.SetFloat(channels[(int)channel].exposedVolumeParameter, mute ? OFF_VOLUME_DB : channels[(int)channel].dBVolume);
        }

        /// <summary>Toggles mute on a channel.</summary>
        /// <remarks>If the channel is muted, it will unmute and vice-versa.</remarks>
        public static void ToggleMuteChannel(Channel.Name channel, float fadeTime = MUTE_FADE_TIME)
        {
            MuteChannel(channel, !channels[(int)channel].muted, fadeTime);
        }

        /// <summary>Allows a UI element to play an <see cref="AudioClip"/> through the <see cref="AudioManager.singleton"/> instance.</summary>
        /// <param name="clip">The <see cref="AudioClip"/> to be played</param>
        /// <param name="volume">Volume at which to play the clip. Default value is 1, or 100%</param>
        public static void PlayStatic(AudioClip clip, float volume = 1f)
        {
            if (!singleton)
            {
                Debug.LogError("<color=#0099ff>AudioManager</color> : AudioManager singleton has not been instantiated.");
                return;
            }

            AudioSource source = GetEngineAudioSource();

            source.clip = clip;
            source.volume = volume;
            source.Play();
        }

        /// <summary>Fades the volume of the <see cref="UnityEngine.Audio.AudioMixerGroup"/> for the given <see cref="Channel.Name"/> in or out depending on the state of <see cref="Channel.muted"/>.</summary>
        private IEnumerator FadeChannel(Channel.Name channel, float fadeTime)
        {
            int index = (int)channel;
            Channel c = channels[index];
            string parameter = c.exposedVolumeParameter;
            float currentVolume = DBVolumeToLinear(c.dBVolume);

            channels[index].fading = true;

            float t = channels[index].muted ? currentVolume : OFF_VOLUME_LINEAR;

            while (channels[index].muted ? (t > 0) : (t < currentVolume))
            {
                singleton.mixer.SetFloat(parameter, LinearVolumeToDB(t));

                if (channels[index].muted)
                    t -= Time.unscaledDeltaTime / fadeTime;
                else
                    t += Time.unscaledDeltaTime / fadeTime;

                yield return null;
            }

            if (channels[index].muted)
                mixer.SetFloat(parameter, OFF_VOLUME_DB);
            else
                mixer.SetFloat(parameter, c.dBVolume);

            channels[index].fading = false;
        }

        public float GetVolumeFromMixer(Channel.Name channel)
        {
            if(mixer.GetFloat(channels[(int)channel].exposedVolumeParameter, out float value))
                return DBVolumeToLinear(value);

            Debug.LogError("<color=#0099ff>AudioManager</color> : Mixer cannot find exposedVolumeParameter " + channels[(int)channel].exposedVolumeParameter + "!");
            return 0f;
        }

        private static float LinearVolumeToDB(float volume)
        {
            return Mathf.Log10(volume) * 20f;
        }

        private static float DBVolumeToLinear(float dBvolume)
        {
            return Mathf.Pow(10f, dBvolume / 20f);
        }

        public static void DeleteSingleton()
        {
            if (singleton != null)
            {
                Destroy(singleton.gameObject);
                singleton = null;
            }
        }
    }
}
