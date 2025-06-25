using UnityEngine;
using System.Collections;

public class FixedCenterX : MonoBehaviour {

    public float devHeight = 9f;
    public float devWidth = 16f;

    void Start()
    {
        ResizeScale();
    }

    void ResizeScale()
    {
        if (SkySreenUtils.GetScreenSizeType() != SkySreenUtils.ScreenSizeType.Size_16_9)
        {
            float targetScale = 1f * Screen.height * devWidth / (Screen.width * devHeight);
            transform.localScale = new Vector3(targetScale * transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
       
    }
}
