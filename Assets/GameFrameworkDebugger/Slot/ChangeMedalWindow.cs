using System.Collections.Generic;
using LuaFramework;
using UnityEngine;


namespace UnityGameFramework.Runtime
{
    public sealed partial class DebuggerComponent
    {
        private sealed class ChangeMedalWindow : ScrollableDebuggerWindowBase
        {
            public ChangeMedalWindow()
            {
            }


            
            private string mainDataText = "";
            private string machineDataText = "{\"total\":6,\"machine\":\"FuFuRiches\",\"done\":0,\"total_spin\":0,\"medal_type\":\"\",\"bronze\":[{\"name\":\"bronze1\",\"icon\":\"1001\",\"got_time\":-1,\"progress\":0.1,\"status\":1,\"spin_count\":1,\"expired_time\":-1,\"medal_count\":0,\"task_info\":{\"template_id\":1000,\"done_num\":10,\"desc\":\"temp task desc\",\"require\":1}},{\"name\":\"bronze2\",\"icon\":\"1002\",\"got_time\":-1,\"progress\":0.2,\"status\":1,\"spin_count\":2,\"medal_count\":0,\"expired_time\":-1,\"task_info\":{\"template_id\":1000,\"done_num\":10,\"desc\":\"temp task desc\",\"require\":2}}],\"silver\":[{\"name\":\"silver1\",\"icon\":\"2001\",\"got_time\":-1,\"progress\":0.1,\"status\":0,\"spin_count\":3,\"medal_count\":0,\"expired_time\":-1,\"task_info\":{\"template_id\":2000,\"done_num\":10,\"desc\":\"temp task desc\",\"require\":1}},{\"name\":\"silver2\",\"icon\":\"2002\",\"got_time\":-1,\"progress\":0.2,\"status\":0,\"spin_count\":4,\"medal_count\":0,\"expired_time\":-1,\"task_info\":{\"template_id\":2000,\"desc\":\"temp task desc\",\"done_num\":10,\"require\":2}}],\"gold\":[{\"name\":\"gold1\",\"icon\":\"3001\",\"got_time\":-1,\"progress\":0,\"status\":0,\"spin_count\":5,\"medal_count\":0,\"expired_time\":-1,\"task_info\":{\"template_id\":3000,\"done_num\":10,\"desc\":\"temp task desc\",\"require\":0}},{\"name\":\"gold2\",\"icon\":\"3002\",\"got_time\":-1,\"progress\":0,\"status\":0,\"spin_count\":6,\"expired_time\":-1,\"medal_count\":0,\"task_info\":{\"template_id\":3000,\"desc\":\"temp task desc\",\"done_num\":10,\"require\":0}}],\"purple\":[{\"name\":\"purple\",\"icon\":\"4001\",\"got_time\":-1,\"progress\":0,\"status\":1,\"spin_count\":7,\"medal_count\":3,\"expired_time\":1648273235000,\"task_info\":{\"template_id\":3000,\"desc\":\"temp task desc\",\"done_num\":10,\"require\":0}}]}";
            
            private string medalRewardText = "{\"medal_info\":{\"total\":6,\"machine\":\"FuFuRiches\",\"done\":2,\"total_spin\":3,\"medal_type\":\"\",\"bronze\":[{\"name\":\"bronze1\",\"icon\":\"1001\",\"got_time\":1647605355000,\"progress\":1,\"status\":2,\"spin_count\":1,\"expired_time\":-1,\"medal_count\":1,\"task_info\":{\"template_id\":1000,\"desc\":\"temp Task Desc\",\"done_num\":10,\"require\":1}},{\"name\":\"bronze2\",\"icon\":\"1002\",\"got_time\":1647605355000,\"progress\":1,\"status\":2,\"spin_count\":2,\"expired_time\":-1,\"medal_count\":1,\"task_info\":{\"template_id\":1000,\"desc\":\"temp Task Desc\",\"done_num\":10,\"require\":2}}],\"silver\":[{\"name\":\"silver1\",\"icon\":\"2001\",\"got_time\":-1,\"progress\":0.1,\"status\":1,\"spin_count\":3,\"expired_time\":-1,\"medal_count\":0,\"task_info\":{\"template_id\":2000,\"desc\":\"temp Task Desc\",\"done_num\":10,\"require\":1}},{\"name\":\"silver2\",\"icon\":\"2002\",\"got_time\":-1,\"progress\":0.6,\"status\":1,\"spin_count\":4,\"expired_time\":-1,\"medal_count\":0,\"task_info\":{\"template_id\":2000,\"desc\":\"temp Task Desc\",\"done_num\":10,\"require\":2}}],\"gold\":[{\"name\":\"gold1\",\"icon\":\"3001\",\"got_time\":-1,\"progress\":0,\"status\":0,\"spin_count\":5,\"expired_time\":-1,\"medal_count\":0,\"task_info\":{\"template_id\":3000,\"desc\":\"temp Task Desc\",\"done_num\":10,\"require\":0}},{\"name\":\"gold2\",\"icon\":\"3002\",\"got_time\":-1,\"progress\":0,\"status\":0,\"spin_count\":6,\"expired_time\":-1,\"medal_count\":0,\"task_info\":{\"template_id\":3000,\"desc\":\"temp Task Desc\",\"done_num\":10,\"require\":0}}],\"purple\":[{\"name\":\"purple\",\"icon\":\"4001\",\"got_time\":-1,\"progress\":0,\"status\":1,\"spin_count\":7,\"medal_count\":3,\"expired_time\":1648273235000,\"task_info\":{\"template_id\":3000,\"desc\":\"temp Task Desc\",\"done_num\":10,\"require\":0}}]},\"machine\":\"FuFuRiches\",\"collect\":[{\"name\":\"bronze1\",\"icon\":\"1001\",\"got_time\":1647589281000,\"progress\":1,\"status\":2,\"spin_count\":1},{\"name\":\"bronze2\",\"icon\":\"2001\",\"got_time\":1647589281000,\"progress\":1,\"status\":2,\"spin_count\":2}]}";
                
