using System;
using System.Collections.Generic;
using Classic;
using Core;
using UnityEngine;
using Libs;

namespace RealYou.Tool
{
    public static class GMCommandUtil
    {
        private const int GMCommandID = 20999;
        private const string AddCoinsCMD_Prefix = "addcoins,";
        private const string CoinsMultiCMD_Prefix = "coinsmulti";
        private const string SetLevelCMD_Prefix = "setlevel,";
        private const string BetChangeTips_Prefix = "bettip";

        public static void SendGM2Server(string cmd)
        {
            if(string.IsNullOrEmpty(cmd))
                return;
            try
            {
                if (cmd.StartsWith(AddCoinsCMD_Prefix))
                {
                    string[] paraList = cmd.Split(',');
                    if (paraList != null && paraList.Length == 2)
                    {
                        long coinsNumber = long.Parse(paraList[1]);
                        UserManager.GetInstance().IncreaseBalanceAndSendMessage(coinsNumber);
                    }

                    return;
                }

                if (cmd.StartsWith(CoinsMultiCMD_Prefix))
                {
                    string[] paraList = cmd.Split(',');
                    if (paraList != null && paraList.Length == 2)
                    {
                        int coinsMulti = int.Parse(paraList[1]);
                        ApplicationConfig.GetInstance().ShowCoinsMultiplier = coinsMulti;
                        UserManager.GetInstance().UserProfile().SaveCoinsMultiplier(coinsMulti);
                        Messenger.Broadcast (SlotControllerConstants.OnBlanceChangeForDisPlay);
                    }
                    return;
                }
                // 显示Bet修改Tips
                if (cmd.StartsWith(BetChangeTips_Prefix))
                {
                    Messenger.Broadcast(GameDialogManager.OpenBetTips);
                    return;
                }
                
                
                if (cmd.StartsWith(SetLevelCMD_Prefix))
                {
                    string[] paraList = cmd.Split(',');
                    if (paraList != null && paraList.Length == 2)
                    {
                        new DelayAction(2f, null, ()=>{
                            BaseGameConsole.ActiveGameConsole().Restart();
                        }).Play();
                    }
                    return;
                }
            }
            catch (Exception e)
            {
                Debug.LogError("GMError:" + e.Message);
                return;
            }
        }
    }
}