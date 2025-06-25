using UnityEngine;

// 给专门的竖版弹框用的，调整好9：16竖版的分辨率，在对应控件的根节点挂上这个脚本即可实现等比缩放
// 比如 SuperJackpotEntranceDialog_p
public class AdaptionPortraitScale : MonoBehaviour
{
    void Awake ()
    {
        var scale = transform.localScale;
        float ratio = 1;
        float screenRatio =  Screen.height* 1f  / Screen.width;

        ratio = AdaptionTools.IphoneSize / screenRatio;
        scale.x *= ratio;
        scale.y *= ratio;
        transform.localScale = scale;
    }
}
