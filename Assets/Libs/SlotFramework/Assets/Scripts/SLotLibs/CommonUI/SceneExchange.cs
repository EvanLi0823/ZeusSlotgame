using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

using Classic;


namespace Libs
{
	public class SceneExchange : UIDialog
	{
		public SceneFade sceneFade = null;
		private SlotMachineConfig config;
		private WaitForSeconds waitTime = new WaitForSeconds (1/60);
        private Image m_SceneImage;
        private GameObject landscapePanel;
        private GameObject portraitPanel;
        private bool isRewardSpin;
       
		protected override void Awake ()
		{
			base.Awake ();
			this.bResponseBackButton = false;
            portraitPanel = Util.FindObject<GameObject>(transform, "Anchor_Portrait/");
            landscapePanel = Util.FindObject<GameObject>(transform, "Anchor_Landscape/");
            portraitPanel.SetActive(false);
            landscapePanel.SetActive(true);
            this.m_SceneImage = Util.FindObject<UnityEngine.UI.Image>(transform, GameConstants.LOADING_BASE_PATH + "LogoImage/"); 
            this.IsBackGroundStop = false;
            Time.timeScale = 1f;
        }

        public override void SetData(object data, bool needRefresh = false)
        {
	        isRewardSpin = false;
            base.SetData(data,needRefresh);
            if (m_Data is SlotMachineConfig)
            {
	            this.config = m_Data as SlotMachineConfig;
	            if (config == null) return;
	            isRewardSpin = config.isRewardSpin;
                if (this.config.IsPortrait)
                {
                    this.m_SceneImage = Util.FindObject<UnityEngine.UI.Image>(transform, "Anchor_Portrait/Panel/LogoImage/");
                    sceneFade = Util.FindObject<SceneFade>(transform, "Anchor_Portrait/");
                    sceneFade.gameObject.SetActive(true);
                    landscapePanel.SetActive(false);
                }

                if (!this.config.IsPortrait)
                { 
	                GameObject logo = Util.FindObject<GameObject>(transform, "Anchor_Landscape/Panel/Logo/");
	                if(logo!=null) logo.SetActive(false);
                }
            }
        }

        public override void Refresh ()
		{
			base.Refresh ();
            this.LoadScene (this.m_Data);
		}

		public void LoadScene (object o, float minDuration = 0)
		{
			//切换场景保存数据
			PlayerPrefs.Save();

			AsyncLogger.Instance.StartTraceLog("SceneExchange:LoadScene");
//			ExecuteTime.StartRecord();
			string sceneName;
			if (o is SlotMachineConfig) {
				this.config = o as SlotMachineConfig;
				sceneName = config.Name ();
				
			} else {
				sceneName = o.ToString ();
				this.config = null;
			}
			
			if (sceneFade == null) {
				sceneFade = Util.FindObject<SceneFade>(transform, "Anchor_Landscape/");
                sceneFade.gameObject.SetActive(true);
                portraitPanel.gameObject.SetActive(false);
            }
			sceneFade.FadeSceneToBlack (sceneName);

			float now = Time.realtimeSinceStartup;
			
			Action<string,float> loadingDelegate = delegate(string levelName, float progress) {
				LevelLoading (sceneName, progress);
			};
			
			Action<string, AsyncOperation> loadWillSuccessDelegate = delegate(string levelName, AsyncOperation asyncOp)
			{
				Action callBack = ()=>
				{
					if (sceneFade != null) 
					{
						sceneFade.OnCompletedLoad ();
					}
				};
				new DelayAction(.5f,null, delegate { callBack(); }).Play();
			};
			Action<string, AsyncOperation> loadSucceededDelegate = delegate(string levelName, AsyncOperation asyncOp)
			{
				asyncOp.allowSceneActivation = true;
			};
			
			Action<string,string> loadFailedDelegate = delegate(string levelName, string message) {

			};
			
			StartCoroutine (AsyncLoadLevel (sceneName, false,
				loadingDelegate,
				loadSucceededDelegate,
				loadFailedDelegate,
				loadWillSuccessDelegate
			));

		}

		public  void LevelLoading (string levelName, float progress)
		{
			if (sceneFade != null) {
				sceneFade.OnLoading (levelName, progress);
			}
		}

