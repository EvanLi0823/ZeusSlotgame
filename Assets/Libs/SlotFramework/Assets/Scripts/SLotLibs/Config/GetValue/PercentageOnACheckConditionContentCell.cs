using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Classic;

public class PercentageOnACheckConditionContentCell : ConditionContentCell 
{
	public float Percentage;
	public PercentageOnACheckConditionContentCell(object obj)
	{
		try{
			Percentage = Utils.Utilities.CastValueFloat(obj);
		} catch (System.Exception ex) {
			Utils.Utilities.LogPlistError (this.GetType().Name + obj.ToString() + ex.Message);
		}
	}
	public override bool ConditionIsOK(){
		return UnityEngine.Random.Range(0.0f, 100.0f) < Percentage;
	}
}