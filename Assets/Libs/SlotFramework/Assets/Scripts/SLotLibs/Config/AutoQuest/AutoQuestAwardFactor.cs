namespace SlotFramework.AutoQuest {
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using Utils;

    public class AutoQuestAwardFactor {

        private AutoQuestConfig QuestConfig { get; set; }

        public AutoQuestAwardFactor(AutoQuestConfig config) {
            QuestConfig = config;
            ParseConfig();
        }

        public const string WinCoins = "win_coins";
        public const string StarWinCoinsMulti = "star_win_coins_multi";
        public const string BronzeWinCoinsMulti = "bronze_win_coins_multi";
        public const string SilverWinCoinsMulti = "silver_win_coins_multi";
        public const string GoldWinCoinsMulti = "gold_win_coins_multi";
        public const string WinCoinsLevelAdd = "win_coins_add";
        public const string WinCoinsInit = "win_coins_init";

        public Dictionary<string, object> AwardWinCoinsDict { get; set; }
        public Dictionary<string, object> WinCoinsStarMultiDict { get; set; }

        public float WinCoinsAdd { get; set; }

        public List<object> WinCoinsInitList { get; set; }

        public void ParseConfig() {
            if (QuestConfig != null && QuestConfig.AwardGeneratorDict != null &&
                QuestConfig.AwardGeneratorDict.ContainsKey(WinCoins)) {
                AwardWinCoinsDict = QuestConfig.AwardGeneratorDict[WinCoins] as Dictionary<string, object>;
                if (AwardWinCoinsDict != null) {
                    if (AwardWinCoinsDict.ContainsKey(StarWinCoinsMulti)) {
                        WinCoinsStarMultiDict = AwardWinCoinsDict[StarWinCoinsMulti] as Dictionary<string, object>;
                    }

                    if (AwardWinCoinsDict.ContainsKey(WinCoinsLevelAdd)) {
                        WinCoinsAdd = Utilities.CastValueFloat(AwardWinCoinsDict[WinCoinsLevelAdd]);
                    }

                    if (AwardWinCoinsDict.ContainsKey(WinCoinsInit)) {
                        WinCoinsInitList = AwardWinCoinsDict[WinCoinsInit] as List<object>;
                    }
                }
            } 
        }

        public float WinCoinsStarMulti(AutoQuestStarType starType) {
            if (WinCoinsStarMultiDict != null) {
                switch (starType) {
                    case AutoQuestStarType.Bronze:
                        return Utilities.CastValueFloat(WinCoinsStarMultiDict[BronzeWinCoinsMulti]);
                    case AutoQuestStarType.Silver:
                        return Utilities.CastValueFloat(WinCoinsStarMultiDict[SilverWinCoinsMulti]);
                    case AutoQuestStarType.Gold:
                        return Utilities.CastValueFloat(WinCoinsStarMultiDict[GoldWinCoinsMulti]);
                }
            }
            return 0;
        }

        public int WinCoinsAward(int questIndex, AutoQuestStarType starType) {
            if (WinCoinsInitList == null) {
                return 0;
            }

            int baseWinCoins = 0;
            if (questIndex < WinCoinsInitList.Count) {
                baseWinCoins = Utilities.CastValueInt(WinCoinsInitList[questIndex]);
            } else {
                baseWinCoins = Utilities.CastValueInt(WinCoinsInitList[WinCoinsInitList.Count - 1]) +
                    (int)((questIndex - WinCoinsInitList.Count) * WinCoinsAdd);
            }

            return Mathf.CeilToInt(baseWinCoins * WinCoinsStarMulti(starType));
        }

        public List<AutoQuestAward> CreateQuestAwards(int questIndex, AutoQuestStarType starType) {

            List<AutoQuestAward> questAwards = new List<AutoQuestAward>();

            AutoQuestAward questAward = null;
            AutoQuestAwardType awardType = (AutoQuestAwardType)Random.Range(0, (int)AutoQuestAwardType.AllAwardCount);
            switch(awardType) {
                case AutoQuestAwardType.Coins: {
                        int winCoins = WinCoinsAward(questIndex, starType);
                        questAward = new AutoQuestAward(AutoQuestAwardType.Coins, winCoins);
                    }
                    break;
            }

            if (questAward != null) {
                questAwards.Add(questAward);
            }

            return questAwards;
        }
    }
}