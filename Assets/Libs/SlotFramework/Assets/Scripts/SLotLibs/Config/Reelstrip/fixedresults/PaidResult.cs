using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Classic;
using System.Text;

public class PaidResult
{
    //创建一个OutCome对象 当调用GetSymbolResults时 outComeIndex++
    //为了处理FreeSpin时的Multiplier 用此OutCome对象来保存outComeIndex++前的OutCome对象
    public OutCome currentOutCome = null;

	public int outComeIndex {
		get;
		private set;
	}

	public string id {
		get;
		private set;
	}

	Dictionary<int,List<OutCome>> outcomes = new Dictionary<int, List<OutCome>> ();

    public PaidResult(Dictionary<string, object> infos)
    {
        outComeIndex = 0;
        string paidInfos = Utils.Utilities.GetValue<string>(infos, "result", "");
        List<object> outComesObject = MiniJSON.Json.Deserialize(paidInfos) as List<object>;
        id = Utils.Utilities.GetValue<string>(infos, "id", "");
        if (outComesObject != null && outComesObject.Count>0 &&outComesObject[0]!=null)
        {
            //20191224出现index有可能没有上传的情况
            OutCome outCome = new OutCome(outComesObject[0] as Dictionary<string, object>);
            outcomes[outCome.SpinIndex] = new List<OutCome>();
            outcomes[outCome.SpinIndex].Add(outCome);
//            for (int i = 0; i < outComesObject.Count; i++)
//            {
//                OutCome outCome = new OutCome(outComesObject[i] as Dictionary<string, object>);
//                //20191126 修改逻辑，只用第一个
//                if (outCome.SpinIndex == 0)
//                {
//                    outcomes[outCome.SpinIndex] = new List<OutCome>();
//                    outcomes[outCome.SpinIndex].Add(outCome);
//                }
//                
//            }
        }

#if UNITY_EDITOR
        StringBuilder str = new StringBuilder();
        foreach(int key in outcomes.Keys)
        {
            List<OutCome> outcomeList = outcomes[key];

            for (int i = 0; i < outcomeList.Count; i++)
            {
                str.Append("OutCome" + key + ":\n");
                str.Append(outcomeList[i].StrSymbol + "\n");
            }
        }
        Debug.Log(str);
#endif
    }


    /// <summary>
    /// 判断PaidResult是否有效
    /// </summary>
    public bool ValidPaidResult()
    {
        if(outcomes.Count==0)
        {
            return false;
        }

        return true;
    }


    /// <summary>
    /// 返回从服务器获取的Pattern的OutCome结果
    /// </summary>
    /// <returns>The out come from server.</returns>
    public OutCome GetOutComeFromServer(bool moveIndex=true)
    {
        if(!outcomes.ContainsKey(outComeIndex))
        {
            currentOutCome = null;
            return null;
        }
        if(outcomes[outComeIndex].Count<1)
        {
            currentOutCome = null;
            return null;
        }

        //currentOutCome保存当前对象
        if (moveIndex == true)
        {
            currentOutCome = outcomes[outComeIndex][0];
            //++outComeIndex 并且改变SequencePaidResultManager中符号结果是否用完的状态
            SequencePaidResultsManager.outOfPaidResult = !outcomes.ContainsKey(++outComeIndex);
        }

        return currentOutCome;
    }


    #region 废弃的老旧查询代码

    /// <summary>
    /// 得到符号的结果 根据moveIndex决定是否移动Index
    /// </summary>
    //public List<List<int>> GetSymbolResults(SymbolMap symbolMap, bool moveIndex = true)
    //{
    //    if (!outcomes.ContainsKey(outComeIndex))
    //    {
    //        currentOutCome = null;
    //        return null;
    //    }
    //    if (outcomes[outComeIndex].Count < 1)
    //    {
    //        currentOutCome = null;
    //        return null;
    //    }
    //    SymbolResult symbolResult = new SymbolResult(symbolMap, outcomes[outComeIndex][0].StrSymbol);
    //    //currentOutCome保存当前对象
    //    if (moveIndex == true)
    //    {
    //        currentOutCome = outcomes[outComeIndex][0];
    //        outComeIndex++;
    //    }

    //    return symbolResult.symbolResults;
    //}

    /// <summary>
    /// 得到符号的结果 不移动Index(目前仅用于DiamondRespin)
    /// </summary>
    //public List<List<int>> GetSymbolResultsNotMove(SymbolMap symbolMap)
    //{
    //    return GetSymbolResults(symbolMap, false);
    //}

    /// <summary>
    /// HaapyEaster获取wild特殊符号
    /// </summary>
//    public List<List<int>> GetHappyEasterWildSymbolResults(SymbolMap symbolMap)
//    {
//        int oldOutComeIndex = outComeIndex - 1;

//#if UNITY_EDITOR
//        Debug.Log("GetHappyEasterSymbolResults:outComeIndex:" + outComeIndex + " outcomes.Containkey:" + outcomes.ContainsKey(oldOutComeIndex));
//#endif
    //    if (!outcomes.ContainsKey(oldOutComeIndex))
    //    {
    //        return null;
    //    }
    //    if (outcomes[oldOutComeIndex].Count < 2)
    //        return null;

    //    SymbolResult symbolResult = new SymbolResult(symbolMap, outcomes[oldOutComeIndex][1].StrSymbol);
    //    return symbolResult.symbolResults;
    //}



    /// <summary>
    /// 返回FreeSpin时 额外的multiplier
    /// </summary>
    //public int GetFreeSpinMultiplier()
    //{
    //    //由于在BaseSlotMachine中的计算顺序，首先取得固定结果的值，然后计算Award时才会取得FreeSpin中multiplier的值 在GetSymbolResults中，outComeIndex首先++ 故再次计算时应该计算outComeIndex的情况

    //    if (currentOutCome != null)
    //    {
    //        return currentOutCome.FreeSpinMultiplier;
    //    }
    //    else
    //    {
    //        return 1;
    //    }
    //}

    /// <summary>
    /// 返回currentOutCome中的RandomD中key对应的数据内容 没有则返回默认值
    /// </summary>
    //public T GetDataFromRandomD<T>(string key, T defaultValue)
    //{
    //    if (currentOutCome != null)
    //    {
    //        if (currentOutCome.mRandomDataDict != null && currentOutCome.mRandomDataDict.ContainsKey(key) && currentOutCome.mRandomDataDict[key] != null)
    //        {
    //            return Utils.Utilities.GetValue<T>(currentOutCome.mRandomDataDict, key, defaultValue);
    //        }
    //        else
    //        {
    //            return defaultValue;
    //        }
    //    }

    //    return defaultValue;
    //}


    #endregion

}
