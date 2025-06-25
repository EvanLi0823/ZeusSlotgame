using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HsvColorChangeShader : MonoBehaviour {
	public Color targetColor;
	public Material material;
	// Use this for initialization
	void Start () {
		material = GetComponent<Material> ();
	}
	
	// Update is called once per frame
	void Update () {
		material.SetColor("_TargetColor", targetColor);	
	}
}
