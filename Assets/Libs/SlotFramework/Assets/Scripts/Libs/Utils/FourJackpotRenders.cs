using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FourJackpotRenders : MonoBehaviour
{
    [SerializeField]
    private UIIncreaseNumber m_GrandTxt;
    [SerializeField]
    private UIIncreaseNumber m_MajorTxt;
    [SerializeField]
    private UIIncreaseNumber m_MinorTxt;
    [SerializeField]
    private UIIncreaseNumber m_MiniTxt;

    private JackPotPrizePool m_GrandJackpot;
    private JackPotPrizePool m_MajorJackpot;
    private JackPotPrizePool m_MinorJackpot;
    private JackPotPrizePool m_MiniJackpot;

    private SlotMachineConfig m_slotConfig;

    private void Awake()
    {
        Messenger.AddListener<SlotControllerConstants.JACKPOT_TYPE>(SlotControllerConstants.REFRESH_JACKPOT_UI_TYPE,RefreshJpUi);
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener<SlotControllerConstants.JACKPOT_TYPE>(SlotControllerConstants.REFRESH_JACKPOT_UI_TYPE,RefreshJpUi);
    }

    public void InitJackpotData(SlotMachineConfig slotConfig)
    {
        m_slotConfig = slotConfig;

        m_GrandJackpot = this.m_slotConfig.GetJackPotPool(SlotControllerConstants.JACKPOT_TYPE.GRAND.ToString());
        m_MajorJackpot = this.m_slotConfig.GetJackPotPool(SlotControllerConstants.JACKPOT_TYPE.MAJOR.ToString());
        m_MinorJackpot = this.m_slotConfig.GetJackPotPool(SlotControllerConstants.JACKPOT_TYPE.MINOR.ToString());
        m_MiniJackpot = this.m_slotConfig.GetJackPotPool(SlotControllerConstants.JACKPOT_TYPE.MINI.ToString());

    }
    
    public double GetJackPotIncreaseValue(string jackpotName)
    {
        JackPotPrizePool jackPotPrizePool = m_slotConfig.GetJackPotPool(jackpotName);
        return jackPotPrizePool.GetExtraAwardValue();
    }

    public double GetJackpotAward(string jackpotName)
    {
        double ret = 0;
        if (m_slotConfig !=null)
        {
            JackPotPrizePool jackpot = this.m_slotConfig.GetJackPotPool(jackpotName);
            if (jackpot != null)
            {
                ret = jackpot.GetTotalAward();
                jackpot.ExtraAward = 0f;
                jackpot.SaveExtraAward(0);
            }
            //awardValue = (long)tmp;
        }
        return ret;
    }

    public void RefreshJpUi(SlotControllerConstants.JACKPOT_TYPE jpType)
    {
        if (jpType == SlotControllerConstants.JACKPOT_TYPE.GRAND)
        {
            m_GrandTxt.SetNumber(this.m_GrandJackpot.GetTotalAward(), 0f);
        }
        else if(jpType == SlotControllerConstants.JACKPOT_TYPE.MAJOR) m_MajorTxt.SetNumber(this.m_MajorJackpot.GetTotalAward(), 0f);
        else if(jpType == SlotControllerConstants.JACKPOT_TYPE.MINOR)m_MinorTxt.SetNumber(this.m_MinorJackpot.GetTotalAward(), 0f);
        else if(jpType == SlotControllerConstants.JACKPOT_TYPE.MINI)m_MiniTxt.SetNumber(this.m_MiniJackpot.GetTotalAward(), 0f);
    }

    public void JackpotNumRefreshDirect()
    {
        m_GrandTxt.SetNumber(this.m_GrandJackpot.GetTotalAward(), 0f);
        m_MajorTxt.SetNumber(this.m_MajorJackpot.GetTotalAward(), 0f);
        if (m_MinorTxt != null)
        {
            m_MinorTxt.SetNumber(this.m_MinorJackpot.GetTotalAward(), 0f);
        }
        if(m_MiniTxt !=null)
        {
            m_MiniTxt.SetNumber(this.m_MiniJackpot.GetTotalAward(), 0f);
        }
    }

    public void JackpotNumRefreshTween()
    {
        m_GrandTxt.IncreaseTo(this.m_GrandJackpot.GetTotalAward());
        m_MajorTxt.IncreaseTo(this.m_MajorJackpot.GetTotalAward());
        if(m_MinorTxt!=null)
        {
            m_MinorTxt.IncreaseTo(this.m_MinorJackpot.GetTotalAward());
        }

        if (m_MiniTxt != null)
        {
            m_MiniTxt.IncreaseTo(this.m_MiniJackpot.GetTotalAward());
        }
    }

    public void AddGrandWithBet()
    {
        long bet = CurrentBetting;
        this.m_GrandJackpot.AddPoolAwardWithBet(bet);
    }

    public void AddMajorWithBet()
    {
        long bet = CurrentBetting;
        this.m_MajorJackpot.AddPoolAwardWithBet(bet);
    }

    public void AddMinorWithBet()
    {
        long bet = CurrentBetting;
        if(this.m_MinorJackpot !=null)
        {
            this.m_MinorJackpot.AddPoolAwardWithBet(bet);
        }
    }

    public void AddMiniWithBet()
    {
        long bet = CurrentBetting;
        if(m_MiniJackpot !=null)
        {
            this.m_MiniJackpot.AddPoolAwardWithBet(bet);
        }
    }

    long CurrentBetting
    {
        get
        {
            if (BaseSlotMachineController.Instance != null)
            {
                return (long)BaseSlotMachineController.Instance.currentBetting;
            }
            else
            {
                return (long)Classic.TestController.Instance.currentBetting;
            }
        }
    }
}
