using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Classic;

/* 
 * 所有的轮子都由pm配置完善，包括quick stop的。curve的x为实际的时间，y坐标为实际的像素值/1000
 * 
 * 下面的已经抛弃
//插值计算方法，目标是保证在第二帧和第三帧中间匀速阶段，根据路径的相差来计算点，确保匀速外的时间和路径是一样的。
//1.先计算之后轮子的运动时间。方法是计算匀速时的速度，多出来的路径除以速度，乘以且加上第一个轮子时间
//2.根据时间计算匀速前的一帧时间和路径。方法是第一个轮子的当前帧时间 * （第一个轮子的总时间/当前轮子总时间）
//3.计算匀速之后的帧的时间和路径。方法如2，但不同的是用2来计算之后的值，然后1-此值。
*/


public class Reel : MonoBehaviour,IReelAdapter {
	protected int ReelIndex;

	protected bool IsLastReel = false;
	
	[HideInInspector]
	public List<SymbolRender> SymbolRenders= new List<SymbolRender>(); //所有参与转动的symbolRender 数组
	protected AnimationCurve m_Curve;

	protected int ReelSymbolNum = 4;//单个轮子需计算的个数,一般为ReelShowNum + 1 +2的倍数。将来需要扩展
	protected int ReelShowNum=3;//，单个轮子显示的个数

	//当前run的普通、quickstop时间,总路径
	protected float m_NormalSpinTime, m_QuickStopTime,m_TotalLength;

	//当前reel走动的时间,用来计算run的时候的路径
	protected float m_SpinRunningTime = 0f;
	//轴开始减速的时间节点
	protected float SymbolsSlowDownTime = 0f;
	//用来判断是否触发过减速时间点的函数回调
	protected bool hasSlowDownHanele = false;

	//当前轮子转动的索引
//	private int m_RunningReelIndex = 0;

	//当前轮子的转动状态
	protected SpinStateEnum m_ReelState;

	public ReelRunData m_RunData;

	//是否已经播放了anti
	protected bool m_hasPlayAnticipation;

	//当前symbol在带子上的索引
	protected int m_DataIndex = 0; //TODO: 考虑是否换带子的时候重置此值 

	//最后停留界面的索引
	[Obsolete]
	protected int m_LastStayScreenIndex = 0;

	//reel对应的controller
	protected ReelController m_ReelController;

	protected BoardConfigs m_boardConfig;

	//最后回弹的时候key位于倒数第几个关键帧
	protected int LastBounceKeyNum = 1;

	//是否处理了回弹逻辑
	protected bool HasProcessBounceHandler =false;

	//是否处理了播放reelstop逻辑
	protected bool HasProcessReelStopSound =false;

    //最后一页的逻辑处理
    protected bool HasProcessLastReelDistance = false;
	//回弹时走动的时间，用来获取是否到达了回弹的地方
	protected float mBounceTime;

	protected float minY,maxY;

	protected float animationRenderY = 0;

	//静止布局时的索引
	public int StaticReelIndex{
		get{
			return this.ReelIndex;
		}
	}

    //最后停留的时候renders 最开始的索引
    protected int StopIndexInRenders, LastStopIndexInRenders;
    //最后停留需处理的id
    protected List<int> m_FinalBoardIdsOneFrame = new List<int>();
    //顺序order
    protected int LayerOrderZ = 0;
    //两个symbol之间的间距
    protected float DistanceTwoSymbols = 0;

    protected bool IsDropReel { set; get; } = false;

