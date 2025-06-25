using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using Libs;

namespace Classic {

    public class DataMining {

        public class MachineSpinTimes {
            public string MachineName { get; set; }
            public int SpinTimes { get; set; }

            public MachineSpinTimes(string machineName, int spinTimes = 0) {
                MachineName = machineName;
                SpinTimes = spinTimes;
            }
        }

        public const string dataMiningDataFileName = "DataMining.txt";

        public bool DataMiningSavedDataLoaded { get; set; }

        // All Machine Names are load everytime from classicConfiguration.plist, so do not save to DataMining.txt.
        public List<string> AllMachineNamesList { get; private set; } 
        public List<string> AllUnlockedMachineNamesList { get; private set; }
        public Dictionary<string, MachineSpinTimes> MachineSpinTimesDict { get; private set; }
        public string MachineNameJustSpined { get; set; }

        public bool MachineUnlockFeatureEnabled { get; set; }
        public List<string> MachineNameListJustUnlocked { get; set; }

        public DataMining() {
            AllMachineNamesList = new List<string>();
            AllUnlockedMachineNamesList = new List<string>();
            MachineSpinTimesDict = new Dictionary<string, MachineSpinTimes>();
            MachineNameListJustUnlocked = new List<string>();
            Messenger.AddListener<int, int>(GameConstants.DataMiningRecordUnlockedMachinesOnLevelUp, RecordUnlockedMachinesOnLevelUp);
        }

        ~DataMining() {
            Messenger.RemoveListener<int, int>(GameConstants.DataMiningRecordUnlockedMachinesOnLevelUp, RecordUnlockedMachinesOnLevelUp);
        }

        public string MachineNameWithMostSpinTimes {
            get {
                if (MachineSpinTimesDict.Count > 0) {
                    List<MachineSpinTimes> machineSpinTimesList = new List<MachineSpinTimes>();
                    foreach (string key in MachineSpinTimesDict.Keys) {
                        machineSpinTimesList.Add(MachineSpinTimesDict[key]);
                    }
                    if (machineSpinTimesList.Count > 0) {
                        machineSpinTimesList.Sort((x, y) => x.SpinTimes.CompareTo(y.SpinTimes));
                        return machineSpinTimesList[machineSpinTimesList.Count - 1].MachineName;
                    }
                }

                return null;
            }
        }

        public void AddSpinTime(string machineName, int spintTimesToAdd = 1) {
            if (DataMiningSavedDataLoaded == false) {
                return;
            }

            if (string.IsNullOrEmpty(machineName) == true) {
                return;
            }

            if (MachineSpinTimesDict.ContainsKey(machineName) == false) {
                MachineSpinTimesDict[machineName] = new MachineSpinTimes(machineName);
            }

            MachineSpinTimesDict[machineName].SpinTimes += spintTimesToAdd;
            MachineNameJustSpined = machineName;
        }

        public List<string> MachineNameListAvaiableAtLevel(int userLevel) {
            List<string> machineNames = new List<string>();
            List<SlotMachineConfig> slotMachines = BaseGameConsole.ActiveGameConsole().SlotMachines();

            for (int i = 0; i < slotMachines.Count; i++) {
                SlotMachineConfig machineConfig = slotMachines[i];
                if (userLevel >= machineConfig.RequiredLevel()) {
                    machineNames.Add(machineConfig.Name());
                }
            }

            return machineNames;
        }

        public void RecordUnlockedMachinesOnLevelUp(int oldLevel, int newLevel) {
            if (DataMiningSavedDataLoaded == false || MachineUnlockFeatureEnabled == false) {
                return;
            }

            List<string> machineNamesUnlocked = new List<string>();
            List<SlotMachineConfig> slotMachines = BaseGameConsole.ActiveGameConsole().SlotMachines();

            for (int i = 0; i < slotMachines.Count; i++) {
                SlotMachineConfig machineConfig = slotMachines[i];
                if (oldLevel < machineConfig.RequiredLevel() &&
                    newLevel >= machineConfig.RequiredLevel()) {
                    machineNamesUnlocked.Add(machineConfig.Name());
                }
            }

            if (machineNamesUnlocked.Count > 0) {
                MachineNameListJustUnlocked = machineNamesUnlocked;
            }

            if (MachineUnlockFeatureEnabled == true) {
                AllUnlockedMachineNamesList = MachineNameListAvaiableAtLevel(newLevel);
            }
        }

        public string DataFileFullPath {
            get {
                return UnityUtil.SavedConfigurationFileFullPath(string.Empty, dataMiningDataFileName);
            }
        }

