using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public struct PopPlanDiscountItem
    {
        //最小等级
        public int MinLevel;
        //最大等级
        public int MaxLevel;
        //当前折扣
        public int Discount;
        //序号
        public int Index;
        public PopPlanDiscountItem(int minLevel,int maxLevel,int discount,int index)
        {
            this.MinLevel = minLevel;
            this.MaxLevel = maxLevel;
            this.Discount = discount;
            Index = index;
        }
    }
    public class BaseOnlineEarningModel
    {
        public const string CurADNumberKey = "CurADNumberKey";
        public const string CurLuckyADNumberKey = "CurLuckyADNumberKey";
        public const string CurFreeADNumberKey = "CurFreeADNumberKey";
        public const string CurBonusADNumberKey = "CurBonusADNumberKey";

        public const string PopSpinWinCountKey = "PopSpinWinCountKey";

        public const string BigPopNumKey = "BigPopNum";
        public const string SmallPopNumKey = "SmallPopNum";
        public const string SpinIntervalTimeKey = "SpinInternalTime";
        public const string GetRewardCountKey = "GetRewardCountKey";
        public const string GetH5RewardCountCountKey = "GetH5RewardCountCountKey";

        public const string PopLevelKey = "PopLevelKey";
        public const string WithDraw_Key = "WithDraw";
        public const string Money_KEY = "money"; 
        public const string DiscountKey = "discount";
        
        public int withDrawMoney = 0;
        //todo 集成到广告模块里
        //关闭spinwin三次后自动弹出插屏广告
        public int ADLimit = 3;
        public int FreeGameADLimit = 3;
        public int BonusGameADLimit = 3;
        //LuckyCash广告弹出次数限制,前15次spin不弹出luckycash弹窗,之后开始计数，按照当前 level spinlimit弹出
        public int LuckyADLimit = 15;
        
        public int PopSpinWinCount
        {
            set
            {      
                SharedPlayerPrefs.SetPlayerPrefsIntValue(PopSpinWinCountKey, value);
            }
            get
            {
                int value = SharedPlayerPrefs.GetPlayerPrefsIntValue(PopSpinWinCountKey, 0);
                return value;
            }
        }
        
        //插屏广告翻倍奖励的倍数，默认为 1
        public int ADMultiple = 1;
        //关闭spinwin弹窗的次数
        public int CurADNumber
        {
            set
            {      
                SharedPlayerPrefs.SetPlayerPrefsIntValue(CurADNumberKey, value);
            }
            get
            {
                int value = SharedPlayerPrefs.GetPlayerPrefsIntValue(CurADNumberKey, 0);
                return value;
            }
        }
        public int CurLuckyADNumber
        {
            set
            {      
                SharedPlayerPrefs.SetPlayerPrefsIntValue(CurLuckyADNumberKey, value);
            }
            get
            {
                int value = SharedPlayerPrefs.GetPlayerPrefsIntValue(CurLuckyADNumberKey, 0);
                return value;
            }
        }
        public int CurFreeADNumber
        {
            set
            {      
                SharedPlayerPrefs.SetPlayerPrefsIntValue(CurFreeADNumberKey, value);
            }
            get
            {
                int value = SharedPlayerPrefs.GetPlayerPrefsIntValue(CurFreeADNumberKey, 0);
                return value;
            }
        }
        public int CurBonusADNumber
        {
            set
            {      
                SharedPlayerPrefs.SetPlayerPrefsIntValue(CurBonusADNumberKey, value);
            }
            get
            {
                int value = SharedPlayerPrefs.GetPlayerPrefsIntValue(CurBonusADNumberKey, 0);
                return value;
            }
        }
        //大弹窗总的弹出次数，一直累计
        public int BigDialogPopNum
        {
            set
            {      
                SharedPlayerPrefs.SetPlayerPrefsIntValue(BigPopNumKey, value);
            }
            get
            {
                int value = SharedPlayerPrefs.GetPlayerPrefsIntValue(BigPopNumKey, 0);
                return value;
            }
        }
        
        //大弹窗弹出后,重置为0
        public int SmallDialogPopNum
        {
            set
            {      
                SharedPlayerPrefs.SetPlayerPrefsIntValue(SmallPopNumKey, value);
            }
            get
            {
                int value = SharedPlayerPrefs.GetPlayerPrefsIntValue(SmallPopNumKey, 0);
                return value;
            }
        }
        //当前 spin 次数
        public int CurSpinTime
        {
            set
            {      
                SharedPlayerPrefs.SetPlayerPrefsIntValue(SpinIntervalTimeKey, value);
            }
            get
            {
                int value = SharedPlayerPrefs.GetPlayerPrefsIntValue(SpinIntervalTimeKey, 0);
                return value;
            }
        }
        //玩家领奖次数
        public int GetRewardCount
        {
            set
            {
                SharedPlayerPrefs.SetPlayerPrefsIntValue(GetRewardCountKey, value);
            }
            get
            {
                int value = SharedPlayerPrefs.GetPlayerPrefsIntValue(GetRewardCountKey, 0);
                return value;
            }
        }
        //弹窗弹出次数
        public int GetH5RewardCount
        {
            set
            {
                SharedPlayerPrefs.SetPlayerPrefsIntValue(GetH5RewardCountCountKey, value);
            }
            get
            {
                int value = SharedPlayerPrefs.GetPlayerPrefsIntValue(GetH5RewardCountCountKey, 0);
                return value;
            }
        }
        
        public int PopLevel
        {
            set
            {      
                SharedPlayerPrefs.SetPlayerPrefsIntValue(PopLevelKey, value);
            }
            get
            {
                int value = SharedPlayerPrefs.GetPlayerPrefsIntValue(PopLevelKey, 1);
                return value;
            }
        }
        
        public List<PopPlanDiscountItem> DiscountLists = new List<PopPlanDiscountItem>();

        public virtual void ParseConfig(Dictionary<string,object> config)
        {
            InitWithDrawConfig(config);
        }

        public void InitWithDrawConfig(Dictionary<string,object> config)
        {
            Dictionary<string,object> withDrawConfig = Utils.Utilities.GetValue<Dictionary<string,object>>(config, WithDraw_Key, null);
            if (withDrawConfig.ContainsKey(Money_KEY))
            {
                withDrawMoney =  Utils.Utilities.GetInt(withDrawConfig,Money_KEY);
            }
            List<object> discountConfig = Utils.Utilities.GetValue<List<object>>(withDrawConfig, DiscountKey, null);
            if (discountConfig == null)
            {
                return;
            }

            for (int i = 0; i < discountConfig.Count; i++)
            {
                Dictionary<string, object> info = discountConfig[i] as Dictionary<string,object>;
                int minlevel = Utils.Utilities.GetInt(info,"minLevel");
                int maxlevel = Utils.Utilities.GetInt(info,"maxLevel");
                int discount = Utils.Utilities.GetInt(info,"discount");
                int index =Utils.Utilities.GetInt(info,"index");
                PopPlanDiscountItem item = new PopPlanDiscountItem(minlevel,maxlevel,discount,index);
                DiscountLists.Add(item);
            }
            DiscountLists.Sort((item1,item2) =>
            {
                return item1.Index < item2.Index ? -1:1;
            });
        }
        
        public virtual bool CanShowH5()
        {
            return false;
        }

        public virtual int CashMultiple()
        {
            return 100;
        }
        
        public virtual bool CanShowBig()
        {
            return false;
        }
        public virtual bool CheckCanPopReward()
        {
            return false;
        }

        public virtual void PopSmallDialogEnd()
        {
        }
        public virtual void PopBigDialogEnd()
        {
        }
        public void AddSpinTime()
        {
            if (CurLuckyADNumber<LuckyADLimit)
            {
                //前20次不弹 luckycash弹板
                AddLuckyADNum();
            }
            else
            {
                // Debug.Log($"AddSpinTime CurSpinTime:{CurSpinTime}");
                CurSpinTime++;
            }
        }
        
        public void ResetSpinTime()
        {
            CurSpinTime=0;
        }
        
        public void AddSmallPop()
        {
            SmallDialogPopNum++;
        }
        public void AddBigPop()
        {
            BigDialogPopNum++;
        }
        public void AddADNum()
        {
            CurADNumber++;
        }
        public int GetADNum()
        {
            return CurADNumber;
        }
        public bool CheckCanPopAD()
        {
            return CurADNumber >= ADLimit;
        }

        public void ResetADNum()
        {
            CurADNumber = 0;
        }
        public void AddFreeInterstitialADNum()
        {
            CurFreeADNumber++;
        }
        public int GetFreeInterstitialADNum()
        {
           return CurFreeADNumber++;
        }
        public bool CheckCanPopFreeInterstitialAD()
        {
            return CurFreeADNumber >= FreeGameADLimit;
        }
        
        public void ResetFreeInterstitialADNum()
        {
            CurFreeADNumber = 0;
        }
        
        public void AddBonusInterstitialADNum()
        {
            CurBonusADNumber++;
        }
        
        public bool CheckCanPopBonusInterstitialAD()
        {
            return CurBonusADNumber >= BonusGameADLimit;
        }
        
        public void ResetBonusInterstitialADNum()
        {
            CurBonusADNumber = 0;
        }
        public int GetBonusInterstitialADNum()
        {
            return CurBonusADNumber++;
        }
        public void AddLuckyADNum()
        {
            CurLuckyADNumber++;
        } 
       
        
        public virtual int GetBigReward()
        {
           return 0;
        }
        public virtual int GetBigRewardMultiple()
        {
            return 0;
        } 
      
        public virtual int GetH5Reward()
        {
            return 0;
        }
        public virtual void AddGetRewardCount()
        {
            GetRewardCount++;
        }
        public virtual void AddGetH5RewardCount()
        {
            GetH5RewardCount++;
        }
        
        //弹窗弹出奖励次数+h5点击获取奖励次数
        private int GetTotalH5RewardCount()
        {
            return GetRewardCount + GetH5RewardCount;
        }
        public virtual void HandleH5Event(int amount)
        {
            
        }

        public virtual int Level()
        {
            return PopLevel;
        }
     
        public List<PopPlanDiscountItem> GetDiscountItem()
        {
            return DiscountLists;
        }
        
        public int GetCurLevelDiscount()
        {
            foreach (var item in DiscountLists)
            {
                if (PopLevel>=item.MinLevel && PopLevel<=item.MaxLevel )
                {
                    return item.Discount;
                }
            }
            return 0;
        }
        
        public virtual int GetRewardsByName(string key,int level = 0)
        {
            return 0;
        }
        
        public int AddPopSpinWinCount()
        {
            PopSpinWinCount++;
            return PopSpinWinCount;
        }
    }
}