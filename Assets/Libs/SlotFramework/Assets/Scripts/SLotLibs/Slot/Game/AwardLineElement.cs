using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Classic;
using Libs;
public class AwardLineElement {
	private readonly string[] PAYLINE_5_COLOR_ARRAY = {"#FFB20Aff","#0EFDF1ff","#FF069Bff","#D61631ff","#26FD33ff"};
	private readonly string[] PAYLINE_9_COLOR_ARRAY = {"#ff3300ff","#0000ffff","#003399ff","#ff9999ff","#ff00ffff","#ff9900ff","#990066ff","#009900ff","#ffdf00ff"};
	private readonly string[] PAYLINE_20_COLOR_ARRAY =
	{
		"#ca1121ff","#0d00d5ff","#28548aff","#dd8993ff","#de00c4ff","#f49f1bff","#640d4dff","#179312ff","#e4d324ff","#930e1cff"
		,"#36eaedff","#b7b936ff","#2ea2d3ff","#47ef3dff","#c3c3c3ff","#639d8cff","#d7155cff","#2d81baff","#48ce7eff","#fffe64ff"
	};
    private readonly string[] PAYLINE_25_COLOR_ARRAY = { "#cba93dff", "#fd3222ff", "#ff318bff", "#75fb4dff", "#ff8d51ff", "#0000ffff", "#983dffff", "#ff392aff", "#75fb4cff",
        "#0100ffff", "#ff9058ff", "#ff3089ff", "#75fbfdff", "#7712ffff", "#ff3221ff", "#81ff5fff", "#1a17ffff", "#ff8e58ff",
        "#ff2d87ff", "#8bfaffff", "#8e35ffff", "#ff3b2eff", "#76fa4eff", "#2926ffff", "#ff8849ff"};
    private readonly string[] PAYLINE_40_COLOR_ARRAY = { "#cba93dff", "#fd3222ff", "#ff318bff", "#75fb4dff", "#ff8d51ff", "#0000ffff", "#983dffff", "#ff392aff", "#75fb4cff",
	    "#0100ffff", "#ff9058ff", "#ff3089ff", "#75fbfdff", "#7712ffff", "#ff3221ff", "#81ff5fff", "#1a17ffff", "#ff8e58ff",
	    "#ff2d87ff", "#8bfaffff", "#8e35ffff", "#ff3b2eff", "#76fa4eff", "#2926ffff", "#ff8849ff","#cba93dff", "#fd3222ff", "#ff318bff", "#75fb4dff", "#ff8d51ff", "#0000ffff", "#983dffff", "#ff392aff", "#75fb4cff",
	    "#0100ffff", "#ff9058ff", "#ff3089ff", "#75fbfdff", "#7712ffff", "#ff3221ff"};
    private readonly string[] PAYLINE_50_COLOR_ARRAY = { "#cba93dff", "#fd3222ff", "#ff318bff", "#75fb4dff", "#ff8d51ff", "#0000ffff", "#983dffff", "#ff392aff", "#75fb4cff",
	    "#0100ffff", "#ff9058ff", "#ff3089ff", "#75fbfdff", "#7712ffff", "#ff3221ff", "#81ff5fff", "#1a17ffff", "#ff8e58ff",
	    "#ff2d87ff", "#8bfaffff", "#8e35ffff", "#ff3b2eff", "#76fa4eff", "#2926ffff", "#ff8849ff","#cba93dff", "#fd3222ff", "#ff318bff", "#75fb4dff", "#ff8d51ff", "#0000ffff", "#983dffff", "#ff392aff", "#75fb4cff",
	    "#0100ffff", "#ff9058ff", "#ff3089ff", "#75fbfdff", "#7712ffff", "#ff3221ff", "#81ff5fff", "#1a17ffff", "#ff8e58ff",
	    "#ff2d87ff", "#8bfaffff", "#8e35ffff", "#ff3b2eff", "#76fa4eff", "#2926ffff", "#ff8849ff"};
    private readonly string[] PAYLINE_80_COLOR_ARRAY = { "#cba93dff", "#fd3222ff", "#ff318bff", "#75fb4dff", "#ff8d51ff", "#0000ffff", "#983dffff", "#ff392aff", "#75fb4cff", "#0100ffff",
	    "#ff9058ff", "#ff3089ff", "#75fbfdff", "#7712ffff", "#ff3221ff", "#81ff5fff", "#1a17ffff", "#ff8e58ff", "#ff2d87ff", "#8bfaffff", 
	    "#8e35ffff", "#ff3b2eff", "#76fa4eff", "#2926ffff", "#ff8849ff","#cba93dff", "#fd3222ff", "#ff318bff", "#75fb4dff", "#ff8d51ff", 
	    "#0000ffff", "#983dffff", "#ff392aff", "#75fb4cff", "#0100ffff", "#ff9058ff", "#ff3089ff", "#75fbfdff", "#7712ffff", "#ff3221ff", 
	    "#81ff5fff", "#1a17ffff", "#ff8e58ff", "#ff2d87ff", "#8bfaffff", "#8e35ffff", "#ff3b2eff", "#76fa4eff", "#2926ffff", "#ff8849ff",
	    "#8e35ffff", "#ff3b2eff", "#76fa4eff", "#2926ffff", "#ff8849ff","#cba93dff", "#fd3222ff", "#ff318bff", "#75fb4dff", "#ff8d51ff", 
	    "#0000ffff", "#983dffff", "#ff392aff", "#75fb4cff", "#0100ffff", "#ff9058ff", "#ff3089ff", "#75fbfdff", "#7712ffff", "#ff3221ff", 
	    "#81ff5fff", "#1a17ffff", "#ff8e58ff", "#ff2d87ff", "#8bfaffff", "#8e35ffff", "#ff3b2eff", "#76fa4eff", "#2926ffff", "#ff8849ff"};
    public List<BaseElementPanel> awardElements = new List<BaseElementPanel> () ;

