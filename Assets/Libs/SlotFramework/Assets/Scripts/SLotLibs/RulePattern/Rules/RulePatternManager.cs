using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using Libs;
using Newtonsoft.Json;

[Serializable]
public class RulePatternManager
{
    private static RulePatternManager instance = null;

    //系统中一直就存在的rule
    public List<IRuleAction> m_AllRuleActions = new List<IRuleAction>();
    private RuleCondition m_RuleCondition = null;
    public string RuleId = "";
    public int Round { set; get; }

    public RuleCondition RuleCondition
    {
        get => m_RuleCondition;
    }

    public static RulePatternManager GetInstance()
    {
        if (instance == null)
        {
//            Log.Error("rule manager create  失败");
            instance = new RulePatternManager();
        }

        return instance;
    }

    RulePatternManager()
    {
    }

    ~RulePatternManager()
    {
    }

    #region 测试skip

//    1. 首先判断是否中feature，如果中了feature，就只用feature的rule【没有feature rule代表正常使用】。
//    如果没中feature，则只用rtp的rule。
//    2.rtp的rule只在第一次create结果的时候判断下skip的概率。如果需要再次create结果则不再判断概率。之后百分之百跳过

    public bool EvalSpinResult(SpinResultData data,RuleSwitch ruleSwitch)
    {
        if (ruleSwitch.IsOpenFeature)
        {
            if (data.IsFeature)
            {
                return DoFeatureRuleSkip(data);
            }
            else
            {
                return DoRTPRuleSkip(data);
            }
        }
        else
        {
            return DoRTPRuleSkip(data);
        }
        
    }

    private bool DoRTPRuleSkip(SpinResultData data)
    {
        //按照rtp的逻辑处理
        List<IRuleAction> ruleActions = m_AllRuleActions.FindAll(delegate(IRuleAction ruleAction)
        {
            return ruleAction.GetRuleType() == RuleActionType.RTP;

        });
        if (ruleActions == null)
        {
            return false;
        }
                
        bool isSkipResult = ruleActions.Exists(delegate(IRuleAction ruleAction)
        {
            bool ret = ruleAction.CheckdResultSkip(data);
            if(ret == true) Log.LogLimeColor($"{ruleAction.GetRuleType()} rule recreate");
            return ret;
        });
        return isSkipResult;
    }

    private bool DoFeatureRuleSkip(SpinResultData data)
    {
        //有feature rule即根据ruleskip，否则将不跳过，直接执行feature。
        IRuleAction ruleAction = m_AllRuleActions.Find(delegate(IRuleAction action)
        {
            return action.GetRuleType() == RuleActionType.FEATURE;
        });
        if(ruleAction !=null)
        {
            bool ret = ruleAction.CheckdResultSkip(data);
            if(ret == true) Log.LogLimeColor($"{ruleAction.GetRuleType()} rule recreate");
            return ret;
        }
        else
        {
            return false; //不跳过，直接执行feature
        }
    }
        
        
//        //排除掉Feature的影响
//        List<IRuleAction> ruleActions = m_AllRuleActions.FindAll(delegate(IRuleAction ruleAction)
//        {
//            if (ruleAction.GetRuleType() == RuleActionType.FEATURE)
//                return ruleSwitch.IsOpenFeature;
//            else if (ruleAction.GetRuleType() == RuleActionType.RTP)
//                return ruleSwitch.IsOpenRtp;
//            else return false;
//            });
//        
//        //按照后端的顺序来
//        bool isSkipResult = ruleActions.Exists(delegate(IRuleAction ruleAction)
//            {
//                bool ret = ruleAction.CheckdResultSkip(data);
//                if(ret == true) Log.LogLimeColor($"{ruleAction.GetRuleType()} rule recreate");
//                return ret;
//            });
//        return isSkipResult;
    

    public void RuleAddOneTime()
    {
        if (this.m_RuleCondition != null)
        {
            this.m_RuleCondition.AddOneTime();
        }
    }
    public bool RuleConditionValid(long _balance,IRuleResult ruleMid)
    {
        //满足condition存在，condition未失效、rules数组不为空
        if (this.m_RuleCondition == null)
        {
            //清除 ruleid字段
            ClearRuleData(ruleMid);
            return false;
        }

        if (this.m_RuleCondition.ConditionInvalid(_balance))
        {
            ClearRuleData(ruleMid);
            return false;
        }

        if (this.m_AllRuleActions.Count <= 0)
        {
            ClearRuleData(ruleMid);
            return false;
        }
        return true;
    }

    
    private void ClearRuleData(IRuleResult ruleMid)
    {
        if (ruleMid !=null &&  GetConfigRuleAction() != null)
        {
            ruleMid.ClearServerConfig();
        }
        
        this.m_RuleCondition = null;
        this.m_AllRuleActions.Clear();
        this.RuleId = "";
        this.Round = 0;
    }

//    public void Test()
//    {
//        IRuleAction rtp = new RtpRuleAction();
//        (rtp as RtpRuleAction).MakeTestData();
//        
//        this.m_AllRuleActions.Add(rtp);
//        IRuleAction feature = new FeatureRuleAction();
//        (feature as FeatureRuleAction).MakeTestData();
//        this.m_AllRuleActions.Add(feature);
//    }
    #endregion

