using System;
using System.Collections.Generic;
using UnityEngine;

namespace DaanRuiter.CMDPlus
{
    public static class CMDStrings
    {
        public const string CMD_COMMAND_INPUT_INSTRUCTIONS = "Enter command..";
        public const string CMD_DATA_FOLDER_PATH = "CMD/";
        public const string CMD_SETTINGS_PATH = CMD_DATA_FOLDER_PATH + "Settings/";
        public const string CMD_LOGGER_PATH = CMD_DATA_FOLDER_PATH + "Logs/";
        public const string CMD_SETTINGS_PATH_DEFAULT = "[DEFAULT]";
        public const string CMD_UNITY_INJECTOR_GAMEOBJECT_NAME = "[CMD]";
        public const string CMD_SETTINGS_PATH_TEMPLATE_FILE = "CMD+/Resources/Templates/template.xml";
        public const string CMD_SETTINGS_PATH_DEFAULT_SETTING_FILES = "CMD+/Resources/Templates/DefaultSettings";

        public static string UnknownCommandException (string command)
        {
            return string.Format("Unknown command: {0}", command);
        }

        public static string NonSerializableTypeException (Type type)
        {
            return string.Format("Type {0} is not serializable, cannot be used as setting", type);
        }

        public static string UnkownFileOrFolder (string path)
        {
            return string.Format("The file/folder at path {0} does not exist. Please make sure the given path is correct.", path);
        }

        public static string UnknownSettingException (string settingIdentifier)
        {
            return string.Format("There is no setting with the identifier {0}.", settingIdentifier);
        }

        public static string ParameterMismatchException (CMDParameterMismatchException exception, bool useRichText = false)
        {
            string expectedString = "(";
            string givenString = "(";
            for (int i = 0; i < exception.ParameterTypes.Length; i++)
            {
                Type expectedType = exception.ParameterTypes[i].Key;
                Type givenType = exception.ParameterTypes[i].Value;

                if (useRichText)
                {
                    if (expectedType == null || givenType == null || expectedType != givenType && !givenType.IsSubclassOf(expectedType))
                    {
                        expectedString += "<color=#c61b1b>";
                        givenString += "<color=#c61b1b>";
                    } else
                    {
                        expectedString += "<color=#14e032>";
                        givenString += "<color=#14e032>";
                    }
                }

                if (expectedType != null)
                {
                    expectedString += expectedType.Name;
                }
                if (givenType != null)
                {
                    givenString += givenType.Name;
                }

                if (useRichText)
                {
                    expectedString += "</color>";
                    givenString += "</color>";
                }

                if (i + 1 < exception.ParameterTypes.Length)
                {
                    if (expectedType != null)
                    {
                        expectedString += ", ";
                    }
                    if (givenType != null)
                    {
                        givenString += ", ";
                    }
                }
            }
            expectedString += ")";
            givenString += ")";

            return string.Format("Parameter mismatch for command: {0}\r\nTypes expected: {1}\r\nTypes given: {2}", exception.MethodInfo.Name, expectedString, givenString);
        }

        public static string Command (string command, params object[] args)
        {
            string result = string.Format("~ {0}", command);

            if (args != null && args.Length > 0)
            {
                result += " (";
                for (int i = 0; i < args.Length; i++)
                {
                    result += string.Format("{0} : {1}", args[i].GetType(), args[i]);
                    if (i + 1 < args.Length)
                    {
                        result += ", ";
                    }
                }
                result += ")";
            }

            return result;
        }

        public static string LogPrefix (LogType type)
        {
            switch (type)
            {
                case LogType.Error:
                return "<b>ERROR: </b>";
                case LogType.Assert:
                return "<b>ASSERT: </b>";
                case LogType.Warning:
                return "<b>WARNING: </b>";
                case LogType.Log:
                return "";
                case LogType.Exception:
                return "<b>EXCEPTION: </b>";
                default:
                return "";
            }
        }

        public static string CleanStackTrace (string stackTrace)
        {
            List<string> split = new List<string>(stackTrace.Split('\r'));
            split.RemoveAll(UnnecessaryFrames);

            string result = string.Join(string.Empty, split.ToArray());
            return result;
        }

        private static bool UnnecessaryFrames (string frame)
        {
            string lowerCase = frame.ToLower();
            return lowerCase.Contains("environment.cs:line 227") || frame.Contains("CMD.cs");
        }
    }
}