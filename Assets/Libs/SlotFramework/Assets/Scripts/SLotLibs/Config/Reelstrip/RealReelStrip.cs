using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Classic
{
    public class RealReelStrip
    {
		private readonly static string DEFAULT_RATIO_NAME= "DefaultR";
		private readonly static string DEFAULT_A_NAME= "DefaultA";
		private readonly static string CONDITION_NODE = "Conditions";
		private readonly static string RATIO_NODE= "R";
		private readonly static string ATOM_DATA_NODE= "A";
		private readonly static string FIXED_RESULTS = "WeightedResultData";

		public List<ReelStripFilterCondition> conditions;
		public Dictionary<string,ReelStripDataRatio> ratiosList;
		public Dictionary<string,AtomData> atomStripDataList;
		public WeightedResults fixedResults;
		public DataFilterResult selectedResult;
		public string defaultRName;
		public string defaultAName;
		// for autopilot  provide the API
		private string currentRName;
		private string currentAName;
		private string currentCName;

		private VirtualReelStrip virtualReelStrip;

		#region 数据解析
		public RealReelStrip(SymbolMap symbolMap,Dictionary<string,object> info){
			if (info==null) {
				return;
			}
			if (info.ContainsKey(DEFAULT_RATIO_NAME)) {
				defaultRName = info [DEFAULT_RATIO_NAME].ToString ();
				currentRName = defaultRName;
			}
			if (info.ContainsKey(DEFAULT_A_NAME)) {
				defaultAName = info [DEFAULT_A_NAME].ToString ();
				currentAName = defaultAName;
			}
			if (info.ContainsKey(CONDITION_NODE)) {
				conditions = new List<ReelStripFilterCondition> ();
				List<object> cons = info[CONDITION_NODE] as List<object>;
				for (int i = 0; i < cons.Count; i++) {
					Dictionary<string,object> consInfo = cons [i] as Dictionary<string,object>;
					ReelStripFilterCondition bsfc = ConditionFactory.CreateCondition( consInfo [BaseCondition.CONDITION_TYPE_NODE].ToString (),consInfo) as ReelStripFilterCondition;
					if (bsfc!=null) {
						conditions.Add(bsfc);
					}
				}
			}
			if (info.ContainsKey(RATIO_NODE)) {
				ratiosList = new Dictionary<string, ReelStripDataRatio>();
				Dictionary<string,object> res= info[RATIO_NODE] as Dictionary<string,object>;
				foreach (string key in res.Keys) {
					ReelStripDataRatio rsdr = null;
					Dictionary<string,object> r = res [key] as Dictionary<string,object>;
					rsdr = new RandomReelStripDataRatio(r as Dictionary<string,object>);
					ratiosList.Add (key,rsdr);
				}
			}
			if (info.ContainsKey(ATOM_DATA_NODE)) {
				atomStripDataList = new Dictionary<string, AtomData> ();
				Dictionary<string,object> aList = info [ATOM_DATA_NODE] as Dictionary<string,object>;
				foreach (string key in aList.Keys) {
					
					Dictionary<string,object> aDInfo = aList[key] as Dictionary<string,object>;
					string type = aDInfo[AtomData.REEL_STRIP_ATOM_DATA_A_TYPE].ToString();
					AtomData ad = AtomDataFactory.CreateAtomData (type, symbolMap, aDInfo);
					if (ad!=null) {
						atomStripDataList.Add (key,ad);
					}
				}
			}
			if (info.ContainsKey(FIXED_RESULTS)) {
				Dictionary<string,object> frs = info [FIXED_RESULTS] as Dictionary<string,object>;
				fixedResults = new WeightedResults (symbolMap, frs);
			}

			
		}
		
		#endregion
		
		//检查条件，筛选出A的比率数据	
		private void CheckConditions(){
			if (!SequencePaidResultsManager.CheckCondition()) {
				if (CheckRewardR()) return;
				if (conditions != null && conditions.Count > 0) {
					for (int i = 0; i < conditions.Count; i++) {
						ReelStripFilterCondition.ConditionResult conditionResult = conditions [i].GetQualifiedConditionResult ();
						if (conditionResult.meetTheCondition) {
							currentCName = "idx:" + i.ToString ();
							SetResultData (conditionResult.dependencyRName);
							return;
						}
					}
				}
			}
			currentCName = "None";
			SetResultData (defaultRName);
		}

		private bool CheckRewardR()
		{
			if(TestController.Instance != null ) return false;
			if (BaseSlotMachineController.Instance.isFreeRun) return false;

			BaseCommandItem baseCommand = CommandManager.Instance.GetCommandByFeature(GameConstants.RemoteR_Key);
			if (baseCommand == null) return false;

			if (!baseCommand.IsHaveTriggerCount())
			{
				baseCommand.OnAccept();
				return false;
			}

			if (!baseCommand.CheckCommandTriggerConditionsIsOk()) return false;

			string RName = baseCommand.GetServerDataFromCommandByKey<string>(RATIO_NODE,string.Empty);
			if (string.IsNullOrEmpty(RName)) return false;
			if(!ratiosList.ContainsKey(RName)) return false;

			SetResultData(RName);
			baseCommand.DoCommandOperation();

			BaseSlotMachineController.Instance.mIsUseRemoteR = true;
			return true;
		}

		private void SetResultData(string RName,bool isInit = false){
			if (!ratiosList.ContainsKey(RName))
			{
				return;
			}
			ReelStripDataRatio.DataRatioResult ratioResult = ratiosList [RName].GetRatioResult ();
			string aKey = ratioResult.selectedResultName;

			if (selectedResult!=null&&selectedResult.selectedFixedResult!=null) {
				selectedResult.selectedFixedResult=null;
			}
			if (atomStripDataList.ContainsKey(aKey)) {
				selectedResult = atomStripDataList [aKey].GetResultData (fixedResults);
			}

            //注释掉以前关于服务器接收结果的方法
            //服务器影响结果改到resultContent中
		

//			if (!isInit) {
//				List<List<int>> remoteRA = SequencePaidResultsManager.GetSymbolResults ();
//				if (remoteRA != null) {

//#if UNITY_EDITOR
//					StringBuilder str = new StringBuilder ("remoteRA:\n");
//					for (int i = 0; i < remoteRA.Count; i++) {
//						for (int j = 0; j < remoteRA [i].Count; j++) {
//							str.Append (remoteRA [i] [j] + " ");
//						}
//						str.Append ("\n");
//					}
//					Debug.Log (str);
//#endif
			//		if (selectedResult != null) {
			//			selectedResult.selectedFixedResult = remoteRA;
			//		}

			//	}
			//}

			if (selectedResult.selectedReelStrips==null) {
				selectedResult.selectedReelStrips = GetDefaultReelStrips();
			}
			currentRName = RName;
			currentAName = aKey;

			#if DEBUG||UNITY_EDITOR
			Messenger.Broadcast<string,string,string> (DebugTestRAPanel.RADATA_CHANGE,currentCName,currentRName,currentAName);
			#endif
		}

		public string GetCurrentUseRName(){
			return currentRName;
		}
		public string GetCurrentUseAName(){
			return currentAName;
		}
		public string GetCurrentUseCName(){
			return currentCName;
		}
		public bool CanMeetTheCondition(List<ReelStrip> currentStripList){
			CheckConditions ();
			return currentStripList.Equals (selectedResult.selectedReelStrips);
		}

		public List<ReelStrip> GetSelectedStrips(){
			if (selectedResult!=null&&selectedResult.selectedReelStrips!=null) {
				return selectedResult.selectedReelStrips;
			}
			return GetInitStrips ();
		}

		public void InitStrips()
		{
			SetResultData (defaultRName,true);
			currentCName = "None";
		}
		public List<ReelStrip> GetInitStrips(){
			return selectedResult.selectedReelStrips;
		}
		private List<ReelStrip> GetDefaultReelStrips(){
			return atomStripDataList[defaultAName].GetResultData(fixedResults).selectedReelStrips;
		}
		public void ResetRAData(string r,string a,List<List<int>> fixedResult){
			if (selectedResult==null) {
				selectedResult = new DataFilterResult ();
			}
			currentRName = r;
			currentAName = a;
			ReelStripAtomData ad = null;
			if (atomStripDataList.ContainsKey(a)&&atomStripDataList[a] as ReelStripAtomData!=null) {
				ad = atomStripDataList [a] as ReelStripAtomData;
			} else {
				if (atomStripDataList.ContainsKey(a)) {
					selectedResult.selectedFixedResult =fixedResult;
				}
				ad = atomStripDataList [defaultAName] as ReelStripAtomData;
			}

			selectedResult.selectedReelStrips = ad.GetResultData (fixedResults).selectedReelStrips;
			#if DEBUG||UNITY_EDITOR
			Messenger.Broadcast<string,string,string> (DebugTestRAPanel.RADATA_CHANGE,currentCName,currentRName,currentAName);
			#endif
		}
	}
}
