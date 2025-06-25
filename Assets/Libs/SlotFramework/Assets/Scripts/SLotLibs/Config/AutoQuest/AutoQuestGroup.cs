namespace SlotFramework.AutoQuest {

    using System;
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;

    [Serializable]
    public class AutoQuestGroup {

        public List<AutoQuest> _QuestsList = new List<AutoQuest>();
        public List<AutoQuest> QuestList {
            get {
                return _QuestsList;
            }
        }

        public AutoQuest QuestByQuestIndex(int questIndex) {
            if (QuestList != null) {
                foreach (AutoQuest quest in QuestList) {
                    if (quest.QuestIndex == questIndex) {
                        return quest;
                    }
                }
            }

            return null;
        }
    }
}