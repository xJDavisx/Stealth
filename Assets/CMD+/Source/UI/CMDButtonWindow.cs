using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace DaanRuiter.CMDPlus {
    public class CMDButtonWindow : CMDWindow {
        private List<CMDCommandButton> m_buttons;
        private Vector2 m_buttonMargin = new Vector2(5f, 3f);
        private Vector2 m_padding = new Vector2(5f, 25f);
        private ICMDTween m_toggleTween;
        private float m_buttonHeight = 16;
        private float m_windowHeight;
        private int m_buttonRows = 2;
        private float m_scrollButtonHeight = 16;
        private float m_scrollButtonWidth = 50;
        private int m_scrollButtonPadding = 4;

        private CMDCommandButton m_leftButton, m_rightButton;
        private int m_rows;
        private int m_collumns;
        private int m_currentIndex = 0;

        public CMDButtonWindow(string windowName, Vector2 position, Vector2 size) : base(windowName, position, size) {
            if (Application.isMobilePlatform)
            {
                if (CMDMobileSettings.Loaded != null)
                {
                    float UIScale = CMDMobileSettings.Loaded.UIScale;

                    m_buttonHeight *= UIScale * 2f;
                    m_scrollButtonHeight *= UIScale;
                    m_scrollButtonWidth *= UIScale;
                    m_buttonMargin *= UIScale;
                }

                DeviceChange.OnOrientationChange += OnDeviceOrientationChanged;
            }

            m_buttons = new List<CMDCommandButton>();
            m_buttons.AddRange(CMDHelper.FindMethodAttributes<CMDCommandButton>(new Assembly[] { Assembly.GetAssembly(typeof(CMD)) }));
            m_buttons.RemoveAll(NonVoidMethod);

            for (int i = m_buttons.Count - 1; i >= 0; i--) {
                CMDCommand command = CMDHelper.ExtractAttribute<CMDCommand>(m_buttons[i].MethodInfo);
                if (command != null && command.MethodInfo != null && command.MethodInfo.GetParameters().Length == 0) {
                    m_buttons[i].AddOnClickListener(m_buttons[i].Execute);
                } else if(m_buttons[i].MethodInfo.GetParameters().Length == 0) {
                    m_buttons[i].AddOnClickListener((CMDEventHandler)Delegate.CreateDelegate(typeof(CMDEventHandler), m_buttons[i].MethodInfo));
                } else {
                    m_buttons.RemoveAt(i);
                }
            }

            skin.button.normal.textColor = Color.white * 0.9f;
            skin.button.padding.top = (int)m_buttonHeight / 2 - (int)skin.button.CalcHeight(new GUIContent("Text"), rect.width / m_buttonRows) / 2;
            skin.button.padding.left = 2;
            skin.button.alignment = TextAnchor.MiddleCenter;

            m_leftButton = new CMDCommandButton("<");
            m_leftButton.AddOnClickListener(LeftButtonPressed);
            m_leftButton.SetColor(Color.white);

            m_rightButton = new CMDCommandButton(">");
            m_rightButton.AddOnClickListener(RightButtonPressed);
            m_leftButton.SetColor(Color.white);

            m_windowHeight = rect.height;
            SetSize(new Vector2(rect.width, 0f));

            OnDeviceOrientationChanged(Input.deviceOrientation);
        }
        ~CMDButtonWindow()
        {
            DeviceChange.OnOrientationChange -= OnDeviceOrientationChanged;
        }

        private void OnDeviceOrientationChanged(DeviceOrientation orientation)
        {
            switch (orientation)
            {
                case DeviceOrientation.Portrait:
                    m_buttonRows = 1;
                    break;
                case DeviceOrientation.LandscapeLeft | DeviceOrientation.LandscapeRight:
                    m_buttonRows = 2;
                    break;
                default:
                    if (Screen.height > Screen.width) //Sometimes orientation isnt set on start
                    {
                        m_buttonRows = 1;
                    }
                    break;
            }
            RecalculateButtonRectangles(0);
        }

        public void AddButton(CMDCommandButton button) {
            m_buttons.Add(button);
            RecalculateButtonRectangles(m_buttons.Count - 1);
        }
        
        public override void OnRender() {
            base.OnRender();
            if (!enabled) { return; }

            int start = m_currentIndex * (m_collumns * m_rows);
            for (int i = start; i < m_buttons.Count; i++) {
                if (i >= start + (m_rows * m_collumns)) {
                    break;
                }

                if (m_buttons[i].Interactable) {
                    skin.button.normal.background = m_buttons[i].Texture;
                } else {
                    skin.button.normal.background = m_buttons[i].NoneInteractableTexture;
                }
                skin.button.hover.background = m_buttons[i].MouseOverTexture;
                
                Rect buttonRect = m_buttons[i].Rect;
                buttonRect.position += rect.position + m_padding;
                buttonRect.width -= m_padding.x;
                buttonRect.position -= new Vector2(m_currentIndex * 2 * (m_buttonMargin.x + rect.width / m_buttonRows), 0f);

                SwapSkin(skin);
                if(GUI.Button(buttonRect, m_buttons[i].Text)) {
                    m_buttons[i].OnButtonClick();
                }
                RevertSkinChange();
            }

            skin.button.normal.background = m_leftButton.Texture;
            SwapSkin(skin);
            m_leftButton.SetRect(new Rect(rect.x + rect.width / 2f - m_scrollButtonWidth, rect.height - m_scrollButtonHeight - m_scrollButtonPadding, m_scrollButtonWidth, m_scrollButtonHeight));
            if (GUI.Button(m_leftButton.Rect, m_leftButton.Text)){
                m_leftButton.OnButtonClick();
            }
            RevertSkinChange();

            skin.button.normal.background = m_rightButton.Texture;
            SwapSkin(skin);
            m_rightButton.SetRect(new Rect(rect.x + rect.width / 2f, rect.height - m_scrollButtonHeight - m_scrollButtonPadding, m_scrollButtonWidth, m_scrollButtonHeight));
            if (GUI.Button(m_rightButton.Rect, m_rightButton.Text)) {
                m_rightButton.OnButtonClick();
            }
            RevertSkinChange();

            contextMenu.OpenIfClickedIn(rect);
        }

        public override void Toggle(bool state) {
            if (m_toggleTween != null) {
                m_toggleTween.Kill();
            }
            float tweenDuration = CMDSettings.Get<float>("CONSOLE_TOGGLE_TWEEN_DURATION", false);
            if (state) {
                base.ToggleVisibility(true);
                m_toggleTween = CMDTweens.AddTween(new CMDTween<float>(OnToggleTweenValueSet, rect.height, m_windowHeight, tweenDuration).OnCompleted(OnToggleOnTweenCompleted));
            } else {
                base.Toggle(false);
                m_toggleTween = CMDTweens.AddTween(new CMDTween<float>(OnToggleTweenValueSet, rect.height, 0f, tweenDuration).OnCompleted(OnToggleOffTweenCompleted));
            }
        }

        private void OnToggleTweenValueSet(float newValue) {
            SetSize(new Vector2(rect.width, newValue));
        }

        private void OnToggleOnTweenCompleted() {
            base.Toggle(true);
        }

        private void OnToggleOffTweenCompleted() {
            base.ToggleVisibility(false);
        }

        private void RecalculateButtonRectangles(int startIndex) {
            m_rows = 0;
            m_collumns = 0;
            for (int i = startIndex; i < m_buttons.Count; i++) {
                float widthButton = rect.width / m_buttonRows;
                float heightButtonsArea = m_windowHeight - m_scrollButtonHeight - m_scrollButtonPadding - m_padding.y * 2f;
                int maxCollums = Mathf.FloorToInt(heightButtonsArea / (m_buttonHeight + m_buttonMargin.y));

                int row = 0;
                int collumn = 0;
                if (i != 0) {
                    row = i;
                    if (i >= maxCollums)
                        row = i - (Mathf.FloorToInt(i / maxCollums) * maxCollums);
                    collumn = Mathf.CeilToInt(i / maxCollums);
                }

                Rect r = new Rect(collumn * (widthButton + m_buttonMargin.x), row * (m_buttonHeight + m_buttonMargin.y), widthButton, m_buttonHeight);
                r.x += m_buttonMargin.x;

                m_buttons[i].SetRect(r);

                if(row + 1> m_rows) {
                    m_rows = row + 1;
                }
                if(collumn + 1 > m_collumns) {
                    m_collumns = collumn + 1;
                }
            }
        }

        private bool NonVoidMethod(CMDCommandButton button) {
            return button.MethodInfo.ReturnType != typeof(void);
        }

        private void LeftButtonPressed() {
            if(m_currentIndex > 0) {
                m_currentIndex--;
            }
        }
        private void RightButtonPressed() {
            if (m_currentIndex + 1 < m_collumns / m_buttonRows) {
                m_currentIndex++;
            }
        }
    }
}