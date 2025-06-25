using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Classic;
using Core;
using Libs;
using System;

public class MultiMainBoard :GoldsReelManager
{
    public bool hasInit = false;
    public bool IsInMultipleBoardMode { set; get; } = false;
    [SerializeField]
    protected MultiBoards multiBoards;

    [HideInInspector]
    public int MultiBoardNum = 4; //决定棋盘的个数，/最终根据feature来进行更改。在freespin的OnEnterGame或initData时候更改

    private void Test()
    {
        IsInMultipleBoardMode = true;

        SwitchBoard();

        LayOutMultiBoards();
    }

    public MultiBoards GetMultiBoards()
    {
        return multiBoards;
    }

    public void ResetAllSingleBoard()
    {
        if (IsInMultipleBoardMode)
        {
            multiBoards.ClearSingleBoardData();
        }
    }
    #region override methods
    public override void InitReels(SlotMachineConfig slotConfig, GameCallback onStop, GameCallback onStart)
    {
        base.InitReels(slotConfig, onStop, onStart);
        if(multiBoards != null)
        {
            multiBoards.InitBoards(slotConfig, onStop, onStart);
        }

        ResetAllSingleBoard();
        hasInit = true;
        //Test();
    }

    public override bool IsMultiBoard()
    {
        return isFreespinBonus;
    }

    //GameConfig的单个board需要根据第几个来切换对应的config
    public override void ChangeGameConfigs(SymbolMap symbolMap, ReelStripManager reelStrips, GameConfigs gameConfigsTemp, ClassicPaytable payTable = null, LineTable lineTable = null)
    {
        if(hasInit && IsInMultipleBoardMode)
        {
            //根据是否多轮处理数据
            List<BoardStripConfig> boardStripConfigs = this.slotConfig.SubBoardStripConfigList; 
            SwitchBoard();

            LayOutMultiBoards();
            multiBoards.ChangeBoardStripConfigs(boardStripConfigs, gameConfigsTemp, this);

            ////避免freespinCount的次数引起问题
            if (!BaseGameConsole.singletonInstance.isForTestScene)
            {
                this.FreespinCount = 0;
            }
        }
        else
        {
            base.ChangeGameConfigs(symbolMap, reelStrips, gameConfigsTemp, payTable, lineTable);
            SwitchBoard();
        }
    }
    //开始转动
    public override bool StartRun()
    {
        this.PlayMultiRunAudio();

        if (State == GameState.READY)
        {
            if(IsInMultipleBoardMode)
            {
                ResetSingleBoardAward();
                ResetFeature();
                multiBoards.StartSpin();
                this.ParseSpinResult();
            }
            else
            {
                base.StartRun();
            }
        }
        return true;
    }

    public void ResetSingleBoardAward()
    {
        multiBoards.ResetSingleBoardAward();
    }

    public virtual void PlayMultiRunAudio()
    {
        Libs.AudioEntity.Instance.StopAwardSymbolAudio();
        
        if(!this.IsInMultipleBoardMode)
        {
            if(this.isFreespinBonus)
            {
                Libs.AudioEntity.Instance.UnPauseBackGroundAudio(Libs.AudioEntity.audioSpecial);
            }
            else if(!(BonusGame != null && BonusGame.IsBonusGame))
            {
                Libs.AudioEntity.Instance.UnPauseBackGroundAudio(Libs.AudioEntity.audioSpin);
            }
        }else
        {
            if (BaseGameConsole.singletonInstance.IsInLobby()) return;
            Libs.AudioEntity.Instance.UnPauseBackGroundAudio(Libs.AudioEntity.audioSpecial);
        }
    }

    public override void PlayStartRunAudio()
    {

    }

    public override void PauseBackGroundAudio(bool reduce)
    {
        if(!this.IsInMultipleBoardMode)
        {
            base.PauseBackGroundAudio(reduce);
            return;
        }
        if(!reduce) return;
        AudioEntity.Instance.PauseBackGroundAudio(AudioEntity.audioSpecial, 100f, ReduceVolume(AudioEntity.audioSpecial));
    }

