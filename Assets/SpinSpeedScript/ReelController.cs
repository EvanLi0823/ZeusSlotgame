using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using Classic;
using Core;

/*TODO: 1. 接口 整合 spin之前设置anti和需要转动的轮子
 * 2. 监听来设置fast模式
 * 3.初始化的时候config的配置
 */

public enum SpinStateEnum
{
	Stop,
	NormalRun,
	Aniticipation,
	QuickStop,
	Gliding,
}
/*
 * anti的每个轮子停下来需要有回调，前面的那个轮子也要有回调
*/
[Serializable]
public struct NormalRunConfig
{
	[Header("所有带子正常的转速")]
	public AnimationCurve[] NormalRunReelCurve;
	[Header("慎用，此curve用来ab测试，需要pm知情情况下使用")]
	public AnimationCurve[] TestRunReelCurve;
}

//[RequireComponent(typeof(BoardConfigs))]
public class ReelController : MonoBehaviour {
	
	//TODO: 将来哪些轮子需要转动，在spin之前设置并赋值一下即可
    [HideInInspector]
	public List<Reel> Reels = new List<Reel>();
	[NonSerialized] public IAnticipationStrategy m_AntiLength;
	public SpinStateEnum SpinState {
		set;
		get;
	}

	
	[SerializeField]
	public NormalRunConfig m_NormalRunReelConfig;

//	public AnimationCurve[] NormalRunReelCurve;

	[SerializeField]
	[Header("急停的转速")]
	public AnimationCurve QuickStopCurve;
//	public float QuickStopAllTime = 0.2f;
//	public float QuicikStopRunLength = 0.3f;

	[Header("在normal spin时是否需要急停的时间")]
	public float QuickStopBeforeNormalTime = 0.5f ;

	[Header("anti 时额外需要转动的距离symbol个数")]
	public int m_AntiExtraSymbolNum = 40;
	[Header("anti相对于正常时间的压缩比")]
	public float m_TimeCompression = 1.2f;

	[Header("fast快速模式时的缩放时间，为原始值除以fast之后的值")]
	private float m_FastTimeScale = 1.25f;
	
	[Header("Plist中配置的转速倍数")]
	private float m_FastTimeScale_Plist = 1f;

	[Header("fast快速模式时的缩放路径，为fast之后的值除以原始值")]
	private float m_FastRunLengthScale = 1.0f;

	[Header("转动不受暂停影响")]
	public bool EnableUnscaleTimeMode = false;
	
	private AnticipationAnimations Anticipation;
	[HideInInspector]
	public bool EnableNetWork = false;
	[Header("转动带子在哪个位置时间点开始拼接，要求必须在结果排面出现之前，并且所有reel必须处在匀速阶段")]
	public float dynamicLinkTime = 1.5f;
	protected float SymbolHeight = 172f;
	
	public List<AnimationCurve> m_AnticipationCurves = new List<AnimationCurve>();

	//播放anti的轮子id
	private List<int> m_AntiReelIds = new List<int> ();

//	实际应该转动的轮子
	private List<Reel> m_RealRunReels = new List<Reel>();

    private bool m_NeedPlaySmartSound = true;

	private IRunDataChange runDataChange;

	//棋盘的配置信息
	protected BoardConfigs m_BoardConfigs ;

	//是否为fast模型
	public bool m_IsFastMode = false;

	protected IRunControllerAdapter adapter;

	//是否采用test 转速 20192113
	private bool m_IsTestSpeedPattern = false;
	// 曲线只初始化一次即可，暂时先不用
//	private bool HasInitAntiCurve = false;

	#region Spin Effect
	SmartAntiEffectController smartEffectController;
	#endregion
	
	[HideInInspector]
	public bool isQuickStop = false;
	void Awake()
	{
		Anticipation = GetComponent<AnticipationAnimations> ();
		runDataChange = GetComponent<IRunDataChange> ();


		smartEffectController = this.GetComponent<SmartAntiEffectController> ();
	}
	void Start()
	{
//		BoardConfigs boardConfigs = GetComponent<BoardConfigs> ();
//
//		LayOut(boardConfigs);

		ResetSpinReelsData ();
	}
		
