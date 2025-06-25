using UnityEngine;
using UnityEngine.UI;
using Libs;

namespace Classic
{
public class AutoSpinAdditionalDialogBase : UIDialog 
{
		[HideInInspector]
    public TMPro.TextMeshProUGUI TextNum;
		[HideInInspector]
    public TMPro.TextMeshProUGUI ImageBellow;

    protected override void Awake()
    {
        base.Awake();
        this.TextNum = Util.FindObject<TMPro.TextMeshProUGUI>(transform,"Panel/GameObject1/TextNum/");
        this.ImageBellow = Util.FindObject<TMPro.TextMeshProUGUI>(transform,"Panel/GameObject2/ImageBellow/");
    }
}}
