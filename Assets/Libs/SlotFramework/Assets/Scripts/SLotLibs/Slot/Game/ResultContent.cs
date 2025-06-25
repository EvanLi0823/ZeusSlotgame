using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using System.Linq;
using Classic;

public class ResultContent
{
	public static readonly int INVALID_RESULT_INDEX = int.MinValue;

	[Obsolete]
	public List<int> ExcludeResultReelIndexes {
		get;
		set;
	}

	public enum CreateResultStrategy
	{
		CreateResultByweight,
		CreateResultByRandom,
		CreateSymbolResultIndependentByWeight
	}

	public List<ReelResult> ReelResults {
		get;
		private set;
	}
	

	public AwardResult awardResult = new AwardResult ();
	public AwardResult lastAwardResult;
	public List<ReelStrip> currentReelStrips;
	GameConfigs gameConfigs;
    private BoardConfigs m_BoardConfig;
    
    //后端化的时候转动的轮子symbol
    public List<List<int>> ReelSpinShowData = new List<List<int>>();

	public ResultContent ()
	{
	}

	public ResultContent(GameConfigs gameConfigs,BoardConfigs boardConfigs)
	{
		ReelResults = new List<ReelResult> ();
		this.m_BoardConfig = boardConfigs;
		for (int i = 0; i < boardConfigs.ReelConfigs.Length; i++)
		{
			ReelResult reelResult = new ReelResult ();
			ReelResults.Add (reelResult);
		}
		this.gameConfigs = gameConfigs;
	}
	

	public ResultContent (ReelStripManager reelStrips, GameConfigs gameConfigs, BoardConfigs boardConfigs)
	{
		ReelResults = new List<ReelResult> ();
		this.gameConfigs = gameConfigs;
        this.m_BoardConfig = boardConfigs;
		switch (gameConfigs.createResultStrategy) {
		case CreateResultStrategy.CreateResultByRandom:
			for (int i = 0; i < gameConfigs.reelConfigs.Length; i++) {
				ReelResult reelResult = new ReelResult ();
				ReelResults.Add (reelResult);
			}
			break;
		case CreateResultStrategy.CreateSymbolResultIndependentByWeight:
		case CreateResultStrategy.CreateResultByweight:
			{
				reelStrips.InitStrips();//获取初始化带子前必须先初始化
				currentReelStrips = reelStrips.GetInitStrips ();

				for (int i = 0; i < currentReelStrips.Count; i++) {
					ReelResult reelResult = new ReelResult (currentReelStrips [i]);
					ReelResults.Add (reelResult);
				}
			}
			break;

		default:
			for (int i = 0; i < gameConfigs.reelConfigs.Length; i++) {
				ReelResult reelResult = new ReelResult ();
				ReelResults.Add (reelResult);
			}
			break;
		}

		InitShowStrip();
	}

	public void ChangeStripsResultContent (ReelStripManager reelStrips, GameConfigs gameConfigs, BoardConfigs boardConfigs)
	{
		this.gameConfigs = gameConfigs;
        this.m_BoardConfig = boardConfigs;
		switch (gameConfigs.createResultStrategy) {
		case CreateResultStrategy.CreateSymbolResultIndependentByWeight:
		case CreateResultStrategy.CreateResultByweight:
			{
				ReelResults.Clear ();
				currentReelStrips = reelStrips.GetSelectedStrips ();
				for (int i = 0; i < currentReelStrips.Count; i++) {
					ReelResult reelResult = new ReelResult (currentReelStrips [i]);
					ReelResults.Add (reelResult);
				}
			}
			break;
		}

		InitShowStrip();
	}

	private void InitShowStrip()
	{
		ReelSpinShowData.Clear();
		foreach (ReelResult reelResult in ReelResults)
		{
			ReelSpinShowData.Add(reelResult.ShowIndexs);
		}
	}

    private int NumberNeedCreate(int reelIndex)
    {
        int ret = 0;
        if(this.m_BoardConfig != null)
        {
            ret = this.m_BoardConfig.ReelConfigs[reelIndex].ReelShowNum;
        }
        else if(this.gameConfigs != null)
        {
            ret = gameConfigs.reelConfigs[reelIndex].resultLenth;
        }
        return ret;
    }

