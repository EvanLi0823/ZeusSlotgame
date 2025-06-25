using UnityEngine;
using UnityEngine.UI;
using Libs;

namespace Classic
{
public class TermsDialogBase : UIDialog 
{
		[HideInInspector]
    public UnityEngine.UI.Image reward_bg_kuang;
		[HideInInspector]
    public UnityEngine.UI.Button CloseButton;
		[HideInInspector]
    public UnityEngine.GameObject PositionGameObject;

    protected override void Awake()
    {
        base.Awake();
        this.reward_bg_kuang = Util.FindObject<UnityEngine.UI.Image>(transform,"Anchor/reward_bg_kuang/");
        this.CloseButton = Util.FindObject<UnityEngine.UI.Button>(transform,"Anchor/CloseButton/");
        UGUIEventListener.Get(this.CloseButton.gameObject).onClick = this.OnButtonClickHandler;
        this.PositionGameObject = Util.FindObject<UnityEngine.GameObject>(transform,"Anchor/PositionGameObject/");
    }
}}
