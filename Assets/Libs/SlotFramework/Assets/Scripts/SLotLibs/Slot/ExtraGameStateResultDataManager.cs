using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Classic;
using Utils;

public class ExtraGameStateResultDataManager {

	public enum ExtraGameType{
		ExtraGameType_None = 0,
		ExtraGameType_BaseGameHitBonus = 1,
		ExtraGameType_BaseGameHitFreeSpin = 2,
		ExtraGameType_BonusGame = 3,
		ExtraGameType_FreespinGame = 4,
		ExtraGameType_ExtraAward = 5,
	}
			
	public List<List<int>> baseResult;
	public long currentBetting; 
	public bool needSavePlayerSlotMachineGameStateData;
	public ExtraGameType currentExtraGameType = ExtraGameType.ExtraGameType_None;


	protected const string baseGameResultKey = "BASE_GAME_RESULT_KEY";
	protected const string currentBetingKey = "CURRENT_BETTING_KEY";
	protected const string currentExtraGameTypeKey = "CURRENT_EXTRA_GAME_TYPE";
	protected const string needSavePlayerSlotManchineGameStateDataKey = "NEED_SAVE_PLAYER_GAME_STATE_DATA";
	public ExtraGameType GetExtraGameTypeOnInit(){
		string prefix = SceneManager.GetActiveScene ().name;
		int type = SharedPlayerPrefs.GetPlayerPrefsIntValue (prefix+currentExtraGameTypeKey,0);
		return (ExtraGameType)type;
	}
	public virtual bool GetNeedRecoveryDataOnInit(){
		string prefix = SceneManager.GetActiveScene ().name;
		return SharedPlayerPrefs.GetPlayerBoolValue (prefix+needSavePlayerSlotManchineGameStateDataKey,false);
	}
	public virtual void SaveSlotMachinePlayerStateData(ReelManager reelManager){
		string prefix = SceneManager.GetActiveScene ().name;
		currentBetting = BaseSlotMachineController.Instance.currentBetting;
		baseResult = reelManager.baseGameResult;
		string resultValue = ConvertResultToString (baseResult);

		currentExtraGameType = reelManager.currentExtraType;
		int currentExtraType = (int)currentExtraGameType;
		SharedPlayerPrefs.SetPlayerPrefsStringValue (prefix+currentBetingKey,currentBetting.ToString());
		SharedPlayerPrefs.SetPlayerPrefsStringValue (prefix+baseGameResultKey,resultValue);
		SharedPlayerPrefs.SetPlayerPrefsBoolValue (prefix+needSavePlayerSlotManchineGameStateDataKey,true);
		SharedPlayerPrefs.SetPlayerPrefsIntValue (prefix+currentExtraGameTypeKey,currentExtraType);
		SharedPlayerPrefs.SavePlayerPreference ();
	}

	public virtual void RecoverySlotMachinePlayerStateData(ReelManager reelManager){
		string prefix = SceneManager.GetActiveScene ().name;
		currentBetting = Utilities.CastValueLong(SharedPlayerPrefs.GetPlayerPrefsStringValue(prefix+currentBetingKey,"0"));
		string results = SharedPlayerPrefs.GetPlayerPrefsStringValue (prefix+baseGameResultKey,"");
		currentExtraGameType = (ExtraGameType)SharedPlayerPrefs.GetPlayerPrefsIntValue (prefix+currentExtraGameTypeKey,0);
		needSavePlayerSlotMachineGameStateData = SharedPlayerPrefs.GetPlayerBoolValue (prefix + needSavePlayerSlotManchineGameStateDataKey, false);
		SharedPlayerPrefs.SetPlayerPrefsBoolValue (prefix+needSavePlayerSlotManchineGameStateDataKey,false);
		SharedPlayerPrefs.SavePlayerPreference ();
		baseResult = ConvertResultToList (results);

		switch (currentExtraGameType) {

		case ExtraGameType.ExtraGameType_BonusGame:
			break;
		case ExtraGameType.ExtraGameType_FreespinGame:
			reelManager.isFreespinBonus = true;
			break;
		case ExtraGameType.ExtraGameType_BaseGameHitBonus:
			break;
		case ExtraGameType.ExtraGameType_BaseGameHitFreeSpin:
			break;
		case ExtraGameType.ExtraGameType_ExtraAward:
			break;
		case ExtraGameType.ExtraGameType_None:
			break;
		default:
			break;
		}
		reelManager.currentExtraType = currentExtraGameType;
		//因为currentbetting这个值随时可能会改变，所以必须全部恢复重置
		BaseSlotMachineController.Instance.RestoreSlotsMachinesGameStateData (currentBetting);
		reelManager.baseGameResult = baseResult;
		reelManager.resultContent.ChangeResult(baseResult);
	}

	public List<List<int>> ConvertResultToList(string results){
		string[] reels = results.Split (';');
		List<List<int>> result = new List<List<int>> ();
		for (int i = 0; i < reels.Length; i++) {
			string[] symbols = reels [i].Split (',');
			result.Add(new List<int> ());
			for (int j = 0; j < symbols.Length; j++) {
				result [i].Add (Utils.Utilities.CastValueInt(symbols[j]));
			}
		}
		return result;
	}
	public string ConvertResultToString(List<List<int>> result){
		if (result==null) {
			return "";
		}
		string resultValue = "";
		for (int i = 0; i < result.Count; i++) {
			for (int j = 0; j < result[i].Count; j++) {
				resultValue += result [i][j].ToString();
				if (j<result[i].Count-1) {
					resultValue += ",";
				}
			}

			if (i < result.Count-1) {
				resultValue += ";";
			}
		}
		return resultValue;
	}
}
