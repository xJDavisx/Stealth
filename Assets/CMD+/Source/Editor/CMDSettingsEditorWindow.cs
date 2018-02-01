using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

namespace DaanRuiter.CMDPlus.Editor {
    public class CMDSettingsEditorWindow : EditorWindow {
        private const string EDITOR_SETTINGS_FILENAME = "EditorSettings.xml";
        private const string LOGO_IMAGE_PATH = "Images/logo";
        private static readonly string[] EXCLUDED_SETTING_FILES = {
            EDITOR_SETTINGS_FILENAME
        };

        private Dictionary<string, List<CMDSetting<object>>> m_loadedSettings;
        private Dictionary<string, List<int>> m_unsaved;
        private Dictionary<string, NewSettingDialogueInfo> m_newSettingDialogues;
        private GUIStyle m_errorStyle;
        private Texture2D m_logoTexture;
        private bool autoSaveEnabled { get { return CMDSettings.Get<bool>("Autosave changes");} }

        [MenuItem("Tools/CMD+/Settings")]
        private static void Init() {
            CMDSettingsEditorWindow window = (CMDSettingsEditorWindow)GetWindow(typeof(CMDSettingsEditorWindow));
            window.Show();
            window.titleContent = new GUIContent("CMDSettings");

            if (CMDHelper.CopyDefaultSettingsIfNeeded()) {
                window.Load();
            }
        }

        [MenuItem("Tools/CMD+/Show logs")]
        private static void ShowLogs() {
            CMDLogger.CreateFoldersIfNecessary();
            System.Diagnostics.Process.Start(@Application.dataPath + "/../" + CMDStrings.CMD_LOGGER_PATH);
        }

        private void OnValueChanged(string settingsFile, int localIndex) {
            CMDSettings.Set(m_loadedSettings[settingsFile][localIndex].Identifier, 
                            m_loadedSettings[settingsFile][localIndex].Data, 
                            settingsFile);

            if (autoSaveEnabled) {
                Save(false);
                Load();
            } else {
                if (!m_unsaved.ContainsKey(settingsFile)) {
                    m_unsaved.Add(settingsFile, new List<int>());
                }
                m_unsaved[settingsFile].Add(localIndex);
            }
        }

        public void Load() {
            CMDSettings.LoadSettings();

            m_loadedSettings = CMDSettings.GetAllPerFile();
            m_unsaved = new Dictionary<string, List<int>>();
            m_newSettingDialogues = new Dictionary<string, NewSettingDialogueInfo>();            
            m_logoTexture = Resources.Load<Texture2D>(LOGO_IMAGE_PATH);

            foreach (var settingFile in m_loadedSettings) {                
                m_newSettingDialogues.Add(settingFile.Key, new NewSettingDialogueInfo(settingFile.Key));
            }
        }

        public void Save(bool showMessage = true) {
            CMDSettings.SaveSettings(showMessage);
            m_unsaved.Clear();
        }

        private void InitStyle() {
            m_errorStyle = new GUIStyle(EditorStyles.largeLabel);
            m_errorStyle.normal.textColor = Color.red;
            m_errorStyle.fontSize = 14;
            m_errorStyle.wordWrap = true;
            m_errorStyle.alignment = TextAnchor.UpperLeft;
        }

        private void OnFocus() {
            if (m_loadedSettings == null) {
                Load();
            }
        }

        private void HandleSettingValue(ref object value, Type dataType, int localIndex, string settingsFile) {
            GUILayout.FlexibleSpace();

            //FLOAT
            if (dataType == typeof(float))
            {
                float temp = (float)value;
                GUI_Float(ref temp);
                if ((float)value != temp)
                {
                    value = temp;
                    OnValueChanged(settingsFile, localIndex);
                }
            }
            //COLOR
            else if (dataType == typeof(string))
            {
                Color parsedColor;
                if (CMDHelper.TryParseColor((string)value, out parsedColor))
                {
                    Color temp = parsedColor;
                    GUI_Color(ref temp);
                    string colorAsString = CMDHelper.ColorToString(temp);
                    if ((string)value != colorAsString)
                    {
                        value = colorAsString;
                        OnValueChanged(settingsFile, localIndex);
                    }
                }
                //STRING
                else
                {
                    string temp = (string)value;
                    GUI_String(ref temp);
                    if ((string)value != temp)
                    {
                        value = temp;
                        OnValueChanged(settingsFile, localIndex);
                    }
                }
                //BOOL
            }
            else if (dataType == typeof(bool))
            {
                bool temp = (bool)value;
                GUI_Bool(ref temp);
                if ((bool)value != temp)
                {
                    value = temp;
                    OnValueChanged(settingsFile, localIndex);
                }
            }
            //INT
            else if (dataType == typeof(int))
            {
                int temp = (int)value;
                GUI_Int(ref temp);
                if ((int)value != temp)
                {
                    value = temp;
                    OnValueChanged(settingsFile, localIndex);
                }
            }
            //UNKOWN
            else
            { //INSERT OTHER TYPES BEFORE THIS ONE
                GUI_UnkownType(dataType);
            }
        }

