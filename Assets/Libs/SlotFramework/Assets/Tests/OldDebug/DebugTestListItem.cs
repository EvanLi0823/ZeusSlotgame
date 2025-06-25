using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DebugTestListItem : MonoBehaviour {
	public Text contentItem;
	public void TestBtnClick(){
		if (contentItem.text!="NONE") {
			DebugMachineDataTestPanel.currentCommonResultName = contentItem.text;
		}
		BaseSlotMachineController.Instance.DoSpin ();
	}
}
