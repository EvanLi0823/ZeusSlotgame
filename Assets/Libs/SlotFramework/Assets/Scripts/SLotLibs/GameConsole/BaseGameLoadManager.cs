using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseGameLoadManager
{
	public static BaseGameLoadManager Instance{ get { return Classic.Singleton<BaseGameLoadManager>.Instance; } }
  	
	private BaseGameLoadManager(){ }

	private const string loadTag_Key = "loadTag_Key";
	public string LoadingTag
	{
		get{ return SharedPlayerPrefs.GetPlayerPrefsStringValue(loadTag_Key, string.Empty); }
		set{ SharedPlayerPrefs.SetPlayerPrefsStringValue(loadTag_Key, value); }
	}

	public void Init() { }

	public bool IsUpdateApp()
	{
		if (string.IsNullOrEmpty(LoadingTag) || !string.Format (GameConstants.StringFormat_0_1_Key, Application.version, loadTag_Key).Equals(LoadingTag))
		{
			LoadingTag = string.Format (GameConstants.StringFormat_0_1_Key, Application.version, loadTag_Key);
			return true;
		}
		
		return false;
	}
}
