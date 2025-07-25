using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace Core
{
    public struct PopPlanItem
    {
        //等级
        public int level;
        //弹出大弹窗的总数，超过大弹窗即为当前等级
        public int bigLimit;
        //在当前等级累计弹出多少个小弹窗才弹一个大弹窗
        public int smallInterval;
        //几次spin后弹出小弹窗
        public int spinLimit;

        public PopPlanItem(int level,int bigLimit,int smallInterval,int spinLimit)
        {
            this.level = level;
            this.bigLimit = bigLimit;
            this.smallInterval = smallInterval;
            this.spinLimit = spinLimit;
        }
    }

    public enum RewardType
    {
        Random = 0,
        Level = 1
    }
    public class PopReward
    {
        readonly string Random_Key = "Random";
        readonly string Min_Key = "min";
        readonly string Max_Key = "max";
        readonly string Level_Key = "Level";
        readonly string Range_Key = "Range";
        public int min;
        public int max;
        public int multiple;
        public string name;
        public RewardType type;
        public List<int> rewards;
        private float range;//用于 level 的浮点范围
        private Dictionary<string, object> data = new Dictionary<string, object>();
        public PopReward(string _name,Dictionary<string,object> _data)
        {
            name = _name;
            data = _data;
            ParseConfig(data);
        }

        void ParseConfig(Dictionary<string,object> data)
        {
            if (data.ContainsKey(Random_Key))
            {
                //random类型在 min和 max区间取值
                type = RewardType.Random;
                Dictionary<string, object> config =
                    Utilities.GetValue<Dictionary<string, object>>(data, Random_Key, null);
                min = Utilities.GetValue<int>(config, Min_Key, 0);
                max = Utilities.GetValue<int>(config, Max_Key, 0);
            }else if (data.ContainsKey(Level_Key))
            {
                range = Utilities.GetFloat(data, Range_Key, 0);
                type = RewardType.Level;
                List<object> rewardList = Utilities.GetValue<List<object>>(data, Level_Key, null);
                if (rewardList==null || rewardList.Count==0)
                {
                    Debug.LogError("ParsePopRewards have Error node name ===="+name);
                    return;
                }
                if (rewards==null)
                {
                    rewards = new List<int>();
                }
                //Todo此处拆箱操作后续想办法优化
                for (int i = 0; i < rewardList.Count; i++)
                {
                    rewards.Add((int)rewardList[i]);
                }
            }
        }

        /// <summary>
        /// 获取奖励值
        /// </summary>
        /// <param name="level">针对Level类型获取奖励时传入的值</param>
        /// <returns></returns>
        public int GetRewards(int level = 0)
        {
            int num = 0;
            if (type == RewardType.Random)
            {
                num = Random.Range(min, max);
            }else if (type == RewardType.Level)
            {
                int baseNum = rewards[level];
                float minNum = baseNum * (1 - range);
                float maxNum = baseNum * (1 + range);
                //向下取整
                num = (int)Math.Floor(Random.Range(minNum,maxNum));
            }
            return num;
        }
    }
    
    public class InfiniteModel : BaseOnlineEarningModel
    {
        static readonly string PopPlan_Key = "PopPlan";
        static readonly string PopReward_Key = "Rewards";
     
       
        static readonly string WithDraw_Key = "WithDraw";

        static readonly string Money_KEY = "money"; 
        static readonly string Level_KEY = "level"; 
        static readonly string BigLimit_KEY = "bigLimit";        
        static readonly string SmallInterval_KEY = "small";
        static readonly string SpinInterval_Key = "spin";
        
        public const string DiscountKey = "discount";
        public const string VedioLimitKey = "VedioLimit";
        public const string FreeGameADLimitKey = "FreeGameADLimit";
        public const string BonusGameADLimitKey = "BonusGameADLimit";

        public const string LuckyVedioLimit = "LuckyVedioLimit";
        public const string VedioMultipleKey = "VedioMultiple";
        public const string CurADNumberKey = "CurADNumberKey";

        public int CurLevelSmallLimit = 0;
        public int CurLevelBigLimit = 0;
        public int CurSpinLimit = 0;

        private List<PopPlanItem> PopPlanList = new List<PopPlanItem>();
        private Dictionary<string,PopReward> RewardDic = new Dictionary<string, PopReward>();

        public override void ParseConfig(Dictionary<string,object> config)
        {
            ADLimit = Utilities.GetInt(config, VedioLimitKey, 3);
            FreeGameADLimit = Utilities.GetInt(config, FreeGameADLimitKey, 3);
            BonusGameADLimit= Utilities.GetInt(config, BonusGameADLimitKey, 3);
            
            ADMultiple = Utilities.GetInt(config, VedioMultipleKey, 1);
            LuckyADLimit = Utilities.GetInt(config, LuckyVedioLimit, 15);
            List<object> popPlan = Utils.Utilities.GetValue<List<object>>(config, PopPlan_Key, null);
            for (int i = 0; i < popPlan.Count; i++)
            {
                Dictionary<string, object> info = popPlan[i] as Dictionary<string, object>;
                if (info == null)
                {
                    return;
                }

                int level = Utils.Utilities.GetValue<int>(info, Level_KEY, 0);
                int bigLimit = Utils.Utilities.GetValue<int>(info, BigLimit_KEY, 0);
                int smallInterval = Utils.Utilities.GetValue<int>(info, SmallInterval_KEY, 0);
                int spinInterval = Utils.Utilities.GetValue<int>(info, SpinInterval_Key, 0);
                PopPlanItem item = new PopPlanItem(level, bigLimit, smallInterval,spinInterval);
                PopPlanList.Add(item);
            }
             PopPlanList.Sort((PopPlanItem item1,PopPlanItem item2)=>
            {
                if (item1.level < item2.level)
                {
                    return -1;
                }
                return 1;
            });
            PopLevel = GetCurrentLevel();
            CurLevelSmallLimit = GetSmallIntervalByLevel(PopLevel);
            CurLevelBigLimit = GetBigLimitByLevel(PopLevel);
            CurSpinLimit = GetSpinLimitByLevel(PopLevel);
            Debug.Log("InfiniteModel CurLevel ="+PopLevel+"   SmallDialogPopNum="+SmallDialogPopNum+"    BigDialogPopNum = "+BigDialogPopNum);
            Dictionary<string,object> popRewards = Utils.Utilities.GetValue<Dictionary<string,object>>(config, PopReward_Key, null);
            if (popRewards == null)
            {
                return;
            }

            foreach (var item in popRewards.Keys)
            {
                Dictionary<string, object> data = Utilities.GetValue<Dictionary<string,object>>(popRewards, item, null);;
                if (data==null)
                {
                    continue;
                }
                PopReward reward = new PopReward(item, data);
                RewardDic[item]=reward;
            }
            
            base.ParseConfig(config);
        }
        
        public int GetMaxLevel()
        {
            return PopPlanList.Count;
        }

        public int WithDrawMoney()
        {
            return withDrawMoney;
        }
        
        public override int GetBigRewardMultiple()
        {
            return 2;
        }   
        public int GetBigLimitByLevel(int level)
        {
            return PopPlanList[level - 1].bigLimit;
        }
        public int GetSmallIntervalByLevel(int level)
        {
            return PopPlanList[level - 1].smallInterval;
        }
        public int GetSpinLimitByLevel(int level)
        {
            return PopPlanList[level - 1].spinLimit;
        }

        public int GetCurrentLevel()
        {
            for (int i = 0; i < PopPlanList.Count; i++)
            {
                if (PopPlanList[i].bigLimit==-1)
                {
                    return PopPlanList[i].level;
                }
                if (PopPlanList[i].bigLimit>BigDialogPopNum)
                {
                    return PopPlanList[i].level;
                }
            }
            return 1;
        }
        public int Level()
        {
            return PopLevel;
        }
        
        //检测能否弹出Luckycash弹窗
        public override bool CheckCanPopReward()
        {
            return CanShowBig();
        }
        
        //弹出小弹窗，小弹窗弹出后，CurSpinLimit 需要重置为 0
        public override void PopSmallDialogEnd()
        {
            CurSpinTime =0;
            AddSmallPop();
        }
        
        //前20次不弹弹窗
        public override bool CanShowBig()
        {
            return  CurLuckyADNumber >= LuckyADLimit && CurSpinTime>=CurSpinLimit;
        }

        //大弹窗弹出后，检测是否可以升级
        public bool CheckCanLevelUp()
        {
            //最后一个等级为无限
            if (CurLevelBigLimit == -1)
            {
                return false;
            }
            return BigDialogPopNum>=CurLevelBigLimit;
        }
  
        //等级加一,重置数据
        public void AddLevel()
        {
            PopLevel += 1;
            Reset();
            Debug.Log("InfiniteModel AddLevel CurLevel ="+PopLevel);
            Messenger.Broadcast(SlotControllerConstants.OnPopLevelChange);
        }

        public void Reset()
        {
            CurLevelSmallLimit = GetSmallIntervalByLevel(PopLevel);
            CurLevelBigLimit = GetBigLimitByLevel(PopLevel);
            CurSpinLimit = GetSpinLimitByLevel(PopLevel);
            Debug.Log("InfiniteModel Reset CurSpinLimit ="+CurSpinLimit+"\nCurLevelSmallLimit="+CurLevelSmallLimit+"\nCurLevelBigLimit = "+CurLevelSmallLimit);
        }
        
        //弹出大弹窗
        public override void PopBigDialogEnd()
        {
            SmallDialogPopNum = 0;
            CurSpinTime =0;
            AddBigPop();
            if (CheckCanLevelUp())
            {
                AddLevel();
            }
        }
        
        public int GetADRewardCash()
        {
            int reward = GetBigReward() * ADMultiple;
            return reward;
        }

        public override void HandleH5Event(int amount)
        {
            base.HandleH5Event(amount);
            //此处应根据不同的模式来做处理
            OnLineEarningMgr.Instance.IncreaseCash(amount);
            Messenger.Broadcast(SlotControllerConstants.OnCashChangeForDisPlay);
        }

        #region Rewards
        public override int GetRewardsByName(string key,int level = 0)
        {
            if (RewardDic == null||!RewardDic.ContainsKey(key))
            {
                return 0;
            }

            PopReward reward = RewardDic[key];
            
            return reward.GetRewards(level);
        }
        #endregion
    }
}