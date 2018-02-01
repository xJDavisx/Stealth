using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jesse.Managers
{
	[RequireComponent(typeof(AudioSource))]
	public class BGMManager : Manager
	{

		Dictionary<BGMNames, AudioClip> BGMClips = new Dictionary<BGMNames, AudioClip>();
		BGMNames currentBGM;
		new AudioSource audio;

		protected override void Awake()
		{
			base.Awake();
			audio = GetComponent<AudioSource>();
			foreach (string s in Enum.GetNames(typeof(BGMNames)))
			{
				BGMClips.Add((BGMNames)Enum.Parse(typeof(BGMNames), s), Resources.Load<AudioClip>("Audio/BGM/" + s.ToLower()));
			}
			CurrentBGM = BGMNames.Cavern;
		}

		public BGMNames CurrentBGM
		{
			get
			{
				return currentBGM;
			}
			set
			{
				if (currentBGM != value)
				{
					currentBGM = value;
					if (audio.isPlaying)
					{
						audio.Stop();
					}
					audio.clip = BGMClips[currentBGM];
					audio.Play();
				}
			}
		}

		public enum BGMNames
		{
			Encounter,
			Cavern
		}
	}
}