	public void CreateRawResult ()
	{

        for (int i = 0; i < ReelResults.Count; i++) {
//			if (ExcludeResultReelIndexes == null || ExcludeResultReelIndexes.Contains (i) == false) {

				ReelResults [i].CreateResult (NumberNeedCreate(i), gameConfigs);
//			}
		}
	}

	public void CreateResultByDesign (int[] designDatas)
	{
		for (int i = 0; i < ReelResults.Count; i++) {
			ReelResults [i].CreateResultByDesign (NumberNeedCreate(i), designDatas [i]);
		}
	}

	public void SaveLastAwardResult ()
	{
		lastAwardResult = awardResult;
		awardResult = new AwardResult ();
	}

	public List<int> GetLastAwardResultElementIndexList (int reelIndex)
	{
		if (lastAwardResult != null) {
			return lastAwardResult.GetAwardElementIndexList (reelIndex);
		}
		return null;
	}

	public bool IsEndReelForLastAwardResult (int reelIndex)
	{
		if (lastAwardResult != null) {
			return lastAwardResult.IsLastReelForAward (reelIndex);
		}
		return false;
	}

	public void KickAwardResult (GameConfigs gameConfigs, AwardResult awardResult)
	{
		if (awardResult == null) {
			return;
		}

		for (int i = 0; i < ReelResults.Count; i++) {
			ReelResults [i].KickAwardResult (NumberNeedCreate(i), gameConfigs, awardResult.GetAwardElementIndexList (i));
		}
	}

	//修改 spin 牌面
	public void ChangeResult (List<List<int>> outResults)
	{
		if (outResults == null)
			return;

		for (int i = 0; i < ReelResults.Count && outResults.Count > i; i++) {
			List<int> subResult = new List<int>(outResults [i]);//需要做深拷贝，不要改变参数的结果值
			if (subResult == null)
				continue;

			ReelResult reelResult = ReelResults [i];
			reelResult.SymbolResults.Clear ();
			reelResult.SymbolResults.AddRange (subResult);
		}
	}

	/// 从服务器接收结果列表(固定的轮子结果不会改变)
	public void ChangePaidResult (List<List<int>> outResults)
	{
		if (outResults == null)
			return;
		for (int i = 0; i < ReelResults.Count && outResults.Count > i; i++) {

//			if (ExcludeResultReelIndexes != null && ExcludeResultReelIndexes.Contains (i)) {
//				BaseSlotMachineController.Instance.reelManager.ReelLocked = true;
//				continue;
//			}
			List<int> subResult = outResults [i];
			if (subResult == null)
				continue;
			ReelResult reelResult = ReelResults [i];
			reelResult.SymbolResults.Clear ();
			reelResult.SymbolResults.AddRange (subResult);
		}
	}

	/// 判断轮子是否有被锁住 用于判断如何向服务器发送OutCome
//	public bool ReelLocked ()
//	{
//		for (int i = 0; i < ReelResults.Count; i++) {
//			if (ExcludeResultReelIndexes != null && ExcludeResultReelIndexes.Contains (i)) {
//				return true;
//			}
//		}
//
//		return false;
//	}

	public void ChangeOneReelResult (int reelIndex, List<int> changedResults)
	{
		if (reelIndex >= 0 && reelIndex < ReelResults.Count) {
			ReelResult reelReulst = ReelResults [reelIndex];
			for (int j = 0; j < changedResults.Count && reelReulst.SymbolResults.Count > j; j++) {
				reelReulst.SymbolResults [j] = changedResults [j];
			}
		}
	}

	public void ChangeOneSymbolResult (int reelIndex, int elementIndex, int symbolIndex)
	{
		if (reelIndex >= 0 && reelIndex < ReelResults.Count) {
			ReelResult reelReulst = ReelResults [reelIndex];
			if (elementIndex >= 0 && elementIndex < reelReulst.SymbolResults.Count) {
				reelReulst.SymbolResults [elementIndex] = symbolIndex;
			}
		}
	}

