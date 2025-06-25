namespace SlotFramework.AutoQuest {

    using System;
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;

    [Serializable]
    public class AutoQuestCatalog {

        public List<int> _FinishedQuestIndexesList = new List<int>();
        public List<int> FinishedQuestIndexesList {
            get {
                return _FinishedQuestIndexesList;
            }
        }

        public int _CurrentProcessingQuestIndex = 0;
        public int CurrentProcessingQuestIndex {
            get {
                return _CurrentProcessingQuestIndex;
            }
            set {
                _CurrentProcessingQuestIndex = value;
            }
        }

        public List<int> _ProcessingQuestIndexesList = new List<int>();
        public List<int> ProcessingQuestIndexesList {
            get {
                return _ProcessingQuestIndexesList;
            }
        }

        public List<int> AllQuestIndexesList {
            get {
                List<int> allQuestIndexes = new List<int>();
                allQuestIndexes.AddRange(FinishedQuestIndexesList);
                allQuestIndexes.AddRange(ProcessingQuestIndexesList);
                return allQuestIndexes;
            }
        }

        public int _NewQuestIndex = 0;
        public int NewQuestIndex {
            get {
                return _NewQuestIndex;
            }
            set {
                _NewQuestIndex = value;
            }
        }
    }
}