        public void LoadConfigAndReadSavedDataFromFile() {
            List<SlotMachineConfig> slotMachines = BaseGameConsole.ActiveGameConsole().SlotMachines();
            for(int i = 0; i < slotMachines.Count; i++) {
                SlotMachineConfig machineConfig = slotMachines[i];
                AllMachineNamesList.Add(machineConfig.Name());

                if (machineConfig.RequiredLevel() > 0) {
                    MachineUnlockFeatureEnabled = true;
                }
            }

            if (MachineUnlockFeatureEnabled == false) {
                AllUnlockedMachineNamesList = AllMachineNamesList;
            } else {
                AllUnlockedMachineNamesList = MachineNameListAvaiableAtLevel(UserManager.GetInstance().UserProfile().Level());
            }
            
            FileManager.AddFileOperation(new ReadFileMessage(DataFileFullPath,
               (string fileFullPath, string fileContent) => {

                   if (string.IsNullOrEmpty(fileContent)) {
                       if (MachineUnlockFeatureEnabled == false) {
                           MachineNameListJustUnlocked = AllMachineNamesList;
                       } else {
                           MachineNameListJustUnlocked = MachineNameListAvaiableAtLevel(UserManager.GetInstance().UserProfile().Level());
                       }

                       goto ReadFileFinished;
                   }

                   DataMiningSerializable dataMiningData = JsonUtility.FromJson<DataMiningSerializable>(fileContent);

                   if (dataMiningData != null) {
                       if (dataMiningData.machineSpinTimesList.Count > 0) {
                           for (int i = 0; i < dataMiningData.machineSpinTimesList.Count; i++) {
                               MachineSpinTimesSerializable machineSpinTimesData = dataMiningData.machineSpinTimesList[i];

                               if (AllMachineNamesList.Contains(machineSpinTimesData.machineName) == true) {
                                   MachineSpinTimesDict[machineSpinTimesData.machineName] =
                                       new MachineSpinTimes(machineSpinTimesData.machineName, machineSpinTimesData.spinTimes);
                               }
                           }
                       }

                       if (AllMachineNamesList.Contains(dataMiningData.machineNameJustSpined) == true) {
                           MachineNameJustSpined = dataMiningData.machineNameJustSpined;
                       }

                       if (MachineUnlockFeatureEnabled == false) {
                           MachineNameListJustUnlocked = AllMachineNamesList;
                       } else {
                           if (dataMiningData.machineUnlockFeatureEnabled == true) {
                               if (dataMiningData.machineNameListJustUnlocked.Count > 0) {
                                   for (int i = 0; i < dataMiningData.machineNameListJustUnlocked.Count; i++) {
                                       string machineNameJustUnlock = dataMiningData.machineNameListJustUnlocked[i];

                                       if (AllMachineNamesList.Contains(machineNameJustUnlock) == true &&
                                           MachineNameListJustUnlocked.Contains(machineNameJustUnlock) == false) {
                                           MachineNameListJustUnlocked.Add(dataMiningData.machineNameListJustUnlocked[i]);
                                       }
                                   }
                               }
                           } else {
                               MachineNameListJustUnlocked = MachineNameListAvaiableAtLevel(UserManager.GetInstance().UserProfile().Level());
                           }
                       }
                   }

                   ReadFileFinished:
                   DataMiningSavedDataLoaded = true;
               }));
        }

        public void SaveDataToFile() {
            DataMiningSerializable dataMiningData = new DataMiningSerializable();
            dataMiningData.machineSpinTimesList = new List<MachineSpinTimesSerializable>();

            if (MachineSpinTimesDict.Count > 0) {
                foreach(string machineName in MachineSpinTimesDict.Keys) {
                    MachineSpinTimesSerializable machineSpinTimesData = new MachineSpinTimesSerializable();
                    machineSpinTimesData.machineName = machineName;
                    machineSpinTimesData.spinTimes = MachineSpinTimesDict[machineName].SpinTimes;
                    dataMiningData.machineSpinTimesList.Add(machineSpinTimesData);
                }
            }

            dataMiningData.machineNameJustSpined = MachineNameJustSpined;
            dataMiningData.machineUnlockFeatureEnabled = MachineUnlockFeatureEnabled;
            dataMiningData.machineNameListJustUnlocked = MachineNameListJustUnlocked;

            string fileContent = JsonUtility.ToJson(dataMiningData);

            FileManager.AddFileOperation(new WriteFileMessage(DataFileFullPath, fileContent,
                (string fileFullPath, string fileContentWritten) => {

                }));
        }
    }

    [Serializable]
    public class MachineSpinTimesSerializable {
        public string machineName;
        public int spinTimes;
    }

    [Serializable]
    public class DataMiningSerializable {
        public List<MachineSpinTimesSerializable> machineSpinTimesList;
        public string machineNameJustSpined;
        public bool machineUnlockFeatureEnabled;
        public List<string> machineNameListJustUnlocked;
    }

} 