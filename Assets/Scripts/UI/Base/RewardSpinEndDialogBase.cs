using UnityEngine;
using UnityEngine.UI;
using Libs;

namespace Classic
{
public class RewardSpinEndDialogBase : UIDialog 
{
    public TMPro.TextMeshProUGUI LeftContent;
    public UnityEngine.UI.Text SpinTimesContent;
    public TMPro.TextMeshProUGUI RightContent;
    public TMPro.TextMeshProUGUI DownContent;
    public UnityEngine.UI.Text TotalWin;
    public UnityEngine.UI.Button BtnOK;
    public TMPro.TextMeshProUGUI BtnTxt;

    protected override void Awake()
    {
        base.Awake();
        //this.LeftContent = Util.FindObject<TMPro.TextMeshProUGUI>(transform,"UpContent/LeftContent/");
        //this.SpinTimesContent = Util.FindObject<UnityEngine.UI.Text>(transform,"UpContent/SpinTimesContent/");
        //this.RightContent = Util.FindObject<TMPro.TextMeshProUGUI>(transform,"UpContent/RightContent/");
        //this.DownContent = Util.FindObject<TMPro.TextMeshProUGUI>(transform,"DownContent/DownContent/");
        //this.TotalWin = Util.FindObject<UnityEngine.UI.Text>(transform,"TotalWin/");
        //this.BtnOK = Util.FindObject<UnityEngine.UI.Button>(transform,"BtnOK/");
        UGUIEventListener.Get(this.BtnOK.gameObject).onClick = this.OnButtonClickHandler;
        //this.BtnTxt = Util.FindObject<TMPro.TextMeshProUGUI>(transform,"BtnOK/BtnTxt/");
    }
}}
