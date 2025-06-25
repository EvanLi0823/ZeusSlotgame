using System;
using Libs;
using UnityEngine;
public class ReSpinStartDialog : ReSpinStartDialogBase
{
    
    protected override void Awake()
    {
        base.Awake();

        if (this.StartFeatureButton == null)
        {
            this.bResponseBackButton = false;
            this.AutoQuit = true;
            this.DisplayTime = 4;
        }

        AudioEntity.Instance.PlayReSpinStartDialogMusic();
    }

    public override void OnButtonClickHandler(GameObject go)
    {
        base.OnButtonClickHandler(go);
        AudioEntity.Instance.StopReSpinStartDialogMusic();
        AudioEntity.Instance.PlayFeatureBtnEffect();
        this.Close();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        this.Close();
    }
}