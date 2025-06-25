using UnityEngine;
using System;
using System.Collections.Generic;

using Core;
using Classic;

namespace Plugins
{
	public class Configuration
	{
		public static string DefaultConfigFileName = "SlotMachines.plist";

		public static  Configuration GetInstance(string configFileName = "SlotMachines.plist")
		{
			if (instance == null)
			{
				lock (syncRoot)
				{
					if (instance == null)
					{
						instance = new Configuration();
					}
				}
			}
			return instance;
		}

		public T GetValue<T>(string key, T defaultValue)
		{
			if (parseResult == null || parseResult.RawConfiguration() == null)
			{
				return defaultValue;
			}

			return CSharpUtil.GetValue(parseResult.RawConfiguration(), key, defaultValue);
		}

		public bool SetValueWithPath<T>(string path, T value)
		{
			if (parseResult == null || parseResult.RawConfiguration() == null) return false;
			return CSharpUtil.SetValueWithPath(parseResult.RawConfiguration(), path, value);
		}

		public T GetValueWithPath<T>(string path, T defaultValue)
		{
			if (parseResult == null || parseResult.RawConfiguration() == null)
			{
				return defaultValue;
			}

			return CSharpUtil.GetValueWithPath(parseResult.RawConfiguration(), path, defaultValue);
		}

		public void CleanConfigData()
		{
			parseResult = null;
		}
		
		public void Init()
		{	
			parseResult = this.ParsePlistDictionary(ReadConfig());
			OnConfigurationParseresultReady(parseResult);
		}


		public ConfigurationParseResult ConfigurationParseResult(string configFilePath = null)
		{
			if (string.IsNullOrEmpty(configFilePath))
			{
				configFilePath = DefaultConfigFileName;
			}

			if (parseResult == null)
			{
				parseResult = this.ParsePlistDictionary(MergeQueryConfigAndPlist(null));
				OnConfigurationParseresultReady(parseResult);
			} 
			return parseResult;
		}

		// private static Dictionary<string,object> MergeQueryConfigAndPlist(string configFilePath = null)
		// {
  //           Dictionary<string,object> dictConfig = ReadConfig(configFilePath);
  //           if (dictConfig == null) return null;
  //           Dictionary<string,object> dictA = new Dictionary<string, object>();
  //           if (BaseGameConsole.singletonInstance != null && BaseGameConsole.singletonInstance.PlistLoad != null)
  //           {
	 //            dictA = BaseGameConsole.singletonInstance.PlistLoad.GetResponseData;
  //           }
  //           if(Utils.Utilities.MergeDictAtoDictB(dictConfig, dictA)) return dictA;
  //           return ReadConfig(configFilePath);
		// }
		private static Dictionary<string,object> MergeQueryConfigAndPlist(Dictionary<string,object> dictA)
		{
			Dictionary<string,object> dictConfig = ReadConfig(null);
			if (dictConfig == null)
			{
				return null;
			}
			if(dictA==null)
				dictA = new Dictionary<string, object>();
			if (Utils.Utilities.MergeDictAtoDictB(dictConfig,dictA))
			{
				return dictA;
			}
            
			return GetConfigFromFile(null);
		}
		
		public static Dictionary<string,object> GetConfigFromFile(string configFilePath = null)
		{
			if (string.IsNullOrEmpty(configFilePath)) configFilePath = DefaultConfigFileName;

			TextAsset ta2 = Resources.Load(configFilePath) as TextAsset;
			if (ta2 == null)
			{
				return null;
			}

			Dictionary<string,object> plist = CSharpUtil.ParsePlistByteArray(ta2.bytes);
			Dictionary<string, object> rtPlist = Utils.Utilities.GetValue<Dictionary<string, object>>(plist, "Data", null);
            
			return rtPlist;
		}

		private static Dictionary<string,object> ReadConfig(string configFilePath = null)
		{
			return GetConfigFromFile(configFilePath);
		}

		public static Dictionary<string,object> ReadConfigInUnityEditor(string configFilePath =null){
			return GetConfigFromFile(configFilePath);
		}
		public static bool LoadingConfig = true;

