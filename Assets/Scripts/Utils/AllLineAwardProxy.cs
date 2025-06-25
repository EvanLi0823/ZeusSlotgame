using System.Collections;
using System.Collections.Generic;
using Classic;

public class AllLineAwardProxy
{
    private ReelManager reelManager;
    
    private SymbolMap symbolMap;
    private ClassicPaytable payTable;
    private GameConfigs gameConfigs;
    private List<LineInfo> lineInfo = new List<LineInfo>();

    public AllLineAwardProxy(ReelManager _reelManager)
    {
        reelManager = _reelManager;
        if(reelManager==null)return;
        symbolMap = reelManager.symbolMap;
        payTable = reelManager.payTable;
        gameConfigs = reelManager.gameConfigs;
    }

    /// <summary>
    /// 全线轮播计算奖励和播放线奖动画的位置
    /// 算出每条线，逐一添加到中奖线里。区别于CalculateProductPayTableAwardInUnidirectional(注释：把同一symbolId都压到一条线里)
    /// </summary>
    public void CheckAllLineSpinResult()
    {
        lineInfo.Clear();
        for (int i = 0; i < reelManager.resultContent.ReelResults.Count; i++)
        {
            foreach (var item in lineInfo) item.EmptyPosition();
    
            for (int j = 0; j < reelManager.resultContent.ReelResults[i].SymbolResults.Count; j++)
            {
                int itemId = reelManager.resultContent.ReelResults[i].SymbolResults[j];
    
                LineInfo info = IsExistInfo(itemId);
    
                if (i == 0)
                {
                    if (info != null) { info.AddSymbolId(i, j); continue; }
                    LineInfo line = new LineInfo(itemId);
                    line.EmptyPosition();
                    line.AddSymbolId(i, j);
                    lineInfo.Add(line);
                    continue;
                }
    
                if (symbolMap.getSymbolInfo(itemId).getBoolValue(SymbolMap.IS_WILD))
                {
                    foreach (var item in lineInfo) item.AddSymbolId(i, j);
                    continue;
                }
    
                if (info != null) info.AddSymbolId(i, j);
            }
        }
    
        for (int i = lineInfo.Count - 1; i >= 0; i--)
        {
            if (payTable.singleSymbolPaysCon.ContainsKey(lineInfo[i].symbolId) && payTable.singleSymbolPaysCon[lineInfo[i].symbolId].awardMap.ContainsKey(lineInfo[i].count))
            {
                lineInfo[i].pay = payTable.singleSymbolPaysCon[lineInfo[i].symbolId].awardMap[lineInfo[i].count];
                lineInfo[i].name = payTable.singleSymbolPaysCon[lineInfo[i].symbolId].awardNames[lineInfo[i].count];
                lineInfo[i].CreateLinePos();
            }
            else
            {
                lineInfo.RemoveAt(i);
            }
        }
        foreach (var lineItem in lineInfo)
        {
            foreach (var info in lineItem.linePos)
            {
                List<AwardResult.SymbolRenderIndex> awardElement = new List<AwardResult.SymbolRenderIndex>();
                List<int> LineResultIndexs = new List<int>();
                for (int i = 0; i < info.Count; i++)
                {
                    awardElement.Add(new AwardResult.SymbolRenderIndex(i, info[i]));
                    LineResultIndexs.Add(this.reelManager.resultContent.ReelResults[i].SymbolResults[info[i]]);
                }
                //Debug.LogError("lineItem.symbolId-----------    "+lineItem.symbolId);
                this.reelManager.resultContent.awardResult.AddAwardPayLine(lineItem.pay, lineItem.name, awardElement, gameConfigs, lineItem.symbolId, true, null, LineResultIndexs);
                BaseSlotMachineController.Instance.baseAward += (lineItem.pay*BaseSlotMachineController.Instance.currentBettingTemp / gameConfigs.ScalFactor);
            }
        }
    }
    private LineInfo IsExistInfo(int itemId)
    {
        foreach (var info in lineInfo)
        {
            if (info.symbolId == itemId) return info;
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
                foreach (var index in symbolPos)
                {
                    if (index.Count == 0) return value;
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
            symbolPos.Add(new List<int>() { });
        }

        public void CreateLinePos()
        {
            linePos.Clear();

            for (int i = 0; i < this.count; i++)
            {
                if (i == 0)
                {
                    foreach (var index in symbolPos[i]) linePos.Add(new List<int>() { index });
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
}
