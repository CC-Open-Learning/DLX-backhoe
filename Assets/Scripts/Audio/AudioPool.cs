/*
 *  AudioPool.cs
 *  
 *  This file extends the functionality of the AudioManager to support resource pooling
 *  for UI-layer sound effects. With a default (arbitrary) buffer size of 4, calls to 
 */


using UnityEngine;
using UnityEngine.Audio;

namespace RemoteEducation.Audio
{
    public partial class AudioManager : MonoBehaviour
    {
        [Tooltip("Defines the number of UI-layer sound effects that can be played simultaneously")]
        [Range(1, 10)]
        [SerializeField] private int engineAudioPoolSize = 4;

        /// <summary>Circular buffer which holds all <see cref="AudioSource"/> instantiated for UI sound effect use</summary>
        private static AudioSource[] pool;

        /// <summary>Tracks the next available <see cref="AudioSource"/> in the circular buffer</summary>
        private static int poolIndex = 0;

        /// <summary>Creates a child <see cref="GameObject"/> for the <see cref="AudioManager"/> to hold the multiple <see cref="AudioSource"/> components used for <see cref="AudioManager.pool"/>.</summary>
        private void CreatePooledResources()
        {
            pool = new AudioSource[engineAudioPoolSize];

            GameObject poolObject = new GameObject("UI Audio Pool");
            poolObject.transform.SetParent(transform);

            for (int i = 0; i < engineAudioPoolSize; i++)
            {
                pool[i] = AddEngineAudioSource(poolObject);
            }
        }

        /// <summary>Adds an <see cref="AudioSource"/> component to the provided <paramref name="poolObject"/>, setting default values and applying the 'Engine' <see cref="AudioMixerGroup"/>.</summary>
        /// <param name="poolObject"><see cref="GameObject"/> to apply the <see cref="AudioSource"/> component to.</param>
        /// <returns>The newly created <see cref="AudioSource"/> component.</returns>
        private static AudioSource AddEngineAudioSource(GameObject poolObject)
        {
            AudioSource source = poolObject.AddComponent<AudioSource>();

            // Engine AudioSource settings
            source.playOnAwake = false;
            source.loop = false;
            source.outputAudioMixerGroup = GetMixerGroup(Channel.Name.Engine);
            source.spatialBlend = 0f;

            return source;
        }

        /// <summary>Advances the circular buffer and returns an available <see cref="AudioSource"/> forplaying UI-layer sound effects</summary>
        /// <returns>An available <see cref="AudioSource"/> for playing a UI-layer sound effect</returns>
        private static AudioSource GetEngineAudioSource()
        {
            return pool[poolIndex = (poolIndex + 1) % singleton.engineAudioPoolSize];
        }
    }
}