        private bool loadBundleError = false;
        private string loadBundleErrorMsg = "";
        private string bundleLevelPath = "";
        private string bundleResourcePath = "";
        private bool timeOut = true;
        readonly int maxRepeateTime = 2;
        private float timeStep = 1 / 60f;
		int step = 0;
		float totalStep = 0;
		AssetBundle sceneBundle = null;
		AssetBundle resourceBundle = null;
        private float maxSceneLoadProgress = 0f;
        private float maxResoureLoadProgress = 0f;
		private bool BLoadDefaultResource = true;
        private float curLoadTime = 0f;
        private bool isRequestSeccess=false;
        protected virtual IEnumerator AsyncLoadLevel (string slotMachineLevelName,
		                                              bool autoActivateNewLevel = false,
		                                              Action<string,float> loadingCallback = null,
		                                              Action<string, AsyncOperation> loadSucceededCallback = null,
		                                              Action<string, string> loadFailedCallback = null,
		                                              Action<string, AsyncOperation> loadWillSuccessCallback = null)
		{		
 
			AsyncLogger.Instance.StartTraceLog ("SceneExchange:AsyncLoadLevel:"+slotMachineLevelName);
			totalStep = 2;
			step = 0;
			sceneBundle = null;
			resourceBundle = null;
            curLoadTime = 0f;
            bundleLevelPath = "";
            bundleResourcePath = "";
            loadBundleError = false;
            loadBundleErrorMsg = "";
			bool isRequestOver=false;
			AsyncOperation ao = SwapSceneManager.Instance.LoadSceneAsync ("EmptyScene");
			while (!ao.isDone) {
				yield return null;
//				LevelLoading(slotMachineLevelName,Mathf.Lerp(0.1f,1f/totalStep,ao.progress));//Second Load EmptyScene Process  1/totalStep
			}
			LevelLoading(slotMachineLevelName,0.1f);//Init Process
            bool enterSlotMachineCB = false;
            float startTime = Time.realtimeSinceStartup;
            if (this.config != null)
            {
                totalStep += 1;
                enterSlotMachineCB = true;
            }
			yield return null; 
			step++;

            if (this.config != null)
            {
                float start = (float)step / totalStep;
                float end = start + 1f / totalStep;
                while (!enterSlotMachineCB)
                {
                    yield return null;
                    float process = Mathf.Min((Time.realtimeSinceStartup - startTime) / 10f,1f);
                    LevelLoading(slotMachineLevelName, Mathf.Lerp(start,end, process));
                    if (Time.realtimeSinceStartup - startTime > 10) enterSlotMachineCB = true;
                }
                step++;
            }
           
           
            if (this.config != null)
            {
	            bool skip = false;
				try {
					this.config.ParseDict ();
					if (this.config == null)
					{
						skip = true;
						throw new Exception("this config parse Dict,lead to config is null");
					}
				} 
				catch (Exception ex) {
					skip = true;
					slotMachineLevelName=HandleExceptionWhenParsePlist (slotMachineLevelName,ex.Message);
				}

                if(!skip &&this.config.UseSpine)
                {
                    this.config.ClearSpineData();
                    yield return StartCoroutine(this.config.InitSpineAsset());
                }
			}

            AsyncOperation asyncOp = null;
            bool existException = false;
            string levelName = string.Empty;
			try
			{
				string assetName = slotMachineLevelName;
				if (config != null && config.Name() == assetName)
				{
					assetName = config.AssetName;
				}

				asyncOp = SwapSceneManager.Instance.LoadSceneAsync (assetName,slotMachineLevelName);
				asyncOp.allowSceneActivation = autoActivateNewLevel;
			} catch (Exception ex) {
				
			}	
			
			//0.9代表成功了，详情：https://docs.unity3d.com/ScriptReference/AsyncOperation-allowSceneActivation.html
			while ( asyncOp.progress < 0.9f) {
				if (loadingCallback != null) {
					
					loadingCallback (slotMachineLevelName, asyncOp.progress / totalStep + (float)step / totalStep);
				}
				yield return waitTime;
			}

			if (loadingCallback != null) {
				loadingCallback (slotMachineLevelName, 1f);
			}


			while (this.config != null && !this.config.IsDataInit) {
				yield return waitTime;
			}

            if (this.config!=null) this.config.IsDataInit = false;
			if (this.config!=null) Messenger.Broadcast (SlotControllerConstants.HIGH_ROLLER_CHECK_KEY);

			
			if (loadWillSuccessCallback != null) {
				loadWillSuccessCallback (slotMachineLevelName, asyncOp);
			}
			
			if (loadSucceededCallback != null) {
				loadSucceededCallback (slotMachineLevelName, asyncOp);
			}
			AsyncLogger.Instance.EndTraceLog ("SceneExchange:AsyncLoadLevel:"+slotMachineLevelName);
		}
        
