using UnityEngine;
using UnityEngine.UI;
using Libs;
using TMPro;
namespace Classic
{
public class RewardVideoRewardDialongBase : UIDialog 
{
//	[HideInInspector]
//    public TMPro.TextMeshProUGUI AwardTitle;
    [HideInInspector]
    public TextMeshProUGUI TextNum;
    [HideInInspector]
    public TMPro.TextMeshProUGUI QuestOKButtonText;
    [HideInInspector]
    public GameObject adImage;
    protected override void Awake()
    {
        base.Awake();
      //  this.AwardTitle = Util.FindObject<TMPro.TextMeshProUGUI>(transform,"WatchAdCollectCoinsDialog/Panel/Animation/title/");
        this.TextNum = Util.FindObject<TMPro.TextMeshProUGUI>(transform,"WatchAdCollectCoinsDialog/Panel/Animation/GameObject (1)/TxtCoinsNum (2)/");
        this.BtnClose = Util.FindObject<UnityEngine.UI.Button>(transform,"WatchAdCollectCoinsDialog/Panel/Animation/X (1)/");
        UGUIEventListener.Get(this.BtnClose.gameObject).onClick = this.OnButtonClickHandler;
        this.QuestOKButtonText = Util.FindObject<TMPro.TextMeshProUGUI>(transform,"WatchAdCollectCoinsDialog/Panel/Animation/anniu (1)/TxtCoinsNum (3)/");
        this.adImage = Util.FindObject<GameObject>(transform,"WatchAdCollectCoinsDialog/Panel/Animation/anniu (1)/adImage/");
        
    }
}}
