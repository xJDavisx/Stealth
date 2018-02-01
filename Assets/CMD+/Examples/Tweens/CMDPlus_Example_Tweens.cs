using UnityEngine;

namespace DaanRuiter.CMDPlus.Examples {
    public class CMDPlus_Example_Tweens : CMDPlus_Example_BaseScript {
        //Define CMDPLUS_EXAMPLES_ENABLED in ProjectSettings -> Player -> Scripting define symbols. to try these out
#if CMDPLUS_EXAMPLES_ENABLED
        private static CMDLogEntry m_floatLogEntry;
        private static CMDLogEntry m_vector3LogEntry;
        private static ICMDTween m_vector3Tween;

        /// <summary>
        /// Example on floating a float
        /// </summary>
        [CMDCommandButton("Start float tween", "1/1/0.6/1")]
        public static void StartFloatTween() {
            CMDTweens.AddTween(new CMDTween<float>(OnFloatTweenedValueUpdated, 0f, 100, 5f)).OnCompleted(OnFloatTweenCompleted);
            m_floatLogEntry = CMD.Log("Tweened float value: ");
        }

        private static void OnFloatTweenCompleted() {
            m_floatLogEntry.SetColor(Color.green);
        }

        private static void OnFloatTweenedValueUpdated(float newValue) {
            m_floatLogEntry.Message = "Tweened float value: " + newValue;
        }

        /// <summary>
        /// Example on floating a Vector3 (Vector2 is also supported)
        /// </summary>
        [CMDCommandButton("Start Vector3 tween", "1/1/0.6/1")]
        public static void StartVector3Tween() {
            m_vector3Tween = CMDTweens.AddTween(new CMDTween<Vector3>(OnVector3TweenedValueUpdated, Vector3.zero, new Vector3(15, 200, 2), 3f))
                .OnCompleted(OnVector3TweenCompleted)
                .OnKill(OnVector3TweenKilled);
            m_vector3LogEntry = CMD.Log("Tweened vector value: ");
        }

        [CMDCommandButton("Kill Vector3 tween", "1/0.45/0.35/1")]
        public static void KillVector3Tween() {
            if(m_vector3Tween != null) {
                m_vector3Tween.Kill();
            }
        }

        private static void OnVector3TweenCompleted() {
            m_vector3LogEntry.SetColor(Color.green);
        }

        private static void OnVector3TweenKilled() {
            m_vector3LogEntry.SetColor(Color.red);
        }

        private static void OnVector3TweenedValueUpdated(Vector3 newValue) {
            m_vector3LogEntry.Message = "Tweened vector3 value: " + newValue;
        }
#endif
    }
}