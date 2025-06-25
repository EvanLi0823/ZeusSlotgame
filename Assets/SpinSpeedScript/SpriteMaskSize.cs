using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteMaskSize : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Awake() {
		SpriteMask mask = GetComponent<SpriteMask> ();
		RectTransform rt = this.transform as RectTransform;
		mask.size = rt.rect.size;
	}
}
