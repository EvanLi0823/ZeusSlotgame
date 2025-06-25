using System;
using System.Collections.Generic;
using Classic;
using UnityEditor;
using UnityEngine;
using Utils;

public class SpinResultProduce
{
    public static SpinResultProduce Instance;

    private static List<ISpinResultProvider> _spinResultProviders = new List<ISpinResultProvider>();

    //用来取重，一个类型数据只能有一个
    private static Dictionary<string, ISpinResultProvider> _spinResultProviderDic =
        new Dictionary<string, ISpinResultProvider>();

    static SpinResultProduce()
    {
        Instance = new SpinResultProduce();
    }

    /// <summary>
    /// 需要注意重置
    /// 需要延迟发送才能获取正确的牌面结果 true 在发送的时候不发送需要自己调用发送 eg : firered7_5reels
    /// </summary>
    public bool NeedDelaySend;

    /// <summary>
    /// 一中类型的数据只能加一次，以最后加的那次为准
    /// </summary>
    /// <param name="provider"></param>
    public static void AddProvider(ISpinResultProvider provider)
    {
        Instance?.InternalAddProvider(provider);
    }

    public void InternalAddProvider(ISpinResultProvider provider)
    {
        _spinResultProviderDic[provider.Name] = provider;
    }
    
    public static void GetProvider(SpinResultType type)
    {
        Instance?.InternalGetProvider(type);
    }

    public ISpinResultProvider InternalGetProvider(SpinResultType type)
    {
        if (_spinResultProviderDic.ContainsKey(type.ToString()))
        {
           return _spinResultProviderDic[type.ToString()];
        }

        return null;
    }
    public static void RemoveProvider(SpinResultType type)
    {
        Instance?.InternalRemoveProvider(type);
    }

    public void InternalRemoveProvider(SpinResultType type)
    {
        if (_spinResultProviderDic.ContainsKey(type.ToString()))
        {
            _spinResultProviderDic.Remove(type.ToString());
        }
    }

    /// <summary>
    /// 注意 这个方法调用的时机，可能会获取不到当前的结果
    /// </summary>
    /// <returns></returns>
    public static Dictionary<string, int> InternalGetWinSymbolList()
    {
        if (BaseSlotMachineController.Instance == null || BaseSlotMachineController.Instance.reelManager == null)
        {
            // Debug.LogAssertion("min.wang 获取的 BaseSlotMachineController 的数据有问题");
            return null;
        }
        if (BaseSlotMachineController.Instance.reelManager.IsMultiBoard())
            return Instance?.GetMultiBoardWinSymbolList();
        return Instance?.GetWinSymbolList();
    }

    private Dictionary<string, int> GetMultiBoardWinSymbolList()
    {
        MultiMainBoard multiMainBoard = (MultiMainBoard)BaseSlotMachineController.Instance.reelManager;
        List<SingleBoard> singleBoards = multiMainBoard.GetMultiBoards().multiReels;
        int singleBoardCount = multiMainBoard.MultiBoardNum;
        if (singleBoards.Count < singleBoardCount) return null;
        Dictionary<string, int> tempDic = new Dictionary<string, int>();
        for (int i = 0; i < singleBoardCount; i++)
        {
            Dictionary<string, int> symbolsList = GetWinSymbolList(true,singleBoards[i]);
            if(symbolsList==null) continue;
            foreach (var key in symbolsList.Keys)
            {
                if (tempDic.ContainsKey(key))
                {
                    tempDic[key] += symbolsList[key];
                }
                else
                {
                    tempDic[key] = symbolsList[key];
                }
            }
        }
        return tempDic;
    }

