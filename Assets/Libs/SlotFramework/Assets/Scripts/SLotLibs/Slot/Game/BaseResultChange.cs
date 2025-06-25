using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Utils;
using System;
namespace Classic
{
    public class BaseResultChange :MonoBehaviour, IResultChangeController
    {
		[Serializable]
		public class Result{
			public List<Reel> result=new List<Reel>();
		}
		[Serializable]
		public class Reel
		{
			public List<string> reel =  new List<string>();
		}

		[SerializeField]
		public List<Result> commonTestResultsList = new List<Result>();

		#if UA_ADS_RECORD_VIDEO
		private int curSpinCount=0;
		private List<List<List<int>>> uaResultsList = new List<List<List<int>>> ();
		void Awake(){
			Messenger.AddListener (GameConstants.OnSlotMachineSceneInit, InitUAResultData);
		}

		void OnDestroy(){
			Messenger.RemoveListener (GameConstants.OnSlotMachineSceneInit, InitUAResultData);
		}

		void InitUAResultData(){
			uaResultsList.Clear ();
			if (BaseSlotMachineController.Instance==null) {
				return;
			}
			ReelManager reelManager = BaseSlotMachineController.Instance.reelManager;
			for (int i = 0; i < commonTestResultsList.Count; i++) {
				List<List<int>> result = new List<List<int>> ();
				Result res = commonTestResultsList [i];
				for (int j = 0; j < res.result.Count; j++) {
					List<int> reel = new List<int> ();
					for (int k = 0; k < res.result[j].reel.Count; k++) {
						int symbolIndex = reelManager.symbolMap.getSymbolIndex(res.result[j].reel[k]);
						reel.Add (symbolIndex);
					}
					result.Add (reel);
				}
				uaResultsList.Add (result);
			}

		}
		public List<List<int>> GetUAVideoResult(){
			int resIdx = curSpinCount++ % uaResultsList.Count;
			return uaResultsList[resIdx];
		}
		public void SetUAUsedResults(ReelManager reelManager){
			if (commonTestResultsList.Count>0) {
				List<List<int>> res = GetUAVideoResult ();
				reelManager.resultContent.ChangeResult (res);
			}
		}
		#endif

        private const string NO_MONEY_ALREADY_APPEAR_TIMES = "NO_MONEY_ALREADY_APPEAR_TIMES";
        private const string BASE_RESULT_CHANGE_SPIN_TIMES = "BASE_RESULT_CHANGE_SPIN_TIMES";

		private const string TIME_PLAY_ALREADY = "TimeAlreadyPlay";
		[SerializeField]
		public bool DisableJackPot = false;
		[SerializeField]
		public bool ReadTestDataFromPlist;
		[SerializeField]
		public bool mTestOn;
		[SerializeField]
		public bool mFreespinTestOn;


        public virtual List<List<int>> GetTestResult (GameConfigs gameConfigs)
        {
			if (ReadTestDataFromPlist)
			{
				if (BaseSlotMachineController.Instance != null) {
					return BaseSlotMachineController.Instance.reelManager.machineTestDataFromPlist.GetSelectedCommonTestResultData(spinCount);
				}
			}
			// else if (BaseSlotMachineController.Instance != null) {
			// 	return BaseSlotMachineController.Instance.reelManager.machineTestDataFromPlist.GetSelectedCommonTestResultData(DebugMachineDataTestPanel.currentCommonResultName);
			// }
            return null;
        }
        
        
        

        public virtual List<List<int>> SpecialReuslt (GameConfigs gameConfigs){

			#if UNITY_EDITOR
			if (ReadTestDataFromPlist)
			{
				if (BaseSlotMachineController.Instance != null) {
					return BaseSlotMachineController.Instance.reelManager.machineTestDataFromPlist.GetSelectedSpecialTestResultData(DebugMachineDataTestPanel.specialResultName);
				}
			}
			#elif  DEBUG||UNITY_ANDROID||UNITY_IOS
			if (BaseSlotMachineController.Instance != null) {
			return BaseSlotMachineController.Instance.reelManager.machineTestDataFromPlist.GetSelectedSpecialTestResultData(DebugMachineDataTestPanel.specialResultName);
			}
			#endif
            return null;
        }