		string HandleExceptionWhenParsePlist(string machineName,string errorMessege){
			
			this.CompleteQuitCallBack += ()=>{
				SwapSceneManager.Instance.ResetState();
				#region POC
				UIManager.Instance.CloseAll(true);
				#endregion
			};
			string filePath = AssetsPathManager.GetMachineLocaRemotelPlistPath (machineName);
            FileUtils.DeleteFile(filePath);
			Dictionary<string,object> dict = new Dictionary<string, object> ();
			dict.Add ("slotName",machineName);
			dict.Add ("reason","Plist Parse Error:"+errorMessege);
			BaseGameConsole.ActiveGameConsole ().LogBaseEvent (Analytics.SCENE_EXCHANGE_EXCEPTION,dict);
			UIManager.Instance.CloseAll(true);
			machineName = GameConstants.LOBBY_NAME;
			
			this.config = null;
			return machineName;
		}
		// private string HandleExceptionWhenLoadingLevel(string machineName,string errorMessege){
		// 	if (config!=null) {
		// 	    FileUtils.DeleteFile(bundleLevelPath);
  //               FileUtils.DeleteFile(bundleResourcePath);
		// 	}
		// 	
  //
		// 	string levelName = GameConstants.LOBBY_NAME;
		// 	this.config = null;
		// 	this.CompleteQuitCallBack += ()=>{
		// 		SwapSceneManager.Instance.ResetState();
		// 		#region POC
		// 		UIManager.Instance.CloseAll(true);
		// 		#endregion
		// 	};
		// 	SlotsAssets.Instance.ClearAllCard ();
  //
		// 	Dictionary<string,object> dict = new Dictionary<string, object> ();
		// 	dict.Add ("slotName",machineName);
		// 	dict.Add ("reason","Loading Level Error:"+errorMessege);
		// 	BaseGameConsole.ActiveGameConsole().LogBaseEvent (Analytics.SCENE_EXCHANGE_EXCEPTION,dict);
		// 	UIManager.Instance.CloseAll(true);
		// 	
		// 	return levelName;
		// }
		
        void SendTimeOutEvent(string machineName,string errorMessege)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("slotName", machineName);
            dict.Add("reason", "Load Bundle Error:" + errorMessege);
            BaseGameConsole.ActiveGameConsole().LogBaseEvent(Analytics.SCENE_EXCHANGE_EXCEPTION, dict);
            UIManager.Instance.CloseAll(true);

        }

        // IEnumerator HandleRequestError(string error, string machineName)
        // {
	       //  this.CompleteQuitCallBack += () =>
	       //  {
		      //   SwapSceneManager.Instance.ResetState();
		      //   #region POC
		      //   UIManager.Instance.CloseAll(true);
		      //   #endregion
	       //  };
	       //  yield return BacktoLobby();
	       //  yield return new WaitForSecondsRealtime(2.0f);
	       //  AlertDialog.ShowOnMachineDataError(null,null,null);
	       //  UIManager.Instance.CloseAll(true);
	       //  if (sceneFade!=null)
	       //  {
		      //   sceneFade.OnCompletedLoad(); 
	       //  }
	       //  //es事件
	       //  Log.Error("InitRequestError:" + error);
	       //  SendInitErrorEvent(machineName, error);
        // }
        //
        // private static AsyncOperation BacktoLobby()
        // {
	       //  SlotsAssets.Instance.ClearAllCard();
        //
	       //  string sceneName = GameConstants.LOBBY_NAME;
	       //
	       //  AsyncOperation asyncOperation = SwapSceneManager.Instance.LoadSceneAsync(sceneName);
	       //  //LobbyCardsManager.Instance.Init();
	       //  return asyncOperation;
        // }

   //      void SendInitErrorEvent(string machineName, string errorMessege)
   //      {
	  //       Dictionary<string, object> dict = new Dictionary<string, object>();
	  //       dict.Add("slotName", machineName);
	  //       dict.Add("ErrorType", "EnterMachine");
	  //       dict.Add("reason", "InitRequestError:" + errorMessege);
			// BaseGameConsole.ActiveGameConsole().LogBaseEvent(Analytics.SERVER_SPIN_ERROR, dict);
   //      }
	}
	 
}
