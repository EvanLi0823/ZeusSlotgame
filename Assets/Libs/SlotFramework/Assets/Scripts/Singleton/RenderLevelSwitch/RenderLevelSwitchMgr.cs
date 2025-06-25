using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderLevelSwitchMgr
{
	public static RenderLevelSwitchMgr Instance{ get { return Classic.Singleton<RenderLevelSwitchMgr>.Instance; } }
	private RenderLevelSwitchMgr()
	{
		Dictionary<string,object> renderLevelDict = Plugins.Configuration.GetInstance().GetValueWithPath<Dictionary<string,object>>(GameConstants.RenderLevelMgr_Key,null);
		if (renderLevelDict != null)
		{
			List<object> loadTimeLevelList = Utils.Utilities.GetValue<List<object>>(renderLevelDict, GameConstants.LoadTimeLevel_Key, null);
			if (loadTimeLevelList != null)
			{
				for (int i = 0; i < loadTimeLevelList.Count; i++)
				{
					float loadTime = Utils.Utilities.CastValueFloat(loadTimeLevelList[i], 0);
					if (loadTime > 0)
						mLevelSecondList.Add(loadTime);
				}
			}
		}
	}

	//1,10,20,30,40,50,60
	public int CurrentRenderLevel = 2;
	public List<float> mLevelSecondList = new List<float>();


	public void SetRenderLevel(double loadTime)
	{
		CurrentRenderLevel = mLevelSecondList.Count;
		for (int i = 0; i < mLevelSecondList.Count; i++)
		{
			if (loadTime > mLevelSecondList[i]) continue;
			
			CurrentRenderLevel = i;
			break;
		}
	}

	public bool CheckRenderLevelIsOK(int checkPointNeedLevel)
	{
		#if UNITY_IOS
		return true;
		#else
		return CurrentRenderLevel <= checkPointNeedLevel;
		#endif
	}
}
