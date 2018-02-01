using UnityEngine;
using UnityEditor;

namespace DaanRuiter.CMDPlus.Editor {
    public class CMDSettingsNewFileEditorPopup : EditorWindow {
        private string fileName = string.Empty;

        public static void Init() {
            CMDSettingsNewFileEditorPopup window = CreateInstance<CMDSettingsNewFileEditorPopup>();
            window.position = new Rect(0, 0, 300, 80);
            window.titleContent = new GUIContent("New file");
            window.Show();
        }

        void OnGUI() {
            EditorGUILayout.LabelField("Insert filename (without file extension)", EditorStyles.boldLabel);
            fileName = GUILayout.TextField(fileName);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Create")) {
                CMDSettings.CreateFile(fileName + ".xml");
                CMDSettingsEditorWindow settingsEditor = (CMDSettingsEditorWindow)GetWindow(typeof(CMDSettingsEditorWindow));
                settingsEditor.Load();
                Close();
            }
            if (GUILayout.Button("Cancel")) {
                Close();
            }
            GUILayout.EndHorizontal();
        }
    }
}