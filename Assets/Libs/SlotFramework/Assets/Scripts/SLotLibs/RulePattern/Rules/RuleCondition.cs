using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class RuleCondition
{
//   public double ServerExpireTime; 
   public float ExpireTime = float.MaxValue;//后端单位为millsec，需要改成sec
   
   public int RuleSpinNum;
   public int CurrentSpinNum; //为动态
   
   public long RuleBalanceMin=0; //balance小于此值就不满足
   public long RuleBalanceMax = long.MaxValue; //balance大于此值就不满足

   public RuleCondition()
   {
      
   }
   public RuleCondition(Dictionary<string, object> obj,long _balance)
   {
      //时间
      if (obj.ContainsKey("a1b6"))
      {
         Dictionary<string, object> _timeDic = obj["a1b6"] as Dictionary<string, object>;
         ExpireTime= RuleUtils.CastValueFloat( _timeDic["c5"]) /1000;
//         ServerExpireTime = RuleUtils.ConvertToUnixTimestamp(DateTime.Now);
      }
//      spin次数
      if (obj.ContainsKey("a2b1"))
      {
         Dictionary<string, object> _spinNumDic = obj["a2b1"] as Dictionary<string, object>;
         if (_spinNumDic.ContainsKey("c3"))
         {
            RuleSpinNum = RuleUtils.CastValueInt(_spinNumDic["c3"]) + 1;
         }
         else if (_spinNumDic.ContainsKey("c4"))
         {
            RuleSpinNum = RuleUtils.CastValueInt(_spinNumDic["c4"]);
         }
      }

//      balance ratio
      if (obj.ContainsKey("a3b2"))
      {
         Dictionary<string, object> _balanceRatioDic = obj["a3b2"] as Dictionary<string, object>;
         if (_balanceRatioDic != null)
         {
            if (_balanceRatioDic.ContainsKey("c1"))
            {
               RuleBalanceMin =(long )( RuleUtils.CastValueFloat(_balanceRatioDic["c1"])* _balance);
            }
            if (_balanceRatioDic.ContainsKey("c2"))
            {
               RuleBalanceMin =(long )( RuleUtils.CastValueFloat(_balanceRatioDic["c2"])* _balance);
            }
            if (_balanceRatioDic.ContainsKey("c4"))
            {
               RuleBalanceMax =(long )( RuleUtils.CastValueFloat(_balanceRatioDic["c4"]) * _balance);
            }
            if (_balanceRatioDic.ContainsKey("c3"))
            {
               RuleBalanceMax =(long )( RuleUtils.CastValueFloat(_balanceRatioDic["c3"]) * _balance);
            }
         }
      }
      //      balance num
      if (obj.ContainsKey("a3b1"))
      {
         Dictionary<string, object> _balanceNumDic = obj["a3b1"] as Dictionary<string, object>;
         if (_balanceNumDic != null)
         {
            if (_balanceNumDic.ContainsKey("c1"))
            {
               RuleBalanceMin =(long )( RuleUtils.CastValueFloat(_balanceNumDic["c1"]));
            }
            if (_balanceNumDic.ContainsKey("c2"))
            {
               RuleBalanceMin =(long )( RuleUtils.CastValueFloat(_balanceNumDic["c2"]));
            }
            if (_balanceNumDic.ContainsKey("c4"))
            {
               RuleBalanceMax =(long )( RuleUtils.CastValueFloat(_balanceNumDic["c4"]));
            }
            if (_balanceNumDic.ContainsKey("c3"))
            {
               RuleBalanceMax =(long )( RuleUtils.CastValueFloat(_balanceNumDic["c3"]));
            }
         }
      }
      
      
      //TODO balance具体的值
   }

   public void AddOneTime()
   {
      CurrentSpinNum++;
   }

   public bool ConditionInvalid(long _currentBalance)
   {
//      时间
      double NowSec = RuleUtils.ConvertToUnixTimestamp(DateTime.Now);

      double tmp =  NowSec - ExpireTime ;
      
      if( tmp > 0){
         return true;
      }
      else
      {
//         Log.LogWhiteColor($"invalid Remaining time:{(-tmp)} sec");
      }
      
//      次数
      if (CurrentSpinNum >= RuleSpinNum)
         return true;

//      balance
      if (_currentBalance >= RuleBalanceMax)
         return true;
      if (_currentBalance < RuleBalanceMin)
         return true;
      
      return false;
   }
}
