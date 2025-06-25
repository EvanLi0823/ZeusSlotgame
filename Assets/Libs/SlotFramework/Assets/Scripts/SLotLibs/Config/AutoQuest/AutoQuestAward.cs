using System.Collections.Generic;namespace SlotFramework.AutoQuest {

    using System;
    using UnityEngine;
    using System.Collections;
    using Classic;
    using Utils;

    [Serializable]
    public class AutoQuestAward {

        public AutoQuestAwardType _AwardType = AutoQuestAwardType.None;
        public AutoQuestAwardType AwardType {
            get {
                return _AwardType;
            }
            set {
                _AwardType = value;
            }
        }

        public long _AwardAmount = 0;
        public long AwardAmount {
            get {
                return _AwardAmount;
            }
            set {
                _AwardAmount = value;
            }
        }

        public AutoQuestAward() {

        }

        public AutoQuestAward(AutoQuestAwardType awardType, int awardAmount) {
            AwardType = awardType;
            AwardAmount = awardAmount;
        }

        public void DoAward() {
            switch(AwardType) {
                case AutoQuestAwardType.Coins: {
						if (AwardAmount > 0) {
							UserManager.GetInstance().IncreaseBalance(AwardAmount);
							Dictionary<string,object> dict = new Dictionary<string,object>();
							dict.Add("increaseCoins",AwardAmount);
							BaseGameConsole.singletonInstance.LogBaseEvent("AutoQuestAward",dict);
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        public string GetAwardDisplayString() {
            switch(AwardType) {
                case AutoQuestAwardType.Coins: {
                        return Utilities.CastToIntStringWithThousandSeparator(AwardAmount);
                    }
            }
            return string.Empty;
        }
    }
}