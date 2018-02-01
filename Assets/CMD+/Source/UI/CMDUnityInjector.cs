using System.Collections.Generic;
#if UNITY_EDITOR
    using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DaanRuiter.CMDPlus {
    public class CMDUnityInjector : MonoBehaviour {
        /// <summary>
        /// Called every MonoBehaviour.OnGUI
        /// </summary>
        public OnGUIEventHandler OnGUIEvent;
        /// <summary>
        /// Called every MonoBehaviour.Update
        /// </summary>
        public CMDEventHandler UnityUpdateEvent;
        /// <summary>
        /// Called on MonoBehaviour.OnApplicationQuit
        /// </summary>
        public CMDEventHandler ApplicationQuitEvent;

        /// <summary>
        /// Should the injector GameObject be destroyed on scene switch (recommended = false)
        /// </summary>
        public bool DestroyOnLoad {
            get {
                return m_destroyOnLoad;
            }
            set {
                m_destroyOnLoad = value;
                if (!m_destroyOnLoad)
                    DontDestroyOnLoad(gameObject);
            }
        }
        /// <summary>
        /// All currently active renderables
        /// </summary>
        public ICMDRenderable[] Renderables { get { return m_renderables.ToArray(); } }

        private List<ICMDRenderable> m_renderables = new List<ICMDRenderable>();
        private List<CMDKeyPressEventHandlerInfo> m_keyEventHandlers = new List<CMDKeyPressEventHandlerInfo>();
        private Queue<Object> m_destroyQueue = new Queue<Object>();
        private bool m_destroyOnLoad;

        /// <summary>
        /// Add a renderable to the OnGUI event
        /// </summary>
        public void AddRenderable(ICMDRenderable renderable) {
            m_renderables.Add(renderable);
        }
        /// <summary>
        /// Add a renderable to the OnGUI event at a specific index
        /// </summary>
        public void AddRenderable(ICMDRenderable renderable, int index) {
            m_renderables.Insert(index, renderable);
        }

        /// <summary>
        /// Remove a renderable from the OnGUI event
        /// </summary>
        /// <param name="renderable"></param>
        public void RemoveRenderable(ICMDRenderable renderable) {
            m_renderables.Remove(renderable);
        }

        /// <summary>
        /// Find a rendrerable of the given type
        /// </summary>
        /// <param name="type">Type desired ICMDRenderable object</param>
        public ICMDRenderable FindRenderable(System.Type type) {
            for (int i = 0; i < m_renderables.Count; i++) {
                if(m_renderables[i].GetType() == type) {
                    return m_renderables[i];
                }
            }
            return null;
        }

        /// <summary>
        /// Returns the index of a renderable
        /// </summary>
        public int GetRenderIndexOf(ICMDRenderable renderable) {
            return System.Array.IndexOf(m_renderables.ToArray(), renderable);
        }

        /// <summary>
        /// Add a callback to a specific key press
        /// </summary>
        public void AddKeyEventHandler(KeyCode keyCode, KeyPressEventHandler callBack) {
            m_keyEventHandlers.Add(new CMDKeyPressEventHandlerInfo(keyCode, callBack));
        }

        /// <summary>
        /// Remove a callback to a specific key press
        /// </summary>
        public void RemoveKeyEventHandler(KeyCode keyCode, KeyPressEventHandler callBack) {
            int removeIndex = -1;
            for (int i = 0; i < m_keyEventHandlers.Count; i++) {
                if(m_keyEventHandlers[i].KeyCode == keyCode && m_keyEventHandlers[i].CallBack == callBack) {
                    removeIndex = i;
                    break;
                }
            }
            if(removeIndex >= 0) {
                m_keyEventHandlers.RemoveAt(removeIndex);
            }
        }

        /// <summary>
        /// Add an UnityEngine.Object to the destroy queue to be destroyed next frame
        /// </summary>
        public void AddObjectToDestroyQueue(Object item) {
            m_destroyQueue.Enqueue(item);
        }

        private void Awake () {
            DestroyOnLoad = false;
            SceneManager.activeSceneChanged += OnActiveLevelChanged;

            gameObject.AddComponent<CMDMobileToggler>();
             if (Application.isMobilePlatform)
             {
#if UNITY_EDITOR
                if (Application.isEditor && (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android && EditorUserBuildSettings.activeBuildTarget != BuildTarget.iOS))
                {
                    return;
                }
 #endif
                gameObject.AddComponent<CMDMobileToggler>();
                gameObject.AddComponent<DeviceChange>();
             }

        }

        private void Update() {
            if(UnityUpdateEvent != null) {
                UnityUpdateEvent();
            }
            for (int i = 0; i < m_destroyQueue.Count; i++) {
                Destroy(m_destroyQueue.Dequeue());
            }
        }

        private void OnGUI () {
            if (CMDHelper.UnityDefaultSkinRef == null) {
                CMDHelper.UnityDefaultSkinRef = GUI.skin; //GUI.skin can only be accessed in OnGUI
            }
            if (OnGUIEvent != null) {
                OnGUIEvent();
            }
            for (int i = 0; i < m_renderables.Count; i++) {
                if (m_renderables[i].Visible) {
                    m_renderables[i].OnRender();
                }
            }
            HandleInput(); //Unity event doesn't seem to work inside of Update
        }

        private void HandleInput() {
            Event currentEvent = Event.current;
            if (currentEvent != null) {
                for (int i = 0; i < m_keyEventHandlers.Count; i++) {
                    CMDKeyPressEventHandlerInfo handler = m_keyEventHandlers[i];
                    if (currentEvent.type == EventType.KeyDown && currentEvent.keyCode == handler.KeyCode) {
                        if (handler.CallBack != null) {
                            handler.CallBack(handler.KeyCode);
                        }
                    }
                }
            }
        }

        private void OnActiveLevelChanged (Scene currentScene, Scene newScene)
        {
            if (m_destroyOnLoad) {
                Destroy(gameObject);
            }
        }

        private void OnApplicationQuit() {
            if(ApplicationQuitEvent != null) {
                ApplicationQuitEvent();
            }
        }
    }

    /// <summary>
    /// Used for CMD keypress events
    /// </summary>
    public struct CMDKeyPressEventHandlerInfo {
        public KeyCode KeyCode;
        public KeyPressEventHandler CallBack;
        public CMDKeyPressEventHandlerInfo(KeyCode keyCode, KeyPressEventHandler callBack) {
            KeyCode = keyCode;
            CallBack = callBack;
        }
    }
}