using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using RemoteEducation.Audio;
using UnityEngine.Audio;
using System;

public class AudioManagerTests
{
    private const float MAX_INITIALIZE_WAIT_TIME = 30f;

    // Timescale trick is not recommended for these tests as the Audio Manager uses UNSCALED time for its Coroutines.
    private AudioManager am;
    private AudioListener a;

    private AudioManager.Channel.Name[] channels;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        channels = (AudioManager.Channel.Name[])Enum.GetValues(typeof(AudioManager.Channel.Name));

        // This shuts up the logger warnings.
        a = new GameObject("Audio Listener Dummy").AddComponent<AudioListener>();
        AudioListener.volume = 0f;
    }

    [UnitySetUp]
    public IEnumerator Setup()
    {
        am = new GameObject("Test Audio Manager").AddComponent<AudioManager>();

        var startTime = Time.unscaledTime;
        yield return new WaitUntil(() => WaitUntilTimeOutWrapper(am.Initialized, startTime, MAX_INITIALIZE_WAIT_TIME));
    }

    [TearDown]
    public void TearDown()
    {
        AudioManager.DeleteSingleton();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        GameObject.Destroy(a.gameObject);
    }

    [Test]
    public void SetMixerGroup_ChangeAudioMixerGroups_Success()
    {
        var audioSource = new GameObject("Test Audio Source").AddComponent<AudioSource>();

        audioSource.SetMixerGroup(AudioManager.Channel.Name.Engine);

        var desiredOutcome = AudioManager.Channel.Name.SimulationBackground;

        audioSource.SetMixerGroup(desiredOutcome);

        var outcome = audioSource.outputAudioMixerGroup;

        GameObject.Destroy(audioSource.gameObject);

        Assert.AreEqual(AudioManager.GetMixerGroup(desiredOutcome), outcome);
    }

    [Test]
    public void SetAndGetVolume_AllChannelsSetDifferently_GetVolumeProvidesCorrectValues()
    {
        var volumes = GenerateVolumes();

        SetVolumes(volumes);

        var results = new float[volumes.Length];

        for (int i = 0; i < channels.Length; i++)
        {
            results[i] = AudioManager.GetVolume(channels[i]);
        }

        Assert.That(results, Is.EqualTo(volumes).Within(0.00001f));
    }

    [UnityTest]
    public IEnumerator MuteChannel_AllChannels_AllMuted()
    {
        MuteAllChannels(false, 0f);

        var volumes = GenerateVolumes();

        SetVolumes(volumes);

        MuteAllChannels(true, 0.5f);

        yield return new WaitForSeconds(1f);

        var results = GetLinearChannelVolumesFromMixer();

        var expected = new float[results.Length];

        for (int i = 0; i < channels.Length; i++)
        {
            expected[i] = AudioManager.OFF_VOLUME_LINEAR;
        }

        Assert.That(results, Is.EqualTo(expected).Within(0.00001f));
    }

    [UnityTest]
    public IEnumerator MuteChannel_MuteAndUnmuteAllChannels_AllUnMuted()
    {
        MuteAllChannels(false, 0f);

        var volumes = GenerateVolumes();

        SetVolumes(volumes);

        MuteAllChannels(true, 0.5f);

        yield return new WaitForSeconds(1f);

        MuteAllChannels(false, 0.5f);

        yield return new WaitForSeconds(1f);

        var results = GetLinearChannelVolumesFromMixer();

        Assert.That(results, Is.EqualTo(volumes).Within(0.00001f));
    }

    [UnityTest]
    public IEnumerator MuteChannel_ToggleMuteAllChannelsConstantly_AllUnMuted()
    {
        MuteAllChannels(false, 0f);

        var volumes = GenerateVolumes();

        SetVolumes(volumes);

        var timings = new float[] { 0.3f, 0.1f, 0.3f, 1f, 0.1f, 0f, 0.1f, 0.0f }; // This absolutely requires an even number of entries otherwise the test fails.

        for (int i = 0; i < timings.Length; i++)
        {
            ToggleMuteAllChannels(0.5f);
            yield return new WaitForSeconds(timings[i]);
        }
       
        yield return new WaitForSeconds(1f);

        var results = GetLinearChannelVolumesFromMixer();

        Assert.That(results, Is.EqualTo(volumes).Within(0.00001f));
    }

    private void MuteAllChannels(bool mute, float fadeTime)
    {
        for (int i = 0; i < channels.Length; i++)
        {
            AudioManager.MuteChannel(channels[i], mute, fadeTime);
        }
    }

    private void ToggleMuteAllChannels(float fadeTime)
    {
        for (int i = 0; i < channels.Length; i++)
        {
            AudioManager.ToggleMuteChannel(channels[i], fadeTime);
        }
    }

    private float[] GenerateVolumes()
    {
        var volumes = new float[channels.Length];

        var delta = 1f / (volumes.Length - 1);

        for (int i = 0; i < volumes.Length; i++)
        {
            volumes[i] = Mathf.Clamp(delta * i, AudioManager.OFF_VOLUME_LINEAR, 1f);
        }

        return volumes;
    }

    private void SetVolumes(float[] volumes)
    {
        for (int i = 0; i < volumes.Length; i++)
        {
            AudioManager.SetVolume(channels[i], volumes[i]);
        }
    }

    private float[] GetLinearChannelVolumesFromMixer()
    {
        var results = new float[channels.Length];

        for (int i = 0; i < channels.Length; i++)
        {
            results[i] = am.GetVolumeFromMixer(channels[i]);
        }

        return results;
    }

    /// <summary>Checks that a condition is met within the given <paramref name="timeOut"/> period.</summary>
    /// <remarks>If this method is called when the difference between <see cref="Time.unscaledTime"/> and <paramref name="startTime"/> exceeds <paramref name="timeOut"/>, it calls <see cref="Assert.Fail"/>.</remarks>
    /// <returns><paramref name="condition"/></returns>
    private static bool WaitUntilTimeOutWrapper(bool condition, float startTime, float timeOut)
    {
        if (startTime + timeOut < Time.unscaledTime)
        {
            Assert.Fail($"WaitUntil condition was not met within {timeOut}s of test time!");
        }

        return condition;
    }
}


