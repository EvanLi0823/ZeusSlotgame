using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using nacl;
using System.Security.Cryptography;
using System;
using Classic;

public static class SequencePaidResultsManager
{
    /// <summary>
    /// 服务器请求noWinResult的状态 
    /// Idle表示没有请求或者请求结束 Wait表示正在请求Result
    /// </summary>
    public enum NoWinResultRequestState
    {
        Idle,
        Wait,
    }

    public static readonly string TYPE_FORCE = "force";
    public static readonly string Balance_Bet = "bb_";
    public static readonly string NO_WIN = "no_win";
    static SequencePaidResults currentPaidResults = new SequencePaidResults();
    public static object status;
    static SequencePaidResults idleResults = new SequencePaidResults();
    static List<SequencePaidResults> bbPaidResults = new List<SequencePaidResults>();
    static SequencePaidResults forcePaidResults = new SequencePaidResults();
    static SequencePaidResults noWinPaidResults = new SequencePaidResults();
    static Dictionary<string, object> patterSwitch = new Dictionary<string, object>();
    public static readonly string PATTER_SWITCH = "PatterEventSwitch";
    public static readonly string ENTER = "enter";
    public static readonly string PAY = "pay";
    public static readonly string LEVEL_UP = "levelup";
    public static readonly string BIG_WIN = "bigwin";
    public static readonly string BET_CHANGE = "betchange";
    public static readonly string NEARLY_OUT_OF_COINS = "nearlyoutofcoins";
    public static readonly string IS_NEW_MACHINE_FOR_USER = "isNewMachineForUser";
    public static readonly string NO_WIN_REQUEST = "no_win";

    //当前SequencePaidResultManager请求noWinResult的时间
    public static float noWinResultRequestTime = Time.realtimeSinceStartup;
    //当前SequencePaidResultManager请求noWinResult的状态
    public static NoWinResultRequestState noWinResultRequestState = NoWinResultRequestState.Idle;
    

	public static string nearlyoutofcoins_skuname;

//    //发送es相关
//    public static string CurrentPuid { set; get; }
//    public static int PuType = -1;
//
//    public static void AddEsSendType(string _puid, int _type)
//    {
//        CurrentPuid = _puid;
//        PuType = _type;
//    }
//
//    public static void ClearEsSendType()
//    {
//        CurrentPuid = "";
//        PuType = -1;
//    }
    public static void Init(Dictionary<string, object> info)
    {
        if (info == null)
            return;
        patterSwitch.Clear();
        patterSwitch = new Dictionary<string, object>(info);
    }

    public static void Reset()
    {
        bbPaidResults.Clear();
        forcePaidResults = new SequencePaidResults();
        noWinPaidResults = new SequencePaidResults();
        currentPaidResults = idleResults;
    }

    public static bool CheckCondition()
    {
        if (!(BaseGameConsole.singletonInstance.IsInSlotMachine()))
        {
            return false;
        }

        if (BaseSlotMachineController.Instance.reelManager.IsCostBetSpin())
        {
            if (forcePaidResults.CheckCondition())
            {
                if (currentPaidResults.isBalanceBetType)
                {
                    bbPaidResults.Remove(currentPaidResults);
                }
                currentPaidResults = forcePaidResults;
                return true;
            }

            if (currentPaidResults.isBalanceBetType)
            {
                if (currentPaidResults.isEnd)
                {
                    bbPaidResults.Remove(currentPaidResults);
                }

                if (currentPaidResults.CheckCondition())
                {
                    return true;
                }
            }

            for (int i = 0; i < bbPaidResults.Count; i++)
            {
                if (bbPaidResults[i].CheckCondition())
                {
                    currentPaidResults = bbPaidResults[i];
                    return true;
                }
            }

            currentPaidResults = idleResults;
        }
        else
        {
            return currentPaidResults.CheckCondition();
        }

        return false;
    }