	void Update()
	{
		for (int i = 0; i < this.m_RealRunReels.Count; i++) {
			this.m_RealRunReels [i].DoUpdate ();
		}
	}

	public List<Reel> GetRunReels()
	{
		return m_RealRunReels;
	}

	#region anti
	public void ShowAnticipation(int index)
	{
		if (Anticipation != null) {
			Anticipation.ShowAnimation (index);
			Libs.AudioEntity.Instance.PlayAnticipationSoundEffect ();
		}
	}

	public void HideAnticipation(int index)
	{
		if (Anticipation != null) {
			Anticipation.HideAnimation (index);
			Libs.AudioEntity.Instance.StopAnticipationSoundEffect ();
		}
	}

	public void HideAllAnti()
	{
		if (Anticipation != null) {
			Anticipation.HideAllAnimation();
		}
	}

	public bool IsContainAnticipation()
	{
		return this.m_AntiReelIds.Count > 0 && Anticipation !=null;
	}

	public bool IsAnticpationReel(int reelIndex)
	{
		return this.m_AntiReelIds.Count > 0 && this.m_AntiReelIds.Contains(reelIndex) && Anticipation != null;
	}

	/// <summary>
	/// 遍历所有的reels，从需要修改的reel开始，往后面依次修改
	/// deltaX的值比较重要，是anti的轮子多走的时间，后面的轮子都根据这个来计算时间
	/// Inits the anti.
	/// </summary>
	void InitAnticipationReelCurve()
	{
		Debug.Log("InitAnticipationReelCurve ==========");
		for (int antiIndex = 0; antiIndex < this.m_AntiReelIds.Count; antiIndex++) {
			float deltaX = 0;

			//当前需要anti的id
			int currentAntiIndex = this.m_AntiReelIds[antiIndex];

			for (int index = antiIndex; index < m_RealRunReels.Count; index++) {
//				20200814 真实的reel的curve的索引
				int realReelIndex = m_RealRunReels[index].GetReelIndex();
				AnimationCurve curve = new AnimationCurve( m_AnticipationCurves[index].keys); //new AnimationCurve (this.m_NormalRunReelConfig.NormalRunReelCurve [index].keys);

				if (realReelIndex == currentAntiIndex) {
					Keyframe[] frames = curve.keys;
					float deltaY = 0;
					if (m_AntiLength != null)
					{
						deltaY = (float)SymbolHeight / 1000 * m_AntiLength.ModifyLength(index);
					}
					else
					{
						deltaY= (float)SymbolHeight / 1000 * m_AntiExtraSymbolNum;	
					}
					float v = CaculateKeyFrameTangent (frames [1], frames [2]);

					deltaX = (deltaY / v) / m_TimeCompression; //时间➗2

					//删除最后一个（第5个），改变第四个的位置

					Keyframe lastFrame = new Keyframe (frames [2].time, frames [4].value, frames [2].inTangent, frames [2].outTangent);
					curve.RemoveKey (4);


					lastFrame.time += deltaX;
					lastFrame.value += deltaY;
					lastFrame.inTangent = CaculateKeyFrameTangent (lastFrame, frames [2]);

					curve.MoveKey (3, lastFrame);

					//倒数第二的斜率
					Keyframe lastSecFrame = frames[2];
					lastSecFrame.outTangent = CaculateKeyFrameTangent (frames [2], lastFrame);
					curve.MoveKey (2, lastSecFrame);

				} else if (realReelIndex > currentAntiIndex) {
					Keyframe[] frames = curve.keys;

					float v = CaculateKeyFrameTangent (frames [1], frames [2]);

					float deltaY = deltaX * v;

					deltaY = (Mathf.Ceil (deltaY * 1000 / SymbolHeight)) * SymbolHeight / 1000;

					float realT = deltaY / v;


					for (int i = frames.Length - 1; i >= 2; i--) {
						frames [i].value += deltaY;
						frames [i].time += realT;

						curve.MoveKey (i, frames [i]);
					}
				}
				m_AnticipationCurves [index] = curve;
				//				m_AntiCurves.Add (curve);
			}
		}
	}

	#endregion