            private string tipText = "";

            protected override void OnDrawScrollableWindow()
            {
                GUILayout.Label("<b>配置Medal:</b>");
                GUILayout.BeginVertical("box");
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("主界面信息", GUILayout.Height(50), GUILayout.Width(150));
                    mainDataText = GUILayout.TextField(mainDataText, GUILayout.Height(50f));
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("机器界面信息", GUILayout.Height(50), GUILayout.Width(150));
                    machineDataText = GUILayout.TextField(machineDataText, GUILayout.Height(50f));
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("获得Medal信息", GUILayout.Height(50), GUILayout.Width(150));
                    medalRewardText = GUILayout.TextField(medalRewardText, GUILayout.Height(50f));
                    GUILayout.EndHorizontal();
                    
                    GUILayout.Label("刷新结果:", GUILayout.Height(50), GUILayout.Width(350));
                    GUILayout.Label(tipText, GUILayout.Height(50), GUILayout.Width(350));
                    
                    if (GUILayout.Button("刷新主界面信息", GUILayout.Height(50f)))
                    {
                        RefreshMainData();
                    }

                    if (GUILayout.Button("刷新机器Medal信息", GUILayout.Height(50f)))
                    {
                        RefreshMachineData();
                    }
                    if (GUILayout.Button("刷新获得Medal信息", GUILayout.Height(50f)))
                    {
                        RefreshReward();
                    }
                    
                    
                }
                GUILayout.EndVertical();
            }

            private void RefreshMainData()
            {
                if (string.IsNullOrEmpty(mainDataText))
                {
                    tipText = "刷新主界面信息失败, 输入为空!";
                    return;
                }

                var dict = MiniJSON.Json.Deserialize(mainDataText) as Dictionary<string, object>;
                if (dict == null || dict.Count == 0)
                {
                    tipText = "刷新主界面信息失败, 输入格式错误,请检查!";
                    return;
                }
                Messenger.Broadcast<string>("MedalEditorRefreshMainData", mainDataText);
                tipText = "刷新主界面信息信息成功";
            }

            private void RefreshMachineData()
            {
                if (string.IsNullOrEmpty(machineDataText))
                {
                    tipText = "刷新机器Medal信息失败, 输入为空!";
                    return;
                }
                var dict = MiniJSON.Json.Deserialize(machineDataText) as Dictionary<string, object>;
                if (dict == null || dict.Count == 0)
                {
                    tipText = "刷新机器Medal信息失败, 输入格式错误,请检查!";
                    return;
                }
                Messenger.Broadcast<string>("MedalEditorRefreshMachineData", machineDataText);
                tipText = "刷新机器Medal信息成功";
            }

            private void RefreshReward()
            {
                if (string.IsNullOrEmpty(medalRewardText))
                {
                    tipText = "刷新获得Medal信息失败, 输入为空!";
                    return;
                }
                var dict = MiniJSON.Json.Deserialize(medalRewardText) as Dictionary<string, object>;
                if (dict == null || dict.Count == 0)
                {
                    tipText = "刷新获得Medal信息失败, 输入格式错误,请检查!";
                    return;
                }
                Messenger.Broadcast<string>("MedalEditorRefreshRewardData", medalRewardText);
                tipText = "刷新获得Medal信息成功";
            }
        }
    }
}