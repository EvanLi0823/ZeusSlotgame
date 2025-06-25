using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortraitHeightSizeScale : MonoBehaviour
{
    public Vector3 iphoneSize = Vector3.zero;
    public Vector3 iphoneXSize = Vector3.zero;
    public Vector3 ipadSize = Vector3.zero;
    [Header("横版的ipad缩放")]
    public Vector3 ipadSize_Landscape = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        if (SkySreenUtils.CurrentOrientation == ScreenOrientation.Portrait) {
            if (IphoneXAdapter.IsIphoneX())
            {
                SetSize(new Vector3(iphoneXSize.x, iphoneXSize.y, iphoneXSize.x));
            }
            else if (SkySreenUtils.GetScreenSizeType()== SkySreenUtils.ScreenSizeType.Size_16_9)
            {
                SetSize(new Vector3(iphoneSize.x, iphoneSize.y, iphoneSize.x));
            }
            else if (SkySreenUtils.GetScreenSizeType()== SkySreenUtils.ScreenSizeType.Size_4_3)
            {
                SetSize(new Vector3(ipadSize.x, ipadSize.y, ipadSize.x));
            }
        }
        else
        {
            if (SkySreenUtils.GetScreenSizeType() == SkySreenUtils.ScreenSizeType.Size_4_3)
            {
                SetSize(new Vector3(ipadSize_Landscape.x, ipadSize_Landscape.y, ipadSize_Landscape.z));
            }
        }
    }

    private void SetSize(Vector3 size)
    {
        if (size == Vector3.zero) return;
        
        var rectTransform = this.transform.GetComponent<RectTransform>();
        Vector2 newSize = new Vector2(size.x, size.y);
        rectTransform.sizeDelta = newSize;
    }


}