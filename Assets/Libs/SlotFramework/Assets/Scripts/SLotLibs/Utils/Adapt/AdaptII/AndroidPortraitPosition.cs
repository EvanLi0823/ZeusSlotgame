using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AndroidPortraitPosition : MonoBehaviour
{
    [Header("直接设置位置数据，不是偏移量")]
    [Header("20.5:9")]
    public Vector3 m_Offset1;
    [Header("19.5:9")]
    public Vector3 m_Offset2;
    [Header("18:9")]
    public Vector3 m_Offset3;
    [Header("16:9")]
    public Vector3 m_Offset4;
    [Header("16:10")]
    public Vector3 m_Offset5;
    [Header("15:10")]
    public Vector3 m_Offset6;
    [Header("4:3")]
    public Vector3 m_Offset7;
    // Use this for initialization
    void Start()
    {
        SetPosition();
    }
    void OnEnable()
    {
        SetPosition();
    }
    void SetPosition()
    {
        if (SkySreenUtils.CurrentOrientation == ScreenOrientation.Portrait)
        {
            float realRadio = Screen.height *1.0f / Screen.width;
            float radio1 = 2460 / 1080f;//20.5:9
            float radio2 = 2436 / 1125f;//19.5:9
            float radio3 = 2160 / 1080f;//18:9
            float radio4 = 1920 / 1080f;//16:9
            float radio5 = 2560 / 1600f;//16:10
            float radio6 = 1133 / 744f;//15:10
            float radio7 = 1920 / 1440f;//4:3
            if (realRadio>=(radio1+radio2)/2)
            {
                transform.localPosition = m_Offset1;
            }else if (realRadio>=(radio2+radio3)/2)
            {
                transform.localPosition = m_Offset2;
            }else if (realRadio>=(radio3+radio4)/2)
            {
                transform.localPosition = m_Offset3;
            }else if (realRadio>=(radio4+radio5)/2)
            {
                transform.localPosition = m_Offset4;
            }else if (realRadio>=(radio5+radio6)/2)
            {
                transform.localPosition = m_Offset5;
            }else if (realRadio>=(radio6+radio7)/2)
            {
                transform.localPosition = m_Offset6;
            }
            else
            {
                transform.localPosition = m_Offset7;
            }
            
        }
    }

}

