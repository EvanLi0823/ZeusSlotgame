using WildWest;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Classic;

public class WildWestSpinResult
{
    private WildWestReelManager WildWest;
    private Dictionary<string,object> slotsConfig;

    public int betLevel = 0;
    public long triggerBetting;
    public BisonFree freeType = BisonFree.SPIN;
    public int featuremul = 0;
    public int initLevel = -1;
    public int freeLevel = 0;
    public int featureItem = 0; //spin出现B01(金牛)个数
    public List<int[]> featureItemTable = new List<int[]>();
    public List<int> equalTable = new List<int>();
    public int freespinItem = 0; //触发FreeSpin的symbol个数
    public int freespinPendant = 0; //FreeSpin中/pick挂件的个数
    public int infreePendant = 0; //FreeSpin中收集挂件的个数
    public Dictionary<int[], int> freespinTable = new Dictionary<int[], int>();

    public int pendantNum = 0; //收集挂件个数
    public List<List<int>> pendantCount = new List<List<int>>(); //收集糖果个数

    public List<LineInfo> lineInfo = new List<LineInfo>();
    public Dictionary<int[], int> wildMultiple = new Dictionary<int[], int>();

    #region WildWest的权重数据
    private Dictionary<int,int> basegame_pendant = new Dictionary<int,int>(); //basegame中挂件权重
    private Dictionary<int,int> freegame_pendant = new Dictionary<int,int>(); //freegame中挂件权重
    private Dictionary<int,int> basegame_wild_multiplet = new Dictionary<int,int>(); //basegame中wild奖励权重
    private Dictionary<int,int> freegame_wild_multiplet = new Dictionary<int,int>(); //freegame中wild奖励权重
    private Dictionary<int,int> freegame_R01 = new Dictionary<int,int>(); //freegame中R01权重
    public Dictionary<int,int> freegame_H01 = new Dictionary<int,int>(); //freegame中收集B04替换H01的权重
    public List<TriggerFreeInfo> triggerFreeInfo = new List<TriggerFreeInfo>(); //触发FreeSpin的数据
    private Dictionary<int,int> retrigger_freegame = new Dictionary<int,int>(); //freegame中retrigger权重
    private List<long> serverFeatureOpenBetList; 
    #endregion

    private Dictionary<int,int> retrigger_pendant = new Dictionary<int,int>(); //freegame中retrigger权重


    private List<int> H01_symbolId = new List<int>();

    private Dictionary<int, int> mulIndex = new Dictionary<int, int>();

    public WildWestSpinResult(WildWestReelManager _WildWest, SlotMachineConfig _config)
    {
        WildWest = _WildWest;
        slotsConfig = _config.extroInfos.infos;
        mulIndex = new Dictionary<int, int>(){{1, 0}, {2, 24}, {3, 25}, {4, 26}, {5, 27}, {8, 28}};
        
        this.InitWildWestConfig();
    }

    private void InitWildWestConfig()
    {
       
        this.InitTable(pendantCount);

        this.InitValueInfos("basegame_pendant", basegame_pendant);
        this.InitValueInfos("freegame_pendant", freegame_pendant);
        this.InitValueInfos("basegame_wild_multiplet", basegame_wild_multiplet);
        this.InitValueInfos("freegame_wild_multiplet", freegame_wild_multiplet);
        this.InitValueInfos("freegame_R01", freegame_R01);
        this.InitValueInfos("freegame_H01", freegame_H01);

        if(slotsConfig.ContainsKey("trigger_freegame"))
        {
            List<object> list = slotsConfig["trigger_freegame"] as List<object>;
            

            int i = 0;
            foreach (var item in list)
            {
                Dictionary<string,object> infos = item as Dictionary<string,object>;
                TriggerFreeInfo freeInfo = new TriggerFreeInfo();
                freeInfo.triggerId = Utils.Utilities.CastValueInt(infos["trigger_id"]);
                freeInfo.Bet = Utils.Utilities.CastValueLong(infos["bet"]);
                Dictionary<string,object> timesInfos = infos["times"] as Dictionary<string,object>;
                freeInfo.times.Add(3, Utils.Utilities.CastValueInt(timesInfos["3"]));
                freeInfo.times.Add(4, Utils.Utilities.CastValueInt(timesInfos["4"]));
                freeInfo.times.Add(5, Utils.Utilities.CastValueInt(timesInfos["5"]));
                foreach (var itemInfo in infos["pendant"] as List<object>)
                {
                    Dictionary<string,object> pendantInfos = itemInfo as Dictionary<string,object>;
                    int value = Utils.Utilities.CastValueInt(pendantInfos["Value"]);
                    int weight = Utils.Utilities.CastValueInt(pendantInfos["Weights"]);
                    freeInfo.pendant.Add(value, weight);
                }

                triggerFreeInfo.Add(freeInfo);

                i++;
            }
        }

        if(slotsConfig.ContainsKey("retrigger_freegame"))
        {
            Dictionary<string,object> retimesInfos = slotsConfig["retrigger_freegame"] as Dictionary<string,object>;
            retrigger_freegame.Add(2, Utils.Utilities.CastValueInt(retimesInfos["2"]));
            retrigger_freegame.Add(3, Utils.Utilities.CastValueInt(retimesInfos["3"]));
            retrigger_freegame.Add(4, Utils.Utilities.CastValueInt(retimesInfos["4"]));
            retrigger_freegame.Add(5, Utils.Utilities.CastValueInt(retimesInfos["5"]));
        }

        H01_symbolId = new List<int>(freegame_H01.Keys);
    }

