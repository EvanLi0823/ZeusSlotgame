using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Libs;
using UnityEngine.UI;


namespace Classic
{
    public class TopPanel : MonoBehaviour
    {
        public CoinsPanel coinsPanel;
        public LevelPanel levelPanel;
        public WithDrawPanel withDrawPanel;
        public MenuPanel menuPanel;
        private long lastBalance;
        private int lastCash;
        private GameObject root;
        public static string CLICK_SETTING_OPEN = "CLICK_SETTING_OPEN";
        public static string CLICK_SETTING_CLOSE = "CLICK_SETTING_CLOSE";
        private bool FlyCoinsPanelCanShow
        {
            get { return gameObject.activeInHierarchy; }
        }

        private bool initOver = false;

        void Awake()
        {
            Messenger.AddListener(GameConstants.OnSceneInit, Init);
            Messenger.AddListener(SlotControllerConstants.OnBlanceChangeForDisPlay, BalanceChange);
            Messenger.AddListener(SlotControllerConstants.OnCashChangeForDisPlay, CashChange);

            Messenger.AddListener<long>(SlotControllerConstants.OnBlanceChange, RefreshBalance);
            Messenger.AddListener(CLICK_SETTING_OPEN, OpenSettingPanel);
            Messenger.AddListener(CLICK_SETTING_CLOSE, CloseSettingPanel);
            Messenger.AddListener(SlotControllerConstants.OnPopLevelChange, OnPopLevelChange);
            Animator[] animators = GetComponentsInChildren<Animator>();
            for (int i = 0; i < animators.Length; i++)
            {
                animators[i].updateMode = AnimatorUpdateMode.UnscaledTime;
            }
        }

        void OnDestroy()
        {
            Messenger.RemoveListener(GameConstants.OnSceneInit, Init);
            Messenger.RemoveListener(CLICK_SETTING_OPEN, OpenSettingPanel);
            Messenger.RemoveListener(CLICK_SETTING_CLOSE, CloseSettingPanel);
            StopAllCoroutines();
            Messenger.RemoveListener(SlotControllerConstants.OnBlanceChangeForDisPlay, BalanceChange);
            Messenger.RemoveListener(SlotControllerConstants.OnCashChangeForDisPlay, CashChange);
            Messenger.RemoveListener<long>(SlotControllerConstants.OnBlanceChange, RefreshBalance);
            Messenger.RemoveListener(SlotControllerConstants.OnPopLevelChange, OnPopLevelChange);
        }

        private void Start()
        {
            Init();
        }

        private void OnEnable()
        {
            Messenger.AddListener(GameConstants.GetTopPanelScaleAdaption, AssignmentProperty);
        }

        private void OnDisable()
        {
            Messenger.RemoveListener(GameConstants.GetTopPanelScaleAdaption, AssignmentProperty);
        }

        public void Init()
        {
            initOver = false;
            if ( BaseGameConsole.singletonInstance.IsInSlotMachine())
            {
                BaseGameConsole.singletonInstance.SlotMachineController.MenuTransform = menuPanel.m_MenuBtn.transform;
                BaseGameConsole.singletonInstance.SlotMachineController.CoinsTransform = coinsPanel.coinTarget;
                BaseGameConsole.singletonInstance.SlotMachineController.CashTransform = withDrawPanel.cashTarget;
            }
            menuPanel.ResetMenuButton();
            lastBalance = -1;
            levelPanel.SetLevel(OnLineEarningMgr.Instance.GetLevel());
            withDrawPanel.InitMoney(OnLineEarningMgr.Instance.Cash());
            this.BalanceChange();
            FlyCoinsPanel.Instance.InitNum(lastBalance);
            initOver = true;
        }

       
        
        void OpenSettingPanel()
        {
        }

        void CloseSettingPanel()
        {
        }
        void OnPopLevelChange()
        {
            levelPanel.SetLevel(OnLineEarningMgr.Instance.GetLevel());
        }

        private void RefreshBalance(long addCoins)
        {
            coinsPanel.SetCoinsNumber(coinsPanel.GetCurrentBalance() + addCoins);
        }

        private void BalanceChange()
        {
            lastBalance = UserManager.GetInstance().UserProfile().Balance();
            coinsPanel.SetCoinsNumber((lastBalance));
            if (FlyCoinsPanelCanShow && initOver) FlyCoinsPanel.Instance.ShowWithCoinsFly(lastBalance);
        }
        private void CashChange()
        {
            lastCash = OnLineEarningMgr.Instance.Cash();
            withDrawPanel.ShowWithCoinsFly(lastCash);
        }
        private float lastValue = -1f;

        private void AssignmentProperty()
        {
            if (transform == null || coinsPanel == null) return;
            FlyCoinsPanel.Instance.topPanelParentScale = transform.parent ? transform.parent : null;
            FlyCoinsPanel.Instance.topPanelRect = (transform as RectTransform);
            FlyCoinsPanel.Instance.GetCurrentCoinsPanelTrans(coinsPanel.transform);
        }

    }
}