using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedRewardSpin : MonoBehaviour {

	public float devHeight = 9f;
	public float devWidth = 16f;
	void Awake(){
        if (SkySreenUtils.GetScreenSizeType() != SkySreenUtils.ScreenSizeType.Size_16_9)
        {
            RectTransform rc = transform as RectTransform;
            Vector2 min = rc.anchorMin;
            Vector2 max = rc.anchorMax;
            float height = max.y - min.y;
            float scaleRatio = Screen.width * devHeight / (Screen.height * devWidth);
            scaleRatio = Mathf.Clamp(scaleRatio, 0.75f, 1.333f);
            float targetHeight = height * scaleRatio;
            max.y = min.y + targetHeight;
            rc.anchorMax = max;
            rc.anchorMin = min;
            rc.offsetMax = Vector2.zero;
            rc.offsetMin = Vector2.zero;
        }
		
	}
}
