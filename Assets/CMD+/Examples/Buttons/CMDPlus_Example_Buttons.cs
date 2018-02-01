using UnityEngine;

namespace DaanRuiter.CMDPlus.Examples {
    public class CMDPlus_Example_Buttons : CMDPlus_Example_BaseScript {
        //Define CMDPLUS_EXAMPLES_ENABLED in ProjectSettings -> Player -> Scripting define symbols. to try these out
#if CMDPLUS_EXAMPLES_ENABLED
        [CMDCommandButton("Simple Button")]
        public static void SimpleButton() {
            Debug.Log("Pressed simple button!");
            //The button is automaticly indexed and added to the CMDButtonWindow
        }

        [CMDCommandButton("Red Button", "1/0/0/1")] //Color format = Red/Green/Blue/Alpha (between 0 - 1)
        public static void RedButton() {
            CMD.Log("Pressed red button!").SetColor(Color.red);
            //You can pass a color for the background as a string. the string will be casted to a color and applied to the background texture
            //The reason it's a string and not a color is that Color is not among the supported C# attribute parameter types.
        }

        [CMDCommandButton("Command/Button")]
        [CMDCommand("This command also has a button")]
        public static void CombinedCommandButton() {
            Debug.Log("Pressed button/executed-command!");
            //You can also stack the attributes to make these functions both commands as well as have buttons to call them
            //Just make sure that if you do this, the function is void & has no parameters
        }

        //BAD PRACTISE
        [CMDCommandButton("This button will not work since it it not static and/or public")]
        private void NonStaticButton() {
            //This button will not be indexed by the console since button need to be both public and static in order to function
        }
#endif
    }
}