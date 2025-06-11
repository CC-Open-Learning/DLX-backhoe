using UnityEngine;

namespace RemoteEducation.Audio
{
    /// <summary>
    ///     Provides UI elements an access point to the static <see cref="AudioManager"/>
    /// </summary>
    public class UISoundEffect : MonoBehaviour
    {
        [Tooltip("Volume at which the UI AudioClips should be played")]
        [SerializeField] [Range(0, 1)] private float volume = 1f;

        /// <summary>
        ///     Array of AudioClips which can be referenced using <see cref="PlayIndex(int)"/>
        /// </summary>
        [SerializeField] private AudioClip[] clips;


        /// <summary>
        ///     Plays the first <see cref="AudioClip"/> in the <see cref="clips"/> array
        /// </summary>
        public void Play()
        {
            PlayIndex(0);
        }

        /// <summary>
        ///     Play the <see cref="AudioClip"/> at the specified 
        ///     <paramref name="index"/> in the <see cref="clips"/> array
        /// </summary>
        /// <param name="index">
        ///     The specified non-negative index of the desired <see cref="AudioClip"/> 
        ///     which must be be less than the size of the <see cref="clips"/> array
        /// </param>
        public void PlayIndex(int index)
        {
            if (clips != null && clips.Length > index)
            {
                AudioManager.PlayStatic(clips[index], volume);
            }
        }
    }
}
