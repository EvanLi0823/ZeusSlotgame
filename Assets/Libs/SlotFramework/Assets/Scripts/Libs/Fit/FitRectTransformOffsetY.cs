using UnityEngine;
using System.Collections;

public class FitRectTransformOffsetY : MonoBehaviour {
	public float offsetBelow = 178f;
	public float offsetTop= -73f;
	void Awake () {
		if(SkySreenUtils.GetScreenSizeType() == SkySreenUtils.ScreenSizeType.Size_4_3){
		RectTransform mTransform = this.transform as RectTransform;
//		float scale =  Screen.width * defaultHeight / (Screen.height * defaultWidth);
		mTransform.offsetMin = new Vector2 (mTransform.offsetMin.x, offsetBelow);
		mTransform.offsetMax = new Vector2 (mTransform.offsetMax.x, offsetTop);

//		mTransform.localScale = new Vector3 (1, scale, transform.localScale.z);
		}
	}
}
