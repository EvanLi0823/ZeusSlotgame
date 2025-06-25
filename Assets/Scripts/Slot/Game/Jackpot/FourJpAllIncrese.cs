namespace Classic
{
    public class FourJpAllIncrese :FourJackPotExtraGame
    {
        public override void OnSpin()
        {
            if (BaseSlotMachineController.Instance.isFreeRun)
            {
                return;
            }

            m_FourJackpotRenders.AddGrandWithBet();
            m_FourJackpotRenders.AddMajorWithBet();
            m_FourJackpotRenders.AddMinorWithBet();
            m_FourJackpotRenders.AddMiniWithBet();
            m_FourJackpotRenders.JackpotNumRefreshTween();
            
        }

        public override void OnSpinForTest(long currentBetting)
        {
            if (m_ReelManager.isFreespinBonus || TestController.Instance.isOneMore)
            {
                return;
            }

            m_FourJackpotRenders.AddGrandWithBet();
            m_FourJackpotRenders.AddMajorWithBet();
            m_FourJackpotRenders.AddMinorWithBet();
            m_FourJackpotRenders.AddMiniWithBet();
        }
    }
}