    private Dictionary<string, int> GetWinSymbolList(bool isSingleBoard = false,ReelManager singleBoard = null)
    {
        ReelManager reelManager = BaseSlotMachineController.Instance.reelManager;
        if (isSingleBoard && singleBoard != null)
            reelManager = singleBoard;
        ResultContent resultContent = reelManager.resultContent;
        SymbolMap symbolMap = reelManager.symbolMap;
        bool linePayValue = !(resultContent?.awardResult?.awardInfos == null || symbolMap == null);

        Dictionary<string, int> tempDic = new Dictionary<string, int>();

        Dictionary<int, List<int>> winSymbolPos = new Dictionary<int, List<int>>();

        if (linePayValue)
        {
            for (int i = 0; i < resultContent.awardResult.awardInfos.Count; i++)
            {
                if (resultContent.awardResult.awardInfos[i].awardValue<=0.00001)
                {
                    continue;
                }

                List<AwardResult.SymbolRenderIndex> symbolRender =
                    resultContent.awardResult.awardInfos[i]?.symbolRenderIndexs;
                if (symbolRender == null)
                {
                    continue;
                }
                for (int j = 0; j < symbolRender.Count; j++)
                {
                    //位置去重
                    int reelIndex = symbolRender[j].reelIndex ;
                    int symbolPos = symbolRender[j].symbolIndexInReel;
                    if (winSymbolPos.ContainsKey(reelIndex))
                    {
                        if (winSymbolPos[reelIndex].Contains(symbolPos))
                        {
                            continue;
                        }

                        winSymbolPos[reelIndex].Add(symbolPos);
                    }
                    else
                    {
                        winSymbolPos[reelIndex] = new List<int> {symbolPos};
                    }

                    if (resultContent.ReelResults[reelIndex] == null || resultContent.ReelResults[reelIndex].SymbolResults ==null )
                    {
                        continue;
                    }
                    
                    string symbolName = symbolMap.getSymbolInfo(resultContent.ReelResults[reelIndex].SymbolResults[symbolPos])
                        ?.name;
                    if (symbolName == null)
                    {
                        continue;
                    }

                    if (tempDic.ContainsKey(symbolName))
                    {
                        tempDic[symbolName] = tempDic[symbolName] + 1;
                    }
                    else
                    {
                        tempDic[symbolName] = 1;
                    }
                }
            }  
        }
        

        List<PaytableElement> scatterPays = reelManager.payTable?.scatterPays;
        if (scatterPays == null)
        {
            return tempDic.Count>0?tempDic:null;
        }
        for (int i = 0; i < scatterPays.Count; i++)
        {
            ScatterPayInfo scatterPayInfo = scatterPays[i].ScatterPay;
            if (scatterPayInfo == null)
            {
                continue;
            }
            List<SymbolPos> symbolPoses = scatterPayInfo.SymbolPoses;
            if (scatterPayInfo.IsValue && scatterPayInfo.AwardValue>0 )
            {
                for (int j = 0; j < symbolPoses.Count; j++)
                {
                    int reelIndex = symbolPoses[i].ReelIndex;
                    int symbolInReelIndex = symbolPoses[i].SymbolInReelPos;
                    string symbolName = symbolPoses[i].Name;
                    if (string.IsNullOrEmpty(symbolName))
                    {
                        continue;
                    }
                    if (winSymbolPos.ContainsKey(reelIndex)&& winSymbolPos[reelIndex].Contains(symbolInReelIndex))
                    {
                        continue;
                    }
                    else
                    {
                        winSymbolPos[reelIndex] = new List<int> {symbolInReelIndex};
                    }
                    
                    
                    if (tempDic.ContainsKey(symbolName))
                    {
                        tempDic[symbolName] = tempDic[symbolName] + 1;
                    }
                    else
                    {
                        tempDic[symbolName] = 1;
                    }
                    
                }
            }
        }


        if (tempDic.Count <= 0)
        {
            return null;
        }

        return tempDic;
    }


    /// <summary>
    /// 注意 这个方法调用的时机，可能会获取不到当前的结果
    /// </summary>
    /// <returns></returns>
    public static Dictionary<string, int> InternalGetSymbolsList()
    {
        if (BaseSlotMachineController.Instance == null || BaseSlotMachineController.Instance.reelManager == null)
        {
            // Debug.LogAssertion("min.wang 获取的 BaseSlotMachineController 的数据有问题");
            return null;
        }

        if (BaseSlotMachineController.Instance.reelManager.IsMultiBoard())
            return Instance?.GetMultiBoardSymbolList();
        return Instance?.GetSymbolsList();
    }

