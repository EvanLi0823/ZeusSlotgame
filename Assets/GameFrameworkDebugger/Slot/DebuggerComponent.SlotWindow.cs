using UnityEngine;


namespace UnityGameFramework.Runtime
{
    public sealed partial class DebuggerComponent
    {
        private sealed class SlotWindow : ScrollableDebuggerWindowBase
        {
            private ISlotGameDebugger _slot;

            public SlotWindow(ISlotGameDebugger slot)
            {
                _slot = slot;
            }

            private string cmdString = "";

            protected override void OnDrawScrollableWindow()
            {
                GUILayout.Label("<b>Slot</b>");
                GUILayout.BeginVertical("box");
                {
                    DrawItem("User Segment", _slot.GetUserSegment());
                    DrawSpaceLine();
                    DrawItem("Is Using Scaled Screen Resolution", _slot.GetUsingScaledScreenResolution().ToString());
                    DrawSpaceLine();
                    DrawItem("Is High Performance Device", _slot.IsHighPerformanceDevice().ToString());
                    DrawSpaceLine();
                    GUILayout.BeginHorizontal();
                    cmdString = GUILayout.TextField(cmdString,GUILayout.Height(50f));
                    if (GUILayout.Button("发送GM",GUILayout.Height(50f)))
                    {
                        _slot.SendGMCmd(cmdString);
                        cmdString = "";
                    }
                    GUILayout.EndHorizontal();
                    DrawSpaceLine();
                    if (GUILayout.Button("Set Use Scaled Screen Resolution(Need Restart)",GUILayout.Height(30f)))
                    {
                        _slot.SetUseScaledScreenResolution();
                    }
                    DrawSpaceLine();
                    if (GUILayout.Button("Set Do Not Use Scaled Screen Resolution(Need Restart)",GUILayout.Height(30f)))
                    {
                        _slot.SetDoNotUseScaledScreenResolution();
                    }
                    DrawSpaceLine();
                    if (GUILayout.Button("Delete PlayerPrefs(Need Restart)", GUILayout.Height(30f)))
                    {
                        _slot.DeletePlayerPrefs();
                    }
                    DrawSpaceLine();
                    if (GUILayout.Button("Delete PersistentData(Need Restart)",GUILayout.Height(30f)))
                    {
                        _slot.DeletePersistentData();
                    }
                    DrawSpaceLine();
                    if (GUILayout.Button("Reload Game",GUILayout.Height(30f)))
                    {
                        _slot.DebugToReload();
                    }
                }
                GUILayout.EndVertical();
            }
        }
    }

    internal interface ISlotGameDebugger
    {
        string GetUserSegment();
        void DeletePersistentData();
        void DeletePlayerPrefs();
        bool GetUsingScaledScreenResolution();
        bool IsHighPerformanceDevice();
        void SetUseScaledScreenResolution();
        void SetDoNotUseScaledScreenResolution();
        void SendGMCmd(string cmd);
        void DebugToReload();
    }
}