using Libs;
using UnityEngine;
using Button = UnityEngine.UI.Button;

public class NewUserGuideDialog : UIDialog
{
    private Button panelButton1;
    protected override void Awake()
    {
        base.Awake();
        panelButton1 = Util.FindObject<Button>(this.transform, "Anchor/Panel1/CloseBtn");
        if (panelButton1!=null)
        {
            UGUIEventListener.Get(panelButton1.gameObject).onClick= this.OnButtonClickHandler;
        }
    }

    public override void OnButtonClickHandler(GameObject go)
    {
        base.OnButtonClickHandler(go);
        if (go = panelButton1.gameObject)
        {
            this.Close();
            //显示引导按钮
            Messenger.Broadcast<bool>(GameConstants.NEW_USER_GUIDE, true);
        }
    }
}
