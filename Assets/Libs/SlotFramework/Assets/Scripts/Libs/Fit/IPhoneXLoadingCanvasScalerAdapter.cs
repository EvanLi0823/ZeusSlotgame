using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IPhoneXLoadingCanvasScalerAdapter : MonoBehaviour
{
    private float scaleFactor = 1f;
    private CanvasScaler canvasScaler;
    void Start()
    {
#if UNITY_EDITOR
        SkySreenUtils.SetScreenResolutions();
#endif
        if (IphoneXAdapter.IsIphoneX())
        {
            canvasScaler = GetComponent<CanvasScaler>();
            if (canvasScaler != null)
            {
                canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
                scaleFactor = SkySreenUtils.DEVICE_WIDTH / 2339f;
                canvasScaler.scaleFactor = scaleFactor;
            }
        }
    }
}
