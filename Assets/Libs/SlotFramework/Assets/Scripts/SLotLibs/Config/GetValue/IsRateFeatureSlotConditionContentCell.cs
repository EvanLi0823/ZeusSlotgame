using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Classic;

public class IsRateFeatureSlotConditionContentCell : ConditionContentCell 
{
	public bool IsCheckFeature;
	public IsRateFeatureSlotConditionContentCell(object obj)
	{
		try{
			IsCheckFeature = Utils.Utilities.CastValueBool(obj);
		} catch (System.Exception ex) {
			Utils.Utilities.LogPlistError (this.GetType().Name + obj.ToString() + ex.Message);
		}
	}
	public override bool ConditionIsOK(){
		if (IsCheckFeature)
			return UserManager.GetInstance().IsLastPlayMachineFeature;

		return false;
	}
}