using System.Collections.Generic;
using UnityEngine;

namespace DaanRuiter.CMDPlus {
    public class CMDTimer {
        internal static List<CMDTimer> TIMERS = new List<CMDTimer>();
        private static bool initialized;

        /// <summary>
        /// DONT USE - TIMERS ARE INTERNALLY ADDED
        /// </summary>
        internal static void UPDATE_TIMERS() {
            for (int i = 0; i < TIMERS.Count; i++) {
                if (!TIMERS[i].m_paused)
                    TIMERS[i].UpdateTimer();
            }
        }

        internal static void INIT() {
            if (initialized) { return; }
            CMD.Controller.UnityInjector.UnityUpdateEvent += UPDATE_TIMERS;
            initialized = true;
        }

        public delegate void OnTimerCompleteEventHandler();
        public OnTimerCompleteEventHandler OnTimerCompleteEvent;

        private float m_duration;
        private float m_startTime;
        private float m_pausedTime;

        private bool m_done;
        private bool m_paused;

        public CMDTimer(float duration, OnTimerCompleteEventHandler callback = null) {
            m_duration = duration;
            m_startTime = Time.time;
            m_done = false;

            if (callback != null)
                OnTimerCompleteEvent += callback;

            TIMERS.Add(this);
        }
        ~CMDTimer() {
            TIMERS.Remove(this);
        }
        
        public void Start() {
            m_paused = false;
            m_pausedTime = 0f;
            m_startTime = Time.time;
            m_done = false;

            if (!TIMERS.Contains(this))
                TIMERS.Add(this);
        }
        /// <summary>
        /// Completely stops the timer
        /// </summary>
        public void Stop() {
            if (TIMERS.Contains(this))
                TIMERS.Remove(this);
        }
        /// <summary>
        /// Paused the timer if it is unpaused and vice versa
        /// </summary>
        public void TogglePause() {
            m_paused = !m_paused;
            if (m_paused)
                m_pausedTime = Time.time;
            else
                m_pausedTime = 0f;
        }
        public void Pause() {
            if (!m_paused) {
                m_paused = true;
                m_pausedTime = Time.time;
            }
        }
        /// <summary>
        /// Resumes the timer if it was paused
        /// </summary>
        public void Resume() {
            m_paused = false;
            if (m_pausedTime > 0f) {
                m_startTime += m_pausedTime - m_startTime;
            }
        }
        public void Restart() {
            Start();
        }
        
        internal void UpdateTimer() {
            if (m_done)
                return;

            if (Time.time >= m_startTime + m_duration) {
                m_done = true;
                if (OnTimerCompleteEvent != null)
                    OnTimerCompleteEvent();
            } else {
                m_done = false;
            }
        }

        public bool IsDone {
            get {
                return Time.time >= m_startTime + m_duration;
            }
        }
        public float Duration {
            get {
                return m_duration;
            }
        }
    }
}