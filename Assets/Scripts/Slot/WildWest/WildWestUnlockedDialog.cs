using Libs;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class WildWestUnlockedDialog : UIDialog
{
    [SerializeField]
    public Button OkButton;

    [SerializeField]
    public List<GameObject> info;

    protected override void Awake ()
    {
        base.Awake ();
        AudioEntity.Instance.StopAllAudio();
        AudioEntity.Instance.PlayFreeGameStartDialogMusic();
        UGUIEventListener.Get(this.OkButton.gameObject).onClick = this.OnButtonClickHandler;
    }

    public override void OnButtonClickHandler (GameObject go)
    {
        base.OnButtonClickHandler (go);
        OkButton.interactable = false;
        this.Close();
    }

    public void OnStart(int index)
    {
        if(index == 0) return;
        info[index-1].SetActive(true);
    }
}
