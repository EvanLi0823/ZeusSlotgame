using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Classic;
using UnityEngine.UI.Extensions;
using Libs;
using DG.Tweening;
using MarchingBytes;
using Pool = Libs.Pool;

public class PayLineAnimation : MonoBehaviour {
	private const string ONLY_PAYLINE_NAME = "ONLY_PAYLINE_NAME";
	private const string FRAME_PAYLINE_LINE_NAME = "FRAME_PAYLINE_LINE_NAME";
	private const string FRAME_PAYLINE_BOX_NAME = "FRAME_PAYLINE_BOX_NAME";
	private const string BOX_PARTICLE_PAYLINE_NAME = "BOX_PARTICLE_PAYLINE_NAME";

    //缓冲池名称前缀，默认为空，多棋盘如果想用和mainreelManager用不同的payline缓冲池，用PaylinePoolNamePrefix脚本配置不同的前缀就可以了
    private string paylinePoolPrefix = "";


    public GameObject payLineGameObject;
    public GameObject frameGameObject;
    public GameObject payLineWithFrameGameObject;
    
    [Header("每个子棋盘设置自己的reelmanager")]
    public ReelManager SelfReelManager;
	private Tween t;

	void Awake()
	{
		Messenger.AddListener<AwardLineElement,ReelManager> (SlotControllerConstants.PAYLINE_ANIMATION_SHOW,ShowAwardPayLine);
        //Messenger.AddListener<AwardLineElement>(SlotControllerConstants.PAYLINE_ANIMATION_SHOW_Slower, ShowAwardPayLineSlower);
		Messenger.AddListener<ReelManager> (SlotControllerConstants.PAYLINE_ANIMATION_HIDE,HideAwardPayLine);
        Messenger.AddListener<List<BaseElementPanel>, GameObject,ReelManager> (SlotControllerConstants.PAYLINE_ANIMATION_SHOW_BOXPARTICLE, ShowAllPayLineBoxParticle);
        Messenger.AddListener<List<BaseElementPanel>,ReelManager>(SlotControllerConstants.PAYLINE_SYMBOL_ANIMATION_HIDE,HidePayLineBoxParticle);
        PaylinePoolNamePrefix paylinePoolNamePrefix = this.gameObject.GetComponent<PaylinePoolNamePrefix>();
        if (null != paylinePoolNamePrefix)
            paylinePoolPrefix = paylinePoolNamePrefix.namePrefix;
    }

	void OnDestroy()
	{
		Messenger.RemoveListener<AwardLineElement, ReelManager> (SlotControllerConstants.PAYLINE_ANIMATION_SHOW,ShowAwardPayLine);
        //Messenger.RemoveListener<AwardLineElement>(SlotControllerConstants.PAYLINE_ANIMATION_SHOW_Slower, ShowAwardPayLineSlower);
		Messenger.RemoveListener<ReelManager> (SlotControllerConstants.PAYLINE_ANIMATION_HIDE,HideAwardPayLine);
        Messenger.RemoveListener<List<BaseElementPanel>,GameObject, ReelManager> (SlotControllerConstants.PAYLINE_ANIMATION_SHOW_BOXPARTICLE, ShowAllPayLineBoxParticle);
        Messenger.RemoveListener<List<BaseElementPanel>,ReelManager>(SlotControllerConstants.PAYLINE_SYMBOL_ANIMATION_HIDE,HidePayLineBoxParticle);

        ClearPools();
    }

    private void ClearPools()
    {
        PoolMgr.DefaultPools.GetOrCreatePool(paylinePoolPrefix + ONLY_PAYLINE_NAME).Clear();
        PoolMgr.DefaultPools.GetOrCreatePool(paylinePoolPrefix + FRAME_PAYLINE_LINE_NAME).Clear();
        PoolMgr.DefaultPools.GetOrCreatePool(paylinePoolPrefix + FRAME_PAYLINE_BOX_NAME).Clear();
        PoolMgr.DefaultPools.GetOrCreatePool(paylinePoolPrefix + BOX_PARTICLE_PAYLINE_NAME).Clear();
    }

