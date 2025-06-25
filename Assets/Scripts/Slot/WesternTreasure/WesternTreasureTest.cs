using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Random = System.Random;

namespace Classic
{
    public class WesternTreasureTest : BaseResultChange
	{
		public ReelTestConfig[] result;
		public PreDefineResult[] preDefineResults;
		
		public override List<List<int>> GetTestResult (GameConfigs gameConfigs)
		{
// #if UNITY_EDITOR
            if (ReadTestDataFromPlist)
            {
                return base.GetTestResult(gameConfigs);
            }
// #elif DEBUG || UNITY_ANDROID || UNITY_IOS
// 		return base.GetTestResult (gameConfigs);
// #endif
            List<List<int>> results = new List<List<int>>();
            for (int i = 0; i < result.Length; i++)
            {
                List<int> temp = new List<int>();
                for (int j = 0; j < gameConfigs.reelConfigs[i].resultLenth; j++)
                {
                    temp.Add((int)result[i].reel[j]);
                }
                results.Add(temp);
            }
            return results;

        }

		List<List<int>> createResult (GameConfigs gameConfigs,ReelTestConfig[] result){
			List<List<int>> results = new List<List<int>> ();
			for (int i=0; i<result.Length; i++) {
				List<int> temp = new List<int> ();
				for (int j=0; j<gameConfigs.reelConfigs[i].resultLenth; j++) {
					temp.Add ((int)result [i].reel [j]);
				}
				results.Add (temp);
			}
			return results;
		}

		public override List<List<int>> SpecialReuslt (GameConfigs gameConfigs)
		{
			PreDefineResult p = preDefineResults [UnityEngine.Random.Range (0, preDefineResults.Length)];
			return createResult (gameConfigs,p.result);
		}

		public override	bool HasSpecialResult(){
			if(base.HasSpecialResult ()){
				return preDefineResults.Length>0;
			}
			return false;
		}
		
		public enum Type
		{
            S01 ,
            W01 ,
			H01,
            H02 ,
			H03,
            H04 ,
			H05,
            L01,
			L02,
            L03 ,
			L04,
            L05,
			L06,
		}

		[Serializable]
		public class ReelTestConfig
		{
			public Type[] reel;
		}

		[Serializable]
		public class PreDefineResult
		{
			public ReelTestConfig[] result;
		}
	}
}