    public static void ChangeSequncePaidResults(string patternInfos)
    {
        if (!(BaseGameConsole.singletonInstance.IsInSlotMachine()))
        {
            return;
        }
        if (string.IsNullOrEmpty(patternInfos))
        {
            return;
        }        
        String messageString = OpenServerFromMessage(patternInfos);
//        Debug.LogAssertion("__________ : " + messageString);
        Dictionary<string, object> p = MiniJSON.Json.Deserialize(messageString) as Dictionary<string, object>;
        
        //rule 初始化
        if (p.ContainsKey("r"))
        {
            Dictionary<string, object> dic = p["r"] as Dictionary<string, object>;
            RulePatternManager.GetInstance().InitRuleInfo(dic,UserManager.GetInstance().UserProfile().Balance());
        }
        
        string machine = Utils.Utilities.GetValue<string>(p, "machine", "NoMachine");

		nearlyoutofcoins_skuname = Utils.Utilities.GetValue<string>(p, "nearlyoutofcoins_skuname", nearlyoutofcoins_skuname);
        if (BaseSlotMachineController.Instance==null
            ||BaseSlotMachineController.Instance.slotMachineConfig==null
            ||!machine.Equals(BaseSlotMachineController.Instance.slotMachineConfig.Name()))
        {
            return;
        }
        Dictionary<string, object> patters = Utils.Utilities.GetValue<Dictionary<string, object>>(p, "patterns", null);
        status = Utils.Utilities.GetValue<object>(p, "status", null);
        if (status!=null && ((Dictionary<string, object>) status).ContainsKey("bet"))
        {
            if (BaseSlotMachineController.Instance.currentBetting > Utils.Utilities.CastValueLong(((Dictionary<string, object>) status)["bet"]))
            {
                return;
            }
        }
        if (patters != null)
        {
            foreach (string typeKey in patters.Keys)
            {
                List<object> patterObjects = patters[typeKey] as List<object>;
                if (patterObjects != null && patterObjects.Count > 0)
                {
                    SequencePaidResults sequencePaidResults = new SequencePaidResults(patterObjects[0] as Dictionary<string, object>,
                        machine, typeKey);
                    if (sequencePaidResults.isForceType)
                    {
                        forcePaidResults = sequencePaidResults;
                    }
                    else if (sequencePaidResults.isBalanceBetType)
                    {
                        bbPaidResults.Add(sequencePaidResults);
                    }
                    else if (sequencePaidResults.isNowinType)
                    {
                        //判断接收到的SequencePaidResults是否有效
                        if (sequencePaidResults.VaildSequencePaidResults())
                        {
                            noWinPaidResults = sequencePaidResults;
                            //收到结果后 改变state
                            noWinResultRequestState = NoWinResultRequestState.Idle;
                            //改变目前的结果标志值
                            outOfPaidResult = false;
                            outOfSequencePaidResults = false;
                        }
                        else
                        {
                            //无效则关闭ReelManager的NoWinPattern功能
                            if(BaseSlotMachineController.Instance.reelManager!=null)
                                BaseSlotMachineController.Instance.reelManager.NoWinPatternSwitch = false;
                        }
                    }
                    break;
                }
            }
        }
    }
    
    public static SequencePaidResults GetCurrentResult()
    {
        return currentPaidResults;
    }

    public static void RequestPatterEvent(string reason, Dictionary<string, object> autopilotInfo = null)
    {
        if (Utils.Utilities.GetValue<bool>(patterSwitch, reason, false))
        {
#if DEBUG_LOG
            Log.Trace($"RequestPatterEvent--{reason}");
#endif
            // HSAccountMgr.Instance.RequestPatterEvent(reason, autopilotInfo);
        }
    }

