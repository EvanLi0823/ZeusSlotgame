using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Classic;

//1.中奖的则移除屏幕外，移动的位置，根据位置坐标错位设置symbolindex。
//2.移动symbol，添加偏移时间
public class DropReel : Reel {
	public List<AnimationCurve> DropDownCurves = new List<AnimationCurve>();

	//延迟数组，用来计算走的时候间隔
	private List<float> delayTimeList = new List<float>();
	private float startWaitTime = 0f;
	public float delayTwoRowTime = 0.2f;
//	private float delayTwoColumTime1 = 0.083f;
//	private float delayTwoColumTime2 = 0.05f;



	public List<float> BETWEEN_REEL_TIME = new List<float>{ 0.06f,0.03f,0.01f};//每个reel之间的时间间隔

	[Header("消除状态下的每行掉落时间")]
	public float delayTwoRowTimeInDrop = 0.2f;
	[Header("消除状态下的每列掉落时间")]
	public List<float> BETWEEN_REEL_TIME_DROP = new List<float>{ 0.06f,0.03f,0.01f};//每个reel之间的时间间隔
	private float SPEED_MULTIPLE = 0.2f;//越往上速度越快 
	//是否都掉落
	private bool IsAllDrop = true;

	private Dictionary<int,DropMoveData> m_DropData = new Dictionary<int, DropMoveData> ();

	private List<int> awardElementIndexList = new List<int>{0};

	private void Awake()
	{
		this.IsDropReel = true;
	}

	protected override void StartSpinHandler ()
	{
		IsAllDrop = this.m_RunData.IsAwardDropMode == false;
		awardElementIndexList = this.m_RunData.AwardIndexList;
		if (IsAllDrop)
		{
			delayTimeList.Clear ();
			float tmp = -startWaitTime;//- delayTwoColumTime * this.GetReelIndex();
			float offsetHeight = (ReelShowNum + 1) * this.m_boardConfig.SymbolHeight;
			for (int i = 0; i < this.SymbolRenders.Count; i++) {
				Vector3 v = this.SymbolRenders [i].InitialPosition;
				v.y += offsetHeight;
				this.SymbolRenders [i].transform.localPosition = v;
				//等差数列，目的是越来越快
				float tmp2 = tmp -  this.GetReelIndex () * BETWEEN_REEL_TIME[i];

				delayTimeList.Add (tmp2);
				tmp -= delayTwoRowTime;

				//重复
				int symbolIndex = m_RunData.ResultSymbols [i];

				SymbolRenders [i].ChangeSymbol (symbolIndex);

				SymbolRenders [i].SetStaticPosition (i);
				SymbolRenders [i].SetAnimationParent (this.m_ReelController.GetAnimationRender (this.ReelIndex, i));
				this.m_ReelController.SetSymbolRenderMap (this.ReelIndex, i, SymbolRenders [i] as BaseElementPanel);
			}

		} else {
			List<int> normalAwardList = new List<int>{ 0,1,2,4,4,4};//
			if (awardElementIndexList != null) {
				awardElementIndexList.ForEach (delegate(int obj) {
					normalAwardList.Remove (obj);
				});
			}

			m_ReelState = SpinStateEnum.Stop;
			Vector3 bottemPosition = this.SymbolRenders [0].InitialPosition;
			Vector3 tmpV = bottemPosition;

			delayTimeList.Clear ();
			float tmp = -startWaitTime;
			for (int i = 0; i < this.SymbolRenders.Count; i++) {
				int positionId = this.SymbolRenders [i].PositionId;
				this.CheckEmptyDropDict (positionId);

				int resultPosition = normalAwardList[i];
				//设置最低端的值
				this.m_DropData  [positionId].DropFromPosition = resultPosition;
				tmpV.y = bottemPosition.y + resultPosition * this.m_boardConfig.SymbolHeight;
				this.SymbolRenders [i].transform.localPosition = tmpV;
				this.SymbolRenders [i].NeedMove = this.m_DropData[positionId].NeedDrop();
				if (SymbolRenders [i].NeedMove)
				{
					m_ReelState = SpinStateEnum.NormalRun;
				}

				//等差数列，目的是越来越快 TODO:需要修改
				float tmp2 = tmp -  this.GetReelIndex () *BETWEEN_REEL_TIME_DROP[i];
				delayTimeList.Add (tmp2);
//				Debug.Log (this.GetReelIndex()+":"+i+":"+ tmp2);
				//只有需要移动的时候才会减少间隔
//				if (this.m_DropData [positionId].NeedDrop() == false) {
					tmp -= delayTwoRowTimeInDrop;
//				}
				//重复
				int symbolIndex = m_RunData.ResultSymbols [i];

				SymbolRenders [i].ChangeSymbol (symbolIndex);

				SymbolRenders [i].SetStaticPosition (i);
				SymbolRenders [i].SetAnimationParent (this.m_ReelController.GetAnimationRender (this.ReelIndex, i));
				this.m_ReelController.SetSymbolRenderMap (this.ReelIndex, i, SymbolRenders [i] as BaseElementPanel);
			}

		}
		this.RecordSymbolsState ();
	}

