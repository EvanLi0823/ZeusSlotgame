using UnityEngine;
using UnityEngine.UI;
using Libs;

namespace Classic
{
public class RenderBase : UIBase 
{
		[HideInInspector]
    public UnityEngine.UI.Image Image;
		[HideInInspector]
    public UnityEngine.UI.Button Button;

    protected override void Awake()
    {
        base.Awake();
        this.Image = Util.FindObject<UnityEngine.UI.Image>(transform,"Image/");
        this.Button = Util.FindObject<UnityEngine.UI.Button>(transform,"Button/");
        UGUIEventListener.Get(this.Button.gameObject).onClick = this.OnButtonClickHandler;
    }
}}
