using UnityEngine;
using UnityEngine.UI;
using Libs;

namespace Classic
{
public class AlertDialogBase : UIDialog 
{
    public TMPro.TextMeshProUGUI TxtTitle;
    public TMPro.TextMeshProUGUI TxtContent;
    public UnityEngine.UI.Button BtnOk;
    public TMPro.TextMeshProUGUI TxtBtn;
    protected override void Awake()
    {
        base.Awake();
        this.TxtTitle = Util.FindObject<TMPro.TextMeshProUGUI>(transform,"Anchor/TxtTitle/");
        this.TxtContent = Util.FindObject<TMPro.TextMeshProUGUI>(transform,"Anchor/TxtContent/");
        this.BtnOk = Util.FindObject<UnityEngine.UI.Button>(transform,"Anchor/BtnOk/");
        this.TxtBtn= Util.FindObject<TMPro.TextMeshProUGUI>(transform,"Anchor/BtnOk/TextMeshPro Text/");
        UGUIEventListener.Get(this.BtnOk.gameObject).onClick = this.OnButtonClickHandler;
    }
}}
