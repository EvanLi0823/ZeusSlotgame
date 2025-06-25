using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using SlotFramework.AutoQuest;

public class AutoQuestDetailMachineQuestPanel : MonoBehaviour {

    public TextMeshProUGUI questDescriptionText;

    public TextMeshProUGUI questDescriptionPrefixText;
    public TextMeshProUGUI questDescriptionAppendixText;
    public Image questSymbolImage;

    public Slider questProgressSlider;
    public TextMeshProUGUI questProgressText;

    public string MachineName { get; set; }
    public AutoQuestItem QuestItem { get; set; }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Refresh(AutoQuestItem questItem, string machineName) {
        MachineName = machineName;
        QuestItem = questItem;

        if (questItem.ItemType != AutoQuestItemType.SymbolAppear) {
            if (questDescriptionText) {
                questDescriptionText.gameObject.SetActive(true);
            }

            if (questDescriptionPrefixText) {
                questDescriptionPrefixText.gameObject.SetActive(false);
            }

            if (questDescriptionAppendixText) {
                questDescriptionAppendixText.gameObject.SetActive(false);
            }

            if (questSymbolImage) {
                questSymbolImage.gameObject.SetActive(false);
            }
        } else {
            if (questDescriptionText) {
                questDescriptionText.gameObject.SetActive(false);
            }

            if (questDescriptionPrefixText) {
                questDescriptionPrefixText.gameObject.SetActive(true);
            }

            if (questDescriptionAppendixText) {
                questDescriptionAppendixText.gameObject.SetActive(true);
            }

            if (questSymbolImage) {
                questSymbolImage.gameObject.SetActive(true);
            }
        }

        switch(questItem.ItemType) {
            case AutoQuestItemType.SpinTimes: {
                    if (questDescriptionText != null) {
                        questDescriptionText.text = string.Format("Spin {0} times", (int)questItem.AmountNeed);
                    }
                }
                break;

            case AutoQuestItemType.WinCoins: {
                    if (questDescriptionText != null) {
                        questDescriptionText.text = string.Format("Win {0} coins", (int)questItem.AmountNeed);
                    }
                }
                break;

            case AutoQuestItemType.Experience: {
                    if (questDescriptionText != null) {
                        questDescriptionText.text = string.Format("Gain {0} exp", (int)questItem.AmountNeed);
                    }
                }
                break;

            case AutoQuestItemType.EnterFreeSpinTimes: {
                    if (questDescriptionText != null) {
                        questDescriptionText.text = string.Format("Do {0} freespin{1}", 
                            (int)questItem.AmountNeed,
                            (int)questItem.AmountNeed > 1 ? "s" : "");
                    }
                }
                break;

//            case AutoQuestItemType.SymbolAppear: {
//                    if (questDescriptionPrefixText != null) {
//                        questDescriptionPrefixText.text = "Get ";
//                    }
//
//                    if (questSymbolImage != null) {
//                        string symbolIconURL =
//                            BaseGameConsole.ActiveGameConsole().autoQuestManager.SymbolBaseURL + 
//                            MachineName + "/" + MachineName + "_" + questItem.SymbolName + ".png";
//                        StartCoroutine(UI.Utils.UIUtil.LoadPictureCoroutine(symbolIconURL, new System.WeakReference(questSymbolImage), true, false));
//                    }
//
//                    if (questDescriptionAppendixText != null) {
//                        questDescriptionAppendixText.text = string.Format( " X {0} times", (int)questItem.AmountNeed);
//                    }
//                }
//                break;

            default:
                break;
        }

        if (questProgressSlider) {
            questProgressSlider.value = questItem.CompletionProgress;
        }

        if (questProgressText) {
            questProgressText.text = string.Format ("{0} / {1}", 
                questItem.Amount > questItem.AmountNeed ? questItem.AmountNeed : questItem.Amount,
                questItem.AmountNeed);
        }
    }
}
