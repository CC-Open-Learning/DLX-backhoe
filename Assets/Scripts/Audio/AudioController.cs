/*
 * SUMMARY: The purpose of this class is to have a dictionary of voice over clips as the value and their name as the key.
 *          This will allow us to store all voice over clips in 1 spot, and allow graph scenarios to make calls like 
 *          AudioController.Instance.PlayClip(Key) to play the voice over clip by key non-linearly from any 
 *          OnEnterVertex() or OnLeaveVertex().
 */



using RemoteEducation.Audio;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class AudioController : MonoBehaviour
{
    [Serializable]
    struct VoiceClip
    {
        public string Name;
        public AudioClip AudioClip;
    }

    private SoundEffect soundEffect;

    //current vertex play audio coroutine
    private Coroutine currentCoroutine;

    private Coroutine reminderCoroutine;

    /// <summary>Dictionary of all voice clips (Value) and their file names (Key)</summary>
    private Dictionary<string, AudioClip> dialogueDictionary = new Dictionary<string, AudioClip>();

    [SerializeField, Tooltip("List of Audio Clips (Key = Name, Value = Audio Clip)")]
    private List<VoiceClip> voiceClips;

    /// <summary> Ensures exactly one AudioController exists in a Unity Scene </summary>
    /// <remarks> This is an implementation of the Singleton design pattern in the Unity environment </remarks>
    public static AudioController Instance;

    private void Start()
    {
        //Get reference to Instance
        if (Instance != null)
        {
            Debug.LogError("AudioController already exists. Deleting this instance's gameobject.");
            Destroy(Instance.gameObject);
        }

        //Assign this object to the static Instance property
        Instance = this;

        soundEffect = GetComponent<SoundEffect>();
        PopulateDictionary();
    }

    /// <summary>Populate the dictionary with all the audio clips, and their names from the VoiceClips list
    /// so we can access them by key non-linearly from the graph scenario</summary>
    private void PopulateDictionary()
    {
        //Add all VoiceClips in the list to the dictionary here
        foreach (VoiceClip voiceClip in voiceClips)
        {
            voiceClip.Name.Trim();

            if (voiceClip.Name.Length > 0)
            {
                dialogueDictionary.Add(voiceClip.Name, voiceClip.AudioClip);
            }
        }
    }

    /// <summary>Play the audio clip matching the given key in VoiceClipDict</summary>
    /// <param name="key">Key for the dictionary entry (The name of the audio clip)</param>
    /// <param name="reminder">if reminder is true the voice over reminder coroutine gets called</param>
    public void PlayClip(string key, bool reminder = false, float wait = 0)
    {
        if (!dialogueDictionary.TryGetValue(key, out AudioClip clip))
        {
            return;
        }
        if (!reminder)
        {
            soundEffect.PlayClip(clip);
        }
        else
        {
            reminderCoroutine = StartCoroutine(VoiceReminder(clip, wait));
        } 
    }

    /// <summary>
    /// Play multiple audio clips sequentially by matching the given key in VoicClipDict</summary>
    /// <param name="keys">List of keys for the dictionary entry (names of the audio clips)(</param>
    public void PlayMultipleClips(string [] keys)
    {
        List<AudioClip> audioClips = new List<AudioClip>();
        foreach (var key in keys)
        {
            if (!dialogueDictionary.TryGetValue(key, out AudioClip clip))
            {
                return;
            }

            audioClips.Add(clip);
        }

        currentCoroutine = StartCoroutine(DelayVoiceClip(audioClips));
    }

    /// <summary>
    /// Plays each one of the clips on the list in sequence</summary>
    /// <param name="audioClips">List of audio clips</param>
    /// <returns></returns>
    private IEnumerator DelayVoiceClip (List<AudioClip> audioClips)
    {
        foreach (var clip in audioClips)
        {
            soundEffect.PlayClip(clip);

            yield return new WaitForSeconds(clip.length);
        }
    }

    // Stop Coroutine for multiple audio clips
    public void StopMultipleClips()
    {
        soundEffect.audioSource.Stop();
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }

    }

    // Stop playing audio clip
    public void StopClip()
    {
        soundEffect.audioSource.Stop();
        if (reminderCoroutine != null)
        {
            StopCoroutine(reminderCoroutine);
            reminderCoroutine = null;
        }

        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }
    }

    public float GetClipLength (string key)
    {
        if (!dialogueDictionary.TryGetValue(key, out AudioClip clip))
        {
            return 0;
        }
        return clip.length;
    }

    public float GetMultipleClipLength(string [] keys)
    {
        float length = 0;

        foreach (var key in keys)
        {
            length += GetClipLength(key);
        }

        return length;
    }

    public IEnumerator VoiceReminder(AudioClip clip, float wait)
    {
        yield return new WaitForSeconds(wait + 10f);
        soundEffect.PlayClip(clip);
    }
}