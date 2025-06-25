using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Classic;
public class FreeSpinStateResulteDataManager : ExtraGameStateResultDataManager {

	public long currentWinCoins;
	public int leftFreeSpinCount;
	public int totalFreeSpinCount;
	public int awardMultipler;
	public int freespinCount;
	public List<List<int>> freespinResult;
	public bool currentFreespinFinished;
	public bool needShowCash;
	public int currentWinCash;
	protected const string CurrentWinCoinsForDisplayKey = "CURRENT_WIN_COINS_FOR_DISPLAY";
	protected const string CurrentWinCashForDisplayKey = "CURRENT_WIN_CASH_FOR_DISPLAY";
	protected const string LeftFreeSpinCountKey = "LEFT_FREESPIN_COUNT";
	protected const string TotalFreeSpinCountKey = "TOTAL_FREESPIN_COUNT";
	protected const string AwardMultiplerKey = "AWARD_MULTIPLER";
	protected const string FreeSpinCountKey = "FREE_SPIN_COUNT";
	protected const string ShowCashKey = "FREE_SPIN_SHOW_CASH";
	protected const string freeSpinResultKey = "FREE_SPIN_RESULT_KEY";
	protected const string currentFreespinFinishedKey = "CURRENT_FREESPIN_FINISHED";
	//protected const string needSavePlayerSlotManchineGameStateDataOnFreeSpinKey = "NEED_SAVE_PLAYER_GAME_STATE_DATA_ON_FREE_SPIN";
//	public override bool GetNeedRecoveryDataOnInit(){
//		string prefix = SceneManager.GetActiveScene ().name;
//		return SharedPlayerPrefs.GetPlayerBoolValue (prefix+needSavePlayerSlotManchineGameStateDataOnFreeSpinKey,false);
//	}
//	public virtual void SetNeedRecoveryDataOnInit(bool needRstore){
//		string prefix = SceneManager.GetActiveScene ().name;
//		SharedPlayerPrefs.SetPlayerPrefsBoolValue (prefix + needSavePlayerSlotManchineGameStateDataOnFreeSpinKey, needRstore);
//		SharedPlayerPrefs.SavePlayerPreference ();
//	}
	public override void SaveSlotMachinePlayerStateData(ReelManager reelManager){

        if (!reelManager.FreespinGame.HaveAddedTimes && !reelManager.FreespinGame.HaveInitedFreeSpinCount && reelManager.FreespinCount > 0)
        {
            reelManager.FreespinGame.currentSpinFinished = true;//弹框默认当次已完成
            reelManager.FreespinGame.LeftTime += reelManager.FreespinCount;
            reelManager.FreespinGame.TotalTime += reelManager.FreespinCount;
        }
        if (reelManager.FreespinGame.LeftTime == 0 && reelManager.FreespinGame.currentSpinFinished)
        {
            return;//当旋转次数完成，并且钱已经添加给玩家后，不对freespin状态进行保存，即也不恢复
		}

		base.SaveSlotMachinePlayerStateData (reelManager);

		string prefix = SceneManager.GetActiveScene ().name;
		if (reelManager.FreespinGame.currentSpinFinished) {
			leftFreeSpinCount = reelManager.FreespinGame.LeftTime;
		} 
		else {	
			leftFreeSpinCount = reelManager.FreespinGame.LeftTime + 1;
		}
		totalFreeSpinCount = reelManager.FreespinGame.TotalTime;
		awardMultipler = reelManager.FreespinGame.multiplier;
		currentWinCoins = BaseSlotMachineController.Instance.winCoinsForDisplay;//不要使用reelManager.FreespinGame.AwardInfo.awardValue,不准确
		
		currentWinCash = BaseSlotMachineController.Instance.winCashForDisplay;
		needShowCash = reelManager.FreespinGame.NeedShowCash;
		
		freespinResult = reelManager.freeSpinResult;
		string resultValue = ConvertResultToString (freespinResult);
		SharedPlayerPrefs.SetPlayerPrefsBoolValue (prefix+currentFreespinFinishedKey,reelManager.FreespinGame.currentSpinFinished);
		SharedPlayerPrefs.SetPlayerPrefsIntValue (prefix + LeftFreeSpinCountKey,leftFreeSpinCount);
		SharedPlayerPrefs.SetPlayerPrefsIntValue (prefix + TotalFreeSpinCountKey, totalFreeSpinCount);
		SharedPlayerPrefs.SetPlayerPrefsIntValue (prefix + AwardMultiplerKey, awardMultipler);
		SharedPlayerPrefs.SetPlayerPrefsBoolValue (prefix + ShowCashKey, needShowCash);
		SharedPlayerPrefs.SetPlayerPrefsIntValue (prefix + CurrentWinCashForDisplayKey, currentWinCash);
		SharedPlayerPrefs.SavePlayerPrefsLong (prefix + CurrentWinCoinsForDisplayKey, currentWinCoins);
		SharedPlayerPrefs.SetPlayerPrefsStringValue (prefix + freeSpinResultKey, resultValue);
		SharedPlayerPrefs.SavePlayerPreference ();

		//SetNeedRecoveryDataOnInit (true);
	}

