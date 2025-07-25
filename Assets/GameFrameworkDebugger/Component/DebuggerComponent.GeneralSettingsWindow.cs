﻿//------------------------------------------------------------
// Game Framework v3.x
// Copyright © 2013-2017 Jiang Yin. All rights reserved.
// Homepage: http://gameframework.cn/
// Feedback: mailto:jiangyin@gameframework.cn
//------------------------------------------------------------

using UnityEngine;

namespace UnityGameFramework.Runtime
{
    public partial class DebuggerComponent
    {
        private sealed class GeneralSettingsWindow : ScrollableDebuggerWindowBase
        {
            private DebuggerComponent m_DebuggerComponent = null;
            private float m_LastIconX = 0f;
            private float m_LastIconY = 0f;
            private float m_LastWindowX = 0f;
            private float m_LastWindowY = 0f;
            private float m_LastWindowWidth = 0f;
            private float m_LastWindowHeight = 0f;
            private float m_LastWindowScale = 0f;

            public override void Initialize(params object[] args)
            {
                if (args.Length < 1)
                {
                    throw new System.ArgumentException("Debugger component is invalid.");
                }

                m_DebuggerComponent = args[0] as DebuggerComponent;
                if (m_DebuggerComponent == null)
                {
                    throw new System.ArgumentException("Debugger component is invalid.");
                }

                m_LastIconX = PlayerPrefs.GetFloat("Debugger.Icon.X", DefaultIconRect.x);
                m_LastIconY = PlayerPrefs.GetFloat("Debugger.Icon.Y", DefaultIconRect.y);
                m_LastWindowX = PlayerPrefs.GetFloat("Debugger.Window.X", DefaultWindowRect.x);
                m_LastWindowY = PlayerPrefs.GetFloat("Debugger.Window.Y", DefaultWindowRect.y);
                m_LastWindowWidth = PlayerPrefs.GetFloat("Debugger.Window.Width", DefaultWindowRect.width);
                m_LastWindowHeight = PlayerPrefs.GetFloat("Debugger.Window.Height", DefaultWindowRect.height);
                m_DebuggerComponent.WindowScale = m_LastWindowScale = PlayerPrefs.GetFloat("Debugger.Window.Scale", DefaultWindowScale);
                m_DebuggerComponent.IconRect = new Rect(m_LastIconX, m_LastIconY, DefaultIconRect.width, DefaultIconRect.height);
                m_DebuggerComponent.WindowRect = new Rect(m_LastWindowX, m_LastWindowY, m_LastWindowWidth, m_LastWindowHeight);
            }

            public override void OnUpdate(float elapseSeconds, float realElapseSeconds)
            {
                if (m_LastIconX != m_DebuggerComponent.IconRect.x)
                {
                    m_LastIconX = m_DebuggerComponent.IconRect.x;
                    PlayerPrefs.SetFloat("Debugger.Icon.X", m_DebuggerComponent.IconRect.x);
                }

                if (m_LastIconY != m_DebuggerComponent.IconRect.y)
                {
                    m_LastIconY = m_DebuggerComponent.IconRect.y;
                    PlayerPrefs.SetFloat("Debugger.Icon.Y", m_DebuggerComponent.IconRect.y);
                }

                if (m_LastWindowX != m_DebuggerComponent.WindowRect.x)
                {
                    m_LastWindowX = m_DebuggerComponent.WindowRect.x;
                    PlayerPrefs.SetFloat("Debugger.Window.X", m_DebuggerComponent.WindowRect.x);
                }

                if (m_LastWindowY != m_DebuggerComponent.WindowRect.y)
                {
                    m_LastWindowY = m_DebuggerComponent.WindowRect.y;
                    PlayerPrefs.SetFloat("Debugger.Window.Y", m_DebuggerComponent.WindowRect.y);
                }

                if (m_LastWindowWidth != m_DebuggerComponent.WindowRect.width)
                {
                    m_LastWindowWidth = m_DebuggerComponent.WindowRect.width;
                    PlayerPrefs.SetFloat("Debugger.Window.Width", m_DebuggerComponent.WindowRect.width);
                }

                if (m_LastWindowHeight != m_DebuggerComponent.WindowRect.height)
                {
                    m_LastWindowHeight = m_DebuggerComponent.WindowRect.height;
                    PlayerPrefs.SetFloat("Debugger.Window.Height", m_DebuggerComponent.WindowRect.height);
                }

                if (m_LastWindowScale != m_DebuggerComponent.WindowScale)
                {
                    m_LastWindowScale = m_DebuggerComponent.WindowScale;
                    PlayerPrefs.SetFloat("Debugger.Window.Scale", m_DebuggerComponent.WindowScale);
                }
            }

