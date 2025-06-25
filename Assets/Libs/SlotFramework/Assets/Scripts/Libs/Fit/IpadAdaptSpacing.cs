using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class IpadAdaptSpacing : MonoBehaviour {
	public float spacing=8.5f;
	// Use this for initialization
	void Awake () {
		if (SkySreenUtils.GetScreenSizeType()==SkySreenUtils.ScreenSizeType.Size_4_3) {
			this.gameObject.GetComponent<VerticalLayoutGroup> ().spacing=spacing;

		}
	}
}
