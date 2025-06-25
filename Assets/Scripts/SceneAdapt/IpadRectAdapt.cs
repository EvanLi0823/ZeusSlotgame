using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IpadRectAdapt : MonoBehaviour
{
    [Header("x为width，y为height")]
    public Vector2 m_Size;
    // Use this for initialization
    void Start () 
    {
        if (SkySreenUtils.CurrentOrientation == ScreenOrientation.Portrait)
        {
            if (SkySreenUtils.GetScreenSizeType() == SkySreenUtils.ScreenSizeType.Size_4_3)
            {
                RectTransform rct = this.transform as RectTransform;
                rct.sizeDelta = new Vector2(m_Size.x, m_Size.y);
            }
        }
    }
}
