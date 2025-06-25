using System.Collections.Generic;
using UnityEngine;
namespace UnityGameFramework.Runtime
{
    public sealed partial class DebuggerComponent
    {
        private sealed class SwapSceneWindow : ScrollableDebuggerWindowBase
        {
            private string machineName = "";
            private string curMachineName = "";
            private bool needLocateSlot = true;

            public SwapSceneWindow()
            {
            }

            protected override void OnDrawScrollableWindow()
            {
                GUILayout.Label("<b>测试跳转机器:</b>");
                GUILayout.BeginVertical("box");
                {
                    needLocateSlot = GUILayout.Toggle(needLocateSlot, "是否定位机器");

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("要跳转场景名 (机器名/Lobby/ClubLobby):", GUILayout.Height(50), GUILayout.Width(150));
                    machineName = GUILayout.TextField(machineName, GUILayout.Height(50f));
                    GUILayout.EndHorizontal();


                    var temp = SwapSceneManager.Instance.GetLogicSceneName();
                    GUILayout.Label("当前场景名: " + temp, GUILayout.Height(50));

                    curMachineName = GUILayout.TextField(curMachineName, GUILayout.Height(50));

                    if (GUILayout.Button("找开头匹配的机器名字,放在上面↑", GUILayout.Height(50f)))
                    {
                        FindMachineName();
                    }

                    if (GUILayout.Button("You Jump!", GUILayout.Height(50f)))
                    {
                        JumpScene();
                    }
                }
                GUILayout.EndVertical();
            }

            private void JumpScene()
            {
                if (string.IsNullOrEmpty(machineName)) return;
            }

            private void FindMachineName()
            {
                if (string.IsNullOrEmpty(curMachineName)) return;

                List<SlotMachineConfig> configs = Plugins.Configuration.GetInstance().ConfigurationParseResult()
                    .AllSlotMachineConfigs();
                if (configs != null)
                {
                    curMachineName = curMachineName.ToLower();
                    var list = configs.FindAll((c) =>
                    {
                        var configName = c.Name().ToLower();
                        return configName.StartsWith(curMachineName);
                    });
                    foreach (var v in list)
                    {
                        curMachineName += " ; " + v.Name();
                    }
                }
            }
        }
    }
}