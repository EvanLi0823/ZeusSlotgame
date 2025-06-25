using Libs;
using UnityEngine;
using UnityEngine.UI;

public class ReSpinStartDialogBase : UIDialog 
{
    [HideInInspector]
    public Button StartFeatureButton;

    protected override void Awake()
    {
        base.Awake();
        this.StartFeatureButton = Util.FindObject<Button>(transform, "animation/StartFeature/");
        if (StartFeatureButton != null)
        {
            UGUIEventListener.Get(this.StartFeatureButton.gameObject).onClick = this.OnButtonClickHandler;
        }
    }
}