    void ShowAwardPayLine(AwardLineElement awardLineData,ReelManager reelManager)
	{
        if (SelfReelManager != null && reelManager !=null && SelfReelManager != reelManager)
        {
            return;
        }

        if (reelManager == null)
        {
	        reelManager = BaseSlotMachineController.Instance.reelManager;
        }

        SelfReelManager = reelManager;

        gameConfigs =reelManager.gameConfigs;
        if (gameConfigs == null) return;

		awardLineData.InitAnimationLine(reelManager,awardLineData.forceInitAnimationLine);
        if (awardLineData.ShowSpaghetti)
        {
	        if (gameConfigs.payLineConfig.payLineType == GameConfigs.PayLineType.AllLineFrame)
		        ShowAwardAllLineWithFrame (awardLineData);
            else if (gameConfigs.payLineConfig.SpaghettiPayLineType == GameConfigs.PayLineType.OnlyLine) 
		        ShowAwardPayLineWithoutFrame(awardLineData,true);
            else if (gameConfigs.payLineConfig.SpaghettiPayLineType == GameConfigs.PayLineType.LineAndFrame) 
		        ShowAwardPayLineWithFrame(awardLineData);
        }
        else if (awardLineData.ShowPaylineType == GameConfigs.PayLineType.OnlyFrame)
        {
	        ShowAwardPaylineOnlyFrame(awardLineData, false);
        }
        else
        {
			if (gameConfigs.payLineConfig.payLineType == GameConfigs.PayLineType.OnlyLine)
				ShowAwardPayLineWithoutFrame (awardLineData, false);
			else if (gameConfigs.payLineConfig.payLineType == GameConfigs.PayLineType.LineAndFrame)
				ShowAwardPayLineWithFrame (awardLineData);
			else if (gameConfigs.payLineConfig.payLineType == GameConfigs.PayLineType.BoxParticle)
				ShowPayLineBoxParticle (awardLineData);
	        else if (gameConfigs.payLineConfig.payLineType == GameConfigs.PayLineType.AllLineFrame)
		        ShowAwardAllLineWithFrame (awardLineData);
			
        }

    }

    #region RedYellowBlueSlotGame
    /// <summary>
    /// 显示中奖线更加缓慢 4s
    /// </summary>
  //  void ShowAwardPayLineSlower(AwardLineElement awardLineData)
  //  {
  //      awardLineData.InitAnimationLine();
  //      if (awardLineData == null || payLineGameObject == null || awardLineData.LinePoints.Count == 0)
  //      {
  //          return;
  //      }
		//GameObject o = (PoolMgr.DefaultPools.GetOrCreatePool (ONLY_PAYLINE_NAME).CreateObject (payLineGameObject) as GameObject); //Instantiate(payLineGameObject); //(PoolMgr.DefaultPools.GetOrCreatePool (PAYLINE_ANIMATION).CreateObject (payLineGameObject) as GameObject);
		//o.name = ONLY_PAYLINE_NAME;
		//o.SetActive(true);
  //      UILineRenderer lineRender = o.GetComponent<UILineRenderer>();
  //      if (lineRender == null)
  //      {
  //          return;
  //      }
  //      lineRender.Points = awardLineData.LinePoints.ToArray();
  //      lineRender.color = awardLineData.LineColor;
		//lineRender.sprite = awardLineData.PayLineSprite;
    //    o.transform.SetParent(this.transform, false);

    //    lineRender.SetClipRect(new Rect(0f, 0f, 0f, 0), true);

    //    float initNum = -Screen.width / 2;

    //    t = DOTween.To(() => initNum, x => initNum = x, Screen.width * 2, 3.5f).OnUpdate(delegate () {
    //        if (lineRender != null)
    //        {
    //            lineRender.SetClipRect(new Rect(-Screen.width / 2, -Screen.height, initNum, Screen.height * 2), true);
    //        }
    //    });
    //}
    #endregion

    #region Show AwardPaylineWithFrame
    GameConfigs gameConfigs;
	void ShowAwardAllLineWithFrame(AwardLineElement awardLineData,bool isSpaghetti = false)
	{
		awardLineElement = awardLineData;
       
		if (awardLineData == null || payLineWithFrameGameObject == null ) return;

		InitGridRenderersData(awardLineData,isSpaghetti);
		

	}
	void ShowAwardPayLineWithFrame(AwardLineElement awardLineData,bool isSpaghetti = false)
    {
        awardLineElement = awardLineData;
       
        if (awardLineData == null || payLineWithFrameGameObject == null || awardLineData.LinePoints.Count == 0) return;

		InitGridRenderersData(awardLineData,isSpaghetti);
	    InitLineRenderersData(awardLineData,isSpaghetti);
    }

