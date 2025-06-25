namespace SlotFramework.AutoQuest {

    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using Utils;

    public class AutoQuestItemFactor {

        private AutoQuestConfig QuestConfig { get; set; }

        public AutoQuestItemFactor(AutoQuestConfig config) {
            QuestConfig = config;
            ParseConfig();
        }

        public const string ItemSpecification = "item_specification";
        public const string BetTimes = "bet_times";
        public const string StarTimesMulti = "star_times_multi";
        public const string BronzeStarTimesMulti = "bronze_star_times_multi";
        public const string SilverStarTimesMulti = "silver_star_times_multi";
        public const string GoldStarTimesMulti = "gold_star_times_multi";
        public const string LevelTimesAdd = "level_times_add";
        public const string TimesInit = "times_init";
        public const string SpinTimes = "spin_times";
        public const string TimesMulti = "times_multi";
        public const string WinCoins = "win_coins";
        public const string CoinsMultiBase = "coins_multi_base";
        public const string CoinsMultiAdjust = "coins_multi_adjust";
        public const string Exp = "exp";
        public const string ExpDiv = "exp_div";
        public const string SymbolAppear = "symbol_appear";
        public const string Name = "Name";
        public const string Value = "Value";
        public const string FreeSpin = "free_spin";

        public Dictionary<string, object> ItemSpecificationDict { get; set; }

        protected Dictionary<string, object> ItemDict(AutoQuestItemType itemType) {
            if (ItemSpecificationDict == null) {
                return null;
            }

            string itemKey =
                itemType == AutoQuestItemType.BetTimes ? BetTimes :
                itemType == AutoQuestItemType.SpinTimes ? SpinTimes :
                itemType == AutoQuestItemType.WinCoins ? WinCoins :
                itemType == AutoQuestItemType.Experience ? Exp :
                itemType == AutoQuestItemType.EnterFreeSpinTimes ? FreeSpin :
                itemType == AutoQuestItemType.SymbolAppear ? SymbolAppear : null;

            if (itemKey == null) {
                return null;
            }

            if (ItemSpecificationDict.ContainsKey(itemKey)) {
                return ItemSpecificationDict[itemKey] as Dictionary<string, object>;
            }

            return null;
        }

        public Dictionary<string, object> ItemBetTimesDict { get; set; }

        public List<object> ItemBetTimesInitList { get; set; }

        public float ItemBetTimesLevelAdd { get; set; }

        public Dictionary<string, object> ItemBetTimesStarMultiDict { get; set; }

        public float ItemBetTimesStarMulti(AutoQuestStarType starType) {
            if (ItemBetTimesStarMultiDict != null) {
                switch (starType) {
                    case AutoQuestStarType.Bronze:
                        return Utilities.CastValueFloat(ItemBetTimesStarMultiDict[BronzeStarTimesMulti]);
                    case AutoQuestStarType.Silver:
                        return Utilities.CastValueFloat(ItemBetTimesStarMultiDict[SilverStarTimesMulti]);
                    case AutoQuestStarType.Gold:
                        return Utilities.CastValueFloat(ItemBetTimesStarMultiDict[GoldStarTimesMulti]);
                }
            }
            return 0;
        }

        public Dictionary<string, object> ItemSpinTimesDict { get; set; }

        public float ItemSpinTimesMulti { get; set; }

        public Dictionary<string, object> ItemWinCoinsDict { get; set; }

        public float WinCoinsMultiBase { get; set; }
        public float WinCoinsMultiAdjust { get; set; }

        public float WinCoinsMulti { get; set; }

        public Dictionary<string, object> ItemExperienceDict { get; set; }

        public float ExperienceDivider { get; set; }

        public Dictionary<string, object> ItemFreeSpinDict { get; set; }

        public Dictionary<string, object> ItemSymbolAppearDict { get; set; }

        private void ParseConfig() {
            if (QuestConfig != null &&
                QuestConfig.ItemGeneratorDict != null &&
                 QuestConfig.ItemGeneratorDict.ContainsKey(ItemSpecification)) {
                ItemSpecificationDict = QuestConfig.ItemGeneratorDict[ItemSpecification] as Dictionary<string, object>;
            }
            
            ItemBetTimesDict = ItemDict(AutoQuestItemType.BetTimes);
            if (ItemBetTimesDict != null) {
                ItemBetTimesInitList = ItemBetTimesDict[TimesInit] as List<object>;
                ItemBetTimesLevelAdd = Utilities.CastValueFloat(ItemBetTimesDict[LevelTimesAdd]);
                ItemBetTimesStarMultiDict = ItemBetTimesDict[StarTimesMulti] as Dictionary<string, object>;
            }

            ItemSpinTimesDict = ItemDict(AutoQuestItemType.SpinTimes);
            if (ItemSpinTimesDict != null) {
                ItemSpinTimesMulti = Utilities.CastValueFloat(ItemSpinTimesDict[TimesMulti]);
            }

            ItemWinCoinsDict = ItemDict(AutoQuestItemType.WinCoins);
            if (ItemWinCoinsDict != null) {
                WinCoinsMultiBase = Utilities.CastValueFloat(ItemWinCoinsDict[CoinsMultiBase]);
                WinCoinsMultiAdjust = Utilities.CastValueFloat(ItemWinCoinsDict[CoinsMultiAdjust]);
                WinCoinsMulti = WinCoinsMultiBase * WinCoinsMultiAdjust;
            }

            ItemExperienceDict = ItemDict(AutoQuestItemType.Experience);
            if (ItemExperienceDict != null) {
                ExperienceDivider = Utilities.CastValueFloat(ItemExperienceDict[ExpDiv]);
            }

            ItemFreeSpinDict = ItemDict(AutoQuestItemType.EnterFreeSpinTimes);
            ItemSymbolAppearDict = ItemDict(AutoQuestItemType.SymbolAppear);
        }


        public int BetTimesNeed(int questIndex, AutoQuestStarType starType) {
            if (ItemBetTimesInitList == null) {
                return 0;
            }

            int baseBetTimes = 0;
            if (questIndex < ItemBetTimesInitList.Count) {
                baseBetTimes = Utilities.CastValueInt(ItemBetTimesInitList[questIndex]);
            } else {
                baseBetTimes = Utilities.CastValueInt(ItemBetTimesInitList[ItemBetTimesInitList.Count - 1]) +
                    (int)((questIndex - ItemBetTimesInitList.Count) * ItemBetTimesLevelAdd);
            }

            return Mathf.CeilToInt(baseBetTimes * ItemBetTimesStarMulti(starType));
        }

        public int SpinTimesNeed(int questIndex, AutoQuestStarType starType) {
            int betTimesNeed = BetTimesNeed(questIndex, starType);
            return Mathf.CeilToInt(betTimesNeed * ItemSpinTimesMulti);
        }

        public int WinCoinsNeed(int questIndex, AutoQuestStarType starType) {
            int betTimesNeed = BetTimesNeed(questIndex, starType);
            return Mathf.CeilToInt(betTimesNeed * WinCoinsMulti);
        }

        public int ExperienceNeed(int questIndex, AutoQuestStarType starType) {
            int betTimesNeed = BetTimesNeed(questIndex, starType);
            return Mathf.CeilToInt(betTimesNeed * WinCoinsMultiBase / ExperienceDivider);
        }

        public int SymbolAppearNeed(int questIndex, AutoQuestStarType starType, string machineName, out string symbolName) {
            if (ItemSymbolAppearDict != null && ItemSymbolAppearDict.ContainsKey(machineName)) {
                List<object> symbolList = ItemSymbolAppearDict[machineName] as List<object>;
                if (symbolList != null) {
                    Dictionary<string, object> symbolDetailDict = symbolList[Random.Range(0, symbolList.Count)] as Dictionary<string, object>;
                    if (symbolDetailDict != null) {
                        if (symbolDetailDict.ContainsKey(Name) &&
                            symbolDetailDict.ContainsKey(Value)) {
                            symbolName = symbolDetailDict[Name] as string;
                            int betTimesNeed = BetTimesNeed(questIndex, starType);
                            return Mathf.CeilToInt(Utilities.CastValueFloat(symbolDetailDict[Value]) * betTimesNeed);
                        }
                    }
                }
            }
            symbolName = string.Empty;
            return 0;
        }

        public int FreeSpinNeed(int questIndex, AutoQuestStarType starType, string machineName) {
            if (ItemFreeSpinDict != null && ItemFreeSpinDict.ContainsKey(machineName)) {
                int betTimesNeed = BetTimesNeed(questIndex, starType);
                return Mathf.CeilToInt(Utilities.CastValueFloat(ItemFreeSpinDict[machineName]) * betTimesNeed);
            }
            return 0;
        }

        public List<AutoQuestItem> CreateQuestItems(AutoQuestMachinesChosen chosenMachines, 
            int questIndex, AutoQuestStarType starType, AutoQuestItemType itemType) {

            List<AutoQuestItem> questItems = new List<AutoQuestItem>();

            for (int i = 0; i < chosenMachines.MachineNamesList.Count; i++) {
                AutoQuestItem questItem = null;
                string machineName = chosenMachines.MachineNamesList[i];
                
                switch (itemType) {
                    case AutoQuestItemType.SpinTimes: {
                            int spinTimesNeed = SpinTimesNeed(questIndex, starType);
                            if (chosenMachines.MachineRelationship == AutoQuestMachineRelationship.And) {
                                spinTimesNeed = Mathf.CeilToInt(spinTimesNeed / ((float)chosenMachines.MachineNamesList.Count));
                            }
                            questItem = new AutoQuestItem(AutoQuestItemType.SpinTimes, machineName, spinTimesNeed);
                        }
                        break;

                    case AutoQuestItemType.WinCoins: {
                            int winCoinsNeed = WinCoinsNeed(questIndex, starType);
                            if (chosenMachines.MachineRelationship == AutoQuestMachineRelationship.And) {
                                winCoinsNeed = Mathf.CeilToInt(winCoinsNeed / ((float)chosenMachines.MachineNamesList.Count));
                            }
                            questItem = new AutoQuestItem(AutoQuestItemType.WinCoins, machineName, winCoinsNeed);
                        }
                        break;

                    case AutoQuestItemType.Experience: {
                            int experienceNeed = ExperienceNeed(questIndex, starType);
                            if (chosenMachines.MachineRelationship == AutoQuestMachineRelationship.And) {
                                experienceNeed = Mathf.CeilToInt(experienceNeed / ((float)chosenMachines.MachineNamesList.Count));
                            }
                            questItem = new AutoQuestItem(AutoQuestItemType.Experience, machineName, experienceNeed);
                        }
                        break;

                    case AutoQuestItemType.EnterFreeSpinTimes: {
                            int enterFreeSpinTimesNeed = FreeSpinNeed(questIndex, starType, machineName);
                            if (chosenMachines.MachineRelationship == AutoQuestMachineRelationship.And) {
                                enterFreeSpinTimesNeed = Mathf.CeilToInt(enterFreeSpinTimesNeed / ((float)chosenMachines.MachineNamesList.Count));
                            }
                            questItem = new AutoQuestItem(AutoQuestItemType.EnterFreeSpinTimes, machineName, enterFreeSpinTimesNeed);
                        }
                        break;

                    case AutoQuestItemType.SymbolAppear: {
                            string symbolName = string.Empty;
                            int symbolAppearNeed = SymbolAppearNeed(questIndex, starType, machineName, out symbolName);
                            if (chosenMachines.MachineRelationship == AutoQuestMachineRelationship.And) {
                                symbolAppearNeed = Mathf.CeilToInt(symbolAppearNeed / ((float)chosenMachines.MachineNamesList.Count));
                            }
                            questItem = new AutoQuestItem(AutoQuestItemType.SymbolAppear, machineName, symbolAppearNeed, symbolName);
                        }
                        break;
                    default:
                        break;
                }

                if (questItem != null) {
                    questItems.Add(questItem);
                }
            }

            return questItems;
        }
    }
}