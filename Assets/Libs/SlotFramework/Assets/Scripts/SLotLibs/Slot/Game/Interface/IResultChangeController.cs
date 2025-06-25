using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Classic
{
	public interface IResultChangeController
	{
		List<List<int>> GetTestResult (GameConfigs gameConfigs);
		List<List<int>> SpecialReuslt (GameConfigs gameConfigs);
		bool isTestOn {
			get;
			set;
		}

		bool HasSpecialResult();

        void ChangeResult (ReelManager baseGamePanel);
	}
}