    private Dictionary<string, int> GetMultiBoardSymbolList()
    {
        MultiMainBoard multiMainBoard = (MultiMainBoard)BaseSlotMachineController.Instance.reelManager;
        List<SingleBoard> singleBoards = multiMainBoard.GetMultiBoards().multiReels;
        int singleBoardCount = multiMainBoard.MultiBoardNum;
        if (singleBoards.Count < singleBoardCount) return null;
        Dictionary<string, int> tempDic = new Dictionary<string, int>();
        for (int i = 0; i < singleBoardCount; i++)
        {
            Dictionary<string, int> symbolsList = GetSymbolsList(true,singleBoards[i]);
            if(symbolsList==null) continue;
            foreach (var key in symbolsList.Keys)
            {
                if (tempDic.ContainsKey(key))
                {
                    tempDic[key] += symbolsList[key];
                }
                else
                {
                    tempDic[key] = symbolsList[key];
                }
            }
        }
        return tempDic;
    }

    /// <summary>
    /// 此处注意西伯利亚虎那个机器获取的数据是否正确
    /// </summary>
    /// <param name="resultContent"></param>
    /// <param name="symbolMap"></param>
    /// <returns></returns>
    private Dictionary<string, int> GetSymbolsList(bool isSingleBoard = false,ReelManager singleBoard = null)
    {
        ReelManager reelManager = BaseSlotMachineController.Instance.reelManager;
        if (isSingleBoard && singleBoard != null)
            reelManager = singleBoard;
        ResultContent resultContent = reelManager.resultContent;
        SymbolMap symbolMap = reelManager.symbolMap;

        if (resultContent?.ReelResults == null || symbolMap == null)
        {
            return null;
        }

        int offSet = 0;
        if (reelManager.gameConfigs.hasBlank)
        {
            offSet = 1;
        }

        List<object> symbols = null;
        Dictionary<string, int> tempDic = new Dictionary<string, int>();
        for (int i = 0; i < resultContent.ReelResults.Count; i++)
        {
            if (resultContent.ReelResults[i].SymbolResults==null)
            {
                continue;
            }
            for (int j = offSet; j < resultContent.ReelResults[i].SymbolResults.Count - offSet; j++)
            {
                
                string symbolName = symbolMap.getSymbolInfo(resultContent.ReelResults[i].SymbolResults[j])?.name;
                if (symbolName == null)
                {
                    continue;
                }

                if (tempDic.ContainsKey(symbolName))
                {
                    tempDic[symbolName] = tempDic[symbolName] + 1;
                }
                else
                {
                    tempDic[symbolName] = 1;
                }
            }
        }

        return tempDic;
    }

    public static void InternalSend()
    {
        if(BaseSlotMachineController.Instance!=null
           &&BaseSlotMachineController.Instance.reelManager!=null&&
           !BaseSlotMachineController.Instance.reelManager.SpinUseNetwork)
            Instance?.Send();
    }