	protected override void DeltaMoveWheel ()
	{
		Debug.Log("DropReel DeltaMoveWheel");
		if (IsAllDrop) {
			if (delayTimeList.Count == 0) {
				return;
			}
			for (int i = 0; i < this.SymbolRenders.Count; i++) {
				float speed = 1f;
				speed += SPEED_MULTIPLE * i;

                float preTime = delayTimeList[i];

                delayTimeList [i] += Time.deltaTime * m_RunData.RunTimeScale * speed; // fast的时候会变快，时间乘以大于1的数
				//代表还未开始
				if (delayTimeList [i] < 0) {
					return;
				}

				float x = delayTimeList [i];// / RunTime;
				float s = m_Curve.Evaluate (x) * 1000f;

				this.SymbolRenders [i].MoveDownOffset (s);

				if (i == this.SymbolRenders.Count - 1 && x >= m_NormalSpinTime) {
					ReelStopHandler (false);
				}

                if (x >= this.mBounceTime && preTime < this.mBounceTime)
                {
                    Libs.AudioEntity.Instance.PlayEffect("symbols_drop");
                }
            }
		} else {
			if (delayTimeList.Count == 0) {
				return;
			}

			for (int i = 0; i < this.SymbolRenders.Count; i++) {
				if (!this.m_DropData [i].NeedDrop()) {
					continue;
				}

				float speed = 1f;

                float preTime = delayTimeList[i];
                delayTimeList [i] += Time.deltaTime;// * m_RunData.RunTimeScale * speed; // fast的时候会变快，时间乘以大于1的数

				//代表还未开始
				if (delayTimeList [i] < 0) {
					continue;
				}
				int id = this.m_DropData[i].DropLength();
				id -= 1;

				float x = delayTimeList [i];// / RunTime;
				float s = this.DropDownCurves [id].Evaluate (x) * 1000f;

				this.SymbolRenders [i].MoveDownOffset (s);

				if (i == this.SymbolRenders.Count - 1 && x >= m_NormalSpinTime) {
					ReelStopHandler (false);
					//Libs.AudioEntity.Instance.PlayEffect("symbols_drop");
				}

                if (x >= this.mBounceTime && preTime < this.mBounceTime)
                {
                    Libs.AudioEntity.Instance.PlayEffect("symbols_drop");
                }
            }
		}
	}

	//去掉急停
	protected override bool NeedQuickStop()
	{
		return false;
	}

	private void CheckEmptyDropDict(int _positionId)
	{
		if (!this.m_DropData.ContainsKey (_positionId)) {
			this.m_DropData[_positionId] = new DropMoveData (_positionId);
		}
	}

	class DropMoveData
	{
		public int DropFromPosition;//下落的开始点
		public int DropToPosition;//下落到的点

		public DropMoveData(int _position)
		{
			DropFromPosition = DropToPosition = _position;
		}
		public bool NeedDrop()
		{
			return DropFromPosition > DropToPosition;
		}

		public int DropLength()
		{
			return DropFromPosition - DropToPosition;
		}
	}
}
