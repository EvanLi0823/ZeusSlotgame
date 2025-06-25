using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class PushTaskDataMgr
{
    public static PushTaskDataMgr Instance{ get { return Classic.Singleton<PushTaskDataMgr>.Instance; } }
    private const string FeatureKey = "task_data";
    
    private PushTaskDataMgr() { }
    
    
    public void ParseTaskDataByPush(int code, Dictionary<string,object> data)
    {
        if (code != 200 || data == null || data.Count == 0) return;
        // Dictionary<string, object> tempData =
        //     Utilities.GetValue<Dictionary<string, object>>(data, "data", null);
        // if (tempData == null || tempData.Count == 0) return;
        // 下行数据
        Messenger.Broadcast<Dictionary<string, object>>(GameConstants.QuestMachineOnResponseMsg, data);
    }

    public void ParseTaskDataByReward()
    {
        List<BaseCommandItem> rewardList = CommandManager.Instance.GetCommandListByFeature(FeatureKey);
        if (rewardList == null || rewardList.Count == 0) return;
        
        rewardList.Sort((item1, item2) =>
        {
            return Utils.Utilities.CastValueInt(item1.CreateTime - item2.CreateTime, 0);
        });

        for (int i = 0; i < rewardList.Count; i++)
        {
            Dictionary<string, object> data =
                new Dictionary<string, object>(
                    Utilities.GetValue<Dictionary<string, object>>(rewardList[i].ItemDict, "data", new Dictionary<string, object>()));
            if (data.Count > 0)
            {
                // 下行数据
                Messenger.Broadcast<Dictionary<string, object>>(GameConstants.QuestMachineOnResponseMsg, data);
            }
            
            rewardList[i].OnAccept();
            rewardList[i] = null;
        }
    }

}
