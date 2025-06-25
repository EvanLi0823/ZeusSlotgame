using UnityEngine;
using UnityEngine.UI;
using Libs;

namespace Classic
{
public class FreeSpinNumWithAwardNumDialogBase : UIDialog 
{
    public TMPro.TextMeshProUGUI TxtFreeSpins;
    public TMPro.TextMeshProUGUI TxtAwardNum;

    protected override void Awake()
    {
        base.Awake();
        this.TxtFreeSpins = Util.FindObject<TMPro.TextMeshProUGUI>(transform,"TxtFreeSpins/");
        this.TxtAwardNum = Util.FindObject<TMPro.TextMeshProUGUI>(transform,"TxtAwardNum/");
    }
}}
