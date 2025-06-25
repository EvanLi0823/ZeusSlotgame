using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IphoneScale : MonoBehaviour {

	public float scaleX = 1f;
	public float scaleY = 1f;
	void Start () {
		RectTransform rect = this.transform as RectTransform;
		if (SkySreenUtils.GetScreenSizeType () == SkySreenUtils.ScreenSizeType.Size_16_9) {
			rect.localScale = new Vector3 (scaleX, scaleY, 1f);
		}
	}

}