    public void Send()
    {
        Dictionary<string, object> dic = new Dictionary<string, object>();
        Dictionary<string, object> machineDic = new Dictionary<string, object>();
        dic["Version"] = "1.0.0";
        dic["Source"] = "client";
        dic["Timestamp"] = DateTime.UtcNow.Ticks;
        //todo ： name 这儿可能需要处理一下名字
        dic["Machine"] = machineDic;

        machineDic["MachineName"] = UserManager.GetInstance().UserProfile().LastChooseSlotName;
        Dictionary<string, object> fullScreenData = GetFullScreenData();
        if (fullScreenData != null && fullScreenData.Count > 0)
        {
            machineDic["FullScreen"] = fullScreenData;
        }

        List<string> nOfKindData = NOfKind;
        if (nOfKindData != null && nOfKindData.Count>0)
        {
            machineDic["NOfAKind"] = new Dictionary<string, object>()
            {
                {"Symbol", nOfKindData},
                {"ReelNum", BaseSlotMachineController.Instance.reelManager.GetReelCount()}//ggg换成BoardConfig
            };
        }

        Dictionary<string, string> specialFeatureData = GetSpecialFeature();
        if (specialFeatureData != null)
        {
            machineDic["SpecialFeature"] = specialFeatureData;
        }

        machineDic["Bet"] = BaseSlotMachineController.Instance.currentBetting;

        string winType = GetWinType();
        if (!string.IsNullOrEmpty(winType))
        {
            machineDic["WinType"] = winType;
        }

        if (_spinResultProviderDic.Count == 0)
        {
            // Debug.LogAssertion("machinQuestData : 此处machine 数据是空，查看是否添加");
            return;
        }

        // 40 MachineQuest|ClubMachine 这两个暂时没有
        machineDic["Source"] = GetSource();
        foreach (KeyValuePair<string, ISpinResultProvider> keyValuePair in _spinResultProviderDic)
        {
            machineDic[keyValuePair.Key] = keyValuePair.Value.Decode();
        }

        _spinResultProviderDic.Clear();
        Messenger.Broadcast(GameConstants.OnSpinEndSendResultEvent, dic);
        Messenger.Broadcast<Dictionary<string, object>>(GameConstants.QuestMachineOnResponseMsg, dic);
        string json = MiniJSON.Json.Serialize(dic);

        // Debug.LogAssertion("machineQuestData : " + json);
#if UNITY_EDITOR
        BaseGameConsole.ActiveGameConsole().LogBaseEvent("MachineQuest0",new Dictionary<string, object>(){{"machinequest",json}});
#endif
    }

    private Dictionary<string,string> SpecialFeature = new Dictionary<string, string>();
    public void SetSpecialFeature(string feature)
    {
        SpecialFeature.Clear();
        if (string.IsNullOrEmpty(feature))
        {
            return ;
        }
        string machineName = UserManager.GetInstance().UserProfile().LastChooseSlotName;
        SpecialFeature.Add(machineName,feature);
        
    }

    private  Dictionary<string,string> GetSpecialFeature()
    {
        Dictionary<string, string> temp = new Dictionary<string, string>();
        if (SpecialFeature!=null)
        {
            foreach (var item in SpecialFeature)
            {
                temp.Add(item.Key,item.Value);
            }
            SpecialFeature.Clear();
        }
        return temp;
    }
    
    
    private List<string> _nOfKind = new List<string>();
    
    
    /// <summary>
    /// 设置之后，取一次值清空上一次的赋值
    /// </summary>
    public List<string> NOfKind
    {
        set { _nOfKind = value; }
        get
        {
            if (_nOfKind==null || _nOfKind.Count==0)
            {
                _nOfKind = GetNOfKind();
            }
            List<string> temp = new List<string>();
            if (_nOfKind != null)
            {
                temp.AddRange(_nOfKind);
            }
            _nOfKind?.Clear();
            return temp;
        }
    }
    
    public List<string> GetNOfKind()//ggg看下41 5ofKind逻辑
    {
        ReelManager reelManager = BaseSlotMachineController.Instance.reelManager;
        if(reelManager==null) return null;
        
        SymbolMap symbolMap = reelManager.symbolMap;
        if (reelManager.awardLines == null || symbolMap == null) 
        {
            return null;
        }

        List<string> symbols = new List<string>();
        int reelNumber = reelManager.GetReelCount();
        CheckNOfKind(reelManager.awardLines, symbolMap, reelNumber, symbols);
        if (symbols.Count == 0)
        {
            return null;
        }

        return symbols;
    }

