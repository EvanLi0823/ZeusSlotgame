using UnityEngine;
public class PortraitAnchor:MonoBehaviour
{
    [SerializeField]
    private Vector2 iphoneMinAnchor = Vector2.zero;
    [SerializeField]
    private Vector2 iphoneMaxAnchor = Vector2.one;
    [SerializeField]
    private Vector2 iphoneXMinAnchor = Vector2.zero;
    [SerializeField]
    private Vector2 iphoneXMaxAnchor = Vector2.one;
    [SerializeField]
    private Vector2 ipadMinAnchor = Vector2.zero;
    [SerializeField]
    private Vector2 ipadMaxAnchor = Vector2.one;
    
    private RectTransform mTransform;
    // Start is called before the first frame update
    void Start()
    {
        if (SkySreenUtils.CurrentOrientation == ScreenOrientation.Portrait)
        {
            mTransform = transform as RectTransform;
            Vector2 minOffset = mTransform.offsetMin;
            Vector2 maxOffset = mTransform.offsetMax;
            if (IphoneXAdapter.IsIphoneX())
            {
                if (iphoneXMinAnchor != Vector2.zero) mTransform.anchorMin = iphoneXMinAnchor;
                if (iphoneXMaxAnchor != Vector2.one) mTransform.anchorMax = iphoneXMaxAnchor;
            }
            else if (SkySreenUtils.GetScreenSizeType()== SkySreenUtils.ScreenSizeType.Size_16_9)
            {
                if (iphoneMinAnchor != Vector2.zero) mTransform.anchorMin = iphoneMinAnchor;
                if (iphoneMaxAnchor != Vector2.one) mTransform.anchorMax = iphoneMaxAnchor;
            }
            else if (SkySreenUtils.GetScreenSizeType()== SkySreenUtils.ScreenSizeType.Size_4_3)
            {
                if (ipadMinAnchor != Vector2.zero) mTransform.anchorMin = ipadMinAnchor;
                if (ipadMaxAnchor != Vector2.one) mTransform.anchorMax = ipadMaxAnchor;
            }

            mTransform.offsetMin = minOffset;
            mTransform.offsetMax = maxOffset;
        }
        else
        {
            if (SkySreenUtils.GetScreenSizeType() == SkySreenUtils.ScreenSizeType.Size_4_3)
            {
                 Vector2 minOffset = mTransform.offsetMin;
                 Vector2 maxOffset = mTransform.offsetMax;
                if (ipadMinAnchor != Vector2.zero) mTransform.anchorMin = ipadMinAnchor;
                if (ipadMaxAnchor != Vector2.one) mTransform.anchorMax = ipadMaxAnchor;
                mTransform.offsetMin = minOffset;
                mTransform.offsetMax = maxOffset;
            }
        }
    }

}