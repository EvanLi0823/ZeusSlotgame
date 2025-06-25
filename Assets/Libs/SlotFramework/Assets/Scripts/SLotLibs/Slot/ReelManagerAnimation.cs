using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Classic;

public class ReelManagerAnimation : MonoBehaviour {
	public GameObject BigAnimation;
	public AnimationCanvas animationCanvas;
	public GameObject runningEffect; 
	public List<BaseElementPanel> awardElements = new List<BaseElementPanel> ();
	public List<AwardLineElement> awardLines = new List<AwardLineElement> ();

	public List<GameObject> reelWinCoinAnimations;
	public List<GameObject> MiddleWinAnimations;
	public List<GameObject> bonusTriggerAnimations;  
	// Use this for initialization
	void Awake () {
		ReelManager reelManager = GetComponent<ReelManager> ();
		//reelManager.BigAnimation = BigAnimation;
		reelManager.animationCanvas = this.animationCanvas;
		reelManager.runningEffect = this.runningEffect;
		reelManager.awardElements = this.awardElements;
		reelManager.awardLines = this.awardLines;
		reelManager.reelWinCoinAnimations = this.reelWinCoinAnimations;
		reelManager.MiddleWinAnimations = this.MiddleWinAnimations;
		reelManager.bonusTriggerAnimations = this.bonusTriggerAnimations;

	}	 
}
