using UnityEngine;
using System.Collections;

public class FitRealHeight : MonoBehaviour {
//	private float devHeight = 9f;
//	private float devWidth = 16f;
	// Use this for initialization
	public float ipadRealHeight = 729f;
	void Start () {
       
        //      float scale = Screen.height * devWidth / (Screen.width * devHeight);
        if (SkySreenUtils.GetScreenSizeType() == SkySreenUtils.ScreenSizeType.Size_4_3)
        {
            RectTransform rt = this.transform as RectTransform;
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, ipadRealHeight);
        }
	}
	 
}
