using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Classic;

public class UserMultiplerConditionContentCell : ConditionContentCell 
{
	public int multipler;
	public UserMultiplerConditionContentCell(object obj)
	{
		try{
			multipler = Utils.Utilities.CastValueInt(obj);
		} catch (System.Exception ex) {
			Utils.Utilities.LogPlistError ("SpecialOfferMarketMultiplerConditionContentCell:"+obj.ToString() + ex.Message);
		}
	}
	public override bool ConditionIsOK(){
		return Core.ApplicationConfig.GetInstance().ShowCoinsMultiplier == multipler;
	}
}
