using System.Collections;
using UnityEngine;
using RemoteEducation.Extensions;

namespace RemoteEducation.Audio
{
    /// <summary>Extends <see cref="UnityEngine.AudioSource"/> to provide more functionality.</summary>
    [RequireComponent(typeof(AudioSource))]
    public class SoundEffect : MonoBehaviour
    {
        public delegate void OnPlayEvent(SoundEffect soundEffect);
        /// <summary>Event that fires whenever a <see cref="SoundEffect"/> is played.</summary>
        public static event OnPlayEvent OnPlay;

        [Tooltip("Select to play Audio when this object is enabled.\nOverrides AudioSource.playOnAwake.")]
        [SerializeField] private bool playOnAwake = true;
        [Tooltip("Disable this GameObject after playing.")]
        [SerializeField] public bool oneShot = false;
        [Tooltip("Delay after OnEnable is called before playing when Play On Awake is selected.")]
        [SerializeField] private float startDelay = 0;
        [Tooltip("The % the volume will randomly be skewed before playing.")]
        [SerializeField] private float randomVolumePercent = 0.05f;
        [Tooltip("The % the pitch will randomly be skewed before playing.")]
        [SerializeField] private float randomPitchPercent = 0.05f;
        [Tooltip("Array of AudioClips the component may choose to play.")]
        [SerializeField] private AudioClip[] extraAudioClips;

        /// <summary>The pitch of <see cref="SoundEffect.audioSource"/> from [-3.0,3.0].</summary>
        public float Pitch
        {
            get { return audioSource.pitch; }
            set { audioSource.pitch = value; }
        }

        /// <summary>The volume of <see cref="SoundEffect.audioSource"/> from [0.0,1.0].</summary>
        public float Volume
        {
            get { return audioSource.volume; }
            set { audioSource.volume = value; }
        }

        private AudioClip audioClip;
        private float startPitch, startVolume;
        private int clipCount;
        private bool initialized = false;
        public AudioSource audioSource;

        void Awake()
        {
            if (!initialized)
                Init();
        }

        /// <summary>Initializes this <see cref="SoundEffect"/> and its <see cref="AudioSource"/>.</summary>
        void Init()
        {
            audioSource = GetComponent<AudioSource>();

            // We have our own playOnAwake to allow for OneShot, Loops, and Delayed start sound effects when the component is enabled.
            // Disabling the original AudioSource.playOnAwake is crucial to make sure that system works.
            audioSource.playOnAwake = false;

            audioClip = audioSource.clip; // Store this since we will be swapping audioSource.clip out constantly, this clip would be lost otherwise.

            startPitch = audioSource.pitch;
            startVolume = audioSource.volume;

            clipCount = extraAudioClips.Length;

            // If the original AudioSource has a clip on it, we add one to the number of clips counted from extraAudioClips.Length
            // Later this is used so that when choosing a random clip, if the number chosen is higher than extraAudioClips.Length, we just use the original clip.
            if (audioClip != null)
                clipCount++;

            initialized = true;
        }

        // This handles our custom playOnAwake logic.
        void OnEnable()
        {
            if (!initialized) // It should not be possible to arrive here without initializing but we'll check anyway.
                Init();

            if (playOnAwake)
            {
                if (oneShot)
                {
                    PlayOneShot();
                }
                else
                {
                    if (startDelay <= 0)
                        Play(audioSource.loop);
                    else
                        PlayDelayed(startDelay);
                }
            }
        }

        private void SetupClip(AudioClip clip, bool loop)
        {
            audioSource.pitch = startPitch.SkewRandomly(randomPitchPercent);
            audioSource.volume = startVolume.SkewRandomly(randomVolumePercent);
            audioSource.clip = clip;
            audioSource.loop = loop;
        }

        /// <summary>Gets a random <see cref="AudioClip"/> from the list of <see cref="SoundEffect.extraAudioClips"/> plus the <see cref="AudioClip"/> set in the AudioSource.</summary>
        /// <remarks>If no <see cref="AudioClip"/> exists on the original <see cref="AudioSource"/> component, the clip will be chosen solely from <see cref="SoundEffect.extraAudioClips"/> as a safety.</remarks>
        /// <returns>A random <see cref="AudioClip"/> from all <see cref="SoundEffect.extraAudioClips"/> or the original <see cref="AudioSource.clip"/>.</returns>
        private AudioClip GetAudioClip()
        {
            if (extraAudioClips == null)
                return audioClip;

            var index = UnityEngine.Random.Range(0, clipCount);

            if (index == extraAudioClips.Length)
                return audioClip;

            return extraAudioClips[index];
        }

