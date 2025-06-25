using System;
using System.Collections;
using System.Collections.Generic;
using Classic;
using Libs;
using UnityEngine;

public class WesternTreasureBonusGame : BonusGame
{
    private WesternTreasureReelManager treeManager;
    public const string TreeBonusGameEnd = "TreeBonusGameEnd";
    protected override void Awake()
    {
        Messenger.AddListener(TreeBonusGameEnd, QuitGame);
       
        base.Awake();
    }
    protected override void OnDestroy()
    {
        Messenger.RemoveListener(TreeBonusGameEnd, QuitGame);
      
        base.OnDestroy();
    }
   
    public override void OnEnterGame(ReelManager reelManager)
    {
        SpinResultProduce.AddProvider(new SpinResultBonusTrigger(TriggerFeatureType.Symbol,TriggerFeatureType.Collect,SpinResultProduce.InternalGetSymbolsList()));
        SpinResultProduce.InternalSend();
        treeManager = reelManager as WesternTreasureReelManager;
        AudioEntity.Instance.StopSlotsBackGroundMusic();
        treeManager.westernTreasureFly.EnterJackpotGame();
        Messenger.Broadcast<bool>(SlotControllerConstants.DisactiveButtons, false);
        Messenger.Broadcast<bool>(SlotControllerConstants.DisableSpinButton, true);
    }

    private void QuitGame() {
        AudioManager.Instance.StopMusicAudio("Jackpot");
        OnQuitGame(treeManager);
        OnGameEnd();
        if (!(treeManager.FreespinCount > 0)&& !treeManager.isFreespinBonus) {
            Messenger.Broadcast<bool>(SlotControllerConstants.ActiveButtons, true);
            Messenger.Broadcast<bool>(SlotControllerConstants.DisableSpinButton, false);
            AudioEntity.Instance.UnPauseBackGroundAudio(Libs.AudioEntity.audioSpin);
        }
        if (treeManager.isFreespinBonus) {
            Messenger.Broadcast<bool>(SlotControllerConstants.DisableSpinButton, false);
            AudioEntity.Instance.UnPauseBackGroundAudio(Libs.AudioEntity.audioSpecial);
        }
    }
    public override void PlayWinBonusGameAnimation(Action callBack)
    {
        PlayWinBonusGameAnimationEndHandler(callBack);
    }
    protected override void PlayWinBonusGameAnimationEndDelayHandler(float delayStartTime, float delayEndTime, System.Action callBack)
    {
       
        callBack?.Invoke();
    }

}