	public override void RecoverySlotMachinePlayerStateData(ReelManager reelManager){
		base.RecoverySlotMachinePlayerStateData (reelManager);

		string prefix = SceneManager.GetActiveScene ().name;
		leftFreeSpinCount = SharedPlayerPrefs.GetPlayerPrefsIntValue (prefix + LeftFreeSpinCountKey,0);
		totalFreeSpinCount = SharedPlayerPrefs.GetPlayerPrefsIntValue (prefix + TotalFreeSpinCountKey, 0);
		awardMultipler = SharedPlayerPrefs.GetPlayerPrefsIntValue (prefix + AwardMultiplerKey, 0);
		string currentWinCoinsKey = prefix + CurrentWinCoinsForDisplayKey;
		currentWinCoins = SharedPlayerPrefs.LoadPlayerPrefsLong (currentWinCoinsKey, SharedPlayerPrefs.GetPlayerPrefsIntValue(currentWinCoinsKey,0));
		string results = SharedPlayerPrefs.GetPlayerPrefsStringValue (prefix + freeSpinResultKey, "");
		currentFreespinFinished = SharedPlayerPrefs.GetPlayerBoolValue (prefix+currentFreespinFinishedKey,true);
		reelManager.FreespinGame.currentSpinFinished = currentFreespinFinished;
		freespinResult = ConvertResultToList (results);
		reelManager.freeSpinResult = freespinResult;
		//needSavePlayerSlotMachineGameStateData = GetNeedRecoveryDataOnInit ();
		//SetNeedRecoveryDataOnInit (false);
		needShowCash = SharedPlayerPrefs.GetPlayerBoolValue(prefix + ShowCashKey,false);
		BaseSlotMachineController.Instance.winCoinsForDisplay = currentWinCoins;
		BaseSlotMachineController.Instance.winCashForDisplay = currentWinCash;
	}

	public virtual void SaveSlotMachinePlayerStateDataOnBaseGame(ReelManager reelManager){

		base.SaveSlotMachinePlayerStateData (reelManager);

		string prefix = SceneManager.GetActiveScene ().name;
		freespinCount = reelManager.FreespinCount;
		awardMultipler = reelManager.FreespinGame.multiplier;

		SharedPlayerPrefs.SetPlayerPrefsIntValue (prefix + FreeSpinCountKey, freespinCount);
		SharedPlayerPrefs.SetPlayerPrefsIntValue (prefix + AwardMultiplerKey, awardMultipler);
		SharedPlayerPrefs.SavePlayerPreference ();
		//SetNeedRecoveryDataOnInit (true);
	}

	public virtual void RecoverySlotMachinePlayerStateDataOnBaseGame(ReelManager reelManager){
		base.RecoverySlotMachinePlayerStateData (reelManager);

		string prefix = SceneManager.GetActiveScene ().name;
	
		freespinCount = SharedPlayerPrefs.GetPlayerPrefsIntValue (prefix + FreeSpinCountKey, 0);
		awardMultipler = SharedPlayerPrefs.GetPlayerPrefsIntValue (prefix + AwardMultiplerKey, 0);
		//needSavePlayerSlotMachineGameStateData = GetNeedRecoveryDataOnInit ();
	}

