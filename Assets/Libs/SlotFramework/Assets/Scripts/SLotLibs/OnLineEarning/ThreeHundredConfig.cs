using System;
using System.Collections.Generic;
using System.Numerics;
using Classic;
using Libs;
using Utils;

namespace Core
{
    public class ThreeHundredConfig : BaseRewardConfig
    {
        static readonly string SmallInterval_KEY = "Small";
        static readonly string SpinInterval_Key = "Spin";
        static readonly string VedioLimitKey = "VedioLimit";
        static readonly string VedioMultipleKey = "VedioMultiple";
        static readonly string CurADNumberKey = "CurADNumberKey";
        static readonly string LimitCountKey = "LimitCount";
        static readonly string LimitRateKey = "LimitRate";
        static readonly string LimitRewardKey = "LimitReward";
        static readonly string GetFirstRewardTimeKey = "GetFirstRewardTime";
        static readonly string HideH5TagKey = "HideH5TagKey";
        
        static readonly string TodayGetRewardCountKey = "TodayGetRewardCountKey";
        static readonly string IsSecondKey = "IsSecondKey";
        //此字段用来标志第二个数组的取值下标，满足走第二个数组条件时会一直累加
        private int GetRewardCountInToday
        {
            set
            {
                SharedPlayerPrefs.SetPlayerPrefsIntValue(TodayGetRewardCountKey, value);
            }
            get
            {
                int value = SharedPlayerPrefs.GetPlayerPrefsIntValue(TodayGetRewardCountKey, 0);
                return value;
            }
        }
        
      
     
        public int CurSmallLimit = 3;
        public int CurSpinLimit = 1;
        public int RewardCountLimit = 20;

        //正常概率为0.95
        private float limitRate = 0.95f;
        //为了数值精确，扩大了CashMultiple倍
        private int LimitRate
        {
            get
            {
                return 9500;
            }
        }
        private int minReward = 100;
        private int maxReward = 200;
        private int[][] arrayOne;
        private int[][] arrayTwo;

        private long getFirstRewardTime
        {
            set
            {
                SharedPlayerPrefs.SavePlayerPrefsLong(GetFirstRewardTimeKey, value);
            }
            get
            {
                return SharedPlayerPrefs.LoadPlayerPrefsLong(GetFirstRewardTimeKey, 0);
            }
        }
       
        public override void ParseConfig(Dictionary<string, object> config)
        {
            ADLimit = Utilities.GetInt(config, VedioLimitKey, 3);
            ADMultiple = Utilities.GetInt(config, VedioMultipleKey, 1);
            CurSmallLimit = Utilities.GetInt(config, SmallInterval_KEY, 3);
            CurSpinLimit = Utilities.GetInt(config, SpinInterval_Key, 1);
            RewardCountLimit = Utilities.GetInt(config, LimitCountKey, 1);
            List<object> array = Utilities.GetValue<List<object>>(config, LimitRewardKey, null);
            if (array.Count == 2)
            {
                minReward = (int)array[0];
                maxReward = (int)array[1];
            }
            base.ParseConfig(config);
            InitData();
            InitRewardArray();
        }

        private void InitData()
        {
            if (SharedPlayerPrefs.GetPlayerPrefsIntValue(IsSecondKey,0)==0)
            {
                DateTime saveTime = TimeUtils.ToDateTimeFromTimeStamp(getFirstRewardTime);
                bool isSameDay = TimeUtils.IsSameDay(DateTime.Now, saveTime);
                if (getFirstRewardTime!=0 && !isSameDay)
                {
                    //重置今天的领奖次数
                    GetRewardCountInToday = 0;
                    //设置标志防止重复重置
                    SharedPlayerPrefs.SetPlayerPrefsIntValue(IsSecondKey,1);
                }
            }
        }
        
