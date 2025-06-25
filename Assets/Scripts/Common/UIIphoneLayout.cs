using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIIphoneLayout : MonoBehaviour {

	public float offsetX = 144;
	void Awake()
	{
		if (IphoneXAdapter.IsIphoneX ()) {
			RectTransform rectTransform = transform as RectTransform;
			if (rectTransform.anchorMax.x == 1f) {
				rectTransform.offsetMax = new Vector2 (rectTransform.offsetMax.x - offsetX, rectTransform.offsetMax.y);
				rectTransform.offsetMin = new Vector2 (rectTransform.offsetMin.x + offsetX, rectTransform.offsetMin.y);
			}
		}
	}
}
