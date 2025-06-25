using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IpadScale : MonoBehaviour {

	public float scaleX = 1f;
	public float scaleY = 1f;
	void Awake () {
		RectTransform rect = this.transform as RectTransform;
		if (SkySreenUtils.GetScreenSizeType () == SkySreenUtils.ScreenSizeType.Size_4_3) {
			if(IsRealPad())
			{
				rect.localScale = new Vector3(scaleX, scaleY, 1f);
			}
		}
		
		
	}
	bool IsRealPad(){
		float screenWidth = Screen.width;
		float screenHeight = Screen.height;
		
			float v = screenHeight / screenWidth;
			
			if (v > 1)
			{
				v = 1 / v;
			}

		if (Mathf.Abs(v - 0.75f ) <0.05)
		{
			return true;
		}

		return false;
	}
}