    #region init
    public virtual void LayOut(ReelController _controller, BoardConfigs _boardConfig,int _currentReelIndex,List<int>  spinData)
    {
		this.m_ReelController = _controller;

		m_boardConfig = _boardConfig;

		this.ReelIndex = _currentReelIndex;
		m_DataIndex = m_boardConfig.ReelConfigs[this.ReelIndex].BeginPosition;
//		1. symbol的坐标初始化
		SymbolRenders.Clear();

		this.SpinData = spinData;

		ReelSymbolNum = _boardConfig.ReelConfigs[_currentReelIndex].ComputeSymbolNum;
		ReelShowNum = _boardConfig.ReelConfigs [_currentReelIndex].ReelShowNum;

		float yCoordinate = this.RenderInitialBotttomPosition();

		this.InitMaxMinPosition();

		if (!_controller.SymbolRenderDic.ContainsKey (_currentReelIndex)) {
			
		}
        Vector2 sizeV = new Vector2(_boardConfig.ReelWidth, _boardConfig.SymbolHeight * this.m_boardConfig.ReelConfigs[ReelIndex].RenderMultiple);
        DistanceTwoSymbols = this.SymbolRenderDistance(sizeV.y);
        for (int i = 0; i < ReelSymbolNum; i++) {
			int _currentSymbolWeightData = SymbolInfoByTurns ();

			GameObject o = Instantiate (_boardConfig.SymbolRender, this.transform, false);
			o.name = "symbol" + i;


            RectTransform rt = o.GetComponent<RectTransform>() ;
			
			rt.sizeDelta = sizeV;//.ReelConfigs [i].ReelHeight);
			//中心点坐标位置的获取方法为：先计算reel的最下端，然后加上当前symbol的总高度/2。 为方便计算下一个的坐标，需要重新加上当前symbol的总高度/2

			float symbolY = yCoordinate;
//			if (_currentSymbolConfig.HeightMultiple != 1) {
//				symbolY  += _boardConfig.SymbolHeight / _currentSymbolConfig.HeightMultiple;
//			}

			symbolY += rt.sizeDelta.y / 2;
			//坐标计算方法为
			Vector3 v = new Vector3(0f,symbolY,LayerOrderZ);

			if (this.m_boardConfig.IsUpOverDown) {
				LayerOrderZ--;
			} else {
				LayerOrderZ++;
			}

			rt.localPosition = v;
			// rt.SetParent(this.transform, false);
			//动画层级的设置,最后一个不用添加了。但需要用ReelShowNum显示的个数来计算
			if (i < ReelShowNum && !this.m_boardConfig.IsReserveAnimation) {
				this.m_ReelController.AddAnimationRender (this.ReelIndex, i, v, sizeV,this.m_boardConfig.IsUpOverDown);
			}

            yCoordinate += DistanceTwoSymbols;

            SymbolRender render = o.GetComponent<SymbolRender> ();
			render.ControllerAdapter = _controller.RunAdapter;
			render.ReelAdapter = this;

            //注意： 要先设置静态位置，再changeSymbol
            render.SetStaticPosition(i);

            render.InitSymbolHandler();
            //映射的实现，方便初始化之后一些功能的实现，初始化基本属性，索引和symbolParent
            this.m_ReelController.SetSymbolRenderMap(this.ReelIndex, i, render as BaseElementPanel);
            //ReelShowNum显示的个数来计算父节点
            if (i < ReelShowNum)
            {
                render.SetAnimationParent(this.m_ReelController.GetAnimationRender(this.ReelIndex, i));
            }

            render.ChangeSymbol (_currentSymbolWeightData);

			//记录每个初始化的位置，方便quickStop
			render.InitializePosition();
			this.SymbolRenders.Add (render);
		}

		//2. init state状态
		m_ReelState = SpinStateEnum.Stop;
		m_QuickStopTime = m_ReelController.QuickStopCurve.keys [m_ReelController.QuickStopCurve.keys.Length - 1].time;

		//3.size大小并记录symbol的状态
		RecordSymbolsState ();
    }
	#endregion

	protected virtual void InitMaxMinPosition()
	{
        float DisplayLength = ReelShowNum * this.m_boardConfig.SymbolHeight * this.m_boardConfig.ReelConfigs[ReelIndex].RenderMultiple;
        float halfHeight = DisplayLength / ReelShowNum / 2;

        //重点：（2*n+1）
        //20190710，考虑到多整出来的symbol，优化逻辑。ReelSymbolNum基本上都是比ReelShowNum多（2*n+1）个，除了respin和drop之外。而respin的移动用不着miny和maxy。
        minY = -DisplayLength / 2 - halfHeight * (ReelSymbolNum - ReelShowNum);
        maxY = DisplayLength / 2 + halfHeight * (ReelSymbolNum - ReelShowNum);
        // minY = -DisplayLength / 2 -halfHeight;
        // maxY = DisplayLength/2 + halfHeight *(2*(ReelSymbolNum-ReelShowNum)-1) ; //考虑多余1个随机的情况

    }

    protected virtual float RenderInitialBotttomPosition()
	{
		return -m_boardConfig.ReelShowHeight(this.ReelIndex)/2;
	}

	protected virtual float SymbolRenderDistance(float height) //相邻symbolRender间隔
	{
		return height;
	}

	#region spin running
	private void ResetSymbolStaticPosition()
	{
		for (int i = 0; i < this.SymbolRenders.Count; i++) {
			this.SymbolRenders [i].SetStaticPosition (-1);
		}
	}

	//记录run之前symbol的位置
	protected virtual void RecordSymbolsState()
	{
		SymbolRenders.ForEach (delegate(SymbolRender obj) {
			obj.RecordPosition (minY , maxY);	
			obj.NeedMove = true;
		});
	}

