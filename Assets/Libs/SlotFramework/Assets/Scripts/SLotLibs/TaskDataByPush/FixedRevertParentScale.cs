using UnityEngine;
using System.Collections;

public class FixedRevertParentScale : MonoBehaviour {
	public float devHeight = 9f;
	public float devWidth = 16f;
	private Vector3 originScaleVector;
	void Start()
	{
		originScaleVector = this.transform.localScale;
		ResizeScale();
	}

	void ResizeScale()
	{
        if (SkySreenUtils.GetScreenSizeType() != SkySreenUtils.ScreenSizeType.Size_16_9)
        {
            float targetScale = 1f * Screen.height * devWidth / (Screen.width * devHeight);
            transform.localScale = new Vector3(targetScale * originScaleVector.x, targetScale * originScaleVector.y, transform.localScale.z);
        }
	
	}
}
