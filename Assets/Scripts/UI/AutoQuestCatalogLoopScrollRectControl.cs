using UnityEngine;
using System.Collections.Generic;
using SlotFramework.AutoQuest;
using UnityEngine.UI;
using Classic;


public class AutoQuestCatalogLoopScrollRectControl : MonoBehaviour {

    public AutoQuestCatalogDialog QuestCatalogDialog { get; set; }
    public LoopVerticalScrollRect verticalScrollRect;

    public List<int> FinishedQuestIndexesList = null;
    public List<int> ProcessingQuestIndexesList = null;
    public List<int> AllQuestIndexesList = new List<int>();
    
    public AutoQuestManager questManager {
        get {
            return BaseGameConsole.ActiveGameConsole().autoQuestManager;
        }
    }

    public void OnInitScrollRect(AutoQuestCatalogDialog questCatalogDialog) {
        QuestCatalogDialog = questCatalogDialog;
        FinishedQuestIndexesList = questManager.QuestCatalog.FinishedQuestIndexesList;
        ProcessingQuestIndexesList = questManager.QuestCatalog.ProcessingQuestIndexesList;
        AllQuestIndexesList = questManager.QuestCatalog.AllQuestIndexesList;
    }

    private void Start() {
        // (new Libs.DelayAction(0.2f, null, () => {
        //     verticalScrollRect.totalCount = AllQuestIndexesList.Count;
        //     verticalScrollRect.ItemTypeEnd = verticalScrollRect.ItemTypeStart = (FinishedQuestIndexesList.Count + 1);
        // })).Play();
    }
}
