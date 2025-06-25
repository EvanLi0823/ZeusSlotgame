using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Classic;

public class PopUpSuspendFactorsConditionContentCell : ConditionContentCell {
	bool IgnoreTriggeredFeature = true;
	public PopUpSuspendFactorsConditionContentCell(object info){
		try {
			IgnoreTriggeredFeature = Utils.Utilities.CastValueBool(info);
		} catch (System.Exception ex) {
			Utils.Utilities.LogPlistError ("PopUpInterruptFactors:"+info.ToString());
		}
	}
	/// <summary>
	/// Conditions the is O.
	/// CheckTriggeredFeatureAboutPopUpDialogInfluencingFactors
	/// </summary>
	/// <returns><c>true</c>, if is O was conditioned, <c>false</c> otherwise.</returns>
	public override bool ConditionIsOK(){
		if (BaseSlotMachineController.Instance!=null) {
			BaseSlotMachineController bsmc = BaseSlotMachineController.Instance;
			if (bsmc.onceMore||bsmc.isFreeRun||bsmc.isBigWin||bsmc.isMegaWin||bsmc.isEpicWin||bsmc.reelManager.HasBonusGame
				||bsmc.reelManager.HasCircleGame||bsmc.reelManager.isFreespinBonus) {
				if (IgnoreTriggeredFeature) {
					return false;
				}
			}
		}
		return true;
	}

}
