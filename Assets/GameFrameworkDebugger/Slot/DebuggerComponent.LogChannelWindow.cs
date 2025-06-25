using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityGameFramework.Runtime
{
    public sealed partial class DebuggerComponent
    {
        private sealed class LogChannelWindow : ScrollableDebuggerWindowBase
        {
            //需要注册
            private bool m_channel1;
            private bool m_channel2;
            private bool m_channel3;
            private bool m_channel4;
            private bool m_channel5;

            protected override void OnDrawScrollableWindow()
            {
                GUILayout.Label("<b>Set Log Channel</b>");
                GUILayout.BeginVertical("box");
                {
                    m_channel1 = GUILayout.Toggle(m_channel1, "channel 1");
                    m_channel2 = GUILayout.Toggle(m_channel2, "channel 2");
                    m_channel3 = GUILayout.Toggle(m_channel3, "channel 3");
                    m_channel4 = GUILayout.Toggle(m_channel4, "channel 4");
                    m_channel5 = GUILayout.Toggle(m_channel5, "channel 5");
                }
                GUILayout.EndVertical();
            }

            public override void OnUpdate(float elapseSeconds, float realElapseSeconds)
            {
                if (m_channel1)
                {
                    
                }
                else
                {
                    
                }
            }
        }
    }
}