	#region spin run
	/// <summary>
	/// 是否采用test的speed，用来做分组ab测试，config配置且TestRunReelCurve有数据时才设置为true用测试模式
	/// </summary>
	/// <param name="configValue"></param>
	public void SetTestSpeedPattern(bool configValue)
	{
		if (configValue && this.m_NormalRunReelConfig.TestRunReelCurve !=null && this.m_NormalRunReelConfig.TestRunReelCurve.Length >0)
		{
			this.m_IsTestSpeedPattern = true;
		}
		else
		{
			this.m_IsTestSpeedPattern = false;
		}
	}
	
	//参数为带子、结算数据、anti的数据，设置轮子的转动curve（如埃及艳后的curve有很多需单独区分）
	public void DoReelsSpin(List<List<int>> resultData,List<List<int>> showData,List<int> _antiReels,List<int> _reelCurves,bool _needPlaySmartSound)
	{
		if (SpinState == SpinStateEnum.Stop) {
			//初始化anti，必须放在判断anti的前面
			this.m_AntiReelIds = _antiReels;
			if (IsContainAnticipation()) {
				m_AnticipationCurves.Clear ();
				if (m_IsTestSpeedPattern)
				{
					m_AnticipationCurves = new List<AnimationCurve> (this.m_NormalRunReelConfig.TestRunReelCurve);
				}
				else
				{
					m_AnticipationCurves = new List<AnimationCurve> (this.m_NormalRunReelConfig.NormalRunReelCurve);
				}
				
				InitAnticipationReelCurve ();
			}
				
			for (int i = 0; i < m_RealRunReels.Count; i++) {
				m_RealRunReels[i].SetUnScaleTimeMode(EnableUnscaleTimeMode);
				//				AnimationCurve curve;
				ReelRunData runData;
				if (IsContainAnticipation()) {
					runData = new ReelRunData (i, IsAnticpationReel(m_RealRunReels[i].GetReelIndex()) , m_AnticipationCurves [_reelCurves[i]],i==m_RealRunReels.Count-1);
				} else
				{
					AnimationCurve speedCurve = this.m_IsTestSpeedPattern
						? m_NormalRunReelConfig.TestRunReelCurve[_reelCurves[i]]
						: m_NormalRunReelConfig.NormalRunReelCurve[_reelCurves[i]];
					runData = new ReelRunData (i,false, speedCurve,i==m_RealRunReels.Count-1);
				}
				//转动的图片数据
				runData.ReelSpinData = showData[m_RealRunReels[i].GetReelIndex()];
				//设置结果
				runData.ResultSymbols = resultData[m_RealRunReels[i].GetReelIndex()];
				
				runData.SetFastScale (m_IsFastMode?this.m_FastTimeScale*m_FastTimeScale_Plist:m_FastTimeScale_Plist, this.m_FastRunLengthScale);

				//如果 有必要则自定义修改
				if (runDataChange != null) {
					runDataChange.ChangeRunData (runData);
				}

				m_RealRunReels [i].StartSpin (runData);
				if (EnableNetWork && i ==0)
				{
					//        有需要网络请求stop的变量
					CanNetStop = false;//		net的时间处理
					this.InitNetRunStopTime(runData.ReelCurve);
				}
			}
			SpinState = SpinStateEnum.NormalRun;
		}

        //reset 处理blink的数组
        m_NeedPlaySmartSound = _needPlaySmartSound;

        this.ResetDealedSmart();
        isQuickStop = false;
	}
	
	public void DoReelsStop()
	{
		m_NeedPlaySmartSound = false;//stop play smart when quick stop
		if(SpinState == SpinStateEnum.NormalRun || SpinState == SpinStateEnum.Aniticipation){
			for (int i = 0; i < m_RealRunReels.Count; i++) {
				m_RealRunReels [i].ClickQuickStopSpin ();
			}
		}
	}

//	所有轮子停止之后的回调
	public void EachReelStopHandler(int reelIndex,bool isFastStop =false)
	{
		if (adapter != null) {
			adapter.DoEachReelStopHandler (reelIndex);
		}
		else
		{
			Debug.LogException(new Exception("ReelController EachReelStopHandler adapter is null"));
		}
		
		//没有处理过当前轮子
		//if (IsDealSmart (reelIndex) == false) {
		//	if (smartEffectController != null) {
		//		smartEffectController.PlaySmartSoundAndAnimation (reelIndex, this);
		//	}
		//}
		bool allStoped = false;
		for (int i = 0; i < Reels.Count; i++)
		{
			if (Reels[i].GetReelCurState() != SpinStateEnum.Stop)
			{
				allStoped = false;
				break;
			}
			allStoped = true;
		}

		if (allStoped)
		{
			AllReelsEnd(isFastStop);
		}
	}

