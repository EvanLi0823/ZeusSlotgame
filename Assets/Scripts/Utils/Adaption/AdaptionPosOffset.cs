using UnityEngine;
/// <summary>
/// 随着屏幕拓宽相对偏移
/// </summary>
public class AdaptionPosOffset : MonoBehaviour
{
    [Header("勾选向左偏移")]
    public bool IsLeft;

    [Header("总偏移量=屏幕拓宽*OffsetMul")]
    public float OffsetMul = 1;

    public bool NeedSafeArea;
    void Awake()
    {
        if (Screen.width*1.0f/Screen.height <= AdaptionTools.IphoneSize)
        {
            return;
        }
        AdaptionTools.MoveX(transform,NeedSafeArea,IsLeft,OffsetMul);
    }
}
