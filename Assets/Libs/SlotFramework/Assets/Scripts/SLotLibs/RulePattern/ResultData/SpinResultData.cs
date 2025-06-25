using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Profiling;
using UnityEngine.Serialization;

[Serializable]
public class SpinResultData
{
    //rule check 最多次数 // 超过 代表有问题
    private readonly int MOST_RULE_CHECK_COUNT = 10; 
    private IRuleResult m_Controller;
    public int RuleUseCount = 0;
    #region system数据，可后期处理
//        private List<List<int>> m_BoardSymbolResults = new List<List<int>>();
//
//        public bool IsFS;
//        public bool IsBonus;
    #endregion

    #region 每次spin变化数据
//    public bool HitBonus;
//    public int TriggerFsCount;
//    public bool HitFs;
//    public bool HitLink; //是否触发link game

    public bool IsFeature;
    //目前只处理basegame和fs的线奖。feature已经Jackpot如有需要,epic等之后处理
//    public bool IsBigWin;

    //中奖线的奖励
    public float TotalLineAwardValue;

    #endregion
    public SpinResultData()
    {
    }

    public void Init(IRuleResult reelManager)
    {
        m_Controller = reelManager;
    }
    /// <summary>
    /// 是否需要rule处理
    /// </summary>
    /// <returns></returns>
    public virtual bool NeedRuleCheck(long _balance)
    {
        if (RulePatternManager.GetInstance().RuleConditionValid(_balance,m_Controller))
        {
            return true;
        }
        return false;
    }
 

    protected void ResultParse(RuleSwitch ruleSwitch)
    {
        RuleResultMediation mediation = m_Controller.GetRuleResult(ruleSwitch);
        IsFeature = mediation.IsFeature;
        TotalLineAwardValue = mediation.AwardValue;

    }

