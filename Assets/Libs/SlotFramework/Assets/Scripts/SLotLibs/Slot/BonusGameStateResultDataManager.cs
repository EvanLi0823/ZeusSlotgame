using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using Classic;
public class BonusGameStateResultDataManager : ExtraGameStateResultDataManager {

	//public const string needSavePlayerSlotManchineGameStateDataOnBonusGameKey = "NEED_SAVE_PLAYER_GAME_STATE_DATA_ON_BONUS_GAME";
	//public bool needSavePlayerSlotMachineGameStateDataOnBonusGame;
	public override void SaveSlotMachinePlayerStateData(ReelManager reelManager){
		base.SaveSlotMachinePlayerStateData (reelManager);
	}

	public override void RecoverySlotMachinePlayerStateData(ReelManager reelManager){
		base.RecoverySlotMachinePlayerStateData (reelManager);
	}
//	public override bool GetNeedRecoveryDataOnInit(){
//		string prefix = SceneManager.GetActiveScene ().name;
//		return SharedPlayerPrefs.GetPlayerBoolValue (prefix+needSavePlayerSlotManchineGameStateDataOnBonusGameKey,false);
//	}
	public virtual void SaveSpecialSlotMachinePlayerStateDataOnBonus(ReelManager reelManager){
		base.SaveSlotMachinePlayerStateData (reelManager);
	}
	public virtual void RecoverySpecialSlotMachinePlayerStateDataOnBonus(ReelManager reelManager){
		base.RecoverySlotMachinePlayerStateData (reelManager);
	}

}
