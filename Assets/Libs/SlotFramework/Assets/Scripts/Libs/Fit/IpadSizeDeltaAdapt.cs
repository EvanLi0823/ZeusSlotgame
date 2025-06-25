using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IpadSizeDeltaAdapt : MonoBehaviour {
	public Vector2 anchoredPosition = new Vector2(0f,0f);
	public Vector2 sizeDelta = new Vector2(0f,0f);
	public bool enableScaleX = false;
	public bool enableScaleY = false;
	public float scaleXFactor = 1f;
	public float scaleYFactor = 1f;
	void Start () {
		if (SkySreenUtils.GetScreenSizeType () == SkySreenUtils.ScreenSizeType.Size_4_3) {
			RectTransform rct = transform as RectTransform;
			if (!enableScaleX)
			{
				scaleXFactor = rct.localScale.x;
			}
			if (!enableScaleY)
			{
				scaleYFactor = rct.localScale.y;
			}
			rct.localScale = new Vector3(scaleXFactor, scaleYFactor, rct.localScale.z);
			rct.anchoredPosition = anchoredPosition;
			rct.sizeDelta = sizeDelta;
		}
	}
}
