using UnityEngine;
using System.Collections;
using NUnit.Framework;
using System.Collections.Generic;

namespace UI.Performance
{
	[TestFixture()]
	public class PerformanceManagerTests
	{

		[TestFixtureSetUp]
		public void FixtureSetup()
		{
			config = Plugins.Configuration.GetInstance ().ConfigurationParseResult ().RawConfiguration();
			originDevices = config.ContainsKey("Devices")?(config ["Devices"] as Dictionary<string,object>):new Dictionary<string,object>();
		}

		[SetUp] 
		public void Setup()
		{
			config.Remove ("Devices");
		}

		[TearDown]
		public void TearDown()
		{
			config["Devices"] = originDevices;
		}

		[Test()]
		public void TestGPUScoreInConfigFile_OK(
			[Values(10.0f)] float score
			)
		{
			AddGPU("ABC",score);
			Assert.AreEqual (score, PerformanceManager.Instance.GPUScoreInConfigFile ("ABC"));
		}

		[Test()]
		public void TestEstimateGPUScore_NoGPUFamily_OK(

			)
		{
			AddGPU ("ABC");
			RemoveGPU ("ABC");

			Assert.AreEqual (PerformanceManager.Instance.DefaultGPUScore(), PerformanceManager.Instance.EstimateGPUScore ("ABC 1",PerformanceManager.Instance.DefaultGPUScore()));
		}

		[Test(),Sequential]
		public void TestEstimateGPUScore_OnlyLowEndGPUInConfig_OK(
			[Values(-1.0f, 0f, 1f,  10f, 100f)] float defaultScore,
			[Values(10f,  10f, 10f, 10f, 100f)] float expectedScore
			)
		{
			AddGPU ("ABC 1",10.0f);
			Assert.AreEqual (expectedScore, PerformanceManager.Instance.EstimateGPUScore ("ABC 2",defaultScore));
		}

		[Test(),Sequential]
		public void TestEstimateGPUScore_OnlyHighEndGPUInConfig_OK(
			[Values(-1.0f, 0f, 1f,  10f, 200f)] float defaultScore,
			[Values(-1.0f, 0f, 1f,  10f, 120f)] float expectedScore
			)
		{
			AddGPU ("ABC 100",120.0f);
			Assert.AreEqual (expectedScore, PerformanceManager.Instance.EstimateGPUScore ("ABC 2",defaultScore));
		}

		[Test(),Sequential]
		public void TestEstimateGPUScore_BothLowAndHighEndGPUInConfig_OK(
			[Values(-1.0f, 0f, 1f,  10f, 20f, 100f, 120f, 130f,200f, 200f)] float defaultScore,
			[Values(10f,  10f, 10f, 10f, 20f, 100f, 120f, 120f,120f, 120f)] float expectedScore
			)
		{
			AddGPU ("ABC 1",10.0f);
			AddGPU ("ABC 100",120.0f);
			Assert.AreEqual (PerformanceManager.Instance.DefaultGPUScore(), PerformanceManager.Instance.EstimateGPUScore ("ABC 2",PerformanceManager.Instance.DefaultGPUScore()));
		}

		[Test()]
		public void TestGPUsInFamilityInConfig_OK()
		{

			SetupTestGPUsInFamilityInConfig_OK ();
			Dictionary<string,object> gpus = PerformanceManager.Instance.GPUsInFamilityInConfig ("A");

			int A_GPU_NUMBER = 4;
			Assert.AreEqual (A_GPU_NUMBER, gpus.Count);
			Assert.IsTrue (gpus.ContainsKey ("A 1"));
			Assert.IsTrue (gpus.ContainsKey ("A 12"));
			Assert.IsTrue (gpus.ContainsKey ("A 123"));
			Assert.IsTrue (gpus.ContainsKey ("A (TM) 1234"));
		}

		[Test(),Sequential]
		public void TestGPUFamilyName_OneCharName_OK(
			[Values("A 1","A 12","A 123","A (TM) 1234")] string gpuDeviceName,
			[Values("A",  "A",   "A",    "A")] string expectedFamilyName
			)
		{
			Assert.AreEqual(expectedFamilyName,PerformanceManager.Instance.GPUFamilyName (gpuDeviceName));
		}

		[Test(),Sequential]
		public void TestGPUFamilyName_Name(
			[Values("ABC 1","ABCD 12","AC 123","AS (TM) 1234")] string gpuDeviceName,
			[Values("ABC",  "ABCD",   "AC",    "AS")] string expectedFamilyName
			)
		{
			Assert.AreEqual(expectedFamilyName,PerformanceManager.Instance.GPUFamilyName (gpuDeviceName));
		}

		[Test(),Sequential]
		public void TestGPUModelNumber_OK(
			[Values("A 1","A 12","A 123","A (TM) 1234","A (TM) 1234 XP")] string gpuDeviceName,
			[Values(1,    12,     123,    1234,        1234)] int expectedModelNumber
			)
		{
			Assert.AreEqual(expectedModelNumber,PerformanceManager.Instance.GPUModelNumber (gpuDeviceName));
		}

		[Test(),Sequential]
		public void TestGPUModel_OK(
			[Values("A 1","A 12","A 123","A (TM) 1234XP")] string gpuDeviceName,
			[Values("1",  "12",  "123",  "1234XP")] string expectedModel
			)
		{
			Assert.AreEqual(expectedModel,PerformanceManager.Instance.GPUModel (gpuDeviceName));
		}

		void SetupTestGPUsInFamilityInConfig_OK ()
		{
			AddGPU ("A 1");
			AddGPU ("A 12");
	        AddGPU ("A 123");
			AddGPU ("A (TM) 1234");

			AddGPU ("B (TM) 1");
			AddGPU ("C (TM) 12");
			AddGPU ("D (TM) 123");
			AddGPU ("E (TM) 1234");
		}

		private void AddGPU(string gpuName, float score)
		{
			Dictionary<string,object> gpuInfo = new Dictionary<string, object> ();
			gpuInfo.Add ("Score", score);

			AddGPU (gpuName, gpuInfo);
		}
		
		private void AddGPU(string gpuName, Dictionary<string,object> gpuInfo = null)
		{
			Dictionary<string,object> devices;
			if (!config.ContainsKey ("Devices")) {
				devices = new Dictionary<string,object> ();
				config ["Devices"] = devices;
			} else {
				devices = config["Devices"] as Dictionary<string,object>;
			}

			Dictionary<string,object> gpus;
			if (!devices.ContainsKey ("GPUs")) {
				gpus = new Dictionary<string, object> ();
				devices ["GPUs"] = gpus;
			} else {
				gpus = devices ["GPUs"]  as Dictionary<string,object>;
			}

			if (gpuInfo == null) {
				gpuInfo = new Dictionary<string, object> ();
			}
			gpus [gpuName] = gpuInfo;

		}

		private void RemoveGPU(string gpuName)
		{
			Dictionary<string,object> devices;
			if (!config.ContainsKey ("Devices")) {
				devices = new Dictionary<string,object> ();
				config ["Devices"] = devices;
			} else {
				devices = config["Devices"] as Dictionary<string,object>;
			}
			
			Dictionary<string,object> gpus;
			if (!devices.ContainsKey ("GPUs")) {
				gpus = new Dictionary<string, object> ();
				devices ["GPUs"] = gpus;
			} else {
				gpus = devices ["GPUs"]  as Dictionary<string,object>;
			}

			gpus.Remove (gpuName);
		}


		protected Dictionary<string,object> config;

		protected Dictionary<string,object> originDevices;
	}
}
