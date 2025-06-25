using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Classic;
public class DebugTestDropDownPanel : MonoBehaviour {

	public Dropdown dropdownList;
	public Toggle TestOn;
	public Toggle FreeSpinTestOn;

	private BaseResultChange baseResultChange;
	private Dictionary<string, Dropdown.OptionData> _DicDropDown;
	private bool EnableTest;
	private bool DisableTest;
	private MachineTestDataFromPlistConfig testConfig;
	// Use this for initialization
	void OnEnable () {

		_DicDropDown=new Dictionary<string, Dropdown.OptionData>();
		DebugMachineDataTestPanel.currentCommonResultName = "";
		DebugMachineDataTestPanel.specialResultName = "";

		//清空默认节点
		dropdownList.options.Clear();
		if (BaseSlotMachineController.Instance!=null) {
			baseResultChange = BaseSlotMachineController.Instance.reelManager.gameObject.GetComponent<BaseResultChange> ();
			testConfig = BaseSlotMachineController.Instance.reelManager.machineTestDataFromPlist;
			foreach (string name in testConfig.commonTestResultsList.Keys) {
				Dropdown.OptionData op=new Dropdown.OptionData();
				op.text = name;
				dropdownList.options.Add(op);
				_DicDropDown.Add(op.text, op);
				if (DebugMachineDataTestPanel.currentCommonResultName=="") {
					DebugMachineDataTestPanel.currentCommonResultName = name;
					dropdownList.captionText.text = name;
				}
			}
		}
	}
	public void ChangeSelectedResultName(){
		DebugMachineDataTestPanel.currentCommonResultName = dropdownList.options [dropdownList.value].text;
	}
	public void ToggleEnableTestMode(){
		if (TestOn.isOn) {
			if (baseResultChange!=null) {
				baseResultChange.isTestOn = true;
			}
		} 
		else {
			if (baseResultChange!=null) {
				baseResultChange.isTestOn = false;
			}
		}
	}

	public void ToggleEnableFreeSpinTestMode(){

		if (FreeSpinTestOn.isOn) {
			if (baseResultChange!=null) {
				baseResultChange.isFreespinTestOn = true;
			}
		} 
		else {
			if (baseResultChange!=null) {
				baseResultChange.isFreespinTestOn = false;
			}
		}
		
	}

}
