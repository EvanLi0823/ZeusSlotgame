using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Classic;
using System.Text;

public class SpinClickedMsgMgr
{
	public const string Error = "eor";
	public const string DOU = ",{0}";
	public const string FEN = ";";
	public const string BonusWin_Key = "bW";
	public const string LinesWin_Key = "lW";
	public const string TotalWin_Key = "TW";
	public const string ScatterWin_Key = "sW";
	public const string JackpotWin_Key = "jW";
	public const string FreespinWin_Key = "fW";
	public const string SymbolReels_Key = "SRs";
	bool IsResining;
	int CurSpinIndex;
	int CurResultIndex;
	OutCome CurSpinCurOutCome;
	OutCome BaseGameCurOutCome;
	List<OutCome> mOutComeList = new List<OutCome>();

	public string FeatureInUse { get; private set;}

	private static SpinClickedMsgMgr instance;
	public static SpinClickedMsgMgr Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new SpinClickedMsgMgr();
			}
			return instance;
		}
	}

	private SpinClickedMsgMgr(){}

	public void ResetData(bool isFreeRun)
	{
		if (!isFreeRun)
		{
			CurSpinIndex = 1;
			mOutComeList.Clear();
		}
		else if (RewardSpinManager.Instance.RewardSpinIsValid()) {
			CurSpinIndex = 1;
			mOutComeList.Clear();
		}

		CurResultIndex = 0;
	}

	public void SetIsRespin(bool flag)
	{
		IsResining = flag;
	}

	public void SetMultiplier(int multiplier)
	{
		if (CurSpinCurOutCome != null)
		{
			if (multiplier > 1)
				CurSpinCurOutCome.Multiplier = multiplier;
		}
	}

	public void SaveBaseGameOutBeforeEnterFreespin()
	{
		BaseGameCurOutCome = CurSpinCurOutCome;
	}

	public void RestroeBaseGameOutAfterExitFreespin()
	{
		CurSpinCurOutCome = BaseGameCurOutCome;
	}

	List<object> GetOutComeListData()
	{
		List<object> spinClickedList = new List<object>();
		for (int i = 0; i < mOutComeList.Count; i++)
		{
			spinClickedList.Add(mOutComeList[i].GetLog());
		}
		return spinClickedList;
	}
	public string GetEnCodeString(){
		List<object> spinClickedList = GetOutComeListData ();
		string spinString = MiniJSON.Json.Serialize (spinClickedList);
		return SequencePaidResultsManager.BoxMessageToServer (spinString);
	}


    public void AddOutCome(ReelManager reelManager)
	{
        if (BaseGameConsole.singletonInstance.isForTestScene)
        {
            return;
        }

		OutCome outCome = new OutCome();
		outCome.IsRespining = IsResining;
		outCome.IsFreespin = reelManager.isFreespinBonus;
		outCome.Str_A = reelManager.reelStrips.GetCurrentUseAName ();
		outCome.Str_R = reelManager.reelStrips.GetCurrentUseRName ();
//		SequencePaidResults currentRemoteRAResult = SequencePaidResultsManager.GetCurrentResult ();
//		if (SequencePaidResultsManager.GetCurrentResult ().isRunning) {
//			outCome.pId = currentRemoteRAResult.patternId;
//			PaidResult paidResult = currentRemoteRAResult.GetCurrentPaidResult ();
//			outCome.spId = paidResult.id;
//		}
		outCome.StrSymbol = GetResultString (reelManager);
  //       if (!IsResining && !reelManager.isFreespinBonus&&!reelManager.IsInBonusGame)
			outCome.SpinIndex = 0;
		// else
		// 	outCome.SpinIndex = CurSpinIndex++;
		
        outCome.ResultIndex = CurResultIndex++;


        if (mOutComeList.Count < 10)
        {
			mOutComeList.Add(outCome);
        }

        if (mOutComeList.Count==10)
        {
	        OutCome outComeEmpty = new OutCome();
	        outComeEmpty.Str_A = "";
	        outComeEmpty.Str_R = "";
	        outComeEmpty.StrSymbol = "";
	        mOutComeList.Add(outComeEmpty);
        }
        
        if (!outCome.IsFreespin && !outCome.IsRespining && !reelManager.IsInBonusGame)
			BaseGameCurOutCome = outCome;
		
		CurSpinCurOutCome = outCome;
	}

    /// <summary>
    /// 对应Pattern的版本
    /// </summary>
    public void AddOutCome(OutCome outCome)
    {
        if (BaseGameConsole.singletonInstance.isForTestScene)
        {
            return;
        }
        if(outCome!=null)
        {
            if (!IsResining && !BaseSlotMachineController.Instance.reelManager.isFreespinBonus &&!BaseSlotMachineController.Instance.reelManager.IsInBonusGame)
            {
                outCome.SpinIndex = 0;
            }else{
                outCome.SpinIndex = CurSpinIndex++;
            }

            CurResultIndex++;
            mOutComeList.Add(outCome);

            if (!outCome.IsFreespin && !outCome.IsRespining)
                BaseGameCurOutCome = outCome;
            CurSpinCurOutCome = outCome;
        }
    }

	public void AddWinDictData(string key,object o)
	{
        if (BaseGameConsole.singletonInstance.isForTestScene)
        {
            return;
        }

		if (CurSpinCurOutCome != null)
		{
			if (!CurSpinCurOutCome.mWinDict.ContainsKey(key))
			{
				CurSpinCurOutCome.mWinDict.Add(key, o);
			}else{
				CurSpinCurOutCome.mWinDict[key] = o;
			}
		}
	}

	public void AddRandomDictData(string key,object o)
	{
        if (BaseGameConsole.singletonInstance.isForTestScene)
        {
            return;
        }

		if (CurSpinCurOutCome != null)
		{
			if (!CurSpinCurOutCome.mRandomDataDict.ContainsKey(key))
			{
				CurSpinCurOutCome.mRandomDataDict.Add(key, o);
			}else{
				CurSpinCurOutCome.mRandomDataDict[key] = o;
			}
		}
	}

	public void OnInitReels()
	{
		IsPreLockStatus = false;
		FeatureInUse = string.Empty;
	}

	bool IsPreLockStatus;
	public void SetLockStatus(bool curLockStatus,string lockValue)
	{
		if (IsPreLockStatus)
		{
			if (curLockStatus)
				FeatureInUse = lockValue;//保持
			else
				FeatureInUse = GameConstants.Unlock_Key;//解锁
		}
		else
		{
			if (curLockStatus)
				FeatureInUse = lockValue;//锁定
			else
				FeatureInUse = string.Empty;//默认
		}

		IsPreLockStatus = curLockStatus;
	}

	public bool IsLockByPreviousSpin()
	{
		return IsPreLockStatus;
	}

	string GetResultString(ReelManager reelManager)
	{
		StringBuilder strRet = new StringBuilder("");
		int count = reelManager.resultContent.ReelResults.Count;
		int count_1 = count - 1;
		for (int i = 0; i < count; i++)
		{
			strRet.Append(GetSymbolNameByIndex(reelManager, i, 0));
			for (int j = 1; j < reelManager.resultContent.ReelResults[i].SymbolResults.Count; j++)
			{
				string strSymbolName = GetSymbolNameByIndex(reelManager, i, j);
				if (!string.IsNullOrEmpty(strSymbolName))
				{
					strRet.AppendFormat(DOU,strSymbolName);
				}
			}

			if(i < count_1) strRet.Append(FEN);
		}

		return strRet.ToString();
	}

	string GetSymbolNameByIndex(ReelManager reelManager,int reelIndex,int symbolIndex)
	{
		string strSymbolName = Error;
		SymbolMap.SymbolElementInfo info = reelManager.symbolMap.getSymbolInfo (reelManager.resultContent.ReelResults [reelIndex].SymbolResults [symbolIndex]);
		if (info != null)
		{
			strSymbolName = info.name;
		}
		return strSymbolName;
	}
}