    //急停处理
    public override void StopRun()
    {
        if (IsInMultipleBoardMode)
        {
            multiBoards.StopRun();
        }
        else
        {
            base.StopRun();
        }
    }
    //用来spin的btn判断的逻辑,用来判断急停
    public override bool isSpining()
    {
        if (IsInMultipleBoardMode)
        {
            return multiBoards.IsSpining();
        }
        else
        {
            return base.isSpining();
        }
    }
    //奖励判断
    public override void CheckAward(bool wildAccumulation, Dictionary<string, object> infos)
    {
        if(IsInMultipleBoardMode)
        {
            multiBoards.CheckResult(this);
//            Log.LogLimeColor(multiBoards.GetRealRunManagers().Count +":"+ this.resultContent.awardResult.awardValue);
        }
        else
        {
            base.CheckAward(wildAccumulation, infos);
        }

    }

    //动画的elements
    public override void AddAwardElements()
    {
        if (IsInMultipleBoardMode)
        {
            multiBoards.AddAwardElements();
        }
        else
        {
            base.AddAwardElements();
        }
    }
    //判断是否播放动画
    public override bool HasAwardSymbol()
    {
        if(IsInMultipleBoardMode)
        {
            return multiBoards.HasAwardSymbol();
        }
        else
        {
            return base.HasAwardSymbol();
        }
    }

    //播放动画
    public override void PlayAwardSymbolAnimation()
    {
        if (IsInMultipleBoardMode)
        {
            multiBoards.PlayAwardSymbolAnimation();
            
            this.awardLines.Clear();
            foreach (var board in multiBoards.GetRealRunManagers())
            {
                foreach (var line in board.awardLines)
                {
                    awardLines.Add(line);
                }
            }
        }
        else
        {
            base.PlayAwardSymbolAnimation();
        }
    }

    //:动画的停止
    public override void StopAllAnimation()
    {
        if (IsInMultipleBoardMode)
        {
            multiBoards.StopAllAnimation();
        }
        else
        {
            base.StopAllAnimation();
        }

    }
    //多棋盘不做处理
    public override void CheckFeature()
    {
        if (IsInMultipleBoardMode)
        {
            ResetFeature();
        }
        else {
            base.CheckFeature();
        }

    }

    public void ResetFeature()
    {
        HitFs = false;
        FreespinCount = 0;
        HasBonusGame = false;
        this.HasCircleGame = false;
        this.HitLinkGame = false;
    }
    
    public override void CheckHitBonus()
    {
        this.HasBonusGame = false;
        this.BonusNum = 0;
        if (IsInMultipleBoardMode)
        {
            return;
        }
        base.CheckHitBonus();
    }

    //同上,reelmanager中获取freespin symbol的时候会调用到
    public override void CheckHitFS()
    {
        this.HitFs = false;
        this.FreespinCount = 0;
        if (IsInMultipleBoardMode)
        {
            return;
        }
        base.CheckHitFS();
    }

    #endregion
    //mulit board的个数
    private  void LayOutMultiBoards()
    {
        multiBoards.LayOutBoards(this.MultiBoardNum);
    }

    protected virtual void SwitchBoard()
    {
        if (IsInMultipleBoardMode)
        {
            this.transform.parent.gameObject.SetActive(false);
            multiBoards.gameObject.SetActive(true);
        }
        else
        {
            this.transform.parent.gameObject.SetActive(true);
            multiBoards.gameObject.SetActive(false);
        }
    }


    protected List<List<List<int>>> AllFreeSpinResult()
    {
        return multiBoards.AllFreeSpinResult();
    }

    public void PlayFSRetriggerAnimation(Action callBack)
    {
        multiBoards.PlayFsRetriggerAnimation();
        new DelayAction(2f,null, delegate
        {
            callBack(); 
        }).Play();
    }

    #region 为了兼容testcontroller 重载 PreStartState();CreateRawResult();
    public override void PreStartState()
    {
        if (IsInMultipleBoardMode)
        {
            multiBoards.PreStartState();
        }
        else
        {
            base.PreStartState();
        }
    }
    public override void CreateRawResult()
    {
        if (IsInMultipleBoardMode)
        {
            multiBoards.CreateRawResult();
        }
        else
        {
            base.CreateRawResult();
        }
    }
    #endregion

}