        private void InitRewardArray()
        {
            arrayOne = new int[][]
            {
                new int[] { 0, 0 }, new int[] { 0, 660000 }, new int[] { 40000, 75000 }, new int[] { 40000, 75000 },
                new int[] { 40000, 75000 }, new int[] { 40000, 75000 }, new int[] { 40000, 60000 }, new int[] { 37800, 56900 },
                new int[] { 35700, 53800 }, new int[] { 33500, 50800 }, new int[] { 33500, 50800 }, new int[] { 29200, 44600 }, 
                new int[] { 27100, 41500 }, new int[] { 24900, 38500 }, new int[] { 22800, 35400 }, new int[] { 22800, 35400 }, 
                new int[] { 18500, 29200 }, new int[] { 16300, 26199 }, new int[] { 14200, 23100 }, new int[] { 12000, 20000 }, 
                new int[] { 12000, 20000 }, new int[] { 9100, 15900 }, new int[] { 9000, 15599 }, new int[] { 8800, 15400 }, 
                new int[] { 8700, 15200 }, new int[] { 8700, 15200 }, new int[] { 8500, 14700 }, new int[] { 8400, 14500 }, 
                new int[] { 8200, 14299 }, new int[] { 8100, 14100 }, new int[] { 8100, 14100 }, new int[] { 7900, 13700 }, 
                new int[] { 7799, 13500 }, new int[] { 7700, 13300 }, new int[] { 7600, 13099 }, new int[] { 7600, 13099 }, 
                new int[] { 7400, 12700 }, new int[] { 7300, 12500 }, new int[] { 7200, 12300 }, new int[] { 7100, 12100 }, 
                new int[] { 7100, 12100 }, new int[] { 6900, 11799 }, new int[] { 6800, 11600 }, new int[] { 6700, 11400 },
                new int[] { 6600, 11200 }, new int[] { 6600, 11200 }, new int[] { 6400, 10900 }, new int[] { 6300, 10800 }, 
                new int[] { 6200, 10599 }, new int[] { 6100, 10400 }, new int[] { 6100, 10400 }, new int[] { 6000, 10100 },
                new int[] { 5899, 10000 }, new int[] { 5800, 9800 }, new int[] { 5700, 9700 }, new int[] { 5700, 9700 },
                new int[] { 5600, 9400 }, new int[] { 5500, 9300 }, new int[] { 5400, 9100 }, new int[] { 5299, 9000 },
                new int[] { 5299, 9000 }, new int[] { 5200, 8700 }, new int[] { 5100, 8600 }, new int[] { 5000, 8500 },
                new int[] { 5000, 8300 }, new int[] { 5000, 8300 }, new int[] { 4800, 8100 }, new int[] { 4800, 8000 },
                new int[] { 4700, 7799 }, new int[] { 4600, 7700 }, new int[] { 4600, 7700 }, new int[] { 4500, 7500 },
                new int[] { 4400, 7400 }, new int[] { 4400, 7300 }, new int[] { 4300, 7200 }, new int[] { 4300, 7200 },
                new int[] { 4200, 7000 }, new int[] { 4100, 6900 }, new int[] { 4100, 6800 }, new int[] { 4000, 6700 },
                new int[] { 4000, 6700 }, new int[] { 3899, 6500 }, new int[] { 3899, 6400 }, new int[] { 3800, 6300 },
                new int[] { 3800, 6200 }, new int[] { 3800, 6200 }, new int[] { 3700, 6000 }, new int[] { 3600, 5899 },
                new int[] { 3600, 5800 }, new int[] { 3500, 5700 }, new int[] { 3500, 5700 }, new int[] { 3400, 5600 },
                new int[] { 3400, 5500 }, new int[] { 3300, 5400 }, new int[] { 3300, 5299 }, new int[] { 3300, 5299 },
                new int[] { 3200, 5200 }, new int[] { 3100, 5100 }, new int[] { 3100, 5000 }, new int[] { 3000, 4900 },
                new int[] { 3000, 4900 }, new int[] { 3000, 4800 }, new int[] { 2900, 4700 }, new int[] { 2900, 4600 },
                new int[] { 2800, 4600 }, new int[] { 2800, 4600 }, new int[] { 2800, 4400 }, new int[] { 2700, 4400 },
                new int[] { 2700, 4300 }, new int[] { 2700, 4200 }, new int[] { 2700, 4200 }, new int[] { 2600, 4100 },
                new int[] { 2500, 4100 }, new int[] { 2500, 4000 }, new int[] { 2500, 3899 }, new int[] { 2500, 3899 },
                new int[] { 2400, 3800 }, new int[] { 2400, 3800 }, new int[] { 2300, 3700 }, new int[] { 2300, 3700 },
                new int[] { 2300, 3700 }, new int[] { 2200, 3500 }, new int[] { 2200, 3500 }, new int[] { 2200, 3400 },
                new int[] { 2100, 3400 }, new int[] { 2100, 3400 }, new int[] { 2100, 3300 }, new int[] { 2100, 3200 },
                new int[] { 2000, 3200 }, new int[] { 2000, 3100 }, new int[] { 2000, 3100 }, new int[] { 1900, 3000 },
                new int[] { 1900, 3000 }, new int[] { 1900, 3000 }, new int[] { 1900, 2900 }, new int[] { 1900, 2900 },
                new int[] { 1800, 2800 }, new int[] { 1800, 2800 }, new int[] { 1800, 2700 }, new int[] { 1700, 2700 },
                new int[] { 1700, 2700 }, new int[] { 1700, 2600 }, new int[] { 1700, 2600 }, new int[] { 1600, 2500 },
                new int[] { 1600, 2500 }, new int[] { 1600, 2500 }, new int[] { 1600, 2400 }, new int[] { 1600, 2400 },
                new int[] { 1500, 2400 }, new int[] { 1500, 2300 }, new int[] { 1500, 2300 }, new int[] { 500, 1000 },
                new int[] { 500, 1000 }, new int[] { 500, 1000 }, new int[] { 500, 1000 }, new int[] { 500, 1000 },
                new int[] { 500, 1000 }, new int[] { 500, 1000 }, new int[] { 500, 1000 }, new int[] { 500, 1000 },
                new int[] { 500, 1000 }, new int[] { 500, 1000 }, new int[] { 500, 1000 }, new int[] { 500, 1000 },
                new int[] { 500, 1000 }, new int[] { 500, 1000 }, new int[] { 500, 1000 }, new int[] { 500, 1000 },
                new int[] { 500, 1000 }, new int[] { 500, 1000 }, new int[] { 500, 1000 }, new int[] { 500, 1000 },
                new int[] { 500, 1000 }, new int[] { 500, 1000 }, new int[] { 500, 1000 }, new int[] { 500, 1000 },
                new int[] { 500, 1000 }, new int[] { 500, 1000 }, new int[] { 500, 1000 }, new int[] { 500, 1000 },
                new int[] { 500, 1000 }, new int[] { 500, 1000 }, new int[] { 500, 1000 }, new int[] { 500, 1000 },
                new int[] { 500, 1000 }, new int[] { 500, 1000 }, new int[] { 500, 1000 }, new int[] { 500, 1000 },
                new int[] { 500, 1000 }, new int[] { 500, 1000 }, new int[] { 500, 1000 }, new int[] { 500, 1000 },
                new int[] { 500, 1000 }, new int[] { 500, 1000 }, new int[] { 500, 1000 }, new int[] { 500, 1000 },
                new int[] { 500, 1000 }, new int[] { 500, 1000 }, new int[] { 500, 1000 }, new int[] { 500, 1000 },
                new int[] { 500, 1000 }, new int[] { 500, 1000 }, new int[] { 500, 1000 }, new int[] { 500, 1000 },
                new int[] { 500, 1000 }, new int[] { 500, 1000 }, new int[] { 500, 1000 }, new int[] { 100, 200 },
                new int[] { 100, 200 }, new int[] { 100, 200 }, new int[] { 100, 200 }, new int[] { 100, 100 },
                new int[] { 100, 200 }, new int[] { 100, 200 }, new int[] { 100, 200 }, new int[] { 100, 200 },
                new int[] { 100, 100 }, new int[] { 100, 200 }, new int[] { 100, 200 }, new int[] { 100, 200 },
                new int[] { 100, 200 }, new int[] { 100, 100 }, new int[] { 100, 200 }, new int[] { 100, 200 },
                new int[] { 100, 200 }, new int[] { 100, 200 }, new int[] { 100, 100 }, new int[] { 100, 200 },
                new int[] { 100, 200 }, new int[] { 100, 200 }, new int[] { 100, 200 }, new int[] { 100, 100 },
                new int[] { 100, 200 }, new int[] { 100, 200 }, new int[] { 100, 200 }, new int[] { 100, 200 },
                new int[] { 100, 100 }, new int[] { 100, 200 }, new int[] { 100, 200 }, new int[] { 100, 200 },
                new int[] { 100, 200 }, new int[] { 100, 100 }
            };

            arrayTwo = new int[][]
            {
                new int[] { 0, 0 }, new int[] { 10000, 17000 }, new int[] { 9900, 16700 }, new int[] { 9700, 16500 },
                new int[] { 9600, 16300 }, new int[] { 9600, 16300 }, new int[] { 9300, 15800 }, new int[] { 9100, 15500 },
                new int[] { 9000, 15300 }, new int[] { 8900, 15100 }, new int[] { 8900, 15100 }, new int[] { 8600, 14600 },
                new int[] { 8500, 14400 }, new int[] { 8400, 14200 }, new int[] { 8200, 14000 }, new int[] { 8200, 14000 },
                new int[] { 8000, 13600 }, new int[] { 7900, 13400 }, new int[] { 7700, 13200 }, new int[] { 7600, 13000 },
                new int[] { 7600, 13000 }, new int[] { 7400, 12600 }, new int[] { 7300, 12400 }, new int[] { 7200, 12200 },
                new int[] { 7100, 12000 }, new int[] { 7100, 12000 }, new int[] { 6900, 11700 }, new int[] { 6800, 11500 },
                new int[] { 6700, 11300 }, new int[] { 6600, 11200 }, new int[] { 6600, 11200 }, new int[] { 6400, 10800 },
                new int[] { 6300, 10700 }, new int[] { 6200, 10500 }, new int[] { 6100, 10400 }, new int[] { 6100, 10400 },
                new int[] { 5899, 10100 }, new int[] { 5800, 9900 }, new int[] { 5700, 9800 }, new int[] { 5700, 9600 },
                new int[] { 5700, 9600 }, new int[] { 5500, 9300 }, new int[] { 5400, 9200 }, new int[] { 5299, 9100 },
                new int[] { 5200, 8900 }, new int[] { 5200, 8900 }, new int[] { 5100, 8700 }, new int[] { 5000, 8500 },
                new int[] { 4900, 8400 }, new int[] { 4900, 8300 }, new int[] { 4900, 8300 }, new int[] { 4700, 8000 },
                new int[] { 4700, 7900 }, new int[] { 4600, 7799 }, new int[] { 4500, 7700 }, new int[] { 4500, 7700 },
                new int[] { 4400, 7500 }, new int[] { 4300, 7300 }, new int[] { 4300, 7200 }, new int[] { 4200, 7100 },
                new int[] { 4200, 7100 }, new int[] { 4100, 6900 }, new int[] { 4000, 6800 }, new int[] { 3899, 6700 },
                new int[] { 3899, 6600 }, new int[] { 3899, 6600 }, new int[] { 3800, 6400 }, new int[] { 3700, 6300 },
                new int[] { 3700, 6200 }, new int[] { 3600, 6100 }, new int[] { 3600, 6100 }, new int[] { 3500, 5899 },
                new int[] { 3400, 5899 }, new int[] { 3400, 5800 }, new int[] { 3300, 5700 }, new int[] { 3300, 5700 },
                new int[] { 3200, 5500 }, new int[] { 3200, 5400 }, new int[] { 3200, 5400 }, new int[] { 3100, 5299 },
                new int[] { 3100, 5299 }, new int[] { 3000, 5100 }, new int[] { 3000, 5000 }, new int[] { 2900, 5000 },
                new int[] { 2900, 4900 }, new int[] { 2900, 4900 }, new int[] { 2800, 4800 }, new int[] { 2800, 4700 },
                new int[] { 2700, 4600 }, new int[] { 2700, 4500 }, new int[] { 2700, 4500 }, new int[] { 2600, 4400 },
                new int[] { 2600, 4300 }, new int[] { 2500, 4300 }, new int[] { 2500, 4200 }, new int[] { 2500, 4200 },
                new int[] { 2400, 4100 }, new int[] { 2400, 4000 }, new int[] { 2300, 4000 }, new int[] { 2300, 3899 },
                new int[] { 2300, 3899 }, new int[] { 2200, 3800 }, new int[] { 2200, 3700 }, new int[] { 2200, 3700 },
                new int[] { 2100, 3600 }, new int[] { 2100, 3600 }, new int[] { 2100, 3500 }, new int[] { 2000, 3500 },
                new int[] { 2000, 3400 }, new int[] { 2000, 3400 }, new int[] { 2000, 3400 }, new int[] { 1900, 3300 },
                new int[] { 1900, 3200 }, new int[] { 1900, 3200 }, new int[] { 1800, 3100 }, new int[] { 1800, 3100 },
                new int[] { 1800, 3000 }, new int[] { 1800, 3000 }, new int[] { 1700, 2900 }, new int[] { 1700, 2900 },
                new int[] { 1700, 2900 }, new int[] { 1700, 2800 }, new int[] { 1600, 2800 }, new int[] { 1600, 2700 },
                new int[] { 1600, 2700 }, new int[] { 1600, 2700 }, new int[] { 1500, 2600 }, new int[] { 1500, 2600 },
                new int[] { 1500, 2500 }, new int[] { 1500, 2500 }, new int[] { 1500, 2500 }, new int[] { 1400, 2400 },
                new int[] { 1400, 2400 }, new int[] { 1400, 2300 }, new int[] { 1400, 2300 }, new int[] { 1400, 2300 },
                new int[] { 1300, 2200 }, new int[] { 1300, 2200 }, new int[] { 1300, 2200 }, new int[] { 1300, 2100 },
                new int[] { 1300, 2100 }, new int[] { 1200, 2100 }, new int[] { 1200, 2100 }, new int[] { 1200, 2000 },
                new int[] { 1200, 2000 }, new int[] { 1200, 2000 }, new int[] { 1100, 1900 }, new int[] { 1100, 1900 },
                new int[] { 1100, 1900 }, new int[] { 1100, 1800 }, new int[] { 1100, 1800 }, new int[] { 500, 1000 },
                new int[] { 500, 1000 }, new int[] { 500, 1000 }, new int[] { 500, 1000 }, new int[] { 500, 1000 },
                new int[] { 500, 1000 }, new int[] { 500, 1000 }, new int[] { 500, 1000 }, new int[] { 500, 1000 },
                new int[] { 500, 1000 }, new int[] { 500, 1000 }, new int[] { 500, 1000 }, new int[] { 500, 1000 },
                new int[] { 500, 1000 }, new int[] { 500, 1000 }, new int[] { 500, 1000 }, new int[] { 500, 1000 },
                new int[] { 500, 1000 }, new int[] { 500, 1000 }, new int[] { 500, 1000 }, new int[] { 500, 1000 },
                new int[] { 500, 1000 }, new int[] { 500, 1000 }, new int[] { 500, 1000 }, new int[] { 500, 1000 },
                new int[] { 500, 1000 }, new int[] { 500, 1000 }, new int[] { 500, 1000 }, new int[] { 500, 1000 },
                new int[] { 500, 1000 }, new int[] { 500, 1000 }, new int[] { 500, 1000 }, new int[] { 500, 1000 },
                new int[] { 500, 1000 }, new int[] { 500, 1000 }, new int[] { 500, 1000 }, new int[] { 500, 1000 },
                new int[] { 500, 1000 }, new int[] { 500, 1000 }, new int[] { 500, 1000 }, new int[] { 500, 1000 },
                new int[] { 500, 1000 }, new int[] { 500, 1000 }, new int[] { 500, 1000 }, new int[] { 500, 1000 },
                new int[] { 500, 1000 }, new int[] { 500, 1000 }, new int[] { 500, 1000 }, new int[] { 500, 1000 },
                new int[] { 500, 1000 }, new int[] { 500, 1000 }, new int[] { 500, 1000 }, new int[] { 500, 1000 },
                new int[] { 500, 1000 }, new int[] { 500, 1000 }, new int[] { 500, 1000 }, new int[] { 500, 1000 },
                new int[] { 500, 1000 }, new int[] { 500, 1000 }, new int[] { 500, 1000 }, new int[] { 500, 1000 },
                new int[] { 500, 1000 }, new int[] { 500, 1000 }, new int[] { 500, 1000 }, new int[] { 500, 1000 },
                new int[] { 500, 1000 }, new int[] { 500, 1000 }, new int[] { 500, 1000 }, new int[] { 500, 1000 },
                new int[] { 100, 200 }, new int[] { 100, 200 }, new int[] { 100, 200 }, new int[] { 100, 200 },
                new int[] { 100, 100 }, new int[] { 100, 200 }, new int[] { 100, 200 }, new int[] { 100, 200 },
                new int[] { 100, 200 }, new int[] { 100, 100 }, new int[] { 100, 200 }, new int[] { 100, 200 },
                new int[] { 100, 200 }, new int[] { 100, 200 }, new int[] { 100, 100 }, new int[] { 100, 200 },
                new int[] { 100, 200 }, new int[] { 100, 200 }, new int[] { 100, 200 }, new int[] { 100, 100 },
                new int[] { 100, 200 }, new int[] { 100, 200 }, new int[] { 100, 200 }, new int[] { 100, 200 },
                new int[] { 100, 100 }, new int[] { 100, 200 }, new int[] { 100, 200 }, new int[] { 100, 200 },
                new int[] { 100, 200 }, new int[] { 100, 100 }, new int[] { 100, 200 }, new int[] { 100, 200 },
                new int[] { 100, 200 }, new int[] { 100, 200 }, new int[] { 100, 100 }, new int[] { 100, 200 },
                new int[] { 100, 200 }, new int[] { 100, 200 }, new int[] { 100, 200 }, new int[] { 100, 100 },
                new int[] { 100, 200 }, new int[] { 100, 200 }, new int[] { 100, 200 }, new int[] { 100, 200 },
                new int[] { 100, 100 }
            };
        }
        
