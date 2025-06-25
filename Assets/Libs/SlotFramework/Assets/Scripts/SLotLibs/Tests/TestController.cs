using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Classic;
using System;
using Core;
using System.IO;
using System.Text;
using Libs;
using System.Threading.Tasks;
using Utils;

namespace Classic
{
    public class TestController : MonoBehaviour
    {
		public string gameName = "ClassicSlotsTest";
		public BaseGameConsole testGameConsole;
        public Dropdown SlotDropdown;
        public InputField repeatTime;
        public InputField initBalance;
        public InputField level;
        public InputField betMoney;
        public InputField outputDir;
        public Text leftTime;
        public Text leftBalance;
        public Text buttonText;
        public Toggle toggle ;
		public SlotMachineConfig classicMachineConfig;
        public long currentBetting;
		public double leftBalanceInt;
        int repeatTimeInt;
        float initBalanceInt;
        private float Coins;
        bool running = false;
        Dictionary<string,PrintAwardInfo> awards = new Dictionary<string, PrintAwardInfo> ();
        Dictionary<string,PrintAwardInfo> freespin = new Dictionary<string, PrintAwardInfo> ();
        //onceMore时奖项统计
        Dictionary<string, PrintAwardInfo> onceMoreAwards = new Dictionary<string, PrintAwardInfo>();
		#region Statistics RTP
		public StatisticsManager statisticsManager = new StatisticsManager();
		#endregion
        string slotName;
        string savePath;
        bool showDetail;
        public ReelManager reelManager;
        ExtraAwardGame extraAward;
        FreespinGame freespinGame;
        BonusGame bonusGame;
        public bool isOneMore = false;
        public static TestController Instance;

        public double oneTimeAward = 0;

		public int triggerFreespinTimes;
        public int reTriggerFreespinTimes;

        //统计onceMore进行了多少次
        int onceMoreTime=0;
        int FreespinTime;

        //触发link的次数
        int TriggerLinkCount = 0;

		public int OnceMoreTriggerCount = 0;

        void Awake ()
        {
            Application.runInBackground = true;
            Application.targetFrameRate = 600;
			outputDir.text = Path.Combine (System.Environment.GetFolderPath (System.Environment.SpecialFolder.Desktop), gameName);
            Instance = this;
            Messenger.AddListener (GameConstants.OnGameConsoleStarted, OnGameConsoleStarted);
			BaseGameConsole.ActiveGameConsole ().isForTestScene = true;
        }

        void OnGameConsoleStarted ()
        {
			List<SlotMachineConfig> lists =new List<SlotMachineConfig>( BaseGameConsole.singletonInstance.SlotMachines ());
			//lists.Reverse ();
			foreach (SlotMachineConfig slotCoinfig in lists) {
                Dropdown.OptionData optionData = new Dropdown.OptionData ();
                optionData.text = slotCoinfig.Name ();
                SlotDropdown.options.Add (optionData);
            }
//			SlotDropdown.value = 0;
			SlotDropdown.captionText.text = lists [0].Name ();
        }

        void OnDestory ()
        {
            Messenger.RemoveListener (GameConstants.OnGameConsoleStarted, OnGameConsoleStarted);
        }

        public void OnClick ()
        {
            if (running) {
                running = false;
                buttonText.text = "Start Test";
            } else {
                running = true;
                buttonText.text = "Stop";
                slotName = SlotDropdown.captionText.text;
			
				classicMachineConfig = BaseGameConsole.singletonInstance.SlotMachineConfig (slotName) as SlotMachineConfig;
				classicMachineConfig.ParseDict();
                findReelConfig (slotName);
                reelManager.InitReels (classicMachineConfig);
                extraAward = (ExtraAwardGame)reelManager.extraAward;
                freespinGame = reelManager.FreespinGame;
                bonusGame = reelManager.BonusGame;
			
                repeatTimeInt = int.Parse (repeatTime.text);
                initBalanceInt = Utilities.CastValueLong(initBalance.text);
                currentBetting = Utilities.CastValueLong(betMoney.text);
                savePath = outputDir.text;
                showDetail = toggle.isOn;
                leftTime.text = "剩余重复次数：" + (repeatTimeInt - 1);
				if (extraAward != null) {
					extraAward.InitForTest (classicMachineConfig.extroInfos.infos, currentBetting);
				}
				 triggerFreespinTimes =0;
				 reTriggerFreespinTimes =0;
				OnceMoreTriggerCount = 0;
                onceMoreTime = 0;
                TriggerLinkCount = 0;
                Coins = reelManager.gameConfigs.ScalFactor;
                
                StartCoroutine (Spin ());
            }
        }

