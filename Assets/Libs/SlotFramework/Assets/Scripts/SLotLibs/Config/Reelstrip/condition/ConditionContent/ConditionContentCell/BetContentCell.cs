using UnityEngine;
using System.Collections;
using Classic;
public class BetContentCell : ConditionContentCell {
	public BetContentCell(object info):base(1,0){
		try {
			string[] temp = info.ToString().Split (',');
			min = Utils.Utilities.CastValueInt(temp[0]);
			max = Utils.Utilities.CastValueInt (temp[1]);
		} catch (System.Exception ex) {
			Utils.Utilities.LogPlistError ("Bet:"+info.ToString());
		}
	}
	public override bool ConditionIsOK(){
		float currentBetting = 0;
		if (BaseSlotMachineController.Instance!=null) {
			currentBetting = BaseSlotMachineController.Instance.currentBetting;
		} else if(TestController.Instance!=null){
			currentBetting = int.Parse(TestController.Instance.betMoney.text);
		}
		int betNum = Utils.Utilities.CastValueInt(currentBetting);
		if (betNum>=min&&betNum<=max) {
			return true;
		}
		return false;
	}
}