	public virtual void DoUpdate()
	{
		if (m_ReelState == SpinStateEnum.Stop) {
			return;
		}

		if (m_ReelState == SpinStateEnum.NormalRun) {
			DeltaMoveWheel ();
		} else if (m_ReelState == SpinStateEnum.QuickStop) {
			DoQuickStop ();
		}
		else if(m_ReelState == SpinStateEnum.Gliding)
		{
			GlidingDeltaMove();
		}

	}
	//初始化数据
	public virtual void StartSpin(ReelRunData _data)
	{
		this.m_RunData = _data;
		this.SpinData = _data.ReelSpinData;
		m_hasPlayAnticipation = false;
		this.m_Curve = _data.ReelCurve;
		this.IsLastReel = _data.IsLastReel;

		m_NormalSpinTime = this.m_Curve.keys [this.m_Curve.keys.Length - 1].time;

		m_TotalLength = this.m_Curve.keys [this.m_Curve.keys.Length - 1].value * 1000f;
		//fast的路径变短，时间变快

		if (!Mathf.Approximately(_data.RunLengthScale ,1f)) {
			m_TotalLength *= _data.RunLengthScale;
		}

		m_SpinRunningTime = 0f;
		
		SymbolsSlowDownTime = 0f;

		hasSlowDownHanele = false;
		
		SymbolsSlowDownTime = m_Curve.keys[m_Curve.keys.Length - 3].time;

		m_LastStayScreenIndex = 0;

		m_ReelState = SpinStateEnum.NormalRun;

		animationRenderY = 0;

		RecordSymbolsState ();

	

		//回弹的距离，需要判断是否为anti,anti则在最后处理，mBounceLength为总长度
		if (!m_RunData.IsAnticipation && (this.m_Curve.keys.Length - 1 - LastBounceKeyNum)>=0) {
			this.mBounceTime = this.m_Curve.keys [this.m_Curve.keys.Length - 1 - LastBounceKeyNum].time  ;
		}
		//重置是否bounce回调的变量
		HasProcessBounceHandler = false;

		HasProcessReelStopSound = false;

        HasProcessLastReelDistance = false;

        LayerOrderZ = 0;

        LastStopIndexInRenders = StopIndexInRenders;//目前用在后端化的索引位置 
        //总长度除以单个symbol高度，然后加上原始数据，除以余数
        StopIndexInRenders = (StopIndexInRenders + (int) Mathf.Round(m_TotalLength / DistanceTwoSymbols))% ReelSymbolNum;
        
        //清除最终的映射关系，在小symbol的respin处理上则不需要清除
        if (!this.m_boardConfig.IsSmallMode && !this.IsDropReel) {
	        this.m_ReelController.ClearSymolRenderMap (this.ReelIndex);
			VerifyNormalSpinState();
        }

        StartSpinHandler ();
	}

	public virtual void ResetSymbolSlowDownTime(AnimationCurve m_AnticipationCurves)
	{
		SymbolsSlowDownTime = m_AnticipationCurves.keys[m_AnticipationCurves.keys.Length - 1].time;
	}

    protected virtual void StartSpinHandler()
	{
        for (int i = 0; i < this.SymbolRenders.Count; i++)
        {
            this.SymbolRenders[i].StartSpinHandler();
        }
    }

