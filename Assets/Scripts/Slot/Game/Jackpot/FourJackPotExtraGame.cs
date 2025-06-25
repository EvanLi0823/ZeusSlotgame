using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Classic;
public class FourJackPotExtraGame : ExtraAwardGame
{
    protected ReelManager m_ReelManager;

    public FourJackpotRenders m_FourJackpotRenders;

    protected override void Awake()
    {
        m_ReelManager = GetComponent<ReelManager>();
    }
    public override void Init(Dictionary<string, object> infos = null, GameCallback onGameOver = null)
    {
        base.Init(infos, onGameOver);

        m_FourJackpotRenders.InitJackpotData(m_ReelManager.slotConfig);
        m_FourJackpotRenders.JackpotNumRefreshDirect();
    }

    public override void InitForTest(Dictionary<string, object> infos, long currentBettting)
    {
        base.InitForTest(infos, currentBettting);
        m_FourJackpotRenders.InitJackpotData(m_ReelManager.slotConfig);
    }

    public override void OnSpin()
    {
        base.OnSpin();

        if (BaseSlotMachineController.Instance.isFreeRun)
        {
            return;
        }

        m_FourJackpotRenders.AddGrandWithBet();
        m_FourJackpotRenders.AddMajorWithBet();

        m_FourJackpotRenders.JackpotNumRefreshTween();
    }

    public override void OnSpinForTest(long currentBetting)
    {
        base.OnSpinForTest(currentBetting);
        if (m_ReelManager.isFreespinBonus || TestController.Instance.isOneMore)
        {
            return;
        }

        m_FourJackpotRenders.AddGrandWithBet();
        m_FourJackpotRenders.AddMajorWithBet();
    }

    public override void OnBetChanger(long bet)
    {
        base.OnBetChanger(bet);
        m_FourJackpotRenders.JackpotNumRefreshDirect();
    }

    public override void OnGameEnd()
    {
        m_FourJackpotRenders.JackpotNumRefreshDirect();
        base.OnGameEnd();
    }

    public override void ForExtraGameTest(ResultContent resultContent, ReelManager gamePanel)
    {
        base.ForExtraGameTest(resultContent, gamePanel);
        GetAwardResult();

    }

}