        int totalSpinTime;
        int totalSpinInFreeSpin;
        int totalAwardTime;
        int winLBerPerLine;
        private StringBuilder DetailsOutputData = new StringBuilder();
        
//        string TestData = "";
        string TestResult = "";
        string TestDataFileName = "";
        string TestResultFileName = "";

        public void Write (string text, string fileName)
        {
            FileStream fs = new FileStream (fileName, FileMode.Create);
            StreamWriter sw = new StreamWriter (fs, Encoding.UTF8);
            sw.Write (text);
            sw.Close ();
            fs.Close ();
        }

        private void OnRollStart ()
        {
            reelManager.resultContent.awardResult.awardValue = 0;

			if (!reelManager.isFreespinBonus && (!isOneMore)) {
				leftBalanceInt -= currentBetting;
				totalSpinTime++;
				#region Statistics RTP
				statisticsManager.CostCoinsNum += currentBetting;
				statisticsManager.SpinNum++;
				#endregion
			}

            PreStartState();
            CreateResult ();

			if (extraAward != null) {
				extraAward.OnSpinForTest (currentBetting);
			}

            if (reelManager.isFreespinBonus && freespinGame != null) {
                freespinGame.OnSpinForTest ();
				if (!isOneMore) {
					totalSpinInFreeSpin++;
				}
            }
        }

        private string OnRollEnd ()
        {
            return GetResult ();
        }

        private void PreStartState() 
        {
            reelManager.PreStartState();
        }

        private void CreateResult ()
        {
//            if (reelManager.UseNewSpinResult)
//            {
//                if (reelManager.SpinData == null)
//                {
//                    reelManager.SpinData = reelManager.CreateSpinResult();
//                }
//                reelManager.SpinData.CreateResult();
//            }
//            else
//            {
			    reelManager.CreateRawResult ();
//            }
            
        }

        private string GetResult ()
        {

            double startSpinCoins = leftBalanceInt;

            string freeSpinInfo = CheckResult ();

            string dataResult = GetDetailResult();

			
            string winResult = startSpinCoins + Blank(startSpinCoins.ToString()) + oneTimeAward + Blank(oneTimeAward.ToString()) + leftBalanceInt;

            string bonusResult = "";

            if (hitBonus)
            {
                if (reelManager.IsNewProcess)
                {
                    bonusResult = "\t\t\t(bonus:)" + reelManager.GetBonusAward() + "\t" + bonusGame.GetTestLog();
                }
                else
                {
                    bonusResult = "\t\t\t(bonus:)" + bonusGame.AwardInfo.awardValue + "\t" + bonusGame.GetTestLog();
                }
            }

            return dataResult + "      " + winResult + "     " + freeSpinInfo + "     " + bonusResult;

        }

        private string Blank(string str)
        {
            string blank = "";
            for (int i = 0; i < 11 - str.Length; i++) blank += " ";
            return blank;
        }

        private string GetDetailResult()
        {
            string dataResult = "";
            
            dataResult += "[ ";
            for (int i = 0; i < reelManager.resultContent.ReelResults.Count; i++) 
            {	
                for (int j = 0; j < reelManager.resultContent.ReelResults [i].SymbolResults.Count; j++) 
                {
                    if (j == 0) 
                    {
                        dataResult +="[" + reelManager.symbolMap.getSymbolInfo (reelManager.resultContent.ReelResults [i].SymbolResults [j]).name;
                    } else 
                    {
                        dataResult += ("," + reelManager.symbolMap.getSymbolInfo (reelManager.resultContent.ReelResults [i].SymbolResults [j]).name);
                    }

                    if (j == reelManager.resultContent.ReelResults [i].SymbolResults.Count - 1) dataResult += "], ";
                }
            }
            dataResult += "]";

            return dataResult;

        }

