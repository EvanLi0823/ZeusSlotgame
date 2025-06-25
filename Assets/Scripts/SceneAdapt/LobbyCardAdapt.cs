using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyCardAdapt : MonoBehaviour {
	public float ipadLength = 1346f;
	public float iphoneXLength = 2066f;
	void Start () {
		RectTransform rt = this.transform as RectTransform;
		if (IphoneXAdapter.IsIphoneX ()) {
			rt.sizeDelta = new Vector2 (iphoneXLength, rt.sizeDelta.y);
		}
		else if (SkySreenUtils.GetScreenSizeType () == SkySreenUtils.ScreenSizeType.Size_16_9) {

		} else if(SkySreenUtils.GetScreenSizeType () == SkySreenUtils.ScreenSizeType.Size_4_3){
			rt.sizeDelta = new Vector2 (ipadLength, rt.sizeDelta.y);
		}
	}
	

}