	void ShowAwardPaylineOnlyFrame(AwardLineElement awardLineData,bool isSpaghetti = false)
	{
		awardLineElement = awardLineData;
		if (awardLineData == null || payLineWithFrameGameObject == null|| awardLineData.LinePoints.Count == 0) return;
		InitGridRenderersData(awardLineData,isSpaghetti);


	}
    List<CustomGridPaylineRenderer> gridRenderers = new List<CustomGridPaylineRenderer>();
    AwardLineElement awardLineElement;

	void InitGridRenderersData(AwardLineElement awardLineData,bool isSpaghetti = false)
    {

//        float width = gameConfigs.reelConfigs.Length;
       
        List<int> ls = new List<int>(awardLineData.AwardElementReelIndexData.Keys);
        gridRenderers.Clear();
        for (int i = 0; i < ls.Count; i++)
        {
			GameObject go =  (PoolMgr.DefaultPools.GetOrCreatePool (paylinePoolPrefix + FRAME_PAYLINE_BOX_NAME).CreateObject (frameGameObject) as GameObject);//Instantiate(frameGameObject);
			go.name = paylinePoolPrefix + FRAME_PAYLINE_BOX_NAME;
			go.SetActive (true);
			CustomGridPaylineRenderer gridRenderer = go.GetComponent<CustomGridPaylineRenderer>();
            if (gridRenderer == null) return;
            RectTransform rt = go.transform as RectTransform;
            int reelIndex = ls[i];
            gridRenderer.Margin = new Vector2(gameConfigs.payLineConfig.LineAndFrameThickness,gameConfigs.payLineConfig.LineAndFrameThickness);
            int rowIndex = awardLineData.AwardElementReelIndexData[ls[i]];
            rt.anchorMin = new Vector2(GetGridLeftAnchor(reelIndex,rowIndex),GetGridBottomAnchor(reelIndex,rowIndex) );
            rt.anchorMax = new Vector2(GetGridRightAnchor(reelIndex,rowIndex), GetGridTopAnchor(reelIndex,rowIndex));
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            gridRenderer.color = awardLineData.LineColor;
			gridRenderer.sprite = awardLineData.PayLineSprite;
            gridRenderer.LineThickness = gameConfigs.payLineConfig.LineAndFrameThickness;
            gridRenderer.BezierMode = UILineRenderer.BezierType.None;
            go.transform.SetParent(this.transform, false);
            gridRenderers.Add(gridRenderer);

			SetLineAlphaChange (gridRenderer, isSpaghetti);
        }
    }

    float GetGridLeftAnchor(int reelIndex,int rowIndex)
    {
	    float width = SelfReelManager.boardController.GetReelWidth() / SelfReelManager.boardController.GetBoardWidth(); 
       
        return reelIndex * GetReelSpacePercent(reelIndex) + reelIndex * width;
    }
    float GetGridRightAnchor(int reelIndex, int rowIndex){
        
	    float width = SelfReelManager.boardController.GetReelWidth() / SelfReelManager.boardController.GetBoardWidth();

        return reelIndex * GetReelSpacePercent(reelIndex) + (reelIndex + 1) * width;
    }
    float GetGridTopAnchor(int reelIndex, int rowIndex){
        
        float elementRelativelyHeight = 1.0f / SelfReelManager.GetReelSymbolRenderCount(reelIndex);//gameConfigs.reelConfigs[reelIndex].elementHeight / gameConfigs.reelConfigs[reelIndex].PanelHeight;

        return (rowIndex + 1) * elementRelativelyHeight;
    }
    float GetGridBottomAnchor(int reelIndex, int rowIndex)
    {

	    float elementRelativelyHeight = 1.0f / SelfReelManager.GetReelSymbolRenderCount(reelIndex); // gameConfigs.reelConfigs[reelIndex].elementHeight / gameConfigs.reelConfigs[reelIndex].PanelHeight;

        return rowIndex * elementRelativelyHeight;
    }
//    protected float GetTotalReelPanelSpaceWidthPercent()
//    {
//        float totalReelPanelSpaceWidthPercent = 1 - GetTotalReelPanelWidthPercent();
//        if (totalReelPanelSpaceWidthPercent < 0)
//        {
//            totalReelPanelSpaceWidthPercent = 0;
//        }
//        return totalReelPanelSpaceWidthPercent;
//    }

//    protected float GetTotalReelPanelWidthPercent()
//    {
//        float totalReelPanelWidth = 0;
//        for (int i = 0; i < gameConfigs.GetReelNumber(); i++)
//        {
//            totalReelPanelWidth += GetReelPanelWidth(i);
//        }
//
//        if (gameConfigs.PanelWidth > 0)
//        {
//            float totalPanelWidthPercent = totalReelPanelWidth / gameConfigs.PanelWidth;
//            if (totalPanelWidthPercent > 1)
//            {
//                totalPanelWidthPercent = 1;
//            }
//            return totalPanelWidthPercent;
//        }
//        return 0;
//    }