	public AwardResult.AwardPayLine awardPayLine;
	private List<Vector2> m_LinePoints = new List<Vector2>();
	private Color m_LineColor = Color.white;
	private Sprite m_PayLineSprite = null;
    public bool ShowSpaghetti = false;
    public GameConfigs.PayLineType ShowPaylineType = GameConfigs.PayLineType.None;
    public bool forceInitAnimationLine = false;
    public bool NeedReBuildPayLine = true;
	private GameObject m_boxParticle = null;
    public AwardLineElement(AwardResult.AwardPayLine awardPayLine){
		this.awardPayLine = awardPayLine;
	}

	public List<Vector2> LinePoints
	{
		get{
			return m_LinePoints;
		}
	}
    private Dictionary<int,int> m_AwardElementRendererData = new Dictionary<int, int>();
    public Dictionary<int, int> AwardElementReelIndexData{
        get{
            return m_AwardElementRendererData;
        }
    }
	public Color LineColor
	{
		get{
			return m_LineColor;
		}
	}

	public Sprite PayLineSprite
	{
		get{
			return this.m_PayLineSprite;
		}
	}

	public GameObject BoxParticle
	{
		get {
			return this.m_boxParticle;
		}
	}

    private void InitAwardSymbolData(){
        m_AwardElementRendererData.Clear();
        for (int i = 0; i < awardElements.Count; i++)
        {
            m_AwardElementRendererData.Add(awardElements[i].ReelIndex,awardElements[i].PositionId);
        }
    }

