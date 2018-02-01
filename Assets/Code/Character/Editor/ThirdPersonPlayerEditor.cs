using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Jesse.Character;

namespace Jesse.Editor.Character
{
	[CustomEditor(typeof(ThirdPersonPlayer))]
	public class ThirdPersonPlayerEditor : PlayerEditor
	{
		private bool CToggle = false;
		private ThirdPersonPlayer thirdPersonPlayer = null;
		public override void OnEnable()
		{
			base.OnEnable();
			thirdPersonPlayer = (ThirdPersonPlayer)target;
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			EditorGUILayout.LabelField("Third Person Player Values", EditorStyles.boldLabel);
			GUILayout.BeginHorizontal();
			GUILayout.Label("A Features", GUILayout.Width(70));
			CToggle = EditorGUILayout.Toggle(CToggle);
			GUILayout.EndHorizontal();
			if (CToggle)
			{
				GUILayout.Space(5);
				GUILayout.BeginHorizontal();
				GUILayout.Label("This Var", GUILayout.Width(70));
				//c.ThisVar = EditorGUILayout.TextField(c.ThisVar);
				GUILayout.EndHorizontal();
				GUILayout.Space(5);
				GUILayout.BeginHorizontal();
				GUILayout.Label("And This One", GUILayout.Width(70));
				//c.AndThisOne = EditorGUILayout.TextField(c.AndThisOne);
				GUILayout.EndHorizontal();
				GUILayout.Space(5);
				GUILayout.BeginHorizontal();
				GUILayout.Label("And This Can Be Slider", GUILayout.Width(70));
				//c.AndThisCanBeSlider = EditorGUILayout.Slider(c.AndThisCanBeSlider, 0f, 100f);
				GUILayout.EndHorizontal();
			}
			//blablabla
		}
	}
}