public class OutCome
{
	public string Str_R { get; set;}
	public string Str_A { get; set;}
	public int SpinIndex { get; set;}//不同spin的索引
	public int Multiplier { get; set;}
	public int ResultIndex { get; set;} //同一次spin，由于symbol动画导致的结果改变的索引
	public bool IsFreespin { get; set;}
	public string StrSymbol{ get; set;}
	public bool IsRespining { get; set;}

    public Dictionary<string, object> RandomD = new Dictionary<string, object>();
    public Dictionary<string, object> mWinDict = new Dictionary<string, object>();
    public Dictionary<string, object> mRandomDataDict = new Dictionary<string, object>();

    private List<List<int>> symbolResults = null;
    public bool translateStrToListIntFlag = false;

    public List<List<int>> SymbolResult{
        get{
            if(translateStrToListIntFlag==false)
            {
                TranslateStrToListInt(BaseSlotMachineController.Instance.reelManager.symbolMap);
                translateStrToListIntFlag = true;
            }

            return symbolResults;
        }
        set{
            symbolResults = value;
        }
    }

    public void TranslateStrToListInt(SymbolMap symbolMap)
    {
        SymbolResult symbolResult = new SymbolResult(symbolMap,StrSymbol);

        if (symbolResult.symbolResults != null)
        {
            this.symbolResults = symbolResult.symbolResults;
        }else{
            this.symbolResults = null;
        }
    }

