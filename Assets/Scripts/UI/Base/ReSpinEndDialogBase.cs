using Libs;
using UnityEngine;
using UnityEngine.UI;


public class ReSpinEndDialogBase : UIDialog 
{
    [HideInInspector]
    public Text TextNum;
    
    [HideInInspector]
    public Button BackToGameButton;

    public string WinTextPath = "animation/TextNum/";
    public string BackToGamePath = "animation/BackToGame/";
    protected override void Awake()
    {
        base.Awake();
        this.TextNum = Util.FindObject<Text>(transform, WinTextPath);
        this.BackToGameButton = Util.FindObject<Button>(transform, BackToGamePath);
        if (BackToGameButton != null)
        {
            UGUIEventListener.Get(this.BackToGameButton.gameObject).onClick = this.OnButtonClickHandler;
        }

        if(TextNum ==null)
        {
            this.TextNum = Util.FindObject<Text>(transform, "animation/Panel/TextNum/");
        }
    }
}
