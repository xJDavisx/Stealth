using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Jesse.UI;
using Jesse.Managers;

namespace Jesse.Editor.UI
{
	[CustomEditor(typeof(GameUI))]
	public class GameUIEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			GameUI ui = (GameUI)target;
			base.OnInspectorGUI();
			//ui.alertUI = (GameObject)EditorGUILayout.ObjectField("Alert UI:", ui.alertUI, typeof(GameObject), true);

			//ui.cautionUI = (GameObject)EditorGUILayout.ObjectField("Caution UI:", ui.cautionUI, typeof(GameObject), true);

			//ui.gameLoseUI = (GameObject)EditorGUILayout.ObjectField("Game Lose UI:", ui.gameLoseUI, typeof(GameObject), true);

			//ui.gameWinUI = (GameObject)EditorGUILayout.ObjectField("Game Win UI:", ui.gameWinUI, typeof(GameObject), true);

			//ui.optionsAudioUI = (GameObject)EditorGUILayout.ObjectField("Audio Options UI:", ui.optionsAudioUI, typeof(GameObject), true);

			//ui.optionsMainUI = (GameObject)EditorGUILayout.ObjectField("Main Options UI:", ui.optionsMainUI, typeof(GameObject), true);

			//ui.pauseUI = (GameObject)EditorGUILayout.ObjectField("Pause UI:", ui.pauseUI, typeof(GameObject), true);

			//ui.quitUI = (GameObject)EditorGUILayout.ObjectField("Quit UI: ", ui.quitUI, typeof(GameObject), true);

		}
	}
}