using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Classic;
/// <summary>
/// Bonus animation manager.
/// 需要的关卡对其添加脚本，不需要的关卡则不需要添加
/// </summary>
public class BonusAnimationManager : MonoBehaviour {
	ReelManager reelManager;
	public List<KeyValuePair<int,int>> smartSoundSymbolAnimationPos = new List<KeyValuePair<int, int>> ();
	// Use this for initialization
	void Awake () {
		reelManager = GetComponent<ReelManager> ();
		Messenger.AddListener (SlotControllerConstants.OnSpinEnd,SpinEndHandle);
		Messenger.AddListener<int> (RollReelPanel.REEL_ANIMATION_END,StopBonusAnimation);
		Messenger.AddListener<int> (RollReelPanel.REEL_FAST_RUN_BEFORE_SLOW_DOWN,PlayBonusAnimation);
		Messenger.AddListener<int,int> (BaseReel.TRIGGER_SMART_SOUND_SYMBOL_POSITION,RecordSmartSoundSymbolPos);
	}
	void OnDestroy(){
		Messenger.RemoveListener (SlotControllerConstants.OnSpinEnd,SpinEndHandle);
		Messenger.RemoveListener<int> (RollReelPanel.REEL_ANIMATION_END,StopBonusAnimation);
		Messenger.RemoveListener<int> (RollReelPanel.REEL_FAST_RUN_BEFORE_SLOW_DOWN,PlayBonusAnimation);
		Messenger.RemoveListener<int,int> (BaseReel.TRIGGER_SMART_SOUND_SYMBOL_POSITION,RecordSmartSoundSymbolPos);
	}
	
	void SpinEndHandle(){
		smartSoundSymbolAnimationPos.Clear ();
	}
	Libs.DelayAction delayA;
	void PlayBonusAnimation(int reelIndex){
		if (delayA!=null) {
			delayA.Stop ();
			delayA = null;
		}
		delayA = new Libs.DelayAction (reelManager.gameConfigs.smartSoundSymbolAnimationDuration,null,()=>{
			for (int i = 0; i < smartSoundSymbolAnimationPos.Count; i++) {
				KeyValuePair<int,int> kvp = smartSoundSymbolAnimationPos [i];
				reelManager.Reels [kvp.Key].positionMap [kvp.Value].StopAnimation ();
				reelManager.Reels [kvp.Key].positionMap [kvp.Value].PlayAnimation ((int)BaseElementPanel.AnimationID.AnimationID_SmartSymbolLoopReminder,false);
			}
		});
		delayA.Play ();
	}
	void StopBonusAnimation(int reelIndex){
		for (int i = 0; i < smartSoundSymbolAnimationPos.Count; i++) {
			KeyValuePair<int,int> kvp = smartSoundSymbolAnimationPos [i];
			reelManager.Reels [kvp.Key].positionMap [kvp.Value].StopAnimation ();
		}
	}
	void RecordSmartSoundSymbolPos(int reelIndex,int elementPos){
		KeyValuePair<int,int> kvp = new KeyValuePair<int, int> (reelIndex,elementPos);
		smartSoundSymbolAnimationPos.Add (kvp);
	}
}
