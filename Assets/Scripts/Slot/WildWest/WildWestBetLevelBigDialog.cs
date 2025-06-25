using Libs;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class WildWestBetLevelBigDialog : UIDialog
{
    private WildWestReelManager WildWest;
    private long maxBet = 0;
    private List<WildWestSpinResult.TriggerFreeInfo> freeInfo;

    public List<Text> bet;
    public List<Button> betBtn;

    private int index = -1;

    public Animator levelAni;

    public List<GameObject> mask;
    
    public void OnStart(WildWestReelManager _WildWest, long _maxBet, List<WildWestSpinResult.TriggerFreeInfo> _freeInfo)
    {
        WildWest = _WildWest;
        maxBet = _maxBet;
        freeInfo = _freeInfo;

        for (int i = 0; i < bet.Count; i++)
        {
            bet[i].text = this.BetLimit(freeInfo[i].Bet);
        }
        
        for (int i = 0; i < freeInfo.Count; i++)
        {
            if(freeInfo[i].Bet <= maxBet)
            {
                index++;
                Button btn = betBtn[i];
                btn.onClick.AddListener(delegate(){SelectBetLevel(btn);});
            }else
            {
                betBtn[i].interactable = false;
            }
        }
        if(index != -1) levelAni.SetTrigger(index.ToString());

        if(index == 0) foreach (var item in mask) item.SetActive(true);
        if(index == 1) mask[1].SetActive(true);
    }

    private void SelectBetLevel(Button btn)
    {
        Libs.AudioEntity.Instance.PlayEffect("enter_game_click");
        foreach (var item in betBtn) item.interactable = false;
        int btnIndex = betBtn.IndexOf(btn);
        levelAni.SetTrigger("open_" + btnIndex.ToString());
        new Libs.DelayAction(0.1f, null, ()=>
        {
            levelAni.SetTrigger("close");
            this.Close();
            WildWest.SetBetLevel(btnIndex, freeInfo[btnIndex].Bet);
        }).Play();
    }

    public string BetLimit(long bet)
    {
        // int value = bet * Core.ApplicationConfig.GetInstance ().ShowCoinsMultiplier;
        // if(value > 1000000)
        // {
        //     return (value/1000000).ToString() + "M";
        // }
        //
        // if(value > 1000)
        // {
        //     return (value/1000).ToString() + "K";
        // }

        return Utils.Utilities.GetBigNumberShow((long)bet);
    } 
}