	public bool ReelHasInvalidResult (int reelIndex)
	{
		ReelResult reelResult = GetReelResult (reelIndex);
		return reelResult != null ? reelResult.HasInvalidResult () : true;
	}
	public Dictionary<string,int> FreeSymbolDic=new Dictionary<string, int>();
	public Dictionary<int, List<int>> GetSpecialSymbolIndex (string speicalTag, SymbolMap symbolMap)
	{
		FreeSymbolDic.Clear();
		Dictionary<int, List<int>> specialSymbolIndex = new Dictionary<int, List<int>> ();
		if (ReelResults != null)
		{
			for (int i = 0; i < ReelResults.Count; i++) {
				int startIndex = 0;
				if(ReelResults [i].SymbolResults==null) continue;
				int endIndex = ReelResults [i].SymbolResults.Count;
				if (gameConfigs!=null)
				{
					if (gameConfigs.hasBlank) {
						startIndex = 1;
						endIndex = ReelResults [i].SymbolResults.Count - 1;
					}
				}
				List<int> indexInReel = new List<int> ();
				for (int j = startIndex; j < endIndex; j++) {
					SymbolMap.SymbolElementInfo info = symbolMap?.getSymbolInfo (ReelResults [i].SymbolResults [j]);
					if (info!=null&&info.getBoolValue (speicalTag)) {
						indexInReel.Add (j);
						RefreshFreeSymbolDic(info.name);
					}
				}
				specialSymbolIndex.Add (i, indexInReel);
			}
		}
		return specialSymbolIndex;
	}
	
	private void RefreshFreeSymbolDic(string name)
	{
		if (FreeSymbolDic.ContainsKey(name))
		{
			FreeSymbolDic[name] += 1;
		}
		else
		{
			FreeSymbolDic[name] = 1;
		}
	}

	public Dictionary<int, List<int>> GetSymbolIndexMap (string symbolName, SymbolMap symbolMap)
	{
		return GetSymbolIndexMap (symbolMap.getSymbolIndex (symbolName), symbolMap);
	}

	public Dictionary<int, List<int>> GetSymbolIndexMap (int symbolIndex, SymbolMap symbolMap)
	{
		Dictionary<int, List<int>> specialSymbolIndex = new Dictionary<int, List<int>> ();
		for (int i = 0; i < ReelResults.Count; i++) {
//			if (!IsCheckExcludeResultReels) {
//				if (ExcludeResultReelIndexes != null) {
//					if (ExcludeResultReelIndexes.Contains (i))
//						continue;
//				}
//			}

			int startIndex = 0;
			int endIndex = ReelResults [i].SymbolResults.Count;

			if (gameConfigs.hasBlank) {
				startIndex = 1;
				endIndex = ReelResults [i].SymbolResults.Count - 1;
			}

			List<int> indexInReel = new List<int> ();
			for (int j = startIndex; j < endIndex; j++) {
				if (ReelResults [i].SymbolResults [j] == symbolIndex)
					indexInReel.Add (j);
			}
			specialSymbolIndex.Add (i, indexInReel);
		}
		return specialSymbolIndex;
	}

	public Dictionary<int, List<int>> GetMiddleLineSpecialSymbolIndex (string speicalTag, SymbolMap symbolMap)
	{
		Dictionary<int, List<int>> specialSymbolIndex = new Dictionary<int, List<int>> ();
		for (int i = 0; i < ReelResults.Count; i++) {
			List<int> indexInReel = new List<int> ();
			int j = ReelResults [i].SymbolResults.Count / 2;
			SymbolMap.SymbolElementInfo info = symbolMap.getSymbolInfo (ReelResults [i].SymbolResults [j]);
			if (info.getBoolValue (speicalTag)) {
				indexInReel.Add (j);
				RefreshFreeSymbolDic(info.name);
			}
			specialSymbolIndex.Add (i, indexInReel);
		}
		return specialSymbolIndex;
	}

	public Dictionary<int, List<int>> GetMiddleLineSymbolIndexMap (string symbolName, SymbolMap symbolMap)
	{
		return GetMiddleLineSymbolIndexMap (symbolMap.getSymbolIndex (symbolName), symbolMap);
	}

	public List<int> GetMiddleLineSymbolIndex ()
	{
		List<int> symbolIndexs = new List<int> ();
		for (int i = 0; i < ReelResults.Count; i++) {

			int j = ReelResults [i].SymbolResults.Count / 2;

			symbolIndexs.Add (ReelResults [i].SymbolResults [j]);
		}
		return symbolIndexs;
	}

