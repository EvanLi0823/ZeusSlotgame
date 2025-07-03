using System;
using System.Collections;
using System.Collections.Generic;
using Classic;
using Core;
using Libs;
using Plugins;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class AdditionalPanel : MonoBehaviour
{
    private const string H5Config = "H5Config";
    private const string OpenKey = "IsOpen";
    private const string TimeIntervalKey= "TimeInterval";

    private const string localResPath = "";
    private Button H5MoreGift;
    private Button H5MoreGame;
    private Button H5MoreGash;
    private Button H5MoreBonus;
    private Button H5MoreBet;
    public List<Button> h5Buttons = new List<Button>();
    //当前展示的H5按钮序号
    private int curShowIndex = -1;
    public SkeletonGraphic qipao;
    //当前展示的h5按钮时间间隔
    private int showInterval = 0;
    private bool isOpen = false;
    private long lastTime = 0;
    private bool needShowQiPao = false;
    private Coroutine showQiPaoCor;
    private bool isClicked = false;
    private void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Button btn = transform.GetChild(i).GetComponent<Button>();
            if (btn != null)
            {
                UGUIEventListener.Get(btn.gameObject).onClick = this.OnH5Click;
                btn.gameObject.SetActive(false);
                h5Buttons.Add(btn);
            }
        }
        Messenger.AddListener<bool>(GameConstants.OnH5InitSuccess, UpdataH5);
        Messenger.AddListener(GameConstants.OnExitH5, OnExitH5);
    }

    void UpdataH5(bool show = false)
    {
        if (h5Buttons==null || h5Buttons.Count==0)
        {
            return;
        }
        if (curShowIndex ==-1)
        {
            return;
        }
        h5Buttons[curShowIndex].gameObject.SetActive(show);
        if (show)
        {
            Debug.Log("AdditionalPanel ShowQiPao");
            NeedShowQiPao();
        }
    }

    private void NeedShowQiPao()
    {
        // needShowQiPao = true;
        // DestroyShowQiPao();
        qipao.Skeleton.SetSlotsToSetupPose();
        qipao.Initialize(true);
        qipao.AnimationState.SetAnimation(0,"animation",true);
        qipao.gameObject.SetActive(true);
    }

    private void DestroyShowQiPao()
    {
        // if (showQiPaoCor!=null)
        // {
        //     StopCoroutine(showQiPaoCor);
        //     showQiPaoCor = null;
        // }
        qipao.gameObject.SetActive(false);
    }
    
    //开启协程，显示气泡
    // IEnumerator ShowQiPao()
    // {
    //     while (needShowQiPao)
    //     {
    //         //qipao.gameObject.SetActive(true);
    //         
    //         //等待两秒
    //         qipao.gameObject.SetActive(true);
    //         qipao.AnimationState.ClearTracks();
    //         qipao.Skeleton.SetSlotsToSetupPose();
    //         qipao.Initialize(true);
    //         qipao.AnimationState.SetAnimation(0,"animation",false);
    //         
    //     }
    //     qipao.gameObject.SetActive(false);
    // }


    // Start is called before the first frame update
    private void Start()
    {
        //获取配置文件
        Dictionary<string, object> dict = Configuration.GetInstance()
            .GetValue<Dictionary<string, object>>(H5Config, null);
        if (dict==null)
        {
            Debug.LogError("H5Config is Null,Please Check the Plist");
            return;
        }

        isOpen = Utils.Utilities.GetValue(dict, OpenKey, false);
        if (!isOpen)
        {
            return;
        }
        
        showInterval = Utils.Utilities.GetValue(dict, TimeIntervalKey, 0);
        curShowIndex = 0;
        Init();
    }
    public void Init()
    {
        bool canShow = OnLineEarningMgr.Instance.CanShowH5() && isOpen;
        UpdataH5(canShow);
    }

    private void OnDestroy()
    {
        if (timeCor!=null)
        {
            StopCoroutine(timeCor);
            timeCor = null;
        }
        Messenger.RemoveListener<bool>(GameConstants.OnH5InitSuccess, UpdataH5);
        Messenger.RemoveListener(GameConstants.OnExitH5, OnExitH5);
    }
    
    
    private void OnH5Click(GameObject go)
    {
        if (isClicked)
        {
            return;
        }
        isClicked = true;
#if UNITY_ANDROID
         SkySreenUtils.CurrentOrientation = ScreenOrientation.Portrait;
#endif
        int cash = OnLineEarningMgr.Instance.Cash();
        string str = ""+OnLineEarningMgr.Instance.ConvertMoneyToDouble(cash,needExchange:false);
#if UNITY_EDITOR || DEBUG
        Dictionary<string, object> data = new Dictionary<string, object>();
        data["amount"] = 3.5f;
        string mockstr = MiniJSON.Json.Serialize(data);
        PlatformManager.Instance.H5AddCash(mockstr);
        PlatformManager.Instance.H5State("");
#else
        if (OnLineEarningMgr.Instance.isThreeHundredOpen())
        {
            int h5Reward = OnLineEarningMgr.Instance.GetH5Reward();
            string str1 = ""+OnLineEarningMgr.Instance.ConvertMoneyToDouble(h5Reward,needExchange:false);
            PlatformManager.Instance.SendMsgToPlatFormByType(MessageType.ShowPromotion,str,str1);
        }else
        {
            PlatformManager.Instance.SendMsgToPlatFormByType(MessageType.ShowPromotion,str);
        }
#endif
    }

    private Coroutine timeCor = null;

    //从H5页面退出
    private void OnExitH5()
    {
        if (!isClicked)
        {
            return;
        }
        isClicked = false;
        //隐藏当前显示的按钮
        h5Buttons[curShowIndex].gameObject.SetActive(false);
        //重置计数
        lastTime = 0;
        //序号自增，展示下一个
        curShowIndex = (curShowIndex==h5Buttons.Count - 1) ? 0 : curShowIndex + 1;
        //销毁气泡展示协程
        needShowQiPao = false;
        DestroyShowQiPao();
        if (timeCor!=null)
        {
            StopCoroutine(timeCor);
            timeCor = null;
        }
        timeCor = StartCoroutine(StartCalculateTime());
    }

    IEnumerator StartCalculateTime()
    {
        while (lastTime<=showInterval)
        {
            yield return new WaitForSecondsRealtime(1f);
            lastTime += 1;
        }
        
        h5Buttons[curShowIndex].gameObject.SetActive(true);
        NeedShowQiPao();
        StopCoroutine(timeCor);
        timeCor = null;
        lastTime = 0;
    }
}