    protected virtual void DeltaMoveWheel()
	{	
        m_FinalBoardIdsOneFrame.Clear();
        float x = 0f;
        float s = 0f;
        if (CheckAnimationCurve(out x,out s)) {
	        for (int i = 0; i < SymbolRenders.Count; i++) {

				if (SymbolRenders [i].MoveDistance (s, ref LayerOrderZ)) {
					//小small的respin形式
					if (this.m_boardConfig.IsSmallMode) {
						//最后一个symbol位置要停止即可
						if (this.m_TotalLength - s > this.m_boardConfig.SymbolHeight)
						{
							SymbolRenders[i].SymbolChangeState = SymbolChangeState.Running;
						}
						else
						{
							if (SymbolRenders[i].SymbolChangeState != SymbolChangeState.ReelStop && m_RunData.ResultSymbols!=null)
							{
								SymbolRenders[i].ChangeSymbol(m_RunData.ResultSymbols[SymbolRenders[i].PositionId], SymbolChangeState.ReelStop);
							}
						}
					} else {
						//判断是否到了最后一个界面，方法为判断最后的距离是否小于距离,掉落和respin 不会走这里，所以计算长度会ok
						if (this.IsRunStop(s) )
						{
                            m_FinalBoardIdsOneFrame.Add(i);
						} else {
							int tmpSymbolId = SymbolInfoByTurns ();
							SymbolRenders [i].ChangeSymbol (tmpSymbolId, SymbolChangeState.Running);//m_ReelController.GetRandomSymbolConfig ());
                            SymbolRenders[i].SetOrderZ(LayerOrderZ);
						}

						//设置symbol的层级关系,First为上压下，配置到boardConfig中
						if (this.m_boardConfig.IsUpOverDown) {
							LayerOrderZ--;
						} else {
							LayerOrderZ++;
						}
					}
				}
             
            }
	        
	        if (m_SpinRunningTime >= SymbolsSlowDownTime && !hasSlowDownHanele) {
				hasSlowDownHanele = true;
				m_ReelController.SymbolSlowDownCallback(this.ReelIndex);
	        }
	        
			FinalBoardSetRenders();

			if (!HasProcessReelStopSound)
			{
				if (!this.m_RunData.IsAnticipation && m_SpinRunningTime >= this.mBounceTime -0.1f ) {
					HasProcessReelStopSound = true;
					m_ReelController.PlayReelStopEffect();
				}
			}
			
			//回弹的回调， 在anti不处理
            if (!HasProcessBounceHandler) {
                if (!this.m_RunData.IsAnticipation && m_SpinRunningTime >= this.mBounceTime  ) {
                    HasProcessBounceHandler = true;
                    m_ReelController.ReelBounceBackHandler (this.ReelIndex,false);
                }
            }

			//play aniticipation
			if (this.m_RunData.IsAnticipation && !m_hasPlayAnticipation && this.m_SpinRunningTime >= this.m_RunData.AntiStartTime) 
			{
				m_hasPlayAnticipation = true;
				//TODO: 做出事件分发
				m_ReelController.ShowAnticipation (this.ReelIndex);
			}
	

            if(HasProcessBounceHandler)
            {
                m_ReelController.ChangeSmartAnimationPosition(this.ReelIndex, this.m_TotalLength - s - animationRenderY);
				animationRenderY = this.m_TotalLength - s;
            }
		} else
        {
	        VerifyStopSymbolRender();
	        
			if (m_RunData.IsAnticipation) {
                //anti的停止状态跟回弹时一样的逻辑
				this.m_ReelController.ReelBounceBackHandler(this.ReelIndex);
			}

			ReelStopHandler (false);
		}

    }


    protected virtual bool CheckAnimationCurve(out float x,out float s)
    {
	    x = s = 0f;
	    bool canRun = m_SpinRunningTime < m_NormalSpinTime;
	    if (canRun)
	    {
//			一直在匀速阶段
		    m_SpinRunningTime += GetDeltaTime() * m_RunData.RunTimeScale; // fast的时候会变快，时间乘以大于1的数
		    if (m_ReelController.EnableNetWork && !m_ReelController.CanNetStop && m_SpinRunningTime>=m_ReelController.NetStopTime)
		    {
			    m_SpinRunningTime -= m_ReelController.OneReelOffsetTime;
			    float offset = m_Curve.Evaluate(m_SpinRunningTime);
			    for (int i = 0; i < this.SymbolRenders.Count; i++)
			    {
				    this.SymbolRenders[i].ResetPageIndex(offset*1000);
			    }
		    }
		    x = m_SpinRunningTime; // / RunTime;
		    s = m_Curve.Evaluate(x) * 1000f; //* RealLength * RunMultiple;
	    }

	    return canRun;
    }
    /**
    //防止 出现2个及以上的停留情况，最后和第一个的顺序会反
    private void DealStoppingRenders()
    {
        //处理两个的情况，三个及以上暂不考虑
        if(m_RenderIdsOneFrame.Count == 2)
        {
            if(m_RenderIdsOneFrame[0] == 0 && m_RenderIdsOneFrame[1]== this.SymbolRenders.Count-1 )
            {
                m_RenderIdsOneFrame[0] = this.SymbolRenders.Count - 1;
                m_RenderIdsOneFrame[1] = 0;
            }
        }
        for(int k = 0; k <m_RenderIdsOneFrame.Count;k++)
        {
            int i = m_RenderIdsOneFrame[k];
            //超出最后一个以及第一个的情况
            if(m_LastStayScreenIndex >= m_RunData.ResultSymbols.Count)
            {
                ResultContent.WeightData _currentSymbolWeightData = SymbolInfoByTurns();
                SymbolRenders[i].ChangeSymbol(_currentSymbolWeightData.value, SymbolChangeState.Running);
                continue;
            }
            else if (m_LastStayScreenIndex == 0)
            {
                if(StopIndexInRenders != i)
                {
                    ResultContent.WeightData _currentSymbolWeightData = SymbolInfoByTurns();
                    SymbolRenders[i].ChangeSymbol(_currentSymbolWeightData.value, SymbolChangeState.Running);
                    continue;
                }
            }

            int symbolIndex = m_RunData.ResultSymbols[m_LastStayScreenIndex];
//            //在ChangeSymbol之前先SetStaticPosition，因为有的机器在changeSymbol时会有额外逻辑需要PositionID
//            SymbolRenders[i].SetStaticPosition(m_LastStayScreenIndex);
//            //设置静止时的索引，同时设置map映射信息
//            SymbolRenders[i].SetAnimationParent(this.m_ReelController.GetAnimationRender(this.ReelIndex, m_LastStayScreenIndex));
//            this.m_ReelController.SetSymbolRenderMap(this.ReelIndex, m_LastStayScreenIndex, SymbolRenders[i] as BaseElementPanel);

            SymbolRenders[i].ChangeSymbol(symbolIndex, SymbolChangeState.ReelStop);//处理转动和停止时的其他逻辑

            if (m_LastStayScreenIndex == 0)
            {
                StopIndexInRenders = i;
            }
            m_LastStayScreenIndex++;

            if (!HasProcessLastReelDistance)
            {
                HasProcessLastReelDistance = true;
                m_ReelController.DoLastReelDistanceHandler(this.ReelIndex, SymbolRenders[i]);
            }

        }
    }
	**/
    /// <summary>
    /// 多出来的长度
    private float ExtraDistance()
    {
        //return 0f;
         int NumMore = Mathf.CeilToInt((float)(ReelSymbolNum - ReelShowNum) / 2f); //上下多出的symbol个数
        float distance = (NumMore -1) * this.m_boardConfig.SymbolHeight * this.m_boardConfig.ReelConfigs[this.ReelIndex].RenderMultiple;

        return distance;
    }

