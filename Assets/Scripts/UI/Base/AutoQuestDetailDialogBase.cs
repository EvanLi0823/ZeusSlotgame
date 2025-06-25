using UnityEngine;
using UnityEngine.UI;
using Libs;

namespace Classic
{
public class AutoQuestDetailDialogBase : UIDialog 
{
		[HideInInspector]
    public UnityEngine.UI.Image ActiveQuestDetailContentPanel;
		[HideInInspector]
    public UnityEngine.UI.Image CurrentAutoQuestItem;
		[HideInInspector]
    public UnityEngine.UI.Image ThreeMachinesPanel;
		[HideInInspector]
    public UnityEngine.UI.Image TwoMachinesPanel;
		[HideInInspector]
    public UnityEngine.UI.Text AwardAmount;
		[HideInInspector]
    public TMPro.TextMeshProUGUI StarTypeAward;
		[HideInInspector]
    public UnityEngine.UI.Button BackButton;
		[HideInInspector]
    public UnityEngine.UI.Button StartButton;
		[HideInInspector]
    public TMPro.TextMeshProUGUI WellDone;
		[HideInInspector]
    public UnityEngine.UI.Button CloseButton;
		[HideInInspector]
    public UnityEngine.UI.Button PreArrow;
		[HideInInspector]
    public UnityEngine.UI.Button NextArrow;

    protected override void Awake()
    {
        base.Awake();
        this.ActiveQuestDetailContentPanel = Util.FindObject<UnityEngine.UI.Image>(transform,"ActiveQuestDetailContentPanel/");
        this.CurrentAutoQuestItem = Util.FindObject<UnityEngine.UI.Image>(transform,"ActiveQuestDetailContentPanel/ActiveQuestPanelBackground/CurrentAutoQuestItem/");
        this.ThreeMachinesPanel = Util.FindObject<UnityEngine.UI.Image>(transform,"ActiveQuestDetailContentPanel/ThreeMachinesPanel/");
        this.TwoMachinesPanel = Util.FindObject<UnityEngine.UI.Image>(transform,"ActiveQuestDetailContentPanel/TwoMachinesPanel/");
        this.AwardAmount = Util.FindObject<UnityEngine.UI.Text>(transform,"ActiveQuestDetailContentPanel/AwardAmount/");
        this.StarTypeAward = Util.FindObject<TMPro.TextMeshProUGUI>(transform,"ActiveQuestDetailContentPanel/StarTypeAward/");
        this.BackButton = Util.FindObject<UnityEngine.UI.Button>(transform,"ActiveQuestDetailContentPanel/BackButton/");
        UGUIEventListener.Get(this.BackButton.gameObject).onClick = this.OnButtonClickHandler;
        this.StartButton = Util.FindObject<UnityEngine.UI.Button>(transform,"ActiveQuestDetailContentPanel/StartButton/");
        UGUIEventListener.Get(this.StartButton.gameObject).onClick = this.OnButtonClickHandler;
        this.WellDone = Util.FindObject<TMPro.TextMeshProUGUI>(transform,"ActiveQuestDetailContentPanel/WellDone/");
        this.CloseButton = Util.FindObject<UnityEngine.UI.Button>(transform,"ActiveQuestDetailContentPanel/CloseButton/");
        UGUIEventListener.Get(this.CloseButton.gameObject).onClick = this.OnButtonClickHandler;
        this.PreArrow = Util.FindObject<UnityEngine.UI.Button>(transform,"PreArrow/");
        UGUIEventListener.Get(this.PreArrow.gameObject).onClick = this.OnButtonClickHandler;
        this.NextArrow = Util.FindObject<UnityEngine.UI.Button>(transform,"NextArrow/");
        UGUIEventListener.Get(this.NextArrow.gameObject).onClick = this.OnButtonClickHandler;
    }
}}
