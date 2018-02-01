using System;
using System.Collections.Generic;
using UnityEngine;

namespace DaanRuiter.CMDPlus {
    public class CMDCommandInputField : CMDInputField {
        private const int MAX_OPTION_COUNT = 5;

        public string AutoCompleteString {
            get {
                if(m_autoCompleteOptions.Count == 0) {
                    return string.Empty;
                }
                return m_autoCompleteOptions[0];
            }
        }

        private List<string> m_autoCompleteOptions = new List<string>();
        private CMDCommandButton m_submitButton;

        public CMDCommandInputField(string windowName, Vector2 position, Vector2 size) : base(windowName, position, size) {
            UserInputChangeEvent += OnUserInput;
            UserInputSubmitEvent += ProcessCommandline;

            m_submitButton = new CMDCommandButton("Submit");
            m_submitButton.SetRect(new Rect(rect.width, rect.y, rect.width * 0.25f, rect.height + 2.5f));
            m_submitButton.SetColor(Color.white);

            skin.button.normal.textColor = Color.white * 0.9f;
            skin.button.alignment = TextAnchor.MiddleCenter;
            skin.button.normal.background = m_submitButton.Texture;
            skin.button.hover.background = m_submitButton.MouseOverTexture;
        }
        ~CMDCommandInputField() {
            UserInputChangeEvent -= OnUserInput;
            UserInputSubmitEvent -= ProcessCommandline;
        }

        public override void OnRender() {
            base.OnRender();
            if (!enabled) { return; }
            if (m_autoCompleteOptions.Count > 0) {
                RenderTextInstructions(m_autoCompleteOptions[0]);
            }

            skin.button.normal.background = m_submitButton.Texture;
            SwapSkin(skin);
            if (GUI.Button(m_submitButton.Rect, m_submitButton.Text))
            {
                ProcessCommandline();
            }
            RevertSkinChange();

            Event currentEvent = Event.current;
            if (currentEvent.type == EventType.KeyUp) {
                if(currentEvent.keyCode == KeyCode.RightArrow) {
                    AutoComplete();
                }
            }
        }

        protected override void RenderTextInstructions(string text) {
            if(text.Length <= userInput.Length) {
                return;
            }
            Color orgColor = GUI.contentColor;
            GUI.contentColor = Color.grey;
            Rect rect = instructionsRect;
            rect.y -= INSTRUCTIONS_Y_POS_CORRECTION;
            if(m_autoCompleteOptions.Count > 0){
                string removed = text.Substring(0, userInput.Length);
                text = text.Remove(0, userInput.Length);
                rect.x += skin.label.CalcSize(new GUIContent(removed)).x;
            }
            GUI.Label(rect, text);
            GUI.contentColor = orgColor;
        }

        private void OnUserInput(string newContent) {
            m_autoCompleteOptions.Clear();
            string[] autoCompleteMatches = GetAvailableAutoCompletes(newContent);
            for (int i = 0; i < Mathf.Clamp(autoCompleteMatches.Length, 0, MAX_OPTION_COUNT); i++) {
                m_autoCompleteOptions.Add(autoCompleteMatches[i]);
            }
        }

        private string[] GetAvailableAutoCompletes(string userInput) {
            CMDCommand[] commands = CMD.Commands;
            List<string> result = new List<string>();
            userInput = userInput.Trim();

            if(userInput == string.Empty) {
                return result.ToArray();
            }

            for (int i = 0; i < commands.Length; i++) {
                string cmdName = commands[i].MethodInfo.Name;
                if (commands[i].IsHiddenInList || result.Contains(cmdName)) {
                    continue;
                }
                if(cmdName.Length >= userInput.Length) {
                    string cmdBeginning = string.Empty;
                    for (int c = 0; c < userInput.Length; c++) {
                        cmdBeginning += cmdName[c];
                    }
                    if(cmdBeginning == userInput) {
                        result.Add(cmdName);
                    }
                }
            }
            return result.ToArray();
        }

        private void AutoComplete() {
            if (AutoCompleteString != string.Empty && userInput != AutoCompleteString) {
                SetUserInput(AutoCompleteString);
                TextEditor editor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
                editor.text = userInput;
                editor.MoveTextEnd();
            }
        }

        private void ProcessCommandline() {
            if(userInput == string.Empty) { return; }

            CMD.ExecuteCommand(userInput);
            userInputHistory.Add(userInput);
            currentHistoryIndex = userInputHistory.Count;
            SetUserInput(string.Empty);
        }
    }
}