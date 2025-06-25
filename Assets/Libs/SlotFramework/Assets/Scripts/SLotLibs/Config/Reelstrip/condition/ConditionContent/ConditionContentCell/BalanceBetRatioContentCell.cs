using UnityEngine;
using System.Collections;
using Classic;
public class BalanceBetRatioContentCell:ConditionContentCell  {
	/// <summary>
	/// Initializes a new instance of the <see cref="BalanceBetRatioContentCell"/> class.
	/// default value is forbidden the conditioncontentcell return true;
	/// </summary>
	/// <param name="info">Info.</param>
	public BalanceBetRatioContentCell(object info):base(1,0){
		try {
			string[] temp = info.ToString().Split (',');
			min = Utils.Utilities.CastValueInt(temp[0]);
			max = Utils.Utilities.CastValueInt (temp[1]);

		} catch (System.Exception ex) {
			Utils.Utilities.LogPlistError ("BalanceBetRatio:"+info.ToString());
		}
	}
	public override bool ConditionIsOK(){
		long currentBetting = 0;
		if (BaseSlotMachineController.Instance!=null) {
			currentBetting = BaseSlotMachineController.Instance.currentBetting;
		} else if(TestController.Instance!=null){
			currentBetting = Utils.Utilities.CastValueLong(TestController.Instance.betMoney.text);
		}
		float balanceNum = Utils.Utilities.CastValueFloat(UserManager.GetInstance ().UserProfile ().Balance());
		float balanceAndBetRatio = balanceNum/currentBetting;
		//Debug.Log ("BalanceBetRatioContentCell:"+balanceAndBetRatio+" min:"+min+" max:"+max);
		if (Mathf.Approximately(balanceAndBetRatio,min)||Mathf.Approximately(balanceAndBetRatio,max)||(balanceAndBetRatio >= min&&balanceAndBetRatio <= max)) {
			return true;
		}
		return false;
	}
}
