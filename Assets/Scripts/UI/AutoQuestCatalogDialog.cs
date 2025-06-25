using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Libs;
using SlotFramework.AutoQuest;

namespace Classic
{
public class AutoQuestCatalogDialog : AutoQuestCatalogDialogBase 
{
        public AutoQuestCatalogListItem activeQuestCatalogListItem = null;
        public AutoQuestCatalogLoopScrollRectControl questScrollRectControl = null;

        protected override void Awake()
    	{
       		base.Awake();
            activeQuestCatalogListItem = ActiveQuestCatalogListItem.GetComponent<AutoQuestCatalogListItem>();
            questScrollRectControl = QuestListScroll.GetComponent<AutoQuestCatalogLoopScrollRectControl>();

            if (CloseButton != null) {
                this.CloseButton.onClick.RemoveAllListeners();
                this.CloseButton.onClick.AddListener(() => {
                    this.Close();
                });
            }

//			Messenger.AddListener<bool> (GameConstants.AutoQuestCloseAllDialogs, Close);
            Messenger.AddListener<bool> (GameConstants.AutoQuestCatalogDialogShowContentPanel, ShowContentPanel);
        }

   		public override void OnButtonClickHandler(GameObject go)
    	{
    	    base.OnButtonClickHandler(go);
    	}

        public void OnInitDialog() {
            if (BaseGameConsole.ActiveGameConsole().autoQuestManager.IsQuestSavedDataLoaded == false) {
                return;
            }

            AutoQuest currentProcessingQuest = BaseGameConsole.ActiveGameConsole().autoQuestManager.CurrentProcessingQuest;
            activeQuestCatalogListItem.SetAutoQuest(currentProcessingQuest);

            questScrollRectControl.OnInitScrollRect(this);

            if (UnlockHintText != null) {
                UnlockHintText.text = string.Format("Next challenge will unlock after finishing challenge #{0}.",
                    BaseGameConsole.ActiveGameConsole().autoQuestManager.LastProcessingQuestIndexForDisplay);
            }
        }

        protected override void Start() {
            base.Start();
            ShowContentPanel(true);
        }

        protected override void OnDestroy() {
            base.OnDestroy();
//			Messenger.RemoveListener<bool> (GameConstants.AutoQuestCloseAllDialogs, Close);
            Messenger.RemoveListener<bool>(GameConstants.AutoQuestCatalogDialogShowContentPanel, ShowContentPanel);
        }

        public void ShowContentPanel(bool show) {
            if (ContentPanel != null) {
                ContentPanel.gameObject.SetActive(show);
            }
        }
    }
}
