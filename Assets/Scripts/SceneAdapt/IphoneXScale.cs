using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IphoneXScale : MonoBehaviour {

	public float scaleX = 1f;
	public float scaleY = 1f;
	void Start () {
		RectTransform rect = this.transform as RectTransform;
		if (IphoneXAdapter.IsIphoneX()) {
			rect.localScale = new Vector3 (scaleX, scaleY, 1f);
		}
	}
}
