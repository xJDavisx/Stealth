using UnityEngine;

namespace DaanRuiter.CMDPlus
{
    public class CMDMobileToggler : MonoBehaviour
    {
        private static readonly Color activeColor = new Color(0, 1, 1, 0.75f);
        private static Texture2D activeTexture;
        private static readonly float buttonScreenPercentage = 7f;

        private static bool IsWindowOpen
        {
            get
            {
                return (CMD.Controller == null || CMD.Controller.View == null) ? false : CMD.Controller.View.Visible;
            }
        }

        private Rect m_leftUpperButton, m_rightUpperButton, m_leftLowerButton, m_RightLowerButton;
        private GUIStyle style;
        private float otherTapTimeStamp;
        private float maxTapTimeDiff = 2;
        private uint tapsRequiredToToggle = 3;
        private uint tapsSinceTimeout = 0;

        private void Awake()
        {
            float buttonSize = (Screen.width + Screen.height) / 2 / 100f * buttonScreenPercentage;

            m_leftUpperButton = new Rect(0, 0, buttonSize, buttonSize);
            m_rightUpperButton = new Rect(Screen.width - buttonSize, 0, buttonSize, buttonSize);
            m_leftLowerButton = new Rect(0, Screen.height - buttonSize, buttonSize, buttonSize);
            m_RightLowerButton = new Rect(Screen.width - buttonSize, Screen.height - buttonSize, buttonSize, buttonSize);

            activeTexture = new Texture2D(1, 1);

            style = GUIStyle.none;
            style.active.background = activeTexture;
            style.active.background.SetPixel(0, 0, activeColor);
            style.active.background.Apply();
        }

        private void OnGUI()
        {
            if (!IsWindowOpen)
            {
                if (GUI.Button(m_leftUpperButton, string.Empty, style))
                {
                    ValidateTap();
                }
                if (GUI.Button(m_rightUpperButton, string.Empty, style))
                {
                    ValidateTap();
                }
            }
            else
            {
                if (GUI.Button(m_leftLowerButton, string.Empty, style))
                {
                    ValidateTap();
                }
                if (GUI.Button(m_RightLowerButton, string.Empty, style))
                {
                    ValidateTap();
                }
            }
        }

        private void ValidateTap()
        {
            if (otherTapTimeStamp > 0)
            {
                otherTapTimeStamp = -1;
                tapsSinceTimeout++;
            }
            else
            {
                otherTapTimeStamp = Time.realtimeSinceStartup;
            }

            if (tapsSinceTimeout >= tapsRequiredToToggle)
            {
                Toggle();
            }
        }

        private void Toggle()
        {
            CMD.ToggleWindows();
            tapsSinceTimeout = 0;
        }

        private void Update()
        {
            if (otherTapTimeStamp > 0 && Time.realtimeSinceStartup >= otherTapTimeStamp + maxTapTimeDiff)
            {
                tapsSinceTimeout = 0;
                otherTapTimeStamp = -1;
            }
        }
    }
}