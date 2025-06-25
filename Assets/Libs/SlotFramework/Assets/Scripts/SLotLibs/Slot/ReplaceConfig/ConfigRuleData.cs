using System.Collections.Generic;
using RealYou.Utility.Log;
using Classic;
using UnityEngine;
using Utils;
using Logger = RealYou.Utility.Log.Logger;

namespace Libs
{
    public class ConfigRuleData
    {
        private ConfigRuleItem ServerItem;
        
        //原始的数据信息
        private ReelStripManager OriginReelStrip;
        private ExtroInfos OriginExtroInfos;
        public List<BoardStripConfig> OriginSubBoardStripConfigList;
//        private Dictionary<string, object> OriginSlotDict;
        public string CurrentUseSnippetName="";

        private ReelManager m_ReelManager;

        public ConfigRuleData(ReelManager _reelManager)
        {
            m_ReelManager = _reelManager;
            OriginReelStrip = _reelManager.reelStrips;
            OriginExtroInfos = _reelManager.slotConfig.extroInfos;
            OriginSubBoardStripConfigList = _reelManager.slotConfig.SubBoardStripConfigList;
        }
        
        public void SetConfigData(Dictionary<string, object> serverData)
        {
            if (ServerItem == null)
            {
                ServerItem = new ConfigRuleItem();
            }

            if (m_ReelManager == null)
                return;
            ServerItem.SetConfigData(serverData,m_ReelManager.slotConfig,this.m_ReelManager.symbolMap);
        }
        
        public void SetServerData(Dictionary<string, object> serverData)
        {
            string machineName = serverData["machine_name"].ToString();
            if (m_ReelManager == null || !m_ReelManager.slotConfig.Name().Equals(machineName))
                return;
            if (ServerItem == null)
            {
                ServerItem = new ConfigRuleItem();
            }
            ServerItem.SetServerData(serverData,m_ReelManager.slotConfig,this.m_ReelManager.symbolMap);
        }
        public bool IsInUseServerConfig()
        {
            return !string.IsNullOrEmpty(CurrentUseSnippetName);
        }

        #region spin 创建结果处理数据
        //做config的替换操作，判断是否在使用中且使用的是对应的snippet name，是的话跳过；
        //否则：正在使用中的snippet 是别的则切换成正常的config且 如果有设置server数据；
        //队列里有snippet name的config却没有使用的话才使用一次；没有snippet name则需添加标记；
        //特例： 需要判断是否已经正在使用configrule，没使用的时候需要清除config，放在clearRule方法里处理
        public void CreateSpinRet(string snippetName)
        {
            RulePatternManager.GetInstance().RuleAddOneTime();
            
            Log.LogLimeColor($"rule config : using rule ");
            
            if(CurrentUseSnippetName.Equals(snippetName))
                return;
            if (!IsInUseServerConfig())
            {
                ExchangeToServerData(snippetName);
            }
            else if(!snippetName.Equals(CurrentUseSnippetName))
            {
                //看本地有没有远程的config
                if (ServerItem != null && ServerItem.Snippet.Equals(snippetName) && m_ReelManager.slotConfig.Name().Equals( ServerItem.MachineName))
                {
                    ExchangeToServerData(snippetName); 
                }
                else
                {
                    ExchangeToOrigin();
                }
            }

            if (IsInUseServerConfig())
            {
                Log.LogLimeColor($"rule config:ServerConfig--  {snippetName} ");
            }
        }

        #endregion

        #region 替换config
        public void ExchangeToOrigin()
        {
            this.m_ReelManager.slotConfig.extroInfos = this.OriginExtroInfos;
            this.m_ReelManager.slotConfig.MainBoardStripConfig.BoardStrips = this.OriginReelStrip;
            this.m_ReelManager.reelStrips = this.OriginReelStrip;
            this.m_ReelManager.slotConfig.SubBoardStripConfigList = this.OriginSubBoardStripConfigList;
            
            this.m_ReelManager.ReplaceConfigData();
            
            Log.LogLimeColor("ExchangeToOrigin");
            
            //仍需要调用替换spin的带子
            this.m_ReelManager.ChangeResultContentOnSpin(this.m_ReelManager.reelStrips, this.m_ReelManager.gameConfigs);
            CurrentUseSnippetName = "";
//            Print();
        }

