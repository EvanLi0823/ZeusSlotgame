using System.Collections;
using System.Collections.Generic;
using Classic;
using UnityEngine;

public class SlotsBet
{
    public static long GetCurrentBet()
    {
        if (BaseGameConsole.singletonInstance.isForTestScene)
        {
            return TestController.Instance.currentBetting;
        }
        else if (BaseSlotMachineController.Instance != null && BaseGameConsole.singletonInstance.IsInSlotMachine())
        {
            return BaseSlotMachineController.Instance.currentBetting;
        }
        else
        {
            //有可能回到大厅的情况
            //return  PlayerPrefs.GetInt("Current_Bet", 0);
        }
        return 0;
    }
    
}
