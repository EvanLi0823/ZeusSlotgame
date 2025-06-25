using UnityEngine;

// 给专门的竖版弹框用的，调整好9：16竖版的分辨率，在对应锚点控件上挂上这个脚本即可实现锚定左右的情况
// 比如 SuperJackpotEntranceDialog_p
public class AdaptionPortraitXOffset : MonoBehaviour
{
    private Vector3 LocalV;

    [Header("True:靠左   False:靠右")]
    public bool isLeft;
        
    void Awake()
    {
        LocalV = (transform as RectTransform).localPosition;
    }
    void Start()
    {
        float realRadio = Screen.height * 1.0f / Screen.width;

        if (realRadio < AdaptionTools.IphoneSize)
        {
            float v = (1920 / realRadio - 1080) / 2.0f / AdaptionTools.IphoneSize;
            float w = isLeft ? LocalV.x - v : LocalV.x + v;
            (transform as RectTransform).localPosition = new Vector3(w, LocalV.y, LocalV.z);
        }
    }
}