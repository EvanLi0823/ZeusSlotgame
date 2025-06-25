using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Classic{
	public class ScatterGame : ExtraAwardGame {

	public virtual void ForTest(long currentBet){
		AwardInfo = new BaseAward ();
		AwardInfo.awardName = "ScatterGame";
		AwardInfo.awardValue = 0;
	}
}
}