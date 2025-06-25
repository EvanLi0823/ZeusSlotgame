using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Libs;
using SlotFramework.AutoQuest;
using DG.Tweening;
using Utils;
using TMPro;

namespace Classic
{
public class AutoQuestDetailDialog : AutoQuestDetailDialogBase {
        public AutoQuest Quest { get; set; }
        public AutoQuestTerm QuestTerm { get; set; }
        public int QuestIndexLoading { get; set; }
        public List<int> QuestIndexesList { get; set; }
        public AutoQuestCatalogListItem currentAutoQuestTitleCatalogListItem;
        public AutoQuestDetailMachinesPanel threeMachinesPanel;
        public AutoQuestDetailMachinesPanel twoMachinesPanel;

        public Vector2 backButtonAnchorMin;
        public Vector2 backButtonAnchorMax;

        public Vector2 backButtonAnchorCenterXMin;
        public Vector2 backButtonAnchorCenterXMax;

         protected override void Awake()
    	{
       		base.Awake();
            if (CurrentAutoQuestItem != null) {
                currentAutoQuestTitleCatalogListItem = CurrentAutoQuestItem.GetComponent<AutoQuestCatalogListItem>();
            }

            if (ThreeMachinesPanel != null) {
                threeMachinesPanel = ThreeMachinesPanel.GetComponent<AutoQuestDetailMachinesPanel>();
            }

            if (TwoMachinesPanel != null) {
                twoMachinesPanel = TwoMachinesPanel.GetComponent<AutoQuestDetailMachinesPanel>();
            }

            if (PreArrow != null) {
                PreArrow.onClick.AddListener(() => {
                    OnPreButtonClicked();
                });
            }

            if (NextArrow != null) {
                NextArrow.onClick.AddListener(() => {
                    OnNextButtonClicked();
                });
            }

            if (BackButton != null) {
                BackButton.onClick.AddListener(() => {
                    OnBackButtonClicked();
                });
            }

            if (StartButton != null) {
                StartButton.onClick.AddListener(() => {
                    OnStartButtonClicked();
                });
            }

            if (CloseButton != null) {
                CloseButton.onClick.AddListener(() => {
                    Messenger.Broadcast<bool>(GameConstants.AutoQuestCatalogDialogShowContentPanel, true);
                    this.Close();
                });
            }

            if (BackButton != null) {
                RectTransform bakcButtonRectTransform = BackButton.transform as RectTransform;
                backButtonAnchorMax = new Vector2(bakcButtonRectTransform.anchorMax.x, bakcButtonRectTransform.anchorMax.y);
                backButtonAnchorMin = new Vector2(bakcButtonRectTransform.anchorMin.x, bakcButtonRectTransform.anchorMin.y);

                float anchorWidth = backButtonAnchorMax.x - backButtonAnchorMin.x;
                backButtonAnchorCenterXMin = new Vector2(0.5f - anchorWidth / 2.0f, backButtonAnchorMin.y);
                backButtonAnchorCenterXMax = new Vector2(0.5f + anchorWidth / 2.0f, backButtonAnchorMax.y);


            }

//			Messenger.AddListener<bool> (GameConstants.AutoQuestCloseAllDialogs, Close);
        }

   		public override void OnButtonClickHandler(GameObject go)
    	{
    	    base.OnButtonClickHandler(go);
    	}


        public AutoQuestManager questManager {
            get {
                return BaseGameConsole.ActiveGameConsole().autoQuestManager;
            }
        }

        public void OnInitDialog(bool setLastSiblingIndex, AutoQuest quest, AutoQuestTerm questTerm = null) {
            if (BaseGameConsole.ActiveGameConsole().autoQuestManager.IsQuestSavedDataLoaded == false) {
                return;
            }

            if (setLastSiblingIndex) {
                this.transform.SetAsLastSibling();
            }
            OnInitDialog(quest, questTerm);
        }

