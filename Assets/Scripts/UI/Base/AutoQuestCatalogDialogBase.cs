using UnityEngine;
using UnityEngine.UI;
using Libs;

namespace Classic
{
public class AutoQuestCatalogDialogBase : UIDialog 
{
		[HideInInspector]
    public UnityEngine.UI.Image ContentPanel;
		[HideInInspector]
    public UnityEngine.UI.Button ActiveQuestCatalogListItem;
		[HideInInspector]
    public UnityEngine.GameObject QuestListScroll;
		[HideInInspector]
    public TMPro.TextMeshProUGUI UnlockHintText;
		[HideInInspector]
    public UnityEngine.UI.Button CloseButton;

    protected override void Awake()
    {
        base.Awake();
        this.ContentPanel = Util.FindObject<UnityEngine.UI.Image>(transform,"ContentPanel/");
        this.ActiveQuestCatalogListItem = Util.FindObject<UnityEngine.UI.Button>(transform,"ContentPanel/ActiveQuestPanelBackground/ActiveQuestCatalogListItem/");
        UGUIEventListener.Get(this.ActiveQuestCatalogListItem.gameObject).onClick = this.OnButtonClickHandler;
        this.QuestListScroll = Util.FindObject<UnityEngine.GameObject>(transform,"ContentPanel/AutoQuestListBg/QuestListScroll/");
        this.UnlockHintText = Util.FindObject<TMPro.TextMeshProUGUI>(transform,"ContentPanel/UnlockHintText/");
        this.CloseButton = Util.FindObject<UnityEngine.UI.Button>(transform,"ContentPanel/CloseButton/");
        UGUIEventListener.Get(this.CloseButton.gameObject).onClick = this.OnButtonClickHandler;
    }
}}
