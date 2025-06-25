using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Libs;

public class FreeGameRetriggerDialog : UIDialog 
{
    [Header("FreeGame继续按钮")]
    public Button ContinueGame;

    [Header("再次触发FreeGame次数")]
    public UIText RetriggerFreeCount;

    protected override void Awake()
    {
        base.Awake();
        AudioEntity.Instance.PauseBackGroundAudio("Special");
        AudioEntity.Instance.StopEffectAudio("BonusTrigger");
        AudioEntity.Instance.PlayFreeGameRetriggerDialogMusic();
        if(ContinueGame != null){UGUIEventListener.Get(this.ContinueGame.gameObject).onClick = this.OnButtonClickHandler; return;}
        this.bResponseBackButton = false;
        this.AutoQuit = true;
        this.DisplayTime = 4; 
    }

    public override void OnButtonClickHandler(GameObject go)
    {
        base.OnButtonClickHandler(go);
        ContinueGame.interactable = false;
        AudioEntity.Instance.StopFreeGameRetriggerDialogMusic();
        AudioEntity.Instance.PlayFeatureBtnEffect();
        this.Close();
    }

    public void OnStart(int count)
    {
		if(this.RetriggerFreeCount != null) this.RetriggerFreeCount.SetText(count.ToString());
    }
}

