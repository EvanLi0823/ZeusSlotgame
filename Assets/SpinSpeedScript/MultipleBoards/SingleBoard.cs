using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Classic;
using Libs;
using UnityEngine.UI;
using Utils;
//包括动画，获得的奖励，把原有ReelManager中的东西copy过来
public class SingleBoard : GoldsReelManager
{
    [HideInInspector]
    public double WinCoins;

    public UIChangeNumber m_WinTxt;

    protected override void SetSingleBoard(bool _isSingleBoard)
    {
        base.SetSingleBoard(true);
    }

    //暂时认为不需要判断anti
    public override void CheckFeature()
    {
    }
    public override void CheckHitFS()
    {

    }

    public override void PlayStartRunAudio()
    {
    }

    public override void PauseBackGroundAudio(bool reduce)
    {

    }
    
    public void ClearWinCoins()
    {
        this.WinCoins = 0;
        if (m_WinTxt != null)
        {
            m_WinTxt.SetInitNumber(0);
        }
    }

    public void PlayWinCoinsAnimation()
    {
        if (m_WinTxt != null)
        {
            m_WinTxt.SetNumber((long)WinCoins);
        }
    }
    
    public int GetBonusNum()
    {
        Dictionary<int, List<int>> bonusSymbolIndex = GetSpecialSymbolIndex(SymbolMap.IS_BONUS);
        int num = Utilities.GetCountInDictList(bonusSymbolIndex);
        return num;
    }

    public bool IsScatterMoreThree()
    {
        Dictionary<int, List<int>> bonusSymbolIndex = GetSpecialSymbolIndex(SymbolMap.IS_FREESPIN);
        int num = Utilities.GetCountInDictList(bonusSymbolIndex);

        return num >= 3;
    }

    public void PlayFsRetriggerAnimation()
    {
        if (IsScatterMoreThree())
        {
            List<BaseElementPanel> list = GetElementsWithSymbolTag (SymbolMap.IS_FREESPIN);
            foreach (var baseElementPanel in list)
            {
                awardElements.Add(baseElementPanel);
                baseElementPanel.PlayAnimation((int) BaseElementPanel.AnimationID.AnimationID_BonusTriggered);
            }
            Libs.AudioEntity.Instance.PlayBonusTriggerEffect ();
        }
    }
//暂时屏蔽假带子对多棋盘的影响 待Free假带子和多棋盘方案明确再复写
    protected override List<List<int>> GetLayOutReelShowData()
    {
        return this.resultContent.ReelSpinShowData;
    }

    protected override List<List<int>> GetShowData()
    {
        List<List<int>> data = new List<List<int>>();
        for (int i = 0; i < resultContent.ReelResults.Count; i++)
        {
            data.Add(resultContent.ReelResults[i].ShowIndexs);
        }
        return data;
    }

   
}
