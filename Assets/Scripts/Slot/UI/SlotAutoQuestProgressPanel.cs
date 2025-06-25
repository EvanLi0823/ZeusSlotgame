using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using SlotFramework.AutoQuest;

public class SlotAutoQuestProgressPanel : MonoBehaviour {

    public Slider questProgressSlider;
    public Image questProgressSliderBackground;
    public GameObject questProgressFillArea;
    public Material darkenMaterial;
    public GameObject questProgressParticle;
    public Image starImage;
    public Sprite emptyStarSprite;
    public Sprite bronzeStarSprite;
    public Sprite silverStarSprite;
    public Sprite goldStarSprite;
    public Image exclamationMarkImage;
    public Animator starAnimator;
    public Animator barAnimator;

    public string MachineName { get; set; }

	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnDestroy() {

    }

    public Sprite starSprite(AutoQuestStarType starType) {
        switch (starType) {
            case AutoQuestStarType.Bronze: {
                    return bronzeStarSprite;
                }

            case AutoQuestStarType.Silver: {
                    return silverStarSprite;
                }

            case AutoQuestStarType.Gold: {
                    return goldStarSprite;
                }
        }
        return null;
    }

    public void InitQuestProgressPanel(string machineName) {
        MachineName = machineName;

        AutoQuest quest = BaseGameConsole.ActiveGameConsole().autoQuestManager.CurrentProcessingQuest;
        if (quest == null || quest.MachinesChosen.MachineNamesList.Contains(machineName) == false) {
            if (questProgressFillArea != null) {
                questProgressFillArea.SetActive(false);
            }

            if (questProgressParticle != null) {
                questProgressParticle.gameObject.SetActive(false);
            }

            if (questProgressSliderBackground != null) {
                questProgressSliderBackground.material = darkenMaterial;
            }

            if (starImage != null) {
                starImage.sprite = emptyStarSprite;
            }

            if (exclamationMarkImage != null) {
                exclamationMarkImage.gameObject.SetActive(true); 
            }

        } else {
            if (questProgressSlider != null) {
                questProgressSlider.value =
                    BaseGameConsole.ActiveGameConsole().autoQuestManager.CurrentProcessingQuestTermProgress(machineName);
            }

            if (questProgressFillArea != null) {
                questProgressFillArea.SetActive(true);
            }

            if (questProgressParticle != null) {
                questProgressParticle.gameObject.SetActive(true);
            }

            if (questProgressSliderBackground != null) {
                questProgressSliderBackground.material = null;
            }

            if (starImage != null) {
                AutoQuestStarType starType = BaseGameConsole.ActiveGameConsole().autoQuestManager.CurrentProcessingQuestTermStarType;
                starImage.sprite = starSprite(starType);
            }

            if (exclamationMarkImage != null) {
                exclamationMarkImage.gameObject.SetActive(false);
            }
        }
    }

    public void UpdateQuestProgressIfNeeded() {
        AutoQuest quest = BaseGameConsole.ActiveGameConsole().autoQuestManager.CurrentProcessingQuest;
        if (quest != null && quest.MachinesChosen.MachineNamesList.Contains(MachineName) == true) {
            float currentMachineQuestProgress =
                BaseGameConsole.ActiveGameConsole().autoQuestManager.CurrentProcessingQuestTermProgress(MachineName);
            
            bool changeQuestProgressSliderValue = false;
            if (questProgressSlider != null) {
                changeQuestProgressSliderValue =
                    currentMachineQuestProgress > 0 && 
                    !Mathf.Approximately(questProgressSlider.value, currentMachineQuestProgress);
            }

            if (changeQuestProgressSliderValue) {
                if (questProgressSlider != null) {
                    questProgressSlider.value = currentMachineQuestProgress;

                    (new Libs.DelayAction(2, () => {
                        barAnimator.SetInteger("state", 1);
                    }, () => {
                        barAnimator.SetInteger("state", 0);

                        float progress = 
                        BaseGameConsole.ActiveGameConsole().autoQuestManager.CurrentProcessingQuestTermProgress(MachineName);

                        if (Mathf.Approximately(progress, 1.0f)) {
                            if (questProgressFillArea != null) {
                                questProgressFillArea.SetActive(false);
                            }

                            if (questProgressParticle != null) {
                                questProgressParticle.gameObject.SetActive(false);
                            }

                            if (questProgressSliderBackground != null) {
                                questProgressSliderBackground.material = darkenMaterial;
                            }

                            if (starImage != null) {
                                starImage.sprite = emptyStarSprite;
                            }

                            if (exclamationMarkImage != null) {
                                exclamationMarkImage.gameObject.SetActive(true);
                            }
                        }
                    })).Play();

                    if (questProgressFillArea != null) {
                        questProgressFillArea.SetActive(true);
                    }

                    if (questProgressParticle != null) {
                        questProgressParticle.gameObject.SetActive(true);
                    }

                    if (questProgressSliderBackground != null) {
                        questProgressSliderBackground.material = null;
                    }

                    if (starImage != null) {
                        AutoQuestStarType starType = BaseGameConsole.ActiveGameConsole().autoQuestManager.CurrentProcessingQuestTermStarType;
                        starImage.sprite = starSprite(starType);
                    }

                    if (exclamationMarkImage != null) {
                        exclamationMarkImage.gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    public void OnQuestProgressBarClicked() {
        if (BaseGameConsole.ActiveGameConsole().autoQuestManager.CurrentProcessingQuest != null) {
            Dictionary<string, object> openAutoQuestDetailDialogParameters = new Dictionary<string, object>();
            openAutoQuestDetailDialogParameters["OpenAsModel"] = true;
            openAutoQuestDetailDialogParameters["Quest"] = BaseGameConsole.ActiveGameConsole().autoQuestManager.CurrentProcessingQuest;
            openAutoQuestDetailDialogParameters["QuestTerm"] = BaseGameConsole.ActiveGameConsole().autoQuestManager.CurrentProcessingQuestTerm;
            Messenger.Broadcast<Dictionary<string, object>>(GameDialogManager.OpenAutoQuestDetailDialog, openAutoQuestDetailDialogParameters);
        }
    }

    public void PlayProgressDoneAnimation() {
        if (BaseGameConsole.ActiveGameConsole().autoQuestManager.QuestJustFinishedTerm != null &&
            BaseGameConsole.ActiveGameConsole().autoQuestManager.QuestTermJustFinished != null) {
            (new Libs.DelayAction(3, () => {
                starAnimator.SetInteger("state", 1);
            }, () => {
                starAnimator.SetInteger("state", 0);
            })).Play();
        }
    }
}
