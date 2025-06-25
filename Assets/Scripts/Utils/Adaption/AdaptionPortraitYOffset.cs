using UnityEngine;

// 给专门的竖版弹框用的，调整好9：16竖版的分辨率，在对应锚点控件上挂上这个脚本即可实现锚定上下的情况
// 比如 SuperJackpotEntranceDialog_p
public class AdaptionPortraitYOffset : MonoBehaviour
{
    private Vector3 LocalV;
    [Header("True:靠下   False:靠上")]
    public bool isBottom;

    void Awake()
    {
        LocalV = (transform as RectTransform).localPosition;
    }
    void Start()
    {

        if (SkySreenUtils.CurrentOrientation == ScreenOrientation.Portrait)
        {
            float realRadio = Screen.height * 1.0f / Screen.width;
            if(realRadio > AdaptionTools.IphoneSize)
            {
                float v = (AdaptionTools.Design_Heigth * realRadio - AdaptionTools.Design_Width) / 2.0f / AdaptionTools.IphoneSize;
                // 如果是IOS。由于刘海问题，需要 额外减去刘海的像素
#if UNITY_IOS
                v = Mathf.Max(0f, v - 50f);
#endif
                float h = isBottom ? LocalV.y - v : LocalV.y + v;
                (transform as RectTransform).localPosition = new Vector3(LocalV.x,h,LocalV.z);
            }
        }
    }
}
