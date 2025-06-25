using Libs;
using UnityEngine;
public class WildWestBetLevelSmallDialog : UIDialog
{
    public void OnStart(int index)
    {
        if(index < 0 || index >= this.transform.childCount) this.Close();
        this.transform.GetChild(index).gameObject.SetActive(true);
    }
}