    protected float GetReelSpacePercent(int reelIndex)
    {
	    return SelfReelManager.boardController.GetBoardSpace() / SelfReelManager.boardController.GetBoardWidth();
//        float space = 0;
//
//        if (gameConfigs.GetReelNumber() > 1)
//        {
//            space = GetTotalReelPanelSpaceWidthPercent() / (gameConfigs.GetReelNumber() - 1);
//        }
//        return space;
    }
//    protected float GetReelPanelWidth(int reelIndex)
//    {
//        return gameConfigs.ReelPanelWidth;
//    }

	void InitLineRenderersData(AwardLineElement awardLineData,bool isSpaghetti = false)
    {
		GameObject o =(PoolMgr.DefaultPools.GetOrCreatePool (paylinePoolPrefix + FRAME_PAYLINE_LINE_NAME).CreateObject (payLineWithFrameGameObject) as GameObject); //Instantiate(payLineWithFrameGameObject);
		o.name= paylinePoolPrefix + FRAME_PAYLINE_LINE_NAME;

        CustomPayLineRenderer lineRender = o.GetComponent<CustomPayLineRenderer>();
        if (lineRender == null) return;

        lineRender.Points = awardLineData.LinePoints.ToArray();
        lineRender.color = awardLineData.LineColor;
		lineRender.sprite = awardLineData.PayLineSprite;
        lineRender.dict.Clear();
        lineRender.UseMargins = false;
        lineRender.LineThickness = gameConfigs.payLineConfig.LineAndFrameThickness;
        o.transform.SetParent(this.transform, false);
        o.transform.SetAsFirstSibling();
        List<int> ls = new List<int>(awardLineData.AwardElementReelIndexData.Keys);
        for (int i = 0; i < ls.Count; i++)
        {
            lineRender.dict.Add(ls[i] + 1, GetAdjacentValidPositions(ls[i], awardLineData.AwardElementReelIndexData[ls[i]]));//reelIndex,List<Vector2>

			this.SetLineAlphaChange (lineRender, isSpaghetti);
        }
        o.SetActive(true);
    }

    List<Vector2> GetAdjacentValidPositions(int reelIndex, int rowIndex)
    {
	    float width = SelfReelManager.GetReelCount();
	    float height = SelfReelManager.GetReelSymbolRenderCount(reelIndex); 
        float normalizeWidth = 1 / width;
        float normalizeHeight = 1 / height;
        float marginX = gridRenderers[0].GetMarginPercentX() / width/2;
        float marginY = gridRenderers[0].GetMarginPercentY() / height/2;
        //Debug.Log("marginX:" + marginX + " marginY:" + marginY + " " + "reelIndex:" + reelIndex + " rowIndex:" + rowIndex);
		Vector2 currentPos;
		if (gameConfigs.payLineConfig.IsUseTexture) {
			currentPos= awardLineElement.LinePoints[reelIndex+1];
		}
		else
		{
			currentPos = new Vector2((reelIndex + reelIndex + 1) * normalizeWidth / 2f, (rowIndex + rowIndex + 1) * normalizeHeight / 2f);
		}
        Vector2 prePos = awardLineElement.LinePoints[reelIndex];
        Vector2 nextPos = awardLineElement.LinePoints[reelIndex + 2];

        Vector2 LTPos = new Vector2(GetGridLeftAnchor(reelIndex,rowIndex) + marginX, GetGridTopAnchor(reelIndex,rowIndex) - marginY);//Left Top
        Vector2 LBPos = new Vector2(GetGridLeftAnchor(reelIndex, rowIndex) + marginX, GetGridBottomAnchor(reelIndex,rowIndex) + marginY);//Left Bottom
        Vector2 RTPos = new Vector2(GetGridRightAnchor(reelIndex, rowIndex) - marginX, GetGridTopAnchor(reelIndex, rowIndex) - marginY);//Right Top
        Vector2 RBPos = new Vector2(GetGridRightAnchor(reelIndex, rowIndex) - marginX, GetGridBottomAnchor(reelIndex, rowIndex) + marginY);//Right Bottom
        //Debug.Log("reelIndex:" + reelIndex + " rowIndex:" + rowIndex + " prePos:" + GetVector2Format(prePos) + " curPos:" + GetVector2Format(currentPos) + " nextPos:" + GetVector2Format(nextPos) + " LTPos:" + GetVector2Format(LTPos) + " LBPos:" + GetVector2Format(LBPos) + " RTPos:" + GetVector2Format(RTPos) + " RBPos:" + GetVector2Format(RBPos));

        return GetTgrgetPoint(prePos, currentPos, nextPos, LTPos, LBPos, RTPos, RBPos);
    }

