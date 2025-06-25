using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Classic;

public class RateSlotRequireSpinNumConditionContentCell : ConditionContentCell 
{
	public int spinNum;
	public RateSlotRequireSpinNumConditionContentCell(object obj)
	{
		try{
			spinNum = Utils.Utilities.CastValueInt(obj);
		} catch (System.Exception ex) {
			Utils.Utilities.LogPlistError (this.GetType().Name + obj.ToString() + ex.Message);
		}
	}

	public override bool ConditionIsOK(){
		return UserManager.GetInstance().LastPlayMachineSpinNum > spinNum;
	}
}