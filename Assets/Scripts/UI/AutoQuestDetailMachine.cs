using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using SlotFramework.AutoQuest;
using Classic;
using UI.Utils;

public class AutoQuestDetailMachine : MonoBehaviour {

    public Image machineIconImage;
    public Button machineIconButton;

    public AutoQuestDetailMachineDownloadPanel DownloadPanel;
    public AutoQuestDetailMachineDownloadingPanel DownloadingPanel;
    public AutoQuestDetailMachineQuestPanel QuestPanel;

    public enum AutoQuestDetailMachineStatus {
        None,
        NeedDownload,
        Downloading,
        Ready,
    }
        
    private AutoQuestDetailMachineStatus _MachineStatus = AutoQuestDetailMachineStatus.Ready;
    public AutoQuestDetailMachineStatus MachineStatus {
        get {
            return _MachineStatus;
        }
        set {
            _MachineStatus = value;

            DownloadPanel.gameObject.SetActive(_MachineStatus == AutoQuestDetailMachineStatus.NeedDownload);
            DownloadingPanel.gameObject.SetActive(_MachineStatus == AutoQuestDetailMachineStatus.Downloading);
            QuestPanel.gameObject.SetActive(_MachineStatus == AutoQuestDetailMachineStatus.Ready);
        }
    }

    public string MachineName { get; set; }

    public AutoQuest Quest { get; set; }

    public AutoQuestTerm QuestTerm { get; set; }

    public bool EnterMachineOnDownloadSuccess { get; set; }

    void Awake() {
        if (machineIconButton != null) {
            machineIconButton.onClick.RemoveAllListeners();
            machineIconButton.onClick.AddListener(() => {
                if (IsMachineReady == true) {
                    EnterMachine();
                } else {
                    EnterMachineOnDownloadSuccess = true;
                }
            });
        }
    }
    
    public void Refresh(AutoQuest quest, AutoQuestTerm questTerm, string machineName) {
        Quest = quest;
        QuestTerm = questTerm;
        MachineName = machineName;
        EnterMachineOnDownloadSuccess = false;

        Refresh();
    }

    public void Refresh() {
//        SlotMachineConfig slotMachineConfig = BaseGameConsole.ActiveGameConsole().SlotMachineConfig(MachineName);
//        if (Libs.AssetBundleLoadManager.Instance.IsUseAssetBundle() && !string.IsNullOrEmpty(slotMachineConfig.RemoteBundleURL())) {
//            if (Libs.AssetBundleLoadManager.Instance.IsInDownloadQueue(slotMachineConfig)) {
//                MachineStatus = AutoQuestDetailMachineStatus.Downloading;
//            } else if (Libs.AssetBundleLoadManager.Instance.NeedDownLoadAssetBundle(slotMachineConfig.Name(), slotMachineConfig.RemoteBundleURL())) {
//                MachineStatus = AutoQuestDetailMachineStatus.NeedDownload;
//            } else {
//                MachineStatus = AutoQuestDetailMachineStatus.Ready;
//            }
//        }
//
//        StartCoroutine(UIUtil.LoadPictureCoroutine(
//            BaseGameConsole.ActiveGameConsole().autoQuestManager.QuestMachineIconURL(MachineName), 
//            new WeakReference(this.machineIconImage), true, false));
//
//        if (DownloadPanel.gameObject.activeSelf) {
//            DownloadPanel.Refresh(MachineName);
//        }
//
//        if (DownloadingPanel.gameObject.activeSelf) {
//            DownloadingPanel.Refresh(MachineName);
//        }
//
//        if (QuestPanel.gameObject.activeSelf) {
//            AutoQuestItem questItem = null;
//            for (int i = 0; i < QuestTerm.QuestItemList.Count; i++) {
//                if (QuestTerm.QuestItemList[i].MachineName == MachineName) {
//                    questItem = QuestTerm.QuestItemList[i];
//                    break;
//                }
//            }
//            QuestPanel.Refresh(questItem, MachineName);
//        }
    }


    public bool IsMachineReady {
        get {
            return MachineStatus == AutoQuestDetailMachineStatus.Ready;
        }
    }

    public void DownloadMachine() {
        if (MachineStatus == AutoQuestDetailMachineStatus.NeedDownload) {
            Refresh();
            if (MachineStatus == AutoQuestDetailMachineStatus.NeedDownload) {
                EnterMachineOnDownloadSuccess = true;
                SlotMachineConfig slotMachineConfig = BaseGameConsole.ActiveGameConsole().SlotMachineConfig(MachineName);
                BaseGameConsole.ActiveGameConsole().autoQuestManager.StartAutoQuest(Quest);
            }
        } else if (MachineStatus == AutoQuestDetailMachineStatus.Downloading) {
            EnterMachineOnDownloadSuccess = true;
        }
    }

    public void OnSlotAssetBundleDownloadDidFail(SlotMachineConfig config, int step, string errorMessage = null) {
        if (config.Name() == MachineName) {
            Refresh();
        }
    }

    public void OnSlotAssetBundleDownloadDidSuccess(SlotMachineConfig config) {
        if (config.Name() == MachineName) {
            Refresh();

            if (EnterMachineOnDownloadSuccess) {
                if (Quest == BaseGameConsole.ActiveGameConsole().autoQuestManager.CurrentProcessingQuest) {
                    EnterMachine();
                }
            }
        }
    }

    public void EnterMachine() {
        if (Quest != null) {
            BaseGameConsole.ActiveGameConsole().autoQuestManager.StartAutoQuest(Quest);
        }

		if (BaseGameConsole.ActiveGameConsole ().IsInSlotMachine () == true &&
		   BaseGameConsole.ActiveGameConsole ().SlotMachineController.slotMachineConfig.Name () != MachineName) {
			BaseGameConsole.ActiveGameConsole ().SlotMachineController.StopAllAnimation ();
			Libs.CoroutineUtil.Instance.StopAllCoroutines ();
			Libs.UIManager.Instance.CloseAll ();
			BaseSlotMachineController.Is_Quit = true;
		}

        Messenger.Broadcast<bool> (GameConstants.AutoQuestCloseAllDialogs, true);
        
        if (BaseGameConsole.ActiveGameConsole().IsInLobby() == true || 
            (BaseGameConsole.ActiveGameConsole().IsInSlotMachine() == true &&
            BaseGameConsole.ActiveGameConsole().SlotMachineController.slotMachineConfig.Name() != MachineName)) {
            SlotMachineConfig slotMachineConfig = BaseGameConsole.ActiveGameConsole().SlotMachineConfig(MachineName);
            Libs.AudioEntity.Instance.PlayEnterRoomEffect();
            Messenger.Broadcast<System.Action, object>(GameDialogManager.OpenSceneExchange, null, slotMachineConfig);
        }
    }

    public void OnSlotAssetBundleDownloading(SlotMachineConfig config, float progress) {
        if (config.Name() == MachineName) {
            if (DownloadingPanel.gameObject.activeSelf == false) {
                Refresh();
            }

            if (DownloadingPanel != null) {
                DownloadingPanel.SetDownloadingProgress(progress);
            }
        }
    }
}
