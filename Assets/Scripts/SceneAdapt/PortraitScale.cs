using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Libs;
public class PortraitScale : MonoBehaviour
{
    public Vector3 iphoneScale = Vector3.zero;
    public Vector3 iphoneXScale = Vector3.zero;
    public Vector3 ipadScale = Vector3.zero;
    [Header("横版的ipad缩放")]
    public Vector2 ipadScale_Landscape = Vector2.zero;
    // Start is called before the first frame update
    void Start()
    {
        SetScale();
    }

    void OnEnable()
    {
        SetScale();
    }

    void SetScale()
    {
        if (SkySreenUtils.CurrentOrientation == ScreenOrientation.Portrait) {
            if (IphoneXAdapter.IsIphoneX())
            {
                if (iphoneXScale != Vector3.zero) transform.localScale = new Vector3(iphoneXScale.x, iphoneXScale.y, iphoneXScale.x);
            }
            if (CommonMobileDeviceAdapter.IsWideScreen())
            {
                if(iphoneXScale != Vector3.zero)
                {
//                    float v = (float) Screen.width / (float) Screen.height;
//                    if (v > 1)
//                    {
//                        v = 1 / v;
//                    }
//                    
                     float mult = ((float)Screen.width / (float)Screen.height) / (1125f / 2436f);
//                     
//                     float v = origin / (2436f / 1125f);

                    transform.localScale = new Vector3(iphoneXScale.x * mult, iphoneXScale.y * mult, 1);
                }
            }
            else if (SkySreenUtils.GetScreenSizeType() == SkySreenUtils.ScreenSizeType.Size_16_9)
            {
                if (iphoneScale != Vector3.zero)
                {

                    if (SkySreenUtils.IsRealIphone())
                    {
                        transform.localScale = new Vector3(iphoneScale.x, iphoneScale.y, iphoneScale.x);
                    }
                    else
                    {
                        float mult = ((float) Screen.width / (float) Screen.height) / (9f / 16f);
                        if (mult > 0)
                        {
                            transform.localScale = new Vector3(mult * iphoneScale.x, mult * iphoneScale.y,
                                mult * iphoneScale.x);
                        }
                        else
                        {
                            transform.localScale = new Vector3(iphoneScale.x, iphoneScale.y, iphoneScale.x);
                        }
                    }
                }
                   
            }else if (SkySreenUtils.GetScreenSizeType() == SkySreenUtils.ScreenSizeType.Size_4_3)
            {
                if (ipadScale != Vector3.zero)
                {
                    if (SkySreenUtils.IsRealPad())
                    {
                        transform.localScale = new Vector3(ipadScale.x, ipadScale.y, ipadScale.x);
                    }
                    else
                    {
                        float mult = ((float)Screen.width / (float)Screen.height) / 0.75f;
                        if (mult>0)
                        {
                            transform.localScale = new Vector3(mult*ipadScale.x, mult*ipadScale.y, mult*ipadScale.x);  
                        }
                        else
                        {
                            transform.localScale = new Vector3(ipadScale.x, ipadScale.y, ipadScale.x);
                        }
                    }
                }
                    
            }
        }
        else
        {
            if (SkySreenUtils.GetScreenSizeType() == SkySreenUtils.ScreenSizeType.Size_4_3)
            {
                if (ipadScale_Landscape != Vector2.zero)
                {
                    if (SkySreenUtils.IsRealPad())
                    {
                        transform.localScale = new Vector3(ipadScale_Landscape.x, ipadScale_Landscape.y, 1f);
                    }
                    else
                    {
                        float mult = ((float)Screen.height / (float)Screen.width) / 0.75f;
                        if (mult>0)
                        {
                            transform.localScale = new Vector3(mult*ipadScale_Landscape.x, mult*ipadScale_Landscape.y, 1);  
                        }
                        else
                        {
                            transform.localScale = new Vector3(ipadScale_Landscape.x, ipadScale_Landscape.y, 1f);
                        }
                    }
                  
                }
            }
        }
    }
}
