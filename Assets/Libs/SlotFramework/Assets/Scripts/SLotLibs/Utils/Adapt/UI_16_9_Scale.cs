using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_16_9_Scale : MonoBehaviour {

	public float scaleX = 1f;
	public float scaleY = 1f;
	void Awake () {
		#if UNITY_EDITOR
		SkySreenUtils.SetScreenResolutions();
		#endif
		if (IphoneXAdapter.IsIphoneX()) return;
		RectTransform rect = this.transform as RectTransform;
		if (SkySreenUtils.GetScreenSizeType () == SkySreenUtils.ScreenSizeType.Size_16_9) {
			rect.localScale = new Vector3 (scaleX, scaleY, 1f);
		}
	}
}
