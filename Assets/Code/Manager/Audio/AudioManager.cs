using System;
using System.Collections;
using System.Collections.Generic;
using Jesse.Character;
using UnityEngine;

namespace Jesse.Managers
{
	[RequireComponent(typeof(AudioSource))]
	public class AudioManager : Manager
	{
		AudioSource audioSource;


		protected override void Awake()
		{
			base.Awake();
			audioSource = GetComponent<AudioSource>();
		}

		public void PlaySound(string resourceName)
		{
			AudioClip c = Resources.Load<AudioClip>(resourceName);
			PlaySound(c);
		}

		public void PlaySound(AudioClip clip)
		{
			if (clip != null)
				if (audioSource != null)
					audioSource.PlayOneShot(clip);
				else
					Debug.LogError("Audio Source is Null");
			else
				Debug.LogError("Audio Clip is Null");
		}
	}
}