    public void PlayAwardAnimation (bool needShowWinText, ReelManager reelManager ,bool ShowSpaghetti = false,bool playlineSound = true)
    {

		bool needFindWinTextAnimation = needShowWinText;

		if (needFindWinTextAnimation) {
			for (int i = awardElements.Count - 1; i >= 0; i--) {
				if ( (awardElements [i].ReelIndex == ((awardElements [i].BoardReelNum / 2))   )){

					awardElements [i].AwardNumber = Utils.Utilities.CastValueLong(System.Math.Round(awardPayLine.awardValue * AwardResult.CurrentResultBet));
					needFindWinTextAnimation = false;
					break;
				} 
			}
		}
		if (needFindWinTextAnimation) {
			for (int i = awardElements.Count - 1; i >= 0; i--) {
				if ( (awardElements [i].ReelIndex == ((awardElements [i].BoardReelNum / 2-1))   )){
					awardElements [i].AwardNumber = Utils.Utilities.CastValueLong(System.Math.Round(awardPayLine.awardValue * AwardResult.CurrentResultBet));
					needFindWinTextAnimation = false;
					break;
				} 
			}
		}
		if (needFindWinTextAnimation) {
			for (int i = awardElements.Count - 1; i >= 0; i--) {
				if ( (awardElements [i].ReelIndex == ((awardElements [i].BoardReelNum / 2+1))   )){
					awardElements [i].AwardNumber = Utils.Utilities.CastValueLong(System.Math.Round(awardPayLine.awardValue * AwardResult.CurrentResultBet));
					needFindWinTextAnimation = false;
					break;
				} 
			}
		}


        if (needFindWinTextAnimation) {
			if (awardElements.Count > 0) {
				awardElements [0].AwardNumber = Utils.Utilities.CastValueLong(System.Math.Round(awardPayLine.awardValue * AwardResult.CurrentResultBet));
			}
        }

		#if PLATFORM_GOLDS
		//gold的ShowSpaghetti时不需要播放symbol动画
		if(!ShowSpaghetti)
		{
			for (int i=awardElements.Count -1; i>=0; i--) {
				if (i < awardPayLine.awardPlayAnimations.Count) {
					awardElements [i].PlayAnimation (awardPayLine.awardPlayAnimations [i]);
				} else {
					throw new UnityException ("awardAnimation error");
				}
			}
        }else
        {
            //if(playlineSound) AudioEntity.Instance.PlayEffect("line_all");
        }
			
		#else
        for (int i=awardElements.Count -1; i>=0; i--) {
            if (i < awardPayLine.awardPlayAnimations.Count) {
		awardElements [i].PlayAnimation (awardPayLine.awardPlayAnimations [i]);
            } else {
                throw new UnityException ("awardAnimation error");
            }
        }
		#endif
		if (awardPayLine.awardValue > 0) {
			Messenger.Broadcast<AwardResult.AwardPayLine> (SlotControllerConstants.ShowAwardAnimation, awardPayLine);
            if (BaseSlotMachineController.Instance.reelManager.gameConfigs.EnableAnimationPayLine && awardElements.Count >= 1&&NeedReBuildPayLine) {
                this.ShowSpaghetti = ShowSpaghetti;
				Messenger.Broadcast<AwardLineElement, ReelManager> (SlotControllerConstants.PAYLINE_ANIMATION_SHOW, this,reelManager);
			}
		}
    }

    /// <summary>
    /// 仅仅播放中奖线动画 不播放中奖线上符号动画 而且中奖线动画被延迟到4s
    /// </summary>
    /// <param name="needShowWinText">If set to <c>true</c> need show window text.</param>
    public void PlayAwardAnimationSlower(bool needShowWinText)
    {

        bool needFindWinTextAnimation = needShowWinText;

        if (needFindWinTextAnimation)
        {
            for (int i = awardElements.Count - 1; i >= 0; i--)
            {
                if ((awardElements[i].ReelIndex == ((awardElements[i].BoardReelNum / 2))))
                {

					awardElements[i].AwardNumber = Utils.Utilities.CastValueLong(System.Math.Round(awardPayLine.awardValue * AwardResult.CurrentResultBet));
                    needFindWinTextAnimation = false;
                    break;
                }
            }
        }
        if (needFindWinTextAnimation)
        {
            for (int i = awardElements.Count - 1; i >= 0; i--)
            {
                if ((awardElements[i].ReelIndex == ((awardElements[i].BoardReelNum / 2 - 1))))
                {
					awardElements[i].AwardNumber = Utils.Utilities.CastValueLong(System.Math.Round(awardPayLine.awardValue * AwardResult.CurrentResultBet));
                    needFindWinTextAnimation = false;
                    break;
                }
            }
        }
        if (needFindWinTextAnimation)
        {
            for (int i = awardElements.Count - 1; i >= 0; i--)
            {
                if ((awardElements[i].ReelIndex == ((awardElements[i].BoardReelNum / 2 + 1))))
                {
					awardElements[i].AwardNumber = Utils.Utilities.CastValueLong(System.Math.Round(awardPayLine.awardValue * AwardResult.CurrentResultBet));
                    needFindWinTextAnimation = false;
                    break;
                }
            }
        }

        if (awardPayLine.awardValue > 0)
        {
            Messenger.Broadcast<AwardResult.AwardPayLine>(SlotControllerConstants.ShowAwardAnimation, awardPayLine);
            if (BaseSlotMachineController.Instance.reelManager.gameConfigs.EnableAnimationPayLine && awardElements.Count >= 1&& NeedReBuildPayLine)
            {
                Messenger.Broadcast<AwardLineElement>(SlotControllerConstants.PAYLINE_ANIMATION_SHOW_Slower, this);
            }
        }
    }


