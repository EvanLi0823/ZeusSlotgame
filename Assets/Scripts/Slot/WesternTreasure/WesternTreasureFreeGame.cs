using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Classic;
using System;
using Libs;
using Spine.Unity;

public class WesternTreasureFreeGame : FreespinGame
{
    List<int> animationIdList = new List<int>();
    public SkeletonGraphic skeleton;
    protected override void Awake()
    {
        base.Awake();
        this.AwardBeforeEnterFS = true; //
    }

    public override void showFirstWinFreespin(int times, System.Action callback)
    {
        base.showFirstWinFreespin(times, callback);
        //(reelManager as MoneyFrenzyReelManager).ExitJackpotGame();

    }

    public override void OnEnterGame(ReelManager reelManager)
    {
        base.OnEnterGame(reelManager);
        Messenger.Broadcast<bool>(SlotControllerConstants.DisableSpinButton, false);
    }
    public override BaseAward AddMore(int time)
    {
        return base.AddMore(time);
    }
    public override void PlayWinFreeSpinAudio()
    {
        Libs.AudioEntity.Instance.PlayBonusTriggerEffect();
    }

    public override void showWinDialog (System.Action callback)
    {
        long coins = BaseSlotMachineController.Instance.winCoinsForDisplay;
        int cash = BaseSlotMachineController.Instance.winCashForDisplay;
        float time = BaseSlotMachineController.Instance.reelManager.gameConfigs.TileAnimationDuration;
        ShowWinDialogHandler(time,coins,callback,cash);
    }
    public override void PlayWinFreespinAnimation(System.Action callBack)
    {
//        Debug.LogError("播放触发动画--------");
        animationIdList.Clear();
        reelManager = BaseSlotMachineController.Instance.reelManager;
        AwardResult.AwardPayLine bounusAwardLine = new AwardResult.AwardPayLine(-1);
        AwardLineElement awardLineElement = new AwardLineElement(bounusAwardLine);
        awardLineElement.awardElements = reelManager.GetFreespinSymbols();
        
        for (int i = 0; i < awardLineElement.awardElements.Count; i++)
        {
            if (awardLineElement.awardElements[i].symbolIndex==0)
            {
                animationIdList.Add((int)BaseElementPanel.AnimationID.AnimationID_BonusTriggered);
            }
            else
            {
                animationIdList.Add((int)BaseElementPanel.AnimationID.AnimationID_NormalAward);
            }
        }
        if (awardLineElement.awardElements.Count > 0)
        {
            bounusAwardLine.awardNumberCount = awardLineElement.awardElements.Count;
            bounusAwardLine.CreateDefaultAnimations(animationIdList);
            PlayBonusTriggerExtraAnimation(awardLineElement.awardElements);
            reelManager.awardElements.AddRange(awardLineElement.awardElements);
            reelManager.resultContent.awardResult.awardInfos.Add(bounusAwardLine);
            reelManager.awardLines.Add(awardLineElement);
        }

        if (reelManager.isFreespinBonus && reelManager.ResultData.LineValue > 0)
        {
            new DelayAction(2, null, () =>
            {
                BaseSlotMachineController.Instance.StopAllAnimation();
                List<BaseElementPanel> list = reelManager.GetFreespinSymbols();
                for (int i = 0; i < list.Count; i++)
                {
                    BaseElementPanel temp = list[i];
                    temp.PlayAnimation(3,false,null,() =>{temp.StopAnimation();});
                }
                PlayWinFreeSpinAudio();
                new DelayAction(2, null, () =>
                {
                    callBack?.Invoke();
                }).Play();
            }).Play();
        }
        else {
            Libs.DelayAction delayAction = new Libs.DelayAction(2f,
            () =>
            {
                BaseSlotMachineController.Instance.PlaySymbolAnimation();
                PlayWinFreeSpinAudio();
                Libs.AudioEntity.Instance.PauseBackGroundAudio(Libs.AudioEntity.audioSpin, 3f);
            },
            () => {
                StopBonusTriggerExtraAnimation();
                callBack?.Invoke();
            });
            new Libs.DelayAction(BaseSlotMachineController.Instance.reelManager.gameConfigs.smartSoundSymbolAnimationDuration, null, delegate ()  // --暂时解决快停动画消失问题
            {
                delayAction.Play();
            }).Play();
        }
    }
}
