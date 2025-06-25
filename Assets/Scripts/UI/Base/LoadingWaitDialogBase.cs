using UnityEngine;
using UnityEngine.UI;
using Libs;

namespace Classic
{
public class LoadingWaitDialogBase : UIDialog 
{
		[HideInInspector]
    public TMPro.TextMeshProUGUI TxtTitle;
		[HideInInspector]
    public TMPro.TextMeshProUGUI TxtContent;

    protected override void Awake()
    {
        base.Awake();
        this.TxtTitle = Util.FindObject<TMPro.TextMeshProUGUI>(transform,"TxtTitle/");
        this.TxtContent = Util.FindObject<TMPro.TextMeshProUGUI>(transform,"TxtContent/");
    }
}}
