using UnityEngine;
using UnityEngine.UI;
using Libs;

namespace Classic
{
public class WaitingDialogBase : UIDialog 
{
    public TMPro.TextMeshProUGUI TxtTitle;
    public TMPro.TextMeshProUGUI TxtContent;

    protected override void Awake()
    {
        base.Awake();
        this.TxtTitle = Util.FindObject<TMPro.TextMeshProUGUI>(transform,"Anchor/TxtTitle/");
        this.TxtContent = Util.FindObject<TMPro.TextMeshProUGUI>(transform,"Anchor/TxtContent/");
    }
}}