		bool hitBonus = false;
        public string CheckResult ()
        {
            reelManager.CheckFeature();
            reelManager.ParseSpinResult();
            
			string strFreespinInfo = "";
			#region Statistics RTP
			double initbalance  = leftBalanceInt;
			#endregion
			reelManager.ReelStripSimulationTest ();
            if (extraAward != null) {
				extraAward.ForExtraGameTest (reelManager.resultContent, reelManager);
            }
            bool needCheck = true;
            hitBonus = false;
            
//            reelManager.CheckHitBonus ();
//            reelManager.CheckHitFS ();
            reelManager.resultContent.awardResult.awardValue = 0;
       
            
            oneTimeAward = 0;
            if(reelManager.HitLinkGame)
            {
                TriggerLinkCount++;
            }
            if (reelManager.HasBonusGame) {
				bonusGame.InitBonusForTest (classicMachineConfig.extroInfos.infos);
				bonusGame.ForTest (currentBetting,reelManager);
                if (reelManager.IsNewProcess)
                {
                    hitBonus = true;   
                }
                else if (bonusGame.AwardInfo.isAwarded) {
					
                    hitBonus = true;
                }
            } 

            if (needCheck) {

				reelManager.CheckAward (classicMachineConfig.wildAccumulation, classicMachineConfig.extroInfos.infos);

//				if (reelManager.resultContent.awardResult.blackAward.isAwarded) {
//					AddNumber (reelManager.resultContent.awardResult.blackAward);
//                }
				for (int i = 0; i < reelManager.resultContent.awardResult.awardInfos.Count; i++)
                {
                    BaseAward lineAward = new BaseAward();
                    lineAward.awardName = reelManager.resultContent.awardResult.awardInfos[i].awardName;
                    lineAward.awardValue = reelManager.resultContent.awardResult.awardInfos[i].awardValue * currentBetting / Coins;
                    
					AddNumber (lineAward);
                }
            }

            if (reelManager.isFreespinBonus) 
            {
				reelManager.resultContent.awardResult.awardValue *= freespinGame.multiplier;
            }

           

			reelManager.PostCheckAwardEvent ();

            reelManager.resultContent.awardResult.awardValue *= currentBetting / (double)reelManager.gameConfigs.ScalFactor;
            
            //20200701 添加jp的处理
            SlotControllerConstants.JACKPOT_TYPE type;
            double jpCoins = reelManager.JpAwardCoin(out type);
            if (jpCoins > 0)
            {
                BaseAward lineAward = new BaseAward();
                lineAward.awardName = type.ToString();
                lineAward.awardValue = jpCoins;
                    
                AddNumber (lineAward);
                reelManager.resultContent.awardResult.awardValue += jpCoins;
            }
            //20200402,暂时添加新流程的处理
            if (reelManager.IsNewProcess)
            {
                if (hitBonus && bonusGame != null)
                {
                    double v = reelManager.GetBonusAward();
                    if (v > 0)
                    {
                        reelManager.resultContent.awardResult.awardValue += v;
                        bonusGame.AwardInfo.awardValue = v;
                        AddNumber(bonusGame.AwardInfo);
                    }
                }
            }
            //20200106, 新的bonus都用新的方式， 即IsOldResultMode为false
            else
            {
                if (hitBonus && bonusGame != null && bonusGame.AwardInfo != null && bonusGame.AwardInfo.isAwarded)
                {
                    if (!bonusGame.IsNewResultMode)
                    {
                        bonusGame.AwardInfo.awardValue *= reelManager.gameConfigs.ScalFactor;
                    }

                    reelManager.resultContent.awardResult.awardValue += bonusGame.AwardInfo.awardValue;
//                    bonusGame.AwardInfo.awardValue *= Coins / currentBetting;
                    AddNumber(bonusGame.AwardInfo);
                }
            }
			
            if (extraAward != null) 
            {
                if (extraAward.AwardInfo.isAwarded) 
                {
					reelManager.resultContent.awardResult.awardValue += extraAward.AwardInfo.awardValue;
//                    extraAward.AwardInfo.awardValue *= Coins / currentBetting;
                    AddNumber (extraAward.AwardInfo);
                }
            }

			if (reelManager.resultContent.awardResult.isAwarded) 
            {
				leftBalanceInt += System.Math.Round(reelManager.resultContent.awardResult.awardValue);
				if (!reelManager.isFreespinBonus)
				{
					totalAwardTime++;
					if (reelManager.resultContent.awardResult.awardValue > currentBetting)
					{
						winLBerPerLine++;
					}
				}
            }

            oneTimeAward = reelManager.resultContent.awardResult.awardValue;
            
            bool oldIsOneMore = isOneMore;
			isOneMore = reelManager.EnableOnceMore();
			reelManager.NextTimeOnceMore = isOneMore;
            //统计onceMore旋转的次数
            if (isOneMore)
            {
                onceMoreTime++;
            }
                
			if (!oldIsOneMore && isOneMore) {
				OnceMoreTriggerCount++;
			}

			if (reelManager.isFreespinBonus) 
            {
				if (reelManager.FreespinCount > 0) 
                {
					FreespinTime += reelManager.FreespinCount;
					reTriggerFreespinTimes ++;
					BaseAward moreAward = freespinGame.AddMore (reelManager.FreespinCount);
                    if (reelManager.IsNewProcess)
                    {
                        double enterTriggerAwardValue = reelManager.FsRetriggerScatterAward();
                        if(enterTriggerAwardValue >0)
                        {
                            strFreespinInfo += "\nScatter ReTriggerAwardValue: " + enterTriggerAwardValue;
                            leftBalanceInt += enterTriggerAwardValue;
                            //同时跟老的对接
                            BaseAward triggerFsAward = new BaseAward();
                            triggerFsAward.awardValue = enterTriggerAwardValue ;// / currentBetting * Coins;
                            triggerFsAward.awardName = $"RetriggerFsScatter{enterTriggerAwardValue}";
                            AddNumber(triggerFsAward);
                        } 
                    }
					else
                    {
                        if (moreAward.isAwarded)
                        {
                            leftBalanceInt += moreAward.awardValue * currentBetting;

                            moreAward.awardValue *= currentBetting;
                            AddNumber(moreAward);
                            strFreespinInfo = "ReTriggerAwardValue: " + moreAward.awardValue ;
                        }
                    }
				}

				if (!isOneMore) FreespinTime--;
				
				if (FreespinTime == 0 && (!isOneMore)) 
                {
					strFreespinInfo = "\nFreespinEnd\n";
					freespinGame.ChangeDataOnQuitGame (reelManager);
					reelManager.NeedReCreatResult = true;
					isOneMore = false;
                    if (extraAward != null) extraAward.OnQuitFreeSpinForTest();
                    
				} else 
                {
					if (isOneMore) 
                    {
						reelManager.NeedReCreatResult = false;
					} else 
                    {
						reelManager.NeedReCreatResult = true;
					}
				}
				freespinGame.LeftTime = FreespinTime;
			} 
            else 
            {
				if (isOneMore) 
                {
					reelManager.NeedReCreatResult = false;
				} else
                {
					reelManager.NeedReCreatResult = true;
				}
				if (FreespinTime == 0 && reelManager.FreespinCount > 0) 
                {
					triggerFreespinTimes  ++;
					reelManager.NeedReCreatResult = true;
					isOneMore = false;
					   
                   //先用新流程的方式处理
                   if (reelManager.IsNewProcess)
                   {
                       double enterFsAwardValue = reelManager.FsEnterScatterAward();
                       if(enterFsAwardValue >0)
                       {
                           strFreespinInfo += "\nFsScatterAwardValue: " + enterFsAwardValue;
                           leftBalanceInt += enterFsAwardValue;
                           
                           //同时跟老的对接
                           BaseAward enterFsAward = new BaseAward();
                           enterFsAward.awardValue = enterFsAwardValue ;// / currentBetting * Coins;
                           enterFsAward.awardName = "EnterFsScatter" + enterFsAward.awardValue;
                           
                           AddNumber(enterFsAward);
                       } 
                       freespinGame.InitData (reelManager.FreespinCount,classicMachineConfig.extroInfos.GetSubInfos(FreespinGame.FreespinSlotMachine),freespinGame.multiplier);
                       freespinGame.ChangerDateOnEnterGame (reelManager);
                       strFreespinInfo = "\n\nFreespinStart" + "\nFreespinCount: " + reelManager.FreespinCount + "\nfreespinGame.multiplier: " + freespinGame.multiplier;

                   }
                    else
                   {
                       freespinGame.InitData (reelManager.FreespinCount,classicMachineConfig.extroInfos.GetSubInfos(FreespinGame.FreespinSlotMachine),freespinGame.multiplier);
                       BaseAward moreAward = freespinGame.ChangerDateOnEnterGame (reelManager);
                       strFreespinInfo = "\n\nFreespinStart" + "\nFreespinCount: " + reelManager.FreespinCount + "\nfreespinGame.multiplier: " + freespinGame.multiplier;

                       if (moreAward.isAwarded)
                       {
                           leftBalanceInt += moreAward.awardValue * currentBetting;
                           moreAward.awardValue *= currentBetting;
                           AddNumber(moreAward);
                           strFreespinInfo += "\nScatterAwardValue: " + moreAward.awardValue ;
                       }
                   }
                   
                  
					FreespinTime += reelManager.FreespinCount;
					freespinGame.LeftTime = FreespinTime;
                    if (extraAward != null) extraAward.OnEnterFreeSpinForTest();
				}
			}

			#region Statistics RTP
			if (BaseGameConsole.ActiveGameConsole ().IsInSlotMachine () && (leftBalanceInt-initbalance) > 0) 
            {
				BaseSlotMachineController slot = BaseGameConsole.ActiveGameConsole ().SlotMachineController;
				slot.statisticsManager.WinCoinsNum += leftBalanceInt-initbalance;
			}
			#endregion
			leftBalance.text = leftBalanceInt.ToString();

			return strFreespinInfo;
        }