    /// <summary>
    /// 不播放中奖线的PlayAwardAnimation(用于RedWhiteBlue)
    /// </summary>
    /// <param name="needShowWinText">If set to <c>true</c> need show window text.</param>
    public void PlayAwardAnimationNoAwardLine(bool needShowWinText)
    {

        bool needFindWinTextAnimation = needShowWinText;

        if (needFindWinTextAnimation)
        {
            for (int i = awardElements.Count - 1; i >= 0; i--)
            {
                if ((awardElements[i].ReelIndex == ((awardElements[i].BoardReelNum / 2))))
                {

					awardElements[i].AwardNumber = Utils.Utilities.CastValueLong(System.Math.Round(awardPayLine.awardValue * AwardResult.CurrentResultBet));
                    needFindWinTextAnimation = false;
                    break;
                }
            }
        }
        if (needFindWinTextAnimation)
        {
            for (int i = awardElements.Count - 1; i >= 0; i--)
            {
                if ((awardElements[i].ReelIndex == ((awardElements[i].BoardReelNum / 2 - 1))))
                {
					awardElements[i].AwardNumber = Utils.Utilities.CastValueLong(System.Math.Round(awardPayLine.awardValue * AwardResult.CurrentResultBet));
                    needFindWinTextAnimation = false;
                    break;
                }
            }
        }
        if (needFindWinTextAnimation)
        {
            for (int i = awardElements.Count - 1; i >= 0; i--)
            {
                if ((awardElements[i].ReelIndex == ((awardElements[i].BoardReelNum / 2 + 1))))
                {
					awardElements[i].AwardNumber = Utils.Utilities.CastValueLong(System.Math.Round(awardPayLine.awardValue * AwardResult.CurrentResultBet));
                    needFindWinTextAnimation = false;
                    break;
                }
            }
        }


        if (needFindWinTextAnimation)
        {
            if (awardElements.Count > 0)
            {
				awardElements[0].AwardNumber = Utils.Utilities.CastValueLong(System.Math.Round(awardPayLine.awardValue * AwardResult.CurrentResultBet));
            }
        }

        for (int i = awardElements.Count - 1; i >= 0; i--)
        {
            if (i < awardPayLine.awardPlayAnimations.Count)
            {
                awardElements[i].PlayAnimation(awardPayLine.awardPlayAnimations[i]);
            }
            else
            {
                throw new UnityException("awardAnimation error");
            }
        }
    }

    public virtual void PlayAwardTipAnimation(AwardResult.AwardPayLine awardPayLine){
        for (int i=awardElements.Count -1; i>=0; i--) {
            if (i < awardPayLine.awardPlayAnimations.Count) {
                awardElements [i].PlayTipAnimation (awardPayLine.awardPlayAnimations [i]);
            } else {
                throw new UnityException ("awardTipAnimation error");
            }
        }
        if (awardPayLine.awardValue > 0) {
            Messenger.Broadcast<AwardResult.AwardPayLine> (SlotControllerConstants.ShowAwardAnimation, awardPayLine);
        }
    }

    public virtual void PauseAwardTipAnimation(){
        for (int i=0; i<awardElements.Count; i++) {
            awardElements [i].PauseTipAnimation ();
        }
    }
	public void PauseAwardAnimation (bool notChange = false,ReelManager reelManager=null)
    {
        for (int i=0; i<awardElements.Count; i++) {
			awardElements [i].PauseAnimation (notChange);
        }
        if (BaseSlotMachineController.Instance.reelManager.gameConfigs.EnableAnimationPayLine&&NeedReBuildPayLine) {
			Messenger.Broadcast (SlotControllerConstants.PAYLINE_ANIMATION_HIDE, reelManager);
		}
    }

    public void PlayNonSelectionAnimation(int animationId){

        for (int i=0; i<awardElements.Count; i++) {
            awardElements [i].PlayNonSelectionAnimation (animationId);
        }
    }

	public void InitAnimationLine(ReelManager reelManager,bool forceInit = false)
	{
		if (awardPayLine.LineIndex < 0&&!forceInit) {
			return;
		}
        if (BaseSlotMachineController.Instance == null || BaseSlotMachineController.Instance.reelManager == null) return;

		//ReelManager reelManager = BaseSlotMachineController.Instance.reelManager;
		if (reelManager.gameConfigs.payLineConfig.IsUseTexture) {
			InitTextureLine (reelManager);
		} else if(reelManager.gameConfigs.payLineConfig.payLineType != GameConfigs.PayLineType.BoxParticle){
			InitNormalLine (reelManager);
		}

		//添加particle粒子效果
		this.m_boxParticle = reelManager.gameConfigs.payLineConfig.boxParticle;
	}

