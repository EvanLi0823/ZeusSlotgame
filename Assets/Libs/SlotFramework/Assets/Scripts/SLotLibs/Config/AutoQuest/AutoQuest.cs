namespace SlotFramework.AutoQuest {

    using System;
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;

    [Serializable]
    public class AutoQuest {

        public int _QuestIndex = 0;
        public int QuestIndex {
            get {
                return _QuestIndex;
            }
            set {
                _QuestIndex = value;
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

        public AutoQuestMachinesChosen _MachinesChosen;
        public AutoQuestMachinesChosen MachinesChosen {
            get {
                return _MachinesChosen;
            }
            set {
                _MachinesChosen = value;
            }
        }

        public List<AutoQuestTerm> _QuestTermsList = new List<AutoQuestTerm>();
        public List<AutoQuestTerm> QuestTermsList {
            get {
                return _QuestTermsList;
            }
        }

        public bool PauseProcessingQuestTerm() {
            bool pauseProcessingTerm = false;
            for (int i = 0; i < QuestTermsList.Count; i++) {
                AutoQuestTerm questTerm = QuestTermsList[i];
                if (questTerm.TermStatus == AutoQuestTermStatus.Processing) {
                    questTerm.TermStatus = AutoQuestTermStatus.NotStarted;
                    pauseProcessingTerm = true;
                }
            }
            return pauseProcessingTerm;
        }

        public bool StartProcessingQuestTerm() {
            for (int i = 0; i < QuestTermsList.Count; i++) {
                AutoQuestTerm questTerm = QuestTermsList[i];
                switch (questTerm.TermStatus) {
                    case AutoQuestTermStatus.Finished:
                        continue;
                    case AutoQuestTermStatus.Processing:
                        return true;
                    case AutoQuestTermStatus.None:
                    case AutoQuestTermStatus.NotStarted: 
                        questTerm.TermStatus = AutoQuestTermStatus.Processing;
                        return true;
                    default:
                        break;
                }
            }
            return false;
        }

        public AutoQuestTerm ProcessingQusetTerm {
            get {
                for (int i = 0; i < (int)AutoQuestStarType.AllStarCount && i < QuestTermsList.Count; i++) {
                    AutoQuestTerm questTerm = QuestTermsList[i];
                    if (questTerm.TermStatus == AutoQuestTermStatus.Processing) {
                        return questTerm;
                    }
                }
                return null;
            }
        }

        public AutoQuestTerm FirstUnfinishedQuestTerm {
            get {
                for (int i = 0; i < (int)AutoQuestStarType.AllStarCount && i < QuestTermsList.Count; i++) {
                    AutoQuestTerm questTerm = QuestTermsList[i];
                    if (questTerm.TermStatus != AutoQuestTermStatus.Finished) {
                        return questTerm;
                    }
                }
                return null;
            }
        }

        public AutoQuestTerm DisplayQuestTerm {
            get {
                return
                    ProcessingQusetTerm != null ? ProcessingQusetTerm: 
                    FirstUnfinishedQuestTerm != null ? FirstUnfinishedQuestTerm : LastQuestTerm;
            }
        }

        public AutoQuestTerm LastQuestTerm {
            get {
                return QuestTermsList[(int)AutoQuestStarType.AllStarCount - 1];
            }
        }

        public bool IsAllQuestTermsFinished() {
            if (QuestTermsList == null) {
                return true;
            }

            for (int i = 0; i < QuestTermsList.Count; i++) {
                AutoQuestTerm questTerm = QuestTermsList[i];
                if (questTerm.TermStatus != AutoQuestTermStatus.Finished) {
                    return false;
                }
            }
            return true;
        }
    }
}