using System.Collections.Generic;
using System.Linq;
using Classic;
using UnityEngine;
using Utils;

namespace System
{
    public class WithDrawManager
    {
        private const string ConfigKey = "WithDrawPanelConfig";
        private const string ItemKey = "ItemConfig";

        private const string PlatformKey = "Platform";
        //用于区分当前点击的是哪一个任务绑定的UI
        public int CurSelectTaskId = 0;
        public bool haveClickShowAccount = false;
        //提现的公共冷却时间
        private int coolTime = 0;
        public static WithDrawManager Instance{
            get{ 
                return Singleton<WithDrawManager>.Instance;
            }
        }
        //提现任务数据
        private Dictionary<string, List<RedeemItemData>> redeemItemDict = new Dictionary<string, List<RedeemItemData>>();
        //提现记录数据
        private List<RecordItemData> recordItemDict = new List<RecordItemData>();

        // private List<RedeemItemData> redeemItemList = new List<RedeemItemData>();
        private bool isConfigReady = false;
        private int PlatFormIndex = -1;
      
        private WithDrawManager() { }
        
        //初始化
        public void OnInit()
        {
            ParseConfig();
        }

        public void ParseConfig()
        {
            Dictionary<string,object> config = Plugins.Configuration.GetInstance().GetValue<Dictionary<string,object>>(ConfigKey,null);
            if (config==null || config.Count == 0)
            {
                Debug.LogError("WithDrawDialog ParseData config is null");
                return;
            }
            coolTime  = Utilities.GetInt(config,WithDrawConstants.CoolTimeKey,0);
            Dictionary<string,object> itemConfigs = Utilities.GetValue<Dictionary<string,object>>(config,ItemKey,null);
            if (itemConfigs==null || itemConfigs.Count == 0)
            {
                Debug.LogError("WithDrawDialog ParseData itemConfig is null");
                return;
            }
            
            List<int> platFormSpriteIndex = LocalizationManager.Instance.GetPlatFormSpriteIndex();
            if (platFormSpriteIndex == null || platFormSpriteIndex.Count == 0)
            {
                Debug.LogError("WithDrawDialog ParseData platFormSpriteIndex is null");
                return;
            }
            //根据平台数截取配置数组
            int count = Math.Min(itemConfigs.Count, platFormSpriteIndex.Count);
            for (int i = 0; i < count; i++)
            {
                string newKey = PlatformKey + i;
                List<object> items = Utilities.GetValue<List<object>>(itemConfigs,newKey,null);
                if (items==null || items.Count == 0)
                {
                    Debug.LogError("WithDrawDialog ParseData  is null");
                    return;
                }

                List<RedeemItemData> redeemItemList = new List<RedeemItemData>();
                for (int j = 0; j < items.Count; j++)
                {
                    Dictionary<string, object> data = items[j] as Dictionary<string, object>;
                    RedeemItemData itemData = new RedeemItemData(data);
                    itemData.SetPlatSprite(platFormSpriteIndex[i]);
                    //任务已完成,创建记录
                    if (itemData.IsFished())
                    {
                        RecordItemData recordItemData = itemData.ToRecordItemData();
                        recordItemDict.Add(recordItemData);
                    }
                    else
                    {
                        redeemItemList.Add(itemData);
                    }
                }
                redeemItemList.Sort((item1,item2)=>
                {
                    return (item1.index < item2.index) ? 1:0;
                });
                redeemItemDict[newKey] = redeemItemList;
            }
            isConfigReady = true;
        }

        public int GetRedeemItemCount(int index)
        {
            if (index<0 || index>=redeemItemDict.Count)
            {
                return 0;
            }
            List<RedeemItemData> redeemItemList = redeemItemDict[PlatformKey + index];
            return redeemItemList.Count;
        }

        public int GetCoolTime()
        {
            return coolTime;
        }
        
        public RedeemItemData GetRedeemItemData(int index)
        {
            List<RedeemItemData> redeemItemList = redeemItemDict[PlatformKey + PlatFormIndex];
            return redeemItemList[index];
        }

        public int GetRecordItemCount()
        {
            if (recordItemDict == null || recordItemDict.Count == 0)
            {
                return 0;
            }
            return recordItemDict.Count;
        }
        
        public RecordItemData GetRecordItemIndex(int index)
        {
            if (index < 0 || index >= recordItemDict.Count)
            {
                return null;
            }
            return recordItemDict[index];
        }
        
        public void RemoveRedeemItem(RedeemItemData data)
        {
            if (data == null)
            {
                return;
            }
            List<RedeemItemData> redeemItemList = redeemItemDict[PlatformKey + PlatFormIndex];
            if (redeemItemList.Contains(data))
            {
                redeemItemList.Remove(data);
            }
        }
        public void AddRecordItemData(RecordItemData itemData)
        {
            if (itemData == null)
            {
                return;
            }
            recordItemDict.Add(itemData);
        }
        
        //当前选中的 toggle 序号
        public void SetPlatFormIndex(int index)
        {
            PlatFormIndex = index;
        }

        public int GetPlatSpriteIndex()
        {
            List<int> index = LocalizationManager.Instance.GetPlatFormSpriteIndex();
            return index[PlatFormIndex];
        }
        
        public void ResetShowAccountTag()
        {
            haveClickShowAccount = false;
        }

        //当前操作的任务Id
        public int GetSelectId()
        {
            return CurSelectTaskId;
        }
        
        public void ResetSelectId()
        {
            CurSelectTaskId = 0;
        }
        
        /// <summary>
        /// 防止多个同时cell点击调用此方法，添加阻截
        /// </summary>
        /// <param name="taskId"></param>
        public void ShowAccountDialog(int taskId,int cash)
        {
            if (haveClickShowAccount)
            {
                return;
            }
            haveClickShowAccount = true;
            CurSelectTaskId = taskId;
            Debug.Log($"[WithDrawManager][ShowAccountDialog] PlatFormIndex:{GetPlatSpriteIndex()}");
            Messenger.Broadcast<int,int>(GameDialogManager.OpenAccountDialogMsg,GetPlatSpriteIndex(),cash);
            // CloseWithDrawDialog();
        }
        public void ShowAccountEnsureDialog(string email,int cash)
        {
            Debug.Log($"[WithDrawManager][ShowAccountDialog] PlatFormIndex:{GetPlatSpriteIndex()}");
            Messenger.Broadcast<int,string,int>(GameDialogManager.OpenAccountEnsureMsg,GetPlatSpriteIndex(),email,cash);
            // ReduceCash(cash);
        }

        public void ReduceCash(int money)
        {
            Debug.Log($"[WithDrawManager][ReduceCash] money:{money}");
            int newCash = OnLineEarningMgr.Instance.Cash() - money;
            OnLineEarningMgr.Instance.SetCash(newCash);
            //广播刷新任务减钱
            Messenger.Broadcast(SlotControllerConstants.OnCashChangeForDisPlay);
            // ShowWithDrawDialog();
        }
        public void ShowWithDrawDialog()
        {
            //打开提现弹窗
            Messenger.Broadcast(GameDialogManager.OpenWithDrawDialog);
        }
        
        //关闭提现弹窗
        public void CloseWithDrawDialog()
        {
            Messenger.Broadcast(GameDialogManager.CloseWithDrawDialog);
        }
    }
}