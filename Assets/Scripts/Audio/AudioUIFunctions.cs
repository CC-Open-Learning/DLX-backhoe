using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

namespace RemoteEducation.Audio
{
    public class AudioUIFunctions : MonoBehaviour
    {
        [SerializeField]
        private AudioManager.Channel.Name channel;

        public void SetVolume(float volume)
        {
            AudioManager.SetVolume(channel, volume);
        }
        public void MuteChannel()
        {
            AudioManager.MuteChannel(channel, true);
        }
        public void UnmuteChannel()
        {
            AudioManager.MuteChannel(channel, false);
        }
        public void ToggleMuteChannel()
        {
            AudioManager.ToggleMuteChannel(channel);
        }
    }
}
