using System;
using System.Collections.Generic;
using Classic;
using DG.Tweening;
using Libs;
using RealYou.Utility.Message;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LuckyCashDialog : UIDialog
{
    private TextMeshProUGUI TMP_Money;
    private Button BtnWatch;
    private Button BtnNotWatch;
    public Transform cashFlyPosition;

    //PopRewardState exit
    private Action endStateCallBack;
    private int cash;
    private int cashAdd;
    private int totalCash;
   

    private bool isPlayAd;
    //是否选择过按钮，要么点击关闭看全屏广告，要么点击看激励广告
    private bool HasClicked = false;
    private bool closeShowAd = false;
    protected override void Awake()
    {
        AudioManager.Instance.AsyncPlayEffectAudio("Bonus_win");
        TMP_Money = Util.FindObject<TextMeshProUGUI>(transform, "Anchor/Animation/TMP_Money");
        BtnWatch = Util.FindObject<Button>(transform, "Anchor/Animation/BtnWatch");
        BtnNotWatch= Util.FindObject<Button>(transform, "Anchor/Animation/BtnNoWatch");
        BtnWatch.onClick.AddListener(OnWatchADButtonClick);
        BtnNotWatch.onClick.AddListener(OnButtonCloseClick);
        PlatformManager.Instance.SendMsgToPlatFormByType(MessageType.BuryPoint,"LuckyCash");
        base.Awake();
        
    }

    protected override void Start()
    {
        base.Start();
        DelayShowClaimBtn();
    }

    void DelayShowClaimBtn()
    {
        if (BtnNotWatch!=null)
        {
            BtnNotWatch.enabled = false;
            BtnNotWatch.gameObject.SetActive(false);
            BtnNotWatch.transform.localScale = Vector3.zero;
            new DelayAction(1f, null, () =>
            {
                BtnNotWatch.gameObject.SetActive(true);
                BtnNotWatch.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack).OnComplete(() =>
                {
                    BtnNotWatch.enabled = true;
                });
            }).Play();
        }
    }
    
    void OnEnable()
    {
        Messenger.AddListener<int>(ADConstants.PlayLuckyCashAD,AdIsPlaySuccessful);
    }
    void OnDisable()
    {
        Messenger.RemoveListener<int>(ADConstants.PlayLuckyCashAD,AdIsPlaySuccessful);
    }
    
    public void SetUIData(int money)
    {
        this.totalCash = money;
        CashRollUp();
    }

    private Tween Cashtween = null;
    private int curCash = 0;
    public float time = 2.3f;
    void CashRollUp()
    {
        //金币滚动
        new DelayAction(0.4f, null,() =>
        {
            Cashtween = Utils.Utilities.AnimationTo(curCash, totalCash, time, SetCashCoins, null, () =>
            {
                SetCashCoins(totalCash);
                Cashtween = null;
            });
        }).Play();
    }
    
    public void OnClickStopUpdate()
    {
        AudioEntity.Instance.StopRollingUpEffect();
        if (Cashtween!=null)
        {
            Cashtween.Kill(true);
            this.SetCashCoins(totalCash);
        }
    }
    
    private void SetCashCoins(int cash)
    {
        this.curCash = cash;
        this.TMP_Money.SetText(OnLineEarningMgr.Instance.GetMoneyStr(cash));
    }
    
    void AdIsPlaySuccessful(int type)
    {
        Debug.Log("LuckyCashDialog  AdIsPlaySuccessful  type========="+type);
        SetCoins();
    }

    //激励广告
    public void OnWatchADButtonClick()
    {
        if (!BtnWatch.enabled)
        {
            return;
        }
        BtnWatch.enabled = false;
        if (HasClicked)
        {
            return;
        }
        HasClicked = true;
        if (isPlayAd)
        {
            return;
        }
        isPlayAd = true;
        OnClickStopUpdate();
        totalCash *= ADManager.Instance.GetADRewardMultiple(ADEntrances.REWARD_VIDEO_ENTRANCE_LUCKYCASH);
        bool rewardADIsReady = ADManager.Instance.RewardAdIsOk(ADEntrances.REWARD_VIDEO_ENTRANCE_LUCKYCASH);
        //广告未加载好
        if (!rewardADIsReady)
        {
            //展示未加载好广告的提示,直接给看广告成功的奖励
            ADManager.Instance.ShowLoadingADsUI(endCallBack:this.SetCoins);
        }
        else
        {
            ADManager.Instance.PlayRewardVideo(ADEntrances.REWARD_VIDEO_ENTRANCE_LUCKYCASH);
        }
    }

    private void OnButtonCloseClick()
    {
        if (!BtnNotWatch.enabled)
        {
            return;
        }
        BtnNotWatch.enabled = false;
        if (HasClicked)
        {
            return;
        }
        HasClicked = true;
        OnClickStopUpdate();
        // totalCash = (int)(totalCash * 0.2f);
        //只有通过 claimX20%点击关闭的弹窗才计入插屏广告的累计次数
        // OnLineEarningMgr.Instance.AddLuckyADNum();
        // if (OnLineEarningMgr.Instance.CheckCanPopLuckyAD())
        // {
        //     //加钱之后重置计数
        //     OnLineEarningMgr.Instance.ResetLuckyADNum();
            bool interstitialADIsReady = ADManager.Instance.InterstitialAdIsOk(ADEntrances.Interstitial_Entrance_CLOSELUCKYCASH);
            //广告未加载好
            if (!interstitialADIsReady)
            {
                //展示未加载好广告的提示,直接给未看广告的奖励
                ADManager.Instance.ShowLoadingADsUI(endCallBack:this.SetCoins);
            }
            else
            {
                //播放广告
                Messenger.Broadcast(ADEntrances.Interstitial_Entrance_CLOSELUCKYCASH);
            }
        // }
        // else
        // {
        //     //直接加 20% 的钱
        //     SetCoins();
        // }
    }
    
    public void SetCoins()
    {
        //加钱动画播放完毕
        OnLineEarningMgr.Instance.IncreaseCash(totalCash);
        PlatformManager.Instance.SendMsgToPlatFormByType(MessageType.UpdateLevel,OnLineEarningMgr.Instance.GetCashTime());
        Messenger.Broadcast<Transform, Libs.CoinsBezier.BezierType, System.Action, CoinsBezier.BezierObjectType>(
            GameConstants.CollectBonusWithType, cashFlyPosition, Libs.CoinsBezier.BezierType.DailyBonus, null,
            CoinsBezier.BezierObjectType.Cash);
        Messenger.Broadcast(SlotControllerConstants.OnCashChangeForDisPlay);
        Libs.AudioEntity.Instance.StopAllEffect();
        Libs.AudioEntity.Instance.PlayCoinCollectionEffect();
        Messenger.Broadcast(SlotControllerConstants.AUTO_SPIN_RESUME);
        new DelayAction(0.8f,null, () =>
        {
            this.Close();
            Messenger.Broadcast(GameConstants.SHOW_WITH_DRAW_TIPS_PANEL);
        }).Play();
    }
}