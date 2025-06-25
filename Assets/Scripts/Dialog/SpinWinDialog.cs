using Libs;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Utils;

public class SpinWinDialog : UIDialog 
{
	[Header("SpinWinDialog配置")]
	public List<Sprite> winType;
	public List<Sprite> winTypeMask;
	public List<Sprite> win;
	public List<Image> winImage;
	public UIText winText;
	public Button collect;
	public SpinWinAnimationEvent animationEvent;
	private Tween tween = null;
    private long curCoins = 0;
    private long totalCoins = 0;
	private SpinWinType spinWinType = SpinWinType.NONE;
	public float time = 5f;
    private bool m_HasClosed = false;
	public Button WatchAdBtn;
	public Button CloseBtnOnAd; //有广告时的关闭按钮
	public Button CollectBtn;
	bool isplayAds;
	string entranceName;
	public GameObject text2XDown;
	public const string PlayBigWinAD = "PlayBigWinAD";
	public const string PlayGBigWinADDefeated = "PlayBigWinADDefeated";
	private bool isBigWin;
	private bool isShowWatchBtn;
	//public UIText multi;
	public int multiNum;
	//public TextMeshProUGUI WatchAdBtnText;
	private double winBetLimit_Min = 100;
	private double winBetLimit_Max = 1000000;
	public float winMultiplesLimit_Max = 16;
	public float winMultiplesLimit_Min = 5;
	private bool isCollect;
	protected override void Awake()
	{
		base.Awake();
		UGUIEventListener.Get(this.collect.gameObject).onClick = this.OnButtonClickHandler;
		text2XDown.SetActive(false);
		isShowWatchBtn = false;
		SetWatchBtn(false);
		this.CollectBtn.gameObject.SetActive(false);
		UGUIEventListener.Get(this.CloseBtnOnAd.gameObject).onClick = this.OnButtonClickHandler;
	}

	public void OnStart(long coins, SpinWinType type)
	{
		if (coins <= 0) { this.Close(); return; }
		isBigWin = type == SpinWinType.BIG;
		spinWinType = type;
		totalCoins = coins;
		animationEvent.index = (int)spinWinType;
		winImage[0].sprite = winType[(int)type];
		winImage[1].sprite = winTypeMask[(int)type];
		winImage[2].sprite = win[(int)type];
		foreach (var item in winImage) item.SetNativeSize();
		this.PlayWinTypeEffect();
		AudioEntity.Instance.PlayRollUpEffect(0.7f);
		bool haveAd = false;
		tween = Utils.Utilities.AnimationTo(this.curCoins, coins, time, UpdateTextUI, null, () =>
		{
			AudioEntity.Instance.StopRollingUpEffect();
			tween = null;
			collect.gameObject?.SetActive(false);
			new DelayAction(haveAd ? 8f : 2f, ()=> {
				this.CloseBtnOnAd.gameObject.SetActive(isShowWatchBtn);
			},
			delegate ()
			{
				if (WatchAdBtn != null)
					WatchAdBtn.enabled = false;
				if (!isplayAds)
				{
					this.CloseDialog();
				}
			}).Play();
		});
	}

	void SetWatchBtn(bool iswatchBtn = false)
	{
		this.CollectBtn.gameObject.SetActive(!iswatchBtn);
		this.WatchAdBtn.gameObject.SetActive(iswatchBtn);
		this.CloseBtnOnAd.gameObject.SetActive(iswatchBtn);
	}

	public void OnCollectBtnClick(GameObject go) {
		if (tween != null)
		{
			tween.Kill(true);
			this.UpdateTextUI(totalCoins);
			collect.gameObject.SetActive(false);
			new DelayAction(8f, null, delegate ()
			{
				if (WatchAdBtn != null) {
					WatchAdBtn.enabled = false;
				}
				if (!isplayAds)
				{
					this.CloseDialog();
				}
			}).Play();
		}
		else {
			return;
		}
		AudioEntity.Instance.StopRollingUpEffect();
	}

	public override void OnButtonClickHandler (GameObject go)
    {
        base.OnButtonClickHandler(go);
        collect.interactable = false;
        this.OnClickStopUpdate();
		isCollect = true;
		Messenger.Broadcast<Transform, Libs.CoinsBezier.BezierType, System.Action>(
			GameConstants.CollectBonusWithType, go.transform, Libs.CoinsBezier.BezierType.DailyBonus, null);
		Messenger.Broadcast(SlotControllerConstants.OnBlanceChangeForDisPlay);
		Libs.AudioEntity.Instance.StopAllEffect();
		Libs.AudioEntity.Instance.PlayCoinCollectionEffect();
	}

   
	private void UpdateTextUI(long num)
    {
        this.curCoins = num;
        winText.SetText(Utils.Utilities.ThousandSeparatorNumber(curCoins));
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
        if(tween == null) 
		{
			this.CloseDialog();
			return;
		}else
		{
			tween.Kill(true);

            this.UpdateTextUI(totalCoins);
			// AudioEntity.Instance.StopMusicAudio("spinwin", 1f);
			new DelayAction(1f,null,delegate() 
			{
                this.CloseDialog();
            }).Play();
		}

        AudioEntity.Instance.StopRollingUpEffect();
    }

	private void CloseDialog()
	{
		if(this.m_HasClosed)
        {
            return;
        }
        m_HasClosed = true;
        this.Close ();

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
public enum SpinWinType
{
	BIG = 0,
	MEGA,
	EPIC,
	NONE
}

