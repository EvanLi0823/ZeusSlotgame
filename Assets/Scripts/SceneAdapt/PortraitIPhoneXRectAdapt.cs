using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PortraitIPhoneXRectAdapt : MonoBehaviour
{
    [Header("x为width，y为height")]
    public Vector2 m_Size;
    // Use this for initialization
    void Start () {
        if (SkySreenUtils.CurrentOrientation== ScreenOrientation.Portrait)
        {
            if (IphoneXAdapter.IsIphoneX())
            {
                RectTransform rct = this.transform as RectTransform;
                rct.sizeDelta = new Vector2(m_Size.x, m_Size.y);
            }
        }
    }
}