    static string OpenServerFromMessage(string patternInfos)
    {
        byte[] bytes = Convert.FromBase64String(patternInfos);
        int chipLenth = bytes.Length - SecretBox.NonceSize;
        // SecretBox box = new SecretBox(HSAccountMgr.Instance.FinalKeyNacl);
        byte[] nonce = new byte[SecretBox.NonceSize];
        Buffer.BlockCopy(bytes, 0, nonce, 0, SecretBox.NonceSize);
        byte[] message = new byte[chipLenth - SecretBox.BoxZeroSize];

        byte[] cipherBoxed;
        byte[] messageBoxed;
        cipherBoxed = new byte[chipLenth + SecretBox.BoxZeroSize];
        Buffer.BlockCopy(bytes, SecretBox.NonceSize, cipherBoxed, SecretBox.BoxZeroSize, chipLenth);
        messageBoxed = new byte[chipLenth + SecretBox.MACSize];
        // box.Open(messageBoxed, cipherBoxed, nonce);
        Buffer.BlockCopy(messageBoxed, SecretBox.ZeroSize, message, 0, chipLenth - SecretBox.MACSize);

        String messageString = System.Text.Encoding.UTF8.GetString(message);
#if UNITY_EDITOR
        Log.Trace("<color=lime>Patter Message:</color>" + messageString);
#endif
        return messageString;
    }

    public static string BoxMessageToServer(string infos)
    {
#if DEBUG_LOG
//        Debug.Log("BoxMessageToServer---" + infos);
#endif
        byte[] nonce = new byte[SecretBox.NonceSize];
        RNGCryptoServiceProvider random = new RNGCryptoServiceProvider();
        random.GetBytes(nonce);
        // SecretBox box = new SecretBox(HSAccountMgr.Instance.FinalKeyNacl);
        byte[] m = System.Text.Encoding.UTF8.GetBytes(infos);
        byte[] finalC = new byte[m.Length + SecretBox.NonceSize + SecretBox.BoxZeroSize];

        byte[] cipherBoxed;
        byte[] messageBoxed;

        messageBoxed = new byte[m.Length + SecretBox.ZeroSize];
        Buffer.BlockCopy(m, 0, messageBoxed, SecretBox.ZeroSize, m.Length);

        cipherBoxed = new byte[m.Length + SecretBox.ZeroSize];

        // box.Box(cipherBoxed, messageBoxed, nonce);
        Array.Clear(messageBoxed, 0, m.Length);
        Buffer.BlockCopy(nonce, 0, finalC, 0, SecretBox.NonceSize);
        Buffer.BlockCopy(cipherBoxed, SecretBox.BoxZeroSize, finalC, SecretBox.NonceSize, m.Length + SecretBox.BoxZeroSize);
        string s = Convert.ToBase64String(finalC);
        return s;
    }

#region OutCome相关

    public static OutCome currentForceOutCome = null;
    public static OutCome preForceOutCome = null;
    public static OutCome currentNoWinOutCome = null;

    /// <summary>
    /// 返回从服务器获取的Pattern的OutCome
    /// </summary>
    public static OutCome GetOutComeFromServer()
    {
        return currentPaidResults.GetOutComeFromServer();
    }

    /// <summary>
    /// 取出新的OutCome
    /// </summary>
    public static void ChangeOutComeFromSequencePaidResults()
    {
        if (BaseSlotMachineController.Instance == null) return;
//        if (BaseSlotMachineController.Instance.reelManager.ReelLockedByNormal) return;

        OutCome outCome = currentPaidResults.GetOutComeFromServer();

        SetCurrentOutcome(outCome);
    }

    /// <summary>
    /// 设置ForcePattern的OutCome
    /// </summary>
    public static void SetCurrentOutcome(OutCome outComeFromServer)
    {
        preForceOutCome = currentForceOutCome;
        currentForceOutCome = outComeFromServer;
    }

    /// <summary>
    /// 返回ForcePattern中的符号结果
    /// </summary>
    public static List<List<int>> SymbolResultsFromForceOutcome()
    {
        if (currentForceOutCome == null)
        {
            return null;
        }

        return currentForceOutCome.SymbolResult;
    }

    /// <summary>
    /// 返回ForcePattern中的上一个符号结果
    /// </summary>
    public static List<List<int>> SymbolResultsFromPreForceOutcome()
    {
        if (preForceOutCome == null)
        {
            return null;
        }
        return preForceOutCome.SymbolResult;
    }


    public static bool outOfPaidResult = false; //20190708,可以认为是outcome数组的最后一个
    public static bool outOfSequencePaidResults = false;


