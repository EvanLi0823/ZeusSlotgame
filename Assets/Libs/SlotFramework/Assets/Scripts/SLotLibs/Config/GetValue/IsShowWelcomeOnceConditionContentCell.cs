using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Classic;

public class IsShowWelcomeOnceConditionContentCell : ConditionContentCell 
{
	public bool IsShowWelcomeOnce;
	public IsShowWelcomeOnceConditionContentCell(object obj)
	{
		try{
			IsShowWelcomeOnce = Utils.Utilities.CastValueBool(obj);
		} catch (System.Exception ex) {
			Utils.Utilities.LogPlistError (this.GetType().Name + obj.ToString() + ex.Message);
		}
	}
	public override bool ConditionIsOK(){
		if(IsShowWelcomeOnce)
			return !UserManager.GetInstance().UserProfile().HasGrantInitialBonus();
		return true;
	}
}