using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Classic;
using Core;
using System;

public class MultiBoards : MonoBehaviour
{
    [SerializeField]
    public VectorPosition boardPositions;
    //private int BoardCount { set; get; } = 2;
    [Header("所有的棋盘")]
    public List<SingleBoard> multiReels = new List<SingleBoard>();

    protected List<SingleBoard> actualBoards = new List<SingleBoard>();

    protected int MultiBoardNum;

    public List<SingleBoard> GetRealRunManagers()
    {
        return actualBoards;
    }

    public void ClearSingleBoardData()
    {
        foreach (SingleBoard singleBoard in multiReels)
        {
            singleBoard.ClearWinCoins();
        }
    }
    
    public void ResetSingleBoardAward()
    {
        foreach (SingleBoard singleBoard in multiReels)
        {
            singleBoard.resultContent.awardResult.Reset();
        }
    }

    public virtual void LayOutBoards(int num)
    {
        MultiBoardNum = num;
        List<Vector3> vectors = GetBoardCoordnates();
        actualBoards.Clear();
        for (int i=0; i < multiReels.Count;i++)
        {
            if(i<num)
            {
                actualBoards.Add(multiReels[i]);

                multiReels[i].transform.parent.gameObject.SetActive(true);
                multiReels[i].transform.parent.localPosition = vectors[i];
            }
            else
            {
                multiReels[i].transform.parent.gameObject.SetActive(false);
            }
        }
    }

    public virtual void InitBoards(SlotMachineConfig slotConfig, GameCallback onStop = null, GameCallback onStart = null)
    {
        List<SingleBoard> reelManagers = multiReels;//实际的个数
        //只需要第一个有回调，别的都不需要有
        if(reelManagers.Count < 2)
        {
            return;
        }

        reelManagers[0].InitReels(slotConfig, onStop, onStart);

        for(int i=1; i< reelManagers.Count;i++)
        {
            reelManagers[i].InitReels(slotConfig,null,null);
        }
    }
    //change gameConfig，即进入game游戏

    public virtual void ChangeBoardStripConfigs(List<BoardStripConfig> boardStripConfigs, GameConfigs gameConfigsTemp, ReelManager reelManager)
    {
        List<SingleBoard> reelManagers = GetRealRunManagers();
        for (int i = 0; i < reelManagers.Count; i++)
        {
            if (i<boardStripConfigs.Count)
            {
                BoardStripConfig config = boardStripConfigs[i];
                reelManagers[i].ChangeGameConfigs(config.SymbolMap, config.BoardStrips, gameConfigsTemp, config.PayTable, config.LineTable());
            }
        }
    }

    //也可根据需求用主plist的数据
    public  void ChangeGameConfigs(SymbolMap symbolMap, ReelStripManager reelStrips, GameConfigs gameConfigsTemp, ClassicPaytable payTable = null, LineTable lineTable = null)
    {
        List<SingleBoard> reelManagers = GetRealRunManagers();
        for (int i = 0; i < reelManagers.Count; i++)
        {
            reelManagers[i].ChangeGameConfigs(symbolMap, reelStrips, gameConfigsTemp, payTable, lineTable);
        }
    }

    //开始spin
    public void StartSpin()
    {
        List<SingleBoard> reelManagers = GetRealRunManagers();
        for (int i = 0; i < reelManagers.Count; i++)
        {
            reelManagers[i].StartRun();
        }
    }

    //急停
    public void StopRun()
    {
        List<SingleBoard> reelManagers = GetRealRunManagers();
        for(int i =0; i< reelManagers.Count; i++)
        {
            reelManagers[i].StopRun();
        }
    }
    //是否在spin中，只需要判断第一个即可
    public virtual bool IsSpining()
    {
        List<SingleBoard> reelManagers = GetRealRunManagers();
        if(reelManagers.Count <1)
        {
            return false;
        }
        return reelManagers[0].isSpining();
    }