    /// <summary>blank的时候需要合并处理
    /// 但为了兼顾多出多个未显示symbol的处理，num为单侧多显示的symbol个数， (num-1) * h 为多出来的symbol计算的长度
    /// </summary>
    /// <returns>The page distance.</returns>
    private float LastPageDistance()
    {
        return ExtraDistance()+(ReelShowNum ) * this.m_boardConfig.SymbolHeight * this.m_boardConfig.ReelConfigs[this.ReelIndex].RenderMultiple;
    }

    public SpinStateEnum GetReelCurState()
    {
	    return m_ReelState;
    }

    protected virtual void ReelStopHandler(bool isFastStop)
	{
		m_hasPlayAnticipation = false;

		if (this.m_RunData.IsAnticipation) {
			m_ReelController.HideAnticipation (this.ReelIndex);
		}
		m_SpinRunningTime = 0f;
		m_ReelState = SpinStateEnum.Stop;
		//每个轮子停下来的回调
		m_ReelController.EachReelStopHandler(this.ReelIndex,isFastStop);
	
		// if (this.IsLastReel) {
		// 	m_ReelController.AllReelsEnd (isFastStop);
		// }
	}
	#endregion
	
	
	#region normal情况下map以及停止处理
	/// <summary>
	/// 校验映射结果，在初始化的时候调用下。另尽量在bounce的时候，有anti的bounce在所有轮子停止之后才调用
	/// </summary>
	protected void VerifyNormalSpinState()
	{
		for (int y = 0; y < ReelShowNum; y++)
		{
			int lastResultRenderIndex = (StopIndexInRenders + y ) % SymbolRenders.Count;
			this.SetStopInfo(lastResultRenderIndex ,y);
//			//在ChangeSymbol之前先SetStaticPosition，因为有的机器在changeSymbol时会有额外逻辑需要PositionID
//			SymbolRenders[lastResultIndex].SetStaticPosition(y);
//			//设置静止时的索引，同时设置map映射信息
//			SymbolRenders[lastResultIndex].SetAnimationParent(this.m_ReelController.GetAnimationRender(this.ReelIndex, y));
//			this.m_ReelController.SetSymbolRenderMap(this.ReelIndex, y, SymbolRenders[lastResultIndex] as BaseElementPanel);
		}
	}

	protected void SetStopInfo(int index, int positionY)
	{
		//在ChangeSymbol之前先SetStaticPosition，因为有的机器在changeSymbol时会有额外逻辑需要PositionID
		SymbolRenders[index].SetStaticPosition(positionY);
		//设置静止时的索引，同时设置map映射信息
		SymbolRenders[index].SetAnimationParent(this.m_ReelController.GetAnimationRender(this.ReelIndex, positionY));
		this.m_ReelController.SetSymbolRenderMap(this.ReelIndex, positionY, SymbolRenders[index] as BaseElementPanel);
	}

