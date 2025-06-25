using Libs;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
public class WildWestFreeBonusStartDialog : UIDialog
{
    [Header("FreeGame开始按钮")]
    public Button StartBtn;

    [Header("FreeGame次数")]
    public UIText FreeSpinCount;

    [Header("wild倍率")]
    public Image MulCount;

    public List<Sprite> mulSprite;


    protected override void Awake ()
    {
        base.Awake ();
        AudioEntity.Instance.StopAllAudio();
        AudioEntity.Instance.PlayFreeGameStartDialogMusic();
        UGUIEventListener.Get(this.StartBtn.gameObject).onClick = this.OnButtonClickHandler;
    }

    public override void OnButtonClickHandler (GameObject go)
    {
        base.OnButtonClickHandler (go);
        StartBtn.interactable = false;
        AudioEntity.Instance.StopFreeGameStartDialogMusic();
        AudioEntity.Instance.PlayFeatureBtnEffect();
        this.Close();
    }

    public void OnStart (long times, int count)
    {
        if(FreeSpinCount != null)FreeSpinCount.SetText(times.ToString());
        if(count == 0)
        {
            MulCount.sprite = mulSprite[0];
        }else if(count == 1)
        {
            MulCount.sprite = mulSprite[1];
        }else if(count == 2)
        {
            MulCount.sprite = mulSprite[2];
        }else if(count == 3 && count == 4)
        {
            MulCount.sprite = mulSprite[3];
        }

        MulCount.SetNativeSize();
    }
}