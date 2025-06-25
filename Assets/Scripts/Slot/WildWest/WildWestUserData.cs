using System;
using System.Collections;
using System.Collections.Generic;
public class WildWestUserData
{
    //用户下注
    public static double currentBet;

    //用户下注
    public static bool allUnLock = false;
    
    //用户平均下注
    public static double averageBet 
    {
        get
        {
            if(userUseBet.Count == 0) return 100;
            double totalBet = 0;
            double count = 0;
            foreach (var value in userUseBet.Keys)
            {
                totalBet += value * userUseBet[value];
                count += userUseBet[value];
            }
            return Math.Round(totalBet/count);
        }
    }

    public static Dictionary<double, double> userUseBet = new Dictionary<double, double>();

    public static void SaveUseBet(double bet)
    {
        if(userUseBet.ContainsKey(bet))
        {
            userUseBet[bet] = userUseBet[bet] + 1;
        }else
        {
            userUseBet.Add(bet, 1);
        }
    }
}