    string GetVector2Format(Vector2 vec)
    {
        return string.Format(vec.x + "," + vec.y);
    }

    private List<Vector2> GetTgrgetPoint(Vector2 prePos, Vector2 curPos, Vector2 nextPos, Vector2 LTPos, Vector2 LBPos, Vector2 RTPos, Vector2 RBPos)
    {
        List<Vector2> list = new List<Vector2>();
        //左侧目标点
        Vector2 LeftTgtPos = Vector2.zero;
        if (IntersectLineSegmentsCheck.LineSeg_ab_cross_cd(LTPos, LBPos, prePos, curPos, ref LeftTgtPos) != -1) list.Add(LeftTgtPos);
        else if (IntersectLineSegmentsCheck.LineSeg_ab_cross_cd(LTPos, RTPos, prePos, curPos, ref LeftTgtPos) != -1) list.Add(LeftTgtPos);
        else if (IntersectLineSegmentsCheck.LineSeg_ab_cross_cd(LBPos, RBPos, prePos, curPos, ref LeftTgtPos) != -1) list.Add(LeftTgtPos);
        //右侧目标点
        Vector2 RightTgtPos = Vector2.zero;
        if (IntersectLineSegmentsCheck.LineSeg_ab_cross_cd(RTPos, RBPos, nextPos, curPos, ref RightTgtPos) != -1) list.Add(RightTgtPos);
        else if (IntersectLineSegmentsCheck.LineSeg_ab_cross_cd(LTPos, RTPos, nextPos, curPos, ref RightTgtPos) != -1) list.Add(RightTgtPos);
        else if (IntersectLineSegmentsCheck.LineSeg_ab_cross_cd(LBPos, RBPos, nextPos, curPos, ref RightTgtPos) != -1) list.Add(RightTgtPos);
        //Debug.Log("GetTgrgetPoint:" + list.Count + " Vector0:" + GetVector2Format(list[0]) + "Vector1:" + GetVector2Format(list[1]));
        return list;
    }

    #endregion

    #region Show AwardPayLineWithoutFrame
	//isSpaghetti用来影响gold中的划线,目前isSpaghetti先不用到
	void ShowAwardPayLineWithoutFrame(AwardLineElement awardLineData,bool isSpaghetti = false){
        if (awardLineData == null || payLineGameObject == null || awardLineData.LinePoints.Count == 0)
        {
            return;
        }
		GameObject o = (PoolMgr.DefaultPools.GetOrCreatePool (paylinePoolPrefix + ONLY_PAYLINE_NAME).CreateObject (payLineGameObject) as GameObject);//Instantiate(payLineGameObject); //(PoolMgr.DefaultPools.GetOrCreatePool (PAYLINE_ANIMATION).CreateObject (payLineGameObject) as GameObject);
		o.name = paylinePoolPrefix + ONLY_PAYLINE_NAME;
		o.SetActive(true);
        UILineRenderer lineRender = o.GetComponent<UILineRenderer>();
        if (lineRender == null)
        {
            return;
        }
        lineRender.Points = awardLineData.LinePoints.ToArray();
        lineRender.color = awardLineData.LineColor;
		lineRender.sprite = awardLineData.PayLineSprite;
        lineRender.LineThickness = gameConfigs.payLineConfig.OnlyLineThickness;
        o.transform.SetParent(this.transform, false);

        lineRender.SetClipRect(new Rect(0f, 0f, 0f, 0), true);

        float initNum = -Screen.width;


        t = DOTween.To(() => initNum, x => initNum = x, Screen.width * 2f, 1f).OnUpdate(delegate () {
            if (lineRender != null)
            {
                lineRender.SetClipRect(new Rect(-Screen.width, -Screen.height, initNum, Screen.height * 2), true);
            }
        });

		SetLineAlphaChange (lineRender, isSpaghetti);
    }

