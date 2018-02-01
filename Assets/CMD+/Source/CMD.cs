using DaanRuiter.CMDPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using CMDPlus;

/// <summary>
/// Main API class for the console
/// </summary>
public static class CMD
{
	public static KeyCode DEFAULT_TOGGLE_KEYCODE
	{
		get
		{
			if (Application.platform == RuntimePlatform.OSXPlayer ||
				Application.platform == RuntimePlatform.OSXEditor)
			{
				return KeyCode.Tab;
			}
			return KeyCode.BackQuote;
		}
	}
	public static readonly CMDVersion VERSION = new CMDVersion(1, 1, 0, 0);

	public static CMDLogEntryRequestEventHandler LogEntryRequestEvent;
	public static CMDCommandExecutionEventHandler CommandExecutionEvent;
	public static CMDWindowCreateRequestEventHandler WindowCreateEvent;
	public static CMDEventHandler InitializedEvent;

	private static CMDButtonWindow buttonWindow;

	/// <summary>
	/// Currently active controller for the console
	/// </summary>
	public static CMDController Controller
	{
		get
		{
#if !CMDPLUS_EXCLUDED_IN_BUILD
			return controller;
#else
            return null;
#endif
		}
	}
	/// <summary>
	/// Indexed CMDCommands by RefreshCommands()
	/// </summary>
	public static CMDCommand[] Commands
	{
		get
		{
#if !CMDPLUS_EXCLUDED_IN_BUILD
			if (!Initialized)
			{
				Init();
			}
			return commands.ToArray();
#else
            return null;
#endif
		}
	}
	/// <summary>
	/// Has the CMD been initialized, either call CMD.Init() or call one of the public static methods to automaticly initialize
	/// </summary>
	public static bool Initialized
	{
		get
		{
#if !CMDPLUS_EXCLUDED_IN_BUILD
			return controller != null;
#else
            return false;
#endif
		}
	}

#if !CMDPLUS_EXCLUDED_IN_BUILD
	private static CMDController controller;
	private static List<CMDCommand> commands;
	private static bool initializing;
#endif

	/// <summary>
	/// Initialize the console, will search for commands & buttons through the codebase if RefreshCommands() has not yet been called.
	/// </summary>
	public static void Init()
	{
#if !CMDPLUS_EXCLUDED_IN_BUILD
		if (!Application.isPlaying || Initialized || initializing)
		{
			return;
		}
		initializing = true;

#if UNITY_EDITOR
		CMDHelper.CopyDefaultSettingsIfNeeded();
#endif
		CMDSettings.LoadSettings();

		Debug.Log("INIT");

		KeyCode toggleKey = DEFAULT_TOGGLE_KEYCODE;
		if (CMDSettings.Has("TOGGLE_KEYCODE") && CMDSettings.Get<string>("TOGGLE_KEYCODE") != "[DEFAULT]")
		{
			toggleKey = (KeyCode)Enum.Parse(typeof(KeyCode), CMDSettings.Get<string>("TOGGLE_KEYCODE"));
		}

		float heightPercentage = 0.7f;
		if (CMDSettings.Has("CONSOLE_SCREEN_HEIGHT_PERCENTAGE"))
		{
			heightPercentage = CMDSettings.Get<float>("CONSOLE_SCREEN_HEIGHT_PERCENTAGE");
		}

		controller = new CMDController(new CMDUnityView("Console", Vector2.zero, new Vector2(Screen.width * 0.7f, Screen.height * heightPercentage)), new CMDModel());
		controller.View.ToggleKey = toggleKey;
		(controller.View as CMDWindow).Draggable = false;
		controller.View.OpenOnError = CMDSettings.Get<bool>("CONSOLE_OPEN_ON_ERROR");

		buttonWindow = new CMDButtonWindow("Buttons", new Vector2(Screen.width * 0.70f, 0f), new Vector2(Screen.width * 0.30f, Screen.height * heightPercentage));
		CMDHelper.SetupWindow(buttonWindow);
		buttonWindow.Toggle(false);
		buttonWindow.ToggleVisibility(false);
		buttonWindow.ToggleKey = toggleKey;
		buttonWindow.Draggable = false;

		Controller.UnityInjector.UnityUpdateEvent += CMDTweens.UpdateTweens;

		CMDTimer.INIT();

#if !CMDPLUS_EXAMPLES_ENABLED
        if (commands == null) {
            RefreshCommands();
        }
#else
		RefreshCommands(new Assembly[1] { Assembly.GetAssembly(typeof(CMD)) });
#endif

		if (InitializedEvent != null)
		{
			InitializedEvent();
		}

		initializing = false;
#endif
	}

#if !CMDPLUS_EXAMPLES_ENABLED
    [CMDCommandButton("Refresh Commands")]
    [CMDCommand("Searches through the CMD assembly for commands")]
    public static void RefreshCommands() {
        RefreshCommands(new Assembly[] { Assembly.GetAssembly(typeof(CMD)) });
    }
#endif

