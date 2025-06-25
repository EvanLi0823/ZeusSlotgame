using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Classic;

public class WesternTreasureJackpotData
{
    public List<JackPotPrizePool> JackPotPrizePoolInfos = new List<JackPotPrizePool>();
    private List<JackpotItemRender> jackpotItemRenders ;

    public WesternTreasureJackpotData(List<JackpotItemRender> _jackpotItemRenders, SlotMachineConfig _slotConfig)
    {
        this.jackpotItemRenders = _jackpotItemRenders;
        this.InitJackPot(_slotConfig);
       
    }

    

    private void InitJackPot(SlotMachineConfig slotConfig)
    {
        JackPotPrizePoolInfos = slotConfig.JackPotPrizeInfos;

        if (JackPotPrizePoolInfos.Count == 0)
        {
            Utils.Utilities.LogPlistError("WesternTreasure JackPotData not config in classicconfig.plist.xml! ");
        }
        for (int i = 0; i < JackPotPrizePoolInfos.Count; i++)
        {
            switch (JackPotPrizePoolInfos[i].AwardName)
            {
                case "GRAND":
                    {
                        JackPotIncreaseMachineConfig increaseConfig = slotConfig.GetJackPotIncreaseConfig(JackPotPrizePoolInfos[i].AwardName);
                        jackpotItemRenders[0].SetPrizePoolData(slotConfig, JackPotPrizePoolInfos[i], increaseConfig);
                    }
                    break;
                case "MAJOR":
                    {
                        JackPotIncreaseMachineConfig increaseConfig = slotConfig.GetJackPotIncreaseConfig(JackPotPrizePoolInfos[i].AwardName);
                        jackpotItemRenders[1].SetPrizePoolData(slotConfig, JackPotPrizePoolInfos[i], increaseConfig);
                    }
                    break;
                case "MINOR":
                    {
                        JackPotIncreaseMachineConfig increaseConfig = slotConfig.GetJackPotIncreaseConfig(JackPotPrizePoolInfos[i].AwardName);
                        jackpotItemRenders[2].SetPrizePoolData(slotConfig, JackPotPrizePoolInfos[i], increaseConfig);
                    }
                    break;
                case "MINI":
                    {
                        JackPotIncreaseMachineConfig increaseConfig = slotConfig.GetJackPotIncreaseConfig(JackPotPrizePoolInfos[i].AwardName);
                        jackpotItemRenders[3].SetPrizePoolData(slotConfig, JackPotPrizePoolInfos[i], increaseConfig);
                    }
                    break;
                default:
                    break;
            }
        }
    }

    
    public void OnSpinJackPot()
    {
        if (BaseSlotMachineController.Instance.isFreeRun)
        {
            return;
        }
        for (int i = 0; i < JackPotPrizePoolInfos.Count; i++)
        {
            JackPotPrizePoolInfos[i].AddPoolAwardWithBet(BaseSlotMachineController.Instance.currentBetting);
        }

        for (int i = 0; i < jackpotItemRenders.Count; i++)
        {
            jackpotItemRenders[i].TxtAwardNum.IncreaseTo(JackPotPrizePoolInfos[i].GetTotalAward());
        }
        UpdataUI(true);
    }
    public void UpdataUI(bool isNeedPlaySound = false)
    {
        if (jackpotItemRenders == null || jackpotItemRenders.Count == 0)
        {
            return;
        }
        for (int i = 0; i < jackpotItemRenders.Count; i++)
        {
            jackpotItemRenders[i].Refresh(isNeedPlaySound);
        }
    }
    public double GetJackPotIncreaseValue(int index)
    {
        string name = GetJackPotName(index);
        JackPotPrizePool jackPotPrizePool = GetJackPotInof(name);
        if (jackPotPrizePool == null) return 0;
        return jackPotPrizePool.GetExtraAwardValue();
    }

    public double GetJackPotAward(string name)
    {
        //Debug.LogError("获取jackpot           "+name);
        JackPotPrizePool jackPotPrizePool = GetJackPotInof(name);
        double jackpotAward = 0;
        if (jackPotPrizePool != null)
        {
            jackpotAward = jackPotPrizePool.GetTotalAward();
            jackPotPrizePool.ExtraAward = 0;
            jackPotPrizePool.SaveExtraAward(jackPotPrizePool.ExtraAward);
        }
        return jackpotAward;
    }
    public double GetJackPotAward(int index)
    {
        string name = GetJackPotName(index);
            
        //Debug.LogError("获取jackpot           "+name);
        JackPotPrizePool jackPotPrizePool = GetJackPotInof(name);
        double jackpotAward = 0;
        if (jackPotPrizePool != null)
        {
            jackpotAward = jackPotPrizePool.GetTotalAward();
            jackPotPrizePool.ExtraAward = 0;
            jackPotPrizePool.SaveExtraAward(jackPotPrizePool.ExtraAward);
        }
       
        return jackpotAward;
    }

    private JackPotPrizePool GetJackPotInof(string jackPotName)
    {
        for (int i = 0; i < JackPotPrizePoolInfos.Count; i++)
        {
            if (jackPotName == JackPotPrizePoolInfos[i].AwardName)
            {
                return JackPotPrizePoolInfos[i];
            }
        }
        return null;
    }
    public string GetJackPotName(int index)
    {
        switch (index)
        {
            case 2:
                return "GRAND";
            case 3:
                return "MAJOR";
            case 4:
                return "MINOR";
            case 5:
                return "MINI";
            default:
                return "";
        }
    }
    public int JackPotIndex(string name)
    {
        switch (name)
        {
            case "GRAND":
                return 1;
            case "MAJOR":
                return 2;
            case "MINOR":
                return 3;
            case "MINI":
                return 4;
            default:
                return -1;
        }
    }
}
