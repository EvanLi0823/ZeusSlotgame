using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Classic;
using System.Collections.Generic;

public class IndependentSymbolReel : BaseReel
{

	public override void InitElements (ReelManager GameContronller, GameConfigs elementConfigs, bool IsEndReel)
	{
		InitData (GameContronller, elementConfigs, IsEndReel);
		Layout (ElementNumber);
	}

	protected override IEnumerator ReelRunning ()
	{
		yield return new WaitForSeconds (elementConfigs.GetReelConfigs (Index).StartDelayTime);
		for (int j = 0; j < this.Elements.Count; j++) {
			this.Elements [j].ChangeColor ();
		}
		if (this.IsEndReel) {
			reelManager.State = GameState.RUNNING;
		}
		float offset = 0;
        float runningTime = elementConfigs.GetReelConfigs (Index).Runtime + (2*Index)* elementConfigs.GetReelConfigs (Index).ElementAtomDelayTime;
		this.State = ReelState.RUNNING;


        IndependentSymbolReel middelIndependentSymbolReel =  (IndependentSymbolReel) reelManager.Reels [2];

        int firstAnticipationSymbolIndex = -1; 
        ResultContent.ReelResult reelResult = reelManager.resultContent.GetReelResult (2);
        List<int> symbolResult = reelResult.SymbolResults;

        SymbolMap.SymbolElementInfo symbolElementInfo = reelManager.symbolMap.getSymbolInfo (symbolResult[symbolResult.Count -1]);


        for (int i = middelIndependentSymbolReel.Elements.Count -1; i >= 0; i--) {
            RollIndependentElement element = (RollIndependentElement) (middelIndependentSymbolReel.Elements [i]);
            symbolElementInfo = reelManager.symbolMap.getSymbolInfo (symbolResult[i]);
            if (symbolElementInfo.isFreespin) {
                firstAnticipationSymbolIndex = i;
                break;
            }
        }

		for (int i = Elements.Count -1; i >=0; i--) {
			RollIndependentElement element = (RollIndependentElement)Elements [i];
            if (element.hasAnticipationAnimation && i < firstAnticipationSymbolIndex) {
                runningTime += elementConfigs.GetReelConfigs (Index).SlowFastRunTime;
            } else {
                runningTime += elementConfigs.GetReelConfigs (Index).ElementAtomDelayTime;
            }

			float runningV = elementConfigs.GetReelConfigs (Index).RunV;
            element.StartRunning (runningV, runningTime);
		}
	}

	public override IEnumerator ReelSlowDown ()
	{
		if (!reelManager.fastStop) {
			yield return new WaitForSeconds (elementConfigs.GetReelConfigs (Index).StopDelay);
		}
		this.State = ReelState.SLOWDOWN;
		foreach (BaseElementPanel baseElement in Elements) {
			((RollIndependentElement)baseElement).symbolAnimationState = RollIndependentElement.SymbolAnimatioState.FIXRUNNING;
		}
	}

}
