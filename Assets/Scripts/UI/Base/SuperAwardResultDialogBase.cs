using UnityEngine;
using UnityEngine.UI;
using Libs;

namespace Classic
{
public class SuperAwardResultDialogBase : UIDialog 
{
    public TMPro.TextMeshProUGUI TxtAwardNum;

    protected override void Awake()
    {
        base.Awake();
        this.TxtAwardNum = Util.FindObject<TMPro.TextMeshProUGUI>(transform,"TxtAwardNum/");
    }
}}