    private void InitValueInfos(string name, Dictionary<int, int> dataInfo)
    {
        if(slotsConfig.ContainsKey(name))
        {
            foreach (var item in slotsConfig[name] as List<object>)
            {
                Dictionary<string,object> infos = item as Dictionary<string,object>;
                int value = Utils.Utilities.CastValueInt(infos["Value"]);
                int weight = Utils.Utilities.CastValueInt(infos["Weights"]);
                dataInfo.Add(value, weight);
            }
        }
    }

    public void OnSpinResult()
    {
        this.InitTable(pendantCount);
        wildMultiple.Clear();
        freespinTable.Clear();
        retrigger_pendant.Clear();
        freespinItem = 0;

        foreach (var item in triggerFreeInfo[betLevel].pendant.Keys) retrigger_pendant.Add(item, triggerFreeInfo[betLevel].pendant[item]);
        
        for (int i = 0; i < WildWest.resultContent.ReelResults.Count; i++)
        {
            for (int j = 0; j < WildWest.resultContent.ReelResults[i].SymbolResults.Count; j++)
            {
                int itemId = WildWest.resultContent.ReelResults[i].SymbolResults[j];
                if(itemId == 23) itemId = WildWest.resultContent.ReelResults[i].SymbolResults[j] = this.R02();
                if(IsPendantId(itemId))
                {
                    pendantCount[i][j] = this.PendantCount();
                    this.SetPendantNum(pendantCount[i][j]);
                } 
                if(IsFreeSpinId(itemId)) 
                {
                    freespinItem ++;
                    freespinTable.Add(new int[2]{i, j}, this.FreeSpinPandent());
                }
                if(itemId == 0)
                {
                    int mul = this.Multiple();
                    if(mul > 1)
                    {
                        wildMultiple.Add(new int[2]{i,j}, mul);
                        WildWest.resultContent.ReelResults[i].SymbolResults[j] = mulIndex[mul];
                    }
                }
            }
        }
        
        //WildWest.FreespinCount = this.FreeSpinCount();

        if(WildWest.isFreespinBonus)
        {
            this.SpinResultInFreeGame();
        } 
        else
        {
            this.freeType = BisonFree.SPIN;
        }

        this.CheckSpinResult();
    }

    private void SpinResultInFreeGame()
    {
        featureItemTable.Clear();

        for (int i = 0; i < WildWest.resultContent.ReelResults.Count; i++)
        {
            for (int j = 0; j < WildWest.resultContent.ReelResults[i].SymbolResults.Count; j++)
            {
                infreePendant += this.pendantCount[i][j];
                int itemId = WildWest.resultContent.ReelResults[i].SymbolResults[j];
                if(itemId == 22) itemId = WildWest.resultContent.ReelResults[i].SymbolResults[j] = this.R01();
                if(equalTable.Contains(itemId)) itemId = WildWest.resultContent.ReelResults[i].SymbolResults[j] = 5;
                if(itemId == 4) featureItemTable.Add(new int[2]{i, j});
            }
        }

        featureItem += featureItemTable.Count;

        this.CheckFreeLevel();
    }

    public void CheckFreeLevel()
    { 
        if(freeLevel == -1) return;
       
        equalTable.Clear();

        int totalCount = this.InitFeatureNum() + featureItem;

        freeLevel = -1;

        for (int i = 0; i < H01_symbolId.Count; i++)
        {
            int min = 0;
            int max = 0;
            if(i != 0) min = freegame_H01[H01_symbolId[i-1]];
            max = freegame_H01[H01_symbolId[i]];
            if(min <= totalCount && totalCount < max) freeLevel = i;
        }

        if(freeLevel == -1) {foreach (var item in H01_symbolId) equalTable.Add(item); return;}
        for (int i = 0; i < freeLevel; i++) equalTable.Add(H01_symbolId[i]);

    }

