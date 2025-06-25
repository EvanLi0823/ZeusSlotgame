using UnityEngine;
using System.Collections;

public class IPadAnchorSet : MonoBehaviour {

	public Vector2 anchorMin = new Vector2(0, 0);
	public Vector2 anchorMax = new Vector2(1, 1);
	void Start () {
		switch (SkySreenUtils.GetScreenSizeType ()) {

		case SkySreenUtils.ScreenSizeType.Size_4_3:
			RectTransform rect = transform as RectTransform;
			rect.anchorMax = anchorMax;
			rect.anchorMin = anchorMin;
			break;
		default:
			break;

		}
	}
}
