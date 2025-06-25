using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SliderProcessEffect : MonoBehaviour {

	public Transform effectTrans;
	public Transform start;
	public Transform end;
	public Slider slider;
	public void RefreshUI(){
		if (slider == null||start==null||end==null||effectTrans==null)
			return;
		float localPosX = Mathf.Lerp (start.localPosition.x, end.localPosition.x, slider.value);
		effectTrans.localPosition = new Vector3 (localPosX,effectTrans.localPosition.y,effectTrans.localPosition.z);
	}
}
