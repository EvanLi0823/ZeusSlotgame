using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPositionOffsetAdapt : MonoBehaviour
{
    [Header("x为向右边偏移，y为向上偏移")]
    public Vector2 IPhoneOffset;
    [Header("x为向右边偏移，y为向上偏移")]
    public Vector2 IPhoneXOffset;
    [Header("x为向右边偏移，y为向上偏移")]
    public Vector2 IPadOffset;
    // Use this for initialization
    void Start()
    {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        float realRadio = screenWidth / screenHeight;
        if (IphoneXAdapter.IsIphoneX())
        {
            if (IPhoneXOffset == Vector2.zero) return;
            RectTransform rct = this.transform as RectTransform;
            Vector3 v = rct.localPosition;
            v.x += IPhoneXOffset.x;
            v.y += IPhoneXOffset.y;
            rct.localPosition = v;
        }
       
        //else if (SkySreenUtils.GetScreenSizeType()== SkySreenUtils.ScreenSizeType.Size_4_3)
        else if(SkySreenUtils.IsRealPad())
        {
            if (IPadOffset == Vector2.zero) return;
            RectTransform rct = this.transform as RectTransform;
            Vector3 v = rct.localPosition;
            v.x += IPadOffset.x;
            v.y += IPadOffset.y;
            rct.localPosition = v;
        }
        else if (SkySreenUtils.GetScreenSizeType()== SkySreenUtils.ScreenSizeType.Size_16_9)
        {
            if (IPhoneOffset == Vector2.zero) return;
            RectTransform rct = this.transform as RectTransform;
            Vector3 v = rct.localPosition;
            v.x += IPhoneOffset.x;
            v.y += IPhoneOffset.y;
            rct.localPosition = v;
        }
    }
}
