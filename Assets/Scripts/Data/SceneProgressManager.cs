using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using Classic;

public class SceneProgressManager  {
	public static string DataFolderName = "SceneDataProgress";
	private static string JsonData = "";
	public static bool HasRestoreException = false;
	public static void SaveSceneJson<T>(string sceneName,T progressData) where T:SceneProgressDataBase
	{
		if (HasRestoreException)
		{
			HasRestoreException = false;
			return;
		}

		string folderPath = Path.Combine (Application.persistentDataPath, DataFolderName);
		if (!Directory.Exists (folderPath)) {
			Directory.CreateDirectory (folderPath);
		}

		string path = Path.Combine (folderPath, sceneName + ".json");

		string json = JsonUtility.ToJson (progressData,false);
//		string encryptJson = StringEncrypter.AesEncrypt (json);
		if (Debug.isDebugBuild) {
			Debug.Log (path);
		}

		try
		{
			File.WriteAllText (path,json);
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw;
		}
	}

	public static T LoadSceneJson<T>(string sceneName) where T: SceneProgressDataBase
	{
		string localPath = Path.Combine(DataFolderName,sceneName+".json");
		string path = Path.Combine (Application.persistentDataPath, localPath);

		if (File.Exists (path)) {
			JsonData = File.ReadAllText (path);
			T result = null;
			try{
				result = JsonUtility.FromJson<T> (JsonData);
				HasRestoreException = false;
			}
			catch(Exception e){
				Debug.LogError (sceneName+"解析json数据报错"+e.Message);
			}
			return result;
		}

		return null;
	}

	public static string GetSlotSaveData(string sceneName)
	{
		string localPath = Path.Combine(DataFolderName,sceneName+".json");
		string path = Path.Combine (Application.persistentDataPath, localPath);
		if (File.Exists (path)) {
			JsonData = File.ReadAllText (path);
			return JsonData;
		}
		return "";
	}

	public static void DeleteProgress(string sceneName, ReelManager reelManager)
	{
		if(string.IsNullOrEmpty(sceneName) || reelManager == null)
			return;
		
		string localPath = Path.Combine(DataFolderName,sceneName+".json");
		string path = Path.Combine (Application.persistentDataPath, localPath);
		if (File.Exists (path)) 
		{
			try
			{
				FileUtils.DeleteFile(path);
				// 恢复时，如果有feature，将json内容上报ES, 便于查询卡死
				if (reelManager.IsInBonusGame || reelManager.isFreespinBonus || reelManager.FreespinCount > 0 
				    || BaseSlotMachineController.Instance.onceMore)
				{
					Dictionary<string, object> para = new Dictionary<string, object>();
					para.Add("JsonData", JsonData);
					para.Add("Exception", HasRestoreException);
					JsonData = "";
					BaseGameConsole.ActiveGameConsole().LogBaseEvent(Analytics.RestoreLocalProgress, para);
				}
			}
			catch(Exception e){
				Debug.LogError (sceneName+"删除json数据报错"+e.Message);
			}
		}
	}

	/*
	public static string ConvertListToString(List<List<int>> result){
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

	public static List<List<int>> ConvertStringToList(string results){
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
	*/
}
