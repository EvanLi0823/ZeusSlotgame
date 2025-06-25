using UnityEngine;
using System.Collections;
using Classic;
public class BalanceContentCell : ConditionContentCell {
    public long minL = 1L;
    public long maxL= 0L;
	public BalanceContentCell(object info){
		try {
			string[] temp = info.ToString().Split (',');
            minL = Utils.Utilities.CastValueLong(temp[0]);
			maxL = Utils.Utilities.CastValueLong (temp[1]);

		} catch (System.Exception ex) {
			Utils.Utilities.LogPlistError ("Balance:"+info.ToString());
		}
	}
	public override bool ConditionIsOK(){
        long balanceNum = UserManager.GetInstance ().UserProfile ().Balance();
        if (balanceNum>=minL&&balanceNum<=maxL) {
			return true;
		}
		return false;
	}
}
