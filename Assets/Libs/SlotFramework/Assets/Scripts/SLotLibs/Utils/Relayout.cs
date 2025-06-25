using UnityEngine;
using System.Collections;

public class Relayout : MonoBehaviour {

	// Use this for initialization
	void Start () {
		RectTransform rt = transform as RectTransform;
		rt.anchorMin = new Vector2 (0, 0);
		rt.anchorMax = new Vector2 (1, 1);
		rt.offsetMax = new Vector2 (0, 0);
		rt.offsetMin = new Vector2 (0, 0);
	}

}
