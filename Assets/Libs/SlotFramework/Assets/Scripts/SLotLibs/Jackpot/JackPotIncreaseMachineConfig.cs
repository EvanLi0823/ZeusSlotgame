using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Utils;
public class JackPotIncreaseMachineConfig  {
	public JackPotIncreaseMachineConfig(Dictionary<string,object> infos)
	{
		this.JackPotType = Utilities.GetValue<string> (infos ,AwardNameKey, "");
		this.IncreaseEverySeconds = Utilities.GetLong (infos,IncreaseEverySecondsStr,0);
		this.ClearFrequencyMaxNum = Utilities.GetInt (infos,ClearFrequencyStr,0);
	}
//	public int JackPotIndex =0;
	public string JackPotType;
	public long IncreaseEverySeconds = 0;
	public int ClearFrequencyMaxNum=0;
//	private const string IndexStr = "Index"; 
	private const string AwardNameKey="AwardName";
	private const string IncreaseEverySecondsStr = "IncreaseEverySeconds";
	private const string ClearFrequencyStr = "ClearFrequency";
}
