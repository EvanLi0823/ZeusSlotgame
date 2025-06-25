using UnityEngine;
using UnityEngine.UI;
using Libs;

namespace Classic
{
public class HighRollerDialogBase : UIDialog 
{
		[HideInInspector]
    public UnityEngine.UI.Button CloseBtn;

    protected override void Awake()
    {
        base.Awake();
        this.CloseBtn = Util.FindObject<UnityEngine.UI.Button>(transform,"CloseBtn/");
        UGUIEventListener.Get(this.CloseBtn.gameObject).onClick = this.OnButtonClickHandler;
    }
}}