        public void AddNumber (BaseAward award)
        {

            if (reelManager.isFreespinBonus) {

                if (freespin.ContainsKey (award.awardName)) {
                    freespin [award.awardName].AddAwardCoins (award.awardValue); 
                } else {
                    freespin.Add (award.awardName, new PrintAwardInfo (award.awardValue));
                }
            } else {
                if (award != null) {
                    if (string.IsNullOrEmpty (award.awardName)) {
						
                    }

                    if(isOneMore)
                    {
                        if (onceMoreAwards.ContainsKey(award.awardName))
                        {
                            onceMoreAwards[award.awardName].AddAwardCoins(award.awardValue);
                        }
                        else
                        {
                            onceMoreAwards.Add(award.awardName, new PrintAwardInfo(award.awardValue));
                        }
                    }
                    else
                    {
                        if (awards.ContainsKey(award.awardName))
                        {
                            awards[award.awardName].AddAwardCoins(award.awardValue);
                        }
                        else
                        {
                            awards.Add(award.awardName, new PrintAwardInfo(award.awardValue));
                        }
                    }

                }
            }

            //Gold结果数据分析  暂时无用
            this.HandleGoldAward(award);
        }

        WaitForUpdate frame = new WaitForUpdate();
//        private async Task RunData ()
//        {
//            //在此方法之后调用thread的内容，类似于方法Task.Run
////            await new WaitForBackgroundThread();
//            while (leftBalanceInt >= currentBetting && running) {
//                OnRollStart ();
//                if(showDetail)
//                {
//                    DetailsOutputData.AppendLine(OnRollEnd());
//                }
//                else
//                {
//                    CheckResult();
//                }
////                TestData += (OnRollEnd () + "\n");
//                await frame;
//            }
//            //thread结束
//            
//        }

