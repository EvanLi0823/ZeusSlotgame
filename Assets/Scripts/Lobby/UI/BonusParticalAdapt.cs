using UnityEngine;
using System.Collections;

public class BonusParticalAdapt : MonoBehaviour {

	// Use this for initialization
	void Start () {
		switch (SkySreenUtils.GetScreenSizeType ()) {

		case SkySreenUtils.ScreenSizeType.Size_4_3:
			transform.localScale = new Vector3 (0.7f, 0.7f, 1f);
			break;
		case SkySreenUtils.ScreenSizeType.Size_16_9:
			transform.localScale = new Vector3 (1f, 1f, 1f);
			break;
		default:
			break;
		}
	}
}
