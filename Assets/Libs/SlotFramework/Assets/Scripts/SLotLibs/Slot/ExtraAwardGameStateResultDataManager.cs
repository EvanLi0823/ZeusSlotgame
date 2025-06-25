using UnityEngine;
using System.Collections;
using Classic;

public class ExtraAwardGameStateResultDataManager : ExtraGameStateResultDataManager {

	public override void SaveSlotMachinePlayerStateData(ReelManager reelManager){
		base.SaveSlotMachinePlayerStateData (reelManager);
	}

	public override void RecoverySlotMachinePlayerStateData(ReelManager reelManager){
		base.RecoverySlotMachinePlayerStateData (reelManager);
	}
}
