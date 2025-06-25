using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Fixed top panel.
/// 因为对话框都缩小到原来的0.75，所以左右边缘对其缩小了总体大小的一半
/// </summary>
public class FixedTopPanel : MonoBehaviour {
	public float devHeight = 9f;
	public float devWidth = 16f;
	public Vector2 LT;
	public Vector2 RB;
	void Awake(){
		if (SkySreenUtils.GetScreenSizeType () != SkySreenUtils.ScreenSizeType.Size_16_9) {
			RectTransform rc = transform as RectTransform;

			Vector2 min = rc.anchorMin;
			Vector2 max = rc.anchorMax;
			float factor = (Screen.height * devWidth) / (Screen.width * devHeight)/2;
			float diffFactor = Mathf.Abs (factor - 1) / 2;
			rc.anchorMin = new Vector2 (min.x,min.y-diffFactor);
			rc.anchorMax = new Vector2 (max.y,max.y+diffFactor);
			rc.offsetMin = new Vector2 (LT.x,LT.y);
			rc.offsetMax = new Vector2 (RB.x,RB.y);
		}

	}
}