        public void OnInitDialog(AutoQuest quest, AutoQuestTerm questTerm = null) {
            if (quest == null) {
                return;
            }

            Quest = quest;
            QuestTerm = questTerm;
            if (QuestTerm == null) {
                QuestTerm = quest.DisplayQuestTerm;
            }
            QuestIndexesList = questManager.QuestCatalog.AllQuestIndexesList;

            if (currentAutoQuestTitleCatalogListItem != null) {
                currentAutoQuestTitleCatalogListItem.SetAutoQuest(Quest);
            }

            if (twoMachinesPanel != null) {
                twoMachinesPanel.gameObject.SetActive(Quest.MachinesChosen.MachineNamesList.Count == 2);
                if (twoMachinesPanel.gameObject.activeSelf) {
                    twoMachinesPanel.Refresh(Quest, QuestTerm);
                }
            }

            if (threeMachinesPanel != null) {
                threeMachinesPanel.gameObject.SetActive(Quest.MachinesChosen.MachineNamesList.Count == 3);
                if (threeMachinesPanel.gameObject.activeSelf) {
                    threeMachinesPanel.Refresh(Quest, QuestTerm);
                }
            }

            if (StarTypeAward != null) {
                string starTypeAwardString = string.Empty;
                switch(QuestTerm.StarType) {
                    case AutoQuestStarType.Bronze: {
                            starTypeAwardString += "BRONZE ";
                        }
                        break;

                    case AutoQuestStarType.Silver: {
                            starTypeAwardString += "SILVER ";
                        }
                        break;

                    case AutoQuestStarType.Gold: {
                            starTypeAwardString += "GOLD ";
                        }
                        break;

                    default:
                        break;
                }
                starTypeAwardString += "AWARDS: ";

                StarTypeAward.text = starTypeAwardString;
            }

            if (AwardAmount != null) {
                AwardAmount.text = Utilities.CastToIntStringWithThousandSeparator(QuestTerm.QuestAwardCoins);
            }
                 
            bool isInSlot = BaseGameConsole.ActiveGameConsole().IsInSlotMachine();

            if (PreArrow != null) {
                bool showPreArrow = !isInSlot && 
                    QuestIndexesList.IndexOf(Quest.QuestIndex) >  0;
                PreArrow.gameObject.SetActive(showPreArrow);
            }

            if (NextArrow != null) {
                bool showNextArrow = !isInSlot &&
                    QuestIndexesList.IndexOf(Quest.QuestIndex) < (QuestIndexesList.Count - 1);
                NextArrow.gameObject.SetActive(showNextArrow);
            }

            if (BackButton != null) {
                bool showBackButton = !isInSlot;
                BackButton.gameObject.SetActive(showBackButton);
            }

            if (StartButton != null) {
                bool showStartButton = !isInSlot &&
                    questManager.QuestCatalog.ProcessingQuestIndexesList.Contains(quest.QuestIndex);
                StartButton.gameObject.SetActive(showStartButton);

                if (BackButton != null) {
                    RectTransform backButtonRectTransform = BackButton.transform as RectTransform;

                    if (showStartButton == false) {
                        backButtonRectTransform.anchorMin = backButtonAnchorCenterXMin;
                        backButtonRectTransform.anchorMax = backButtonAnchorCenterXMax;
                    } else {
                        backButtonRectTransform.anchorMin = backButtonAnchorMin;
                        backButtonRectTransform.anchorMax = backButtonAnchorMax;
                    }
                }
            }

            if (WellDone != null) {
                bool showWellDoneText = isInSlot && questTerm != null && questTerm.TermStatus == AutoQuestTermStatus.Finished;
                WellDone.gameObject.SetActive(showWellDoneText);
            }
        }

        public void OnPreButtonClicked() {
            int index = QuestIndexesList.IndexOf(Quest.QuestIndex);
            if (index == 0) {
                return;
            }

            index--;
            int questIndex = QuestIndexesList[index];
            int questGroupIndex = AutoQuestManager.QuestGroupIndexByQuestIndex(questIndex);
            if (questManager.QuestGroupDict.ContainsKey(questGroupIndex)) {
                AutoQuestGroup questGroup = questManager.QuestGroupDict[questGroupIndex];
                AutoQuest quest = questGroup.QuestByQuestIndex(questIndex);
                ActiveQuestDetailContentPanel.transform.DOLocalMoveX(Screen.width * 2, 0.3f).OnComplete<Tween>(() => {
                    OnInitDialog(quest);
                    ActiveQuestDetailContentPanel.transform.DOLocalMoveX(-Screen.width * 2, 0f).OnComplete<Tween>(() => {
                        ActiveQuestDetailContentPanel.transform.DOLocalMoveX(0, 0.3f);
                    });
                });
            } else {
                this.QuestIndexLoading = questIndex;

                Dictionary<string, object> loadQuestGroupUserInfo = new Dictionary<string, object>();
                loadQuestGroupUserInfo["QuestIndex"] = questIndex;
                loadQuestGroupUserInfo["QuestGroupIndex"] = questGroupIndex;

                questManager.LoadQuestGroupFromFile(questGroupIndex, (object userInfo) => {
                    Dictionary<string, object> loadQuestGroupUserInfoDict = userInfo as Dictionary<string, object>;
                    int qIndex = Utilities.CastValueInt(loadQuestGroupUserInfoDict["QuestIndex"]);
                    int qgIndex = Utilities.CastValueInt(loadQuestGroupUserInfoDict["QuestGroupIndex"]);

                    if (this.QuestIndexLoading == qIndex) {
                        AutoQuestGroup questGroup = questManager.QuestGroupDict[qgIndex];
                        AutoQuest quest = questGroup.QuestByQuestIndex(qIndex);

                        ActiveQuestDetailContentPanel.transform.DOLocalMoveX(Screen.width * 2, 0.3f).OnComplete<Tween>(() => {
                            OnInitDialog(quest);
                            ActiveQuestDetailContentPanel.transform.DOLocalMoveX(-Screen.width * 2, 0f).OnComplete<Tween>(() => {
                                ActiveQuestDetailContentPanel.transform.DOLocalMoveX(0, 0.3f);
                            });
                        });
                    }
                }, loadQuestGroupUserInfo);
            }
        }