	public Dictionary<int, List<int>> GetMiddleLineSymbolIndexMap (int symbolIndex, SymbolMap symbolMap = null)
	{
		Dictionary<int, List<int>> specialSymbolIndex = new Dictionary<int, List<int>> ();
		for (int i = 0; i < ReelResults.Count; i++) {
//			if (!IsCheckExcludeResultReels) {
//				if (ExcludeResultReelIndexes != null) {
//					if (ExcludeResultReelIndexes.Contains (i))
//						continue;
//				}
//			}

			List<int> indexInReel = new List<int> ();
			int j = ReelResults [i].SymbolResults.Count / 2;
			if (symbolIndex == ReelResults [i].SymbolResults [j])
				indexInReel.Add (j);
			specialSymbolIndex [i] = indexInReel;
		}
		return specialSymbolIndex;
	}

	public Dictionary<int, List<int>> GetSpecialSymbolIndexInReelData (string speicalTag, SymbolMap symbolMap)
	{
		Dictionary<int, List<int>> specialSymbolIndex = new Dictionary<int, List<int>> ();
		for (int i = 0; i < ReelResults.Count; i++) {
			int startIndex = 0;
			int endIndex = ReelResults [i].reelData.Count;
			List<int> indexInReel = new List<int> ();
			for (int j = startIndex; j < endIndex; j++) {
				SymbolMap.SymbolElementInfo info = symbolMap.getSymbolInfo (ReelResults [i].reelData [j].value);
				if (info.getBoolValue (speicalTag)) {
					indexInReel.Add (j);
				}
			}
			specialSymbolIndex.Add (i, indexInReel);
		}
		return specialSymbolIndex;
	}

	//检测指定索引的带子上是否包含指定的symbol
	public bool CheckTheReelExistTheSymbolByName (string symbolName, int reelIndex, SymbolMap symbolMap)
	{
		if (reelIndex < 0 || reelIndex >= ReelResults.Count)
			return false;
            
		int startIndex = 0;
		int endIndex = ReelResults [reelIndex].SymbolResults.Count;
		if (gameConfigs.hasBlank) {
			startIndex = 1;
			endIndex = ReelResults [reelIndex].SymbolResults.Count - 1;
		}

		for (int i = startIndex; i < endIndex; i++) {
			SymbolMap.SymbolElementInfo info = symbolMap.getSymbolInfo (ReelResults [reelIndex].SymbolResults [i]);
			if (symbolName.Equals (info.name)) {
				return true;
			}
		}

		return false;
	}

	public Dictionary<int, List<int>> GetSymbolIndexByName (string symbolName, SymbolMap symbolMap)
	{
		Dictionary<int, List<int>> specialSymbolIndex = new Dictionary<int, List<int>> ();
		for (int i = 0; i < ReelResults.Count; i++) {
			int startIndex = 0;
			int endIndex = ReelResults [i].SymbolResults.Count;
			if (gameConfigs.hasBlank) {
				startIndex = 1;
				endIndex = ReelResults [i].SymbolResults.Count - 1;
			}
			List<int> indexInReel = new List<int> ();
			for (int j = startIndex; j < endIndex; j++) {
				SymbolMap.SymbolElementInfo info = symbolMap.getSymbolInfo (ReelResults [i].SymbolResults [j]);
				if (symbolName.Equals (info.name)) {
					indexInReel.Add (j);
				}
			}
			specialSymbolIndex.Add (i, indexInReel);
		}
		return specialSymbolIndex;
	}

	public ReelResult GetReelResult (int reelIndex)
	{
		if (ReelResults != null) {
			if (reelIndex < ReelResults.Count) {
				return ReelResults [reelIndex];
			}
		}
		return null;
	}
    //修改整条带子的数据
    public void ChangerReelDataReelValue(int reelIndex,  int value)
    {
        ReelResults[reelIndex].ChangerReelDataReelValue(value);
    }

    public void ChangerReelDateElementValue (int reelIndex, int elementIndex, int value)
	{
		if(reelIndex<0||reelIndex>=ReelResults.Count) return;
		ReelResults [reelIndex].ChangerReelDateElementValue (elementIndex, value);
	}