	//普通轮子回弹时的回调
	public void ReelBounceBackHandler(int reelIndex,bool needPlayStopSound = true)
	{
        if (IsDealSmart(reelIndex))
        {
            return;
        }
        if (adapter != null) {
			adapter.ReelBounceBackHandler (reelIndex);
		}

        if (needPlayStopSound)
        {
	        PlayReelStopEffect();
        }
		
		if (smartEffectController != null && m_NeedPlaySmartSound) {
			smartEffectController.PlaySmartSoundAndAnimation (reelIndex, this);
		}
		//处理过当前轮子则设置
		DealSmart (reelIndex);
	}

	public virtual void ResetSymbolSlowDownTime()
	{
		for (int i = 0; i < m_BoardConfigs.ReelConfigs.Length; i++)
		{
			Reels[i].ResetSymbolSlowDownTime(m_AnticipationCurves[i]);
		}
	}
	
	// 轴即将停止时的回调
	public virtual void SymbolSlowDownCallback(int ReelIndex)
    {
	    if (adapter != null)
	    {
			adapter.DoEachReelSlowDownHandler(ReelIndex);    
	    }
    }

	public virtual void PlayReelStopEffect()
	{
		Libs.AudioEntity.Instance.PlayReelStopEffect ();
	}

    public void DoLastReelDistanceHandler(int reelIndex,SymbolRender s)
    {
        if(adapter != null) adapter.LastReelDistanceHandler(reelIndex, s);
    }

    public void ChangeSmartAnimationPosition(int reelId,float offsetY)
    {
        if(adapter!=null)
        {
            adapter.ChangeSmartAnimationPosition(reelId, offsetY);
        }
    }

 //   public void AntiStopHandler(int reelIndex)
 //{
 //	Libs.AudioEntity.Instance.PlayReelStopEffect();
 //}

 // ReSharper disable Unity.PerformanceAnalysis
 public void AllReelsEnd(bool isFastStop)
	{
		//如果是快速停止则需要播放停止音效
		//if (isFastStop) {
		//	Libs.AudioEntity.Instance.PlayReelStopEffect ();
		//}
		Debug.Log("EachReelStopHandler AllReelsEnd");

		SpinState = SpinStateEnum.Stop;
		
		if (adapter != null) {
			adapter.DoRunStopHandler (isFastStop);
		}
		else
		{
			Debug.LogException(new Exception("AllReelsEnd adapter is Null"));
		}
		
		Messenger.Broadcast (ReelConstant.REEL_STOP_RUN_HANDLER);
	}

    //注意在link等轮子不需要转动的时候调用此方法，出了link等之后需要调用ResetSpinReelsData方法，避免切换layout的时候出问题
	public void SetSpinReelsData(List<int> _spinReelIds)
	{
		this.m_RealRunReels = new List<Reel> (); //禁止使用Clear方法，否则下面的Reels会变为空，C#引用问题。
		for (int i = 0; i < _spinReelIds.Count; i++) {
			int reelId = _spinReelIds [i];
			this.m_RealRunReels.Add(this.Reels[reelId]);
		}
	}

	//如果anti或spin reel有改变不是通用的，则设置下此值
	public void ResetSpinReelsData()
	{
		this.m_RealRunReels = this.Reels;
	}

	public void SetAnitiReelIds(List<int> _antiReelIds)
	{
		this.m_AntiReelIds = _antiReelIds;
	}

	public void SetFastMode(bool _fast)
	{
		this.m_IsFastMode = _fast;
	}
    public bool GetFastMode()
    {
        return this.m_IsFastMode;
    }

    public void SetPlistFastTimeScale(float v)
    {
	    m_FastTimeScale_Plist = v;
    }

