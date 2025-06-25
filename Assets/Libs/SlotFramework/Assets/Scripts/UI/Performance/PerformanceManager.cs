using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Utils;
using System.Text.RegularExpressions;

namespace UI.Performance
{
	public class PerformanceManager
	{

		public static PerformanceManager Instance {
			get {
				if (singletonInstance == null) {
					lock (syncRoot) {
						if (singletonInstance == null) {
							singletonInstance = new PerformanceManager ();
						}

					}
				}
				return singletonInstance;
			}
		}

		public void Register(IGPUConsumer consumer)
		{
			gpuConsumers.Add (consumer);
		}

		public string Version()
		{
			return Plugins.Configuration.GetInstance ().GetValueWithPath<string> ("/Devices/PerformanceManagerVersion", DefaultVersion);
		}

		public float GPUScore()
		{
			float score;
			Version versionInConfig, currentVersion;
			versionInConfig = new Version (Version ());
			currentVersion = new System.Version(PlayerPrefs.GetString("PerformanceManagerVersion",DefaultVersion));
            if (versionInConfig.CompareTo (currentVersion) > 0 || PlayerPrefs.GetFloat ("GPUScore", -1f) < 0) {

				score = DeviceModelScoreInConfigFile();

				if (score < 0) {
					score = GPUScoreInConfigFile ();
				}
				
				if (score < 0) {
					score = EstimateGPUScore ();
				}

				if (score < 0) {
					float defaultScore = DefaultGPUScore ();
					score = defaultScore;
				}

				PlayerPrefs.SetFloat ("GPUScore", score);
				PlayerPrefs.SetString ("PerformanceManagerVersion", versionInConfig.ToString ());
			} else {
				score = PlayerPrefs.GetFloat ("GPUScore",GPUDefaultScore);
			}
			return score;
		}



		public float DefaultGPUScore()
		{
			return Plugins.Configuration.GetInstance ().GetValueWithPath<float> ("/Devices/GPUDefaultScore", GPUDefaultScore);
		}

		public float GPUThresholdScore()
		{
			return Plugins.Configuration.GetInstance ().GetValueWithPath<float> ("/Devices/GPUThresholdScore", GPUThreasholdScore);
		}
		
		public virtual float GPUScoreInConfigFile(string graphicsDeviceName = null)
		{
			float score = -1.0f;

			if (string.IsNullOrEmpty (graphicsDeviceName)) {
				graphicsDeviceName = SystemInfo.graphicsDeviceName;
			}

			Dictionary<string, object> parms = Plugins.Configuration.GetInstance ().GetValueWithPath<Dictionary<string, object>> ("/Devices/GPUs/" + graphicsDeviceName, null);
			object scoreObj;
			if (parms != null && parms.TryGetValue("Score",out scoreObj)) {
				score = Convert.ToSingle(scoreObj);
			}
			return score;
		}

		public virtual float DeviceModelScoreInConfigFile(string deviceModel = null)
		{

			float score = -1.0f;
			
			if (string.IsNullOrEmpty (deviceModel)) {
				deviceModel = SystemInfo.deviceModel;
			}
			
			Dictionary<string, object> parms = Plugins.Configuration.GetInstance ().GetValueWithPath<Dictionary<string, object>> ("/Devices/Models/" + deviceModel, null);
			object scoreObj;
			if (parms != null && parms.TryGetValue("Score",out scoreObj)) {
				score = Convert.ToSingle(scoreObj);
			}
			return score;
		}
		
		public virtual float EstimateGPUScore(string graphicsDeviceName = null,float defaultScore = -1.0f)
		{
			float score = defaultScore;
			int modelNumber = GPUModelNumber (graphicsDeviceName);
			
			if (modelNumber > 0) {
				Dictionary<string,object> family = GPUsInFamilityInConfig (GPUFamilyName (graphicsDeviceName));
				foreach(string gpu in family.Keys) {
					object scoreObj;
					float aGPUScore = -1.0f;
					if ((family[gpu] as Dictionary<string,object>).TryGetValue("Score",out scoreObj)){
						 aGPUScore = Convert.ToSingle(scoreObj);
					}

					int aGPUModelNumber = GPUModelNumber(gpu);
					if (aGPUModelNumber >= modelNumber && aGPUScore < score) {
						score = aGPUScore;
					} else if (aGPUModelNumber < modelNumber && aGPUScore > score){
					    score = aGPUScore;
					}
				}
			}
			
			return score;
			
		}
		
