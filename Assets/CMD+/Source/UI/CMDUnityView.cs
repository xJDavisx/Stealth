using System;
using System.Collections.Generic;
using UnityEngine;

namespace DaanRuiter.CMDPlus {
    public interface ICMDView : ICMDRenderable {
        bool OpenOnError { get; set; }
        void RenderLogEntries(CMDLogEntry[] entries);
    }

    public class CMDUnityView : CMDWindow, ICMDView {
        public bool OpenOnError { get; set; }

        private ICMDTween m_toggleTween;

        private CMDCommandInputField m_inputField;
        private CMDStackTraceView m_stackTraceViewer;

        private CMDTimer m_scrollTimer;
        private int m_scrollPosition;
        private float windowHeight;

        public CMDUnityView(string windowName, Vector2 position, Vector2 size) : base(windowName, position, size) {
            Rect inputRect = rect;
            inputRect.position += new Vector2(0f, inputRect.height);
            inputRect.width *= 0.8f;
            inputRect.height = skin.label.fontSize * 2;

            m_inputField = new CMDCommandInputField("CommandInput", inputRect.position, inputRect.size);
            m_inputField.SetBackgroundColor(Color.black);
            m_inputField.InputInstructions = CMDStrings.CMD_COMMAND_INPUT_INSTRUCTIONS;
            m_inputField.DisplayName = false;
            
            m_stackTraceViewer = new CMDStackTraceView("StackTrace", new Vector2(0, 500), new Vector2(250, 100));
            m_stackTraceViewer.ToggleKey = TOGGLE_DISABLED_KEYCODE;
            m_stackTraceViewer.DisplayName = false;
            m_stackTraceViewer.Draggable = true;
            
            AddChild(m_inputField);
            AddChild(m_stackTraceViewer);
            AddChild(contextMenu);

            windowHeight = rect.height;
            SetSize(new Vector2(rect.width, 0f));
            
            DisplayName = false;

            CMD.InitializedEvent += OnCMDInitialized;
        }
        ~CMDUnityView() {
            CMD.InitializedEvent -= OnCMDInitialized;
            CMD.Controller.UnityInjector.UnityUpdateEvent -= OnUnityUpdate;
        }

        public void RenderLogEntries(CMDLogEntry[] entries) {
            base.OnRender();

            Array.Reverse(entries);

            if (entries.Length > 0) {
                m_scrollPosition = Mathf.Clamp(m_scrollPosition, 0, entries.Length - 1);
            } else {
                m_scrollPosition = 0;
            }

            float lineHeight = skin.label.fontSize;
            float relativeY = 0f;
            
            for (int i = m_scrollPosition; i < entries.Length; i++) {

                CMDLogEntry entry = entries[i];
                string displayContent = CMDStrings.LogPrefix(entry.Type);

                if (CMDSettings.Get<bool>("CONSOLE_DISPLAY_TIMESTAMP"))
                {
                    displayContent = entry.PostTime.ToString("HH:MM") + ": " + entry.Message;
                }
                else
                {
                    displayContent = entry.Message;
                }

                float height = skin.label.CalcHeight(new GUIContent(displayContent), rect.width);
                float y = (rect.position.y + rect.size.y) - height - relativeY;
                float xPadding = 2f;
                Rect entryRect = new Rect(rect.position.x + xPadding, y, rect.size.x - xPadding, height);

                //Alt Background
                if (i % 2 == 0) {
                    GUI.DrawTexture(entryRect, altBackgroundTexture);
                }

                //Message
                Color previousContentColor = GUI.contentColor;
                GUI.contentColor = entry.Color;
                GUI.Label(entryRect, displayContent);
                GUI.contentColor = previousContentColor;
                
                if (contextMenu.OpenIfClickedIn(entryRect)) {
                    contextMenu.Context = new CMDContextMenuInfo(new List<KeyValuePair<string, CMDEventHandler>>() {
                        new KeyValuePair<string, CMDEventHandler>("Show stacktrace", delegate() {
                            m_stackTraceViewer.ShowStackTrace(new Vector2(0f, m_inputField.Rect.y + m_inputField.Rect.height), entry.StackTrace);
                            m_inputField.Toggle(false);
                        }),
                        new KeyValuePair<string, CMDEventHandler>("Remove entry", delegate() {
                            CMD.Controller.Model.RemoveEntry(entry);
                        }),
                        new KeyValuePair<string, CMDEventHandler>("Open Script", delegate() {
                            OpenScript(entry.StackTrace);
                        })});
                }                

                relativeY += entryRect.height;
                if (relativeY + lineHeight >= rect.height) {
                    break;
                }
            }
        }