        private int GetMaxValue()
        {
            CountryType country = OnLineEarningMgr.Instance.GetCountry();
            int money= 0;;
            if(country == CountryType.BR){//巴西
                money =1600;
            }else if(country == CountryType.PK){//巴基斯坦
                money = 82500;
            }else if(country == CountryType.ID){//印尼
                money = 4550000;
            }else if(country == CountryType.RU){ //俄罗斯
                money = 30000;
            }else if(country == CountryType.AR){ //阿根廷
                money = 64200;
            }else if(country == CountryType.CO){ //哥伦比亚
                money = 1451100;
            }else if(country == CountryType.MX){//墨西哥
                money = 5400;
            }else if(country == CountryType.PE){//秘鲁
                money = 1140;
            }else if(country == CountryType.KR){ //韩国
                money = 400000;
            }else{ //美元
                money = 300;
            }
            return money;
        }
        
        //玩家当前所获金钱超过这个值需要做特殊判断
        //已扩大CashMultiple
        public int GetLimitCount()
        {
            int money= GetMaxValue();;
            return money * LimitRate;
        }
        
        public int GetReward(bool nextValue)
        {
            //钱数是扩大CashMultiple倍的数值
            int totalMoney = OnLineEarningMgr.Instance.Cash();
            //金钱临界值 
            int limitMoney = GetLimitCount();
            //最大钱数
            int maxValue = GetMaxValue() * CashMultiple();
            DateTime saveTime = TimeUtils.ToDateTimeFromTimeStamp(getFirstRewardTime);
            bool isSameDay = TimeUtils.IsSameDay(DateTime.Now, saveTime);
            if (totalMoney>=limitMoney)
            {
                return HandleMaxReward(totalMoney,maxValue);
            }

            if (getFirstRewardTime == 0 || isSameDay)
            {
                return this.HandleFirstReward(nextValue);
            }
            return HandleRegularReward(nextValue);
        }