        private async Task NewTestFunc()
        {
//            await new WaitForBackgroundThread();
//            await Task.Run(() =>
//            {
                int count = 0;
                while (leftBalanceInt >= currentBetting && running)
                {
                    OnRollStart();
                    if (showDetail)
                    {
                        DetailsOutputData.AppendLine(OnRollEnd());
                    }
                    else
                    {
                        CheckResult();
                    }

                    count++;
                    if (count == 600)
                    {
                        count = 0;
                        await frame;
                    }
                }
//            });
        
            
            await frame;
        }

        IEnumerator Spin ()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo (savePath);
            if (!directoryInfo.Exists) {
                directoryInfo.Create ();
            }

            while (repeatTimeInt > 0 && running) {
                TestDataFileName = savePath + "/" +  slotName + "_Detail详情"   + ".txt";
                TestResultFileName = savePath + "/"  + slotName + "_基本输出"  + ".txt";
                totalSpinTime = 0;
                totalAwardTime = 0;
                winLBerPerLine = 0;
//                TestData = "";
                DetailsOutputData.Clear();
                TestResult = "";
                totalSpinInFreeSpin = 0;
                leftBalanceInt = initBalanceInt;
                FreespinTime = 0;
                isOneMore = false;
                awards.Clear ();
                freespin.Clear ();
                this.reelManager.ClearTestLog();
                
                yield return  NewTestFunc().AsIEnumerator();
                
//                yield return RunData().AsIEnumerator();
//                while (leftBalanceInt >= currentBetting && running) {
//                    OnRollStart ();
//                    TestData += (OnRollEnd () + "\n");
//                    yield return new WaitForSeconds (0.001f); 
//                }
			
			
			
                TestResult += "总共Spin的次数 : " + totalSpinTime + "\n";
                TestResult += "总共中奖的次数 : " + totalAwardTime + "\n";
                TestResult += "总共中奖时， win > Total Bet的次数 : " + winLBerPerLine + "\n";
                TestResult += "每种中奖情况出现的次数 : " + "\n";
			
			
                foreach (string key in awards.Keys) {
                    TestResult += key + "  " + awards [key].GetAwardInfo () + "\n";
                }

                TestResult += "TriggerLink的触发次数 : " + TriggerLinkCount + "\n";

                TestResult += "TriggerFreespinTimes In Normal Game"  + triggerFreespinTimes + "\n";
				TestResult += "触发OnceMore的次数："  + OnceMoreTriggerCount + "\n";

                TestResult += "freespin\n";
				TestResult += "ReTriggerFreespinTimes In FreeSpin Game"  + reTriggerFreespinTimes + "\n";
                TestResult += "总共FreeSpin的次数 : " + totalSpinInFreeSpin + "\n";

                foreach (string key in freespin.Keys) {
                    TestResult += key + "  " + freespin [key].GetAwardInfo () + "\n";
                }

                TestResult += "OnceMore中的中奖情况 : " + onceMoreTime + "\n";
                foreach (string key in onceMoreAwards.Keys)
                {
                    TestResult += key + "  " + onceMoreAwards[key].GetAwardInfo() + "\n";
                }
                TestResult += this.reelManager.GetTestLog();
                if (showDetail)
                    Write (DetailsOutputData.ToString(), TestDataFileName);
                Write (TestResult, TestResultFileName);	
                repeatTimeInt--;

                leftTime.text = "剩余重复次数：" + repeatTimeInt;

                //处理Gold结果数据
                this.HandleGoldData();
                yield  return new WaitForSeconds (0.16f);
            }
            running = false;
            buttonText.text = "Start Test";
        }


