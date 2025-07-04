using Libs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CardSystem
{
    public class CardSystemGetCardDialog:UIDialog   
    {
        private int CardId;
        private Button btn_cardPack;
        private Button btn_collect;
        private Transform item;
        
        protected override void Awake()
        {
            base.Awake();
            btn_cardPack = Util.FindObject<Button>(this.transform,"Anchor/btn_cardPack");
            btn_collect = Util.FindObject<Button>(this.transform,"Anchor/btn_collect");
            item = Util.FindObject<Transform>(this.transform,"Anchor/content/item_card");
            if (btn_cardPack!=null)
            {
                UGUIEventListener.Get(btn_cardPack.gameObject).onClick = OnButtonClickHandler;
            }
            if (btn_collect!=null)
            {
                UGUIEventListener.Get(btn_collect.gameObject).onClick = OnButtonClickHandler;
            }
        }

        public void SetCardId(int cardId)
        {
            CardId = cardId;
            ShowCardItem();
        }

        private void ShowCardItem()
        {
            if (item != null)
            {
                // Assuming CardSystemManager has a method to create card item UI
                Transform img_new = Util.FindObject<Transform>(item.transform, "img_new");
                if (img_new != null)
                {
                    img_new.gameObject.SetActive(CardSystemManager.Instance.IsNewCard(CardId));
                }
                int level = CardSystemManager.Instance.GetCardLevel(CardId);
                Transform img_xing = Util.FindObject<Transform>(item.transform, "img_xing");
                //一颗星星不显示
                // if (level > 1)
                // {
                    // 更新星星数量
                    for (int j = 1; j <= level; j++)
                    {
                        GameObject star = Util.FindObject<GameObject>(img_xing.transform, "" + j);
                        star.SetActive(true);
                    }
                // }
                GameObject img_cardbg = Util.FindObject<GameObject>(item.transform, "img_bg/" + level);
                img_cardbg.SetActive(true);
               
                int cardCount = CardSystemManager.Instance.GetCardCount(CardId);
                if (cardCount > 1)
                {
                    Transform img_redPoint = Util.FindObject<Transform>(item.transform, "img_redPoint");
                    TextMeshProUGUI text_count = Util.FindObject<TextMeshProUGUI>(item.transform, "img_redPoint/tmp_count");
                    img_redPoint.gameObject.SetActive(true);
                    text_count.text = cardCount.ToString();
                }
                //加载卡牌图标
                Transform img_card = Util.FindObject<Transform>(item.transform, "img_card");
                img_card.gameObject.SetActive(false);
                LoadCardItem();
                CardSystemManager.Instance.CollectCard(CardId);
            }
            else
            {
                Debug.LogError("Item transform is not set.");
            }
        }
        
        void LoadCardItem()
        {
            string cardName = "Cards/"+CardSystemManager.Instance.GetCardName(CardId);
            AddressableManager.Instance.LoadAsset<Sprite>(cardName, (asset) =>
            {
                if (asset != null)
                {
                    Image img_card = Util.FindObject<Image>(item.transform, "img_card");
                    img_card.sprite = asset;
                    img_card.SetNativeSize();
                    img_card.gameObject.SetActive(true);
                }
                else
                {
                   Debug.LogError($"Failed to load card sprite for CardId: {CardId}, CardName: {cardName}");
                }
            });
        }
        
        public override void OnButtonClickHandler(GameObject go)
        {
            base.OnButtonClickHandler(go);
            if (go == btn_cardPack.gameObject)
            {
                // Handle card pack button click
                Debug.Log("Card Pack button clicked");
                CardSystemManager.Instance.ShowCollectionDialog();
            }
            else if (go == btn_collect.gameObject)
            {
                // Handle collect button click
                this.Close();
            }
        }

        protected override void BtnCloseClick(GameObject closeBtnObject)
        {
            base.BtnCloseClick(closeBtnObject);
            Messenger.Broadcast(CardSystemConstants.RefreshLotteryMsg);
        }

        public override void Close()
        {
            base.Close();
            Messenger.Broadcast(CardSystemConstants.RefreshLotteryMsg);
        }
    }
}