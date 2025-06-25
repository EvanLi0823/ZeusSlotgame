namespace SlotFramework.AutoQuest {

    using System;
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;

    public enum AutoQuestMachineRelationship {
        Unknown = 0,
        And,
        Or
    }

    public enum AutoQuestItemType {
        None = -2,
        BetTimes = -1,
        SpinTimes,
        WinCoins,
        Experience,
        EnterFreeSpinTimes,
        SymbolAppear,
        AllTypesCount
    }

    public enum AutoQuestAwardType {
        None = -1,
        Coins,
        AllAwardCount
    }

    public enum AutoQuestStarType {
        None = -1,
        Bronze,
        Silver,
        Gold,
        AllStarCount
    }

    public enum AutoQuestTermStatus {
        None = 0,
        NotStarted,
        Processing,
        Finished
    }

    public enum AutoQuestFileStatus {
        None = 0,
        Loading,
        Saving,
        Unmodified,
        Modified,
    }

    [Serializable]
    public class AutoQuestMachinesChosen {
        public AutoQuestMachineRelationship _MachineRelationship = AutoQuestMachineRelationship.Unknown;
        public AutoQuestMachineRelationship MachineRelationship {
            get {
                return _MachineRelationship;
            }
            set {
                _MachineRelationship = value;
            }
        }

        public List<string> _MachineNameList = new List<string>();
        public List<string> MachineNamesList {
            get {
                return _MachineNameList;
            }
        }
    }

    [Serializable]
    public class AutoQuestTerm {
        public AutoQuestStarType _StarType = AutoQuestStarType.None;
        public AutoQuestStarType StarType {
            get {
                return _StarType;
            }
            set {
                _StarType = value;
            }
        }

        public AutoQuestMachineRelationship _MachinesRelationship;
        public AutoQuestMachineRelationship MachinesRelationship {
            get {
                return _MachinesRelationship;
            }
            set {
                _MachinesRelationship = value;
            }
        }

        public List<AutoQuestItem> _QuestItemList = new List<AutoQuestItem>();
        public List<AutoQuestItem> QuestItemList {
            get {
                return _QuestItemList;
            }
        }

        public List<string> MachineNamesUncompleted {
            get {
                List<string> machineNames = new List<string>();
                List<AutoQuestItem> items = null;
                if (MachinesRelationship == AutoQuestMachineRelationship.And) {
                    items = QuestItemList;
                } else if (MachinesRelationship == AutoQuestMachineRelationship.Or) {
                    items = new List<AutoQuestItem>(QuestItemList);
                    items.Sort((x, y) =>
                    x.CompletionProgress == y.CompletionProgress ? 
                    ((items.IndexOf(x)).CompareTo(items.IndexOf(y))) : -((x.CompletionProgress).CompareTo(y.CompletionProgress)));
                }

                for (int i = 0; i < items.Count; i++) {
                    AutoQuestItem questItem = items[i];
                    if (questItem.IsAmountMeetNeed == false) {
                        machineNames.Add(questItem.MachineName);
                    }
                }

                return machineNames;
            }
        }

        public bool IsQuestItemsAccomplished() {
            if (QuestItemList == null || QuestItemList.Count == 0) {
                return true;
            }

            switch(MachinesRelationship) {
                case AutoQuestMachineRelationship.And: {
                        for (int i = 0; i < QuestItemList.Count; i++) {
                            AutoQuestItem questItem = QuestItemList[i];
                            if (questItem.IsAmountMeetNeed == false) {
                                return false;
                            }
                        }
                        return true;
                    }

                case AutoQuestMachineRelationship.Or: {
                        for (int i = 0; i < QuestItemList.Count; i++) {
                            AutoQuestItem questItem = QuestItemList[i];
                            if (questItem.IsAmountMeetNeed == true) {
                                return true;
                            }
                        }
                        return false;
                    }

                default:
                    break;
            }

            return true;
        }

        public List<AutoQuestAward> _QuestAwardList = new List<AutoQuestAward>();
        public List<AutoQuestAward> QuestAwardList {
            get {
                return _QuestAwardList;
            }
        }

        public long QuestAwardCoins {
            get {
                long awardCoins = 0;

                for (int i = 0; i < QuestAwardList.Count; i++) {
                    AutoQuestAward questAward = QuestAwardList[i];
                    switch (questAward.AwardType) {
                        case AutoQuestAwardType.Coins: {
                                awardCoins += questAward.AwardAmount;
                            }
                            break;

                    }
                }
                return awardCoins;
            }
        }

        public AutoQuestTermStatus _TermStatus = AutoQuestTermStatus.None;
        public AutoQuestTermStatus TermStatus {
            get {
                return _TermStatus;
            }
            set {
                _TermStatus = value;
            }
        }
    }
}