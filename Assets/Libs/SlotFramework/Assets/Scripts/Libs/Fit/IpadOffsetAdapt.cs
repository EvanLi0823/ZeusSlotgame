using UnityEngine;
using System.Collections;

public class IpadOffsetAdapt : MonoBehaviour {
	public Vector2 offsetMin = new Vector2(0f,0f);
	public Vector2 offsetMax = new Vector2(0f,0f);

	void Awake () {
		if (SkySreenUtils.GetScreenSizeType () == SkySreenUtils.ScreenSizeType.Size_4_3) {
			if(IsRealPad())
			{
				RectTransform rct = transform as RectTransform;
				rct.offsetMax = offsetMax;
				rct.offsetMin = this.offsetMin;
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
	 
}