        private void OnGUI() {
            if (m_loadedSettings == null) { return; }

            GUILayout.Space(5);
            if(m_logoTexture != null) {
                GUI.DrawTexture(new Rect(Screen.width / 2f - m_logoTexture.width / 2f, 0, m_logoTexture.width, m_logoTexture.height), m_logoTexture);
            }
            GUILayout.Space(m_logoTexture.height);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Load")) {
                Load();
            }
            if (GUILayout.Button("Save")) {
                Save();
            }
            if (GUILayout.Button("Open folder")) {
                System.Diagnostics.Process.Start(@Application.dataPath + "/../" + CMDStrings.CMD_SETTINGS_PATH);
            }
            Color orgBGColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0.4f, 0.85f, 0.65f);
            if (GUILayout.Button("New file")) {
                CMDSettingsNewFileEditorPopup.Init();
            }
            GUI.backgroundColor = orgBGColor;
            GUILayout.EndHorizontal();

            if (m_loadedSettings.ContainsKey(EDITOR_SETTINGS_FILENAME)) {
                GUI_EditorSettings(m_loadedSettings[EDITOR_SETTINGS_FILENAME]);
            }

            GUI_Line();

            foreach (var settingFile in m_loadedSettings) {
                if (IsFileExcluded(settingFile.Key)) { continue; }

                GUILayout.BeginHorizontal();
                GUI_Title(settingFile.Key);
                orgBGColor = GUI.backgroundColor;
                GUI.backgroundColor = new Color(1f, 0.35f, 0.35f);
                if (GUILayout.Button("Delete file", GUILayout.Width(150))) {
                    CMDSettings.DeleteFile(settingFile.Key);
                    Load();
                    return;
                }
                GUI.backgroundColor = orgBGColor;
                GUILayout.EndHorizontal();
                GUILayout.Space(5);

                List<int> removeList = new List<int>();
                for (int i = 0; i < settingFile.Value.Count; i++) {
                    CMDSetting<object> setting = settingFile.Value[i];

                    GUILayout.BeginHorizontal();

                    GUI_Name(setting.Identifier, i, settingFile.Key);
                    HandleSettingValue(ref setting.Data, setting.Data.GetType(), i, settingFile.Key);
                    if (GUILayout.Button("X", GUILayout.Width(20))) {
                        removeList.Add(i);
                    }

                    GUILayout.EndHorizontal();
                }

                for (int i = 0; i < removeList.Count; i++) {
                    string identifier = settingFile.Value[removeList[i]].Identifier;
                    
                    settingFile.Value.RemoveAt(removeList[i]);
                    CMDSettings.Remove(identifier);
                }

                GUI_NewSettingDialogue(settingFile.Key);
            }

