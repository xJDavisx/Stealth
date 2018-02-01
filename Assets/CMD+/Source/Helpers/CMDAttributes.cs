using System;
using System.Reflection;
using System.Diagnostics;
using UnityEngine;
using DaanRuiter.CMDPlus;

public interface ICMDAttribute {
    MethodInfo MethodInfo { get; set; }
    StackFrame OriginFrame { get; set; }
}

public class CMDCommand : Attribute, ICMDAttribute {
    /// <summary>
    /// Is the command hidden when calling the List command
    /// </summary>
    public bool IsHiddenInList { get; private set; }
    /// <summary>
    /// The description rendered next to the command name when calling the List command
    /// </summary>
    public string Description { get; private set; }
    public MethodInfo MethodInfo { get; set; }
    public StackFrame OriginFrame { get; set; }

    public CMDCommand (string description = "", bool hiddenInList = false) {
        Description = description;
        IsHiddenInList = hiddenInList;
    }
}

public class CMDCommandButton : Attribute, ICMDAttribute {
    /// <summary>
    /// The text displayed on the button
    /// </summary>
    public string Text;
    /// <summary>
    /// Is the button currently interactable
    /// </summary>
    public bool Interactable = true;
    public Rect Rect { get; private set; }
    public MethodInfo MethodInfo { get; set; }
    public StackFrame OriginFrame { get; set; }

    internal Texture2D Texture { get; private set; }
    internal Texture2D MouseOverTexture { get; private set; }
    internal Texture2D NoneInteractableTexture { get; private set; }

    private CMDEventHandler buttonPressedEvent;

    public CMDCommandButton(string displayText, string buttonColor) {
        Color color = Color.white;
        CMDHelper.TryParseColor(buttonColor, out color);
        Init(displayText, color);
    }
    public CMDCommandButton (string displayText) {
        Init(displayText, Color.white);
    }
    ~CMDCommandButton () {
        CMD.Controller.UnityInjector.AddObjectToDestroyQueue(Texture);
        CMD.Controller.UnityInjector.AddObjectToDestroyQueue(MouseOverTexture);
        CMD.Controller.UnityInjector.AddObjectToDestroyQueue(NoneInteractableTexture);
    }

    public CMDCommandButton AddOnClickListener (CMDEventHandler callBack) {
        buttonPressedEvent += callBack;
        return this;
    }

    public CMDCommandButton RemoveOnClickListener (CMDEventHandler callBack) {
        buttonPressedEvent -= callBack;
        return this;
    }

    /// <summary>
    /// Sets the color of the background texture
    /// </summary>
    public CMDCommandButton SetColor (Color color) {
        Texture = CMDHelper.GenerateVerticalGradientTexture(new Color[] { color * 0.35f, color * 0.55f },
                                                                        new float[] { 0.95f, 0.7f },
                                                                        1, 10);
        return this;
    }

    /// <summary>
    /// Sets the color of the mouse over texture
    /// </summary>
    public CMDCommandButton SetMouseOverColor (Color color) {
        MouseOverTexture.SetPixel(0, 0, color);
        MouseOverTexture.Apply();
        return this;
    }

    /// <summary>
    /// Sets the color of the non interactable texture
    /// </summary>
    public CMDCommandButton SetNoneInteractableColor (Color color) {
        NoneInteractableTexture.SetPixel(0, 0, color);
        NoneInteractableTexture.Apply();
        return this;
    }

    public void SetRect(Rect rect) {
        Rect = rect;
    }

    public void Execute() {
        CMD.ExecuteCommand(MethodInfo);
    }

    internal void OnButtonClick () {
        if (!Interactable) { return; }

        if (buttonPressedEvent != null) {
            buttonPressedEvent();
        }
    }

    private void Init(string displayText, Color buttonColor) {
        Text = displayText;

        SetColor(buttonColor);

        MouseOverTexture = new Texture2D(1, 1);
        MouseOverTexture.SetPixel(0, 0, Color.white * 0.85f);
        MouseOverTexture.Apply();

        NoneInteractableTexture = new Texture2D(1, 1);
        NoneInteractableTexture.SetPixel(0, 0, Color.white * 0.65f);
        NoneInteractableTexture.Apply();
    }
}