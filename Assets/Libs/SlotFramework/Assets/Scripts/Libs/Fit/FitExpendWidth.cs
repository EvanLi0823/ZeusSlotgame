using UnityEngine;
using System.Collections;

public class FitExpendWidth : MonoBehaviour {

	public float DefaultWidthRatio =16f;
	public float DefaultHeightRatio = 9f;
	void Awake ()
	{
        if (SkySreenUtils.GetScreenSizeType() == SkySreenUtils.ScreenSizeType.Size_16_9)
        {
            return;
        }
		float targetScale = transform.localScale.x * Screen.height * DefaultWidthRatio / (Screen.width * DefaultHeightRatio);

		transform.localScale = new Vector3 (targetScale,transform.localScale.y, transform.localScale.z);
	}
}
