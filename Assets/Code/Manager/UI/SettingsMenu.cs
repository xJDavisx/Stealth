using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Jesse.Managers
{
	public class SettingsMenu : Manager
	{

		public AudioMixer mixer;
		[SerializeField]
		string masterVolumeVarName = "Master";
		[SerializeField]
		string musicVolumeVarName = "Music";
		[SerializeField]
		string effectsVolumeVarName = "SoundFX";
		[SerializeField]
		Slider masterSlider;
		[SerializeField]
		Slider musicSlider;
		[SerializeField]
		Slider effectsSlider;

		protected override void Start()
		{
			base.Start();
			if (masterSlider)
			{
				masterSlider.value = GetMixerFloat(masterVolumeVarName);
			}
			else
			{
				Debug.LogError("You need to add a Slider for the Master Volume in the " + gameObject.name + " Game Object");
			}
			if (musicSlider)
			{
				musicSlider.value = GetMixerFloat(musicVolumeVarName);
			}
			else
			{
				Debug.LogError("You need to add a Slider for the Music Volume in the " + gameObject.name + " Game Object");
			}
			if (effectsSlider)
			{
				effectsSlider.value = GetMixerFloat(effectsVolumeVarName);
			}
			else
			{
				Debug.LogError("You need to add a Slider for the Sound FX Volume in the " + gameObject.name + " Game Object");
			}
		}

		public float MasterVolume
		{
			get
			{
				return GetMixerFloat(masterVolumeVarName);
			}
			set
			{
				SetMixerFloat(masterVolumeVarName, value);
			}
		}

		public float MusicVolume
		{
			get
			{
				return GetMixerFloat(musicVolumeVarName);
			}
			set
			{
				SetMixerFloat(musicVolumeVarName, value);
			}
		}

		public float SoundFXVolume
		{
			get
			{
				return GetMixerFloat(effectsVolumeVarName);
			}
			set
			{
				SetMixerFloat(effectsVolumeVarName, value);
			}
		}

		float GetMixerFloat(string varName)
		{
			float value = Mathf.NegativeInfinity;
			if (mixer)
			{
				if (!mixer.GetFloat(varName, out value))
				{
					Debug.LogError("Float with name " + varName + " not found in mixer!");
				}
			}
			else
			{
				Debug.LogError("Mixer is null!");
			}

			return value;
		}

		private void SetMixerFloat(string varName, float value)
		{
			if (mixer)
			{
				if (!mixer.SetFloat(varName, value))
				{
					Debug.LogError("Float with name " + varName + " not found in mixer!");
				}
			}
			else
			{
				Debug.LogError("Mixer is null!");
			}
		}

	}
}