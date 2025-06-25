using System.Collections.Generic;
using Classic;
using Libs;
using MarchingBytes;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

namespace CardSystem
{
    public class CardSystemCollectionDialog : UIDialog,LoopScrollDataSource,LoopScrollPrefabSource
    {
        private BaseCardCollect cardCollect;
        [Header("倾斜角度")]
        public int TiltAngle = 15;
        public TextMeshProUGUI tmp_coin;
        public Button btn_collect;
        public TextMeshProUGUI tmp_info;
        public LoopVerticalScrollRect loopScrollRect;
        public Transform content;
        public GameObject itemPrefab;
        // public LocalizedString tmp_Info_Str;
        public TextMeshProUGUI tmp_cardInfo;
        private string poolName = "CardSystemCardItem";
        private List<BaseCard> Cards;
        protected override void Awake()
        {
            base.Awake();
            Cards = CardSystemManager.Instance.GetCardsInfo();
            string info = OnLineEarningMgr.Instance.GetMoneyStr(CardSystemManager.Instance.GetCurCollectionCoins());
            tmp_coin.text = info;
            PoolResourceManager.Instance.InitPool(poolName,itemPrefab,14);
            loopScrollRect.prefabSource = this;
            loopScrollRect.dataSource = this;
            loopScrollRect.totalCount = Cards.Count;
            loopScrollRect.RefillCells();
        }

        public override void Refresh()
        {
            base.Refresh();
            // int cardCount = CardSystemManager.Instance.GetHaveCardTypeCount();
            // tmp_Info_Str.Arguments = new object[]{cardCount};
            // // Debug.Log("CardSystemCollectionDialog tmp_Info_Str"+tmp_Info_Str.GetLocalizedString());
            // tmp_info.text = tmp_Info_Str.GetLocalizedString();
            tmp_cardInfo.text = $"{CardSystemManager.Instance.GetHaveCardTypeCount()}/{CardSystemManager.Instance.GetTotalCardTypeCount()}";
        }

        public override void Close()
        {
            base.Close();
            CardSystemManager.Instance.ClearNewCardIndex();
        }

        #region LoopScrollRect必需实现
        public void ProvideData(Transform transform, int idx)
        {
            CardUI cardUI = transform.GetComponent<CardUI>();
            BaseCard itemData = Cards[idx];
            //释放之前绑定的数据对象
            cardUI.UpdateData(idx,itemData);
        }

        public GameObject GetObject(int index)
        {
            GameObject go= PoolResourceManager.Instance.GetObjectFromPool(poolName);
            go.AddComponent<CardUI>();
            go.transform.localEulerAngles = new Vector3(0, 0, Random.Range(-TiltAngle, TiltAngle));
            return go;
        }

        public void ReturnObject(Transform trans)
        {
            Destroy(trans.GetComponent<CardUI>());
            PoolResourceManager.Instance.ReturnTransformToPool(trans);
        }
        #endregion
    }
}