    public float CaculateKeyFrameTangent(Keyframe k1, Keyframe k2)
	{
		return (float)(((double)k2.value - (double)k1.value)/((double)k2.time - (double)k1.time) );
	}

	#endregion

	#region layout

	public  void LayOut(BoardConfigs _config,List<List<int>> reelsSpinData,IRunControllerAdapter _adapter)
	{
		if (_config == null) {
			return;
		}

		if (_config.ReelConfigs == null) {
			return;
		}

		this.m_BoardConfigs = _config;

		this.SymbolHeight = m_BoardConfigs.SymbolHeight;

		this.adapter = _adapter;

		//TODO: 1.需要删除其节点，比如在freespin中
		Util.DestroyChildren(this.transform);

        //清除animation层
        if (!_config.IsReserveAnimation)
        {
            this.symbolAnimationController.DestroyAnimation();
        }


        //2. 设置所有的reel,如果是respin 的话
        this.Reels.Clear();

		this.LayoutNormalReel (reelsSpinData);

		//3.如果curve没有设置，则需要程序先把匀速阶段的值校准正确，暂时先去掉，如使用的话需要修改里面的方法
//		AdjustLayoutCurves();

		SpriteMask sm = GetComponent<SpriteMask> ();
		if (sm != null) {
			sm.updateSprites ();
		}

		//4.初始化状态
		SpinState = SpinStateEnum.Stop;
	}
	public void TestSetBoardConfig(BoardConfigs _config)
	{
		this.m_BoardConfigs = _config;
	}


	protected virtual void LayoutNormalReel(List<List<int> > reelData)
	{
		float xCoordinate = m_BoardConfigs.ReelPositionX(); //当前中心点x的坐标
		for (int i = 0; i < m_BoardConfigs.ReelConfigs.Length; i++) {
			ClearSymolRenderMap (i);

			GameObject o = Instantiate (m_BoardConfigs.ReelPrefab);
			o.name = "reel" + i;

			RectTransform rt = o.GetComponent<RectTransform>() ;
			Vector2 sizeV = new Vector2 (m_BoardConfigs.ReelShowWidth(i), m_BoardConfigs.ReelShowHeight(i));
			rt.sizeDelta = sizeV; //.ReelConfigs [i].ReelHeight);

			Vector3 v = new Vector3 (xCoordinate, m_BoardConfigs.ReelConfigs[i].ReelYOffset, 0f);
			rt.localPosition = v;
			rt.SetParent(this.transform,false);

			if (symbolAnimationController != null && !this.m_BoardConfigs.IsReserveAnimation) {
				symbolAnimationController.AddAnimationReel (i, v,sizeV);
			}

			xCoordinate += m_BoardConfigs.ReelSpace + m_BoardConfigs.ReelWidth;

			Reel reel = o.GetComponent<Reel> ();

			reel.LayOut (this, m_BoardConfigs,i,reelData[i]);

			//如果有mask则设置
			SpriteMask mask = o.GetComponent<SpriteMask> ();
			if (mask != null) {
				mask.size = GetMaskSizeValue(rt.rect.size);
			}

			this.Reels.Add (reel);
		}

	}

	/// <summary>
	/// 设置MaskValue
	/// </summary>
	/// <param name="rectSize"></param>
	/// <returns></returns>
	public virtual Vector2 GetMaskSizeValue(Vector2 rectSize)
	{
		return rectSize;
	}


	public IRunControllerAdapter RunAdapter
	{
		get{
			return this.adapter;
		}
	}
//2019.12.23 注释不用
	//判断是不是symbol height的整数倍，如果是则不操作，否则需要计算多出来的高度，
	//然后计算多出的时间，将匀速后的值后移。计算方式有点类同于anti
	//2019.02.15 需将最后一帧更改成就取整的的，暂时先去掉，如使用的话需要修改里面的方法
//	private void AdjustLayoutCurves()
//	{
//		int len = this.m_NormalRunReelConfig.NormalRunReelCurve.Length;
//		for (int i = 0; i < len ;i++) {
//			AnimationCurve curve = this.m_NormalRunReelConfig.NormalRunReelCurve [i];
//			Keyframe[] frames = curve.keys;
//
//			float allMoveLength = frames [frames.Length - 1].value;
//			float extraLength =(float) ((double)allMoveLength * 1000f % SymbolHeight);
//
//			float deltaY= (SymbolHeight - extraLength)/1000f;
//
//			float v = CaculateKeyFrameTangent (frames [1], frames [2]);
//
//			float deltaT = deltaY / v;
//
//
//			for (int j = frames.Length - 1; j >= 2; j--) {
//				frames [j].value += deltaY;
//				frames [j].time += deltaT;
//
//				curve.MoveKey (j, frames [j]);
//			}
//		}
//	}


