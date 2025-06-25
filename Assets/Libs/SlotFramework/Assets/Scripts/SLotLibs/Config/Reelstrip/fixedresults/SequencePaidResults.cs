using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Classic;

public class SequencePaidResults
{
	public string patternId {
		get;
		private set;
	}

	List<PaidResult> paidResults = new List<PaidResult> ();

	public bool isIdle {
		get { return paidResultIndex == -1 && paidResults.Count == 0; }
	}

	public bool isReady {
		get { return paidResultIndex == -1 && paidResults.Count > 0; }
	}

	public bool isRunning {
		get{ return  paidResultIndex < paidResults.Count && paidResultIndex >= 0 && paidResults.Count > 0; }
	}

	public bool isEnd {
		get;
		private set;
	}

	string machine;

	public bool isForceType{
		get;
		private set;
	}

	public bool isBalanceBetType{
		get;
		private set;
	}

	public bool isNowinType{
		get;
		private set;
	}

	int paidResultIndex = -1;
	string type;

	public SequencePaidResults ()
	{
        
	}

	 delegate bool MethodtDelegate();
	MethodtDelegate triggerCondition;

	public SequencePaidResults (Dictionary<string,object> infos, string machine, string type)
	{
		this.machine = machine;
		this.type = type;
		patternId = Utils.Utilities.GetValue<string> (infos, "pattern_id", "");
		List<object> spins = Utils.Utilities.GetValue<List<object>> (infos, "spins", null);
		if (spins != null) {
			for (int i = 0; i < spins.Count; i++) {
				if (spins [i] !=null)
				{
					PaidResult paidResult = new PaidResult(spins[i] as Dictionary<string, object>);
					paidResults.Add(paidResult);
				}
			}
		}
		if (SequencePaidResultsManager.TYPE_FORCE.Equals (this.type)) {
			isForceType = true;
			triggerCondition = ForceTrigger;
		} else if (this.type.StartsWith (SequencePaidResultsManager.Balance_Bet)) {
			isBalanceBetType = true;
			string a = type.Substring (SequencePaidResultsManager.Balance_Bet.Length);
			int b = 0; 
			int.TryParse (a, out b);
			triggerCondition = () => {
				return UserManager.GetInstance ().UserProfile ().Balance () <= BaseSlotMachineController.Instance.currentBetting * b;
			};
		} else if (SequencePaidResultsManager.NO_WIN.Equals (this.type)) {
			isNowinType = true;
            triggerCondition = ForceTrigger;
		} else {
			triggerCondition = ForceNotTrigger;
		}
	}

	bool ForceTrigger(){
		return true;
	}

	bool ForceNotTrigger(){
		return false;
	}

