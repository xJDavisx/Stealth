using System;
using System.Collections.Generic;
using UnityEngine;

namespace DaanRuiter.CMDPlus {
    public static class CMDTweens {
        private static List<ICMDTween> tweens = new List<ICMDTween>();

        /// <summary>
        /// Add a tween to the active tween list
        /// </summary>
        /// <param name="tween">the tween to add</param>
        /// <returns>the added tween</returns>
        public static ICMDTween AddTween(ICMDTween tween) {
            tweens.Add(tween);
            return tween;
        }

        internal static void UpdateTweens() {
            ICMDTween[] killedTweens = tweens.FindAll(KilledTweens).ToArray();
            for (int i = 0; i < killedTweens.Length; i++) {
                if (killedTweens[i].KilledEvent != null) {
                    killedTweens[i].KilledEvent();
                }
                tweens.Remove(killedTweens[i]);
            }

            ICMDTween[] completedTweens = tweens.FindAll(CompletedTweens).ToArray();
            for (int i = 0; i < completedTweens.Length; i++) {
                if (completedTweens[i].CompletedEvent != null) {
                    completedTweens[i].CompletedEvent();
                }
                tweens.Remove(completedTweens[i]);
            }

            for (int i = 0; i < tweens.Count; i++) {
                tweens[i].UpdateTween();
            }
        }

        private static bool CompletedTweens(ICMDTween tween) {
            return tween.IsCompleted();
        }

        private static bool KilledTweens(ICMDTween tween) {
            return tween.IsKilled();
        }
    }

    public interface ICMDTween {
        CMDTweenEventHandler CompletedEvent { get; }
        CMDTweenEventHandler KilledEvent { get; }
        bool IsCompleted();
        bool IsKilled();
        void UpdateTween();
        void Kill();
        ICMDTween OnCompleted(CMDTweenEventHandler callback);
        ICMDTween OnKill(CMDTweenEventHandler callback);
    }

    public class CMDTween<TweenValueType> : ICMDTween where TweenValueType : struct {
        public CMDTweenEventHandler CompletedEvent { get { return m_completedEvent; } }
        public CMDTweenEventHandler KilledEvent { get { return m_killedEvent; } }

        /// <summary>
        /// Has the tween been completed
        /// </summary>
        /// <returns>if the normalized progess has surpassed 1</returns>
        public bool IsCompleted() {
            if (normalizedProgress >= 1.0f) {
                UpdateTween(1f);
            }
            return normalizedProgress >= 1.0f;
        }

        /// <summary>
        /// Has the tween been killed
        /// </summary>
        /// <returns>If the tween has been killed</returns>
        public bool IsKilled() {
            return m_isKillRequested;
        }

        private Action<TweenValueType> m_setter;
        private CMDTweenEventHandler m_completedEvent;
        private CMDTweenEventHandler m_killedEvent;
        private float m_duration;
        private float m_startTimeStamp;
        private bool m_paused;
        private bool m_isKillRequested;

        private TweenValueType m_target;
        private TweenValueType m_startValue;

        private float normalizedProgress {
            get {
                return Mathf.Clamp((Time.realtimeSinceStartup - m_startTimeStamp) / (m_duration / 100f) / 100f, 0f, 1f);
            }
        }

        /// <summary>
        /// Creates a new Tween for the given value type. IMPORTANT: make sure to add it to the active tween list using CMDTweens.AddTween(tween);
        /// </summary>
        /// <param name="valueSetCallback">This function will be called every frame with the newly tweened value</param>
        /// <param name="currentValue">The starting value of the tween</param>
        /// <param name="targetValue">The target value of the tween</param>
        /// <param name="duration">How long it should take to get from the start value to the target value</param>
        public CMDTween(Action<TweenValueType> valueSetCallback, TweenValueType currentValue, TweenValueType targetValue, float duration) {
            m_setter = valueSetCallback;
            m_target = targetValue;
            m_startValue = currentValue;
            m_startTimeStamp = Time.realtimeSinceStartup;
            SetDuration(duration);
        }

        /// <summary>
        /// Updates the value with the progress calculated using the duration and Time.realtimeSinceStartup
        /// </summary>
        public void UpdateTween() {
            UpdateTween(normalizedProgress);
        }

        /// <summary>
        /// Updates the value with the progress calculated using the given normalized progress. (0 - 1)
        /// </summary>
        /// <param name="progress">the progress to set the value to (0 - 1)</param>
        public void UpdateTween(float progress) {
            if (typeof(TweenValueType) == typeof(float)) {
                float startValue = (float)Convert.ToDouble(m_startValue);
                float target = (float)Convert.ToDouble(m_target);
                m_setter((TweenValueType)Convert.ChangeType(startValue + (target - startValue) * progress, typeof(float)));
            } 
            else if (typeof(TweenValueType) == typeof(Vector2)) {
                Vector2 startValue = (Vector2)Convert.ChangeType(m_startValue, typeof(Vector2));
                Vector2 target = (Vector2)Convert.ChangeType(m_target, typeof(Vector2));
                m_setter((TweenValueType)Convert.ChangeType(startValue + (target - startValue) * progress, typeof(Vector2)));
            } 
            else if (typeof(TweenValueType) == typeof(Vector3)) {
                Vector3 startValue = (Vector3)Convert.ChangeType(m_startValue, typeof(Vector3));
                Vector3 target = (Vector3)Convert.ChangeType(m_target, typeof(Vector3));
                m_setter((TweenValueType)Convert.ChangeType(startValue + (target - startValue) * progress, typeof(Vector3)));
            }
        }

        /// <summary>
        /// Will kill the tween on it's current value
        /// </summary>
        public void Kill() {
            m_isKillRequested = true;
        }

        /// <summary>
        /// Add an event handler to the tween's completion event
        /// </summary>
        /// <param name="callback"></param>
        /// <returns>The tween you added the event handler to</returns>
        public ICMDTween OnCompleted(CMDTweenEventHandler callback) {
            m_completedEvent += callback;
            return this;
        }

        /// <summary>
        /// Add an event handler to the tween's kill event
        /// </summary>
        /// <param name="callback"></param>
        /// <returns>The tween you added the event handler to</returns>
        public ICMDTween OnKill(CMDTweenEventHandler callback) {
            m_killedEvent += callback;
            return this;
        }

        /// <summary>
        /// Set the duration of the tween
        /// </summary>
        /// <param name="duration">the new duration for the tween</param>
        /// <returns>the tween you set the duration of</returns>
        public ICMDTween SetDuration(float duration) {
            m_duration = duration;
            if (m_duration <= default(float)) {
                m_duration = float.Epsilon;
            }
            return this;
        }
    }
}