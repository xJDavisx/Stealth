using UnityEngine;

namespace DaanRuiter.CMDPlus.Examples {
    public class CMDPlus_Example_Commands : CMDPlus_Example_BaseScript {
        //Define CMDPLUS_EXAMPLES_ENABLED in ProjectSettings -> Player -> Scripting define symbols. to try these out
#if CMDPLUS_EXAMPLES_ENABLED
        [CMDCommand]
        public static void SimpleCommand() {
            Debug.Log("Executed a simple command!");
            //This is a command with the minimum requirements met to function
        }

        [CMDCommand]
        public static void ExampleMessagesTypes() {
            CMD.Log("Log message");
            CMD.Warning("Warning message");
            CMD.Error("Error message");
        }

        [CMDCommand("A description of the command")]
        public static void CommandWithDescription() {
            Debug.Log("Executed a command with a description!");
            //The description is displayed when calling the List command
        }

        [CMDCommand("Command with all supported argument types")]
        public static void CommandWithArguments(string stringArgument, float floatArgument, int intArgument, bool boolArgument, Color colorArgument) {
            Debug.Log(string.Format("Executed a command with the arguments: {0}, {1}, {2}, {3}, {4}", stringArgument, floatArgument, intArgument, boolArgument, colorArgument));
            //This command will execute with the parsed parameters given in the console.
            //The supported value types are:
            // - string
            // - float
            // - int
            // - bool
            // - color (given as a string like: R/G/B/A)
        }

        [CMDCommand("This command does not show up when calling the List command", true)]
        public static void HiddenCommand() {
            Debug.Log("Executed an hidden command");
            //This command can be called using the console but will not show up when calling the List command
            //List command is not available while CMDPLUS_EXAMPLES_ENABLED is defined
        }

        [CMDCommand("Adds a and b and multiplies the result with c")]
        public static int SimpleCalculationCommand(int a, int b, int c) {
            return (a + b) * c; //Commands that return something will automaticly print the returned object into the console
        }

        //BAD PRACTISE
        [CMDCommand("This command will not work since it it not static and/or public")]
        private void NonStaticCommand() {
            //This command will not be indexed by the console since commands need to be both public and static in order to function
        }
#endif
    }
}