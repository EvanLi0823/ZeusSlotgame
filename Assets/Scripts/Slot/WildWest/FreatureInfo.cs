using UnityEngine;
using System.Collections.Generic;

namespace WildWest
{
    public class FreatureInfo
    {
        public bool ISUNLOCK = false; //当前模块是否激活

        public bool ISACHIEVE= false; //当前模块是否解锁完成
        
        public int pendant = 0; //解锁当前模块的子模块所需成本

        public PageInfo pageInfo; //解锁完成当前模块的奖励信息

        public int infoIndex = 0; //当前解锁的itemIndex

        public const int itemCount = 9; //子模块个数

        public List<ItemInfo> itemInfo = new List<ItemInfo>(); //子模块信息表

        public List<int[]> coinInfo = new List<int[]>(); //子模块信息表

        private Dictionary<string,object> infos = new Dictionary<string,object>();

        public FreatureInfo(object _info)
        {
            infos = _info as Dictionary<string,object>;
            Dictionary<string,object> freeInfo = infos["super_free"] as Dictionary<string,object>;
            pendant = Utils.Utilities.CastValueInt(freeInfo["pendant"]);

            pageInfo = new PageInfo("SUPERFREE", 
                                    Utils.Utilities.CastValueInt(freeInfo["bonustimes"]), 
                                    Utils.Utilities.CastValueInt(freeInfo["times"]), 
                                    Utils.Utilities.CastValueInt(freeInfo["index"]),
                                    freeInfo["superbonus_wild_multiplet"] as List<object>
                                    );

            this.ResetItemInfo();
        }

        public void ResetItemInfo()
        {
            coinInfo.Clear();

            ISACHIEVE = false;

            infoIndex = 0;

            foreach (var coin in infos["coin"] as List<object>)
            {
                Dictionary<string,object> info = coin as Dictionary<string,object>;
                coinInfo.Add(new int[]{Utils.Utilities.CastValueInt(info["Value"]), Utils.Utilities.CastValueInt(info["Weights"])});
            }

            itemInfo.Clear();

            List<ItemInfo> infoTabel = new List<ItemInfo>(); //子模块信息表

            for (int i = 0; i < itemCount-2; i++)
            {
                ItemInfo info = new ItemInfo();
                info.rate = this.Rate();
                infoTabel.Add(info);
            }

            infoTabel = this.RandomSortList(infoTabel); //打乱重排

            int x2Index = Random.Range(0, infoTabel.Count-2);

            List<int> index = new List<int>();

            for (int i = 0; i < x2Index; i++) index.Add(i);

            for (int i = x2Index + 2; i < infoTabel.Count-1; i++) index.Add(i);

            int freeIndex = index[Random.Range(0, index.Count-1)];

            for (int i = 0; i < infoTabel.Count; i++)
            {
                if(i == x2Index) itemInfo.Add(new ItemInfo("2X"));
                if(i == freeIndex) itemInfo.Add(new ItemInfo("BONUS"));
                itemInfo.Add(infoTabel[i]);
            }
        }

        public List<T> RandomSortList<T>(List<T> TList)
        {
            List<T> randomList = new List<T>();

            for(int i = TList.Count - 1; i >=0 ; i--)
            {
                int index = Random.Range(0, TList.Count);
                randomList.Add(TList[index]);
                TList.RemoveAt(index);
            }
            
            return randomList;
        }

        public float Rate()
        {
            int weight = 0;
            int totalWeight = 0;
            foreach (var item in coinInfo) totalWeight += item[1];
            int random = UnityEngine.Random.Range(0, totalWeight);
            foreach (var info in coinInfo)
            {
                if(weight <= random && random < (weight + info[1]))
                {
                    float value = float.Parse(info[0].ToString());
                    coinInfo.Remove(info);
                    return value;
                }
                weight += info[1];
            }
            return 0;
        }

        public ItemInfo GetItemInfo(int _index)
        {
            itemInfo[infoIndex].index = _index;
            itemInfo[infoIndex].coin = itemInfo[infoIndex].rate * WildWestUserData.averageBet;
            if(itemInfo[infoIndex].name == "2X") itemInfo[infoIndex+1].indexDouble = _index;
            if(itemInfo[infoIndex].indexDouble != -1) itemInfo[infoIndex].coin *= 2;
            if(infoIndex == itemInfo.Count - 1) ISACHIEVE = true;
            return itemInfo[infoIndex++];
        }

        public class PageInfo
        {
            public PageInfo(string _name, int _bonustimes, int _times, int _index, List<object> wildInfo)
            {
                name = _name;
                bonustimes = _bonustimes;
                times = _times;
                index = _index;
                this.RandomInfos(wildInfo);
            }

            public string name = ""; //解锁当前模块的奖励形式

            public int times = 0; //freespin次数

            public int bonustimes = 0; //freespin次数

            public int rate = 0; //wild倍率

            public int index = 0; //freespin等级

            public Dictionary<int,int> wild_multiplet = new Dictionary<int,int>(); //basegame中wild奖励权重

            private void RandomInfos(List<object> wildInfo)
            {
                foreach (var item in wildInfo)
                {
                    Dictionary<string,object> infos = item as Dictionary<string,object>;
                    int value = Utils.Utilities.CastValueInt(infos["Value"]);
                    int weight = Utils.Utilities.CastValueInt(infos["Weights"]);
                    wild_multiplet.Add(value, weight);
                }
            }
        }

        public class ItemInfo
        {
            public string name = "COIN"; //解锁当前子模块的奖励形式
            
            public int index = -1; //该子模块信息对应的UI索引

            public int indexDouble = -1; //翻倍对应的ID

            public float rate = 0; //奖励倍率（一般为当前下注的几倍）

            public double coin = 0; //解锁时获得的奖励（由于某些主题取平均下注，所以要记录解锁时的奖励）

            public ItemInfo(string _name = "COIN")
            {
                name = _name;
            }
        }
    }
}