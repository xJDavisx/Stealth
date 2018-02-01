using System.Collections.Generic;
using UnityEngine;

namespace DaanRuiter.CMDPlus {
    public class CMDInputField : CMDWindow {
        public const float INSTRUCTIONS_Y_POS_CORRECTION = 2f;
        public const float INSTRUCTIONS_X_POS_CORRECTION = -4f;

        public string InputInstructions;
        public string Input { get { return userInput; } }
        public bool NeedsFocusSet;

        protected CMDInputFieldContentChangeEventHandler UserInputChangeEvent;
        protected CMDEventHandler UserInputSubmitEvent;

        protected string userInput = string.Empty;
        protected List<string> userInputHistory;
        protected int currentHistoryIndex;
        protected Rect instructionsRect;

        public CMDInputField(string windowName, Vector2 position, Vector2 size) : base(windowName, position, size) {
            userInputHistory = new List<string>();
            UpdateInstructionsTextRect();
        }

        public override void OnRender() {
            base.OnRender();
            if (!enabled) { return; }

            SwapSkin(CMDHelper.UnityDefaultSkinRef);

            GUI.SetNextControlName(name);
            string tempUserInput = GUI.TextField(rect, userInput, 100);
            if (NeedsFocusSet) {
                GUI.FocusControl(name);
                NeedsFocusSet = false;
            }

            if (tempUserInput != userInput) {
                if(UserInputChangeEvent != null) {
                    UserInputChangeEvent(tempUserInput);
                }
            }
            userInput = tempUserInput;
            RevertSkinChange();

            UpdateInstructionsTextRect();
            if (userInput == string.Empty) {
                RenderTextInstructions(InputInstructions);
            }

            if(GUI.GetNameOfFocusedControl() != name) { return; }
            
            Event currentEvent = Event.current;
            KeyEvent(currentEvent.keyCode, currentEvent.type);
        }
        
        public void SetUserInput(string newUserInput) {
            userInput = newUserInput;
            if(UserInputChangeEvent != null) {
                UserInputChangeEvent(userInput);
            }
        }

        protected virtual void KeyEvent(KeyCode keyCode, EventType eventType) {
            switch (keyCode) {
                case KeyCode.Return:
                if (userInput != string.Empty) {
                    if (UserInputSubmitEvent != null) {
                        UserInputSubmitEvent();
                    }
                }
                break;
                case KeyCode.UpArrow:
                if (eventType == EventType.KeyUp) {
                    if (currentHistoryIndex - 1 >= 0) {
                        currentHistoryIndex--;
                    }
                    if (userInputHistory.Count > 0 && currentHistoryIndex < userInputHistory.Count) {
                        if (userInputHistory[currentHistoryIndex] != string.Empty) {
                            userInput = userInputHistory[currentHistoryIndex].Trim();
                            if (UserInputChangeEvent != null) {
                                UserInputChangeEvent(userInput);
                            }
                        }
                    }
                }
                break;
                case KeyCode.DownArrow:
                if (eventType == EventType.KeyUp) {
                    if (currentHistoryIndex + 1 < userInputHistory.Count) {
                        currentHistoryIndex++;
                    }
                    if (userInputHistory.Count > 0 && currentHistoryIndex < userInputHistory.Count) {
                        if (userInputHistory[currentHistoryIndex] != string.Empty) {
                            userInput = userInputHistory[currentHistoryIndex].Trim();
                            if (UserInputChangeEvent != null) {
                                UserInputChangeEvent(userInput);
                            }
                        }
                    }
                }
                break;
            }
        }

        protected virtual void RenderTextInstructions(string text) {
            Color orgColor = GUI.contentColor;
            GUI.contentColor = Color.grey;
            Rect rect = instructionsRect;
            rect.y -= INSTRUCTIONS_Y_POS_CORRECTION;
            GUI.Label(rect, text);
            GUI.contentColor = orgColor;
        }

        private void UpdateInstructionsTextRect() {
            instructionsRect = new Rect(rect);
            instructionsRect.height /= 1.25f;
            instructionsRect.x += 4f;
        }
    }
}