    /// <summary>
    /// 检查是否需要请求NoWinPattern 如果需要则请求
    /// </summary>
    public static bool NeedRequestNoWinPattern()
    {
        bool needRequest = false;

        //首先检查 结果是否用完
        needRequest = outOfPaidResult && outOfSequencePaidResults;
        //检查noWinPaidResults是否处于Idle状态 
        if (noWinPaidResults.isIdle)
        {
            needRequest = true;
        }

        //如果needRequest为true 则向服务器请求NoWinPattern
        RequestNoWinPattern(needRequest);

        return needRequest;
    }

    /// <summary>
    /// 请求noWinPattern
    /// </summary>
    public static void RequestNoWinPattern(bool requestNoWinPattern)
    {
        //如果目前处于NoWinResultRequestState.Wait状态 检查与上次时间的差值 如果大于20 则重置状态
        if (noWinResultRequestState == NoWinResultRequestState.Wait)
        {
            //处理从服务器拿不到数据的情况 如果20s内没有从数据库中拿到数据 则重置状态
            float twoRequestInterval = Time.realtimeSinceStartup - noWinResultRequestTime;
            if (twoRequestInterval > 20f)
            {
                noWinResultRequestState = NoWinResultRequestState.Idle;
            }
        }

        if (requestNoWinPattern)
        {
            if (noWinResultRequestState == NoWinResultRequestState.Idle)
            {
                SequencePaidResultsManager.RequestPatterEvent(SequencePaidResultsManager.NO_WIN_REQUEST);
#if DEBUG_LOG
                Debug.Log("向服务器请求NoWin");
#endif
                noWinResultRequestState = NoWinResultRequestState.Wait;
                noWinResultRequestTime = Time.realtimeSinceStartup;
            }
        }
    }


    /// <summary>
    /// 返回NoWinPattern对应的符号结果
    /// </summary>
    public static List<List<int>> SymbolResultsFromNoWinOutcome()
    {
        List<List<int>> results = null;

//        if (BaseSlotMachineController.Instance.reelManager.ReelLockedByNormal)
//        {
//            return results;
//        }

        //在这里进行调用
        if (noWinPaidResults.CheckCondition())
        {
            currentNoWinOutCome = noWinPaidResults.GetOutComeFromServer();
            if (currentNoWinOutCome != null)
            {
                results = currentNoWinOutCome.SymbolResult;
            }

            //如果结果用完 则向服务器请求NoWinPattern
            RequestNoWinPattern(outOfPaidResult && outOfSequencePaidResults);
        }

        return results;
    }

    /// <summary>
    /// 返回ForcePattern中的FreeSpin时的Multiplier倍数
    /// </summary>
    public static int FreeSpinMultiplierFromForceOutcome()
    {
        if (currentForceOutCome != null)
        {
            return currentForceOutCome.Multiplier;
        }else{
            return 1;
        }
    }

    /// <summary>
    /// 返回currentServerOutcome中RandomD中的内容
    /// </summary>
    public static T RandomDFromForceOutcome<T>(string key, T defaultValue)
    {
        if (currentForceOutCome != null)
        {
            if (currentForceOutCome.mRandomDataDict != null && currentForceOutCome.mRandomDataDict.ContainsKey(key) && currentForceOutCome.mRandomDataDict[key] != null)
            {
                return Utils.Utilities.GetValue<T>(currentForceOutCome.mRandomDataDict, key, defaultValue);
            }else{
                return defaultValue;
            }
        }

        return defaultValue;
    }

    /// <summary>
    /// 返回currentServerOutcome中RandomD中是否含有key值
    /// </summary>
    public static bool RandomDContainKey(string key)
    {
        if (currentForceOutCome != null)
        {
            if (currentForceOutCome.mRandomDataDict != null && currentForceOutCome.mRandomDataDict.ContainsKey(key) && currentForceOutCome.mRandomDataDict[key] != null)
            {
                return true;
            }else{
                return false;
            }
        }

        return false;
    }


#endregion

}

