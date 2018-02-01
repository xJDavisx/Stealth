using System;
using System.Collections.Generic;
using UnityEngine;

// Delegates are in a seperate place so they don't clutter the Intelli Sense for CMD.cs
namespace DaanRuiter.CMDPlus {
    //Log Entries
    public delegate CMDLogEntry 
        CMDLogEntryRequestEventHandler (LogType type, string message, string stackTrace);
    public delegate void 
        CMDLogEntryCreatedEventHandler (CMDLogEntry logEntry);

    //Commands
    public delegate KeyValuePair<Exception, CMDLogEntry> 
        CMDCommandExecutionEventHandler (string command, string stackTrace, params object[] args);
    
    //Unity
    public delegate void
        OnGUIEventHandler ();
    public delegate void
        KeyPressEventHandler(KeyCode keyCode);

    //Tweens
    public delegate void
        CMDTweenEventHandler();

    //Windows
    public delegate CMDWindow
        CMDWindowCreateRequestEventHandler (string windowName, Vector2 position, Vector2 size);
    public delegate void
        CMDWindowRenderEventHandler (CMDWindow window);
    public delegate void 
        CMDWindowToggleEventHandler (bool newState);

    //Input
    public delegate void
        CMDInputFieldContentChangeEventHandler(string newContent);

    //CMD
    public delegate void 
        CMDEventHandler();
    public delegate void
        CMDParamEventHandler(params object[] args);
}