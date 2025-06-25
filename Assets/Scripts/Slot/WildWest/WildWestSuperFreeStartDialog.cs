using Libs;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class WildWestSuperFreeStartDialog : UIDialog
{
    [Header("FreeGame开始按钮")]
    public Button StartBtn;
    public List<GameObject> superType;

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

    public void OnStart (int index)
    {
        superType[index].SetActive(true);
    }
}
