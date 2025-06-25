using Libs;
using DG.Tweening;
using UnityEngine;

public class ReSpinEndDialog : ReSpinEndDialogBase
{
    private bool audioStop = false;
    private Tween tween = null;
    private long initNum = 0;
    private long totalCoins = 0;
    private int reSpinCount = 0;

    protected override void Awake()
    {
        base.Awake();

        if (this.BackToGameButton == null)
        {
            this.bResponseBackButton = false;
            this.AutoQuit = true;
            this.DisplayTime = 4;
        }
        AudioEntity.Instance.PlayReSpinEndDialogMusic();
    }

    public override void OnButtonClickHandler(GameObject go)
    {
        base.OnButtonClickHandler(go);
        AudioEntity.Instance.StopReSpinEndDialogMusic();
        AudioEntity.Instance.PlayFeatureBtnEffect();
        this.StopTweenAnimation();
    }

    public void SetNum(long coins)
    {
        this.isModel = false;
        //ReSpin
        totalCoins = coins;
//        reSpinCount = ReSpinSpinNum;

        AudioEntity.Instance.PlayRollUpEffect(0.5f, true);
        tween = Utils.Utilities.AnimationTo(this.initNum, coins, 3f, CaculateTxt, null, () => {
            tween = null;
            if (!audioStop)
            {
                audioStop = true;
                AudioEntity.Instance.StopRollingUpEffect();
                AudioEntity.Instance.PlayRollEndEffect();
            }
            new DelayAction(1f, null, delegate () {
                if (this != null)
                {
                    if (this.BackToGameButton == null) this.Close();
                }

            }).Play();
		}).SetUpdate(true);
    }

    private void CaculateTxt(long value)
    {
        this.initNum = value;
        if (this.TextNum != null)
        {
            this.TextNum.text = Utils.Utilities.ThousandSeparatorNumber(value);
        }
    }

    private void StopTweenAnimation()
    {
        if (!audioStop)
        {
            audioStop = true;
            AudioEntity.Instance.StopRollingUpEffect();
            AudioEntity.Instance.PlayRollEndEffect();
        }

        if (tween != null)
        {
            tween.Complete();
            this.TextNum.text = Utils.Utilities.ThousandSeparatorNumber(totalCoins);
            tween = null;
            new DelayAction(0.3f, null, delegate () { this.Close(); }).Play();
        }
        else{
            this.Close();
        }
    }
}
