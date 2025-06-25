using UnityEngine;
using System.Collections;
using SlotFramework.AutoQuest;
using Libs;

public class AutoQuestDetailMachineDownloadPanel : MonoBehaviour {

    public string MachineName { get; set; }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Refresh(string machineName) {
        MachineName = machineName;
    }

    public void OnMachineDownloadButtonClicked() {
//        SlotMachineConfig slotMachineConfig = BaseGameConsole.ActiveGameConsole().SlotMachineConfig(MachineName);
//        if (Libs.AssetBundleLoadManager.Instance.IsUseAssetBundle() && !string.IsNullOrEmpty(slotMachineConfig.RemoteBundleURL())) {
//            if (Libs.AssetBundleLoadManager.Instance.NeedDownLoadAssetBundle(slotMachineConfig.Name(), slotMachineConfig.RemoteBundleURL())) {
//                Libs.AssetBundleLoadManager.Instance.Download(slotMachineConfig);
//            }
//        }
    }
}