        public override void Toggle(bool state) {
            if (m_toggleTween != null) {
                m_toggleTween.Kill();
            }

            float tweenDuration = CMDSettings.Get<float>("CONSOLE_TOGGLE_TWEEN_DURATION", false);
            if (state) {
                base.ToggleVisibility(true);
                m_inputField.Toggle(true);
                m_toggleTween = CMDTweens.AddTween(new CMDTween<float>(OnToggleTweenValueSet, rect.height, windowHeight, tweenDuration).OnCompleted(OnToggleOnTweenCompleted));
            } else {
                base.Toggle(false);
                base.ToggleChildren(false);
                contextMenu.ToggleVisibility(false);
                m_stackTraceViewer.ToggleVisibility(false);

                m_toggleTween = CMDTweens.AddTween(new CMDTween<float>(OnToggleTweenValueSet, rect.height, 0f, tweenDuration).OnCompleted(OnToggleOffTweenCompleted));
            }
        }

        protected override void OnContextMenuOpened() {
            base.OnContextMenuOpened();
            m_inputField.Toggle(false);
        }

        protected override void OnContextMenuClosed() {
            base.OnContextMenuClosed();
            m_inputField.Toggle(true);
            contextMenu.Context = new CMDContextMenuInfo(new List<KeyValuePair<string, CMDEventHandler>>() {
                new KeyValuePair<string, CMDEventHandler>("Hide", ToggleOff)
            });
        }

        private void OnUnityUpdate() {
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");
            if (scrollInput > 0f) {
                m_scrollPosition++;
            } else if (scrollInput < 0f) {
                m_scrollPosition--;
            }
        }

        private void OnCMDInitialized() {
            CMD.Controller.UnityInjector.AddRenderable(m_inputField, 0);
            CMD.Controller.UnityInjector.AddRenderable(m_stackTraceViewer, 0);
            CMD.Controller.UnityInjector.AddRenderable(ContextMenu);
            CMD.Controller.UnityInjector.UnityUpdateEvent += OnUnityUpdate;
        }

        private void OnToggleOnTweenCompleted() {
            base.Toggle(true);
            m_inputField.NeedsFocusSet = true;
            m_inputField.ToggleVisibility(true);
        }

        private void OnToggleOffTweenCompleted() {
            base.ToggleVisibility(false);
            ToggleChildren(false);
            ToggleVisibilyChildren(false);
        }

        private void OnToggleTweenValueSet(float newValue) {
            SetSize(new Vector2(rect.width, newValue));
            m_inputField.SetPosition(new Vector2(0f, newValue));
        }

        private void OpenScript(string pathToScript) {
            string[] splitStack = pathToScript.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
            if (splitStack.Length == 0) { return; }

            string fromString = "in ";
            string toString = ":line ";

            string targetFrame = splitStack[1];

            int from = targetFrame.IndexOf(fromString);
            int to = targetFrame.IndexOf(toString);

            string targetFile = targetFrame.Substring(from + fromString.Length, to - from - fromString.Length);
            int lineNumber = int.Parse(targetFrame.Substring(to + toString.Length));
            CMDHelper.OpenScriptInIDE(targetFile, lineNumber);
        }
    }
}