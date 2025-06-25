namespace SlotFramework.AutoQuest {

    using System;
    using UnityEngine;
    using System.Collections;

    [Serializable]
    public class AutoQuestItem {

        public string _MachineName = string.Empty;
        public string MachineName {
            get {
                return _MachineName;
            }
            set {
                _MachineName = value;
            }
        }

        public AutoQuestItemType _ItemType = AutoQuestItemType.None;
        public AutoQuestItemType ItemType {
            get {
                return _ItemType;
            }
            set {
                _ItemType = value;
            }
        }

        public float _AmountNeed = 0;
        public float AmountNeed {
            get {
                return _AmountNeed;
            }
            set {
                _AmountNeed = value;
            }
        }

        public float _Amount = 0;
        public float Amount {
            get {
                return _Amount;
            }
            set {
                _Amount = value;
            }
        }

        public float CompletionProgress {
            get {
                return Amount / AmountNeed;
            }
        }

        public string _SymbolName;
        public string SymbolName {
            get {
                return _SymbolName;
            }

            set {
                _SymbolName = value;
            }
        }

        // Default constructor should be kept for Serialization
        public AutoQuestItem() {

        }

        public AutoQuestItem(AutoQuestItemType itemType, string machineName, float amoutNeed, string symbolName = "", float amount = 0) {
            ItemType = itemType;
            MachineName = machineName;
            AmountNeed = amoutNeed;
            Amount = amount;
            SymbolName = symbolName;
        }

        public void AddAmount(float amount) {
            Amount += amount;
        }

        public bool IsAmountMeetNeed {
            get {
                return Amount >= AmountNeed;
            }
        }
    }
}