    private void CheckNOfKind(List<AwardLineElement> awardLineElement,SymbolMap symbolMap,int reelNumber,List<string> symbols)
    {
        for (int i = 0; i < awardLineElement.Count; i++)
        {
            if(!awardLineElement[i].awardPayLine.isAwarded || awardLineElement[i].awardPayLine.LineSymbolIndexs.Count == 0) continue;
            //symbolname: count
            Dictionary<string, int> tempDic = new Dictionary<string, int>();
            int symbolTotalCount = 0;
            for (int j = 0; j < awardLineElement[i].awardPayLine.LineSymbolIndexs.Count; j++)
            {
                SymbolMap.SymbolElementInfo symbolElementInfo =
                    symbolMap.getSymbolInfo(awardLineElement[i].awardPayLine.LineSymbolIndexs[j]);

                if (symbolElementInfo == null || symbolElementInfo.Index < 0)
                {
                    break;
                }

                string symbolName = symbolElementInfo.name;

                if (symbolElementInfo.isWild)
                {
                    symbolTotalCount++;
                    continue;
                }
                if (tempDic.ContainsKey(symbolName))
                {
                    tempDic[symbolName] = tempDic[symbolName] + 1;
                }
                else
                {
                    tempDic[symbolName] = 1;
                }

                if (!symbolElementInfo.IsBlank())
                {
                    symbolTotalCount++;
                }
                
            }

            if (symbolTotalCount == reelNumber && tempDic.Count == 1)
            {
                foreach (string symbolName in tempDic.Keys)
                {
                    if (symbols.Contains(symbolName))
                    {
                        continue;
                    }
                    symbols.Add(symbolName);
                }
            }
        }
    }
    
    private Dictionary<string, object> GetFullScreenData()
    {
        if (BaseSlotMachineController.Instance.reelManager.gameConfigs.hasBlank)
        {
            return null;
        }

        ResultContent resultContent = BaseSlotMachineController.Instance.reelManager.resultContent;
        SymbolMap symbolMap = BaseSlotMachineController.Instance.reelManager.symbolMap;

        if (resultContent?.awardResult == null || symbolMap == null || resultContent.awardResult.awardInfos == null)
        {
            return null;
        }

        Dictionary<string,int> dicTemp=new Dictionary<string, int>();
        List<string> symbols= new List<string>();
        bool haveWild = false;
        int symbolCount = 0;
        string symbolName=String.Empty;
        for (int i = 0; i < resultContent.ReelResults.Count; i++)
        {
            for (int j = 0; j < resultContent.ReelResults[i].SymbolResults.Count; j++)
            {
                SymbolMap.SymbolElementInfo info = symbolMap.getSymbolInfo(resultContent.ReelResults[i].SymbolResults[j]);
                if (info == null)
                {
                    return null;
                }

                if (info.isWild)
                {
                    haveWild = true;
                    //continue;
                }

                if (dicTemp.ContainsKey(info.name))
                {
                    dicTemp[info.name] += 1;
                }
                else
                {
                    dicTemp[info.name] = 1;
                    symbolName = info.name;
                }
                
            }
        }

        if (dicTemp.Count==1||(haveWild&&dicTemp.Count==2))
        {
            Dictionary<string,object> dic=new Dictionary<string, object>();
            dic["HaveWild"] = haveWild;
            dic["Symbol"] = symbolName;
            return dic;
        }

        return null;
    }

    public string GetWinType()
    {
        if (BaseSlotMachineController.Instance.totalAward <= 0.1)
        {
            return string.Empty;
        }
        
        if (BaseSlotMachineController.Instance.isEpicWin)
        {
            return "Epic";
        }
        
        if (BaseSlotMachineController.Instance.isBigWin)
        {
            return "Big";
        }
        
        if (BaseSlotMachineController.Instance.isMiddleWin)
        {
            return "Middle";
        }

        if (BaseSlotMachineController.Instance.isSmallWin)
        {
            return "Small";
        }

        if (BaseSlotMachineController.Instance.isTinyWin)
        {
            return "Tiny";
        }

        if (BaseSlotMachineController.Instance.isMegaWin)
        {
            return "Mega";
        }
        return string.Empty;
    }

    private string GetSource()
    {
        
        if (BaseSlotMachineController.Instance.slotMachineConfig.isInClub)
        {
            return "ClubMachine";
        }
        
        return string.Empty;
    }
}


public enum TriggerFeatureType
{
    Symbol,
    Pick,
    Wheel,
    Collect,
    OtherBonus
}

public enum SpinResultType
{
    BaseGame,
    FreeGame,
    ReSpinGame,
    Scatter,
    Pick,
    Wheel,
    BonusTrigger,
    ReSpinTrigger,
    FreeTrigger,
    WildJackpot,
    MachineJackpot,
    MiniGame,
    SuperJackpot
}

public enum JackPotNameType
{
    Common,
    Rapid
}