    /// <summary>
    /// 此方法是用于打印带子元素位置
    /// </summary>
    /// <returns></returns>
    public List<List<int>> GetReelElementPositonIndexes()
    {
        List<List<int>> ret = new List<List<int>>();
        for (int i = 0; i < ReelResults.Count; i++)
        {
            int startIndex = 0;
            int endIndex = ReelResults[i].SymbolResults.Count;
            if (gameConfigs.hasBlank)
            {
                startIndex = 1;
                endIndex = ReelResults[i].SymbolResults.Count - 1;
            }
            List<int> indexInReel = new List<int>();
            for (int j = startIndex; j < endIndex; j++)
            {
                indexInReel.Add(j);
            }
            ret.Add(indexInReel);
        }
        return ret;
    }

    public class ReelResult
	{
		public List<WeightData> reelData {
			get;
			private set;
		}

		public List<int> SymbolResults;
		public int resultFirstAtReel;
		public int resultLastAtReel;
		private int totalWeight = 0;

		private List<int> _ShowIndexs = new List<int>();
		public List<int> ShowIndexs
		{
			get
			{
				if (_ShowIndexs.Count == 0)
				{
					InitShowResult();
				}
				return _ShowIndexs; 
			}
			set => _ShowIndexs = value;
		}

		public ReelResult (int lenth, bool hasHalf, int iconLenth)
		{
			SymbolResults = new List<int> ();
			reelData = new List<WeightData> ();

			totalWeight = 0;
			for (int i = 0; i < lenth; i++) {
				if (hasHalf) {
					WeightData weightData2 = new WeightData (-1);
					reelData.Add (weightData2);
				}
				int temp = UnityEngine.Random.Range (0, iconLenth);
				WeightData weightData = new WeightData (temp);
				totalWeight += weightData.weight;
				reelData.Add (weightData);
			}
		}

		void InitShowResult()
		{
			reelData.ForEach(delegate(WeightData data) { this._ShowIndexs.Add(data.value); });
		}

		public ReelResult ()
		{
			SymbolResults = new List<int> ();
			reelData = new List<WeightData> ();
		}

		public ReelResult (ReelStrip reelStrip)
		{
			SymbolResults = new List<int> ();
			reelData = new List<WeightData> ();
			for (int i = 0; i < reelStrip.stripElementInfos.Count; i++) {
				WeightData weightData = new WeightData (reelStrip.stripElementInfos [i].symboIndex, reelStrip.stripElementInfos [i].weight, reelStrip.stripElementInfos [i].symbolName);

				totalWeight += weightData.weight;
				reelData.Add (weightData);
			}

		}

		/// <summary>
		/// 替换掉原有的静态数据
		/// Res the change reel strip.
		/// </summary>
		/// <param name="_reelStrip">Reel strip.</param>
		public void ReChangeReelStrip (ReelStrip _reelStrip)
		{
			totalWeight = 0;
			reelData.Clear ();
			for (int i = 0; i < _reelStrip.stripElementInfos.Count; i++) {
				WeightData weightData = new WeightData (_reelStrip.stripElementInfos [i].symboIndex, _reelStrip.stripElementInfos [i].weight);
				totalWeight += weightData.weight;
				reelData.Add (weightData);
			}
		}

		public void CreateResult (int lenth, GameConfigs gameConfigs)
		{
			switch (gameConfigs.createResultStrategy) {
			case CreateResultStrategy.CreateResultByRandom:
				CreateResultByRandom (lenth, gameConfigs);
				break;
			case CreateResultStrategy.CreateResultByweight:
				CreateResultByweight (lenth);
				break;
			case CreateResultStrategy.CreateSymbolResultIndependentByWeight:
				CreateSymbolResultIndependent (lenth);
				break;
			default:
				CreateResultByRandom (lenth, gameConfigs);
				break;
			}
		}

		public void CreateResultByDesign (int lenth, int centerIndex)
		{
			resultFirstAtReel = centerIndex;
			resultFirstAtReel = (resultFirstAtReel - lenth / 2 + reelData.Count) % reelData.Count;
			SymbolResults.Clear ();
			//Debug.Log ("CreateResultByDesign resultFirstAtReel:"+resultFirstAtReel);
			for (int i = 0; i < lenth; i++) {
				int value = reelData [(i + resultFirstAtReel) % reelData.Count].value;
				SymbolResults.Add (value);
				//Debug.Log ("CreateResultByDesign i:"+i+" value:"+value);
			}

			resultLastAtReel = (lenth - 1 + resultFirstAtReel) % reelData.Count;
			//Debug.Log ("CreateResultByDesign resultLastAtReel:"+resultLastAtReel);
		}

