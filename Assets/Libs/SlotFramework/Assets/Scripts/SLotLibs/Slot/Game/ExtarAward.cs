using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Classic
{
	public class ExtarAward : BaseExtraAward
	{
		public virtual void ForTest(ResultContent resultContent){
		}

		public virtual void InitForTest(Dictionary<string,object> infos,float currentBettting){
		}

		public virtual void OnSpinForTest (float currentBetting){
		}
	}
}