	private void ShowPayLineBoxParticle(AwardLineElement awardLineData)
	{
		if (awardLineData.BoxParticle == null) {
			return;
		}

		for (int i = 0; i < awardLineData.awardElements.Count; i++) {
			GameObject o = (PoolMgr.DefaultPools.GetOrCreatePool (paylinePoolPrefix + BOX_PARTICLE_PAYLINE_NAME).CreateObject (awardLineData.BoxParticle) as GameObject); //Instantiate (awardLineData.BoxParticle); //(PoolMgr.DefaultPools.GetOrCreatePool (PAYLINE_ANIMATION).CreateObject (payLineGameObject) as GameObject);
			o.name = paylinePoolPrefix + BOX_PARTICLE_PAYLINE_NAME;
			o.SetActive (true);
			o.transform.SetParent (this.transform, false);
			RectTransform rt = awardLineData.awardElements [i].transform as RectTransform;
			if (rt != null)
			{
				o.transform.position = rt.position; //new Vector3 (, awardLineData.LinePoints [i].y, 0f);
				if (rt.parent != null)
					SetChildPaylinePosition(rt.parent.name, o);
			} 
		}

	}

	private void ShowAllPayLineBoxParticle(List<BaseElementPanel> elements,GameObject boxParticle,ReelManager reelManager)
	{
		if (boxParticle == null) {
			return; 
		}

        if (SelfReelManager != null && reelManager != null && SelfReelManager != reelManager)
        {
            return;
        }
		if(elements==null||boxParticle==null) return;
        for (int i = 0; i < elements.Count; i++) {
	        if(elements[i] == null) return;
	        Pool pool = PoolMgr.DefaultPools?.GetOrCreatePool(paylinePoolPrefix + BOX_PARTICLE_PAYLINE_NAME);
	        if(pool==null) return;
	        GameObject o = pool.CreateObject (boxParticle) as GameObject; 
	        if(o==null) return;
			o.name = paylinePoolPrefix + BOX_PARTICLE_PAYLINE_NAME;
			o.SetActive (true);
			o.transform.SetParent (this.transform, false);

			if (elements[i] != null&&elements [i].transform!=null)
			{	
				RectTransform rt = elements [i].transform as RectTransform;
				if (rt != null)
				{
					o.transform.position = rt.position; //new Vector3 (, awardLineData.LinePoints [i].y, 0f);
					if (rt.parent != null)
						SetChildPaylinePosition(rt.parent.name, o);
				}
			}
		}
	}

	private Color alphaColor = new Color(1f,1f,1f,0f);
	private Color halfAlphaColor = new Color(1f,1f,1f,0.3f);
	Sequence s1;