    public void ReSetFreeSpinData()
    {
        freeType = BisonFree.SPIN;
        featuremul = 0;
        initLevel = -1;
        freeLevel = 0;
        featureItem = 0;
        featureItemTable.Clear();
        equalTable.Clear();
        freespinItem = 0;
        freespinPendant = 0; 
        infreePendant = 0;
        freespinTable.Clear();
        this.InitTable(pendantCount);
    }
    
    public void InitSuperFreeData(FreatureInfo info)
    {
        if(info.pageInfo.index == -1)
        {
            this.initLevel = 3;
            freeLevel = -1;
            foreach (var itemId in H01_symbolId) equalTable.Add(itemId);
        }else
        {
            this.freeLevel = info.pageInfo.index;
            this.initLevel = this.freeLevel - 1;
            for (int i = 0; i < this.freeLevel; i++) equalTable.Add(H01_symbolId[i]);
        }
    }

    private int InitFeatureNum()
    {
        if(initLevel == -1) return 0;
        return freegame_H01[H01_symbolId[initLevel]];
    }

    public int featureItemNum()
    {
        return featureItem;
    }

    public int GetLeftCount()
    {
        if(freeLevel == -1) return 0;
        return freegame_H01[H01_symbolId[freeLevel]] - this.InitFeatureNum() -featureItem;
    }

    public int FreeSpinCount()
    {
        if(WildWest.isFreespinBonus)
        {
            if(retrigger_freegame.ContainsKey(freespinItem)) return retrigger_freegame[freespinItem];
            return 0;
        }
        if(triggerFreeInfo[betLevel].times.ContainsKey(freespinItem)) return triggerFreeInfo[betLevel].times[freespinItem];
        return 0;
    }

    public int FreeSpinPandent()
    {
        int weight = 0;
        int totalWeight = 0;
        foreach (var key in retrigger_pendant.Keys) totalWeight += retrigger_pendant[key];
        int random = UnityEngine.Random.Range(0, totalWeight);
        foreach (var key in retrigger_pendant.Keys)
        {
            if(weight <= random && random < (weight + retrigger_pendant[key]))
            {
                int value = key;
                retrigger_pendant.Remove(key);
                return value;
            }
            weight += retrigger_pendant[key];
        }

        int wrongValue = 0;
        if (triggerFreeInfo[betLevel].pendant.Count > 0)
            wrongValue = triggerFreeInfo[betLevel].pendant.First().Key;
        return wrongValue;
    }

    private bool IsPendantId(int symbolId)
    {
        if(symbolId >= 16 && symbolId <= 21) return true;
        return false;
    }

    private bool IsFreeSpinId(int symbolId)
    {
        return WildWest.symbolMap.getSymbolInfo(symbolId).getBoolValue(SymbolMap.IS_FREESPIN);
    }
    
    private void InitTable(List<List<int>> info)
    {
        info.Clear();
        for (int i = 0; i < 5; i++)
        {
            List<int> list = new List<int>();
            for (int j = 0; j < 4; j++) list.Add(0);
            info.Add(list);
        }
    }

    public int R01()
    {
        return this.RandomValue(freegame_R01);
    }

    public int R02()
    {
        return triggerFreeInfo[betLevel].triggerId;
    }

    public void SetPendantNum(int value)
    {
        if(value > 0 && pendantNum >= 140000) return;
        pendantNum += value;
    }
    
    public int PendantCount()
    {
        if(WildWest.isFreespinBonus)
        {
            return this.RandomValue(freegame_pendant);
        }
        return this.RandomValue(basegame_pendant);
    }

    private int Multiple()
    {
        if(WildWest.isFreespinBonus)
        {
            if(this.freeType != BisonFree.SPIN)
            {
                return this.RandomValue(WildWest.featureTable[featuremul].pageInfo.wild_multiplet);
            }
            
            return this.RandomValue(freegame_wild_multiplet);
        }
        return this.RandomValue(basegame_wild_multiplet);
    }
   
    private int RandomValue(Dictionary<int,int> table)
    {
        int weight = 0;
        int totalWeight = 0;
        foreach (var key in table.Keys) totalWeight += table[key];
        int random = Random.Range(0, totalWeight);
        foreach (var key in table.Keys)
        {
            if(weight <= random && random < (weight + table[key])) return key;
            weight += table[key];
        }
        return new List<int>(table.Keys)[0];
    }