	#endregion

	#region data

	#if TEST_DEMO
	//根据总的背后每个reel的symbol的总个数来设置值的总数量
	private List<List<string>> CreateSpinData()
	{
		List<List<string>> result = new List<List<string>> ();
		for (int i = 0; i < this.m_BoardConfigs.ReelConfigs.Length; i++) {
			int randomValue = UnityEngine.Random.Range(0, this.m_BoardConfigs.ReelConfigs [i].ReelWeightData.Count);

			List<string> eachReelSymbols = new List<string> (); 
			for (int j = 0; j < this.m_BoardConfigs.ReelConfigs [i].ComputeSymbolNum; j++) {
				int index = (randomValue + j) % this.m_BoardConfigs.ReelConfigs [i].ReelWeightData.Count;
				eachReelSymbols.Add (this.m_BoardConfigs.ReelConfigs [i].ReelWeightData [index].SymbolName);
			}

			result.Add(eachReelSymbols);

		}

		return result;
	}

	#endif

	#endregion


	#region Get Property
	public int GetReelRenderCount(int reelIndex)
	{
		return m_BoardConfigs.ReelConfigs[reelIndex].ReelShowNum ;
	}

	public int GetReelCount()
	{
		return this.Reels.Count;
	}

    public float GetReelWidth()
    {
        return m_BoardConfigs.ReelWidth;
    }

    public float GetBoardWidth()
    {
	    return m_BoardConfigs.ReelPanelWidth;
    }
    
    public float GetBoardSpace()
    {
	    return m_BoardConfigs.ReelSpace;
    }
    #endregion

    #region symbol render map 静止的时候棋盘上的数据map
    public Dictionary<int,Dictionary<int,Classic.BaseElementPanel>> SymbolRenderDic = new Dictionary<int, Dictionary<int, Classic.BaseElementPanel>>();
	//1.设置进去map,记得需要去重置信息，在当前轮子开始转动的时候设置，其实也不用设置，停止后设置也行
	public void ClearSymolRenderMap(int reelIndex)
	{
		if (this.SymbolRenderDic.ContainsKey (reelIndex)) {
			this.SymbolRenderDic [reelIndex].Clear ();
		}
	}
	private void ClearAllRenderMap()
	{
		this.SymbolRenderDic.Clear ();
	}
	//设置停止界面render的映射，用来获取信息
	public void SetSymbolRenderMap(int reelIndex, int positionIndex,BaseElementPanel render)
	{
		if (!SymbolRenderDic.ContainsKey (reelIndex)) {
			SymbolRenderDic [reelIndex] = new Dictionary<int, BaseElementPanel> ();
		}
		SymbolRenderDic [reelIndex] [positionIndex] = render;
	}

	//2.根据索引获取
	public BaseElementPanel GetSymbolRender(int reelIndex, int positionIndex)
	{
		if (!SymbolRenderDic.ContainsKey (reelIndex) || !SymbolRenderDic[reelIndex].ContainsKey (positionIndex)) {
			if (!BaseGameConsole.ActiveGameConsole().isForTestScene)
			{
				Debug.LogError ("位于"+reelIndex+":"+positionIndex + "的映射为空");
			}
			return null;
		}
		return SymbolRenderDic [reelIndex] [positionIndex];
	}

	public bool ContainSymbolRender(int reelIndex, int positionIndex)
	{
		return SymbolRenderDic.ContainsKey(reelIndex) && SymbolRenderDic[reelIndex].ContainsKey(positionIndex) &&
		       SymbolRenderDic[reelIndex][positionIndex] != null;
	}
	
