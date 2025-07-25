using System;
using System.Collections.Generic;
using UnityEngine;
using Activity;
using CardSystem.Activity;
using TMPro;
using UnityEngine.UI;

namespace CardSystem
{
    public class CardLotteryIcon:BaseIcon
    {
        private Button clickButton;

        private TextMeshProUGUI tmpProgress;

        private Image progressBar;

        private CardLotteryActivity activity;
        // private GameObject handIcon;
        public override void OnInit(int Id, Dictionary<string, object> data)
        {
            base.OnInit(Id, data);
            // 这里可以添加特定的初始化逻辑
            Debug.Log($"CardLotteryIcon Initialized with ID: {Id}");
            activity = ActivityManager.Instance.GetActivityByID(activityId) as CardLotteryActivity;
            if (activity == null)
            {
                Debug.LogError("WithDrawTaskActivity is null, activityId: " + activityId);
                this.OnDestroy();
            }
            RefreshProgress(activity.GetProgress());
        }

        private void Awake()
        {
            clickButton = GetComponent<Button>();
            if (clickButton!=null)
            {
                clickButton.onClick.AddListener(OnButtonClick);
            }
            // if (handIcon == null)
            // {
            //     handIcon = transform.Find("spine_hand")?.gameObject;
            // }
            tmpProgress = Utils.Utilities.RealFindObj<TextMeshProUGUI>(transform, "img_progressbg/tmp_progress");
            progressBar = Utils.Utilities.RealFindObj<Image>(transform, "img_progressbg/img_progressbar");
        }
        public void ShowHand()
        {
            // 显示小手的逻辑
            Debug.Log("Card Lottery Icon Shown");
            // if (handIcon.activeInHierarchy)
            // {
            //     return;
            // }
            // handIcon.SetActive(true);
        }

        public override void RefreshProgress(float progress,string info=null)
        {
            // 刷新进度的逻辑
            Debug.Log("Refreshing Card Lottery Icon Progress"+progress);
            tmpProgress.text = string.Format("{0:0}%", progress * 100);
            progressBar.fillAmount = progress;
        }

        protected override void AddListener()
        {
            // 添加监听器的逻辑
            Debug.Log("Adding listeners for Card Lottery Icon");
        }

        protected override void RemoveListener()
        {
            // 移除监听器的逻辑
            Debug.Log("Removing listeners for Card Lottery Icon");
        }
        
        void OnButtonClick()
        {
            // handIcon.SetActive(false);
            ActivityManager.Instance.OnClickIcon(activityId);
        }
        
        public override void OnDestroy()
        {
            base.OnDestroy();
            Debug.Log("Card Lottery Icon Destroyed");
        }
    }
    
}