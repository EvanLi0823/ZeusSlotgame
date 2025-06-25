using System.Collections.Generic;
using System.Linq;
using Classic;

namespace Libs
{
    public class ServerBetDataMock
    {
        public static Dictionary<string,bool> flagDict = new Dictionary<string, bool>();
        private static Dictionary<string, object> config = null;
        public static void AddServerBetData(Dictionary<string,object> dict,string trigger)
        {
            #if UNITY_EDITOR
            if (dict == null) return;
            if (string.IsNullOrEmpty(trigger)) return;
           // if (flagDict.ContainsKey(trigger)) return;

            switch (trigger)
            {
                case GameConstants.TRIGGER_EVENT_APPOPEN_EVENT:
                   if(dict.ContainsKey(GameConstants.TRIGGER_EVENT_USER_STATUS_KEY)) 
                       dict.Remove(GameConstants.TRIGGER_EVENT_USER_STATUS_KEY);
                   
                    if (!dict.ContainsKey(GameConstants.TRIGGER_EVENT_USER_STATUS_KEY))
                    {
                        string data = GetBetDataStr();
                        dict.Add(GameConstants.TRIGGER_EVENT_USER_STATUS_KEY,MiniJSON.Json.Deserialize(data));
                    }
                    break;
                case GameConstants.TRIGGER_EVENT_LEVELUP_EVENT:
                    if (!dict.ContainsKey(trigger))
                    {
                        dict.Add(trigger,null);
                    }

                    config = dict[trigger] as Dictionary<string, object>;
                    if (config!=null&&config.ContainsKey(GameConstants.TRIGGER_EVENT_USER_STATUS_KEY))
                    {
                        config.Remove(GameConstants.TRIGGER_EVENT_USER_STATUS_KEY);
                    
                    }
                    if (config==null||!config.ContainsKey(GameConstants.TRIGGER_EVENT_USER_STATUS_KEY))
                    {
                        if(config==null) config = new Dictionary<string, object>();
                        string data = GetBetDataStr();
                    
                        config.Add(GameConstants.TRIGGER_EVENT_USER_STATUS_KEY,MiniJSON.Json.Deserialize(data));
                        dict[trigger] = config;
                    }
                    break;
                case GameConstants.TRIGGER_EVENT_PURCHASE_EVENT:
                    if (!dict.ContainsKey(trigger))
                    {
                        dict.Add(trigger,null);
                    }

                    config = dict[trigger] as Dictionary<string, object>;
                    if (config!=null&&config.ContainsKey(GameConstants.TRIGGER_EVENT_USER_STATUS_KEY))
                    {
                        config.Remove(GameConstants.TRIGGER_EVENT_USER_STATUS_KEY);
                    }
                    if (config==null||!config.ContainsKey(GameConstants.TRIGGER_EVENT_USER_STATUS_KEY))
                    {
                        if(config==null) config = new Dictionary<string, object>();
                        string data = GetBetDataStr();
                    
                        config.Add(GameConstants.TRIGGER_EVENT_USER_STATUS_KEY,MiniJSON.Json.Deserialize(data));
                        dict[trigger] = config;
                    }
                    break;
                case GameConstants.TRIGGER_EVENT_ENTER_MACHINE_KEY:
                    if (!dict.ContainsKey(trigger))
                    {
                        dict.Add(trigger,null);
                    }

                    config = dict[trigger] as Dictionary<string, object>;
                    if (config!=null&&config.ContainsKey(GameConstants.TRIGGER_EVENT_USER_STATUS_KEY))
                    {
                        config.Remove(GameConstants.TRIGGER_EVENT_USER_STATUS_KEY);
                    }
                    if (config==null||!config.ContainsKey(GameConstants.TRIGGER_EVENT_USER_STATUS_KEY))
                    {
                        if(config==null) config = new Dictionary<string, object>();
                        string data = GetBetDataStr();
                        config.Add(GameConstants.TRIGGER_EVENT_USER_STATUS_KEY,MiniJSON.Json.Deserialize(data));
                        dict[trigger] = config;
                    }
                    break;

            }
          
           if(!flagDict.ContainsKey(trigger)) flagDict.Add(trigger,true);
            #endif
        }

        public static string GetBetDataStr()
        {
            string data = "";
//            if (string.IsNullOrEmpty(data))
//            {
//                return data;
//            }
            int level = UserManager.GetInstance().UserProfile().Level();
            if (level >= 15)
            {
                data = "{\"avg_bet\":205116,\"init_bet\":225000,\"level_exp\":[3677941,19425,4.37283881095619],\"op_feature_bet\":4000,\"rcmd_bet\":37500,\"feature_open_bet\":[5000],\"user_bet_list\":[100,750,1500,2000,3750,7500,17500,37500,75000,87500,125000,175000,225000,375000,500000,875000,1250000,1750000,2250000,3750000]}";
            }
            else if (level>=10)
            {
                data = "{\"avg_bet\":205116,\"init_bet\":225000,\"level_exp\":[3677941,19425,4.37283881095619],\"op_feature_bet\":3000,\"rcmd_bet\":37500,\"feature_open_bet\":[3750],\"user_bet_list\":[100,750,1500,2000,3750,7500,17500,37500]}";
            }
            else if (level>=8)
            {
                data = "{\"avg_bet\":205116,\"init_bet\":225000,\"level_exp\":[3677941,19425,4.37283881095619],\"op_feature_bet\":1200,\"rcmd_bet\":37500,\"feature_open_bet\":[2000],\"user_bet_list\":[100,750,1500,2000,3750,7500]}";
            }
            else if (level>=6)
            {
                data = "{\"avg_bet\":205116,\"init_bet\":225000,\"level_exp\":[3677941,19425,4.37283881095619],\"op_feature_bet\":1000,\"rcmd_bet\":37500,\"feature_open_bet\":[1500],\"user_bet_list\":[100,750,1500,2000,3750]}";

            }
            else if (level >= 4)
            {
                data = "{\"avg_bet\":205116,\"init_bet\":1500,\"level_exp\":[3677941,19425,4.37283881095619],\"op_feature_bet\":500,\"rcmd_bet\":37500,\"feature_open_bet\":[750],\"user_bet_list\":[100,750,1500,2000]}";
            }
            else if (level >= 2)
            {
                data = "{\"avg_bet\":205116,\"init_bet\":150,\"level_exp\":[3677941,19425,4.37283881095619],\"op_feature_bet\":200,\"rcmd_bet\":37500,\"feature_open_bet\":[100],\"user_bet_list\":[100,750]}";
            }
            else
            {
                data = "{\"avg_bet\":205116,\"init_bet\":100,\"level_exp\":[3677941,19425,4.37283881095619],\"op_feature_bet\":50,\"rcmd_bet\":37500,\"feature_open_bet\":[50],\"user_bet_list\":[100]}";
            }

            return data;
        }
    }
}