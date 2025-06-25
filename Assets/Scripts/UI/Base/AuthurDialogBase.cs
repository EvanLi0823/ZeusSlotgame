using UnityEngine;
using UnityEngine.UI;
using Libs;

namespace Classic
{
public class AuthurDialogBase : UIDialog 
{
    public TMPro.TextMeshProUGUI number;
    public UnityEngine.UI.Image IconImage;

    protected override void Awake()
    {
        base.Awake();
        this.number = Util.FindObject<TMPro.TextMeshProUGUI>(transform,"number/");
        this.IconImage = Util.FindObject<UnityEngine.UI.Image>(transform,"IconImage/");
    }
}}
