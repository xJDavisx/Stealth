using System;
using System.Collections.Generic;
using UnityEngine;

namespace DaanRuiter.CMDPlus {
    public class CMDController {
        /// <summary>
        /// The currently active view of the console
        /// </summary>
        public ICMDView View { get; private set; }
        /// <summary>
        /// The currently active datamodel of the console
        /// </summary>
        public CMDModel Model { get; private set; }
        /// <summary>
        /// The currently active Unity hook of the console
        /// </summary>
        public CMDUnityInjector UnityInjector { get; private set; }

        public CMDController(ICMDView view, CMDModel model) {
            View = view;
            Model = model;

            UnityInjector = CMDHelper.GenerateUnityInjector();
            UnityInjector.OnGUIEvent += OnGUIEvent;
            UnityInjector.ApplicationQuitEvent += OnApplicationQuit;

            CMD.LogEntryRequestEvent += OnEntryRequestEvent;
            CMD.WindowCreateEvent += OnWindowCreateEvent;
            CMD.CommandExecutionEvent += OnCommandExecutionEvent;

            if (CMDSettings.Has("ATTACH_TO_UNITY_CONSOLE") && CMDSettings.Get<bool>("ATTACH_TO_UNITY_CONSOLE")) {
                Application.logMessageReceived += OnLogMessageReceived;
            }
        }
        ~CMDController () {
            CMD.LogEntryRequestEvent -= OnEntryRequestEvent;
            CMD.WindowCreateEvent -= OnWindowCreateEvent;
            CMD.CommandExecutionEvent -= OnCommandExecutionEvent;
            Application.logMessageReceived -= OnLogMessageReceived;
            UnityInjector.OnGUIEvent -= OnGUIEvent;
            UnityInjector.ApplicationQuitEvent -= OnApplicationQuit;
        }

        private CMDLogEntry OnEntryRequestEvent(LogType type, string message, string stackTrace) {
            if (type == LogType.Error || type == LogType.Exception) {
                if (View != null && View.OpenOnError && !View.Enabled) {
                    View.Toggle(true);
                }
            }
            return Model.CreateEntry(type, message, false, CMDStrings.CleanStackTrace(stackTrace));
        }

        private CMDWindow OnWindowCreateEvent(string windowName, Vector2 position, Vector2 size) {
            CMDWindow window = new CMDWindow(windowName, position, size);
            CMDHelper.SetupWindow(window);
            return window;
        }

        private KeyValuePair<Exception, CMDLogEntry> OnCommandExecutionEvent (string commandName, string stackTrace, params object[] args) {
            CMDCommand command = CMD.FindCommand(commandName);
            stackTrace = CMDStrings.CleanStackTrace(stackTrace);

            if(command != null) {
                bool parametersMatch = CMDHelper.CompareParameters(command.MethodInfo, args);
                if (!parametersMatch) {
                    return new KeyValuePair<Exception, CMDLogEntry>(CMDHelper.GenerateParamMismatchException(command.MethodInfo, args), null);
                }
                CMDLogEntry entry = Model.CreateEntry(LogType.Log, CMDStrings.Command(commandName, args), true, stackTrace);

                Color parsedColor;
                if(CMDHelper.TryParseColor(CMDSettings.Get<string>("COLOR_COMMAND_NAME_EXECUTION"), out parsedColor)) {
                    entry.SetColor(parsedColor);
                }else {
                    entry.SetColor(Color.green);
                }

                if(command.MethodInfo.ReturnType == typeof(void)) {
                    command.MethodInfo.Invoke(null, args);
                } else {
                    CMD.Log(command.MethodInfo.Invoke(null, args));
                }
                return new KeyValuePair<Exception, CMDLogEntry>(null, entry);
            }
            return new KeyValuePair<Exception, CMDLogEntry>(
                new CMDUnkownCommandException(commandName), 
                Model.CreateEntry(LogType.Log, CMDStrings.Command(commandName, args), true, stackTrace).SetColor(Color.red));
        }

        private void OnLogMessageReceived(string condition, string stackTrace, LogType type) {
            OnEntryRequestEvent(type, condition, stackTrace);
        }

        private void OnGUIEvent() {
            View.RenderLogEntries(Model.Entries);
        }

        private void OnApplicationQuit() {
            if (CMDSettings.Has("SAVE_LOGS_ON_EXIT") && CMDSettings.Get<bool>("SAVE_LOGS_ON_EXIT")) {
                CMDLogger.SaveEntriesToFile(Model.Entries, Application.dataPath + "/../" + CMDStrings.CMD_LOGGER_PATH + "log " + 
                    DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss-tt") + ".txt");
            }
        }
    }
}