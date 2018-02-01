using System.Collections.Generic;
using UnityEngine;

namespace DaanRuiter.CMDPlus {
    public class CMDContextMenu : Object, ICMDRenderable {
        public CMDContextMenuInfo Context {
            get { return m_context; }
            set {
                m_context = value;
                m_rect.size = new Vector2(CONTEXT_BUTTON_WIDTH, 20f * value.Options.Count);
            }
        }
        public bool Enabled {
            get {
                return m_enabled;
            }
        }
        public bool Visible {
            get {
                return m_visible;
            }
        }
        public KeyCode ToggleKey {
            get {
                return CMDWindow.TOGGLE_DISABLED_KEYCODE;
            }
            set {}
        }
        public int ToggleMouseIndex = 1;

        public CMDEventHandler OpenRequestEvent;
        public CMDEventHandler CloseRequestEvent;

        private const float CONTEXT_BUTTON_WIDTH = 200f;
        private Texture2D m_backgroundTexture;
        private Texture2D m_altBackgroundTexture;
        private CMDContextMenuInfo m_context;
        private Rect m_rect;
        private GUISkin m_skin, m_previousSkin;
        private bool m_enabled;
        private bool m_visible;

        public CMDContextMenu(string windowName, Vector2 position, Vector2 size, GUISkin skin) {
            m_rect = new Rect(position, size);
            m_skin = skin;

            m_backgroundTexture = new Texture2D(1, 1);
            m_backgroundTexture.SetPixel(0, 0, CMDWindow.DEFAULT_WINDOW_BACKGROUND_COLOR);
            m_backgroundTexture.Apply();

            m_altBackgroundTexture = new Texture2D(1, 1);
            m_altBackgroundTexture.SetPixel(0, 0, CMDWindow.DEFAULT_WINDOW_BACKGROUND_COLOR * CMDHelper.CMD_ALT_BACKGROUND_COLOR_MODIFIER);
            m_altBackgroundTexture.Apply();

            m_skin = ScriptableObject.CreateInstance<GUISkin>();
            m_skin.button.normal.textColor = Color.white * 0.9f;
            m_skin.button.padding.top = (int)20f / 2 - (int)m_skin.button.CalcHeight(new GUIContent("Text"), CONTEXT_BUTTON_WIDTH) / 2;
            m_skin.button.padding.left = 2;
            m_skin.button.normal.background = m_backgroundTexture;
            m_skin.button.hover.background = m_altBackgroundTexture;
            m_skin.button.hover.textColor = Color.yellow;
        }
        ~CMDContextMenu ()
        {
            CMD.Controller.UnityInjector.AddObjectToDestroyQueue(m_backgroundTexture);
            CMD.Controller.UnityInjector.AddObjectToDestroyQueue(m_altBackgroundTexture);
        }

        public void OnRender() {
            if (!m_enabled) { return; }
            if (m_context == null) { return; }
            SwapSkin(m_skin);

            Vector2 mousePosition = Input.mousePosition;
            mousePosition.y = Screen.height - mousePosition.y;

            GUI.DrawTexture(m_rect, m_backgroundTexture);
            for (int i = 0; i < Context.Options.Count; i++) {
                Rect buttonRect = new Rect(m_rect);
                buttonRect.size = new Vector2(CONTEXT_BUTTON_WIDTH, 20f);
                buttonRect.y += i * buttonRect.height;
                if (GUI.Button(buttonRect, Context.Options[i].Key)) {
                    if (Context.Options[i].Value != null) {
                        Context.Options[i].Value();
                    }
                    if (CloseRequestEvent != null) {
                        CloseRequestEvent();
                    }
                    Toggle(false);
                    ToggleVisibility(false);
                }
            }

            if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1)) {
                if (!m_rect.Contains(mousePosition)) {
                    if (CloseRequestEvent != null) {
                        CloseRequestEvent();
                    }
                    Toggle(false);
                    ToggleVisibility(false);
                }
            }
            RevertSkinChange();
        }

        public void Toggle(bool state) {
            m_enabled = state;
            if (state) {
                if(OpenRequestEvent != null) {
                    OpenRequestEvent();
                }
            }
        }

        public bool Toggle() {
            Toggle(!m_enabled);
            return m_enabled;
        }

        public void ToggleVisibility(bool state) {
            m_visible = state;
        }

        public void SetSize(Vector2 size) {
            m_rect.size = size;
        }

        public void SetPosition(Vector2 position) {
            m_rect.position = position;
        }

        public void Destroy() {
            CMD.Controller.UnityInjector.AddObjectToDestroyQueue(this);
        }

        public bool OpenIfClickedIn(Rect targetRect) {
            Vector2 mousePosition = Input.mousePosition;
            mousePosition.y = Screen.height - mousePosition.y;
            //COntext
            if (targetRect.Contains(mousePosition) && Input.GetMouseButtonUp(ToggleMouseIndex)) {
                Toggle(true);
                ToggleVisibility(true);
                SetPosition(mousePosition);
                return true;
            }
            return false;
        }

        private void SwapSkin(GUISkin skin) {
            m_previousSkin = GUI.skin;
            GUI.skin = skin;
        }

        private void RevertSkinChange() {
            if (m_previousSkin != null) {
                GUI.skin = m_previousSkin;
            }
        }
    }

    public class CMDContextMenuInfo {
        public bool Active;
        public List<KeyValuePair<string, CMDEventHandler>> Options;

        public CMDContextMenuInfo(List<KeyValuePair<string, CMDEventHandler>> options) {
            Options = options;
            Active = true;
        }
    }
}