        public void OnNextButtonClicked() {
            int index = QuestIndexesList.IndexOf(Quest.QuestIndex);
            if (index == QuestIndexesList.Count - 1) {
                return;
            }

            index++;
            int questIndex = QuestIndexesList[index];
            int questGroupIndex = AutoQuestManager.QuestGroupIndexByQuestIndex(questIndex);
            if (questManager.QuestGroupDict.ContainsKey(questGroupIndex)) {
                AutoQuestGroup questGroup = questManager.QuestGroupDict[questGroupIndex];
                AutoQuest quest = questGroup.QuestByQuestIndex(questIndex);

                ActiveQuestDetailContentPanel.transform.DOLocalMoveX(-Screen.width * 2, 0.3f).OnComplete<Tween>(() => {
                    OnInitDialog(quest);
                    ActiveQuestDetailContentPanel.transform.DOLocalMoveX(Screen.width * 2, 0f).OnComplete<Tween>(() => {
                        ActiveQuestDetailContentPanel.transform.DOLocalMoveX(0, 0.3f);
                    });
                });
            } else {
                this.QuestIndexLoading = questIndex;

                Dictionary<string, object> loadQuestGroupUserInfo = new Dictionary<string, object>();
                loadQuestGroupUserInfo["QuestIndex"] = questIndex;
                loadQuestGroupUserInfo["QuestGroupIndex"] = questGroupIndex;

                questManager.LoadQuestGroupFromFile(questGroupIndex, (object userInfo) => {
                    Dictionary<string, object> loadQuestGroupUserInfoDict = userInfo as Dictionary<string, object>;
                    int qIndex = Utilities.CastValueInt(loadQuestGroupUserInfoDict["QuestIndex"]);
                    int qgIndex = Utilities.CastValueInt(loadQuestGroupUserInfoDict["QuestGroupIndex"]);

                    if (this.QuestIndexLoading == qIndex) {
                        AutoQuestGroup questGroup = questManager.QuestGroupDict[qgIndex];
                        AutoQuest quest = questGroup.QuestByQuestIndex(qIndex);

                        ActiveQuestDetailContentPanel.transform.DOLocalMoveX(-Screen.width * 2, 0.3f).OnComplete<Tween>(() => {
                            OnInitDialog(quest);
                            ActiveQuestDetailContentPanel.transform.DOLocalMoveX(Screen.width * 2, 0f).OnComplete<Tween>(() => {
                                ActiveQuestDetailContentPanel.transform.DOLocalMoveX(0, 0.3f);
                            });
                        });
                    }
                }, loadQuestGroupUserInfo);
            }
        }

        public void OnBackButtonClicked() {
            Messenger.Broadcast<bool>(GameConstants.AutoQuestCatalogDialogShowContentPanel, true);
            this.Close();
        }

        public void OnStartButtonClicked() {
            bool questStarted = BaseGameConsole.ActiveGameConsole().autoQuestManager.StartAutoQuest(Quest);

            AutoQuestDetailMachinesPanel machinesPanel = null;
            if (threeMachinesPanel != null && threeMachinesPanel.gameObject.activeSelf) {
                machinesPanel = threeMachinesPanel;
            } else if (twoMachinesPanel != null && twoMachinesPanel.gameObject.activeSelf) {
                machinesPanel = twoMachinesPanel;
            }
            
            machinesPanel.EnterMachine();
        }

        protected override void Start() {
            base.Start();
            Messenger.Broadcast<bool>(GameConstants.AutoQuestCatalogDialogShowContentPanel, false);
        }

        protected override void OnDestroy() {
            base.OnDestroy();
//			Messenger.RemoveListener<bool> (GameConstants.AutoQuestCloseAllDialogs, Close);
        }
    }
}
