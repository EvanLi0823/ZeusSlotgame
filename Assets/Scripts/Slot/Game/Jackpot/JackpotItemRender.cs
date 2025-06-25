using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;
namespace Classic{
    public class JackpotItemRender : MonoBehaviour
    {
        [FormerlySerializedAs("NeedUnlock")]
        public bool UseAnimationControl = true;
        protected readonly string ANI_LOCK = "Lock";
        protected readonly string ANI_UnLOCK = "Unlock";
        protected readonly string ANI_AWARD = "HitAward";
        protected readonly string ANI_Idle = "Idle";
        protected readonly string ANI_ACTIVE = "Active";
        protected readonly string ANI_Awarded = "Awarded";
        protected readonly string AUDIO_JACKPOT_ON = "Jackpot-on";
        protected readonly string AUDIO_JACKPOT_OFF = "Jackpot-off";
        //  public GameObject BackGround;
        public UIIncreaseNumber TxtAwardNum;
        public CommonNumberUpText jackPotCoinsText;
        public NumbertSlideIncrease numbertSlideIncreaseText;
        public Animator m_Animator;

        private JackPotPrizePool PrizePool = null;
        private JackPotIncreaseMachineConfig IncreaseConfig = null;
        private SlotMachineConfig slotConfig;
        private bool m_IsUnlock = false;    //是否解锁状态
        private bool m_IsInHitAward = false; // 在循环的中奖状态
        public void SetPrizePoolData(SlotMachineConfig _slotConfig, JackPotPrizePool prizeInfo, JackPotIncreaseMachineConfig increaseConfig, bool needRefresh = false)
        {
            this.slotConfig = _slotConfig;
            this.PrizePool = prizeInfo;
            this.IncreaseConfig = increaseConfig;
            if (needRefresh)
            {
                this.Refresh();
            }
        }

        public void Refresh(bool isBetChange = false)
        {
            if (this.PrizePool == null||BaseSlotMachineController.Instance==null)
            {
                return;
            }
            long increaseNum = IncreaseConfig != null ? IncreaseConfig.IncreaseEverySeconds : 0;
            if (this.PrizePool.MinBet <= BaseSlotMachineController.Instance.currentBetting)
            {

                if (this.TxtAwardNum!=null)
                {
                    this.TxtAwardNum.SetNumber(PrizePool.GetTotalAward(), increaseNum);
                }
                if (this.jackPotCoinsText!=null)
                {
                    if(this.jackPotCoinsText.gameObject.activeSelf)
                    {
                        this.jackPotCoinsText.SetAutoNumberUpText(PrizePool.GetTotalAward(),(int)increaseNum);
                    }
                }

                if(numbertSlideIncreaseText != null)
                {
                    if(numbertSlideIncreaseText.gameObject.activeSelf)
                    {
                        numbertSlideIncreaseText.SetText(PrizePool.GetTotalAward());
                    }
                }

                if (!m_IsUnlock)
                {
                    if (UseAnimationControl) this.m_Animator.SetTrigger(ANI_UnLOCK);
                    m_IsUnlock = true;
                    if (isBetChange)
                    {
                        Libs.AudioEntity.Instance.PlayEffect(AUDIO_JACKPOT_ON);
                    }
                }
            }
            else
            {
                if (this.TxtAwardNum!=null)
                {
                    this.TxtAwardNum.SetNumber(PrizePool.GetUnlockShowAward(), increaseNum);
                }
                if (this.jackPotCoinsText!=null)
                {
                    if(jackPotCoinsText.gameObject.activeSelf)
                    {
                        jackPotCoinsText.SetAutoNumberUpText(PrizePool.GetUnlockShowAward(),(int)increaseNum);
                    }
                }

                if(numbertSlideIncreaseText != null)
                {
                    if(numbertSlideIncreaseText.gameObject.activeSelf)
                    {
                        numbertSlideIncreaseText.SetText(PrizePool.GetUnlockShowAward());
                    }
                }

                if (m_IsUnlock)
                {
                    if (UseAnimationControl) this.m_Animator.SetTrigger(ANI_LOCK);
                    Messenger.Broadcast<int>(JackPotConstants.HEAD_TIPS_SHOW, this.PrizePool.JackPotIndex);
                    m_IsUnlock = false;
                    if (isBetChange)
                    {
                        Libs.AudioEntity.Instance.PlayEffect(AUDIO_JACKPOT_OFF);
                    }
                }
            }

        }

		private void callBack(SlotMachineConfig _slotConfig, int jackPotIndex, double awardValue)
        {
            if (_slotConfig == null || this.slotConfig == null || this.PrizePool == null)
            {
                return;
            }

            if (_slotConfig.Name().Equals(this.slotConfig.Name()))
            {
                if (jackPotIndex == this.PrizePool.JackPotIndex)
                {
                    Refresh();
                }
            }
        }

        private void hitAward(int jackpotIndex)
        {
            if (PrizePool != null)
            {
                if (this.PrizePool.MinBet <= BaseSlotMachineController.Instance.currentBetting)
                {
                    if (jackpotIndex == this.PrizePool.JackPotIndex)
                    {
                        this.m_IsInHitAward = true;
                        if(UseAnimationControl) this.m_Animator.SetTrigger(ANI_AWARD);
                    }
                }
            }
        }

        private void resetNormal()
        {
            if (m_IsInHitAward)
            {
                if (UseAnimationControl) this.m_Animator.SetTrigger(ANI_Idle);
            }
        }

        void Awake()
        {
			Messenger.AddListener<SlotMachineConfig, int, double>(JackPotManager.JACKPOT_DATA_MACHINE_CHANGE, callBack);
            Messenger.AddListener<int>(JackPotConstants.SCATTER_HIT_AWARD, hitAward);
            Messenger.AddListener(JackPotConstants.RESET_NORMAL, resetNormal);
        }

        void OnDestroy()
        {
			Messenger.RemoveListener<SlotMachineConfig, int, double>(JackPotManager.JACKPOT_DATA_MACHINE_CHANGE, callBack);
            Messenger.RemoveListener<int>(JackPotConstants.SCATTER_HIT_AWARD, hitAward);
            Messenger.RemoveListener(JackPotConstants.RESET_NORMAL, resetNormal);
        }
    }
}

