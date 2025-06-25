using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using Classic;

public class SpecialEntryHandler
{
	public const string FAVORITE_MACHINE_CAMPAIGN = "FavoriteMachine";
	public const string HIDDEN_MACHINE_CAMPAIGN = "HiddenMachine";

	private static SpecialEntryHandler instance;
	public static SpecialEntryHandler Instance {
		get {
			if (instance == null) {
				instance = new SpecialEntryHandler();
			}
			return instance;
		}
	}

	// 返回特殊的favorite机器的名称，若不存在则返回空串
	public string FindAdCampaignMachineName(string campaignName) {
		var result = FindAdCampaignMachineNames(campaignName);
		return result.Count > 0 ? FindAdCampaignMachineNames(campaignName)[0] : string.Empty;
	}

	public List<string> FindAdCampaignMachineNames(string campaignName)
	{
		var ret = new List<string>();
		return ret;
	}

	public void RecordAdCampaignMachineUsage(string campaignName, string machineName) {
		SharedPlayerPrefs.SetPlayerPrefsIntValue(campaignName + "_" + machineName, SharedPlayerPrefs.GetPlayerPrefsIntValue(campaignName + "_" + machineName, 0) + 1);
	}

	public int GetAdCampaignMachineUsage(string campaignName, string machineName) {
		return SharedPlayerPrefs.GetPlayerPrefsIntValue(campaignName + "_" + machineName, 0);
	}


	// 用于储存正则表达式匹配信息的类
	private class AdCampaignMatch {
		private string prefix;
		private string postfix;
		public string eventType;
		public void Match(string data) {
			matchResults.Clear();
			string pattern = @"(?i)(?<=" + prefix + @")\d+(?=" + postfix + ")";
			foreach (Match match in Regex.Matches(data, pattern)) {
				matchResults.Add(int.Parse(match.Value));
			}
		}
		public List<int> matchResults = new List<int>();
		public AdCampaignMatch(string prefix, string postfix, string eventType) {
			this.prefix = prefix;
			this.postfix = postfix;
			this.eventType = eventType;
		}
	}
}