    public void CheckSpinResult()
    {
        lineInfo.Clear();

        for (int i = 0; i < WildWest.resultContent.ReelResults.Count; i++)
        {
            foreach (var item in lineInfo) item.EmptyPosition();

            for (int j = 0; j < WildWest.resultContent.ReelResults[i].SymbolResults.Count; j++)
            {
                int itemId = ReSetSymbolId(WildWest.resultContent.ReelResults[i].SymbolResults[j]);
                
                LineInfo info = IsExistInfo(itemId);
                
                if(i == 0)
                {
                    if(info != null) {info.AddSymbolId(i, j);continue;}
                    LineInfo line = new LineInfo(itemId);
                    line.EmptyPosition();
                    line.AddSymbolId(i, j);
                    lineInfo.Add(line);
                    continue;
                }

                if(WildWest.symbolMap.getSymbolInfo(itemId).getBoolValue(SymbolMap.IS_WILD))
                {
                    foreach (var item in lineInfo) item.AddSymbolId(i, j);
                    continue;
                } 

                if(info != null) info.AddSymbolId(i, j);
            }
        }

        for (int i = lineInfo.Count - 1; i >=0; i--)
        {
            if(WildWest.payTable.singleSymbolPaysCon.ContainsKey(lineInfo[i].symbolId) && WildWest.payTable.singleSymbolPaysCon[lineInfo[i].symbolId].awardMap.ContainsKey(lineInfo[i].count))
            {
                lineInfo[i].pay = WildWest.payTable.singleSymbolPaysCon[lineInfo[i].symbolId].awardMap[lineInfo[i].count];
                lineInfo[i].name = WildWest.payTable.singleSymbolPaysCon[lineInfo[i].symbolId].awardNames[lineInfo[i].count];
                lineInfo[i].CreateLinePos();
            }else
            {
                lineInfo.RemoveAt(i);
            }
        }

        if(this.lineInfo.Count == 0) wildMultiple.Clear();
    }

    private int ReSetSymbolId(int id)
    {
        if(id >= 16 && id <= 21) return id - 6;
        if(id == 4) return 5;
        return id;
    }
    
    public int MultipleValue()
    {
        int value = 1;
        foreach (var item in wildMultiple.Values)
        {
            value *= item;
        }
        return value;
    }
    
    private LineInfo IsExistInfo(int itemId)
    {
        foreach (var info in lineInfo)
        {
            if(info.symbolId == itemId) return info;
        }
        return null;
    }

    public class LineInfo
    {
        public int symbolId = 0;

        public int count
        {
           get
           {
               int value = 0;
               foreach(var index in symbolPos)
               {
                    if(index.Count == 0) return value;
                    value++;
               }
               return value;
           } 
        }
        
        public string name = "";
        
        public float pay = 0;

        public List<List<int>> linePos = new List<List<int>>();

        private List<List<int>> symbolPos = new List<List<int>>();

        public LineInfo(int _symbolId)
        {
            symbolId = _symbolId;
        }

        public void AddSymbolId(int reel, int id)
        {
            symbolPos[reel].Add(id);
        }

        public void EmptyPosition()
        {
            symbolPos.Add(new List<int>(){});
        }

        public void CreateLinePos()
        {  
            linePos.Clear();

            for (int i = 0; i < this.count; i++)
            { 
                if(i == 0)
                {
                    foreach (var index in symbolPos[i]) linePos.Add(new List<int>(){index});
                    continue;
                }

                List<List<int>> lineTabel = new List<List<int>>();

                foreach (var item in linePos)
                {
                    for (int j = 0; j < symbolPos[i].Count; j++)
                    {
                        List<int> symbolIndex = new List<int>(item);
                        symbolIndex.Add(symbolPos[i][j]);
                        lineTabel.Add(symbolIndex);
                    }
                }

                linePos.Clear();

                foreach (var item in lineTabel) linePos.Add(new List<int>(item));
            }
        }
    }

    //freeSpin配置数据类
    public class TriggerFreeInfo
    {
        public int triggerId = 0;

        public long Bet = 0;
        public Dictionary<int, int> times = new Dictionary<int, int>();

        public Dictionary<int, int> pendant = new Dictionary<int, int>();
    }

}

namespace WildWest
{
    public enum BisonFree
    {
        SPIN, 
        BONUS, 
        SUPER
    }
}