	private void InitTextureLine(ReelManager reelManager)
	{
		//ReelManager reelManager = BaseSlotMachineController.Instance.reelManager;
		InitAwardSymbolData();
		if (PaylinesAssets.Instance != null)
		{
			int renderIndex = Math.Max(awardPayLine.LineIndex, 0);
			if (reelManager.gameConfigs.payLineConfig.payLineType != GameConfigs.PayLineType.AllLineFrame)
			{
				m_LinePoints = PaylinesAssets.Instance.GetPayLinePoints (renderIndex);
			}
			this.m_PayLineSprite = PaylinesAssets.Instance.GetPayLineSprite (renderIndex);
		}

	}

	private void InitNormalLine(ReelManager reelManager)
	{
		//ReelManager reelManager = BaseSlotMachineController.Instance.reelManager;
		int lineTableMaxValue = reelManager.lineTable.MaxValue;

		Core.PayLine payLine= awardPayLine.payLine;

		int lineTableCount = reelManager.lineTable.TotalPayLineCount ();
		m_LinePoints.Clear ();
		//		for (int i = 0; i < awardElements.Count; i++) {
		//			if (i == 0) {
		//				m_LinePoints.Add (new Vector2 (0, awardElements[0].CenterAnchorY)); 
		//			}
		//			Vector2 v = new Vector2 (awardElements [i].ReelContronller.reelManager.Reels [i].CenterAnchorX, awardElements [i].CenterAnchorY);
		//			m_LinePoints.Add(v); 
		//		}
		//
		//		m_LinePoints.Add (new Vector2 (1f, awardElements[awardElements.Count - 1].CenterAnchorY));
		if (reelManager.gameConfigs.hasBlank) {
			int N = (lineTableMaxValue +1);
			m_LinePoints.Add (new Vector2 (0, (float)payLine.RowNumberAt (0)/N));
			for (int i = 0; i < payLine.GetSize (); i++) {
				m_LinePoints.Add (new Vector2 ((2f*i+1f)/(reelManager.GetReelCount()*2f), (float)payLine.RowNumberAt (i) / N));
			}
			m_LinePoints.Add (new Vector2 (1, (float)payLine.GetLastNumber() / N));

		} else {
			int N = (lineTableMaxValue +1)*2;
			m_LinePoints.Add (new Vector2 (0, (float)(payLine.RowNumberAt (0) * 2+1)/N));
			for (int i = 0; i < payLine.GetSize (); i++) {
				m_LinePoints.Add (new Vector2 ((2f*i+1f)/(reelManager.GetReelCount()*2f), (float)(payLine.RowNumberAt (i) *2+1) / N));
			}
			m_LinePoints.Add (new Vector2 (1, (float)(payLine.GetLastNumber()*2+1) / N));
		}

		Color color = Color.white;
		InitAwardSymbolData();
		string[] currentPayLine = null;
		if (lineTableCount <= 5) {
			currentPayLine = PAYLINE_5_COLOR_ARRAY;
		} else if (lineTableCount <= 9) {
			currentPayLine = PAYLINE_9_COLOR_ARRAY;
		}
		else if(lineTableCount <= 20)
		{
			currentPayLine = PAYLINE_20_COLOR_ARRAY;
		}else if (lineTableCount <= 25)
		{
			currentPayLine = PAYLINE_25_COLOR_ARRAY;
		}
		else if (lineTableCount <= 50)
		{
			currentPayLine = PAYLINE_50_COLOR_ARRAY;
		}
		else if (lineTableCount <= 80)
		{
			currentPayLine = PAYLINE_80_COLOR_ARRAY;
		}
		else if (lineTableCount <= 40)
		{
			currentPayLine = PAYLINE_40_COLOR_ARRAY;
		}
		else {
			Log.Error (lineTableCount + "is big than playlins count");
		}

		if (awardPayLine.LineIndex < currentPayLine.Length) {
			ColorUtility.TryParseHtmlString (currentPayLine [awardPayLine.LineIndex], out color);
		}
		m_LineColor = color;

	}

    //	对于没有blank的机器（FruitBar，2x5Reels）
    //	分母N＝（所有payline数字中最大者＋1）＊2
    //	比如FruitBar，payline数字0，1，2，3，最大3，N＝8（8等分）
    //	分子M＝对应payline数字＊2＋1
    //	比如FruitBar，payline：0000，M＝0*2+1=1（画在八分之一处）
    //
    //		对于由blank的机器（3x）
    //		分母N＝（所有payline数字中最大者＋1）
    //		分子M＝对应pauline数字
    //		比如3x，payline数字123，则N＝4（4等分）
    //		当payline数字＝1时，M＝1（画在四分之一处）
    //
}
