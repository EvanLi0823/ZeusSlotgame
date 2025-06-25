using System.Collections.Generic;
using Classic;
using Core;
using UnityEngine;

namespace Libs
{
    public enum AwardType
    {
        Coin = 10000,
        Cash = 10001
    }
    
   
    public class RewardManager
    {
        public static RewardManager Instance{
            get{ 
                return Singleton<RewardManager>.Instance;
            }
        }

        private RewardManager(){}
        
        private BaseAwardItem CreateReward(int type,int count)
        {
            BaseAwardItem baseAward = null;
            switch (type)
            {
                case (int)AwardType.Coin:
                    baseAward = new CoinAwardItem();
                    baseAward.type = AwardType.Coin;
                    baseAward.count = count;
                    // 处理金币奖励
                    Debug.Log($"奖励金币: {baseAward.count}");
                    break;
                case (int)AwardType.Cash:
                    // 处理现金奖励
                    baseAward = new CashAwardItem();
                    baseAward.type = AwardType.Cash;
                    baseAward.count = count;
                    Debug.Log($"奖励现金: {baseAward.count}");
                    break;
                default:
                    Debug.LogError("未知奖励类型");
                    break;
            }
            return baseAward;
        }

        public List<BaseAwardItem> CreateRewardByStr(string rewardList)
        {
            List<BaseAwardItem> awards = new List<BaseAwardItem>();
            string[] rewards = rewardList.Split(';');
            for (int i = 0; i < rewards.Length; i++)
            {
                string singleRewardStr = rewards[i];
                string[] singleReward = singleRewardStr.Split(',');
                if (singleReward.Length < 2)
                {
                    Debug.LogError($"奖励格式错误: {singleRewardStr}");
                    continue;
                }
                if (!int.TryParse(singleReward[0], out int type) || !int.TryParse(singleReward[1], out int count))
                {
                    Debug.LogError($"奖励类型或数量解析失败: {singleRewardStr}");
                    continue;
                }
                BaseAwardItem baseAward = CreateReward(type, count);
                awards.Add(baseAward);
            }

            return awards;
        }
        
        public void GrantAwardByStr(string rewardList)
        {
            if (string.IsNullOrEmpty(rewardList))
            {
                Debug.LogError("奖励列表为空");
                return;
            }
            List<BaseAwardItem> awards = CreateRewardByStr(rewardList);
            foreach (var award in awards)
            {
                award.AwardPrizes();
            }
        }

        public void GrantAward(List<BaseAwardItem> awards)
        {
            if (awards == null || awards.Count == 0)
            {
                Debug.LogError("奖励列表为空");
                return;
            }

            foreach (var award in awards)
            {
                award.AwardPrizes();
            }
        }
    }
}