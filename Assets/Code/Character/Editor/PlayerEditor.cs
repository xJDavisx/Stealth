using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Jesse.Character;

namespace Jesse.Editor.Character
{
	[CustomEditor(typeof(Player))]
	public class PlayerEditor : CharacterEditor
	{
		private static bool showPlayerValues = false;
		private Player player = null;

		public override void OnEnable()
		{
			base.OnEnable();
			player = (Player)target;
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			GUILayout.BeginHorizontal();
			showPlayerValues = EditorGUILayout.Toggle(showPlayerValues, GUILayout.Width(boolWidth));
			EditorGUILayout.LabelField("Player Values", EditorStyles.boldLabel);
			GUILayout.EndHorizontal();
			if (showPlayerValues)
			{
				GUILayout.Space(5);

				player.MaxHealth = EditorGUILayout.FloatField("Max Health", player.MaxHealth);
				
			}
		}
	}
}