using Libs;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class SpinWinDialog_old : UIDialog 
{
	public List<GameObject> spinWinDialog;
	public List<Button> collectBtn;
	public List<UIText> coinsUItext;

	private Button collect;
	private UIText coinsText;
	private Tween tween = null;
    private long curCoins = 0;
    private long totalCoins = 0;
    private bool isStop = false;
	private SpinWinType spinWinType = SpinWinType.NONE;

    public override void OnButtonClickHandler (GameObject go)
    {
        base.OnButtonClickHandler(go);
        collect.interactable = false;
        this.OnClickStopUpdate();
    }

    public void OnStart (long coins, SpinWinType type)
    {
		totalCoins = coins;
        spinWinDialog[(int)type].SetActive(true);
        collect = collectBtn[(int)type];
        coinsText = coinsUItext[(int)type];
        spinWinType = type;
		UGUIEventListener.Get(this.collect.gameObject).onClick = this.OnButtonClickHandler;
		
		this.PlayWinTypeEffect();
		AudioEntity.Instance.PlayMuisc("win_bg");
        AudioEntity.Instance.PlayRollUpEffect();

        tween = Utils.Utilities.AnimationTo (this.curCoins, coins, 3f, UpdateTextUI, null,()=>
        {
            AudioEntity.Instance.StopRollingUpEffect();
            tween = null;
            new DelayAction(1f,null,delegate() 
            {
                if(collect != null && isStop) this.CloseDialog();
            }).Play();
			}).SetUpdate(true);
    }

	private void UpdateTextUI(long num)
    {
        this.curCoins = num;
        coinsText.SetText(Utils.Utilities.ThousandSeparatorNumber(curCoins));
    }

	private void PlayWinTypeEffect()
	{
		switch(spinWinType)
		{
			case SpinWinType.EPIC :
				AudioEntity.Instance.PlayEffect("epic_win");
				break;
			case SpinWinType.MEGA :
				AudioEntity.Instance.PlayEffect("mega_win");
				break;
			case SpinWinType.BIG :
				AudioEntity.Instance.PlayEffect("big_win");
				break;
		}
	}

	public void OnClickStopUpdate()
    {
        if(tween == null) this.CloseDialog();
        isStop = true;
        tween.Kill(true);
        AudioEntity.Instance.StopRollingUpEffect();
        this.UpdateTextUI(totalCoins);
    }

	private void CloseDialog()
	{
		this.Close ();

		AudioEntity.Instance.StopMusicAudio("win_bg");

		Messenger.Broadcast(GameConstants.CLOSE_SPIN_WIN_DIALOG);

		switch(spinWinType)
		{
			case SpinWinType.EPIC :
				CommandTriggerManager.Instance.CheckMomentConditions (GameConstants.CLOSE_EPIC_WIN_DIALOG);
				Messenger.Broadcast(GameConstants.CLOSE_EPIC_WIN_DIALOG);
				break;
			case SpinWinType.MEGA :
				CommandTriggerManager.Instance.CheckMomentConditions (GameConstants.CLOSE_MEGA_WIN_DIALOG);
				Messenger.Broadcast(GameConstants.CLOSE_MEGA_WIN_DIALOG);
				break;
			case SpinWinType.BIG :
				CommandTriggerManager.Instance.CheckMomentConditions (GameConstants.CLOSE_BIG_WIN_DIALOG);
				Messenger.Broadcast(GameConstants.CLOSE_BIG_WIN_DIALOG);
				break;
		}
	}
}

