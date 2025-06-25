using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Classic;
using UnityEngine.UI;
using Libs;

public class RespinSymbolRender : WebmSymbolRender {

	[HideInInspector]
	public bool CanRun = true;  //用来标识是jackpot的symbol的话则不需要旋转，为false值。

	public WebmSymbolRender m_DefaultSymbolRender;
	public WebmSymbolRender m_FirstSymbolRender;
	public WebmSymbolRender m_SecondSymbolRender;

	private List<WebmSymbolRender> mSymbolRenders = new List<WebmSymbolRender>();


	public Mask m_Mask;

	void Awake()
	{
		this.m_DefaultSymbolRender = m_FirstSymbolRender;
		mSymbolRenders.Add (m_FirstSymbolRender);
		mSymbolRenders.Add (m_SecondSymbolRender);
	}

	protected override void SymbolChangeHandler ()
	{
		this.m_DefaultSymbolRender.ChangeSymbol (this.symbolIndex);
	}

	public override void SetStaticPosition(int index)
	{
		base.SetStaticPosition (index);
		this.m_DefaultSymbolRender.PositionId = index;
	}

	public override void InitSymbolHandler(){
		this.m_FirstSymbolRender.ControllerAdapter = this.m_SecondSymbolRender.ControllerAdapter = this.ControllerAdapter;
		this.m_FirstSymbolRender.ReelAdapter = this.m_SecondSymbolRender.ReelAdapter = this.ReelAdapter;

		this.m_DefaultSymbolRender.PositionId = this.PositionId;
		m_SecondSymbolRender.PositionId = this.PositionId;
		//		this.SymbolIndex = SymbolIndex;	
		AnimationGO = null;

		m_SecondSymbolRender.ChangeSymbol( CreateRandomIndex ());
	
		Vector2 v = (this.transform as RectTransform).sizeDelta;
		(m_FirstSymbolRender.transform as RectTransform).sizeDelta = v;
		(m_SecondSymbolRender.transform as RectTransform).sizeDelta = v;

		(m_SecondSymbolRender.transform as RectTransform).localPosition = new Vector3 (0f, v.y, 0f);
	}

	public override void RecordPosition(float minY, float maxY)
	{
		Vector2 v = (this.transform as RectTransform).sizeDelta;
		m_FirstSymbolRender.RecordPosition (-v.y,v.y);
		m_SecondSymbolRender.RecordPosition (-v.y,v.y);
	}

	public int CreateRandomIndex ()
	{
		List<ResultContent.WeightData> reelResult =  (this.ControllerAdapter as ReelManager).resultContent.ReelResults [this.ReelIndex].reelData; 

		int randomIndex = UnityEngine.Random.Range (0, reelResult.Count);
		return reelResult[randomIndex].value;
	}

	public override bool MoveDistance(float offsetAllY,ref int zIndex)
	{
		if (!CanRun) {
			return false;
		}

		bool bFirstChangePage = false, bSecondChangePage = false;
		if (this.m_FirstSymbolRender.MoveDistance (offsetAllY, ref zIndex)) {
			this.m_FirstSymbolRender.ChangeSymbol (CreateRandomIndex ());
			bFirstChangePage = true;
		}
		if (this.m_SecondSymbolRender.MoveDistance (offsetAllY, ref zIndex)) {
			this.m_SecondSymbolRender.ChangeSymbol (CreateRandomIndex ());
			bSecondChangePage = true;
			//			m_DefaultSymbolRender = m_FirstSymbolRender;
		}
		return bFirstChangePage || bSecondChangePage;
	}

	public override void InitializePosition()
	{
		this.m_FirstSymbolRender.InitializePosition ();
		this.m_SecondSymbolRender.InitializePosition ();
	}

	public override void QuickStopMoveDistance(float offsetY)
	{
		if (!CanRun) {
			return;
		}
		this.m_FirstSymbolRender.QuickStopMoveDistance (offsetY);
		this.m_SecondSymbolRender.QuickStopMoveDistance (offsetY);
	}
}
