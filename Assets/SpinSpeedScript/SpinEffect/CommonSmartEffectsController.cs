using Classic;

/// <summary>
/// 自己控制停止smartanticipation
/// </summary>
public class CommonSmartEffectsController : PreloadSmartAntiEffectController
{
    public override void PlaySmartSoundAndAnimation(int reelIndex, ReelController controller)
    {
        if (SmartPositions.ContainsKey(reelIndex))
        {
            foreach (int position in SmartPositions[reelIndex].Keys)
            {
                BaseElementPanel render = controller.GetSymbolRender(reelIndex, position);
                if (render != null)
                {
                    render.PlayAnimation((int)BaseElementPanel.AnimationID.AnimationID_SmartSymbolReminder, false, null,
                        () => { StopSmartAnimation(reelIndex, controller, render.SymbolIndex); }, 0f, true);
                    Messenger.Broadcast<int>(SlotControllerConstants.PLAY_ANTICIPATION_EFFECT, reelIndex);
                    //TODO:smart的音效优化

                    Libs.AudioEntity.Instance.PlaySmartSoundEffect(reelIndex);

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
    }
}