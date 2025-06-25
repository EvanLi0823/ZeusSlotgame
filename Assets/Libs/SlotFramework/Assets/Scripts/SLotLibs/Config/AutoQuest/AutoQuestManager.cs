namespace SlotFramework.AutoQuest {

    using System;
    using UnityEngine;
    using UnityEngine.Events;
    using System.Collections;
    using System.Collections.Generic;
    using Libs;
    using Utils;

    public class AutoQuestManager {

        public AutoQuestFactor QuestFactor { get; set; }
        public bool IsQuestSavedDataLoaded { get; set; }
        public string MachineBaseURL { get; set; }
        public string SymbolBaseURL { get; set; }

        public AutoQuest QuestJustFinishedTerm { get; set; }
        public AutoQuestTerm QuestTermJustFinished { get; set; }

        public const string QuestPathName = "AutoQuest";
        public const string QuestMachinePathBaseURL = "MachineIconBaseURL";
        public const string QuestSymbolIconPathBaseURL = "SymbolIconBaseURL";
        public const string QuestCatalogFileName = "QuestCatalog.txt";
        public const string QuestGroupFileNameFormat = "QuestGroup_{0}.txt";

        public const int QuestGroupUnit = 10;

        public string QuestMachineIconURL(string machineName) {
            return MachineBaseURL + machineName + ".png";
        }

        public static int QuestGroupIndexByQuestIndex(int questIndex) {
            return questIndex / QuestGroupUnit;
        }

        public static string QestItemTypeString(AutoQuestItemType itemType) {
            switch (itemType) {
                case AutoQuestItemType.SpinTimes: {
                        return "SPIN!SPIN!SPIN!";
                    }
                case AutoQuestItemType.WinCoins: {
                        return "WIN MORE PRIZE!!!";
                    }

                case AutoQuestItemType.Experience: {
                        return "LVL UP!R U READY?";
                    }

                case AutoQuestItemType.SymbolAppear: {
                        return "COLLECT SYMBOLS!!!";
                    }

                case AutoQuestItemType.EnterFreeSpinTimes: {
                        return "SPIN FOR FREE!!!";
                    }
                default:
                    break;
            }

            return string.Empty;
        }

        public AutoQuestManager() {
            Messenger.AddListener(SlotControllerConstants.OnRechargeableSpinWillStart, OnRechargeableSpinStart);
        }

        ~AutoQuestManager() {
            Messenger.RemoveListener(SlotControllerConstants.OnRechargeableSpinWillStart, OnRechargeableSpinStart);
        }

        protected bool IsDoneQuestItemAfterRechargeableSpinStarted { get; set; }
        public void OnRechargeableSpinStart() {
            IsDoneQuestItemAfterRechargeableSpinStarted = false;
        }

        public int LastProcessingQuestIndexForDisplay {
            get {
                if (QuestCatalog != null) {
                    return QuestCatalog.NewQuestIndex;
                }
                return 0;
            }
        }

        public AutoQuest CurrentProcessingQuest { get; set; }

        public AutoQuestTerm CurrentProcessingQuestTerm {
            get {
                if (CurrentProcessingQuest == null) {
                    return null;
                }

                return CurrentProcessingQuest.ProcessingQusetTerm;
            }
        }

        public float CurrentProcessingQuestTermProgress(string machineName) {
            AutoQuestTerm questTerm = CurrentProcessingQuestTerm;
            if (questTerm == null) {
                return 0;
            }

            float itemAmountsSum = 0;
            float itemAmountsNeedSum = 0;
            for (int i = 0; i < questTerm.QuestItemList.Count; i++) {
                AutoQuestItem questItem = questTerm.QuestItemList[i];
                if (questItem.MachineName == machineName) {
                    itemAmountsSum += questItem.Amount;
                    itemAmountsNeedSum += questItem.AmountNeed;
                }
            }

            if (itemAmountsNeedSum == 0) {
                return 0;
            }

            float progress =  itemAmountsSum / itemAmountsNeedSum;
            if (progress < 0) return 0;
            if (progress > 1) return 1;
            return progress;
        }


        public string CurrentProcessingQuestTermProgressString(string machineName) {
            AutoQuestTerm questTerm = CurrentProcessingQuestTerm;
            if (questTerm == null) {
                return string.Empty;
            }

            float itemAmountsSum = 0;
            float itemAmountsNeedSum = 0;
            for (int i = 0; i < questTerm.QuestItemList.Count; i++) {
                AutoQuestItem questItem = questTerm.QuestItemList[i];
                if (questItem.MachineName == machineName) {
                    itemAmountsSum += questItem.Amount;
                    itemAmountsNeedSum += questItem.AmountNeed;
                }
            }

			return string.Format(GameConstants.StringFormat_0_1_Key, itemAmountsSum, itemAmountsNeedSum);
        }

        public AutoQuestStarType CurrentProcessingQuestTermStarType { 
            get {
                AutoQuestTerm questTerm = CurrentProcessingQuestTerm;
                if (questTerm == null) {
                    return AutoQuestStarType.None;
                }
                return questTerm.StarType;
            }
        }

        public AutoQuestCatalog QuestCatalog { get; set; }
        public string QuestCatalogFileFullPath {
            get {
                return UnityUtil.SavedConfigurationFileFullPath(QuestPathName, QuestCatalogFileName);
            }
        }

        public bool QuestCatalogModified { get; set; }
        public DateTime QuestCatalogModifiedDate { get; set; }

        public void LoadAutoQuestConfig(Dictionary<string, object> questConfigDict) {
            QuestFactor = new AutoQuestFactor(questConfigDict);
            if (questConfigDict.ContainsKey(QuestMachinePathBaseURL)) {
                MachineBaseURL = questConfigDict[QuestMachinePathBaseURL] as string;
                SymbolBaseURL = questConfigDict[QuestSymbolIconPathBaseURL] as string;
            }
        }

        public void MarkQuestConfigAsModified() {
            QuestCatalogModified = true;
            QuestCatalogModifiedDate = DateTime.Now;
        }

        public string QuestGroupFileFullPath(int questGroupIndex) {
            return UnityUtil.SavedConfigurationFileFullPath(QuestPathName, string.Format(QuestGroupFileNameFormat, questGroupIndex));
        }

        public Dictionary<int, AutoQuestGroup> _QuestGroupDict = new Dictionary<int, AutoQuestGroup>();
        public Dictionary<int, AutoQuestGroup> QuestGroupDict {
            get {
                return _QuestGroupDict;
            }
        }

        public Dictionary<int, AutoQuestFileStatus> _QuestGroupStatusDict = new Dictionary<int, AutoQuestFileStatus>();
        public Dictionary<int, AutoQuestFileStatus> QuestGroupStatusDict {
            get {
                return _QuestGroupStatusDict;
            }
        }

        public Dictionary<int, DateTime> _QuestGroupModifyDateDict = new Dictionary<int, DateTime>();
        public Dictionary<int, DateTime> QuestGroupModifyDateDict {
            get {
                return _QuestGroupModifyDateDict;
            }
        }

        public void MarkQuestGroupAsModified(int questGroupIndex) {
            QuestGroupStatusDict[questGroupIndex] = AutoQuestFileStatus.Modified;
            QuestGroupModifyDateDict[questGroupIndex] = DateTime.Now;
        }

        public int CurrentProcessingQuestIndex {
            get {
                if (CurrentProcessingQuest != null) {
                    return CurrentProcessingQuest.QuestIndex;
                }
                return 0;
            }
        }

        public int CurrentProcessingQuestGroupIndex {
            get {
                return QuestGroupIndexByQuestIndex(CurrentProcessingQuestIndex);
            }
        }
        

        protected class ActionWithInfo {

            public UnityAction<object> _Action;
            public UnityAction<object> Action {
                get {
                    return _Action;
                }
                set {
                    _Action = value;
                }
            }

            private object _Info;
            public object Info {
                get {
                    return _Info;
                }
                set {
                    _Info = value;
                }
            }

            public ActionWithInfo (UnityAction<object> action, object info) {
                Action = action;
                Info = info;
            }

            public void DoActionWithInfo() {
                if (Action != null) {
                    Action(Info);
                }
            }

            public static ActionWithInfo NewInstance(UnityAction<object> action, object info) {
                ActionWithInfo actionWithInfo = new ActionWithInfo(action, info);
                return actionWithInfo;
            }
        }

        protected Dictionary<int, List<ActionWithInfo>> loadQuestGroupFromFileCallBacks = 
            new Dictionary<int, List<ActionWithInfo>>();

        public void LoadQuestGroupFromFile(int questGroupIndex, UnityAction<object> loadedQuestGroupAction, object loadQuestGroupUserInfo = null) {
            if (loadQuestGroupFromFileCallBacks.ContainsKey(questGroupIndex) == false) {
                loadQuestGroupFromFileCallBacks[questGroupIndex] = new List<ActionWithInfo>();
            }
            loadQuestGroupFromFileCallBacks[questGroupIndex].Add(new ActionWithInfo(loadedQuestGroupAction, loadQuestGroupUserInfo));

            if (QuestGroupStatusDict.ContainsKey(questGroupIndex) &&
                QuestGroupStatusDict[questGroupIndex] == AutoQuestFileStatus.Loading) {
                return;
            }

            QuestGroupStatusDict[questGroupIndex] = AutoQuestFileStatus.Loading;
            Dictionary<string, object> loadQuestGroupDict = new Dictionary<string, object>();
            loadQuestGroupDict["QuestGroupIndex"] = questGroupIndex;
            loadQuestGroupDict["LoadQuestGroupUserInfo"] = loadQuestGroupUserInfo;

            FileManager.AddFileOperation(new ReadFileMessage(QuestGroupFileFullPath(questGroupIndex), loadQuestGroupDict,
                (string fileFullPath, string fileContent, object userInfo) => {
                    Dictionary<string, object> lqgDict = userInfo as Dictionary<string, object>;
                    int gIndex = Utilities.CastValueInt(lqgDict["QuestGroupIndex"]);
                    if (QuestGroupDict.ContainsKey(gIndex) == false) {
                        QuestGroupDict[gIndex] = string.IsNullOrEmpty(fileContent) == false ? JsonUtility.FromJson<AutoQuestGroup>(fileContent) : new AutoQuestGroup();
                        QuestGroupStatusDict[gIndex] = AutoQuestFileStatus.Unmodified;
                        QuestGroupModifyDateDict[gIndex] = DateTime.Now;
                    }

                    //if (loadedQuestGroupAction != null) {
                    //    loadedQuestGroupAction(lqgDict["LoadQuestGroupUserInfo"]);
                    //}

                    if (loadQuestGroupFromFileCallBacks.ContainsKey(gIndex)) {
                        for (int i = 0; i < loadQuestGroupFromFileCallBacks[gIndex].Count; i++) {
                            ActionWithInfo ai = loadQuestGroupFromFileCallBacks[gIndex][i];
                            if (ai != null) {
                                ai.DoActionWithInfo();
                            }
                        }
                        loadQuestGroupFromFileCallBacks[gIndex].Clear();
                    }
                }));
        }

        public void SaveQuestGroupToFile(int questGroupIndex) {
            if (QuestGroupStatusDict.ContainsKey(questGroupIndex) == false ||
                QuestGroupStatusDict[questGroupIndex] != AutoQuestFileStatus.Modified) {
                return;
            }

            QuestGroupStatusDict[questGroupIndex] = AutoQuestFileStatus.Saving;
            AutoQuestGroup questGroup = QuestGroupDict[questGroupIndex];
            string questGroupJsonString = JsonUtility.ToJson(questGroup);
            Dictionary<string, object> saveQuestGroupUserInfo = new Dictionary<string, object>();
            saveQuestGroupUserInfo["GroupIndex"] = questGroupIndex;
            saveQuestGroupUserInfo["ModifyDate"] = QuestGroupModifyDateDict.ContainsKey(questGroupIndex) ? QuestGroupModifyDateDict[questGroupIndex] : DateTime.MinValue;

            FileManager.AddFileOperation(new WriteFileMessage(QuestGroupFileFullPath(questGroupIndex), questGroupJsonString, saveQuestGroupUserInfo,
                (string fileFullPath, string fileContentWritten, object userInfo) => {
                    Dictionary<string, object> userInfoDict = userInfo as Dictionary<string, object>;
                    int gIndex = Utilities.CastValueInt(userInfoDict["GroupIndex"]);
                    DateTime dt = (DateTime)userInfoDict["ModifyDate"];
                    if (QuestGroupModifyDateDict.ContainsKey(gIndex) == false || dt == QuestGroupModifyDateDict[gIndex]) {
                        QuestGroupStatusDict[gIndex] = AutoQuestFileStatus.Unmodified;
                    }
                }));
        }

        public void LoadSavedAutoQuestsFromFile() {
            FileManager.AddFileOperation(new ReadFileMessage(QuestCatalogFileFullPath, (string fileFullPath, string fileContent) => {
                if (string.IsNullOrEmpty(fileContent)) {
                    QuestCatalog = new AutoQuestCatalog();
                    AutoQuest quest = CreateNewAutoQuest();
                    StartAutoQuest(quest);
                    IsQuestSavedDataLoaded = true;
                    Messenger.Broadcast(GameConstants.AutoQuestSavedDataLoadFinish);
                } else {
                    QuestCatalog = JsonUtility.FromJson<AutoQuestCatalog>(fileContent);
                    int questIndex = QuestCatalog.CurrentProcessingQuestIndex;
                    int questGroupIndex = QuestGroupIndexByQuestIndex(QuestCatalog.CurrentProcessingQuestIndex);
                    Dictionary<string, object> loadQuestGroupParameterDict = new Dictionary<string, object>();
                    loadQuestGroupParameterDict["QuestIndex"] = questIndex;
                    loadQuestGroupParameterDict["QuestGroupIndex"] = questGroupIndex;
                    LoadQuestGroupFromFile(questGroupIndex, (object parametersObject) => {
                        Dictionary<string, object> paramtersDict = parametersObject as Dictionary<string, object>;
                        int qIndex = Utilities.CastValueInt(paramtersDict["QuestIndex"]);
                        int qgIndex = Utilities.CastValueInt(paramtersDict["QuestGroupIndex"]);
                        AutoQuest quest = QuestGroupDict[qgIndex].QuestByQuestIndex(qIndex);
                        if (quest == null) {
                            Debug.LogWarning("Quest Group File Error!");
                            // TODO: Dealing Error here. Try to recover from previous auto quest backup.
                        } else {
                            StartAutoQuest(quest);
                        }

                        // TODO: Add More QuestGroup Cache here.

                        IsQuestSavedDataLoaded = true;
                        Messenger.Broadcast(GameConstants.AutoQuestSavedDataLoadFinish);
                    }, loadQuestGroupParameterDict);
                }
            }));
        }

        public void SaveAutoQuest() {
            if (IsQuestSavedDataLoaded == false) {
                return;
            }

            if (QuestCatalogModified == true) {
                string QuestCatalogJsonString = JsonUtility.ToJson(QuestCatalog);
                FileManager.AddFileOperation(new WriteFileMessage(QuestCatalogFileFullPath, QuestCatalogJsonString, QuestCatalogModifiedDate,
                    (string fileFullPath, string fileContent, object userInfo) => {
                        DateTime lastModifyDate = (DateTime)userInfo;
                        if (lastModifyDate == QuestCatalogModifiedDate) {
                            QuestCatalogModified = false;
                        }
                    }));
            }

            if (QuestGroupStatusDict != null) {
                List<int> QuestGroupIndexes = new List<int>(QuestGroupStatusDict.Keys);
                foreach (int questGroupIndex in QuestGroupIndexes) { 
                    if (QuestGroupStatusDict[questGroupIndex] == AutoQuestFileStatus.Modified) {
                        SaveQuestGroupToFile(questGroupIndex);
                    }
                }
            }
        }

        public AutoQuest CreateNewAutoQuest() {
            AutoQuest quest = CreateAutoQuest(QuestCatalog.NewQuestIndex);
            if (quest != null) {
                QuestCatalog.NewQuestIndex++;
                QuestCatalog.ProcessingQuestIndexesList.Add(quest.QuestIndex);
                MarkQuestConfigAsModified();

                int questGroupIndex = QuestGroupIndexByQuestIndex(quest.QuestIndex);
                if (QuestGroupDict.ContainsKey(questGroupIndex)) {
                    AutoQuestGroup qg = QuestGroupDict[questGroupIndex];
                    AutoQuest q = qg.QuestByQuestIndex(quest.QuestIndex);
                    if (q != null) {
                        qg.QuestList.Remove(q);
                    }

                    qg.QuestList.Add(quest);
                    MarkQuestGroupAsModified(questGroupIndex);
                } else {
                    Dictionary<string, object> loadQuestGroupParamentersDict = new Dictionary<string, object>();
                    loadQuestGroupParamentersDict["Quest"] = quest;
                    loadQuestGroupParamentersDict["QuestIndex"] = quest.QuestIndex;
                    loadQuestGroupParamentersDict["QuestGroupIndex"] = questGroupIndex;
                    LoadQuestGroupFromFile(questGroupIndex, (object parametersObject) => {
                        Dictionary<string, object> parametersDict = parametersObject as Dictionary<string, object>;
                        int qIndex = Utilities.CastValueInt(parametersDict["QuestIndex"]);
                        int qgIndex = Utilities.CastValueInt(parametersDict["QuestGroupIndex"]);
                        AutoQuestGroup qg = null;
                        if (QuestGroupDict.ContainsKey(qgIndex)) {
                            qg = QuestGroupDict[qgIndex];
                        } else {
                            qg = new AutoQuestGroup();
                            QuestGroupDict[qgIndex] = qg;
                        }
                        AutoQuest q = qg.QuestByQuestIndex(qIndex);
                        if (q != null) {
                            qg.QuestList.Remove(q);
                        }
                        q = parametersDict["Quest"] as AutoQuest;
                        qg.QuestList.Add(q);
                        MarkQuestGroupAsModified(qgIndex);
                    }, loadQuestGroupParamentersDict);
                }
            }
            return quest;
        }

        public AutoQuest CreateAutoQuest(int questIndex) {
            if (QuestFactor != null) {
                return QuestFactor.CreateNewQuest(questIndex);
            }
            return null;
        }

        public void PauseAutoQuest(AutoQuest quest) {
            if (quest == null) {
                return;
            }

            RegisterDoAutoQuestTermItems(quest, null, false);
            bool questPaused = quest.PauseProcessingQuestTerm();

            if (questPaused) {
                int questGroupIndex = QuestGroupIndexByQuestIndex(quest.QuestIndex);
                MarkQuestGroupAsModified(questGroupIndex);
            }
        }

        public bool StartAutoQuest(AutoQuest quest) {
            if (quest == null || quest.IsAllQuestTermsFinished() == true) {
                return false;
            }

            if (quest == CurrentProcessingQuest) {
                return true;
            }

            PauseAutoQuest(CurrentProcessingQuest);
            CurrentProcessingQuest = quest;

            if (QuestCatalog.CurrentProcessingQuestIndex != quest.QuestIndex) {
                QuestCatalog.CurrentProcessingQuestIndex = quest.QuestIndex;
                MarkQuestConfigAsModified();
            }

            bool questStarted = quest.StartProcessingQuestTerm();
            RegisterDoAutoQuestTermItems(quest, null, true);

            if (questStarted) {
                int questGroupIndex = QuestGroupIndexByQuestIndex(quest.QuestIndex);
                MarkQuestGroupAsModified(questGroupIndex);
            }

            return true;
        }

        public Dictionary<int, bool> _AutoQuestItemRegisterStatus = new Dictionary<int, bool>();
        public Dictionary<int, bool> AutoQuestItemRegisterStatus {
            get {
                return _AutoQuestItemRegisterStatus;
            }
        }

        public void RegisterDoAutoQuestTermItems(AutoQuest quest, AutoQuestTerm questTerm, bool register) {
            if (quest == null) {
                return;
            }

            if (questTerm == null) {
                questTerm = quest.ProcessingQusetTerm;
            }

            if (questTerm == null) {
                return;
            }

            List<AutoQuestItem> questItems = questTerm.QuestItemList;

            for (int i = 0; i < questItems.Count; i++) {
                AutoQuestItem questItem = questItems[i];

                if (AutoQuestItemRegisterStatus.ContainsKey((int)quest.ItemType)) {
                    if (AutoQuestItemRegisterStatus[((int)quest.ItemType)] == register) {
                        continue;
                    }
                }

                AutoQuestItemRegisterStatus[((int)quest.ItemType)] = register;
                
                switch (questItem.ItemType) {
                    case AutoQuestItemType.SpinTimes: {
                            if (register) {
                                Messenger.AddListener(SlotControllerConstants.OnSpinStart, OnAddAutoQuestItemSlotSpinTimes);
                            } else {
                                Messenger.RemoveListener(SlotControllerConstants.OnSpinStart, OnAddAutoQuestItemSlotSpinTimes);
                            }
                        }
                        break;

                    case AutoQuestItemType.WinCoins: {
                            if (register) {
								Messenger.AddListener<double>(SlotControllerConstants.OnWinCoins, OnAddAutoQuestItemWinCoins);
                            } else {
								Messenger.RemoveListener<double>(SlotControllerConstants.OnWinCoins, OnAddAutoQuestItemWinCoins);
                            }
                        }
                        break;

                    case AutoQuestItemType.Experience: {
                            if (register) {
                                Messenger.AddListener<float>(SlotControllerConstants.OnGainExperience, OnAddAutoQuestItemGainExperience);
                            } else {
                                Messenger.RemoveListener<float>(SlotControllerConstants.OnGainExperience, OnAddAutoQuestItemGainExperience);
                            }
                        }
                        break;

                    case AutoQuestItemType.EnterFreeSpinTimes: {
                            if (register) {
                                Messenger.AddListener(SlotControllerConstants.OnEnterFreespin, OnAddAutoQuestItemEnterFreespin);
                            } else {
                                Messenger.RemoveListener(SlotControllerConstants.OnEnterFreespin, OnAddAutoQuestItemEnterFreespin);
                            }
                        }
                        break;

                    case AutoQuestItemType.SymbolAppear: {
                            // Sometimes ReelManager's CheckAward would change result symbols.
                            // So remain the symbol check to the win coins stage
                            if (register) {
                                Messenger.AddListener(SlotControllerConstants.OnSpinEnd, OnAddAutoQuestItemSymbolAppear);
                            } else {
                                Messenger.RemoveListener(SlotControllerConstants.OnSpinEnd, OnAddAutoQuestItemSymbolAppear);
                            }
                        }
                        break;

                    default:
                        break;
                }
            }
        }

        public delegate float AmountToAddDelegate(AutoQuestItem questItem);

        public void OnDoAutoQuestItem(AutoQuestItemType itemType, float amountToAdd) {
            OnDoAutoQuestItem(itemType, amountToAdd, null);
        }

        public void OnDoAutoQuestItem(AutoQuestItemType itemType, AmountToAddDelegate amoutToAddDelegate) {
            OnDoAutoQuestItem(itemType, 0, amoutToAddDelegate);
        }

        public void OnDoAutoQuestItem(AutoQuestItemType itemType, float amountToAdd, AmountToAddDelegate amoutToAddDelegate) {
            if (IsDoneQuestItemAfterRechargeableSpinStarted == true) {
                return;
            }

            AutoQuest quest = CurrentProcessingQuest;
            AutoQuestTerm questTerm = CurrentProcessingQuestTerm;

            if (questTerm == null) {
                return;
            }

            if (BaseGameConsole.ActiveGameConsole().IsInSlotMachine())
            {
                string currentSlotName = SwapSceneManager.Instance.GetLogicSceneName();
                List<AutoQuestItem> questItems = questTerm.QuestItemList;
                for (int i = 0; i < questItems.Count; i++) {
                    AutoQuestItem questItem = questItems[i];
                    if (questItem.MachineName != currentSlotName || questItem.ItemType != itemType) {
                        continue;
                    }

                    if (questItem.IsAmountMeetNeed) {
                        continue;
                    }

					float amount = amoutToAddDelegate == null ? amountToAdd : amoutToAddDelegate(questItem);
                    questItem.AddAmount(amount);
                    IsDoneQuestItemAfterRechargeableSpinStarted = true;
                }
            }

            MarkQuestGroupAsModified(CurrentProcessingQuestGroupIndex);
            Messenger.Broadcast(GameConstants.AutoQuestDoCurrentProcessingQuestTermItem);
            
            bool isCurrentProcessingQuestTermFinished = IsQuestTermDoneItems(questTerm);
            if (isCurrentProcessingQuestTermFinished) {

                RegisterDoAutoQuestTermItems(quest, questTerm, false);
                Messenger.Broadcast(GameConstants.AutoQuestCurrentProcessingQuestTermItemsDone);

                QuestJustFinishedTerm = quest;
                QuestTermJustFinished = questTerm;

                List<AutoQuestAward> awardList = questTerm.QuestAwardList;
                for (int i = 0; i < awardList.Count; i++) {
                    AutoQuestAward award = awardList[i];
                    award.DoAward();
                }
                questTerm.TermStatus = AutoQuestTermStatus.Finished;
                Messenger.Broadcast(GameConstants.AutoQuestCurrentProcessingQuestTermDone);

                int nextQuestIndex = -1;
                if (QuestCatalog.ProcessingQuestIndexesList.Contains(quest.QuestIndex)) {
                    int questIndexIndex = QuestCatalog.ProcessingQuestIndexesList.IndexOf(quest.QuestIndex);
                    int nextQuestIndexIndex = questIndexIndex + 1;
                    if (nextQuestIndexIndex < QuestCatalog.ProcessingQuestIndexesList.Count) {
                        nextQuestIndex = QuestCatalog.ProcessingQuestIndexesList[nextQuestIndexIndex];
                    }
                }

                bool isCurrentProcessingQuestFinished = quest.IsAllQuestTermsFinished();
                if (isCurrentProcessingQuestFinished) {
                    if (QuestCatalog.FinishedQuestIndexesList.Contains(quest.QuestIndex) == false) {
                        QuestCatalog.FinishedQuestIndexesList.Add(quest.QuestIndex);
                        MarkQuestConfigAsModified();
                    }

                    if (QuestCatalog.ProcessingQuestIndexesList.Contains(quest.QuestIndex) == true) {
                        QuestCatalog.ProcessingQuestIndexesList.Remove(quest.QuestIndex);
                        MarkQuestConfigAsModified();
                    }
                }
                StartNextAutoQuest(nextQuestIndex);

                SaveAutoQuest();
            }
        }

        public void StartNextAutoQuest(int nextQuestIndex) {
            if (nextQuestIndex == -1) {
                AutoQuest quest = CreateNewAutoQuest();
                StartAutoQuest(quest);
                Messenger.Broadcast(GameConstants.AutoQuestCurrentProcessingQuestChanged);
            } else {
                int nextQuestGroupIndex = QuestGroupIndexByQuestIndex(nextQuestIndex);
                if (QuestGroupDict.ContainsKey(nextQuestGroupIndex) == true) {
                    AutoQuestGroup questGroup = QuestGroupDict[nextQuestGroupIndex];
                    AutoQuest quest = questGroup.QuestByQuestIndex(nextQuestIndex);
                    StartAutoQuest(quest);
                    Messenger.Broadcast(GameConstants.AutoQuestCurrentProcessingQuestChanged);
                } else {
                    Dictionary<string, object> loadQuestGroupParamentersDict = new Dictionary<string, object>();
                    loadQuestGroupParamentersDict["QuestIndex"] = nextQuestIndex;
                    loadQuestGroupParamentersDict["QuestGroupIndex"] = nextQuestGroupIndex;
                    LoadQuestGroupFromFile(nextQuestGroupIndex, (object userInfo) => {
                        Dictionary<string, object> paramtersDict = userInfo as Dictionary<string, object>;
                        int qIndex = Utilities.CastValueInt(paramtersDict["QuestIndex"]);
                        int qgIndex = Utilities.CastValueInt(paramtersDict["QuestGroupIndex"]);
                        AutoQuestGroup qg = QuestGroupDict[qgIndex];
                        AutoQuest q = qg.QuestByQuestIndex(qIndex);
                        StartAutoQuest(q);
                        Messenger.Broadcast(GameConstants.AutoQuestCurrentProcessingQuestChanged);
                    }, loadQuestGroupParamentersDict);
                }
            }
        }

        public void OnAddAutoQuestItemSlotSpinTimes() {
            if (BaseGameConsole.ActiveGameConsole().SlotMachineController != null &&
                BaseGameConsole.ActiveGameConsole().SlotMachineController.isFreeRun == false) {
                OnDoAutoQuestItem(AutoQuestItemType.SpinTimes, 1);
            }
        }

		public void OnAddAutoQuestItemWinCoins(double winCoins) {
			OnDoAutoQuestItem(AutoQuestItemType.WinCoins,(float)winCoins);
        }

        public void OnAddAutoQuestItemGainExperience(float experience) {
            OnDoAutoQuestItem(AutoQuestItemType.Experience, experience);
        }

        public void OnAddAutoQuestItemEnterFreespin() {
            OnDoAutoQuestItem(AutoQuestItemType.EnterFreeSpinTimes, 1);
        }

        public void OnAddAutoQuestItemSymbolAppear() {
            OnDoAutoQuestItem(AutoQuestItemType.SymbolAppear, (AutoQuestItem questItem) => {
                string symbolName = questItem.SymbolName;
                Dictionary<int, List<int>> symbolIndexMap = BaseGameConsole.ActiveGameConsole().SlotMachineController.reelManager.GetSymbolIndexMap(symbolName);
                return Utilities.GetCountInDictList(symbolIndexMap);
            });
        }

        public bool IsQuestTermDoneItems(AutoQuestTerm questTerm) {
            if (questTerm == null) {
                return false;
            }
            return questTerm.IsQuestItemsAccomplished();
        }

        public AutoQuest LoadedQuestByQuestIndex(int questIndex) {
            int questGroupIndex = QuestGroupIndexByQuestIndex(questIndex);
            if (QuestGroupDict.ContainsKey(questGroupIndex)) {
                AutoQuestGroup questGroup = QuestGroupDict[questGroupIndex];
                return questGroup.QuestByQuestIndex(questIndex);
            }
            return null;
        }
    }
}