using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace DaanRuiter.CMDPlus {
    public interface ICMDRenderable{
        bool Enabled { get; }
        bool Visible { get; }
        KeyCode ToggleKey { get; set; }

        void OnRender ();
        void Toggle (bool newState);
        bool Toggle ();
        void ToggleVisibility(bool newState);
        void SetSize (Vector2 size);
        void SetPosition (Vector2 postion);
        void Destroy();
    }
    
    public class CMDWindow : Object, ICMDRenderable {
        public const KeyCode TOGGLE_DISABLED_KEYCODE = KeyCode.Joystick8Button9;
        public static readonly Color DEFAULT_WINDOW_BACKGROUND_COLOR = new Color(0.15f, 0.15f, 0.15f, 1f);
        public static readonly Color DEFAULT_WINDOW_TEXT_COLOR = new Color(1f, 1f, 1f, 1f);

        public CMDWindowToggleEventHandler WindowToggleEvent;
        /// <summary>
        /// Should the name be displayed at the top of the window
        /// </summary>
        public bool DisplayName = true;

        public Rect Rect { get { return rect; } }
        public CMDContextMenu ContextMenu { get { return contextMenu; } }
        /// <summary>
        /// Is the behaviour of the window enabled
        /// </summary>
        public bool Enabled { get { return enabled; } }
        /// <summary>
        /// is the rendering of the window enabled
        /// </summary>
        public bool Visible { get { return visible; } }

        public bool Draggable {
            get {
                return draggable;
            }set {
                draggable = value;
                if (!CMD.Initialized) { return; }
            }
        }
        public KeyCode ToggleKey {
            get { return toggleKey; }
            set {
                if (CMD.Initialized) {
                    CMD.Controller.UnityInjector.RemoveKeyEventHandler(ToggleKey, OnKeyPressEvent);
                }
                toggleKey = value;
                if (CMD.Initialized) {
                    CMD.Controller.UnityInjector.AddKeyEventHandler(ToggleKey, OnKeyPressEvent);
                }
            }
        }

        protected List<ICMDRenderable> children = new List<ICMDRenderable>();
        protected Rect rect;
        protected bool enabled;
        protected bool visible;
        protected bool focused;
        protected bool draggable;
        protected KeyCode toggleKey;
        protected GUISkin skin, previousSkin;
        protected Texture2D backgroundTexture;
        protected Texture2D altBackgroundTexture;
        protected CMDContextMenu contextMenu;
        protected new string name;
        
        private bool m_isDragging;
        private Vector2 m_dragStartPosition;
        private Vector2 m_dragStartMouseOffset;

        public CMDWindow (string windowName, Vector2 position, Vector2 size) {
            name = windowName;
            SetPosition(position);
            SetSize(size);

            Color backgroundColor = DEFAULT_WINDOW_BACKGROUND_COLOR;
            Color parsedColor;
            if (CMDHelper.TryParseColor(CMDSettings.Get<string>("COLOR_BACKGROUND_COLOR"), out parsedColor))
            {
                backgroundColor = parsedColor;
            }

            backgroundTexture = new Texture2D(1, 1);
            backgroundTexture.SetPixel(0, 0, backgroundColor);
            backgroundTexture.Apply();

            altBackgroundTexture = new Texture2D(1, 1);
            altBackgroundTexture.SetPixel(0, 0, backgroundColor * CMDHelper.CMD_ALT_BACKGROUND_COLOR_MODIFIER);
            altBackgroundTexture.Apply();

            contextMenu = new CMDContextMenu("ContextMenu", Vector2.zero, Vector2.zero, skin);
            contextMenu.ToggleKey = TOGGLE_DISABLED_KEYCODE;
            contextMenu.CloseRequestEvent += OnContextMenuClosed;
            contextMenu.OpenRequestEvent += OnContextMenuOpened;
            contextMenu.Context = new CMDContextMenuInfo(new List<KeyValuePair<string, CMDEventHandler>>() {
                new KeyValuePair<string, CMDEventHandler>("Hide", ToggleOff)
            });

            Color textColor = DEFAULT_WINDOW_TEXT_COLOR;
            if (CMDHelper.TryParseColor(CMDSettings.Get<string>("COLOR_TEXT"), out parsedColor))
            {
                textColor = parsedColor;
            }

            skin = ScriptableObject.CreateInstance<GUISkin>();
            skin.label.normal.textColor = textColor;
            skin.label.alignment = TextAnchor.LowerLeft;
            skin.label.wordWrap = true;
            skin.label.fontSize = 12;

            if (Application.isMobilePlatform)
            {
                if (CMDMobileSettings.Loaded != null)
                {
                    skin.label.fontSize = CMDMobileSettings.Loaded.TextSize;
                    skin.button.fontSize = CMDMobileSettings.Loaded.TextSize;
                }
            }

            if (!CMD.Initialized) {
                CMD.InitializedEvent += OnCMDInitialized;
            }else {
                OnCMDInitialized();
            }
        }
        ~CMDWindow ()
        {
            CMD.Controller.UnityInjector.AddObjectToDestroyQueue(backgroundTexture);
            CMD.Controller.UnityInjector.AddObjectToDestroyQueue(altBackgroundTexture);
        }

        /// <summary>
        /// Toggles the window's behaviour on if it's off and vice versa
        /// </summary>
        /// <returns>The new state</returns>
        public virtual bool Toggle () {
            Toggle(!enabled);
            return enabled;
        }

        /// <summary>
        /// Toggles the window's behaviour to the given state
        /// </summary>
        /// <param name="state">The target state</param>
        public virtual void Toggle(bool state) {
            enabled = state;
            if(WindowToggleEvent != null) {
                WindowToggleEvent(enabled);
            }
        }

        /// <summary>
        /// Toggles the window's rendering to the given state
        /// </summary>
        /// <returns>The new state</returns>
        public virtual void ToggleVisibility(bool state) {
            visible = state;
        }
        
        public void SetSize(Vector2 size) {
            rect.size = size;
        }
        public void SetPosition(Vector2 position) {
            rect.position = position;
        }

        /// <summary>
        /// Set the color for the background texture
        /// </summary>
        public void SetBackgroundColor(Color color) {
            backgroundTexture.SetPixel(0, 0, color);
            backgroundTexture.Apply();
        }

        /// <summary>
        /// Will destroy the window the next frame
        /// </summary>
        public void Destroy() {
            CMD.Controller.UnityInjector.AddObjectToDestroyQueue(this);
        }

        public virtual void OnRender () {
            GUI.skin = skin;            
            GUI.DrawTexture(rect, backgroundTexture, ScaleMode.StretchToFill, false);
            if (DisplayName) { RenderName(); }
        }

        internal void ToggleOff() {
            Toggle(false);
            ToggleVisibility(false);
        }

        protected void RenderName() {
            if (name != string.Empty) {
                Vector2 titleSize = skin.label.CalcSize(new GUIContent(name));
                GUI.Label(new Rect(rect.position + new Vector2(rect.size.x / 2f, 0f) - new Vector2(titleSize.x / 2f, 0f), titleSize), name);
            }
        }

        protected void AddChild(ICMDRenderable renderable) {
            children.Add(renderable);
        }
        protected void RemoveChild(ICMDRenderable renderable) {
            if (children.Contains(renderable)) {
                children.Remove(renderable);
            }
        }
        protected void ToggleChildren(bool state) {
            for (int i = 0; i < children.Count; i++) {
                children[i].Toggle(state);
            }
        }
        protected void ToggleVisibilyChildren(bool state) {
            for (int i = 0; i < children.Count; i++) {
                children[i].ToggleVisibility(state);
            }
        }

        protected void SwapSkin(GUISkin skin) {
            previousSkin = GUI.skin;
            GUI.skin = skin;
        }

        protected void RevertSkinChange() {
            if(previousSkin != null) {
                GUI.skin = previousSkin;
            }
        }

        protected virtual void OnContextMenuClosed() {

        }

        protected virtual void OnContextMenuOpened() {

        }

        private void OnKeyPressEvent(KeyCode key) {
            Toggle();
        }

        private void OnUnityUpdate() {
            if(!Enabled || !Visible)
            {
                return;
            }

            Vector2 convertedMousePosition = Input.mousePosition;
            convertedMousePosition.y = Screen.height - convertedMousePosition.y;

            if (Draggable) {
                if (Input.GetMouseButtonDown(0)) {
                    if (rect.Contains(convertedMousePosition)) {
                        StartDragging(convertedMousePosition);
                    }
                }
                if (m_isDragging) {
                    UpdateDraggedPosition(convertedMousePosition);
                    if (Input.GetMouseButtonUp(0)) {
                        EndDragging();
                    }
                }
            }            

            contextMenu.OpenIfClickedIn(rect);
        }

        private void OnCMDInitialized() {
            CMD.Controller.UnityInjector.UnityUpdateEvent += OnUnityUpdate;
        }

        private void StartDragging(Vector2 mousePosition) {
            m_dragStartPosition = rect.position;
            m_dragStartMouseOffset = rect.position - mousePosition;
            m_isDragging = true;
        }

        private void UpdateDraggedPosition(Vector2 mousePosition) {
            rect.position = m_dragStartPosition - (m_dragStartPosition - mousePosition) + m_dragStartMouseOffset;
        }

        private void EndDragging() {
            m_isDragging = false;
        }
    }
}