	#endregion

	#region animation symbol动画相关
	public SymbolAnimationController symbolAnimationController;
	
	public void AddAnimationRender(int reelIndex, int positionId,Vector3 v,Vector2 sizeV,bool isUpOverDown)
	{
		if (symbolAnimationController != null) {
			symbolAnimationController.AddAnimationRender (reelIndex, positionId, v,sizeV,isUpOverDown);
		}
	}

	public Transform GetAnimationRender(int reelIndex, int positionId)
	{
		if (symbolAnimationController != null) {
			if (symbolAnimationController.GetAnimationRender (reelIndex, positionId) != null)
			{
				return symbolAnimationController.GetAnimationRender (reelIndex, positionId).transform;
			}
		}
		return null;
	}

	public void ReSetAnimationRender(int reelIndex)
    {
        if (symbolAnimationController != null)
        {
            symbolAnimationController.ReSetAnimationRender(reelIndex);
        }
    }

    public Transform GetAnimationReel(int reelIndex)
    {
        if (symbolAnimationController != null)
        {
            return symbolAnimationController.GetAnimationReel(reelIndex).transform;
        }
        return null;
    }
    //public void ExchangeAnimationRender(int fromReel,int fromPosition,int toReel,int toPosition)
    //{
    //    if(symbolAnimationController!=null)
    //    {
    //        symbolAnimationController.ExchangeAnimationRender(fromReel, fromPosition, toReel, toPosition);
    //    }
    //}
    #endregion

    #region smartSound
    private List<bool> DealedSmartReels= new List<bool> ();

	private void ResetDealedSmart()
	{
		for (int i = 0; i < this.GetReelCount (); i++) {
			if (i<DealedSmartReels.Count) {
				DealedSmartReels [i] = false;
			} else {
				DealedSmartReels.Add (false);
			}
		}
	}
	
	private void DealSmart(int reelIndex)
	{
		DealedSmartReels [reelIndex] = true;
	}

	private bool IsDealSmart(int reelIndex)
	{
		return reelIndex < DealedSmartReels.Count  && DealedSmartReels [reelIndex];
	}

    #endregion

    #region gliding 滑动
 
    private int m_GlidindReelNum = 0;
    private int m_CurrentGlidingNum = 0;
    //moveNum,正值为向下移动,负值为向下移动
    public void Gliding(Dictionary<int, ReelGlidingData> reelGlidingData, Action callback = null,float TotalGlidingTime = .5f)
    {
        Libs.AudioEntity.Instance.PlayGlidingEffect();
        m_GlidindReelNum = m_CurrentGlidingNum= 0;
        //m_GlidindReelNum = reelGlidingData.Keys.Count;
        foreach (int key in reelGlidingData.Keys)
        {
            if(reelGlidingData[key].IsGliding)
            {
                m_GlidindReelNum++;
                Reels[key].GlidingByLength(reelGlidingData[key].IsUp, reelGlidingData[key].GlidingLength, delegate {
                    m_CurrentGlidingNum++;
                    if(m_CurrentGlidingNum == m_GlidindReelNum)
                    {
                        callback?.Invoke();
                    }

                },TotalGlidingTime);
            }

        }
    }


    public void Gliding(int reelIndex,bool isUp, int moveNum,Action callback =null)
	{
		Reels[reelIndex].GlidingByLength(isUp,moveNum, callback);
	}


    //设置底部的未显示的symbol，一般用在gliding中
    public void SetTopNotShowSymbol(int reelIndex,int num, int symbolIndex)
    {
        Reels[reelIndex].SetTopNotShowSymbol(num, symbolIndex);
    }
    //设置头部的未显示的symbol，一般用在gliding中
    public void SetBelowNotShowSymbol(int reelIndex,int num, int symbolIndex)
    {
        Reels[reelIndex].SetBelowNotShowSymbol(num, symbolIndex);
    }

    #endregion
    
    #region net 网络请求, 转速相关

    //初始化时调用
    //是否可以网络响应的时候停止
    [HideInInspector]
    public bool CanNetStop;
    
//    走一个牌面需要的时长
	[HideInInspector]
    public float OneReelOffsetTime = 0;
    //网络请求时reel停止的时间
    [HideInInspector]
    public float NetStopTime = 0;
//    public void ReceiveServerResults()
//    {
//	    CanNetStop = true;
//    }
    
