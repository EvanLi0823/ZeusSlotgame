using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Classic;
public class DebugTestListViewPanel : MonoBehaviour {



	private BaseResultChange baseResultChange;
	private List<GameObject> ListViewItems=new List<GameObject> ();
	public GameObject ItemGo;
	private bool EnableTest;
	private bool DisableTest;
	private MachineTestDataFromPlistConfig testConfig;
	// Use this for initialization
	void Awake () {

		DebugMachineDataTestPanel.currentCommonResultName = "";
		DebugMachineDataTestPanel.specialResultName = "";
		for (int i = 0; i < ListViewItems.Count; i++) {
			Destroy (ListViewItems [i]);
		}
		ListViewItems.Clear ();

	}

	void Start()
	{
		if (BaseSlotMachineController.Instance!=null) {
			baseResultChange = BaseSlotMachineController.Instance.reelManager.gameObject.GetComponent<BaseResultChange> ();
			testConfig = BaseSlotMachineController.Instance.reelManager.machineTestDataFromPlist;
			foreach (string name in testConfig.commonTestResultsList.Keys) {
				GameObject go = Instantiate (ItemGo) as GameObject;
				go.GetComponent<DebugTestListItem> ().contentItem.text = name;
				go.transform.SetParent(this.transform);
				RectTransform rec = go.transform as RectTransform;
				rec.localPosition = Vector3.zero;
				rec.offsetMax = Vector2.zero;
				rec.offsetMin = Vector2.zero;
				rec.localScale = Vector3.one;
				ListViewItems.Add (go);
			}
		}
	}
}