        public float GetBetWeight()
        {
            if (reelManager.isFreespinBonus) {
                return  freespinGame.multiplier * currentBetting / reelManager.gameConfigs.ScalFactor;
            } else {
                return currentBetting / reelManager.gameConfigs.ScalFactor;
            }
        }
        public void findReelConfig (string name)
        {
            GameObject prefab  = Resources.Load<GameObject>(name + "/MiddlePanel");
            #if UNITY_EDITOR
            if (prefab == null)//直接和场景相关的资源不建议翻到resource下，此大小在2M左右，而且直接关联的prefab放到resource下会增加unity构建查找索引时间。增加启动时间
            {
                prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Machines/" + name + "/Prefab/MiddlePanel.prefab");
            }
            #endif
            GameObject g = Instantiate (prefab);
            
			g.transform.SetParent(this.transform);
            g.transform.localScale = Vector3.zero;
            reelManager = g.GetComponentInChildren<ReelManager> ();
        }
        /*----------------------------------------------------------------------------------------------------------------------------------------- */
        #region 存储Gold数据
        protected List<string[]> dataTableList = new List<string[]>();
        protected Dictionary<string,PrintAwardInfo> baseAwards = new Dictionary<string, PrintAwardInfo> ();
        protected Dictionary<string,PrintAwardInfo> freeAwards = new Dictionary<string, PrintAwardInfo> ();
        protected Dictionary<string,PrintAwardInfo> reSpinAwards = new Dictionary<string, PrintAwardInfo> ();
        
