using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Classic;

public class RateOneSlotNumConditionContentCell : ConditionContentCell 
{
	public int RateNum;
	public RateOneSlotNumConditionContentCell(object obj)
	{
		try{
			RateNum = Utils.Utilities.CastValueInt(obj);
		} catch (System.Exception ex) {
			Utils.Utilities.LogPlistError (this.GetType().Name + obj.ToString() + ex.Message);
		}
	}
	public override bool ConditionIsOK(){
		return UserManager.GetInstance().UserProfile().GetRatingDialogPopedTime(UserManager.GetInstance().LastPlayMachine) < RateNum;
	}
}