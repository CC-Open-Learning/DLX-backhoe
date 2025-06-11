using System;
using System.Collections.Generic;
using UnityEngine;

namespace RemoteEducation.Helpers.Unity
{
	[Obsolete("Use RemoteEducation.SoundEffect instead!",false)]
	public class SoundRandomizer : MonoBehaviour
	{
		[SerializeField] private List<AudioSource> soundEffects = null;

		void Start()
		{
			for (int i = 0; i < soundEffects.Count; ++i)
			{
				if ((soundEffects[i] == null) || (soundEffects[i].clip == null))
				{
					soundEffects.RemoveAt(i);
					--i;
				}
			}
		}

		public void Play()
		{
			soundEffects[UnityEngine.Random.Range(0, soundEffects.Count)].Play();
		}
	}
}