    public void CheckResult(ReelManager reelManager)
    {
        reelManager.resultContent.awardResult.awardValue = 0;
        reelManager.resultContent.awardResult.awardInfos.Clear();
        List<SingleBoard> reelManagers = GetRealRunManagers();
        for (int i = 0; i < reelManagers.Count; i++)
        {
            reelManagers[i].CheckAward(reelManagers[i].slotConfig.wildAccumulation, null);
            double v = reelManagers[i].resultContent.awardResult.awardValue;
            reelManager.resultContent.awardResult.awardValue += v ;
            
            //只是用来显示用
            reelManagers[i].WinCoins += v * SlotsBet.GetCurrentBet()/ reelManager.gameConfigs.ScalFactor;
            reelManager.resultContent.awardResult.awardInfos.AddRange(reelManagers[i].resultContent.awardResult.awardInfos);
        }
    }

    public void AddAwardElements()
    {
        List<SingleBoard> reelManagers = GetRealRunManagers();
        for (int i = 0; i < reelManagers.Count; i++)
        {
            reelManagers[i].AddAwardElements();
        }
    }

    public void PlayAwardSymbolAnimation()
    {
        List<SingleBoard> reelManagers = GetRealRunManagers();
        for (int i = 0; i < reelManagers.Count; i++)
        {
            reelManagers[i].PlayAwardSymbolAnimation();
            reelManagers[i].PlayWinCoinsAnimation();
        }
    }
    //判断播放中奖
    public bool HasAwardSymbol()
    {
        List<SingleBoard> reelManagers = GetRealRunManagers();
        for (int i = 0; i < reelManagers.Count; i++)
        {
            if(reelManagers[i].HasAwardSymbol())
            {
                return true;
            }
        }
        return false;
    }

    public void StopAllAnimation()
    {
        List<SingleBoard> reelManagers = GetRealRunManagers();
        for (int i = 0; i < reelManagers.Count; i++)
        {
            reelManagers[i].StopAllAnimation();
        }
    }

    public void PlayFsRetriggerAnimation()
    {
        List<SingleBoard> reelManagers = GetRealRunManagers();
        reelManagers.ForEach(delegate(SingleBoard board) { board.PlayFsRetriggerAnimation(); });
    }

    protected virtual List<Vector3> GetBoardCoordnates()
    {
        if (this.MultiBoardNum == 2)
        {
            return boardPositions.boardVectors2;
        }
        else if (this.MultiBoardNum == 3)
        {
            return boardPositions.boardVectors3;
        }
        else if (this.MultiBoardNum == 4)
        {
            return boardPositions.boardVectors4;
        }
        return new List<Vector3>();
    }

    public List<List<List<int>>> AllFreeSpinResult()
    {
        List<List<List<int>>> result = new List<List<List<int>>>();
         List<SingleBoard> reelManagers = GetRealRunManagers();
        for(int i= 0; i <reelManagers.Count; i++)
        {
            result.Add(reelManagers[i].baseGameResult);
        }
        return result;
    }

    public void PreStartState()
    {
        List<SingleBoard> reelManagers = GetRealRunManagers();
        for (int i = 0; i < reelManagers.Count; i++)
        {
            reelManagers[i].PreStartState();
        }
    }

    public void CreateRawResult()
    {
        List<SingleBoard> reelManagers = GetRealRunManagers();
        for (int i = 0; i < reelManagers.Count; i++)
        {
            reelManagers[i].CreateRawResult();
        }
    }

    public List<double> GetMultiBoardWinCoins()
    {
        List<Double> result = new List<Double>();
        List<SingleBoard> reelManagers = GetRealRunManagers();
        for (int i = 0; i < reelManagers.Count; i++)
        {
            result.Add( reelManagers[i].WinCoins);
        }

        return result;
    }

    public void RecoveryBoardWinCoins(List<Double> list)
    {
        if (list == null || list.Count == 0)
        {
            return;
        }
        
        List<SingleBoard> reelManagers = GetRealRunManagers();

        int minLength = Mathf.Min(list.Count, reelManagers.Count);
        for (int i = 0; i < minLength; i++)
        {
            reelManagers[i].WinCoins = list[i];
            reelManagers[i].PlayWinCoinsAnimation();
        }
    }

}
[Serializable]
public class VectorPosition
{
    public List<Vector3> boardVectors2 = new List<Vector3>();
    public List<Vector3> boardVectors3 = new List<Vector3>();
    public List<Vector3> boardVectors4 = new List<Vector3>();
}