	/// <summary>
	/// 最后一个页面处理结果
	/// </summary>
	protected virtual void FinalBoardSetRenders()
	{
		if (m_FinalBoardIdsOneFrame.Count < 0)
			return;
		
		for (int k = 0; k < m_FinalBoardIdsOneFrame.Count; k++)
		{
			//symbolRenders 中的索引
			int index = m_FinalBoardIdsOneFrame[k];
//			最终停留的时候在棋盘上的position
			int positionY = (index - this.StopIndexInRenders + SymbolRenders.Count) % SymbolRenders.Count;
			//在屏幕内的用最终结果，在外边的用随机数据，将来也可以扩展
			
//			Log.LogWhiteColor($"needDeal:{this.ReelIndex}--{positionY}--数组索引：{index}");
			if (positionY >= ReelShowNum)
			{
				int tmpSymbolId = SymbolInfoByTurns();
				SymbolRenders[index]?.ChangeSymbol(tmpSymbolId, SymbolChangeState.Running);
			}
			else
			{
				if (SymbolRenders[index]?.SymbolChangeState != SymbolChangeState.ReelStop)
				{
//					Log.LogWhiteColor($"2--needDeal:{this.ReelIndex}--{positionY}--数组索引：{index}");
					if (m_RunData.ResultSymbols.Count>positionY)
					{
						int symbolIndex = m_RunData.ResultSymbols[positionY]; 
						SymbolRenders[index]?.ChangeSymbol(symbolIndex, SymbolChangeState.ReelStop);//处理转动和停止时的其他逻辑
					}
				}
			}
			
			if (!HasProcessLastReelDistance)
			{
				HasProcessLastReelDistance = true;
				if (SymbolRenders[index]!=null)
				{
					m_ReelController.DoLastReelDistanceHandler(this.ReelIndex, SymbolRenders[index]);
				}
			}
		}
		
	}
	#region 正常转动轮子停止矫正
///TODO	停止针对有些状态不对的symbolRender 的优化处理，暂时留着。
	protected void VerifyStopSymbolRender()
	{
		for (int y = 0; y < ReelShowNum; y++)
		{
			int _index =   (StopIndexInRenders + y ) % SymbolRenders.Count;

			if (_index < 0 || _index >= SymbolRenders.Count) continue;
			
			SymbolRender _render = SymbolRenders[_index];
			

			if (_render.SymbolChangeState != SymbolChangeState.ReelStop)
			{
				Log.LogLimeColor($"重新校验 轮子id：{this.ReelIndex} 实际位置：{y}--数组索引：{_index}，");
				if (m_RunData.ResultSymbols!=null && m_RunData.ResultSymbols.Count > 0 && y < m_RunData.ResultSymbols.Count)
				{
					int _symbolIndex = m_RunData.ResultSymbols[y];
					_render.ChangeSymbol(_symbolIndex, SymbolChangeState.ReelStop);
				}
			}
		}
	}

	#endregion
	#endregion

	#region quick stop
	/// <summary>
	/// Needs the quick stop. 判断是否需要急停，目前根据时间点来计算
	/// </summary>
	/// <returns><c>true</c>, if quick stop was needed, <c>false</c> otherwise.</returns>
	protected virtual bool NeedQuickStop()
	{
		if (m_ReelState == SpinStateEnum.NormalRun) {
			//			if (m_Curve.Evaluate(this.m_NormalSpinTime) - m_Curve.Evaluate(this.m_SpinRunningTime) 
			//			    > m_ReelController.QuickStopCurve[m_ReelController.QuickStopCurve.length-2].value)
			if (this.m_NormalSpinTime - this.m_SpinRunningTime > m_ReelController.QuickStopCurve[m_ReelController.QuickStopCurve.length-1].time) 
			{
				return true;
			}
		}
		return false;
	}
	public virtual void ClickQuickStopSpin()
	{
		if (NeedQuickStop ()) {
			m_ReelController.SpinState = SpinStateEnum.QuickStop;
			m_ReelState = SpinStateEnum.QuickStop;
            Messenger.Broadcast<int>(BoardConstant.START_QUICK_STOP_HANDLER,ReelIndex);

            m_SpinRunningTime = GetDeltaTime() *(m_ReelController.GetRunReels().Count - ReelIndex-1);
			m_SpinRunningTime = Mathf.Min (Mathf.Max (0, m_SpinRunningTime), m_QuickStopTime); //限制在范围内，避免出现


			//处理停止的面板symbol
			for (int i = 0; i < SymbolRenders.Count; i++) {
				if(m_RunData?.ResultSymbols == null) continue;
                if (i < m_RunData.ResultSymbols.Count)
                {
                    int symbolIndex = m_RunData.ResultSymbols[i];

                    //注意： 要先设置静态位置，再changeSymbol
                    if (!this.m_boardConfig.IsSmallMode)
                    {
                        //设置静止时的索引，同时设置map映射信息

                        SetStopInfo(i, i);
                        
//                        SymbolRenders[i].SetStaticPosition(i);
//                        SymbolRenders[i].SetAnimationParent(this.m_ReelController.GetAnimationRender(this.ReelIndex, i));
//
//                        this.m_ReelController.SetSymbolRenderMap(this.ReelIndex, i, SymbolRenders[i] as BaseElementPanel);
                    }
                    SymbolRenders[i].ChangeSymbol(symbolIndex, SymbolChangeState.ReelStop);
                }

                //设置symbol的层级关系,First为上压下，配置到boardConfig中
                if (this.m_boardConfig.IsUpOverDown)
                {
                    LayerOrderZ--;
                }
                else
                {
                    LayerOrderZ++;
                }
                SymbolRenders[i].SetOrderZ(LayerOrderZ);

            }

            StopIndexInRenders = 0; //急停stop的时候为0

            m_ReelController.ReelBounceBackHandler(this.ReelIndex);
            m_ReelController.isQuickStop = true;
		}

    }