        public bool isTestOn {
            get {
				// #if UNITY_EDITOR || DEBUG
				// 	return mTestOn;
				// #endif
                return mTestOn;
            }
            set {
                mTestOn = value;
            }
        }
        public bool isFreespinTestOn {
            get {
                return mFreespinTestOn;
            }
            set {
                mFreespinTestOn = value;
            }
        }

        private int spinCount { 
	        get {
		        return SharedPlayerPrefs.GetPlayerPrefsIntValue(BASE_RESULT_CHANGE_SPIN_TIMES,0);
	        }
	        set {
		        SharedPlayerPrefs.SetPlayerPrefsIntValue(BASE_RESULT_CHANGE_SPIN_TIMES,value);
	        }
		}
        
        private bool isInit = false;
        private List<BigwinLimitAward> BigwinLimitAwards = new List<BigwinLimitAward>();

        
        public virtual  bool HasSpecialResult(){
            if (!isInit) {
                isInit = true;
            }

            if (BaseSlotMachineController.Instance == null) {
                return false;
            }

            //          SlotMachineConfig config = BaseSlotMachineController.Instance.slotMachineConfig;
            //          if (UserManager.GetInstance().UserProfile().Level() <=  requireLevel && BaseSlotMachineController.Instance.currentBetting <= RequireMaxBet)
            //              return true;

            return true;
        }
        
		private SlotMachineConfig CurrentSlotMachineConfig
        {
            get{
                return BaseSlotMachineController.Instance.slotMachineConfig;
            }
        }

        private int NoMoneyAwardAppearTimesLimit
        {
            get{
                return SharedPlayerPrefs.GetPlayerPrefsIntValue (NO_MONEY_ALREADY_APPEAR_TIMES, 0);
            }
            set{
                SharedPlayerPrefs.SetPlayerPrefsIntValue (NO_MONEY_ALREADY_APPEAR_TIMES, value);
                SharedPlayerPrefs.SavePlayerPreference ();
            }
        }


        public void ChangeResult(ReelManager baseGamePanel){
			
			if (BaseSlotMachineController.Instance == null) {
                return;
            }

			#if UNITY_EDITOR||DEBUG
			if (DisableJackPot) {
				DebugMachineDataTestPanel.currentCommonResultName = "NONE";
			}
			#endif

            if (!isFreespinTestOn&& BaseSlotMachineController.Instance.isFreeRun) {
                return;
            }
			if (isTestOn&&DebugMachineDataTestPanel.currentCommonResultName!=DebugMachineDataTestPanel.JACKPOTNAME) {
				baseGamePanel.resultContent.ChangeResult (GetTestResult (baseGamePanel.gameConfigs));
				spinCount++;
				return;
			}
			if (!HasSpecialResult()) {
				return;
			}

			spinCount++;
        }
    }

    class BigwinLimitAward
    {
        private  const string SPECIAL_LIMIT_LOCAL ="SpecialLimitSpinNumber_";
        private int Index;
        public int SpinTime;
        public bool HasReceivedAward
        {
            get {
                return SharedPlayerPrefs.GetPlayerBoolValue (SPECIAL_LIMIT_LOCAL+ SwapSceneManager.Instance.GetLogicSceneName()+ Index, false);
            }
            set{
                SharedPlayerPrefs.SetPlayerPrefsBoolValue (SPECIAL_LIMIT_LOCAL+SwapSceneManager.Instance.GetLogicSceneName() + Index, true);
                SharedPlayerPrefs.SavePlayerPreference ();
            }
        }

        public BigwinLimitAward(int beforeSpin,int afterSpin,int index)
        {
            this.Index = index;
            this.SpinTime = UnityEngine.Random.Range (beforeSpin,afterSpin);
        }
    }
}