        public void ExchangeToServerData(string snippetName)
        {
            if (ServerItem == null)
                return ;

            if (string.IsNullOrEmpty(snippetName))
            {
                return;
            }

            if (!snippetName.Equals(ServerItem.Snippet))
            {
                return;
            }

            if (!this.m_ReelManager.slotConfig.Name().Equals(ServerItem.MachineName))
            {
                return;
            }

            bool isReplace = false;
            if(ServerItem.extroInfos != null)
            {
                this.m_ReelManager.slotConfig.extroInfos = ServerItem.extroInfos;
                isReplace = true;
            }
            if(ServerItem.ReelStrip!=null)
            {
                this.m_ReelManager.slotConfig.MainBoardStripConfig.BoardStrips = ServerItem.ReelStrip;
                this.m_ReelManager.reelStrips = ServerItem.ReelStrip;
                this.m_ReelManager.reelStrips.InitStrips();
                
                isReplace = true;
            }

            if (ServerItem.SubBoardStripConfigList != null)
            {
                this.m_ReelManager.slotConfig.SubBoardStripConfigList = ServerItem.SubBoardStripConfigList;
                isReplace = true;
            }
            
            if(isReplace)
            {
                this.m_ReelManager.ReplaceConfigData();
                
                this.m_ReelManager.ChangeResultContentOnSpin(this.m_ReelManager.reelStrips, this.m_ReelManager.gameConfigs);
                CurrentUseSnippetName = snippetName;
            }

//            Print();
        }

        public void ClearServerConfigData()
        {
            if(IsInUseServerConfig())
            {
                ExchangeToOrigin();
            }
            
            this.ServerItem = null;
            Log.LogLimeColor($"rule config: clear");
        }
        private void Print()
        {
//            Debug.Log( Utilities.CastValueInt(this.m_ReelManager.slotConfig.extroInfos.infos["UnlockSFSBet"]));
        }
        
        #endregion
    }


    class ConfigRuleItem
    {
        public string MachineName;
        public string Snippet ="rtp_20";
        public Dictionary<string, object> ServerDictData;

        //服务器转换的数据
        public ReelStripManager ReelStrip;    
        public ExtroInfos extroInfos;
        public List<BoardStripConfig> SubBoardStripConfigList;

        public void SetServerData(Dictionary<string, object> serverData,SlotMachineConfig machineConfig, SymbolMap symbolMap)
        {
            //https://yapi.inhyperloop.com/project/14/interface/api/3096

            if (serverData == null)
                return;
            
            this.MachineName = serverData["machine_name"].ToString();
            Snippet = serverData["snippet"].ToString();

            Dictionary<string, object> plistData =  serverData["config"] as Dictionary<string, object>;
            this.SetConfigData(plistData,machineConfig,symbolMap);
        }
        
        public void SetConfigData(Dictionary<string, object> plistData,SlotMachineConfig machineConfig, SymbolMap symbolMap)
        {
            Dictionary<string, object> originDict = machineConfig.PlistDict;
            this.ServerDictData = plistData;
            extroInfos =  null;
            ReelStrip = null;
            SubBoardStripConfigList = null;
            
            
//            Dictionary<string,object> mergeFrom = new Dictionary<string, object>(originDict);
//            Utils.Utilities.MergeDictAtoBNoCover(originDict, plistData);

            if(Logger.Level != LogLevel.None)
            {
                string str = MiniJSON.Json.Serialize(originDict);
                Logger.GlobalLogger.Log(str);
            }
            
            
            if (plistData.ContainsKey(ReelStripManager.MACHINE_MATH_NODE))
            {
                Dictionary<string, object> serverDict = plistData[ReelStripManager.MACHINE_MATH_NODE] as Dictionary<string, object>;
                Dictionary<string, object> localDict = originDict[ReelStripManager.MACHINE_MATH_NODE] as Dictionary<string, object>;
                
                Utils.Utilities.MergeDictAtoBNoCover(localDict, serverDict);
                
                this.ReelStrip = new ReelStripManager(symbolMap,serverDict);
            }
            
            if(plistData.ContainsKey(ExtroInfos.EXTRA_INFOS))
            {
                Dictionary<string, object> serverDict = plistData[ExtroInfos.EXTRA_INFOS] as Dictionary<string, object>;
                Dictionary<string, object> localDict = originDict[ExtroInfos.EXTRA_INFOS] as Dictionary<string, object>;
                
                Utils.Utilities.MergeDictAtoBNoCover(localDict, serverDict);
                
                this.extroInfos = new ExtroInfos(serverDict);
            }
            
            if(plistData.ContainsKey(SlotMachineConfig.SUB_BOARDS))
            {
                this.SubBoardStripConfigList = new List<BoardStripConfig>();
                List<object> list = plistData[SlotMachineConfig.SUB_BOARDS] as List<object>;
                for(int i =0; i < list.Count; i++)
                {
                    Dictionary<string, object> o = list[i] as Dictionary<string, object>;
                    BoardStripConfig config = new BoardStripConfig();
                    config.InitStripConfig(o,machineConfig);
                    this.SubBoardStripConfigList.Add(config);
                }
            }
            
            Log.LogLimeColor("server config received");
        }

        private void LayoutData()
        {
            
        }
    }
}

