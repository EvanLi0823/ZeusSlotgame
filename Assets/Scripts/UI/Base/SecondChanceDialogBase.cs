using UnityEngine;
using UnityEngine.UI;
using Libs;

namespace Classic
{
public class SecondChanceDialogBase : UIDialog 
{
		[HideInInspector]
    public TMPro.TextMeshProUGUI DescText;
		[HideInInspector]
    public TMPro.TextMeshProUGUI ForMoney;
		[HideInInspector]
    public TMPro.TextMeshProUGUI PlusText;
		[HideInInspector]
    public UnityEngine.UI.Text CoinsText;
		[HideInInspector]
    public UnityEngine.UI.Button GoToStoreButton;

    protected override void Awake()
    {
        base.Awake();
        this.DescText = Util.FindObject<TMPro.TextMeshProUGUI>(transform,"Texts/DescText/");
        this.ForMoney = Util.FindObject<TMPro.TextMeshProUGUI>(transform,"Texts/ForMoney/");
        this.PlusText = Util.FindObject<TMPro.TextMeshProUGUI>(transform,"Texts/PlusText/");
        this.CoinsText = Util.FindObject<UnityEngine.UI.Text>(transform,"Texts/CoinsText/");
        this.GoToStoreButton = Util.FindObject<UnityEngine.UI.Button>(transform,"Buttons/GoToStoreButton/");
        UGUIEventListener.Get(this.GoToStoreButton.gameObject).onClick = this.OnButtonClickHandler;
    }
}}
