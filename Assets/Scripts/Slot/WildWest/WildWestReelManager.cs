using Classic;
using WildWest;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class WildWestReelManager : GoldsReelManager
{
    public Button pendantBtn;
    public GameObject pendantB05;
    public GameObject moveCanvas;
    public WildWestBackgroudPanel bisonUI;
    private List<WildWestSymbolRender> wildRender = new List<WildWestSymbolRender>();

    #region FreeSpin
    public GameObject taurus;
    public GameObject taurusEffect;
    #endregion

    private long bet = 0;
    public int nextIndex = 0;
    public int gatherIndex = 0;
    public WildWestSpinResult spinresult {get; private set;}

    public List<FreatureInfo> featureTable = new List<FreatureInfo>();

    protected override void Awake()
    {
        base.Awake();
        Messenger.AddListener<long> (SlotControllerConstants.OnBetChange, BetLevelSmallDialog);
    }
    void OnDestory()
    {
        base.Awake();
        Messenger.RemoveListener<long> (SlotControllerConstants.OnBetChange, BetLevelSmallDialog);
    }

    public override void InitReels(SlotMachineConfig slotConfig, Classic.GameCallback onStop, Classic.GameCallback onStart)
    {
        onStart +=()=> {this.RunStart();};

        onStop +=()=> {this.RunStop();};

        spinresult = new WildWestSpinResult(this, slotConfig);

        base.InitReels(slotConfig, onStop, onStart);
                
        if(slotConfig.extroInfos.infos.ContainsKey("feature_link_info"))
        {
            List<object> infos = slotConfig.extroInfos.infos["feature_link_info"] as List<object>;

            for (int i = 0; i < infos.Count; i++)
            {
                FreatureInfo feature = new FreatureInfo(infos[i]);
                if(i == 0) feature.ISUNLOCK = true;
                featureTable.Add(feature);
            }
        }

        bet = BaseSlotMachineController.Instance.currentBetting;

        pendantBtn.onClick.AddListener(()=>
        {
            WildWestGatherDialog();
        });
    }

    private void RunStart()
    {
        pendantBtn.interactable = false;
    }

    private void RunStop()
    {
        pendantBtn.interactable = true;
    }

    // public override SmartAntiEffectController GetSmartController()
    // {
    //     if(this.isFreespinBonus) return this.freespinSmartController;

    //     return this.GetComponent<SmartAntiEffectController>();
    // }

    public override void CreateRawResult()
    {
        base.CreateRawResult();
    }

    /// <summary>
    /// 用于填充替换数据，用于基础及rule中
    /// </summary>
    public override void CreateRealAwardResult()
    {
        base.CreateRealAwardResult();    
    }
    public override void ParseSpinResult()
    {
        
        base.ParseSpinResult();
    }
    public override void InitAtStart() 
    { 
        this.IsNewProcess = true; 
    } 
    public override void CheckHitFS()
    {
        spinresult.OnSpinResult();
        this.FreespinCount = spinresult.FreeSpinCount();
        HitFs = this.FreespinCount > 0;
    }




    public override bool StartRun()
    {
        if(!this.isFreespinBonus)
        {
            this.StartRunSetBet(BaseSlotMachineController.Instance.currentBetting);
            WildWestUserData.SaveUseBet(BaseSlotMachineController.Instance.currentBetting);
        }
        else
        {
            StopSymbolAnimation();
        }

        return base.StartRun();
    }
    public  void StopSymbolAnimation()
    {
        for (int i = 0; i < this.awardElements.Count; i++)
        {
            if (this.awardElements[i] != null)
            {
                this.awardElements[i].StopAnimation();
            }
        }
    }
    protected override void HandleRollEndEvent(System.Action callback)
    {
        wildRender.Clear();

        if(this.isFreespinBonus)
        {
            if(this.spinresult.featureItemTable.Count == 0)
            {
                this.RotationEnd(callback);
                return;
            }
            StartCoroutine(FreeSpinRotationEnd(callback));
        }
        else
        {
            this.RotationEnd(callback);
        }
    }

    private void RotationEnd(System.Action callback)
    {
        if(this.spinresult.wildMultiple.Count == 0)
        {
            base.HandleRollEndEvent(callback);
            if(IsGatherPendant()) this.GatherPendant();
        }else
        {
            StartCoroutine(WildWestRollEnd(callback));
        }
    }

    private IEnumerator FreeSpinRotationEnd(System.Action callback)
    {
        List<WildWestSymbolRender> renderList = new List<WildWestSymbolRender>();
        List<TaurusMove> taurusMoveList = new List<TaurusMove>();

        foreach (var item in this.spinresult.featureItemTable)
        {
            WildWestSymbolRender render = this.GetSymbolRender(item[0], item[1]) as WildWestSymbolRender;
            render.PlayAnimation(1, false, null, ()=>{render.stop = true;});
            renderList.Add(render);
            
            GameObject taurusObject = Instantiate(taurusEffect, this.moveCanvas.transform);
            taurusObject.transform.localScale = Vector3.one;
            taurusObject.transform.position = render.transform.position;
            TaurusMove taurusMove = taurusObject.GetComponent<TaurusMove>();
            taurusMove.OnStartMove(taurus.transform.position, 0.6f);
            taurusObject.SetActive(false);
            taurusMoveList.Add(taurusMove);
        }

        yield return new WaitForSecondsRealtime(1f);

        foreach (var item in taurusMoveList)
        {
            item.gameObject.SetActive(true);
            item.StartMove();
        }
        Libs.AudioEntity.Instance.PlayEffect("collect_B04");
        yield return new WaitUntil(()=>IsMoveStop(taurusMoveList));
        
        bisonUI.SetFreeSpinUI();

        yield return new WaitUntil(()=>AnimationStop(renderList));

        foreach (var item in renderList)
        {
            item.StopAnimation();
            item.ChangeSymbol(5);
        }

        this.RotationEnd(callback);

        yield break;
    }

    private bool IsMoveStop(List<TaurusMove> movetable)
    {
        foreach (var item in movetable)
        {
            if(!item.stop) return false;
        }
        return true;
    }

    private bool IsGatherPendant()
    {
        for (int i = 0; i < this.spinresult.pendantCount.Count; i++)
        {
            for (int j = 0; j < this.spinresult.pendantCount[i].Count; j++)
            {
                if(this.spinresult.pendantCount[i][j] != 0) return true;
            }
        }
        return false;
    }

    private IEnumerator WildWestRollEnd(System.Action callback)
    {   
        Libs.AudioEntity.Instance.PlayEffect("wild_multiply");

        foreach (var item in this.spinresult.wildMultiple.Keys)
        {
           WildWestSymbolRender render = this.GetSymbolRender(item[0], item[1]) as WildWestSymbolRender;
           if(this.FreespinCount > 0)
           {
               render.stop = true;
           }else
           {
                render.PlayAnimation(1, false, null, ()=>{render.stop = true;});
           }
           wildRender.Add(render);
        }
        
        yield return new WaitUntil(()=>AnimationStop(wildRender));
        
        base.HandleRollEndEvent(callback);
        if(IsGatherPendant()) this.GatherPendant();
        yield break;
    }

    private bool AnimationStop(List<WildWestSymbolRender> symbolRender)
    {
        if(symbolRender.Count == 0) return true;
        foreach (var item in symbolRender)
        {
            if(item.stop) return true;
        }
        return false;
    }
    
    protected override void ReCheckAward(Dictionary<string, object> infos)
    {
        this.spinresult.CheckSpinResult();

        int mul = this.spinresult.MultipleValue();
        
        foreach (var lineItem in this.spinresult.lineInfo)
        {
            foreach (var info in lineItem.linePos)
            {
                List<BaseElementPanel> awardElement = new List<BaseElementPanel>();
                List<int> LineResultIndexs = new List<int>();
                for (int i = 0; i < info.Count; i++)
                {
                    awardElement.Add(this.GetSymbolRender(i, info[i]));
                    LineResultIndexs.Add(this.resultContent.ReelResults[i].SymbolResults[info[i]]);
                }
                
                for (int i = LineResultIndexs.Count; i < this.resultContent.ReelResults.Count; i++)
                {
                    LineResultIndexs.Add(23);
                }
                this.resultContent.awardResult.AddAwardPayLine(lineItem.pay * mul, lineItem.name, awardElement, gameConfigs, 1000+lineItem.symbolId, true, null, LineResultIndexs);
            }
        }

        foreach (var render in wildRender) this.awardElements.Add(render);
        
        wildRender.Clear();

        BaseSlotMachineController.Instance.baseAward = resultContent.awardResult.awardValue;
    }
    
    private void GatherPendant()
    {        
        for (int i = 0; i < this.spinresult.pendantCount.Count; i++)
        {
            for (int j = 0; j < this.spinresult.pendantCount[i].Count; j++)
            {
                if(this.spinresult.pendantCount[i][j] == 0) continue;

                WildWestSymbolRender render =  this.GetSymbolRender(i, j) as WildWestSymbolRender;
                GameObject pendant = Instantiate(pendantB05, this.moveCanvas.transform);
                pendant.transform.localScale = Vector3.one;
                pendant.transform.position = render.count.transform.parent.transform.position;
                PendantMove pendantMove = pendant.GetComponent<PendantMove>();
                pendantMove.coin.text = render.count.text;
                render.SetCountUI(0);
                Libs.AudioEntity.Instance.PlayEffect("fly_to_meter_appear");
                new Libs.DelayAction(0.3f, null, ()=>
                {
                    Libs.AudioEntity.Instance.PlayEffect("fly_to_meter_fly");
                }).Play();
                pendantMove.OnStartMove(pendantBtn.transform.position, 0.5f);
            }
        }  

        new Libs.DelayAction(0.8f, null, ()=>
        {
            bisonUI.SetPendantUI();
        }).Play();
    }

    public void WildWestGatherDialog(bool unlock = false)
    {
        if(this.AutoRun || this.isFreespinBonus || this.FreespinCount > 0) return;
        
        Libs.UIManager.Instance.OpenMachineDialog<WildWestGatherDialog>((dialog)=>
        {
            dialog.OnStart(this, gatherIndex, unlock);
        }, null, inAnimation:Libs.UIAnimation.Scale, outAnimation:Libs.UIAnimation.Scale, maskAlpha:0.8f);
    }
    
    private void BetLevelBigDialog()
    {
        
        long maxbet = UserManager.GetInstance().MaximumBetting (UserManager.GetInstance().UserProfile().Level());

        Libs.UIManager.Instance.OpenMachineDialog<WildWestBetLevelBigDialog>( (dialog)=>
        {
            dialog.OnStart(this, maxbet, this.spinresult.triggerFreeInfo);
        }, ()=>
        {
            Libs.UIManager.Instance.OpenMachineDialog<WildWestBetLevelSmallDialog>( (dialog)=>
            {
                dialog.OnStart(this.spinresult.betLevel);
            }, null, inAnimation:Libs.UIAnimation.NOAnimation, outAnimation:Libs.UIAnimation.NOAnimation, maskAlpha:0.8f);
            
        },inAnimation: Libs.UIAnimation.Scale, outAnimation:Libs.UIAnimation.Scale, maskAlpha:0.8f);
    }

    private void BetLevelSmallDialog(long value)
    {
        if(this.isFreespinBonus || this.FreespinCount > 0) return;
        int index = this.BetLevelIndex(value);
        bet = BaseSlotMachineController.Instance.currentBetting;
        if(index == -1) return;
        this.SetBetLevel(index);
        Libs.UIManager.Instance.OpenMachineDialog<WildWestBetLevelSmallDialog>( (dialog)=>
        {
            dialog.OnStart(this.spinresult.betLevel);
        }, null, inAnimation:Libs.UIAnimation.NOAnimation, outAnimation:Libs.UIAnimation.NOAnimation, maskAlpha:0.8f);
    }

    public void SetBetLevel(int level, long _bet = -1)
    {
        this.spinresult.betLevel = level;
        if(_bet == -1) return;
        bet = _bet;
        BaseSlotMachineController.Instance.currentBetting = bet;
        BaseSlotMachineController.Instance.currentBettingTemp = bet;
        Messenger.Broadcast<long> (SlotControllerConstants.OnBetChange, bet);
    }

    public int BetLevelIndex(long value)
    {
        if( bet < this.spinresult.triggerFreeInfo[1].Bet)
        {
            if(value >= this.spinresult.triggerFreeInfo[2].Bet) return 2;
            if(value >= this.spinresult.triggerFreeInfo[1].Bet) return 1;
        }

        if( bet >= this.spinresult.triggerFreeInfo[1].Bet && bet < this.spinresult.triggerFreeInfo[2].Bet)
        {
            if(value < this.spinresult.triggerFreeInfo[1].Bet) return 0;
            if(value >= this.spinresult.triggerFreeInfo[2].Bet) return 2;
        }

        if(bet >= this.spinresult.triggerFreeInfo[2].Bet)
        {
            if(value < this.spinresult.triggerFreeInfo[1].Bet) return 0;
            if(value < this.spinresult.triggerFreeInfo[2].Bet) return 1;
        }
        return -1;
    }

    public void StartRunSetBet(long value)
    {
        if(value >= this.spinresult.triggerFreeInfo[2].Bet)
        {
            this.spinresult.betLevel = 2;
        }else if(value >= this.spinresult.triggerFreeInfo[1].Bet)
        {
            this.spinresult.betLevel = 1;
        }else
        {
            this.spinresult.betLevel = 0;
        }
    }

    public void SetFreePendant()
    {
        this.spinresult.SetPendantNum(this.spinresult.freespinPendant);
        bisonUI.SetPendantUI(false);
    }

    public void InitFreeSpinData(string name, FreatureInfo info)
    {
        this.FeatureFreeSpinData();

        this.spinresult.ReSetFreeSpinData();

        if(name == "BONUS"){
            this.freespinFinished = false;
            this.spinresult.freeType = BisonFree.BONUS;
            this.FreespinCount = info.pageInfo.bonustimes;
            this.spinresult.featuremul = this.featureTable.IndexOf(info);
        }else if(name == "SUPERFREE"){
            this.freespinFinished = false;
            this.JudgeIsAllUnLock(info);
            this.spinresult.freeType = BisonFree.SUPER;
            this.FreespinCount = info.pageInfo.times;
            this.spinresult.featuremul = this.featureTable.IndexOf(info);
            this.spinresult.InitSuperFreeData(info);
        }

        new Libs.DelayAction(1f, null, ()=>
        {
            this.StartFeatureFree();
        }).Play();
    }

    public void JudgeIsAllUnLock(FreatureInfo info)
    {
        if(WildWestUserData.allUnLock)
        {
            foreach (var item in featureTable)
            {
                if(item.ISACHIEVE) item.ResetItemInfo();
            }
        }else
        {   
            int index = featureTable.IndexOf(info);
            if(index < featureTable.Count - 1)
            {
                featureTable[index+1].ISUNLOCK = true;
                nextIndex = index+1;
            } 
        }

        if(WildWestUserData.allUnLock) return;

        WildWestUserData.allUnLock = true;

        foreach (var item in featureTable)
        {
            if(!item.ISACHIEVE) WildWestUserData.allUnLock = false;
        }

        if(WildWestUserData.allUnLock)
        {
            foreach (var item in featureTable) if(item.ISACHIEVE) item.ResetItemInfo();
        }
    }
    public override void BeforeNextSpinHandler(System.Action callback)
    {
        if (!ResultStateManager.Instante.slotController.onceMore && IsSendEsCurrentSpin() && spinresult.triggerBetting!=0)
        {
            BaseSlotMachineController.Instance.currentBetting = spinresult.triggerBetting;
            BaseSlotMachineController.Instance.RestoreSlotsMachinesGameStateData(spinresult.triggerBetting);
            Messenger.Broadcast(SlotControllerConstants.OnAverageBet, false);
            Messenger.Broadcast(SlotControllerConstants.OnBetChange, spinresult.triggerBetting);
            spinresult.triggerBetting = 0;
        }

        base.BeforeNextSpinHandler(callback);
    }
    public void StartFeatureFree()
    {
        this.SetAverageBetState();
        ResultStateManager.Instante.PlayAwardAfterFeature();
        BaseSlotMachineController.Instance.winCoinsForDisplay = 0;
        Messenger.Broadcast<bool>(SlotControllerConstants.DisableSpinButton, true);
        Messenger.Broadcast<bool> (SlotControllerConstants.DisactiveButtons,this.gameConfigs.EnableSpinBtnInFreeSpin);
        Messenger.Broadcast<float, long, long>(SlotControllerConstants.OnChangeWinText, 1f, 0, 0);
    }

    public void SetAverageBetState(bool diable = true,bool isRecover=false)
    {
        Messenger.Broadcast<bool>(SlotControllerConstants.OnAverageBet, true);
        if(diable) Messenger.Broadcast<bool>(SlotControllerConstants.DisableSpinButton, true);
        if (!isRecover)
        {
            spinresult.triggerBetting = BaseSlotMachineController.Instance.currentBetting;
        }
        BaseSlotMachineController.Instance.currentBettingTemp = Utils.Utilities.CastValueLong(WildWestUserData.averageBet.ToString());
        this.StartRunSetBet(BaseSlotMachineController.Instance.currentBettingTemp);
        BaseSlotMachineController.Instance.currentBetting = Utils.Utilities.CastValueLong(WildWestUserData.averageBet.ToString());
       
    }

    public void FeatureFreeSpinData()
    {
        this.baseGameResult = new List<List<int>>();
        
        for (int i = 0; i < 5; i++)
        {
            List<int> symbolId = new List<int>();
            for (int j = 0; j < 4; j++)
            {
                if(i == 0)
                {
                    symbolId.Add(Random.Range(10, 15));
                }else if(i == 1)
                {
                    symbolId.Add(Random.Range(5, 9));
                }else
                {
                    symbolId.Add(Random.Range(5, 15));
                }
            }
            baseGameResult.Add(symbolId);
        }
        resultContent.ChangeResult(baseGameResult);
    }

    #region 数据保存恢复
    private WildWestProgressData progressData = null;

    protected override void CreateProgreeData()
    {
        this.SaveGatherGameData();
        progressData = new WildWestProgressData();
        this.progressData.averageBetTable = this.progressData.DictionaryToString<double, double>(WildWestUserData.userUseBet);
        this.progressData.currentBetting = BaseSlotMachineController.Instance.currentBetting;
        this.progressData.RName = reelStrips.GetCurrentUseRName ();
        this.progressData.AName = reelStrips.GetCurrentUseAName ();
        progressData.triggerBetting = spinresult.triggerBetting;
        this.SaveFreeGameData();
    }

    private void SaveFreeGameData()
    {
        this.progressData.betlevel = this.spinresult.betLevel.ToString();
        this.progressData.freeType = ((int)this.spinresult.freeType).ToString();
        this.progressData.featureItem = this.spinresult.featureItem.ToString();
        this.progressData.featuremul = this.spinresult.featuremul.ToString();
        this.progressData.initLevel = this.spinresult.initLevel.ToString();
        this.progressData.freeLevel = this.spinresult.freeLevel.ToString();
        this.progressData.infreePendant = this.spinresult.infreePendant.ToString();
        this.progressData.freespinPendant = this.spinresult.freespinPendant.ToString();
        this.progressData.equalTable = Utils.Utilities.ListToStr<int>(this.spinresult.equalTable);
        List<int[]> itemList = new List<int[]>(this.spinresult.freespinTable.Keys);

        foreach (var index in itemList)
        {
            if(itemList.IndexOf(index) == itemList.Count-1)
            {
                this.progressData.freespinTable += index[0].ToString() + "_" + 
                                                    index[1].ToString() + "_" +
                                                    this.spinresult.freespinTable[index].ToString();
            }else
            {
                this.progressData.freespinTable += index[0].ToString() + "_" + 
                                                    index[1].ToString() + "_" +
                                                    this.spinresult.freespinTable[index].ToString() + ";";
            }
        }

        this.progressData.Ispick = (this.FreespinGame as WildWestFreeSpin).Ispick.ToString();

        if(!this.freespinFinished && this.FreespinGame != null)
        {
            if (isFreespinBonus) 
            {
                if (FreespinCount>0 && IsAlreadyAddFreeSpinCount)
                {
                    FreespinCount = 0;
                }
                progressData.IsInFreespin = true;
                progressData.freespinCount = FreespinCount;
				progressData.awardMultipler = FreespinGame.multiplier;
                progressData.leftFreeSpinCount = FreespinGame.LeftTime;
				progressData.totalFreeSpinCount = FreespinGame.TotalTime;
                if (progressData.leftFreeSpinCount==progressData.totalFreeSpinCount)
                {
                    progressData.freespinCount = 0;
                }
                progressData.currentWinCoins = BaseSlotMachineController.Instance.winCoinsForDisplay;
                this.progressData.FreespinResultString = Utils.Utilities.ConvertListToString<int>(freeSpinResult);
            }else if(FreespinCount>0)
            {
                progressData.currentWinCoins = BaseSlotMachineController.Instance.winCoinsForDisplay;
                progressData.freespinCount = FreespinCount;
            }

            this.progressData.BaseGameResultString = Utils.Utilities.ConvertListToString<int>(baseGameResult);
        }
    }

    private void SaveGatherGameData()
    {
        SharedPlayerPrefs.SetPlayerPrefsBoolValue("ALLUNLOCK", WildWestUserData.allUnLock);
        PlayerPrefs.SetInt("PENDANT", this.spinresult.pendantNum);
        PlayerPrefs.SetInt("GATHERINDEX", gatherIndex);
        PlayerPrefs.SetInt("NEXTINDEX", nextIndex);
        string info = "";
        foreach (var table in featureTable)
        {
            string tableinfo = table.ISUNLOCK.ToString() + "," + table.ISACHIEVE.ToString() + "," + table.infoIndex.ToString() + ",";

            foreach (var item in table.itemInfo)
            {
                string dataInfo = item.name.ToString() + "_" +
                                  item.index.ToString() + "_" +
                                  item.indexDouble.ToString() + "_" +
                                  item.rate.ToString() + "_" +
                                  item.coin.ToString();
                if(table.itemInfo.IndexOf(item) == table.itemInfo.Count-1)
                {
                    tableinfo += dataInfo;
                }else
                {
                    tableinfo += dataInfo + "|";
                }
            }

            if(featureTable.IndexOf(table) == featureTable.Count-1)
            {
                info += tableinfo;
            }else
            {
                info += tableinfo + ';';
            }
        }

        PlayerPrefs.SetString("FEATURE", info);
    }

    protected override void SaveBaseProgressData()
	{

    }

    protected override void SaveProgressDataToLocal ()
    {
        SceneProgressManager.SaveSceneJson<WildWestProgressData>(slotConfig.Name(), progressData);
    }
    
    protected override void LoadLocalProgress ()
    {
        this.ReStoreGatherGameData();
        bisonUI.InitBisonUI(this);
        this.progressData = SceneProgressManager.LoadSceneJson<WildWestProgressData> (slotConfig.Name());
        if(this.progressData == null)
        {
            this.BetLevelBigDialog();
            return;
        } 
        WildWestUserData.userUseBet = this.progressData.userUseBet;
        spinresult.triggerBetting = progressData.triggerBetting;
        this.ReStoreFreeGame();
    }

    private void ReStoreGatherGameData()
    {
        WildWestUserData.allUnLock = SharedPlayerPrefs.GetPlayerBoolValue("ALLUNLOCK", false);
        this.spinresult.pendantNum = PlayerPrefs.GetInt("PENDANT", 0);
        this.gatherIndex = PlayerPrefs.GetInt("GATHERINDEX", 0);
        this.nextIndex = PlayerPrefs.GetInt("NEXTINDEX", 0);
        string info = PlayerPrefs.GetString("FEATURE", "");
        if(string.IsNullOrEmpty(info)) return;
        string[] featureInfo = info.Split(';');
        for (int i = 0; i < featureInfo.Length; i++)
        {
            string[] data = featureInfo[i].Split(',');
            featureTable[i].ISUNLOCK = bool.Parse(data[0]);
            featureTable[i].ISACHIEVE = bool.Parse(data[1]);
            featureTable[i].infoIndex = int.Parse(data[2]);
            string[] itemInfo = data[3].Split('|');
            for (int j = 0; j < itemInfo.Length; j++)
            {
                string[] strInfo = itemInfo[j].Split ('_');
                featureTable[i].itemInfo[j].name = strInfo[0];
                featureTable[i].itemInfo[j].index = int.Parse(strInfo[1]);
                featureTable[i].itemInfo[j].indexDouble = int.Parse(strInfo[2]);
                featureTable[i].itemInfo[j].rate = float.Parse(strInfo[3]);
                featureTable[i].itemInfo[j].coin = double.Parse(strInfo[4]);
            }
        }
    }

    private void ReStoreFreeGame()
    {
        this.spinresult.freeType = (BisonFree)int.Parse(this.progressData.freeType);
        this.spinresult.featuremul = int.Parse(this.progressData.featuremul);
        this.spinresult.featureItem = int.Parse(this.progressData.featureItem);
        this.spinresult.initLevel = int.Parse(this.progressData.initLevel);
        this.spinresult.freeLevel = int.Parse(this.progressData.freeLevel);
        this.spinresult.infreePendant = int.Parse(this.progressData.infreePendant);
        this.spinresult.freespinPendant = int.Parse(this.progressData.freespinPendant);
        this.spinresult.equalTable = Utils.Utilities.StrToList<int>(this.progressData.equalTable);

        if(!string.IsNullOrEmpty(this.progressData.freespinTable))
        {
            this.spinresult.freespinTable.Clear();
            string[] strInfo = this.progressData.freespinTable.Split (';');
            foreach (var item in strInfo)
            {
                string[] info = item.Split ('_');
                this.spinresult.freespinTable.Add(new int[2]{int.Parse(info[0]), int.Parse(info[1])}, int.Parse(info[2]));
            }
        }
        (this.FreespinGame as WildWestFreeSpin).Ispick = bool.Parse(this.progressData.Ispick);

        bool isReStoreFree = false;

        if (progressData.IsInFreespin) 
        {
            this.spinresult.betLevel = int.Parse(this.progressData.betlevel);
            isReStoreFree = true;
            this.isFreespinBonus = true;
            FreespinGame.NeedRecoveryData = true;
            this.FreespinCount = progressData.freespinCount;
            resultContent.ChangeResult(this.progressData.FreespinResultList);
            ChangeSymbols(this.progressData.FreespinResultList);
            BaseSlotMachineController.Instance.winCoinsForDisplay = progressData.currentWinCoins;
            BaseSlotMachineController.Instance.RestoreSlotsMachinesGameStateData(this.progressData.currentBetting);
            if(this.spinresult.freeType != BisonFree.SPIN) this.SetAverageBetState(false,true);
            ResultStateManager.Instante.slotController.spinning = true;
            ResultStateManager.Instante.RestoreFreespin(progressData.leftFreeSpinCount, progressData.totalFreeSpinCount, progressData.currentWinCoins, true, progressData.awardMultipler, true, false, true);
        } 
        else if(progressData.freespinCount > 0)
        {
            this.spinresult.betLevel = int.Parse(this.progressData.betlevel);
            isReStoreFree = true;
            resultContent.ChangeResult(this.progressData.BaseGameResultList);
            ChangeSymbols(this.progressData.BaseGameResultList);
            this.FreespinCount = progressData.freespinCount;
            Messenger.Broadcast<bool>(SlotControllerConstants.DisableSpinButton, true);
            BaseSlotMachineController.Instance.winCoinsForDisplay = progressData.currentWinCoins;
            BaseSlotMachineController.Instance.RestoreSlotsMachinesGameStateData(this.progressData.currentBetting);
            if(this.spinresult.freeType != BisonFree.SPIN) this.SetAverageBetState(true,true);
            ResultStateManager.Instante.RestoreFreespinOnBaseGame(progressData.freespinCount);
        } 

        bet = BaseSlotMachineController.Instance.currentBetting;

        if(!isReStoreFree)
        {
            this.spinresult.ReSetFreeSpinData();
            this.BetLevelBigDialog();
        } 

    }
    
    protected override void RestoreBaseProgressData()
    {

    }

    #endregion
}
