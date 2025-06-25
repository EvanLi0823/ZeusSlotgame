using Classic;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WesternTreasureSmartAnimation : SmartAntiEffectController
{
    List<int> fastReels = new List<int>();
    List<List<int>> smartPanel = new List<List<int>>();
    public override List<int> CheckSmartPositionAndAnticipation(List<ResultContent.ReelResult> reelsSpinData, SymbolMap symbolMap, ReelManager reelManager)
    {
       base.CheckSmartPositionAndAnticipation(reelsSpinData, symbolMap, reelManager);
        fastReels.Clear();
        smartPanel.Clear();
        int freeSymbolCount = (reelManager as WesternTreasureReelManager).spinResult.freeTriggerSymbolDic.Count;
       
        if (freeSymbolCount==2)
        {
            fastReels.Add(2);
        }
        if (freeSymbolCount>=3)
        {
            fastReels.Add(2);
            fastReels.Add(3);
        }
        if (freeSymbolCount>=4)
        {
            fastReels.Add(4);
            fastReels.Add(5);
        }
        ResultContent content = reelManager.resultContent;
        for (int i = 0; i < content.ReelResults.Count; i++) {
            for (int j = 0; j < content.ReelResults[i].SymbolResults.Count; j++)
            {
                if (content.ReelResults[i].SymbolResults[j] == 1)
                {
                    smartPanel.Add(new List<int>() { i, j });
                }
            }
        }
        for (int i = 0; i < content.ReelResults.Count; i++)
        {
            bool haveSmart = false;
            for (int j = 0; j < content.ReelResults[i].SymbolResults.Count; j++)
            {
                if (content.ReelResults[i].SymbolResults[j] == 0 || content.ReelResults[i].SymbolResults[j] == 1) {
                    haveSmart = true;
                    if (content.ReelResults[i].SymbolResults[j] == 0) {
                        smartPanel.Add(new List<int>() { i,j});
                    }
                }
            }
            if (!haveSmart) {
                break;
            }
        }
        return fastReels;
    }
    
    private int s01Count;
    public override void PlaySmartSoundAndAnimation(int reelIndex, ReelController controller)
    {
        for (int i = 0; i < smartPanel.Count; i++)
        {
            BaseElementPanel render = controller.GetSymbolRender(smartPanel[i][0], smartPanel[i][1]);
            if (render.ReelIndex == reelIndex) {
                render.PlayAnimation((int)BaseElementPanel.AnimationID.AnimationID_SmartSymbolReminder, false, null,
                    () =>
                    {
                        render.StopAnimation();
                    }, 0f, true);
                if (render.SymbolIndex == 0) {
                    PlayS01SmartSound();
                }

                if (ClearAnimationTime == -1) continue;
                Libs.DelayAction da = new Libs.DelayAction(ClearAnimationTime, null, () =>
                {
                    render.StopAnimation();
                    Messenger.Broadcast<int>(SlotControllerConstants.STOP_ANTICIPATION_EFFECT, reelIndex);
                });
                da.Play();
                stopBlinkAniList.Add(da);
            }
        }
    }

    private void PlayS01SmartSound() {
        AudioManager.Instance.AsyncPlayEffectAudio("S01_smart_sound");
    }
}
