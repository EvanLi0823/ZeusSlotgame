namespace SlotFramework.AutoQuest {

    using UnityEngine;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Classic;
    using Utils;

    public class AutoQuestMachineChooser {

        private AutoQuestConfig QuestConfig { get; set; }
        private DataMining DataMining {
            get {
                return DataManager.GetInstance().DataMining;
            }
        }

        public const string MachineChooser = "machine_chooser";
        public const string MachineList = "machine_list";
        public const string ChooseRelationsip = "choose_relationship";
        public const string RelationshipAnd = "relationship_and";
        public const string RelationshipProbability = "relationship_probability";
        public const string MachineCount = "machine_count";
        public const string Count = "count";
        public const string Probability = "probability";
        public const string AllCountProbability = "all_count_probability";
        public const string RelationshipOr = "relationship_or";
        public const string ChooseStrategy = "choose_strategy";
        public const string UsualMachine = "usual_machine";
        public const string NewMachine = "new_machine";
        public const string NowMachine = "now_machine";
        public const string AllMachine = "all_machine";
        public const string AllStrategyProbability = "all_strategy_probability";

        public AutoQuestMachineChooser(AutoQuestConfig config) {
            QuestConfig = config;
            ParseConfig();
        }

        public void ParseConfig() {
            if (QuestConfig != null &&
                QuestConfig.ItemGeneratorDict != null &&
                QuestConfig.ItemGeneratorDict.ContainsKey(MachineChooser)) {
                MachineChooserDict = QuestConfig.ItemGeneratorDict[MachineChooser] as Dictionary<string, object>;
            }

            if (MachineChooserDict != null &&
                MachineChooserDict.ContainsKey(MachineList)) {
                List<object> machines = MachineChooserDict[MachineList] as List<object>;
                MachinesList = new List<string>();
                for (int i = 0; i < machines.Count; i++) {
                    MachinesList.Add(machines[i] as string);
                }
            }

            if (MachineChooserDict != null &&
                MachineChooserDict.ContainsKey(ChooseRelationsip)) {
                ChooseRelationshipDict = MachineChooserDict[ChooseRelationsip] as Dictionary<string, object>;
            }

            if (ChooseRelationshipDict != null &&
                ChooseRelationshipDict.ContainsKey(RelationshipAnd)) {
                RelationshipAndDict = ChooseRelationshipDict[RelationshipAnd] as Dictionary<string, object>;
                SetRelationshipDictAllProbability(RelationshipAndDict);
            }

            if (ChooseRelationshipDict != null &&
                ChooseRelationshipDict.ContainsKey(RelationshipOr)) {
                RelationshipOrDict = ChooseRelationshipDict[RelationshipOr] as Dictionary<string, object>;
                SetRelationshipDictAllProbability(RelationshipOrDict);
            }

            if (MachineChooserDict != null && MachineChooserDict.ContainsKey(ChooseStrategy)) {
                ChooseStrategyDict = MachineChooserDict[ChooseStrategy] as Dictionary<string, object>;

                if (ChooseStrategyDict != null && ChooseStrategyDict.ContainsKey(AllStrategyProbability) == false) {
                    float allStrategyProbability =
                        Utilities.CastValueFloat(ChooseStrategyDict[UsualMachine]) +
                        Utilities.CastValueFloat(ChooseStrategyDict[NewMachine]) +
                        Utilities.CastValueFloat(ChooseStrategyDict[NowMachine]) +
                        Utilities.CastValueFloat(ChooseStrategyDict[AllMachine]);
                    ChooseStrategyDict[AllStrategyProbability] = allStrategyProbability;
                }
            }
        }

        public Dictionary<string, object> MachineChooserDict { get; set; }

        public List<string> MachinesList { get; set; }

        public Dictionary<string, object> ChooseRelationshipDict { get; set; }

        public Dictionary<string, object> RelationshipAndDict { get; set; }
        
        public Dictionary<string, object> RelationshipOrDict { get; set; }

        public void SetRelationshipDictAllProbability(Dictionary<string, object> dict) {
            if (dict != null && dict.ContainsKey(AllCountProbability) == false) {
                List<object> machineCountList = dict[MachineCount] as List<object>;
                float allMachineCountProbability = 0;
                for (int i = 0; i < machineCountList.Count; i++) {
                    Dictionary<string, object> machineCountDict = machineCountList[i] as Dictionary<string, object>;
                    if (machineCountDict != null) {
                        allMachineCountProbability += Utilities.CastValueFloat(machineCountDict[Probability]);
                    }
                }
                dict[AllCountProbability] = allMachineCountProbability;
            }
        }
        
        public Dictionary<string, object> ChooseStrategyDict { get; set; }

        public AutoQuestMachineRelationship RandomMachineRelationship() {
            if (RelationshipAndDict == null || RelationshipOrDict == null) {
                return AutoQuestMachineRelationship.Unknown;
            }

            float andProbability = Utilities.CastValueFloat(RelationshipAndDict[RelationshipProbability]);
            float orProbability = Utilities.CastValueFloat(RelationshipOrDict[RelationshipProbability]);
            float allProbability = andProbability + orProbability;
            float randomProbability = UnityEngine.Random.Range(0, allProbability);
            if (randomProbability < andProbability) {
                return AutoQuestMachineRelationship.And;
            } else {
                return AutoQuestMachineRelationship.Or;
            }
        }


        public int RandomMachineCount(AutoQuestMachineRelationship machineRelationship) {
            Dictionary<string, object> relationshipDict =
                machineRelationship == AutoQuestMachineRelationship.And ? RelationshipAndDict : 
                machineRelationship == AutoQuestMachineRelationship.Or ? RelationshipOrDict : null;

            if (relationshipDict != null) {
                float randomMachineCountProbability = UnityEngine.Random.Range(0, Utilities.CastValueFloat(relationshipDict[AllCountProbability]));

                List<object> machineCountList = relationshipDict[MachineCount] as List<object>;
                float lastSumMachineCountProbability = 0;
                float sumMachineCountProbability = 0;
                Dictionary<string, object> randomMachineCountDict = null;

                for (int i = 0; i < machineCountList.Count; i++) {
                    Dictionary<string, object> machineCountDict = machineCountList[i] as Dictionary<string, object>;
                    if (machineCountDict == null) {
                        continue;
                    }

                    lastSumMachineCountProbability = sumMachineCountProbability;
                    sumMachineCountProbability += Utilities.CastValueFloat(machineCountDict[Probability]);
                    if (lastSumMachineCountProbability <= randomMachineCountProbability &&
                        randomMachineCountProbability < sumMachineCountProbability) {
                        randomMachineCountDict = machineCountDict;
                        break;
                    }
                }

                if (randomMachineCountDict != null) {
                    if (randomMachineCountDict.ContainsKey(Count)) {
                        return Utilities.CastValueInt(randomMachineCountDict[Count]);
                    }
                }
            }

            return 0;
        }



        public AutoQuestMachinesChosen ChooseMachines() {
            if (QuestConfig == null) {
                return null;
            }

            AutoQuestMachinesChosen machinesChosen = new AutoQuestMachinesChosen();
            AutoQuestMachineRelationship machineRelationship = RandomMachineRelationship();
            machinesChosen.MachineRelationship = machineRelationship;
            int randomMachineCount = RandomMachineCount(machineRelationship);

            if (randomMachineCount > 0) {
                List<string> ChosenMachineNames = new List<string>();
                
                while (ChosenMachineNames.Count < randomMachineCount) {
                    string randomMachineName = RandomChooseMachine();
                    if (string.IsNullOrEmpty(randomMachineName)) {
                        Debug.LogWarning("WANING: CHOOSE EMPTY MACHINE!");
                        continue; 
                    }

                    if (ChosenMachineNames.Contains(randomMachineName)) {
                        continue;
                    }

                    if (MachinesList != null && MachinesList.Count > 0 &&
                        MachinesList.Contains(randomMachineName) == false) {
                        continue;
                    }
                    ChosenMachineNames.Add(randomMachineName);
                }

                machinesChosen.MachineNamesList.Clear();
                machinesChosen.MachineNamesList.AddRange(ChosenMachineNames);
                return machinesChosen;
            }

            return null;
        }

        public string RandomChooseMachine() {
            float mostSpinTimesMachineProbability = Utilities.CastValueFloat(ChooseStrategyDict[UsualMachine]);
            float justUnlockedMachineProbability = Utilities.CastValueFloat(ChooseStrategyDict[NewMachine]);
            float playingMachineProbability = Utilities.CastValueFloat(ChooseStrategyDict[NowMachine]);
            float allMachineProbability = Utilities.CastValueFloat(ChooseStrategyDict[AllMachine]);

            if (ChooseStrategyDict != null) {
                float allStrategyProbability = Utilities.CastValueFloat(ChooseStrategyDict[AllStrategyProbability]);
                float randomStrategyProbability = UnityEngine.Random.Range(0, allStrategyProbability);
                string randomStrategyMachineName = null;

                float lastSumStrategyProbility = 0;
                float sumStrategyProbability = 0;
                foreach(KeyValuePair<string, object> strategyProbability in ChooseStrategyDict) {
                    lastSumStrategyProbility = sumStrategyProbability;
                    sumStrategyProbability += Utilities.CastValueFloat(strategyProbability.Value);
                    if (lastSumStrategyProbility <= randomStrategyProbability && 
                        randomStrategyProbability < sumStrategyProbability) {
                        randomStrategyMachineName = strategyProbability.Key;
                        break;
                    }
                }

                if (randomStrategyMachineName != null) {
                    switch(randomStrategyMachineName) {
                        case UsualMachine:
                            return DataMining.MachineNameWithMostSpinTimes;
                        case NewMachine:
                            List<string> justUnlockedMachineNames = DataMining.MachineNameListJustUnlocked;
                            return justUnlockedMachineNames[UnityEngine.Random.Range(0, justUnlockedMachineNames.Count)];
                        case NowMachine:
                            return DataMining.MachineNameJustSpined;
                        case AllMachine:
                            List<string> allUnlockedMachineNames = DataMining.AllUnlockedMachineNamesList;
                            return allUnlockedMachineNames[UnityEngine.Random.Range(0, allUnlockedMachineNames.Count)];
                    }
                }
            }

            return null;
        }
    }
}