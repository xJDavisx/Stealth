using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Jesse.Character;

namespace Jesse.Editor.Character
{
	[CustomEditor(typeof(Jesse.Character.Character))]
	public class CharacterEditor : UnityEditor.Editor
	{
		private static bool showCharacterValues = false,
			showViewValues = false;
		private Jesse.Character.Character character = null;
		protected float lableWidth = 140;
		protected float boolWidth = 10;

		public virtual void OnEnable()
		{
			character = (Jesse.Character.Character)target;
		}

		public override void OnInspectorGUI()
		{

			GUILayout.BeginHorizontal();
			showCharacterValues = EditorGUILayout.Toggle(showCharacterValues, GUILayout.Width(boolWidth));
			EditorGUILayout.LabelField("Character Values", EditorStyles.boldLabel);
			GUILayout.EndHorizontal();
			if (showCharacterValues)
			{
				GUILayout.Space(5);

				character.MaxHealth = EditorGUILayout.FloatField("Max Health", character.MaxHealth);

				character.Health = EditorGUILayout.FloatField("Health", character.Health);

				character.CanRegenHealth = EditorGUILayout.Toggle("Regenerate Health", character.CanRegenHealth);

				if (character.CanRegenHealth)
				{
					character.HealthRegenWaitTime = EditorGUILayout.FloatField("Regen Wait Time", character.HealthRegenWaitTime);

					character.HealthRegenSpeed = EditorGUILayout.FloatField("Regen Speed", character.HealthRegenSpeed);

				}
				GUILayout.Space(5);

				GUILayout.BeginHorizontal();
				showViewValues = EditorGUILayout.Toggle(showViewValues, GUILayout.Width(boolWidth));
				EditorGUILayout.LabelField("View Values", EditorStyles.boldLabel);
				GUILayout.EndHorizontal();

				if (showViewValues)
				{
					character.MaxViewAngle = EditorGUILayout.FloatField("Max View Angle", character.MaxViewAngle);

					character.MinViewAngle = EditorGUILayout.FloatField("Min View Angle", character.MinViewAngle);

					character.ViewAngle = EditorGUILayout.FloatField("View Angle", character.ViewAngle);

					character.MinViewAngle = EditorGUILayout.FloatField("Min View Angle", character.MinViewAngle);

					character.MinViewAngle = EditorGUILayout.FloatField("Min View Angle", character.MinViewAngle);

					character.MinViewAngle = EditorGUILayout.FloatField("Min View Angle", character.MinViewAngle);

				}
			}
		}
	}
}