        public virtual void HandleGoldAward(BaseAward goldAward)
        {
            if (reelManager.isFreespinBonus) 
            {
                SaveFreeAward(goldAward);
                return;
            }

            SaveBaseAward(goldAward);
        }

        public virtual void SaveBaseAward(BaseAward goldAward)
        {
            if(goldAward.awardName == "RespinAward")
            {
                goldAward.awardName = "Base-RespinAward";
                return;
            }
            if (baseAwards.ContainsKey (goldAward.awardName)) 
            {
                baseAwards[goldAward.awardName].AddAwardCoins (goldAward.awardValue); 
            } 
            else 
            {
                baseAwards.Add (goldAward.awardName, new PrintAwardInfo (goldAward.awardValue)); 
            }
        }

        public virtual void SaveFreeAward(BaseAward goldAward)
        {
            if(goldAward.awardName == "RespinAward")
            {
                goldAward.awardName = "Free-RespinAward";
                SaveReSpinAward(goldAward);
                return;
            }
            string[] nameList = goldAward.awardName.Split('_');
            if(nameList.Length == 2)
            {
                if(nameList[0] == "InFreespinScatter")
                {
                    BaseAward curValue = new BaseAward();
                    curValue.awardName = nameList[1] + "Scatter";
                    curValue.awardValue = goldAward.awardValue * Coins;
                    SaveBaseAward(curValue);
                    return;
                }
            }
            if (freeAwards.ContainsKey (goldAward.awardName)) 
            {
                freeAwards[goldAward.awardName].AddAwardCoins (goldAward.awardValue); 
            } 
            else 
            {
                freeAwards.Add (goldAward.awardName, new PrintAwardInfo (goldAward.awardValue)); 
            }
        }

        public virtual void SaveReSpinAward(BaseAward goldAward)
        {
            if (reSpinAwards.ContainsKey (goldAward.awardName)) 
            {
                reSpinAwards [goldAward.awardName].AddAwardCoins (goldAward.awardValue); 
            } 
            else 
            {
                reSpinAwards.Add (goldAward.awardName, new PrintAwardInfo (goldAward.awardValue)); 
            }
        }

        public virtual void SaveExtraAward(BaseAward goldAward)
        {
           
        }

        public void HandleGoldData()
        {
            if (reelManager != null) reelManager.ChangTestData();
            
            dataTableList.Clear();

            AddThemeInfo();
            AddAnalyzeData();
            AddBaseGameData();
            AddFreeGameData();
            AddReSpinGameData();
            AddExtraGameData();

            WriteDataTable();
        }

        public void AddThemeInfo()
        {
            dataTableList.Add(new string[2]
            {
                "ThemeName",
                slotName
            });

            dataTableList.Add(new string[2]
            {
                "PayLine",
                Coins.ToString()
            });

            dataTableList.Add(new string[2]
            {
                "Coins",
                Coins.ToString()
            });
        }

        public void AddAnalyzeData()
        {
             AddSplitLine();

            //主题总的RTP
            dataTableList.Add(new string[2]
            {
                "ThemeRTP",
                GetThemeRTP()
            });

            //BaseGame的RTP
            dataTableList.Add(new string[2]
            {
                "BaseGameRTP",
                GetBaseGameRTP()
            });

            //FreeGame的Hit和RTP
            dataTableList.Add(new string[2]
            {
                "FreeGameHit",
                GetFreeGameHit()
            });
            dataTableList.Add(new string[2]
            {
                "FreeGameRTP",
                GetFreeGameRTP()
            });

             //RespinGame的Hit和RTP
            dataTableList.Add(new string[2]
            {
                "ReSpinGameHit",
                GetReSpinGameHit()
            });
            dataTableList.Add(new string[2]
            {
                "RespinGameRTP",
                GetReSpinGameRTP()
            });

             //ExtreGame的Hit和RTP
            dataTableList.Add(new string[2]
            {
                "ExtreGameHit",
                GetExtraGameHit()
            });
            dataTableList.Add(new string[2]
            {
                "ExtraGameRTP",
                GetExtraGameRTP()
            });
        }