		protected delegate T ParseElement<T>(Dictionary<string,object> dict,object context);

		protected delegate T ParseDictElement<T>(string name,Dictionary<string,object> dict,object context);

		protected Dictionary<string,T> ParseDictionary<T>(Dictionary<string,object> dictObj, ParseDictElement<T> elementParse, object context)
		{
			Dictionary<string,T> result = new Dictionary<string, T>();
			foreach (string key in dictObj.Keys)
			{
				Dictionary<string,object> dict = (Dictionary<string,object>)dictObj[key];
				T tObj = elementParse(key, dict, context);
				if (tObj != null)
				{
					result.Add(key, tObj);
				}
			}
			return result;
		}

		protected List<T> ParseLists<T>(List<object> listObject, ParseElement<T> elementParse, object context)
		{
			List<T> list = new List<T>();
			foreach (object obj in listObject)
			{
				Dictionary<string,object> dict = (Dictionary<string,object>)obj;
				T tobj = elementParse(dict, context);
				if (tobj != null)
				{
					list.Add(tobj);
				}
			}
			return list;
		}

		private void OnConfigurationParseresultReady(ConfigurationParseResult parseResult)
		{
			if(parseResult.ApplicationConfig()!=null)
			{
				ApplicationConfig.GetInstance().CopyValuesFrom(parseResult.ApplicationConfig());
			}
		}

		protected ConfigurationParseResult ParsePlistDictionary(Dictionary<string,object> config)
		{
			if (config == null)
			{
				return parseResult;
			}

			ConfigurationParseResult result = new ConfigurationParseResult();
			result.SetRawConfiguration(config);
			parseResult = result;
			
			//处理level bet 相关的内容。之后等级相关的处理可以放在这里
			DataManager.GetInstance().SetData(config);
			//LineTables
			Dictionary<string,object> lineTableListDict = Utils.Utilities.GetValue<Dictionary<string,object>>(config,LineTable.LINE_TABLES_KEY,null);
			if(lineTableListDict == null) throw new ArgumentNullException ("LineTables dict not in big plist");
			Dictionary<string,LineTable> lineTables = ParseDictionary<LineTable>(lineTableListDict, LineTable.ParseLineTable, result);
			result.SetLineTables(lineTables);
			
			Dictionary<string,object> OnLineEarningConfigDict = Utils.Utilities.GetValue<Dictionary<string,object>>(config,ON_LINE_EARNING_CONFIG_KEY,null);
			if(OnLineEarningConfigDict == null) throw new ArgumentNullException ("OnLineEarningConfig dict not in big plist");
			result.SetOnLineEarningConfig(OnLineEarningConfigDict);
			
			//ApplicationConfig
			Dictionary<string,object> appConfigDict = Utils.Utilities.GetValue<Dictionary<string,object>>(config,APPLICATION_CONFIG_KEY,null);
			if(appConfigDict == null) throw new ArgumentNullException ("ApplicationConfig dict not in big plist");
			ApplicationConfig appConfig = ApplicationConfig.ParseAppConfig(appConfigDict);
			result.SetApplicationConfig(appConfig);
			
			SlotMachineConfigParse classicMachineConfig = new SlotMachineConfigParse(config);
			List<SlotMachineConfig> slotMachineConfigs = classicMachineConfig.ParseSlotConfigs(result);
			
			
			
			result.SetAllSlotMachineConfigs(slotMachineConfigs);
			result.SetSlotMachineConfigs(slotMachineConfigs);
			return result;
		}

		public const string APPLICATION_CONFIG_KEY = "ApplicationConfig";
		
		public const string ON_LINE_EARNING_CONFIG_KEY = "OnLineEarningConfig"; //网赚模式 0--》无限模式  1--》300模式

		private ConfigurationParseResult parseResult = null;
		protected static Configuration instance;
		private static object syncRoot = new System.Object();
		private static Utils.Logger logger = Utils.Logger.GetUnityDebugLogger(typeof(Configuration), false);
	}
}
