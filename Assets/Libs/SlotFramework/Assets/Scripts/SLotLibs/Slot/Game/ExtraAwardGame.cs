using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Classic
{
	public class ExtraAwardGame : BaseExtraAward
	{
		public virtual void ForExtraGameTest(ResultContent resultContent,ReelManager gamePanel){
		}

        public virtual void OnEnterFreeSpinForTest() {
        }

        public virtual void OnQuitFreeSpinForTest() {
        }

		public virtual void InitForTest(Dictionary<string,object> infos,long currentBettting){
			AwardInfo = new BaseAward ();
		}

		public virtual void OnSpinForTest (long currentBetting){
		}
	}
}
