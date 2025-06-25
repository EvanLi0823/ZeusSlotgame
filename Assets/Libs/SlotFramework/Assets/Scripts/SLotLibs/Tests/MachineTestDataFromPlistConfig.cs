using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Classic;

public class MachineTestDataFromPlistConfig {
	
	public const string TEST_RESULT_DATA_LIST_CONFIG = "MachineDebugTestData";

	public const string COMMON_TEST_RESULTS_LIST_DATA = "CommonTestResultsListData";
	public const string SPECIAL_TEST_RESULTS_LIST_DATA = "SpecialTestResultsListData";

	public const string TEST_RESULT_NAME = "TestResultName";
	public const string TEST_RESULT_DATA = "TestResultData";

	public Dictionary<string,List<List<int>>> commonTestResultsList = new Dictionary<string, List<List<int>>>();
	public Dictionary<string,List<List<int>>> specialTestResultsList = new Dictionary<string, List<List<int>>>();
	public MachineTestDataFromPlistConfig(SymbolMap symbolMap,Dictionary<string,object> infos){ 

		if (infos==null) {
			return;
		}
		commonTestResultsList.Clear ();

		ParseTestDataFromPlist (symbolMap, infos);
	}

	protected void ParseTestDataFromPlist(SymbolMap symbolMap,Dictionary<string,object> infos){
		if (infos.ContainsKey(COMMON_TEST_RESULTS_LIST_DATA)) {
			List<object> resultsList = infos [COMMON_TEST_RESULTS_LIST_DATA] as List<object>;
			if (resultsList!=null) {
				foreach (object item in resultsList) {
					Dictionary<string,object> testResult = item as Dictionary<string,object>;
					if (!testResult.ContainsKey(TEST_RESULT_NAME)) {
						continue;
					}
					string resultName = testResult[TEST_RESULT_NAME].ToString();
					if (testResult.ContainsKey (TEST_RESULT_DATA)) {
						List<List<int>> results = new List<List<int>> ();
						List<object> resultData = testResult [TEST_RESULT_DATA] as List<object>;
						foreach (object record in resultData) {
							string[] lineSymbols = record.ToString ().Split (',');
							List<int> symbolsIndex = new List<int> ();
							foreach (string symbolName in lineSymbols) {
								symbolsIndex.Add (symbolMap.getSymbolIndex (symbolName));
							}
							results.Add (symbolsIndex);
						}
                        if (!commonTestResultsList.ContainsKey(resultName))
                        {
                            commonTestResultsList.Add(resultName, results);
                        }
                        else
                        {
                            Utils.Utilities.LogPlistError("ParseTestDataFromSlotPlist resultName:"+resultName+" has exist!,please setup test case");
                        }
                    } 
					else {
                        if (!commonTestResultsList.ContainsKey(resultName))
                        {
                            commonTestResultsList.Add(resultName, null);
                        }
                        else
                        {
                            Utils.Utilities.LogPlistError("ParseTestDataFromSlotPlist resultName:" + resultName + " has exist!,please setup test case");
                        }
					}
				}
			}
		}

		if (infos.ContainsKey(SPECIAL_TEST_RESULTS_LIST_DATA)) {
			List<object> resultsList = infos [SPECIAL_TEST_RESULTS_LIST_DATA] as List<object>;
			if (resultsList!=null) {
				foreach (object item in resultsList) {
					Dictionary<string,object> testResult = item as Dictionary<string,object>;
					if (!testResult.ContainsKey(TEST_RESULT_NAME)) {
						continue;
					}
					string resultName = testResult[TEST_RESULT_NAME].ToString();
					if (testResult.ContainsKey (TEST_RESULT_DATA)) {
						List<List<int>> results = new List<List<int>> ();
						List<object> resultData = testResult [TEST_RESULT_DATA] as List<object>;
						foreach (object record in resultData) {
							string[] lineSymbols = record.ToString ().Split (',');
							List<int> symbolsIndex = new List<int> ();
							foreach (string symbolName in lineSymbols) {
								symbolsIndex.Add (symbolMap.getSymbolIndex (symbolName));
							}
							results.Add (symbolsIndex);
						}
						specialTestResultsList.Add (resultName,results);
					} else {
						specialTestResultsList.Add (resultName,null);
					}
				}
			}
		}
	}

	public List<List<int>> GetSelectedCommonTestResultData(string resultName){
		if (commonTestResultsList.ContainsKey(resultName)) {
			return commonTestResultsList[resultName];
		}
		return null;
	}

	public List<List<int>> GetSelectedSpecialTestResultData(string resultName){
		if (specialTestResultsList.ContainsKey(resultName)) {
			return specialTestResultsList[resultName];
		}
		return null;
	}

	//根据 spin次数来获取模拟的结果,需要锁定
	public List<List<int>> GetSelectedCommonTestResultData(int spinCount)
	{
		Debug.Log("GetSelectedCommonTestResultData spinCount========"+spinCount);
		if (spinCount>=commonTestResultsList.Count)
		{
			return null;
		}
		string keyAtIndex = commonTestResultsList.Keys.ElementAt(spinCount);
		Debug.Log("GetSelectedCommonTestResultData TestResultName========"+keyAtIndex);
		var value = commonTestResultsList[keyAtIndex];
		return value;
	}
}