	public virtual void SaveSlotMachinePlayerMiddleStateDataWithoutExtraGameType(ReelManager reelManager){
		if (reelManager.FreespinGame.LeftTime==0&&reelManager.FreespinGame.currentSpinFinished &&
        (reelManager.BonusGame == null || (reelManager.BonusGame != null && !reelManager.BonusGame.IsBonusGame))) {
			return;//当旋转次数完成，并且钱已经添加给玩家后，不对freespin状态进行保存，即也不恢复
		}

        string prefix = SceneManager.GetActiveScene ().name;
        leftFreeSpinCount = reelManager.FreespinGame.LeftTime;
        totalFreeSpinCount = reelManager.FreespinGame.TotalTime;
		awardMultipler = reelManager.FreespinGame.multiplier;
		currentWinCoins = BaseSlotMachineController.Instance.winCoinsForDisplay;//不要使用reelManager.FreespinGame.AwardInfo.awardValue,不准确

		freespinResult = reelManager.freeSpinResult;
		string resultValue = ConvertResultToString(freespinResult);
		SharedPlayerPrefs.SetPlayerPrefsStringValue (prefix+freeSpinResultKey,resultValue);
		SharedPlayerPrefs.SetPlayerPrefsBoolValue (prefix+currentFreespinFinishedKey,reelManager.FreespinGame.currentSpinFinished);
		SharedPlayerPrefs.SetPlayerPrefsIntValue (prefix + LeftFreeSpinCountKey,leftFreeSpinCount);
		SharedPlayerPrefs.SetPlayerPrefsIntValue (prefix + TotalFreeSpinCountKey, totalFreeSpinCount);
		SharedPlayerPrefs.SetPlayerPrefsIntValue (prefix + AwardMultiplerKey, awardMultipler);
		SharedPlayerPrefs.SavePlayerPrefsLong (prefix + CurrentWinCoinsForDisplayKey, currentWinCoins);

		SharedPlayerPrefs.SavePlayerPreference ();
		//SetNeedRecoveryDataOnInit (true);
	}

	public virtual void RecoverySlotMachinePlayerMiddleStateDataWithoutExtraGameType(ReelManager reelManager){
		base.RecoverySlotMachinePlayerStateData (reelManager);

		string prefix = SceneManager.GetActiveScene ().name;
		leftFreeSpinCount = SharedPlayerPrefs.GetPlayerPrefsIntValue (prefix + LeftFreeSpinCountKey,0);
		totalFreeSpinCount = SharedPlayerPrefs.GetPlayerPrefsIntValue (prefix + TotalFreeSpinCountKey, 0);
		awardMultipler = SharedPlayerPrefs.GetPlayerPrefsIntValue (prefix + AwardMultiplerKey, 0);
		string currentWinCoinsKey = prefix + CurrentWinCoinsForDisplayKey;
		currentWinCoins = SharedPlayerPrefs.LoadPlayerPrefsLong (currentWinCoinsKey, SharedPlayerPrefs.GetPlayerPrefsIntValue(currentWinCoinsKey,0));
		currentFreespinFinished = SharedPlayerPrefs.GetPlayerBoolValue (prefix+currentFreespinFinishedKey,true);
		string results = SharedPlayerPrefs.GetPlayerPrefsStringValue (prefix + freeSpinResultKey, "");
		freespinResult = ConvertResultToList (results);
		reelManager.freeSpinResult = freespinResult;
		reelManager.FreespinGame.currentSpinFinished = currentFreespinFinished;
		//needSavePlayerSlotMachineGameStateData = GetNeedRecoveryDataOnInit ();
		BaseSlotMachineController.Instance.winCoinsForDisplay = currentWinCoins;
		//SetNeedRecoveryDataOnInit (false);

	}
}
