using UnityEngine;
using System.Collections;
using Classic;
public class PlayerLevelContentCell : ConditionContentCell {
	public PlayerLevelContentCell(object info):base(1,0){
		try {
			string[] temp = info.ToString().Split (',');
			min = Utils.Utilities.CastValueInt(temp[0]);
			max = Utils.Utilities.CastValueInt (temp[1]);
		} catch (System.Exception ex) {
			Utils.Utilities.LogPlistError ("PlayerLevel:"+info.ToString());
		}
	}

	public override bool ConditionIsOK(){
		int level = UserManager.GetInstance ().UserProfile ().Level ();
		//Debug.Log ("playerlevel:"+level+" min:"+min+" max:"+max);
		if (level>=min&&level<=max) {
			return true;
		}
		return false;
	}
}
