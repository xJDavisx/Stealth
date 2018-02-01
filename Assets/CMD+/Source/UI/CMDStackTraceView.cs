using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace DaanRuiter.CMDPlus {
    public class CMDStackTraceView : CMDWindow {
        private string m_currentStackTrace;
        private float m_width = 600f;
        private KeyValuePair<string, CMDEventHandler>[] m_buttons;

        private Texture2D m_buttonTexture;
        private Texture2D m_mouseOverTexture;
        private float m_buttonWidth = 150;
        private int m_topPadding = 8;

        public CMDStackTraceView(string windowName, Vector2 position, Vector2 size) : base(windowName, position, size) {
            m_buttonTexture = CMDHelper.GenerateVerticalGradientTexture(new Color[] { Color.gray * 0.65f, Color.gray * 0.85f },
                                                                        new float[] { 0.95f, 0.7f },
                                                                        1, 10);

            m_mouseOverTexture = new Texture2D(1, 1);
            m_mouseOverTexture.SetPixel(0, 0, Color.white * 0.85f);
            m_mouseOverTexture.Apply();

            skin.label.padding = new RectOffset(8, 8, m_topPadding, 8);
            skin.button.normal.textColor = Color.white * 0.9f;
            skin.button.padding.top = 2;
            skin.button.padding.left = 2;
            skin.button.normal.background = m_buttonTexture;
            skin.button.hover.background = m_mouseOverTexture;

            List<KeyValuePair<string, CMDEventHandler>> buttons = new List<KeyValuePair<string, CMDEventHandler>> {
                new KeyValuePair<string, CMDEventHandler>("Close", ToggleOff),
                new KeyValuePair<string, CMDEventHandler>("Copy", CopyStack)};
            m_buttons = buttons.ToArray();
        }
        ~CMDStackTraceView() {
            CMD.Controller.UnityInjector.AddObjectToDestroyQueue(m_buttonTexture);
            CMD.Controller.UnityInjector.AddObjectToDestroyQueue(m_mouseOverTexture);
        }

        public void ShowStackTrace(Vector2 position, string stackTrace) {
            Toggle(true);
            ToggleVisibility(true);
            rect.position = position;
            rect.size = new Vector2(m_width, skin.label.CalcHeight(new GUIContent(stackTrace), m_width));
            rect.height += m_topPadding + 3;
            m_currentStackTrace = stackTrace;
        }

        public override void OnRender() {
            base.OnRender();
            if (m_currentStackTrace == string.Empty) { return; }
            GUI.Label(rect, m_currentStackTrace);

            for (int i = 0; i < m_buttons.Length; i++) {
                Rect buttonRect = new Rect(rect.position, new Vector2(m_buttonWidth, 20f));
                buttonRect.position += new Vector2(3, 3);
                buttonRect.x +=  (i * m_buttonWidth) + (i * 3);
                if (GUI.Button(buttonRect, m_buttons[i].Key)) {
                    m_buttons[i].Value();
                }
            }
        }

        public override void Toggle(bool state) {
            base.Toggle(state);
            if (!state) {
                m_currentStackTrace = string.Empty;
            }
        }

        private new void ToggleOff() {
            base.ToggleOff();
            Toggle(false);
            ToggleVisibility(false);
        }

        private void CopyStack() {
            GUIUtility.systemCopyBuffer = m_currentStackTrace;
        }
    }
}