		private void CreateResultByweight (int lenth)
		{
			int temp = UnityEngine.Random.Range (0, totalWeight);
			resultFirstAtReel = reelData.Count - 1;

			int lastWeights = 0;
			int weights = reelData [0].weight;
			for (int i = 1; i < reelData.Count; i++) {
				if (lastWeights <= temp && temp < weights) {
					resultFirstAtReel = i - 1;
					break;
				} else {
					lastWeights = weights;
					weights += reelData [i].weight;
				}
			}

			resultFirstAtReel = (resultFirstAtReel - lenth / 2 + reelData.Count) % reelData.Count;
			SymbolResults.Clear ();

			for (int i = 0; i < lenth; i++) {
				SymbolResults.Add (reelData [(i + resultFirstAtReel) % reelData.Count].value);
			}

			resultLastAtReel = (lenth - 1 + resultFirstAtReel) % reelData.Count;
		}

		public int PreSymbolIndex (int preLength)
		{
			return reelData [(resultFirstAtReel - preLength + reelData.Count) % reelData.Count].value;
		}

		public int AfterSymbolIndex (int afterLength)
		{
			return reelData [(resultLastAtReel + afterLength) % reelData.Count].value;
		}

		public void UpdateReelPosition (List<int> result)
		{
			int startIndex = -1;
			if (result == null || result.Count == 0)
				return;
			int matchNum = 0;
			int length = result.Count - 1;//减去隐藏元素
			for (int i = 0; i < reelData.Count; i++) {
				matchNum = 0;
				if (reelData [i].value == result [0]) {
					matchNum++;
					for (int j = 1; j < length; j++) {
						if (reelData [(i + j) % reelData.Count].value == result [j])
							matchNum++;
						else
							break;
					}
				}
				if (matchNum == length) {
					startIndex = i;
					break;
				}
			}

			if (startIndex != -1) {
				resultFirstAtReel = startIndex;
				resultLastAtReel = (length - 1 + resultFirstAtReel) % reelData.Count;
			}

		}

		private void CreateResultByRandom (int lenth, GameConfigs gameConfigs)
		{
			SymbolResults.Clear ();
			for (int i = 0; i < lenth; i++) {
				SymbolResults.Add (gameConfigs.RandGenSymbol ());
			}
		}

		private void CreateSymbolResultIndependent (int lenth)
		{
			SymbolResults.Clear ();
			for (int i = 0; i < lenth; i++) {
				int temp = UnityEngine.Random.Range (0, totalWeight);
				resultFirstAtReel = reelData.Count - 1;
				int lastWeights = 0;
				int weights = reelData [0].weight;
				for (int j = 1; j < reelData.Count; j++) {
					if (lastWeights <= temp && temp < weights) {
						resultFirstAtReel = j - 1;
						break;
					} else {
						lastWeights = weights;
						weights += reelData [j].weight;
					}
				}
				SymbolResults.Add (reelData [resultFirstAtReel].value);
			}

		}

		public void KickAwardResult (int lenth, GameConfigs gameConfigs, List<int> awardElementIndexList)
		{
			if (awardElementIndexList == null) {
				return;
			}

			awardElementIndexList.Sort ();

			switch (gameConfigs.createResultStrategy) {
			case CreateResultStrategy.CreateResultByweight:
				{
					for (int i = awardElementIndexList.Count - 1; i >= 0; i--) {
						SymbolResults.RemoveAt (awardElementIndexList [i]);
						resultLastAtReel = (resultLastAtReel + 1) % reelData.Count;
						SymbolResults.Add (reelData [resultLastAtReel].value);
					}
				}
				break;

			case CreateResultStrategy.CreateResultByRandom:
			default:
				{
					for (int i = awardElementIndexList.Count - 1; i >= 0; i--) {
						SymbolResults.RemoveAt (i);
						SymbolResults.Add (gameConfigs.RandGenSymbol ());
					}
				}
				break;
			}
		}


		public bool HasInvalidResult ()
		{
			return SymbolResults == null || SymbolResults.Count == 0 || SymbolResults.Contains (INVALID_RESULT_INDEX);
		}

