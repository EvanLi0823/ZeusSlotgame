using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Activity;
using Libs;
using MarchingBytes;
using Plugins;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;
using Utils;




public class WithDrawDialog : UIDialog
{
    public TextMeshProUGUI money;
    public ToggleGroup panelToggleGroup;
    public Toggle redeemToggle, recordToggle;

    public GameObject panelRedeem;
    public GameObject panelRecord;

    private WithDrawRedeemPanelItem redeemPanelItem;
    private WithDrawRecordPanelItem recordPanelItem;
    public TextMeshProUGUI taskInfo;
    public override void Refresh()
    {
        base.Refresh();
        UpdateCashNum();
        WithDrawTaskActivity withDrawTaskActivity = ActivityManager.Instance.GetActivityByType(ActivityType.WithDrawTask) as WithDrawTaskActivity;
        if (withDrawTaskActivity!=null)
        {
            taskInfo.text = withDrawTaskActivity.GetIconTaskInfo();
        }
    }

    protected override void Awake()
    {
        base.Awake();
        redeemToggle.onValueChanged.AddListener(OnPanelToggleChanged);
        recordToggle.onValueChanged.AddListener(OnPanelToggleChanged);
        
        if (panelRedeem!=null)
        {
            redeemPanelItem = panelRedeem.AddComponent<WithDrawRedeemPanelItem>();
        }
        if (panelRecord!=null)
        {
            recordPanelItem = panelRecord.AddComponent<WithDrawRecordPanelItem>();
        }
    }

    protected override void Start()
    {
        base.Start();
        //默认展示第一个
        redeemToggle.isOn = true;
    }

    protected override void OnEnable()
    {
        Messenger.AddListener(GameDialogManager.CloseWithDrawDialog,Close);
        Messenger.AddListener(SlotControllerConstants.OnCashChangeForDisPlay,UpdateCashNum);

    }

    protected override void OnDisable()
    {
        Messenger.RemoveListener(GameDialogManager.CloseWithDrawDialog,Close);
        Messenger.RemoveListener(SlotControllerConstants.OnCashChangeForDisPlay,UpdateCashNum);
    }

    public void UpdateCashNum()
    {
        money.text = OnLineEarningMgr.Instance.GetMoneyStr(OnLineEarningMgr.Instance.Cash(),needIcon:false);
    }

    void OnPanelToggleChanged(bool value)
    {
        Toggle selected = panelToggleGroup.ActiveToggles().FirstOrDefault();
        if (selected != null)
        {
            if (selected == redeemToggle)
            {
                redeemPanelItem.Show();
                recordPanelItem.Hide();
            }else if (selected == recordToggle)
            {
                redeemPanelItem.Hide();
                recordPanelItem.Show();
            }
        }
    }
}
