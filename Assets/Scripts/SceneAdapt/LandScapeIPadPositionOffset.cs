using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandScapeIPadPositionOffset : MonoBehaviour
{
    public Vector2 m_Offset;
    public bool enableScaleX = false;
    public bool enableScaleY = false;
    public float scaleXFactor = 1f;
    public float scaleYFactor = 1f;
    private void Start()
    {
        AdjustLoadingLayout();
    }
    void AdjustLoadingLayout()
    {
        if (SkySreenUtils.CurrentOrientation == ScreenOrientation.LandscapeLeft)
        {
            if (SkySreenUtils.GetScreenSizeType() == SkySreenUtils.ScreenSizeType.Size_4_3)
            {
                RectTransform rct = this.transform as RectTransform;
                if (!enableScaleX)
                {
                    scaleXFactor = rct.localScale.x;
                }
                if (!enableScaleY)
                {
                    scaleYFactor = rct.localScale.y;
                }
                rct.localScale = new Vector3(scaleXFactor, scaleYFactor, rct.localScale.z);
                Vector3 v = rct.localPosition;
                v.x += m_Offset.x;
                v.y += m_Offset.y;
                rct.localPosition = v;
            }
        }
    }
}
