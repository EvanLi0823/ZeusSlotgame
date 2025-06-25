using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Classic{
    public class JackPotScatterGame : ExtraAwardGame
    {
        public int m_MinScatterNum = 3;
        [Header("索引奖励对应是0最小jackpot，根据索引依次递增")]
        public List<JackpotItemRender> jackpotItemRenders;
        protected List<JackPotPrizePool> JackPotPrizePoolInfos = new List<JackPotPrizePool>();
        ReelManager reelManager;
        public override void Init(Dictionary<string, object> infos, GameCallback onGameOver = null)
        {
            base.Init(infos, onGameOver);
            reelManager = GetComponent<ReelManager>();
            InitJackPot(BaseSlotMachineController.Instance.slotMachineConfig);
            if (JackPotPrizePoolInfos.Count > 0)
            {
                this.UpdataUI();
            }
        }
        private void InitJackPot(SlotMachineConfig slotConfig)
        {
            JackPotPrizePoolInfos = slotConfig.JackPotPrizeInfos;
            if (JackPotPrizePoolInfos.Count == 0)
            {
                Utils.Utilities.LogPlistError("JackPotData not config in classicconfig.plist.xml! ");
            }
            for (int i = 0; i < JackPotPrizePoolInfos.Count; i++)
            {
                if (JackPotPrizePoolInfos[i].JackPotIndex >= m_MinScatterNum)
                {
                    JackPotIncreaseMachineConfig increaseConfig = slotConfig.GetJackPotIncreaseConfig(JackPotPrizePoolInfos[i].AwardName);
                    jackpotItemRenders[JackPotPrizePoolInfos[i].JackPotIndex - m_MinScatterNum].SetPrizePoolData(slotConfig, JackPotPrizePoolInfos[i], increaseConfig);
                }
            }
        }
        public void UpdataUI(bool isNeedPlaySound = false)
        {
            for (int i = 0; i < jackpotItemRenders.Count; i++)
            {
                jackpotItemRenders[i].Refresh(isNeedPlaySound);
            }
        }

        public JackPotPrizePool GetSpecialJackPotPrizePool(int ScatterNum)
        {
            JackPotPrizePool info = null;

            for (int i = 0; i < JackPotPrizePoolInfos.Count; i++)
            {
                if (JackPotPrizePoolInfos[i].JackPotIndex == ScatterNum)
                {
                    info = JackPotPrizePoolInfos[i];
                }
            }

            return info;
        }


        public override void OnSpin()
        {
            if (BaseSlotMachineController.Instance.isFreeRun)
            {
                return;
            }
            for (int i = 0; i < JackPotPrizePoolInfos.Count; i++)
            {
                JackPotPrizePoolInfos[i].AddPoolAwardWithBet(BaseSlotMachineController.Instance.currentBetting);
            }
            UpdataUI();
        }

        public override void OnBetChanger(long bet)
        {
            UpdataUI(true);
        }

        public override void OnSpinForTest(long currentBetting)
        {
            for (int i = 0; i < JackPotPrizePoolInfos.Count; i++)
            {
                JackPotPrizePoolInfos[i].AddPoolAwardWithBet(currentBetting);
            }
        }
        public override void InitForTest(Dictionary<string, object> infos, long currentBettting)
        {
            InitJackPot(TestController.Instance.classicMachineConfig);
        }
    }
}
