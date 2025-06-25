using UnityEngine;
using UnityEngine.UI;

public class WildWestPendant : MonoBehaviour
{
    public Text pendantcount;

    private WildWestSpinResult spinresult;

    void Awake()
    {
        spinresult = (BaseSlotMachineController.Instance.reelManager as WildWestReelManager).spinresult;
    }

    void Start()
    {
        if(spinresult != null && pendantcount != null) pendantcount.text = spinresult.freespinPendant.ToString();
    }
}