	public OutCome()
	{
		Multiplier = 1;
		SpinIndex = 0;
		ResultIndex = 0;
	}

    /// <summary>
    /// 用于从服务器解析
    /// </summary>
	public OutCome(Dictionary<string,object> infos)
	{
        Str_A = Utils.Utilities.GetValue<string>(infos, "A", "A1");
        Str_R= Utils.Utilities.GetValue<string>(infos, "R", "R1");
		SpinIndex  =(int) Utils.Utilities.GetValue<long>(infos,"sId",0);
		ResultIndex= (int)Utils.Utilities.GetValue<long>(infos,"rId",0);
		StrSymbol= Utils.Utilities.GetValue<string>(infos,"SRs","");
		IsFreespin= Utils.Utilities.GetValue<bool>(infos,"isF",false);
		IsRespining= Utils.Utilities.GetValue<bool>(infos,"isR",false);
        //从Infos读取FreeSpin时的Multiplier 默认为1
        Multiplier=(int)Utils.Utilities.GetValue<long>(infos, "mul", 1);

        if(infos.ContainsKey("ranD")&&infos["ranD"]!=null)
        {
            mRandomDataDict = infos["ranD"] as Dictionary<string, object>;
        }
	}
	
	public Dictionary<string,object> GetLog()
	{
		Dictionary<string,object> outComeDict = new Dictionary<string, object>();
			outComeDict.Add ("R", Str_R);
			outComeDict.Add ("A", Str_A);
			if (IsFreespin)
				outComeDict.Add ("isF", IsFreespin);
			if (IsRespining)
				outComeDict.Add ("isR", IsRespining);
			if (SpinIndex > 0)
				outComeDict.Add ("sId", SpinIndex);
			if (Multiplier > 1)
				outComeDict.Add ("mul", Multiplier);
			if (ResultIndex > 0)
				outComeDict.Add ("rId", ResultIndex);
			if (mWinDict.Count > 0)
				outComeDict.Add ("winD", mWinDict);
			if (mRandomDataDict.Count > 0)
				outComeDict.Add ("ranD", mRandomDataDict);
			outComeDict.Add ("SRs", StrSymbol);

		return outComeDict;
	}
}