    public void RuleCreateResult(RuleSwitch ruleSwitch)
    {
        Profiler.BeginSample("createSpinResult");
        RuleUseCount = 0;
        bool checkSkip = false;
        do
        {
            m_Controller.RuleCreateResult();
            
            ResultParse(ruleSwitch);

            RuleUseCount++;
            
            checkSkip = RulePatternManager.GetInstance().EvalSpinResult(this,ruleSwitch);

        } while (checkSkip == true && RuleUseCount < MOST_RULE_CHECK_COUNT);

        RulePatternManager.GetInstance().Round = RuleUseCount;
        Log.LogLimeColor($"ruleUseCount:{RuleUseCount}   {RulePatternManager.GetInstance().RuleCondition.CurrentSpinNum }");
//        if (checkSkip == false)
//        {
            RulePatternManager.GetInstance().RuleAddOneTime();
//        }else if (ruleUseCount > MOST_RULE_CHECK_COUNT)
//        {
//            //待处理,超过次数了
//        }
        Profiler.EndSample();
    }

//    #region obsole
//    
//    public void CreateResult()
//    {
//        if (BaseGameConsole.singletonInstance.isForTestScene)
//        {
//            m_Controller.resultContent.CreateRawResult();
//        }
//        else
//        {
//            CreateBaseResult();
//            m_Controller.CheckCommonJackPot(); //将来有空需要处理
//            ForceDataOutcome();
//            m_Controller.CheckAnticipationAnimation(); //可能跟rule有重复计算，将来可优化
//        }
//    }
//    private bool isUseForcePattern = false;
//    OutCome currentServerOutCome = null;
//
//    private void ForceDataOutcome()
//    {
//        SpinClickedMsgMgr.Instance.ResetData(BaseSlotMachineController.Instance.isFreeRun ||
//                                             m_Controller.IsInBonusGame);
//        bool isPatternOutComeSucceed = false;
//        //使用了OutCome 而且没有轮子被固定住 则使用AddOutCome(OutCome outcome)方法上传
//        //                    if(isUsePattern && ReelLocked ==false)
//        if (isUseForcePattern)
//        {
//            if (currentServerOutCome != null)
//            {
//                //使用服务器
//                SpinClickedMsgMgr.Instance.AddOutCome(currentServerOutCome);
//                isPatternOutComeSucceed = true;
//            }
//        }
//
//        if (isPatternOutComeSucceed == false)
//        {
//            SpinClickedMsgMgr.Instance.AddOutCome(m_Controller);
//        }
//    }
//
//    public virtual void CreateBaseResult()
//    {
//        //重置是否使用标识变量及OutCome
//        isUseForcePattern = false;
//        currentServerOutCome = null;
//        
//        //去掉了NeedReCreatResult，原则上都需要create
//
//        //20200120 限制只有付费的情况才用pattern，将来再考虑其他
//        //首先检查服务器Pattern结果是否存在 如果使用了Force或者bbPattern 则不再使用NoWinPattern
//        //首先更新目前正在使用的服务器OutCome
//        if (m_Controller.IsFeeSpin())
//        {
//            SequencePaidResultsManager.ChangeOutComeFromSequencePaidResults();
//            List<List<int>> patternSymbolResult = SequencePaidResultsManager.SymbolResultsFromForceOutcome();
//            if (m_Controller.ValidResultInfoFromServer(patternSymbolResult))
//            {
//                m_Controller.resultContent.ChangePaidResult(patternSymbolResult);
//                isUseForcePattern = true;
//                currentServerOutCome = SequencePaidResultsManager.currentForceOutCome;
//#if UNITY_EDITOR
//                Log.LogLimeColor(m_Controller.resultContent.DebugReelContent(m_Controller.resultContent,
//                    BaseSlotMachineController.Instance.reelManager.symbolMap, 0));
//#endif
//            }
//
//            //如果启用了NoWin和并且不是在测试场景
//            if (!isUseForcePattern && m_Controller.NoWinPatternSwitch && !BaseGameConsole.singletonInstance.isForTestScene)
//            {
//                //计算此次结果的中奖金币数
//                float awardTempValue = AwardResult.CreateAwardDataTemp(m_Controller,
//                    BaseSlotMachineController.Instance.slotMachineConfig.wildAccumulation);
//                List<List<int>> noWinResultFromServer = null;
//                //当用户此次不中奖而且也不触发Respin等Feature 调用NO_WIN PATTERN(切记：对于GoldMine这类使用slot作为bonus的机器，记得将Bonus使用的ReelManager的WhetherRespin方法重写使其返回true)
//                if (awardTempValue < 0.1f)
//                {
//                    //当slotConfig进行了NoWinRequestPossibility配置的时候
//                    if (m_Controller.slotConfig.NoWinRequestPossibility > 0)
//                    {
//                        //判断是否需要进行NoWinPattern的请求 调用该方法会改变currentNoWinOutCome 如果需要进行请求 则返回true
//                        if (!SequencePaidResultsManager.NeedRequestNoWinPattern())
//                        {
//                            //若随机到了 则可从服务器获取NoWinPattern信息
//                            if (UnityEngine.Random.Range(0, 1000) < m_Controller.slotConfig.NoWinRequestPossibility)
//                            {
//                                noWinResultFromServer = SequencePaidResultsManager.SymbolResultsFromNoWinOutcome();
//                                if (m_Controller.ValidResultInfoFromServer(noWinResultFromServer))
//                                {
//                                    m_Controller.resultContent.ChangePaidResult(noWinResultFromServer);
//                                    isUseForcePattern = true;
//                                    currentServerOutCome = SequencePaidResultsManager.currentNoWinOutCome;
//#if UNITY_EDITOR
//                                    Log.Trace(m_Controller.resultContent.DebugReelContent(noWinResultFromServer, m_Controller.symbolMap, 2));
//#endif
//                                }
//                            }
//                        }
//                    }
//                }
//            }
//        }
//
//        //没有用force pattern
//        if (isUseForcePattern == false)
//        {
//            if (m_Controller.reelStrips.selectedResult.selectedFixedResult != null)
//            {
//                //Plist配置设定改变结果值
//                m_Controller.resultContent.ChangePaidResult(m_Controller.reelStrips.selectedResult.selectedFixedResult);
//#if UNITY_EDITOR
//                Log.Trace(m_Controller.resultContent.DebugReelContent(m_Controller.reelStrips.selectedResult.selectedFixedResult, m_Controller.symbolMap, 3));
//#endif
//            }
//            else
//            {
//                //普通的数据处理，需要考虑rule
//                if (NeedRuleCheck())
//                {
//                    this.RuleCheck();
//                }
//                else
//                {
//                    m_Controller.resultContent.CreateRawResult();
//                }
//            }
//        }
//
//        //场景模拟测试
//        if (m_Controller.resultChangeContronller != null)
//        {
//            m_Controller.resultChangeContronller.ChangeResult(m_Controller);
//        }
//    }
//
//    #endregion
  
}