        private int HandleMaxReward(int totalMoney,int maxValue)
        {
            Random random = new Random();
            int rateNum = random.Next(minReward, maxReward);
            int rewardNum = (maxValue - totalMoney) / rateNum;
            int newNum = totalMoney + rewardNum;
            if (newNum>=maxValue)
            {
                rewardNum = 0;
                //关闭H5按钮
                SharedPlayerPrefs.SetPlayerPrefsBoolValue(HideH5TagKey,false);
                Messenger.Broadcast<bool>(GameConstants.OnH5InitSuccess,false);
                //通知原生部分
                PlatformManager.Instance.SendMsgToPlatFormByType(MessageType.UserAmount,newNum);
            }
            return rewardNum;
        }

        private int HandleFirstReward(bool nextValue)
        {
            Random random = new Random();
            int rewardNum = 0;
            if (GetRewardCount>=this.arrayOne.Length)
            {
                rewardNum = random.Next(minReward, maxReward);
            }
            else
            {
                int[] array = nextValue ? this.arrayOne[GetRewardCount + 1] : this.arrayOne[GetRewardCount];
                rewardNum = GetRewardCount == 1 ? array[1] : random.Next(array[0], array[1]);
                if (GetRewardCount ==1)
                {
                    //存储第一次获奖时间
                    getFirstRewardTime = TimeUtils.GetTimestampInSeconds(DateTime.Now);
                }
            }
            return rewardNum;
        }
        /**非第一天奖励*/
        private int HandleRegularReward(bool nextValue)
        {
            Random random = new Random();
            int rewardNum = 0;
            //获奖次数小于 20次使用列表 1
            if (GetRewardCount<this.RewardCountLimit)
            {
                int[] array = nextValue ? this.arrayOne[GetRewardCount + 1] : this.arrayOne[GetRewardCount];
                rewardNum = random.Next(array[0], array[1]);
            }
            else
            {
                if (GetRewardCountInToday>arrayTwo.Length)
                {
                    rewardNum = random.Next(minReward, maxReward);
                }
                else
                {
                    int[] array = nextValue ? this.arrayTwo[GetRewardCountInToday + 1] : this.arrayTwo[GetRewardCountInToday];
                    rewardNum = random.Next(array[0], array[1]);
                }
            }
            return rewardNum;
        }
        public override int CashMultiple()
        {
            return 10000;
        }
        public override void PopSmallDialogEnd()
        {
            CurSpinTime =0;
            AddSmallPop();
        }
        public override void PopBigDialogEnd()
        {
            SmallDialogPopNum = 0;
            CurSpinTime =0;
            AddBigPop();
        }
        public override int GetBigRewardMultiple()
        {
            return 2;
        }   
        public override void AddGetRewardCount()
        {
            GetRewardCount++;
            if (CanShowH5())
            {
                Messenger.Broadcast<bool>(GameConstants.OnH5InitSuccess,true);
            }
            DateTime saveTime = TimeUtils.ToDateTimeFromTimeStamp(getFirstRewardTime);
            bool isSameDay = TimeUtils.IsSameDay(DateTime.Now, saveTime);
            if (!isSameDay && GetRewardCount>=RewardCountLimit)
            {
                GetRewardCountInToday++;
            }

            int newlevel = (int)Math.Ceiling(GetRewardCount/10.0f);
            if (newlevel >PopLevel)
            {
                PopLevel = newlevel;
                //广播升级
                Messenger.Broadcast(SlotControllerConstants.OnPopLevelChange);
            }
        }
        
        public override bool CheckCanPopReward()
        {
            return CurSpinTime>=CurSpinLimit;
        }

        public override bool CanShowBig()
        {
            return SmallDialogPopNum >= CurSmallLimit;
        }
        public override bool CanShowH5()
        {
            return GetRewardCount >=1 && SharedPlayerPrefs.GetPlayerBoolValue(HideH5TagKey,true);
        }

        public override int GetH5Reward()
        {
            return GetReward(true);
        }
        
        public override void HandleH5Event(int amount)
        {
            if (OnLineEarningMgr.Instance.Cash() + amount< GetMaxValue() * CashMultiple())
            {
                OnLineEarningMgr.Instance.IncreaseCash(amount);
                Messenger.Broadcast(SlotControllerConstants.OnCashChangeForDisPlay);
            }
            else if(OnLineEarningMgr.Instance.Cash()+amount>GetMaxValue()*9990)
            {
                Messenger.Broadcast<bool>(GameConstants.OnH5InitSuccess,false);
                SharedPlayerPrefs.SetPlayerPrefsBoolValue(HideH5TagKey,false);
            }
        }

        #region Rewards
        public override int GetRewardsByName(string key,int level = 0)
        {
            return 0;
        }
        #endregion
    }
}