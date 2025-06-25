using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandScapeScale : MonoBehaviour
{

    public float MinRatio = 1.33333f;
    public float MaxRatio = 1.777778f;
    
    [Header("Android横版的缩放,设备宽高比在区间内缩放")] public Vector2 LandscapeScale = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        if (SkySreenUtils.CurrentOrientation == ScreenOrientation.Portrait) return; 
        
        var ratio = (Screen.width / (float)Screen.height);
        if (Application.platform == RuntimePlatform.Android)
        {
            if (MinRatio <= ratio && ratio <= MaxRatio && LandscapeScale != Vector2.zero)
            {
                transform.localScale = new Vector3(LandscapeScale.x, LandscapeScale.y, 1f);
            }
        }
        #if UNITY_ANDROID
        if (MinRatio <= ratio && ratio <= MaxRatio && LandscapeScale != Vector2.zero)
        {
            transform.localScale = new Vector3(LandscapeScale.x, LandscapeScale.y, 1f);
        }
        #endif 
    }
}