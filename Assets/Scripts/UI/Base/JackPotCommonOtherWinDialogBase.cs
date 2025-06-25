using UnityEngine;
using UnityEngine.UI;
using Libs;

namespace Classic
{
public class JackPotCommonOtherWinDialogBase : UIDialog 
{
    public Image MachineImage;
		[HideInInspector]
    public TMPro.TextMeshProUGUI NameText;
		[HideInInspector]
    public TMPro.TextMeshProUGUI WinText;
		[HideInInspector]
    public UnityEngine.UI.Image HeadImage;

    protected override void Awake()
    {
        base.Awake();
            if (MachineImage==null)
            {
                 this.MachineImage = Util.FindObject<UnityEngine.UI.Image>(transform,"Panel/Content/MachineImage/");
            }
       
        this.NameText = Util.FindObject<TMPro.TextMeshProUGUI>(transform,"Panel/Content/NameText/");
        this.WinText = Util.FindObject<TMPro.TextMeshProUGUI>(transform,"Panel/Content/WinText/");
        this.HeadImage = Util.FindObject<UnityEngine.UI.Image>(transform,"Panel/Content/HeadImage/");
    }
}}
