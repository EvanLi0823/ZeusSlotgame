using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using SlotFramework.AutoQuest;
using TMPro;
using UI.Utils;
using Classic;
using Utils;

public class AutoQuestCatalogListItem : MonoBehaviour {

    public Sprite NormalBackgroundSprite;
    public Sprite FinishedBackgroundSprite;

    public Sprite ActiveTitleBackgroundSprite;
    public Sprite NormalTitleBackgroundSprite;
    public Sprite FinishedTitleBackgroundSprite;

    public Sprite BronzeStarSprite;
    public Sprite SilverStarSprite;
    public Sprite GoldStarSprite;

    public Image BackgroundImage;
    public Image TitleBackgroundImage;

    public Image BronzeStarImage;
    public Image SilverStarImage;
    public Image GoldStarImage;

    public GameObject BronzeStarLight;
    public GameObject SilverStarLight;
    public GameObject GoldStarLight;

    public TextMeshProUGUI QuestName;

    public List<Image> MachineIconsList;
    public List<TextMeshProUGUI> MachineRelateionAndsList;
    public List<TextMeshProUGUI> MachineRelateionOrsList;

    public int QuestIndex { get; set; }

    public AutoQuestCatalogDialog QuestCatalogDialog { get; set; }

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        
    }
    
    public AutoQuest Quest { get; set; }

    public void SetAutoQuest(AutoQuest quest) {
        if (quest == null) {
            return;
        }

        Quest = quest;
        QuestIndex = quest.QuestIndex;

        Refresh();
    }

    public void Refresh() {
        if (Quest == null) {
            return;
        }

        if (Quest.IsAllQuestTermsFinished()) {
            BackgroundImage.sprite = FinishedBackgroundSprite;
            TitleBackgroundImage.sprite = FinishedTitleBackgroundSprite;
        } else {
            BackgroundImage.sprite = NormalBackgroundSprite;
            if (Quest.QuestIndex == BaseGameConsole.ActiveGameConsole().autoQuestManager.CurrentProcessingQuestIndex) {
                TitleBackgroundImage.sprite = ActiveTitleBackgroundSprite;
            } else {
                TitleBackgroundImage.sprite = NormalTitleBackgroundSprite;
            }
        }

        for (int i = 0; i < Quest.QuestTermsList.Count; i++) {
            AutoQuestTerm questTerm = Quest.QuestTermsList[i];
            bool showLight = Quest.ProcessingQusetTerm != null ? Quest.ProcessingQusetTerm == questTerm : false;

            switch (questTerm.StarType) {
                case AutoQuestStarType.Bronze: {
                        if (questTerm.IsQuestItemsAccomplished()) {
                            BronzeStarImage.sprite = BronzeStarSprite;
                            BronzeStarImage.color = Color.white;
                        } else {
                            BronzeStarImage.sprite = null;
                            BronzeStarImage.color = Color.clear;

                            if (BronzeStarLight != null) {
                                BronzeStarLight.SetActive(showLight);
                            }
                        }
                    }
                    break;

                case AutoQuestStarType.Silver: {
                        if (questTerm.IsQuestItemsAccomplished()) {
                            SilverStarImage.sprite = SilverStarSprite;
                            SilverStarImage.color = Color.white;
                        } else {
                            SilverStarImage.sprite = null;
                            SilverStarImage.color = Color.clear;

                            if (SilverStarLight != null) {
                                SilverStarLight.SetActive(showLight);
                            }
                        }

                    }
                    break;

                case AutoQuestStarType.Gold: {
                        if (questTerm.IsQuestItemsAccomplished()) {
                            GoldStarImage.sprite = GoldStarSprite;
                            GoldStarImage.color = Color.white;
                        } else {
                            GoldStarImage.sprite = null;
                            GoldStarImage.color = Color.clear;

                            if (GoldStarLight != null) {
                                GoldStarLight.SetActive(showLight);
                            }
                        }
                    }
                    break;

                default:
                    break;
            }
        }

        QuestName.text = string.Format("{0}# ", Quest.QuestIndex + 1) + AutoQuestManager.QestItemTypeString(Quest.ItemType);

        if (Quest.QuestTermsList.Count > 0) {
            AutoQuestMachinesChosen machineChosen = Quest.MachinesChosen;

            int machineIndex = 0;
            int machineIconIndex = 0;
            for (machineIndex = machineChosen.MachineNamesList.Count - 1,
                machineIconIndex = MachineIconsList.Count - 1;
                machineIconIndex >= 0;
                machineIconIndex--,
                machineIndex--) {

                int machineRelationshipIconIndex = machineIconIndex - 1;

                bool showMachineRelationshipIcon = machineIndex > 0;
                if (machineRelationshipIconIndex >= 0) {
                    MachineRelateionAndsList[machineRelationshipIconIndex].gameObject.SetActive(showMachineRelationshipIcon);
                    MachineRelateionOrsList[machineRelationshipIconIndex].gameObject.SetActive(showMachineRelationshipIcon);
                }

                bool showMachineIcon = machineIndex >= 0;
                if (machineIconIndex >= 0) {
                    MachineIconsList[machineIconIndex].gameObject.SetActive(showMachineIcon);
                }

                if (machineIndex >= 0) {
                    string machineName = machineChosen.MachineNamesList[machineIndex];

                    if (machineRelationshipIconIndex >= 0) {
                        if (MachineRelateionAndsList[machineRelationshipIconIndex].gameObject.activeSelf &&
                            MachineRelateionOrsList[machineRelationshipIconIndex].gameObject.activeSelf) {
                            MachineRelateionAndsList[machineRelationshipIconIndex].gameObject.SetActive(
                                machineChosen.MachineRelationship == AutoQuestMachineRelationship.And);
                            MachineRelateionOrsList[machineRelationshipIconIndex].gameObject.SetActive(
                                machineChosen.MachineRelationship == AutoQuestMachineRelationship.Or);
                        }
                    }

//                    StartCoroutine(UIUtil.LoadPictureCoroutine(
//                        BaseGameConsole.ActiveGameConsole().autoQuestManager.QuestMachineIconURL(machineName),
//                        new System.WeakReference(MachineIconsList[machineIconIndex]), true, false));
                }
            }
        }
    }

    public AutoQuestManager questManager {
        get {
            return BaseGameConsole.ActiveGameConsole().autoQuestManager;
        }
    }

    public void ScrollCellIndex(int idx) {
        if (questManager.IsQuestSavedDataLoaded == false) {
            return;
        }

        Debug.Log("ScrollCellIndex: " + idx);
        List<int> allQuestIndexesList = questManager.QuestCatalog.AllQuestIndexesList;
        if (idx >= 0 && idx < allQuestIndexesList.Count) {
            QuestIndex = allQuestIndexesList[idx];
            int questGroupIndex = AutoQuestManager.QuestGroupIndexByQuestIndex(QuestIndex);
            
            if (questManager.QuestGroupDict.ContainsKey(questGroupIndex)) {
                AutoQuestGroup questGroup = questManager.QuestGroupDict[questGroupIndex];
                AutoQuest quest = questGroup.QuestByQuestIndex(QuestIndex);
                SetAutoQuest(quest);
            } else {
                Dictionary<string, object> loadQuestGroupParameterDict = new Dictionary<string, object>();
                loadQuestGroupParameterDict["QuestIndex"] = QuestIndex;
                loadQuestGroupParameterDict["QuestGroupIndex"] = questGroupIndex;
                loadQuestGroupParameterDict["WSelf"] = new WeakReference(this);
                questManager.LoadQuestGroupFromFile(questGroupIndex, (object parametersObject) => {
                    Dictionary<string, object> paramtersDict = parametersObject as Dictionary<string, object>;
                    int qIndex = Utilities.CastValueInt(paramtersDict["QuestIndex"]);
                    int qgIndex = Utilities.CastValueInt(paramtersDict["QuestGroupIndex"]);

                    WeakReference wself = paramtersDict["WSelf"] as WeakReference;
                    if (wself != null && wself.IsAlive) {
                        AutoQuestCatalogListItem item = wself.Target as AutoQuestCatalogListItem;
                        if (item != null && qIndex == item.QuestIndex) {
                            if (questManager.QuestGroupDict.ContainsKey(qgIndex)) {
                                AutoQuestGroup qGroup = questManager.QuestGroupDict[qgIndex];
                                AutoQuest q = qGroup.QuestByQuestIndex(qIndex);
                                item.SetAutoQuest(q);
                            }
                        }
                    }
                }, loadQuestGroupParameterDict);
            }
        }
    }    
}