    #region 初始化create

    public void InitRuleInfo(Dictionary<string, object> rulesObj,long _balance)
    {
        if (rulesObj == null) return;
        m_RuleCondition = null;
        this.m_AllRuleActions.Clear();
        
        if (rulesObj.ContainsKey("stop_at"))
        {
            this.m_RuleCondition = new RuleCondition(rulesObj["stop_at"] as Dictionary<string, object>,_balance);    
        }

        if (rulesObj.ContainsKey("rule"))
        {
//           TODO: 校验完备性，数组里的rule都必须能够解析，有一个解析那就stop抛弃。
            int count = 0;
            List<object> list = rulesObj["rule"] as List<object>;
            for (int i = 0; i < list.Count; i++)
            {
                Dictionary<string, object> dic = list[i] as Dictionary<string, object>;
                if (dic == null) continue;
                if (dic.ContainsKey("name"))
                {
                    string name = dic["name"].ToString();
                    if (name.Equals("d3"))//nearmiss
                    {
                    
                    }
                    else if (name.Equals("d1")) //rtp
                    {
                        IRuleAction action= new RtpRuleAction(dic);
                        this.m_AllRuleActions.Add(action);
                        Log.LogWhiteColor($"{action.GetRuleType()} Rule Create");
                        count++;
                    }
                    else if (name.Equals("d2"))//feature
                    {
                        IRuleAction action= new FeatureRuleAction(dic);
                        this.m_AllRuleActions.Add(action);
                        Log.LogWhiteColor($"{action.GetRuleType()} Rule Create");
                        count++;
                    }
                    else if (name.Equals("d4"))
                    {
                        IRuleAction action = new ConfigRuleAction(dic);
                        this.m_AllRuleActions.Add(action);
                        Log.LogLimeColor($"{action.GetRuleType()} Rule Create");
                        count++;
                    }
                }
                
            }

            if (count != list.Count)
            {
                this.ClearRuleData(null);
                return;
            }
        }

        if (rulesObj.ContainsKey("rule_id"))
        {
            RuleId = rulesObj["rule_id"].ToString();
//            Log.LogWhiteColor(RuleId);
        }
    }

    #endregion

    #region ruleConfig相关

    public ConfigRuleAction GetConfigRuleAction()
    {
        List<IRuleAction> ruleActions = m_AllRuleActions.FindAll(delegate(IRuleAction ruleAction)
        {
            return ruleAction.GetRuleType() == RuleActionType.REPACE_CONFIG;

        });
        if(ruleActions!=null && ruleActions.Count >0)
        {

            return ruleActions[0] as ConfigRuleAction;
        }
        else
        {
            return null;
        }
    }

    #endregion

    #region save存储
    private static readonly  string RuleFolderName = "RuleDataProgress";
    //数据最好跟mid相关
    public static void SaveLocalRuleData()
    {
        //先不处理
        return;
        if (instance == null || instance.m_RuleCondition == null)
        {
            return;
        }

//        if (instance.CheckRuleConditionOk() == false) return;
        //TODO
        string folderPath = Path.Combine (Application.persistentDataPath,RuleFolderName);
        if (!Directory.Exists (folderPath)) {
            Directory.CreateDirectory (folderPath);
        }
        
        string path = Path.Combine (folderPath, RuleFolderName + ".json");
        try
        {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(instance); //JsonUtility.ToJson (obj,true);

            File.WriteAllText(path, json);

//            LoadLocalRuleData();
        } catch(Exception e){
            Debug.LogError (path+"解析rule json数据报错"+e.Message);
        }
    }

    public static void LoadLocalRuleData()
    {
        //先不处理
        return;
        
        string folderPath = Path.Combine (Application.persistentDataPath, RuleFolderName);
        string path = Path.Combine (folderPath, RuleFolderName + ".json");
        if (File.Exists (path)) {
            string jsonData = File.ReadAllText (path);
            try{
                var settings = new JsonSerializerSettings();
//                settings.Converters.Add(new RuleActionConverter());
                
                instance = Newtonsoft.Json.JsonConvert.DeserializeObject<RulePatternManager>(jsonData,settings);
                
            }
            catch(Exception e){
                Debug.LogError (path+"解析rule json数据报错"+e.Message);
            }

        }

        if (instance == null)
        {
            instance = new RulePatternManager();
        }
    }

    #endregion
}