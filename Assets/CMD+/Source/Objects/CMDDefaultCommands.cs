using System;
using System.Reflection;
using UnityEngine;

namespace DaanRuiter.CMDPlus {
    public static class CMDDefaultCommands {
#if !CMDPLUS_EXAMPLES_ENABLED
        [CMDCommand("", true)]
        public static void Help() {
            List();
        }

        [CMDCommandButton("List Commands", "0.65/0.65/1/1")]
        [CMDCommand("List all commands")]
        public static void List() {
            CMDCommand[] commands = CMD.Commands;

            for (int i = 0; i < commands.Length; i++) {
                if (commands[i].IsHiddenInList) { continue; }
                string commandName = commands[i].MethodInfo.Name;
                string parametersString = string.Empty;
                string description = commands[i].Description;

                ParameterInfo[] parameters = commands[i].MethodInfo.GetParameters();
                for (int p = 0; p < parameters.Length; p++) {
                    parametersString += "<color=#9ddba5>" + parameters[p].ParameterType.Name + "</color>";
                    parametersString += " <color=#859386>" + parameters[p].Name + "</color>";
                    if (parameters[p].IsOptional) {
                        parametersString += " =  <color=#c7d1e0>" + parameters[p].DefaultValue + "</color>";
                    }
                    if(p + 1 < parameters.Length) {
                        parametersString += ", ";
                    }
                }

                CMD.Log(string.Format("{0}({1}) - <color=#7788aa><i>{2}</i></color>", commandName, parametersString, description));

            }
        }

        [CMDCommandButton("Clear Log", "1/0.8/0.8/1")]
        [CMDCommand("Clear the console log entries")]
        public static void Clear() {
            CMD.Controller.Model.ClearEntries();
        }

        [CMDCommand("Set the value of an existing setting or create a new entry if it doesn't exist yet")]
        public static void SetSetting(string identifier, object data, string settingFile) {
            CMDSettings.Set(identifier, data, settingFile);
        }

        [CMDCommandButton("About")]
        [CMDCommand("About the console")]
        public static void About() {
            CMD.Log("CMD+ | ver: " + CMD.VERSION);
            CMD.Log("By: Daan Ruiter");
            CMD.Log("daanruiter.net");
            CMD.Log("contact@daanruiter.net");
        }

        [CMDCommandButton("System Info")]
        [CMDCommand("Displays information about the system & hardware")]
        public static void SystemInfo() {
            CMD.Log("Device name: <b>" + UnityEngine.SystemInfo.deviceName + "</b>");
            CMD.Log("Device model: <b>" + UnityEngine.SystemInfo.deviceModel + "</b>");
            CMD.Log("Device type: <b>" + UnityEngine.SystemInfo.deviceType + "</b>");
            CMD.Log("Operating system: <b>" + UnityEngine.SystemInfo.operatingSystem + "</b>");
            CMD.Log("System memory: <b>" + UnityEngine.SystemInfo.systemMemorySize + " MB</b>");
            CMD.Log("Graphics memory: <b>" + UnityEngine.SystemInfo.graphicsMemorySize + "MB</b>");
            CMD.Log("CPU cores: <b>" + UnityEngine.SystemInfo.processorCount + " cores</b>");
            CMD.Log("CPU fequency: <b>" + UnityEngine.SystemInfo.processorFrequency + "MHz</b>");
            CMD.Log("Graphics API: <b>" + UnityEngine.SystemInfo.graphicsDeviceType + "</b>");
            CMD.Log("Graphics device: <b>" + UnityEngine.SystemInfo.graphicsDeviceName + "</b>");
        }
        
        [CMDCommand("", true)]
        public static void Hello() {
            CMD.Log(string.Format("Hi there {0}!", Environment.MachineName));
        }

        [CMDCommand("Saves all log entries to a txt file")]
        public static void SaveLogs(string fileName) {
            string path = Application.dataPath + "/../" + CMDStrings.CMD_LOGGER_PATH + fileName;
            string[] splitPath = path.Split('.');
            if(splitPath.Length == 1) {
                if(splitPath[0] != ".txt") {
                    path.Remove(path.Length - splitPath[0].Length, splitPath[0].Length);
                    path += ".txt";
                }
            }else {
                path += ".txt";
            }
            CMDLogger.SaveEntriesToFile(CMD.Controller.Model.Entries, path);
        }

        [CMDCommandButton("Save Settings")]
        [CMDCommand("Save all settings to their respective XML files")]
        public static void SaveSettings() {
            CMDSettings.SaveSettings(Application.dataPath + "/../" + CMDStrings.CMD_SETTINGS_PATH, true);
        }

        [CMDCommandButton("Close")]
        public static void CloseView() {
            CMD.Controller.View.Toggle(false);
            CMD.Controller.UnityInjector.FindRenderable(typeof(CMDButtonWindow)).Toggle(false);
        }

        [CMDCommand("List all current renderables")]
        public static void ListRenderables() {
            ICMDRenderable[] renderables = CMD.Controller.UnityInjector.Renderables;
            for (int i = 0; i < renderables.Length; i++) {
                CMD.Log(string.Format("Type: {0} | Enabled: {1} | Visible: {2} | Toggle Key: {3}", renderables[i].GetType().Name, renderables[i].Enabled, renderables[i].Visible, renderables[i].ToggleKey));
            }
        }
#endif
    }
}