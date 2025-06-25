using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Classic;
public class InMachineContentCell : ConditionContentCell {
	private bool isInMachine = false;
	public InMachineContentCell(object info){
		try {
			isInMachine = Utils.Utilities.CastValueBool(info);
		} catch (System.Exception ex) {
			Utils.Utilities.LogPlistError ("InMachineContentCell:value format is not correct:"+info.ToString());
		}
	}
	public override bool ConditionIsOK(){
		if (isInMachine) {
			return BaseGameConsole.ActiveGameConsole ().IsInSlotMachine ();
		}
		return BaseGameConsole.ActiveGameConsole ().IsInLobby ();
	}
}
