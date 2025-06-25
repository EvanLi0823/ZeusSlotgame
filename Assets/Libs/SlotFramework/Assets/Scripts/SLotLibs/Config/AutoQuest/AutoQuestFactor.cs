namespace SlotFramework.AutoQuest {

    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;

    public class AutoQuestFactor {

        public AutoQuestConfig QuestConfig { get; set; }
        public AutoQuestAwardFactor QuestAwardFactor { get; set; }
        public AutoQuestItemFactor QuestItemFactor { get; set; }
        public AutoQuestMachineChooser QuestMachineChooser { get; set; }

        public AutoQuestFactor(Dictionary<string, object> questConfigDict) {
            QuestConfig = new AutoQuestConfig(questConfigDict);
            QuestAwardFactor = new AutoQuestAwardFactor(QuestConfig);
            QuestItemFactor = new AutoQuestItemFactor(QuestConfig);
            QuestMachineChooser = new AutoQuestMachineChooser(QuestConfig);
        }

        public AutoQuest CreateNewQuest(int questIndex) {

            if (QuestMachineChooser == null || QuestItemFactor == null || QuestAwardFactor == null) {
                return null;
            }

            AutoQuest quest = new AutoQuest();
            quest.QuestIndex = questIndex;
            quest.QuestTermsList.Clear();
            quest.MachinesChosen = QuestMachineChooser.ChooseMachines(); ;
            quest.ItemType = (AutoQuestItemType)Random.Range(0, (int)AutoQuestItemType.AllTypesCount);

            for (int i = 0; i < (int)AutoQuestStarType.AllStarCount; i++) {
                AutoQuestTerm questTerm = new AutoQuestTerm();

                questTerm.MachinesRelationship = quest.MachinesChosen.MachineRelationship;
                questTerm.TermStatus = AutoQuestTermStatus.NotStarted;
                questTerm.StarType = (AutoQuestStarType)(i);

                questTerm.QuestItemList.Clear();
                questTerm.QuestItemList.AddRange(QuestItemFactor.CreateQuestItems(
                    quest.MachinesChosen, questIndex, (AutoQuestStarType)(i), quest.ItemType));

                questTerm.QuestAwardList.Clear();
                questTerm.QuestAwardList.AddRange(QuestAwardFactor.CreateQuestAwards(
                    questIndex, (AutoQuestStarType)(i)));

                quest.QuestTermsList.Add(questTerm);
            }
            
            return quest;
        }
    }
}