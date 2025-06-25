using UnityEngine;

public class AdaptActivate : MonoBehaviour
{
    public GameObject Ipad;
    public GameObject Iphone;
    public GameObject IphoneX;

    void Start () 
    {
        if(IphoneXAdapter.IsIphoneX())
        {
            this.IphoneX.SetActive(true);
        }else if(SkySreenUtils.GetScreenSizeType () == SkySreenUtils.ScreenSizeType.Size_4_3)
        {
            this.Ipad.SetActive(true);
        }else
        {
            this.Iphone.SetActive(true);
        }
	}
}
