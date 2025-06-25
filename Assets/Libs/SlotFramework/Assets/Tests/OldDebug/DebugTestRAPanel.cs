using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DebugTestRAPanel : MonoBehaviour {

	public readonly static string RADATA_CHANGE = "RADataChange";
	public Text curUseCondition;
	public Text curUseR;
	public Text CurUseA;
	void Awake(){
		#if !DEBUG && !UNITY_EDITOR
		this.gameObject.SetActive(false);
		return;
		#endif

		Messenger.AddListener<string,string,string> (RADATA_CHANGE,ChangeTextUIContent);
	}

	void OnDestroy(){
		#if !DEBUG && !UNITY_EDITOR
		return;
		#endif
		Messenger.RemoveListener<string,string,string> (RADATA_CHANGE,ChangeTextUIContent);
	}
	void ChangeTextUIContent(string condition,string r,string a){
		if (curUseCondition!=null) {
			curUseCondition.text = "curCondtion:"+condition;
		}
		if (curUseR!=null) {
			curUseR.text = "curUseR:" + r;
		}
		if (CurUseA!=null) {
			CurUseA.text = "curUseA:" + a;
		}
	}
}
