using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Classic;
using UnityEngine.UI;

public class DebugMachineDataTestPanel : MonoBehaviour {
	//public const string DISABLE_TEST_PANEL = "DisableTestPanel";
	public const string JACKPOTNAME ="JackPot";

	public static string currentCommonResultName = "";
	public static string specialResultName = "";
	public bool IsShowDebugTestHUD = false;
	public GameObject DebugTestHUDPanel;
	public GameObject RAPanel;
	private bool dynamicInitData = false;
	private BaseResultChange baseResultChange;
	void Awake(){
		Messenger.AddListener (GameConstants.DO_SPIN,DisableDebugPanel);
		Messenger.AddListener (SlotControllerConstants.OnSpinEnd,OnSpinEnd);
		//Messenger.AddListener (DISABLE_TEST_PANEL,DisableDebugPanel);
	}
	void DisableDebugPanel(){
		DebugTestHUDPanel.SetActive(false);
		IsShowDebugTestHUD = false;
	}
	void OnDestroy(){
		Messenger.RemoveListener (GameConstants.DO_SPIN, DisableDebugPanel);
		Messenger.RemoveListener (SlotControllerConstants.OnSpinEnd,OnSpinEnd);
		//Messenger.RemoveListener (DISABLE_TEST_PANEL,DisableDebugPanel);
	}

	void OnSpinEnd(){
		DisableTest ();
	}
	void EnableTest(){
		#if !UNITY_EDITOR
		if (baseResultChange!=null) {
			baseResultChange.isTestOn = true;
		}
		#endif

	}
	void DisableTest(){
		#if !UNITY_EDITOR
		if (baseResultChange!=null) {
		baseResultChange.isTestOn = false;
		}
		#endif

	}
#if DEBUG || UNITY_EDITOR
    // Update is called once per frame
    void Update () {
	
		if (!dynamicInitData&&BaseSlotMachineController.Instance!=null&&BaseSlotMachineController.Instance.reelManager!=null) {
			dynamicInitData = true;
			baseResultChange = BaseSlotMachineController.Instance.reelManager.gameObject.GetComponent<BaseResultChange> ();
		}

		if (Input.touchCount == 2||Input.GetKeyDown(KeyCode.Return)) {
			if (!IsShowDebugTestHUD) {
				IsShowDebugTestHUD = true;
				if (DebugTestHUDPanel!=null) {
					DebugTestHUDPanel.SetActive(true);
					EnableTest();
				}
			}
			if (RAPanel!=null&&!RAPanel.activeSelf) {
				RAPanel.SetActive(true);
			}
		}
		if (Input.touchCount == 3||Input.GetKeyDown(KeyCode.Space)) {
			if (RAPanel!=null) {
				RAPanel.SetActive(!RAPanel.activeSelf);
			}
		}
	
	}

#endif
}
