using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Classic;

public class MaxBalanceCurrentBalanceRatioConditionContentCell:ConditionContentCell
{

	public MaxBalanceCurrentBalanceRatioConditionContentCell (object info):base(1,0)
	{
		
		try {
			string[] temp = info.ToString ().Split (',');
			min = Utils.Utilities.CastValueInt (temp [0]);
			max = Utils.Utilities.CastValueInt (temp [1]);
		} catch (System.Exception ex) {
			Utils.Utilities.LogPlistError ("MaxBalanceCurrentBalanceRatio:"+info.ToString());
		}
	}

	public override bool ConditionIsOK ()
	{
		long MaxBalance = UserManager.GetInstance ().MaxBalance;
		long currentBalance = UserManager.GetInstance ().UserProfile ().Balance ();
		if (currentBalance==0) {
			currentBalance = 1;
		}
		float maxBalanceCurrentBalanceRatio = (float)MaxBalance / (float)currentBalance;
		if (Mathf.Approximately (maxBalanceCurrentBalanceRatio, min) ||Mathf.Approximately (maxBalanceCurrentBalanceRatio, max) || (maxBalanceCurrentBalanceRatio >= min && maxBalanceCurrentBalanceRatio <= max)) {
			return true;
		}
		return false;
	}
}