	/// <summary>
	/// Searches through the given assemblies for commands and indexes them.
	/// </summary>
	/// <param name="assemblies">The assemblyes to search through</param>
	/// <returns>The amount of commands found. will return -1 if the console has been disabled</returns>
	public static int RefreshCommands(Assembly[] assemblies)
	{
#if !CMDPLUS_EXCLUDED_IN_BUILD
		float startTime = Time.realtimeSinceStartup;
		commands = new List<CMDCommand>();
		commands.AddRange(CMDHelper.FindMethodAttributes<CMDCommand>(assemblies));
		commands = commands.OrderBy(c => c.MethodInfo.Name).ToList();

		Log("Refreshed Commands in " + (Time.realtimeSinceStartup - startTime).ToString("0.000") + " seconds");
		return commands.Count;
#else
        return -1;
#endif
	}

	/// <summary>
	/// Executes a public static method like it where a CMDCommand
	/// </summary>
	/// <param name="command">The public static method to execute</param>
	/// <param name="args">The parameters for the method</param>
	/// <returns>The message stating that the command has been executed</returns>
	public static CMDLogEntry ExecuteCommand(MethodInfo command, params object[] args)
	{
#if !CMDPLUS_EXCLUDED_IN_BUILD
		if (!Initialized)
		{
			Init();
		}

		if (LogEntryRequestEvent != null)
		{
			KeyValuePair<Exception, CMDLogEntry> result = CommandExecutionEvent(command.Name, Environment.StackTrace, args);
			if (result.Key != null)
			{
				Error(result.Key);
			}
			return result.Value;
		}
#endif
		return new CMDLogEntry(LogType.Log, "CMD_DISABLED");
	}

	/// <summary>
	/// Executes a public static method like it where a CMDCommand
	/// </summary>
	/// <param name="command">Name of the command to be executed</param>
	/// <param name="args">The parameters for the method</param>
	/// <returns>The message stating that the command has been executed</returns>
	public static CMDLogEntry ExecuteCommand(string command, params object[] args)
	{
#if !CMDPLUS_EXCLUDED_IN_BUILD
		if (!Initialized)
		{
			Init();
		}

		if (LogEntryRequestEvent != null)
		{
			KeyValuePair<Exception, CMDLogEntry> result = CommandExecutionEvent(command, Environment.StackTrace, args);
			if (result.Key != null)
			{
				Error(result.Key);
			}
			return result.Value;
		}
#endif
		return new CMDLogEntry(LogType.Log, "CMD_DISABLED");
	}

	/// <summary>
	/// Splits the given string on spaces and executes the first string as the method and the rest as it's parameters
	/// </summary>
	/// <param name="commandLine">The command line to be split on spaces. the first word should be the command name while the rest should be it's parameters</param>
	/// <returns>The message stating that the command has been executed</returns>
	public static CMDLogEntry ExecuteCommand(string commandLine)
	{
#if !CMDPLUS_EXCLUDED_IN_BUILD
		string[] splitComandLine = commandLine.Split(' ');
		if (splitComandLine.Length == 0)
		{
			return null;
		}

		string[] parameters = new string[splitComandLine.Length - 1];
		for (int i = 0; i < parameters.Length; i++)
		{
			parameters[i] = splitComandLine[i + 1];
		}
		return ExecuteCommand(splitComandLine[0], CMDHelper.CastParameters(parameters));
#else
        return new CMDLogEntry(LogType.Log, "CMD_DISABLED");
#endif
	}

	/// <summary>
	/// Logs a message to the CMD+'s console
	/// </summary>
	/// <param name="message">Message to log</param>
	/// <returns>the created log entry</returns>
	public static CMDLogEntry Log(object message)
	{
#if !CMDPLUS_EXCLUDED_IN_BUILD
		if (!Initialized && !initializing)
		{
			Init();
		}
		if (LogEntryRequestEvent != null)
		{
			return LogEntryRequestEvent(LogType.Log, message.ToString(), Environment.StackTrace);
		}
#endif
		return new CMDLogEntry(LogType.Log, "CMD_DISABLED");
	}