            protected override void OnDrawScrollableWindow()
            {
                GUILayout.Label("<b>Window Settings</b>");
                GUILayout.BeginVertical("box");
                {
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label("Position:", GUILayout.Width(60f));
                        GUILayout.Label("Drag window caption to move position.");
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    {
                        float width = m_DebuggerComponent.WindowRect.width;
                        GUILayout.Label("Width:", GUILayout.Width(60f));
                        if (GUILayout.RepeatButton("-", GUILayout.Width(30f)))
                        {
                            width--;
                        }
                        width = GUILayout.HorizontalSlider(width, 100f, Screen.width - 20f);
                        if (GUILayout.RepeatButton("+", GUILayout.Width(30f)))
                        {
                            width++;
                        }
                        width = Mathf.Clamp(width, 100f, Screen.width - 20f);
                        if (width != m_DebuggerComponent.WindowRect.width)
                        {
                            m_DebuggerComponent.WindowRect = new Rect(m_DebuggerComponent.WindowRect.x, m_DebuggerComponent.WindowRect.y, width, m_DebuggerComponent.WindowRect.height);
                        }
                    }
                    GUILayout.EndHorizontal();
                    DrawSpaceLine();

                    GUILayout.BeginHorizontal();
                    {
                        float height = m_DebuggerComponent.WindowRect.height;
                        GUILayout.Label("Height:", GUILayout.Width(60f));
                        if (GUILayout.RepeatButton("-", GUILayout.Width(30f)))
                        {
                            height--;
                        }
                        height = GUILayout.HorizontalSlider(height, 100f, Screen.height - 20f);
                        if (GUILayout.RepeatButton("+", GUILayout.Width(30f)))
                        {
                            height++;
                        }
                        height = Mathf.Clamp(height, 100f, Screen.height - 20f);
                        if (height != m_DebuggerComponent.WindowRect.height)
                        {
                            m_DebuggerComponent.WindowRect = new Rect(m_DebuggerComponent.WindowRect.x, m_DebuggerComponent.WindowRect.y, m_DebuggerComponent.WindowRect.width, height);
                        }
                    }
                    GUILayout.EndHorizontal();
                    DrawSpaceLine();

                    GUILayout.BeginHorizontal();
                    {
                        float scale = m_DebuggerComponent.WindowScale;
                        GUILayout.Label("Scale:", GUILayout.Width(60f));
                        if (GUILayout.RepeatButton("-", GUILayout.Width(30f)))
                        {
                            scale -= 0.01f;
                        }
                        scale = GUILayout.HorizontalSlider(scale, 0.5f, 4f);
                        if (GUILayout.RepeatButton("+", GUILayout.Width(30f)))
                        {
                            scale += 0.01f;
                        }
                        scale = Mathf.Clamp(scale, 0.5f, 4f);
                        if (scale != m_DebuggerComponent.WindowScale)
                        {
                            m_DebuggerComponent.WindowScale = scale;
                        }
                    }
                    GUILayout.EndHorizontal();
                    
                    DrawSpaceLine();

                    if (GUILayout.Button("Reset Window Settings", GUILayout.Height(30f)))
                    {
                        m_DebuggerComponent.IconRect = DefaultIconRect;
                        m_DebuggerComponent.WindowRect = DefaultWindowRect;
                        m_DebuggerComponent.WindowScale = DefaultWindowScale;
                    }
                }
                GUILayout.EndVertical();
            }
        }
    }
}
