using UnityEngine;
using UnityEngine.UI;
using Libs;

namespace Classic
{
public class RateMachineDialogBase : UIDialog 
{
		[HideInInspector]
    public UnityEngine.UI.Image BgImage_left;
		[HideInInspector]
    public UnityEngine.UI.Image BgImage_right;
		[HideInInspector]
    public TMPro.TextMeshProUGUI text_down;
		[HideInInspector]
    public TMPro.TextMeshProUGUI text_up;
		[HideInInspector]
    public UnityEngine.UI.Button BtnOk;
		[HideInInspector]
    public UnityEngine.UI.Button Button1;
		[HideInInspector]
    public UnityEngine.UI.Button Button2;
		[HideInInspector]
    public UnityEngine.UI.Button Button3;
		[HideInInspector]
    public UnityEngine.UI.Button Button4;
		[HideInInspector]
    public UnityEngine.UI.Button Button5;
		[HideInInspector]
    public UnityEngine.UI.Slider MoveSlider;
		[HideInInspector]
    public TMPro.TextMeshProUGUI LabelAward;
		[HideInInspector]
    public UnityEngine.UI.Button BtnClose1;
		[HideInInspector]
    public TMPro.TextMeshProUGUI CloseBtn;
    public Image SceneName;

    protected override void Awake()
    {
		base.Awake();
        this.BgImage_left = Util.FindObject<UnityEngine.UI.Image>(transform,"BgImage_left/");
        this.BgImage_right = Util.FindObject<UnityEngine.UI.Image>(transform,"BgImage_right/");
        this.text_down = Util.FindObject<TMPro.TextMeshProUGUI>(transform,"text_down/");
        this.text_up = Util.FindObject<TMPro.TextMeshProUGUI>(transform,"text_up/");
        this.BtnOk = Util.FindObject<UnityEngine.UI.Button>(transform,"BtnOk/");
        UGUIEventListener.Get(this.BtnOk.gameObject).onClick = this.OnButtonClickHandler;
        this.Button1 = Util.FindObject<UnityEngine.UI.Button>(transform,"Button1/");
        UGUIEventListener.Get(this.Button1.gameObject).onClick = this.OnButtonClickHandler;
        this.Button2 = Util.FindObject<UnityEngine.UI.Button>(transform,"Button2/");
        UGUIEventListener.Get(this.Button2.gameObject).onClick = this.OnButtonClickHandler;
        this.Button3 = Util.FindObject<UnityEngine.UI.Button>(transform,"Button3/");
        UGUIEventListener.Get(this.Button3.gameObject).onClick = this.OnButtonClickHandler;
        this.Button4 = Util.FindObject<UnityEngine.UI.Button>(transform,"Button4/");
        UGUIEventListener.Get(this.Button4.gameObject).onClick = this.OnButtonClickHandler;
        this.Button5 = Util.FindObject<UnityEngine.UI.Button>(transform,"Button5/");
        UGUIEventListener.Get(this.Button5.gameObject).onClick = this.OnButtonClickHandler;
        this.MoveSlider = Util.FindObject<UnityEngine.UI.Slider>(transform,"MoveSlider/");
		this.LabelAward = Util.FindObject<TMPro.TextMeshProUGUI>(transform,"BtnOk/LabelAward/");
		this.BtnClose1 = Util.FindObject<UnityEngine.UI.Button>(transform,"BtnClose1/");
		UGUIEventListener.Get(this.BtnClose1.gameObject).onClick = this.OnButtonClickHandler;
		this.CloseBtn = Util.FindObject<TMPro.TextMeshProUGUI>(transform,"BtnClose1/CloseBtn/");
            if (SceneName==null)
            {
                  this.SceneName = Util.FindObject<UnityEngine.UI.Image>(transform,"SceneName/");
            }
    }
}}