	/// <summary>
	/// Logs a warning to the CMD+'s console
	/// </summary>
	/// <param name="message">Message to log</param>
	/// <returns>the created log entry</returns>
	public static CMDLogEntry Warning(object message)
	{
#if !CMDPLUS_EXCLUDED_IN_BUILD
		if (!Initialized && !initializing)
		{
			Init();
		}

		if (LogEntryRequestEvent != null)
		{
			return LogEntryRequestEvent(LogType.Warning, message.ToString(), Environment.StackTrace);
		}
#endif
		return new CMDLogEntry(LogType.Log, "CMD_DISABLED");
	}

	/// <summary>
	/// Logs an error to the CMD+'s console. Will toggle on if the setting CONSOLE_OPEN_ON_ERROR is true
	/// </summary>
	/// <param name="message">Error to display/log</param>
	/// <returns>the created log entry</returns>
	public static CMDLogEntry Error(object message)
	{
#if !CMDPLUS_EXCLUDED_IN_BUILD
		if (!Initialized && !initializing)
		{
			Init();
		}

		if (LogEntryRequestEvent != null)
		{
			return LogEntryRequestEvent(LogType.Error, message.ToString(), Environment.StackTrace);
		}
#endif
		return new CMDLogEntry(LogType.Log, "CMD_DISABLED");
	}

	/// <summary>
	/// Logs an Exception to the CMD+'s console. Will toggle on if the setting CONSOLE_OPEN_ON_ERROR is true
	/// </summary>
	/// <param name="exception">Exception to display/log</param>
	/// <returns>the created log entry</returns>
	public static CMDLogEntry Exception(Exception exception)
	{
#if !CMDPLUS_EXCLUDED_IN_BUILD
		if (!Initialized && !initializing)
		{
			Init();
		}

		if (LogEntryRequestEvent != null)
		{
			return LogEntryRequestEvent(LogType.Exception, exception.Message, Environment.StackTrace);
		}
#endif
		return new CMDLogEntry(LogType.Log, "CMD_DISABLED");
	}

	/// <summary>
	/// Creates a new CMDWindow and adds it to the UnityInjector's update/render loops
	/// </summary>
	/// <param name="windowName">Name of the window</param>
	/// <param name="position">Position of the window</param>
	/// <param name="size">Size of the window</param>
	/// <returns>The created window</returns>
	public static CMDWindow CreateWindow(string windowName, Vector2 position, Vector2 size)
	{
#if !CMDPLUS_EXCLUDED_IN_BUILD
		if (!Initialized && !initializing)
		{
			Init();
		}

		if (WindowCreateEvent != null)
		{
			return WindowCreateEvent(windowName, position, size);
		}
#endif
		return new CMDWindow("CMD_DISABLED", Vector2.zero, Vector2.zero);
	}

	/// <summary>
	/// Finds an indexed command with the given name
	/// </summary>
	/// <param name="command">Name of the command (case sensitive)</param>
	/// <returns></returns>
	public static CMDCommand FindCommand(string command)
	{
#if !CMDPLUS_EXCLUDED_IN_BUILD
		for (int i = 0; i < commands.Count; i++)
		{
			if (commands[i].MethodInfo.Name.Trim() == command)
			{
				return commands[i];
			}
		}
		return null;
#else
        return new CMDCommand("CMD_DISABLED");
#endif
	}

	public static void ToggleWindows()
	{
#if !CMDPLUS_EXCLUDED_IN_BUILD
		if (!Initialized && !initializing)
		{
			Init();
		}

		Controller.View.Toggle();
		buttonWindow.Toggle();
#endif
	}
}

namespace CMDPlus
{
	public struct CMDVersion
	{
		public int Major;
		public int Minor;
		public int Build;
		public int Revision;

		public CMDVersion(int major, int minor)
		{
			Major = major;
			Minor = minor;
			Build = -1;
			Revision = -1;
		}
		public CMDVersion(int major, int minor, int build, int revision)
		{
			Major = major;
			Minor = minor;
			Build = build;
			Revision = revision;
		}

		public override string ToString()
		{
			string result = string.Format("{0}.{1}", Major, Minor);
			if (Build >= 0)
			{
				result += string.Format(".{0}", Build);
			}
			if (Revision >= 0)
			{
				result += string.Format(".{0}", Revision);
			}
			return result;
		}
	}
}