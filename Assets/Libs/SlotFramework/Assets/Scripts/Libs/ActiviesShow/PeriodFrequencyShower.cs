using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Libs;
using System;

/// <summary>
/// 在配置时间段按指时间间隔触发指定次数
/// </summary>
public class PeriodFrequencyTrigger
{
	public const string Count_Key = "showCount";

	public const string StartTime_Key = "StartTime";
	public const string EndTime_Key = "EndTime";

	public const string Expiration_Key = "expiration";
	public const string StartShowTime_Key = "startShowTime";

	public long EndTimeStamp { get; private set; }

	private int TriggerCount;
	private int TriggrtInterval;
	private long StartTimeStamp;
	private string strTriggerCountKey;
	private string strLastTriggerimeKey;
	private bool IsDataValid = false;//初始化为false,最终有效设置为true

	public PeriodFrequencyTrigger(Dictionary<string,object> dict,string strKey)
	{
		if (string.IsNullOrEmpty(strKey)) return;
		strTriggerCountKey = strKey + GameConstants._TriggerCount;
		strLastTriggerimeKey = strKey + GameConstants._LastTriggerTime;

		TriggerCount = Utils.Utilities.GetInt(dict, Count_Key, 1);//默认弹一次
		TriggrtInterval = Utils.Utilities.GetInt(dict, GameConstants.Interval_Key, 30);//默认调用弹出函数的时候弹出

		StartTimeStamp = Max(Utils.Utilities.GetLong(dict, StartTime_Key, 0),Utils.Utilities.GetLong(dict, StartShowTime_Key, 0));
		if (StartTimeStamp < 0) StartTimeStamp = 0;

		EndTimeStamp = Max(Utils.Utilities.GetLong(dict, EndTime_Key, -1),Utils.Utilities.GetLong(dict, Expiration_Key, -1));
		if (EndTimeStamp < 0) EndTimeStamp = -1;

//		if (StartTimeStamp >= EndTimeStamp && EndTimeStamp != -1)
//			return;

		IsDataValid = true;
	}

    public void CopyTimeDuration(Dictionary<string,object> tgtDict){
        long subEndTimeStamp = Utils.Utilities.GetLong(tgtDict, Expiration_Key, -1);
        if(subEndTimeStamp == -1) subEndTimeStamp = Utils.Utilities.GetLong(tgtDict, EndTime_Key, -1);
        if(subEndTimeStamp > EndTimeStamp || subEndTimeStamp == -1) tgtDict[Expiration_Key] = EndTimeStamp;

        if (!tgtDict.ContainsKey(StartTime_Key) && !tgtDict.ContainsKey(StartShowTime_Key)) tgtDict[StartTime_Key] = StartTimeStamp;
    }

	public void SetStartTimeStamp(long startTimeStamp)
	{
		StartTimeStamp = startTimeStamp;
		if (StartTimeStamp < 0) StartTimeStamp = 0;
	}

	public void SetEndTimeStamp(long endTimeStamp)
	{
		EndTimeStamp = endTimeStamp;
		if (StartTimeStamp < 0) EndTimeStamp = -1;
	}

	public bool IsValid() { return IsDataValid; }

	private long Max(long v1,long v2)
	{
		if (v1 > v2)
			return v1;
		else
			return v2;
	}

	public int HasTriggeredCount
	{
		get{ return SharedPlayerPrefs.GetPlayerPrefsIntValue(strTriggerCountKey, 0);}
		set{ SharedPlayerPrefs.SetPlayerPrefsIntValue(strTriggerCountKey, value); }
	}

	public long mLastTriggerTime
	{
		get{ return SharedPlayerPrefs.LoadPlayerPrefsLong(strLastTriggerimeKey, 0);}
		set{ SharedPlayerPrefs.SavePlayerPrefsLong(strLastTriggerimeKey, value); }
	}

	public void OnTrigger()
	{
		HasTriggeredCount++;
		mLastTriggerTime = TimeUtils.ConvertDateTimeLong(System.DateTime.Now);
	}

	public bool IsHaveTriggerCount()
	{
		if (TriggerCount == -1) return true;
		if (TriggerCount > 0)
		{
			if (HasTriggeredCount < TriggerCount)
				return true;
		}

		return false;
	}

	private bool IsTriggerCDOK()
	{
		return (TimeUtils.ConvertDateTimeLong(DateTime.Now) - mLastTriggerTime) > TriggrtInterval;
	}

	public bool IsCanTrigger()
	{
        if (!IsInTimeRange()) return false;
        if (!IsHaveTriggerCount()) return false;

        return IsTriggerCDOK();
	}

	public bool IsInTimeRange()
	{
		long curTimeStamp = TimeUtils.ConvertDateMillTimeLong(DateTime.Now);
		return StartTimeStamp < curTimeStamp && (curTimeStamp < EndTimeStamp || EndTimeStamp == -1);
	}

	public void DelLocalData()
	{
		SharedPlayerPrefs.DeleteLocalDataBykey(strTriggerCountKey);
		SharedPlayerPrefs.DeleteLocalDataBykey(strLastTriggerimeKey);
	}

	public bool IsExpiration()
	{
		if (EndTimeStamp == -1)return false;
		return TimeUtils.ConvertDateMillTimeLong(DateTime.Now) > EndTimeStamp;
	}

	public void ResetTriggerCountForPlistActivity()
	{
		long startTimeSecond = StartTimeStamp / 1000;
		if (mLastTriggerTime < startTimeSecond)
			HasTriggeredCount = 0;
	}
}