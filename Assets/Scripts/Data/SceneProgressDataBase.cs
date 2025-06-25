using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class SceneProgressDataBase {
	public string BaseGameResultString=string.Empty;

	public bool IsInFreespin;  	  //	freespin进行中
//	public bool IsJustInFreespin; //刚进入freespin

	public long currentWinCoins;
	public int leftFreeSpinCount;
	public int totalFreeSpinCount;
	public int awardMultipler;
	public int freespinCount;
	public string FreespinResultString;
//	public bool currentFreespinFinished;
	public string RName;
	public string AName;
	public long currentBetting;
	public bool needShowCash;
	public int currentWinCash;
	public bool IsInBonusGame;  //是否在bonus中
	public bool baseAwardAdded;//freespin前是否已经计算了basegame的线奖
	public List<List<int>> BaseGameResultList{
		get{
			if (!string.IsNullOrEmpty (BaseGameResultString)) {
				return Utils.Utilities.ConvertStringToList<int> (BaseGameResultString);
			}
			return null;
		}
	}

	public List<List<int>> FreespinResultList{
		get{
			if (!string.IsNullOrEmpty (FreespinResultString)) {
				return Utils.Utilities.ConvertStringToList<int> (FreespinResultString);
			}
			return null;
		}
	}
//	public List<List<int>> RespinResultList{
//		get{
//			if (!string.IsNullOrEmpty (RespinResultString)) {
//				return SceneProgressManager.ConvertStringToList (RespinResultString);
//			}
//			return null;
//		}
//	}

//	public static string ConvertListToString(List<List<int>> result){
//		if (result==null) {
//			return "";
//		}
//		string resultValue = "";
//		for (int i = 0; i < result.Count; i++) {
//			for (int j = 0; j < result[i].Count; j++) {
//				resultValue += result [i][j].ToString();
//				if (j<result[i].Count-1) {
//					resultValue += ",";
//				}
//			}
//
//			if (i < result.Count-1) {
//				resultValue += ";";
//			}
//		}
//		return resultValue;
//	}
//
//	public static List<List<int>> ConvertStringToList(string results){
//		string[] reels = results.Split (';');
//		List<List<int>> result = new List<List<int>> ();
//		for (int i = 0; i < reels.Length; i++) {
//			string[] symbols = reels [i].Split (',');
//			result.Add(new List<int> ());
//			for (int j = 0; j < symbols.Length; j++) {
//				result [i].Add (Utils.Utilities.CastValueInt(symbols[j]));
//			}
//		}
//		return result;
//	}
}