	void  DoQuickStop()
	{
		if (m_SpinRunningTime <= m_QuickStopTime) {
//			Debug.Log (this.ReelIndex + ":" + _SpinRunningTime);
			m_SpinRunningTime += GetDeltaTime();
			
			float timeX = m_SpinRunningTime;// / m_ReelController.QuickStopAllTime;

			//倒着计算总的距离,先从最远的距离回来

			float deltaS = m_ReelController.QuickStopCurve.keys [m_ReelController.QuickStopCurve.keys.Length - 1].value * 1000 - m_ReelController.QuickStopCurve.Evaluate (timeX) * 1000;
			//float s = this.m_Curve.keys [this.m_Curve.keys.Length - 1].value * 1000 - deltaS;// (1-m_ReelController.QuickStopCurve.Evaluate (timeX)) * m_ReelController.QuicikStopRunLength * RealLength;
			for (int i = 0; i < SymbolRenders.Count; i++) {

				SymbolRenders [i].QuickStopMoveDistance (deltaS);
			}


//			yield return 0;
		} else {
//		CurrentReelIsSpinning = false;
			ReelStopHandler (true);
		}
//		yield return null;
	}

	protected virtual bool IsRunStop(float s)
	{
		return   (this.m_TotalLength - s) <= LastPageDistance();
//        return this.m_TotalLength - s > ExtraDistance() && this.m_TotalLength - s < LastPageDistance();
	}
	#endregion

	#region data 处理

	public List<int> SpinData;
	//转动每次相加1时的数据
	public virtual int SymbolInfoByTurns()
	{
		m_DataIndex = Mathf.Max(0, Mathf.Min (m_DataIndex, SpinData.Count - 1));
		int weightData= SpinData[m_DataIndex];
		m_DataIndex = (m_DataIndex + 1) % SpinData.Count;

		return weightData;
	}
	
	#endregion

	#region 接口
	public int GetReelShowNumber()
	{
		return this.ReelShowNum;
	}
	public int GetReelIndex()
	{
		return this.StaticReelIndex;
	}

	public BoardConfigs GetBoardConfig()
	{
		return this.m_boardConfig;
	}
	#endregion

	public void SetUnScaleTimeMode(bool unscale)
	{
		this.unscaleTimeMode = unscale;
	}
	private bool unscaleTimeMode = false;
	public virtual float GetDeltaTime()
	{
		if (unscaleTimeMode)
		{
			return Time.unscaledDeltaTime;
		}

		return Time.deltaTime;
	}

	#region gliding移动固定的symbol个数
	private AnimationCurve m_GlidingCurve = AnimationCurve.EaseInOut(0,0,1,1);
	private float m_currentGlidingTime =0f;
	private float m_TotalGlidingTime=.5f;
	private Action m_GlidingEndCallback;
	
    private bool m_IsUp;
	private int m_moveLength;
	public void GlidingByLength(bool _isUp,int _moveLength, System.Action callback = null,float TotalGlidingTime = .5f)
	{
        this.m_IsUp = _isUp;
		m_currentGlidingTime = 0f;
		m_ReelState = SpinStateEnum.Gliding;
		m_moveLength = _moveLength;
		m_GlidingEndCallback = callback;
        m_TotalGlidingTime = TotalGlidingTime;// 0.25f *_moveLength; 临时为1s，有需要的时候再去处理

		this.RecordSymbolsState();
        this.m_ReelController.ClearSymolRenderMap(this.ReelIndex);
    }

