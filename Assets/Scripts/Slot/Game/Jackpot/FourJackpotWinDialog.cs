using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Libs;
using UnityEngine.UI;
using DG.Tweening;


/// <summary>
///适合4个jp的奖励弹窗 
/// </summary>
public class FourJackpotWinDialog : UIDialog
{
    public UIChangeNumber m_ChangeTxt;
    public Button m_CloseBtn;

    protected override void Awake()
    {
        base.Awake();

        UGUIEventListener.Get(this.m_CloseBtn.gameObject).onClick = this.OnButtonClickHandler;
//        m_CloseBtn.image.raycastTarget = false;
//        new DelayAction(m_ChangeTxt.ShowTime,null, delegate
//        {
//            m_CloseBtn.image.DOColor(Color.white, .5f);
//            m_CloseBtn.image.raycastTarget = true;
//        }).Play();
    }

    public override void OnButtonClickHandler(GameObject go)
    {
        base.OnButtonClickHandler(go);
        AudioEntity.Instance.PlayClickEffect();
        this.Close();
    }

    public void SetNumber(long num)
    {
        m_ChangeTxt.SetInitNumber(0);
        m_ChangeTxt.SetNumber((long) (num));
    }
}