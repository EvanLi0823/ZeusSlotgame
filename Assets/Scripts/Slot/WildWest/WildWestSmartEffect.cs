using System.Collections;
using Classic;
using System.Collections.Generic;
using UnityEngine;

public class WildWestSmartEffect : SmartAntiEffectController
{
    public override List<int> CheckSmartPositionAndAnticipation(List<ResultContent.ReelResult> reelsSpinData, SymbolMap symbolMap, ReelManager reelManager)
    {
        StopAllSmartAndAntiEffectAnimation(reelManager);
        SmartPositions.Clear();
        AntiReels.Clear();
        stopBlinkAniList.Clear();

        if (SmartConfig == null || SmartConfig.Count == 0)
            return AntiReels;

        if(reelManager.isFreespinBonus)
        {
            foreach(var info in SmartConfig)
            {
                info.TriggerCount = 2;
                info.SmartTriggerAntiReelCount = 1;
            }
        }else
        {
            foreach(var info in SmartConfig)
            {
                info.TriggerCount = 3;
                info.SmartTriggerAntiReelCount = 2;
            }
        }

        for (int i = 0; i < SmartConfig.Count; i++) 
        {
            SmartConfigs smartConfig = SmartConfig[i];
         
            if (smartConfig.AllPossibleNum == 0) {
                continue;
            }

            if (smartConfig.SmartType == SmartAntiType.Continuous || smartConfig.SmartType == SmartAntiType.Scatter)
            {
                ContinuousScatterAnticipation(smartConfig, reelsSpinData, symbolMap);
            }
            else if(smartConfig.SmartType == SmartAntiType.SameLine)
            {
                SameLineAnticipation(smartConfig, reelsSpinData, symbolMap);
            }
        }
        return AntiReels;
    }

    public override void PlaySmartSoundAndAnimation(int reelIndex, ReelController controller)
    {
        if (SmartPositions.ContainsKey(reelIndex)) {
            foreach (int position in SmartPositions[reelIndex].Keys) {
                BaseElementPanel render = controller.GetSymbolRender(reelIndex, position);
                if (render != null) {
                    render.PlayAnimation((int)BaseElementPanel.AnimationID.AnimationID_SmartSymbolReminder, false, null, ()=>{ StopSmartAnimation(reelIndex, controller,render.SymbolIndex); }, 0f, true);
                    Messenger.Broadcast<int>(SlotControllerConstants.PLAY_ANTICIPATION_EFFECT, reelIndex);
                    //TODO:smart的音效优化

                    Libs.AudioEntity.Instance.PlaySmartSoundEffect(reelIndex);

                    if (ClearAnimationTime == -1) continue;
                    Libs.DelayAction da = new Libs.DelayAction(ClearAnimationTime, null, () => {
                        render.StopAnimation();
                        Messenger.Broadcast<int>(SlotControllerConstants.STOP_ANTICIPATION_EFFECT, reelIndex);
                    });
                    da.Play();
                    stopBlinkAniList.Add(da);
                }
            }

        }
    }
}