		public bool HasInvalidResultAt (int elementIndex)
		{
			return SymbolResults == null || SymbolResults.Count == 0 || elementIndex >= SymbolResults.Count || SymbolResults [elementIndex] == INVALID_RESULT_INDEX;
		}

        public void ChangerReelDataReelValue(int value)
        {
	        for (int i = 0; i < reelData.Count; i++)
	        {
				reelData[i].value = value;
				ShowIndexs[i] = value;
	        }
        }

        public void ChangerReelDateElementValue (int index, int value)
		{
			reelData [index].value = value;
			ShowIndexs[index] = value;
		}

		/// 传进去一个数组，代表需要剔除的索引个数，得到正确的数据.
		//返回动画显示的需要gliding出现的数组，存储可用来做动画处理
		//public void ReplaceGlidingResults(bool isUp, int length)
		//{
		//	if(isUp)
		//	{
  //              //向上的是走前面的数据，移除后面的
  //              for (int i = 0; i < length; i++)
  //              {
  //                  SymbolResults.RemoveAt(SymbolResults.Count - 1);
  //                  resultFirstAtReel = (resultFirstAtReel - 1 + reelData.Count) % reelData.Count;
  //                  int v = reelData[resultFirstAtReel].value;
  //                  SymbolResults.Insert(0, v);
  //              }
  //          }
		//	else{
  //              //向下的是走后面的数据
  //              for (int i = 0; i < length; i++)
  //              {
  //                  SymbolResults.RemoveAt(0);
  //                  resultLastAtReel = (resultLastAtReel + 1) % reelData.Count;
  //                  int v = reelData[resultLastAtReel].value;
  //                  SymbolResults.Add(v);
  //              }
  //          }
		//}
	}

	public class WeightData
	{
		public int value; //symbol的索引Index，有的引用地方只修改了此值而忽略了symbolName
		public int weight;
		public string SymbolName;

		public WeightData (int value, int weight = 1, string _SymbolName = "")
		{
			this.value = value;
			this.weight = weight;
			this.SymbolName = _SymbolName;
		}
	}

	#if UNITY_EDITOR

	/// <summary>
	/// 返回结果值STR字符串
	/// </summary>
	/// <returns>The reel content.</returns>
	public string DebugReelContent (List<List<int>> resultList, SymbolMap symbolMap, string beginStr)
	{
		if (resultList == null || symbolMap == null) {
			return null;
		}

		System.Text.StringBuilder testInfo = new System.Text.StringBuilder ();

		if (beginStr != null) {
			testInfo.Append (beginStr);
		}

		for (int i = 0; i < resultList.Count; i++) {
			for (int j = 0; j < resultList [i].Count; j++) {
				SymbolMap.SymbolElementInfo info = symbolMap.getSymbolInfo (resultList [i] [j]);
				testInfo.Append (info.name + " ");
			}
			testInfo.Append ("\n");
		}

		return testInfo.ToString ();
	}

	/// <summary>
	/// Type参数:0对应从服务器获取 1对应正常结果 2对应nowin
	/// </summary>
	public string DebugReelContent (List<List<int>> resultList, SymbolMap symbolMap, int type)
	{
		string str = null;
		switch (type) {
		case 0:
			str = DebugReelContent (resultList, symbolMap, "Result:接收到服务器信息\n");
			break;
		case 1:
			str = DebugReelContent (resultList, symbolMap, "Normal Result:\n");
			break;
		case 2:
			str = DebugReelContent (resultList, symbolMap, "NoWin Result:\n");
			break;
		case 3:
			str = DebugReelContent (resultList, symbolMap, "前20次 Result:\n");
			break;
		default:
			break;
		}

		return str;
	}


	/// <summary>
	/// Type参数:0对应从服务器获取 1对应正常结果 2对应nowin 3对应前20次结果
	/// </summary>
	public string DebugReelContent (ResultContent resultContent, SymbolMap symbolMap, int type)
	{
		List<List<int>> result = new List<List<int>> ();

		for (int i = 0; i < resultContent.ReelResults.Count; i++) {
			List<int> tempList = new List<int> ();
			for (int j = 0; j < resultContent.ReelResults [i].SymbolResults.Count; j++) {
				tempList.Add (resultContent.ReelResults [i].SymbolResults [j]);
			}
			result.Add (tempList);
		}

		return DebugReelContent (result, symbolMap, type);
	}


	#endif
}
