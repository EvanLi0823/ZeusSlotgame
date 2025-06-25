using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NearIpadScale : MonoBehaviour
{
    public float scaleX = 1f;
    public float scaleY = 1f;
    void Awake () {
        RectTransform rect = this.transform as RectTransform;
       
		float screenWidth = Screen.width;
		float screenHeight = Screen.height;
		if (Screen.width < Screen.height)
		{
			screenWidth = Screen.height;
			screenHeight = Screen.width;
		}

		float radio = screenWidth / screenHeight;
		if (radio < 1.4f)
		{
			rect.localScale = new Vector3 (scaleX, scaleY, 1f);
		}
    }
}