            string[] mobileSettings = AssetDatabase.FindAssets("t:CMDMobileSettings");
            if (mobileSettings != null && mobileSettings.Length > 0)
            {
                if (GUILayout.Button("Select Mobile Settings asset"))
                {
                    Selection.activeObject = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(mobileSettings[0]), typeof(CMDMobileSettings));
                }
            }
        }

        private void GUI_EditorSettings(List<CMDSetting<object>> settings) {
            GUI_Title("Editor Settings");
            for (int i = 0; i < settings.Count; i++) {
                CMDSetting<object> setting = settings[i];
                GUILayout.BeginHorizontal();
                GUI_Name(setting.Identifier, i, EDITOR_SETTINGS_FILENAME);
                HandleSettingValue(ref setting.Data, setting.Data.GetType(), i, EDITOR_SETTINGS_FILENAME);
                GUILayout.EndHorizontal();
            }
        }

        private void GUI_Float(ref float value) {
            value = EditorGUILayout.FloatField(value, GUILayout.Width(Screen.width * 0.4f));
        }

        private void GUI_Bool(ref bool value) {
            value = EditorGUILayout.Toggle(value, GUILayout.Width(Screen.width * 0.4f));
        }

        private void GUI_Int(ref int value) {
            value = EditorGUILayout.IntField(value, GUILayout.Width(Screen.width * 0.4f));
        }

        private void GUI_String(ref string value) {
            value = EditorGUILayout.TextField(value, GUILayout.Width(Screen.width * 0.4f));
        }

        private void GUI_Color(ref Color value) {
            value = EditorGUILayout.ColorField(value, GUILayout.Width(Screen.width * 0.4f));
        }

        private void GUI_Name(string name, int localIndex, string fileName) {
            if (IsSettingUnsaved(localIndex, fileName)) {
                EditorGUILayout.LabelField(name, EditorStyles.boldLabel, GUILayout.Width(Screen.width * 0.5f));
            } else {
                EditorGUILayout.LabelField(name, GUILayout.Width(Screen.width * 0.5f));
            }
        }

        private void GUI_Title(string title) {
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
        }

        private void GUI_UnkownType(Type type) {
            if (m_errorStyle == null) { InitStyle(); }
            EditorGUILayout.LabelField(string.Format("UNKOWN TYPE: {0}", type.Name), m_errorStyle, GUILayout.Width(Screen.width * 0.4f));
        }

        private void GUI_NewSettingDialogue(string settingFile) {
            NewSettingDialogueInfo info = m_newSettingDialogues[settingFile];
            
            if (!info.Openend) {
                if (GUILayout.Button("New Setting")) {
                    info.Open();
                }
            } else {
                if (GUILayout.Button("Cancel")) {
                    info.Close();
                }

                GUILayout.BeginHorizontal();
                info.SettingName = GUILayout.TextField(info.SettingName, GUILayout.Width(Screen.width * 0.55f));
                if (GUILayout.Button("Add", GUILayout.Width(Screen.width * 0.4f))) {
                    if (!CMDSettings.Has(info.SettingName) && info.CurrentSelectedType != null) {
                        info.Add();
                        CMDSetting<object>[] allSettings = CMDSettings.GetAll();
                        m_loadedSettings[settingFile].Add(allSettings[allSettings.Length - 1]);
                        OnValueChanged(settingFile, m_loadedSettings[settingFile].Count - 1);
                    }
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUI_DialogueTypeButton("String", typeof(string), info);
                GUI_DialogueTypeButton("Color", typeof(Color), info);
                GUI_DialogueTypeButton("Float", typeof(float), info);
                GUI_DialogueTypeButton("Int", typeof(int), info);
                GUI_DialogueTypeButton("Bool", typeof(bool), info);
                GUILayout.EndHorizontal();
            }
        }

        private void GUI_Line() {
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
        }

        private void GUI_DialogueTypeButton(string text, Type type, NewSettingDialogueInfo info) {
            if(info.CurrentSelectedType == type) {
                GUI.backgroundColor = Color.cyan;
            }else {
                GUI.backgroundColor = Color.white;
            }
            if (GUILayout.Button(text)) {
                info.CurrentSelectedType = type;
            }
            GUI.backgroundColor = Color.white;
        }

        private bool IsFileExcluded(string fileName) {
            for (int i = 0; i < EXCLUDED_SETTING_FILES.Length; i++) {
                if (fileName == EXCLUDED_SETTING_FILES[i]) {
                    return true;
                }
            }
            return false;
        }

        private bool IsSettingUnsaved(int localIndex, string fileName) {
            if (m_unsaved.ContainsKey(fileName)) {
                return m_unsaved[fileName].Contains(localIndex);
            }
            return false;
        }
    }

    public class NewSettingDialogueInfo {
        public bool Openend { get; private set; }
        public string SettingFileName { get; private set; }
        public Type CurrentSelectedType;
        public string SettingName;

        private const string DEFAULT_SETTING_NAME = "SETTING_NAME";

        public NewSettingDialogueInfo(string settingFileName) {
            Openend = false;
            SettingFileName = settingFileName;
            SettingName = DEFAULT_SETTING_NAME;
        }

        public void Open() {
            Openend = true;
        }

        public void Close() {
            Openend = false;
            CurrentSelectedType = null;
            SettingName = DEFAULT_SETTING_NAME;
        }

        public void Add() {
            object data = null;

            if (CurrentSelectedType == typeof(float))
            {
                data = 0f;
            }
            else if (CurrentSelectedType == typeof(string))
            {
                data = string.Empty;
            }
            else if (CurrentSelectedType == typeof(Color))
            {
                data = "0/0/0/1";
            }
            else if (CurrentSelectedType == typeof(bool))
            {
                data = false;
            }
            else if (CurrentSelectedType == typeof(int))
            {
                data = 0;
            }

            if(data != null) {
                CMDSettings.Set(SettingName, data, SettingFileName);
            }else {
                Debug.Log("Unkown type: " + CurrentSelectedType);
            }

            Close();
        }
    }
}