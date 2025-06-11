using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Audio;
using TMPro;

namespace RemoteEducation.Audio
{
    public static class Extensions
    {
        public static void SetMixerGroup(this AudioSource audioSource, AudioManager.Channel.Name channel)
        {
            audioSource.outputAudioMixerGroup = AudioManager.GetMixerGroup(channel);
        }
    }
}
