using UnityEngine;
using UnityEngine.UI;

namespace DaanRuiter.CMDPlus {
    public class CMDPlus_Example_BaseScript : MonoBehaviour {
        [SerializeField] private Text toggleInstructionsText;

        private void Start() {
            if (toggleInstructionsText == null) { return; }

            KeyCode toggleKey = CMD.DEFAULT_TOGGLE_KEYCODE;
            if (CMDSettings.Has("TOGGLE_KEYCODE") && CMDSettings.Get<string>("TOGGLE_KEYCODE") != "[DEFAULT]")
            {
                toggleKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), CMDSettings.Get<string>("TOGGLE_KEYCODE"));
            }
            toggleInstructionsText.text = string.Format("Press <b>{0}</b> to toggle the console on/off\r\nMake sure to unselect the input bar before toggling off", toggleKey);
        }
    }
}