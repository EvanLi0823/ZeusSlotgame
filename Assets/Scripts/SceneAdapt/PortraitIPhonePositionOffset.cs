using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortraitIPhonePositionOffset : MonoBehaviour
{
    public Vector2 m_Offset;
    // Use this for initialization
    void Start()
    {
        if (SkySreenUtils.CurrentOrientation == ScreenOrientation.Portrait)
        {
            if (IphoneXAdapter.IsIphoneX()) return;
            if (SkySreenUtils.GetScreenSizeType() == SkySreenUtils.ScreenSizeType.Size_16_9)
            {
                RectTransform rct = this.transform as RectTransform;
                Vector3 v = rct.localPosition;
                v.x += m_Offset.x;
                v.y += m_Offset.y;
                rct.localPosition = v;
            }
        }
    }

}
