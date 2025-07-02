using System.Collections.Generic;
using Activity;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using Utils;

namespace CardSystem
{
    public class CardPackIcon: BaseIcon
    {
        private Button clickButton;
        private TextMeshProUGUI tmp_info;
        public override void OnInit(int Id, Dictionary<string, object> data)
        {
            base.OnInit(Id, data);
            // 这里可以添加特定的初始化逻辑
            Debug.Log($"CardLotteryIcon Initialized with ID: {Id}");
        }

        private void Awake()
        {
            clickButton = GetComponent<Button>();
            if (clickButton!=null)
            {
                clickButton.onClick.AddListener(OnButtonClick);
            }
            tmp_info = Utilities.RealFindObj<TextMeshProUGUI>(transform,"tmp_bg/tmp_info");
            UpdateCard();
        }
        

        public override void RefreshProgress(float f,string info = null)
        {
            // 刷新进度的逻辑
            Debug.Log("Refreshing Card Lottery Icon Progress");
        }

        protected override void AddListener()
        {
            // 添加监听器的逻辑
            Debug.Log("Adding listeners for Card Lottery Icon");
            Messenger.AddListener(CardSystemConstants.GetCardNewTypeCountMsg,UpdateCard);
        }

        protected override void RemoveListener()
        {
            // 移除监听器的逻辑
            Debug.Log("Removing listeners for Card Lottery Icon");
            Messenger.RemoveListener(CardSystemConstants.GetCardNewTypeCountMsg,UpdateCard);
        }

        void UpdateCard()
        {
            int HasCollectNum = CardSystemManager.Instance.GetHaveCardTypeCount();
            int TargetNum = CardSystemManager.Instance.GetTotalCardTypeCount();
            if (tmp_info != null)
            {
                tmp_info.text = $"{HasCollectNum}/{TargetNum}";
            }
        }
        
        void OnButtonClick()
        {
            ActivityManager.Instance.OnClickIcon(activityId);
        }
        
        public override void OnDestroy()
        {
            base.OnDestroy();
            Debug.Log("Card Lottery Icon Destroyed");
        }
    }
}