		public virtual Dictionary<string,object> GPUsInFamilityInConfig(string gpuFamilyName)
		{
			Dictionary<string,object> gpuFamily = new Dictionary<string, object>();
			
			Dictionary<string, object> allGPUs = Plugins.Configuration.GetInstance ().GetValueWithPath<Dictionary<string, object>> ("/Devices/GPUs", null);
			if (allGPUs != null) {
				foreach (string key in allGPUs.Keys) {
					if (key.TrimStart ().StartsWith (gpuFamilyName)) {
						gpuFamily.Add (key, allGPUs [key]);
					}
				}
			}
			return gpuFamily;
		}
		
		public virtual string GPUFamilyName(string graphicsDeviceName = null)
		{
			if (string.IsNullOrEmpty(graphicsDeviceName)) {
				graphicsDeviceName = SystemInfo.graphicsDeviceName;
			}
			
			Dictionary<string,object> config = Plugins.Configuration.GetInstance ().ConfigurationParseResult ().RawConfiguration ();
			string familtyName = RegexSingleGroupValue(graphicsDeviceName,
			                                           CSharpUtil.GetValueWithPath<string>(config,"/Devices/GPUFamiltyNameFormat","(\\w+)(\\s[\\(\\)\\w]+)+"),
			                                           CSharpUtil.GetValueWithPath<int>(config,"/Devices/GPUFamiltyNameGroupIndex",0)
			                                           );
			return familtyName;
		}
		
		public virtual int GPUModelNumber(string graphicsDeviceName = null)
		{
			
			int modelNumber = -1;
			
			if (string.IsNullOrEmpty(graphicsDeviceName)) {
				graphicsDeviceName = SystemInfo.graphicsDeviceName;
			}
			
			Dictionary<string,object> config = Plugins.Configuration.GetInstance ().ConfigurationParseResult ().RawConfiguration ();
			string numberString = RegexSingleGroupValue(graphicsDeviceName,
			                                            CSharpUtil.GetValueWithPath<string>(config,"/Devices/GPUModelNumberFormat","([\\w\\(\\)]+\\s)+(\\d+)(\\s[\\w\\(\\)]+)*"),
			                                            CSharpUtil.GetValueWithPath<int>(config,"/Devices/GPUModelNumberGroupIndex",1)
			                                            );
			try {
				modelNumber = Convert.ToInt32(numberString);
			} catch (Exception e) {
				logger.LogWarningF("Found invalid ModelNumber. GraphicsDeviceName:{0}.Error:{1}",graphicsDeviceName,e);
			}
			
			return modelNumber;
		}
		
		public virtual string GPUModel(string graphicsDeviceName = null)
		{
			string modelName = "";
			
			if (string.IsNullOrEmpty(graphicsDeviceName)) {
				graphicsDeviceName = SystemInfo.graphicsDeviceName;
			}
			
			Dictionary<string,object> config = Plugins.Configuration.GetInstance ().ConfigurationParseResult ().RawConfiguration ();
			modelName = RegexSingleGroupValue(graphicsDeviceName,
			                                  CSharpUtil.GetValueWithPath<string>(config,"/Devices/GPUModelNameFormat","([\\w\\(\\)\\b]+\\s)+([\\b\\w]+)$"),
			                                  CSharpUtil.GetValueWithPath<int>(config,"/Devices/GPUModelNameGroupIndex",1)
			                                  );
			
			return modelName;
		}

		public bool IsLowEndGPU()
		{
			bool isLowEnd = false;
			if (PerformanceManager.Instance.GPUScore () < PerformanceManager.Instance.GPUThresholdScore()) {
				isLowEnd = true;
			}
			return isLowEnd;
		}

		private string RegexSingleGroupValue(string input, string pattern, int groupIndex = 0)
		{
			string ret = "";
			
			MatchCollection matches = Regex.Matches(input,pattern,RegexOptions.IgnoreCase);
			if (matches.Count == 1 && matches[0].Groups.Count > groupIndex) {
				ret = matches[0].Groups[groupIndex+1].Value;
			}
			return ret;
		}
	

		private List<IGPUConsumer> gpuConsumers = new List<IGPUConsumer> ();

		protected static PerformanceManager singletonInstance;

		private static readonly string DefaultVersion = "1.0.0";
		private static readonly float GPUDefaultScore = 30;
		private static readonly float GPUThreasholdScore = 21.6f; //Refer to A6(iPhone5C):25.5

		private static object syncRoot = new System.Object ();
		private static global::Utils.Logger logger = global::Utils.Logger.GetUnityDebugLogger(typeof(PerformanceManager),false);
	}
}