    private void SetLineAlphaChange(UILineRenderer lineRender,bool isSpaghetti=false){
		//之前是用classic和gold来进行区分，后来因为移植机器也要用到，因而用texture来进行区分
		if (gameConfigs.payLineConfig.IsUseTexture)
		{
//			if (s != null)
//				s.Kill();
			DOTween.Kill(lineRender);
			Sequence s = DOTween.Sequence();
			
            switch (gameConfigs.payLineConfig.lineType)
            {
                case GameConfigs.LineType.None:
			        lineRender.color = Color.white;
                    s.Append (lineRender.DOColor (alphaColor, 0.8f).SetEase(Ease.InCubic))
				        .Append (lineRender.DOColor (Color.white, 0.8f).SetEase(Ease.OutQuart))
				        .Append (lineRender.DOColor (alphaColor, 0.4f).SetEase(Ease.InCubic));
                    break;
                case GameConfigs.LineType.Fade:
                    lineRender.color = alphaColor;
                    if (BaseSlotMachineController.Instance.reelManager.AutoRun) {
                        s.Append(lineRender.DOColor(halfAlphaColor, 0.1f).SetEase(Ease.InQuart))
                        .Append(lineRender.DOColor(Color.white, 0.3f).SetEase(Ease.InQuart))
                        .AppendInterval(0.2f)
//                        .Append(lineRender.DOColor(Color.white, 0.2f).SetEase(Ease.InQuart))
                        .Append(lineRender.DOColor(halfAlphaColor, 0.3f).SetEase(Ease.OutQuart))
                        .Append(lineRender.DOColor(alphaColor, 0.1f).SetEase(Ease.OutQuart));
                    }
                    else {
                        s.Append(lineRender.DOColor(halfAlphaColor, 0.1f).SetEase(Ease.InQuart))
                            .Append(lineRender.DOColor(Color.white, 0.3f).SetEase(Ease.InQuart))
                            .AppendInterval(0.2f)
//                            .Append(lineRender.DOColor(Color.white, 0.2f).SetEase(Ease.InQuart))
                            .Append(lineRender.DOColor(halfAlphaColor, 0.3f).SetEase(Ease.OutQuart))
                            .Append(lineRender.DOColor(alphaColor, 0.1f).SetEase(Ease.OutQuart))
                            .Append(lineRender.DOColor(halfAlphaColor, 0.1f).SetEase(Ease.InQuart))
                            .Append(lineRender.DOColor(Color.white, 0.3f).SetEase(Ease.InQuart))
                            .AppendInterval(0.2f)
//                            .Append(lineRender.DOColor(Color.white, 0.2f).SetEase(Ease.InQuart))
                            .Append(lineRender.DOColor(halfAlphaColor, 0.3f).SetEase(Ease.OutQuart))
                            .Append(lineRender.DOColor(alphaColor, 0.1f).SetEase(Ease.OutQuart));
                    }
                    //.Append(lineRender.DOColor(alphaColor, 0.4f).SetEase(Ease.Linear));
                    break;
                default:
                    break;
            }
			s.Play ();
			
//			if (isSpaghetti) {
//				lineRender.DOColor (alphaColor, 2.0f).SetEase(Ease.InQuart); //本为1.6s，但为了方便游戏中autospin，改为了2s
//			} else {
//				Sequence s = DOTween.Sequence();
//				s.Append (lineRender.DOColor (alphaColor, 0.8f).SetEase(Ease.InQuart))
//					.Append (lineRender.DOColor (Color.white, 0.8f).SetEase(Ease.OutQuart))
//					.Append (lineRender.DOColor (alphaColor, 0.4f).SetEase(Ease.Linear));
//				s.Play ();
//
//			}
		}
	}
    #endregion

    #region HidePayLine
    void HideAwardPayLine(ReelManager reelManager)
    {
        if (SelfReelManager != null && reelManager != null && SelfReelManager != reelManager)
        {
            return;
        }

        //        Util.DestroyChildren(this.transform);
        if (t != null)
        {
            t.Kill();
            t = null;
        }
      for (int i = transform.childCount - 1; i >= 0; i--) {           
          GameObject o = transform.GetChild (i).gameObject;
          if (o.activeSelf) {
//				o.SetActive (false);
                PoolMgr.DefaultPools.GetOrCreatePool (o.name).DestoryObject (o);
          }
      }
    }

    private void HidePayLineBoxParticle(List<BaseElementPanel> elements, ReelManager reelManager)
    {
	    if (SelfReelManager != null && reelManager != null && SelfReelManager != reelManager)
	    {
		    return;
	    }

	    if (elements == null || elements.Count == 0) return;
	    //        Util.DestroyChildren(this.transform);
	    if (t != null)
	    {
		    t.Kill();
		    t = null;
	    }
	    for (int i = transform.childCount - 1; i >= 0; i--) {

		    foreach (var element in elements)
		    {
			    GameObject o = transform.GetChild (i).gameObject;
			    if(o==null || element==null)continue;
			    if (element.transform.position==o.transform.position)//==近似相等
			    {
				    if (o.activeSelf) {
//					    o.SetActive (false);
					    PoolMgr.DefaultPools.GetOrCreatePool (o.name).DestoryObject (o);
				    }

				    break;
			    }
		    }
		   
	    }
    }
    #endregion

    /// <summary>
    /// 处理佐罗异形棋盘粒子框偏移
    /// </summary>
    /// <param name="reelName"></param>
    /// <param name="target"></param>
    public virtual void SetChildPaylinePosition(string reelName,GameObject target)
    {
	    
    }
}
