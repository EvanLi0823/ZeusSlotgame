namespace SlotFramework.AutoQuest {

    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;

    public class AutoQuestConfig {

        public const string QuestGenerator = "quest_generator";
        public const string AwardGenerator = "award_generator";
        public const string ItemGenerator = "item_generator";

        public Dictionary<string, object> QuestConfigDict { get; set; }

        public AutoQuestConfig(Dictionary<string, object> questConfigDict) {
            QuestConfigDict = questConfigDict;
            ParseConfig();
        }

        public void ParseConfig() {
            if (QuestConfigDict != null && QuestConfigDict.ContainsKey(QuestGenerator)) {
                QuestGeneratorDict = QuestConfigDict[QuestGenerator] as Dictionary<string, object>;
            }

            if (QuestGeneratorDict != null && QuestGeneratorDict.ContainsKey(ItemGenerator)) {
                ItemGeneratorDict = QuestGeneratorDict[ItemGenerator] as Dictionary<string, object>;
            }

            if (QuestGeneratorDict != null && QuestGeneratorDict.ContainsKey(AwardGenerator)) {
                AwardGeneratorDict = QuestGeneratorDict[AwardGenerator] as Dictionary<string, object>;
            }
        }

        public Dictionary<string, object> QuestGeneratorDict { get; set; }
        
        public Dictionary<string, object> ItemGeneratorDict { get; set; }

        public Dictionary<string, object> AwardGeneratorDict { get; set; }
    }
}