    //在匀速的最后一个阶段处理匀速循环的事件
    public  void InitNetRunStopTime(AnimationCurve curve)
    {
	    int RunEndFrame = 2;
	    float tangentValue = (curve.keys[RunEndFrame].value - curve.keys[RunEndFrame-1].value) * 1000/
	                         (curve.keys[RunEndFrame].time - curve.keys[RunEndFrame-1].time);
	    float height=  (m_BoardConfigs.SymbolHeight * m_BoardConfigs.ReelConfigs[0].ReelShowNum);
	    OneReelOffsetTime = height / tangentValue;
//		多整了2列
	    NetStopTime = curve.keys[RunEndFrame].time - OneReelOffsetTime * 2;
    }
	#endregion 


	#region Server相关的逻辑，方法1开始的时候参数：reelCurves转速配置、哪些轮子需要转动；方法2开始转动参数： 停止时的带子数据、 anti带子、CheckPlaySmartSound

	public void ServerStartSpin(List<List<int>> _spinData, List<int> _reelCurves,bool _needPlaySmartSound)
	{
		if (SpinState == SpinStateEnum.Stop)
		{
			m_AntiReelIds.Clear();
			for (int i = 0; i < m_RealRunReels.Count; i++) {
				ReelRunData runData;
//				if (IsContainAnticipation()) {
//					runData = new ReelRunData (i, IsAnticpationReel(m_RealRunReels[i].GetReelIndex()) , m_AnticipationCurves [_reelCurves[i]],i==m_RealRunReels.Count-1);
//				} else
//				{
					AnimationCurve speedCurve = this.m_IsTestSpeedPattern
						? m_NormalRunReelConfig.TestRunReelCurve[_reelCurves[i]]
						: m_NormalRunReelConfig.NormalRunReelCurve[_reelCurves[i]];
					runData = new ReelRunData (i,false, speedCurve,i==m_RealRunReels.Count-1);
//				}
				//转动的图片数据
				runData.ReelSpinData = _spinData [m_RealRunReels [i].StaticReelIndex];
				
				runData.SetFastScale (m_IsFastMode?this.m_FastTimeScale*m_FastTimeScale_Plist:m_FastTimeScale_Plist, this.m_FastRunLengthScale);

				//如果 有必要则自定义修改
				if (runDataChange != null) {
					runDataChange.ChangeRunData (runData);
				}

				m_RealRunReels [i].StartSpin (runData);
				
				if (EnableNetWork && i ==0)
				{
					//        有需要网络请求stop的变量
					CanNetStop = false;//		net的时间处理
					this.InitNetRunStopTime(runData.ReelCurve);
				}
			}
			SpinState = SpinStateEnum.NormalRun;
		}

		//reset 处理blink的数组
		m_NeedPlaySmartSound = _needPlaySmartSound;

		this.ResetDealedSmart();
	}

	public void ServerStopSpin(List<List<int>> _retData,List<int> _antiReels)
	{
		CanNetStop = true;
		
		this.m_AntiReelIds = _antiReels;
		if (IsContainAnticipation()) {
			
			m_AnticipationCurves.Clear ();
			if (m_IsTestSpeedPattern)
			{
				m_AnticipationCurves = new List<AnimationCurve> (this.m_NormalRunReelConfig.TestRunReelCurve);
			}
			else
			{
				m_AnticipationCurves = new List<AnimationCurve> (this.m_NormalRunReelConfig.NormalRunReelCurve);
			}
			InitAnticipationReelCurve ();
			ResetSymbolSlowDownTime();
		}
		
		for (int i = 0; i < m_RealRunReels.Count; i++)
		{
			m_RealRunReels[i].SetServerRetData(_retData[m_RealRunReels[i].StaticReelIndex]);
			if (IsContainAnticipation())
			{
				m_RealRunReels[i].SetAntiData(IsAnticpationReel(m_RealRunReels[i].GetReelIndex()) , m_AnticipationCurves [i]); 
			}
			
		}

		
	}
	

	#endregion
}