    /// <summary>
    /// 用于判断从服务器获得的是否有效
    /// </summary>
    public bool VaildSequencePaidResults()
    {
        if(paidResults.Count==0)
        {
            return false;
        }

        for (int i = 0; i < paidResults.Count;i++)
        {
            if(paidResults[i].ValidPaidResult()==false)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 返回从服务器获取的Pattern的OutCome
    /// </summary>
    public OutCome GetOutComeFromServer(bool moveIndex=true)
    {
        if(!isValid)
        {
            return null;
        }
		if(paidResultIndex >= paidResults.Count || paidResultIndex < 0)
        {
            Reset();
            return null;
        }
        OutCome tempOutCome = paidResults[paidResultIndex].GetOutComeFromServer();
        //如果该SequencePaidResults是no_win类型 则更新状态 检查该outCome是否是最后一个
        if(isNowinType)
        {
            SequencePaidResultsManager.outOfSequencePaidResults = paidResultIndex >= (paidResults.Count - 1);
        }
        if(tempOutCome==null)
        {
            Reset();
            return null;
        }

        return tempOutCome;
    }

	bool isValid = false;
//	Dictionary<string,object> infos = new Dictionary<string, object> ();
	public bool CheckCondition ()
	{
		isValid = false;
		if (BaseSlotMachineController.Instance == null) {
			isValid = false;
		} else {
			if (isReady) {
                if (BaseSlotMachineController.Instance.slotMachineConfig.Name ().Equals (machine) && BaseSlotMachineController.Instance.reelManager.IsCostBetSpin () ) 
                {
					if (triggerCondition()) {
						paidResultIndex = 0;
						isValid = true;
						SetESPuid();
						// BaseSlotMachineController.Instance.reelManager.PatternEsId = patternId;
						// BaseSlotMachineController.Instance.reelManager.PatternEsType = GetPatternType();
						// SequencePaidResultsManager.AddEsSendType(this.patternId,);
						// infos.Add ("puid",patternId);
						// infos.Add ("ptype",type);
						// BaseGameConsole.singletonInstance.LogBaseEvent ("PUevent",infos);
					}
				} else {
					isValid = false;
				}
			} else {
				if (isRunning) {
                    if (BaseSlotMachineController.Instance.reelManager.IsCostBetSpin ()) {
						paidResultIndex++;
                    }
					if (paidResultIndex < paidResults.Count) 
					{
						isValid = true;
						SetESPuid();
					} else {
						Reset ();
						isValid = false;
						isEnd = true;
					}
				}
			}
		}

		return isValid;
	}

	/// <summary>
	/// 设置发送ES事件所携带的PUID
	/// 修复后端发送no_win_3后，前端只在第一次spin时携带puid,后两次spin无puid信息
	/// </summary>
	private void SetESPuid()
	{
		if (paidResultIndex != -1 && paidResults.Count > 0)
		{
			BaseSlotMachineController.Instance.reelManager.PatternEsId = patternId;
			BaseSlotMachineController.Instance.reelManager.PatternEsType = GetPatternType();
		}
	}

	private int GetPatternType()
	{
		if (this.isForceType)
		{
			return 1;
		}
		else if (this.isNowinType)
		{
			return 2;
		}
		//将来扩展
		return 0;
	}

	public void Reset ()
	{
		paidResults.Clear ();
		paidResultIndex = -1;
	}
	
//	public string GetInfos()
//	{
//		return Utils.Utilities.GetValue<string>(infos, "puid",string.Empty);
//	}
//
//	public void ClearInfos()
//	{
//		infos.Clear();
//	}

	public PaidResult GetCurrentPaidResult(){
		if (isRunning) {
			return  paidResults [paidResultIndex];
		}
		return null;
	}

    //废弃老旧的查询代码 目前仅供参考
    //public List<List<int>>  GetSymbolResults ()
    //{
    //  if (!isValid)
    //      return null;
    //  if (paidResultIndex >= paidResults.Count) {
    //      Reset ();
    //      return null;
    //  }
    //  List<List<int>> result = paidResults [paidResultIndex].GetSymbolResults (BaseSlotMachineController.Instance.reelManager.symbolMap);
    //  if (result == null) {
    //      Reset ();
    //      return null;
    //  }
    //  return result;
    //}

    //废弃老旧的查询代码 目前仅供参考
    //public List<List<int>> GetSymbolResultsNotMove()
    //{
    //    if (!isValid)
    //        return null;
    //    if (paidResultIndex >= paidResults.Count)
    //    {
    //        Reset();
    //        return null;
    //    }
    //    List<List<int>> result = paidResults[paidResultIndex].GetSymbolResultsNotMove(BaseSlotMachineController.Instance.reelManager.symbolMap);
    //    if (result == null)
    //    {
    //        Reset();
    //        return null;
    //    }
    //    return result;
    //}

    //废弃老旧的查询代码 目前仅供参考
    //public List<List<int>> GetHappyEasterWildSymbolResults()
    //{
    //    if (!isValid)
    //        return null;
    //    if (paidResultIndex >= paidResults.Count)
    //    {
    //        Reset();
    //        return null;
    //    }
    //    List<List<int>> result = paidResults[paidResultIndex].GetHappyEasterWildSymbolResults(BaseSlotMachineController.Instance.reelManager.symbolMap);
    //    if (result == null)
    //    {
    //        Reset();
    //        return null;
    //    }
    //    return result;
    //}

    //废弃老旧的查询代码 目前仅供参考
    //public int GetFreeSpinMultiplier()
    //{
    //    if (!isValid)
    //        return 1;
    //    if (paidResultIndex >= paidResults.Count)
    //    {
    //        Reset();
    //        return 1;
    //    }
    //    int freeSpinMultiplier=paidResults[paidResultIndex].GetFreeSpinMultiplier();
    //    if(freeSpinMultiplier>0)
    //    {
    //        return freeSpinMultiplier;
    //    }
    //    return 1;
    //}

    //废弃老旧的查询代码 目前仅供参考
    //public T GetDataFromRandomD<T>(string key, T defaultValue)
    //{
    //    if (!isValid)
    //        return defaultValue;
    //    if (paidResultIndex >= paidResults.Count)
    //    {
    //        Reset();
    //        return defaultValue;
    //    }
    //    return paidResults[paidResultIndex].GetDataFromRandomD<T>(key,defaultValue);
    //}

}