        /// <summary>Chooses a random <see cref="AudioClip"/> from this object and plays it.</summary>
        public void Play(bool loop = false)
        {
            SetupClip(GetAudioClip(), loop);

            PlayAudioSource();
        }

        /// <summary>Chooses a random <see cref="AudioClip"/> from this object and plays it for a specified <paramref name="duration"/> before fading out.</summary>
        public void PlayFor(float duration)
        {
            SetupClip(GetAudioClip(), true);

            PlayAudioSource();

            StartCoroutine(_PlayFor(duration));
        }
        private IEnumerator _PlayFor(float duration)
        {
            yield return new WaitForSeconds(duration);

            FadeOut();
        }

        /// <summary>Chooses a random <see cref="AudioClip"/> from this object and plays it. Upon completion, disables this component's <see cref="GameObject"/></summary>
        public void PlayOneShot()
        {
            SetupClip(GetAudioClip(), false);

            PlayAudioSource();

            var delay = audioSource.clip.length;

            StartCoroutine(DisableTimer(delay));
        }
        private IEnumerator DisableTimer(float delay)
        {
            yield return new WaitForSeconds(delay);

            gameObject.SetActive(false);
        }

        /// <summary>Moves the <see cref="Transform"/> of this <see cref="SoundEffect"/> to the given <paramref name="pos"/> before choosing a random <see cref="AudioClip"/> from this object and playing it.</summary>
        public void PlayAt(Vector3 pos, bool loop = false)
        {
            transform.position = pos;

            SetupClip(GetAudioClip(), loop);

            PlayAudioSource();
        }

        /// <summary>Chooses a random <see cref="AudioClip"/> from this object and plays it after a set <paramref name="delay"/>.</summary>
        public void PlayDelayed(float delay, bool loop = false)
        {
            StartCoroutine(Delay(delay, loop));
        }
        private IEnumerator Delay(float delay, bool loop = false)
        {
            yield return new WaitForSeconds(delay);

            Play(loop);
        }

        /// <summary>Plays a specific index of <see cref="SoundEffect.extraAudioClips"/>.</summary>
        public void PlayIndex(int index, bool loop)
        {
            if (extraAudioClips.Length <= index)
                return;

            SetupClip(extraAudioClips[index], loop);

            PlayAudioSource();
        }

        /// <summary>Plays a specific index of <see cref="SoundEffect.extraAudioClips"/>.</summary>
        /// <remarks>This overload is primarily meant for using PlayIndex in Unity Events in the Inspector.</remarks>
        public void PlayIndex(int index)
        {
            if (startDelay > 0)
            {
                PlayIndexDelayed(index, startDelay, audioSource.loop);
                return;
            }

            PlayIndex(index, false);
        }

        /// <summary>Plays a specific index of <see cref="SoundEffect.extraAudioClips"/> after a set <paramref name="delay"/>.</summary>
        public void PlayIndexDelayed(int index, float delay, bool loop = false)
        {
            StartCoroutine(DelayIndex(index, delay, loop));
        }
        private IEnumerator DelayIndex(int index, float delay, bool loop = false)
        {
            yield return new WaitForSeconds(delay);

            PlayIndex(index, loop);
        }

        /// <summary>Fades <see cref="SoundEffect.audioSource"/> out over the set <paramref name="fadeTime"/>.</summary>
        public void FadeOut(float fadeTime = 0.35f)
        {
            StartCoroutine(_FadeOut(fadeTime));
        }
        private IEnumerator _FadeOut(float fadeTime)
        {
            for (float t = 0; t < 1; t += Time.deltaTime / fadeTime)
            {
                audioSource.volume = Mathf.Lerp(startVolume, 0, t);
                yield return null;
            }

            audioSource.Stop();
        }

        /// <summary>Plays <see cref="SoundEffect.audioSource"/> and notifies all subscribers of the <see cref="SoundEffect.OnPlay"/> <see langword="event"/>.</summary>
        private void PlayAudioSource()
        {
            audioSource.Play();

            OnPlay?.Invoke(this);
        }

        /// <summary>Sets up the supplied <see cref="AudioClip"/> from this object and plays it.</summary>
        public void PlayClip(AudioClip clip, bool loop = false)
        {
            SetupClip(clip, loop);

            PlayAudioSource();
        }
    }
}