        public void AddBaseGameData()
        {
            AddSplitLine();

            dataTableList.Add(new string[4]{"AwardName", "Pay", "Hit", "RTP"});
            foreach (var key in baseAwards.Keys)
            {
                dataTableList.Add(new string[4]
                {
                    key,
                    baseAwards[key].linePay.ToString(),
                    SetPercentage(baseAwards[key].awardCount/(double)totalSpinTime),
                    SetPercentage(baseAwards[key].awardConis/totalSpinTime/currentBetting)
                });
            }

            baseAwards.Clear();
        }

        public void AddFreeGameData()
        {
            AddSplitLine();

            dataTableList.Add(new string[4]{"AwardName In Free", "Pay In Free", "Hit In Free", "RTP In Free"});
            foreach (var key in freeAwards.Keys)
            {
                dataTableList.Add(new string[4]
                {
                    key,
                    freeAwards[key].linePay.ToString(),
                    SetPercentage(freeAwards[key].awardCount/(double)totalSpinTime),
                    SetPercentage(freeAwards[key].awardConis/totalSpinTime/currentBetting)
                });
            }

            freeAwards.Clear();
        }

        public virtual void AddReSpinGameData()
        {
            AddSplitLine();
            reSpinAwards.Clear();
        }

        public virtual void AddExtraGameData()
        {
            AddSplitLine();
        }

        public virtual string GetThemeRTP()
        {
            return SetPercentage(1 - ((initBalanceInt - leftBalanceInt)/((double)totalSpinTime * currentBetting)));
        }

        public virtual string GetBaseGameRTP()
        {
            double totalCoins = 0;
            foreach (var key in baseAwards.Keys)
            {
                totalCoins += baseAwards[key].awardConis;
            }

            return SetPercentage(totalCoins/totalSpinTime/currentBetting);
        }

        public virtual string GetFreeGameHit()
        {
            return SetPercentage((double)triggerFreespinTimes/totalSpinTime);
        }
        public virtual string GetFreeGameRTP()
        {
            double totalCoins = 0;
            foreach (var key in freeAwards.Keys)
            {
                totalCoins += freeAwards[key].awardConis;
            }

            return SetPercentage(totalCoins/totalSpinTime/currentBetting);
        }

        public virtual string GetReSpinGameHit()
        {
            return "";
        }
        public virtual string GetReSpinGameRTP()
        {
            return "";
        }

        public virtual string GetExtraGameHit()
        {
            return "";
        }
        public virtual string GetExtraGameRTP()
        {
            return "";
        }

        private void AddSplitLine()
        {
            string[] line = new string[10];
            for (int i = 0; i < line.Length; i++)
            {
                line[i] = "————————————————————";
            }
            dataTableList.Add(line);
        }

        private string SetPercentage(double value)
        {
            return String.Format("{0:N4}",value * 100) + "%"; 
        }

        public void WriteDataTable()
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach(var data in dataTableList)
            {
                stringBuilder.AppendLine(string.Join(",", data));
            }        
            StreamWriter outStream = System.IO.File.CreateText(savePath + "/" + slotName + ".csv");
            outStream.WriteLine(stringBuilder);
            outStream.Close();
        }

        #endregion
    }


    public class PrintAwardInfo
    {
        public int awardCount = 0;
		public double awardConis = 0;
        public double linePay = 0;

		public PrintAwardInfo (double firstAwardCoins)
        {
            this.awardConis = firstAwardCoins;
            this.awardCount = 1;
            linePay = firstAwardCoins;
        }

        public void AddAwardCoins (double addAwardCoins)
        {
            this.awardConis += addAwardCoins;
            this.awardCount++;
        }

        public string GetAwardInfo ()
        {
            return "中奖次数：" + this.awardCount + " 中奖金额：" + this.awardConis;
        }
    }
}