	protected virtual void  GlidingDeltaMove()
	{
		if(m_currentGlidingTime >= m_TotalGlidingTime)
		{
			GldingByPercent(1f);
		
			m_ReelState = SpinStateEnum.Stop;
            //TODO
            m_LastStayScreenIndex = 0;
            if (this.m_IsUp)
            {
                StopIndexInRenders = (StopIndexInRenders - m_moveLength + SymbolRenders.Count) % SymbolRenders.Count;
            }
            else
            {
                StopIndexInRenders = (StopIndexInRenders + m_moveLength + SymbolRenders.Count) % SymbolRenders.Count;
            }

            for (int i =0; i < ReelShowNum;i++)
            {
                int index = 0;
                if (this.m_IsUp)
                {
                    index = (StopIndexInRenders + i) % SymbolRenders.Count;
                }
                else
                {
                    index = (StopIndexInRenders + i) % SymbolRenders.Count;
                }
                
                this.SetStopInfo(index,i);
//                SymbolRenders[index].SetStaticPosition(m_LastStayScreenIndex);
//                SymbolRenders[index].SetAnimationParent(this.m_ReelController.GetAnimationRender(this.ReelIndex, m_LastStayScreenIndex));
//                this.m_ReelController.SetSymbolRenderMap(this.ReelIndex, m_LastStayScreenIndex, SymbolRenders[index] as BaseElementPanel);
                m_LastStayScreenIndex++;
            }

            if (m_GlidingEndCallback != null)
            {
                m_GlidingEndCallback.Invoke();
            }

            //SymbolRenders[i].SetStaticPosition(m_LastStayScreenIndex);
            ////设置静止时的索引，同时设置map映射信息
            //SymbolRenders[i].SetAnimationParent(this.m_ReelController.GetAnimationRender(this.ReelIndex, m_LastStayScreenIndex));
            //this.m_ReelController.SetSymbolRenderMap(this.ReelIndex, m_LastStayScreenIndex, SymbolRenders[i] as BaseElementPanel);
        }
		else
			{
				m_currentGlidingTime += GetDeltaTime();
				float v = m_currentGlidingTime/m_TotalGlidingTime;
				GldingByPercent(v);
			}
	}

	private void GldingByPercent(float v)
	{
		float distance = m_GlidingCurve.Evaluate(v) * m_moveLength * this.m_boardConfig.SymbolHeight *  this.m_boardConfig.ReelConfigs[ReelIndex].RenderMultiple ;
		for (int i = 0; i < SymbolRenders.Count; i++) {
            if(this.m_IsUp)
            {
                SymbolRenders[i].MoveUpDistance(distance, ref LayerOrderZ);
            }
            else
            {
                SymbolRenders[i].MoveDistance(distance, ref LayerOrderZ);
            }
		}
	}

    //设置底部的未显示的symbol，一般用在gliding中
	public void SetBelowNotShowSymbol(int preNum,int symbolIndex)
    {
        int index = (this.StopIndexInRenders - preNum + SymbolRenders.Count) % SymbolRenders.Count;

        SymbolRenders[index].ChangeSymbol(symbolIndex,SymbolChangeState.ReelStop);
    }
    //设置头部的未显示的symbol，一般用在gliding中，需要减一，因为加了ReelShowNum
    public void SetTopNotShowSymbol(int afterNum, int symbolIndex)
    {
        int index = (this.StopIndexInRenders + afterNum + this.ReelShowNum-1) % SymbolRenders.Count;

        SymbolRenders[index].ChangeSymbol(symbolIndex, SymbolChangeState.ReelStop);
    }

    #endregion

    #region server Data
	public AnimationCurve Curve
	{
		get => m_Curve;
	}
    public void SetServerRetData(List<int> symbolResult)
    {
	    this.m_RunData.ResultSymbols = symbolResult;
    }

    public virtual void SetAntiData(bool isAnti,AnimationCurve curve)
    {
	    this.m_Curve = curve;
	    
	    //回弹的距离，需要判断是否为anti,anti则在最后处理，mBounceLength为总长度
	    if (!isAnti) {
		    this.mBounceTime = this.m_Curve.keys [this.m_Curve.keys.Length - 1 - LastBounceKeyNum].time;
	    }

	    this.m_RunData.IsAnticipation = isAnti;
	    this.m_RunData.AntiStartTime = this.Curve.keys[2].time;
	    m_NormalSpinTime = this.m_Curve.keys [this.m_Curve.keys.Length - 1].time;

	    m_TotalLength = this.m_Curve.keys [this.m_Curve.keys.Length - 1].value * 1000f;
	    
	    //总长度除以单个symbol高度，然后加上原始数据，除以余数
	    StopIndexInRenders = (LastStopIndexInRenders + (int) Mathf.Round(m_TotalLength / DistanceTwoSymbols))% ReelSymbolNum;
	    
	    //清除最终的映射关系，在小symbol的respin处理上则不需要清除
	    if (!this.m_boardConfig.IsSmallMode && !this.IsDropReel) {
		    this.m_ReelController.ClearSymolRenderMap (this.ReelIndex);
		    VerifyNormalSpinState();
	    }
    }
    #endregion

}
