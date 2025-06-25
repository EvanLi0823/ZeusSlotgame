using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using Libs;
using System;
using Classic;
using Beebyte.Obfuscator;
[Skip]
[Serializable]
public class SuperWheelController : MonoBehaviour
{
	public List<WheelElement> wheelElements;

	protected float ANGLE_INTERVAL = 30f;
	public int TotalCircleNum = 2;

	protected float totalAngleDistance = 720f;
	protected const float CIRCLE_ANGLE_LENGTH = 360f;
	protected float slowDownDistance = 180f;
	protected float startSpeedUpDistance = 15f;
	protected float currentTravelDistance = 0f;
	[HideInInspector]
	public RectTransform mWheelRectTransfm;
	[HideInInspector]
	public int awardIndex = 0;

	private int preAwardIndex = 0;
	public bool EnableContinue = false;
	[HideInInspector]
	public BaseAward awardInfo = new BaseAward();

	protected enum RollState
	{
		RollState_None,
		RollState_StartRunBack,
		RollState_Running,
		RollState_FixedRunning,
		RollState_SlowDown,
		RollState_Stop,
		RollState_StopRunBack,
	}

	protected RollState currentState = RollState.RollState_None;

	protected float timeStep
	{
		get { return Time.fixedDeltaTime; }
	}

	private GameCallback onWheelStartRoll;
    private WheelCallback onWheelRuning;
	private GameCallback onWheelEnd;

	public void OnInitGameConfigForLua(Transform wheelTransfm, Action onWheelStartRoll, Action onWheelEnd, Action<float> wheelCallback)
	{
		this.mWheelRectTransfm = wheelTransfm as RectTransform;
		this.onWheelStartRoll = () => onWheelStartRoll();
		if (wheelCallback != null)
			this.onWheelRuning = param => wheelCallback(param);
		else
			this.onWheelRuning = null;
		this.onWheelEnd = () => onWheelEnd();

		if (wheelElements == null) wheelElements = new List<WheelElement>();
	}
	//初始化游戏相关设置
	public void OnInitGameConfig(Image wheel, GameCallback onWheelStartRoll = null, GameCallback onWheelEnd = null,WheelCallback wheelCallback = null)
	{
		this.mWheelRectTransfm = wheel.rectTransform;
		this.onWheelStartRoll = onWheelStartRoll;
        this.onWheelRuning = wheelCallback;
		this.onWheelEnd = onWheelEnd;

		if (wheelElements == null) wheelElements = new List<WheelElement>();
	}

	public void OnInitGameConfig(Transform wheelTransfm, GameCallback onWheelStartRoll = null, GameCallback onWheelEnd = null,WheelCallback onWheelRuning =null)
	{
		this.mWheelRectTransfm = wheelTransfm as RectTransform;
		this.onWheelStartRoll = onWheelStartRoll;
        this.onWheelRuning = onWheelRuning;
		this.onWheelEnd = onWheelEnd;
        

		if (wheelElements == null) wheelElements = new List<WheelElement>();
	}

	public void InitCallBack (GameCallback onWheelStartRoll, GameCallback onWheelEnd = null,WheelCallback wheelCallback = null)
	{
		if (onWheelStartRoll != null)
			this.onWheelStartRoll = onWheelStartRoll;
		if (onWheelEnd != null)
			this.onWheelEnd = onWheelEnd;
        if (wheelCallback != null)
            this.onWheelRuning = wheelCallback;
	}

	void Awake()
	{
		if (wheelElements != null && wheelElements.Count > 0)
		{
			ANGLE_INTERVAL = CIRCLE_ANGLE_LENGTH / wheelElements.Count;
		}
	}

    public void InitBaseData()
    {
        if (wheelElements != null && wheelElements.Count > 0)
        {
            ANGLE_INTERVAL = CIRCLE_ANGLE_LENGTH / wheelElements.Count;
        }
    }

	//重置转轮的角度以及转旋转偏移量.状态
	public void ResetCircle(){
		if (mWheelRectTransfm == null) return;

		mWheelRectTransfm.eulerAngles = new Vector3(0, 0, 0);
		preAwardIndex = awardIndex;


		preAngle = Quaternion.identity;
		currentState = RollState.RollState_None;
//		awardInfo = new BaseAward();

		bCollision = false;

		startRunbackInitFlag = false;
		inverseSpin = true;

		initRunning = false;

		initSlowDown = false;

		InitStopRunBackFlag = false;
	}
	public void Reset(bool forceResetAll = false)
	{
		if (mWheelRectTransfm == null) return;
        

		if (forceResetAll || !EnableContinue)
		{
			mWheelRectTransfm.eulerAngles = new Vector3(0, 0, 0);
			preAwardIndex = awardIndex = 0;
		}
		else
		{
			preAwardIndex = awardIndex;
		}

		preAngle = currentAngle = mWheelRectTransfm.localRotation;
		currentState = RollState.RollState_None;
		awardInfo = new BaseAward();

		bCollision = false;

		startRunbackInitFlag = false;
		inverseSpin = true;

		initRunning = false;

		initSlowDown = false;

		InitStopRunBackFlag = false;
	}

	void OnDisable()
	{
		Reset();
	}

	void FixedUpdate()
	{
		BaseStateUpdate();
		PlayPointerAnimation();
	}

	protected Quaternion preAngle = Quaternion.identity;
	protected Quaternion currentAngle = Quaternion.identity;
	[HideInInspector]
	public bool bCollision = false;

	public void PlayPointerAnimation()
	{
		if (mWheelRectTransfm == null) return;
        
		currentAngle = mWheelRectTransfm.localRotation;
		if (Mathf.Abs(Quaternion.Angle(currentAngle, preAngle)) >= ANGLE_INTERVAL)
		{
			preAngle = currentAngle;
			bCollision = true;
		}
	}

	public bool HasStopRunBack = true;
	public bool HasStartRunBack = true;
	public virtual void BaseStateUpdate()
	{
		switch (currentState)
		{
			case RollState.RollState_StartRunBack:
				StartRunBack();
				break;
			case RollState.RollState_Running:
				Running();
				break;
			case RollState.RollState_FixedRunning:
				FixedRunning();
				break;
			case RollState.RollState_SlowDown:
				Slowdown();
				break;
			case RollState.RollState_Stop:
				Stop();
				break;
			case RollState.RollState_StopRunBack:
				StopRunBack();
				break;
			default:
				break;
		}
	}

	public void StartRoll()
	{
		if (onWheelStartRoll != null) onWheelStartRoll();

		if(HasStartRunBack) currentState = RollState.RollState_StartRunBack;
		else currentState = RollState.RollState_Running;

		float tempDistance = 0;
		if (EnableContinue && preAwardIndex != 0){
			tempDistance = (preAwardIndex) * (CIRCLE_ANGLE_LENGTH / wheelElements.Count);
			if (!inverseSpin) tempDistance = CIRCLE_ANGLE_LENGTH - tempDistance;
		}

		tempDistance += TotalCircleNum * CIRCLE_ANGLE_LENGTH + (CIRCLE_ANGLE_LENGTH - (awardIndex) * (CIRCLE_ANGLE_LENGTH / wheelElements.Count));
		if(HasStartRunBack) tempDistance += startSpeedUpDistance;
		if(HasStopRunBack) tempDistance += ANGLE_INTERVAL / 2;//回正使用

		totalAngleDistance = tempDistance;
	}

	#region first startRunBack Stage

	void StartRunBack()
	{
		PreStartRunBack();
		StartRunBackInProcess();
		AfterStartRunBack();
	}

	private bool startRunbackInitFlag = false;
	private bool inverseSpin = true;
	[Header("旋转起来正常的速度")]
	public float startRunbackSpeed = 160f;
	float startRunBackEulerAngleZ;
	float startRunBack_vStep;
	float currentStartSpeedUpDistance;

	void PreStartRunBack()
	{
		if (startRunbackInitFlag) return;

		startRunbackInitFlag = true;
		startRunBackEulerAngleZ = mWheelRectTransfm.localEulerAngles.z;
		currentStartSpeedUpDistance = 0;
	}

	void StartRunBackInProcess()
	{
       
		if (currentStartSpeedUpDistance < startSpeedUpDistance)
		{
			if (inverseSpin)
			{
				startRunBack_vStep = Mathf.Lerp(startRunbackSpeed, 1f, currentStartSpeedUpDistance / startSpeedUpDistance);
				startRunBackEulerAngleZ += startRunBack_vStep * timeStep;
				currentStartSpeedUpDistance += startRunBack_vStep * timeStep;
				mWheelRectTransfm.localEulerAngles = Vector3.forward * startRunBackEulerAngleZ;
			}
			else
			{
				startRunBack_vStep = 2 * Mathf.Lerp(startRunbackSpeed, 1f, currentStartSpeedUpDistance / startSpeedUpDistance);
				startRunBackEulerAngleZ -= startRunBack_vStep * timeStep;
				currentStartSpeedUpDistance -= startRunBack_vStep * timeStep;
				mWheelRectTransfm.localEulerAngles = Vector3.forward * startRunBackEulerAngleZ;
			}
           //if(onWheelRuning != null)
           // {
           //     onWheelRuning(mWheelRectTransfm.localEulerAngles.z);
           // }
		}
		else
		{
			if (inverseSpin) inverseSpin = false;
			else currentState = RollState.RollState_Running;
		}
	}

	void AfterStartRunBack()
	{
		
	}

	#endregion

	#region second Running Stage

	void Running()
	{
		PreRunning();
		RunningInProcess();
		AfterRunning();
	}


	bool initRunning = false;
	float startRunEulerAngleZ;
	float startRunSpeed;
	float startRun_vStep;

	void PreRunning()
	{
		if (initRunning) return;

		startRunEulerAngleZ = mWheelRectTransfm.localEulerAngles.z;
		currentTravelDistance = 0;
		startRunSpeed = startRunbackSpeed * 2;
		initRunning = true;
	}

	void RunningInProcess()
	{
		if (totalAngleDistance - currentTravelDistance > slowDownDistance)
        {
            if (CheckFastStop){
                if (BaseSlotMachineController.Instance.reelManager.fastStop){
                    currentTravelDistance = totalAngleDistance;
                    mWheelRectTransfm.localEulerAngles = Vector3.forward * currentTravelDistance;
                    //if (onWheelRuning != null)
                    //{
                    //    onWheelRuning(mWheelRectTransfm.localEulerAngles.z);
                    //}
                    return;
                }
            }

            startRun_vStep = startRunSpeed * timeStep;
            currentTravelDistance += startRun_vStep;
            startRunEulerAngleZ -= startRun_vStep;
            mWheelRectTransfm.localEulerAngles = Vector3.forward * startRunEulerAngleZ;
            //if (onWheelRuning != null)
            //{
            //    onWheelRuning(mWheelRectTransfm.localEulerAngles.z);
            //}
        }
		else
		{
			currentState = RollState.RollState_SlowDown;
		}
        
	}

	void AfterRunning()
	{

	}

	#endregion

	#region third FixedRunning Stage  temp Repeal

	void FixedRunning()
	{
		PreFixedRunning();
		FixedRunningInProcess();
		AfterFixedRunning();
	}

	void PreFixedRunning()
	{
       
	}

	void FixedRunningInProcess()
	{

      
	}

	void AfterFixedRunning()
	{
        
	}

	#endregion

	#region fourth SlowDown Stage

	void Slowdown()
	{
		PreSlowDown();
		SlowDownInProcess();
		AfterSlowDown();
	}

	bool initSlowDown = false;
	float slowDown_Velocity;
	float slowDownEulerAnglesZ;
	float slowDown_vStep;

	[Header("减速时的速度")]
	public float m_SlowDownMinSpeed = 1f;

	void PreSlowDown()
	{
		if (initSlowDown) return;

		slowDown_Velocity = startRunSpeed;
		slowDownEulerAnglesZ = mWheelRectTransfm.localEulerAngles.z;
		initSlowDown = true;
	}

    [HideInInspector]
    public bool CheckFastStop {get;set;}
	void SlowDownInProcess()
	{
		float distanceDiff = totalAngleDistance - currentTravelDistance;
		if (distanceDiff > 0)
		{
			if (distanceDiff < slowDownDistance){
                if (CheckFastStop){
                    if (BaseSlotMachineController.Instance.reelManager.fastStop){
                        currentTravelDistance = totalAngleDistance;
                        mWheelRectTransfm.localEulerAngles = Vector3.forward * currentTravelDistance;
                        if (onWheelRuning != null)
                        {
                            onWheelRuning(mWheelRectTransfm.localEulerAngles.z);
                        }
                        return;
                    }
                }
                   
                slowDown_vStep = Mathf.Lerp(m_SlowDownMinSpeed, slowDown_Velocity, distanceDiff / slowDownDistance) * timeStep;
                currentTravelDistance += slowDown_vStep;
                slowDownEulerAnglesZ -= slowDown_vStep;
                mWheelRectTransfm.localEulerAngles = Vector3.forward * slowDownEulerAnglesZ;
                if (onWheelRuning != null)
                {
                    onWheelRuning(mWheelRectTransfm.localEulerAngles.z);
                }
            } 
		}
		else
		{
			if (HasStopRunBack) currentState = RollState.RollState_StopRunBack;
			else EndRun();
		}
	}

	void AfterSlowDown()
	{
        
	}

	#endregion

	#region fifth Stop Stage  temp Repeal

	void Stop()
	{
		PreStop();
		StopInProcess();
		AfterStop();
	}

	void PreStop()
	{
     
	}

	void StopInProcess()
	{
		
	}

	void AfterStop()
	{
       
	}

	#endregion

	#region sixth StopRunBack Stage

	void StopRunBack()
	{
		if (!InitStopRunBackFlag){
			InitStopRunBackFlag = true;
			stopRunBackDistance = ANGLE_INTERVAL / 2;
			stopRunBackOffset = 0f;
			stopRunBackEulerAngleZ = mWheelRectTransfm.localEulerAngles.z;
		}

		stopRunBack_vStep = StopRunBackSpeed * timeStep;
		stopRunBackOffset += stopRunBack_vStep;

		stopRunBackEulerAngleZ += stopRunBack_vStep;
		mWheelRectTransfm.localEulerAngles = Vector3.forward * stopRunBackEulerAngleZ;

		if (stopRunBackOffset >= stopRunBackDistance){
			EndRun();
		}
	}

	float stopRunBackDistance;
	float stopRunBackOffset;
	[Header("停止前回弹的速度")]
	public float StopRunBackSpeed = 10f;
	bool InitStopRunBackFlag = false;
	float stopRunBack_vStep;
	float stopRunBackEulerAngleZ;

	void EndRun()
	{
		mWheelRectTransfm.localEulerAngles = Vector3.forward * ((awardIndex * 360f) / wheelElements.Count);
		//mWheelRectTransfm.localRotation = Quaternion.Euler(new Vector3(0, 0, (awardIndex * 360f) / wheelElements.Count));
		if (onWheelEnd != null) onWheelEnd();

		currentState = RollState.RollState_None;
	}

	#endregion

	public WheelElement GenResult(float ScalFactor = 1.0f)
	{
		return BaseGenResult(ScalFactor);
	}

	WheelElement BaseGenResult(float ScalFactor)
	{
		float totalWeight = 0;
		for (int i = 0; i < wheelElements.Count; i++)
		{
			totalWeight += wheelElements[i].Weight;
		}
		int temp = UnityEngine.Random.Range(0, (int)totalWeight);
		awardIndex = wheelElements.Count - 1;

		float lastWeights = 0;
		float weights = wheelElements[0].Weight;
		for (int i = 1; i < wheelElements.Count; i++)
		{
			if (lastWeights <= temp && temp < weights)
			{
				awardIndex = i - 1;
				break;
			}
			else
			{
				lastWeights = weights;
				weights += wheelElements[i].Weight;
			}
		}
		awardInfo.awardValue = wheelElements[awardIndex].Pay / ScalFactor;
		return wheelElements[awardIndex];
	}

	public int HourlyBonusCollectTimes
	{
		get{ return SharedPlayerPrefs.GetPlayerPrefsIntValue("HourlyBonusCollectTimes", 0); }
		set{ SharedPlayerPrefs.SetPlayerPrefsIntValue("HourlyBonusCollectTimes", value); }
	}

	public WheelElement PseudoRandomReslut(float ScalFactor = 1.0f)
	{
		int random_count = 1;
		Dictionary<string,object> random_dic = Plugins.Configuration.GetInstance().GetValueWithPath<Dictionary<string, object>>("LotteryPseudoRandom", null);

		int collect_times = HourlyBonusCollectTimes;
		if (random_dic != null) 
		{
			int clircle_index = collect_times % 7;
			if (collect_times > 0) {
				switch (clircle_index) {
				case 3:
					random_count = Utils.Utilities.CastValueInt (random_dic ["EveryThreeTimes"], 1);
					break;	
				case 0:
					random_count = Utils.Utilities.CastValueInt (random_dic ["EverySevenTimes"], 1);
					break;	
				}
			}
		}
			
		float totalWeight = 0;
		for (int i = 0; i < wheelElements.Count; i++)
		{
			totalWeight += wheelElements[i].Weight;
		}

		int index = 0;
		for (int j = 0; j < random_count; j++) 
		{
			int temp = UnityEngine.Random.Range (0, (int)totalWeight);
			index = wheelElements.Count - 1;
			float lastWeights = 0;
			float weights = wheelElements [0].Weight;
			for (int i = 1; i < wheelElements.Count; i++) {
				if (lastWeights <= temp && temp < weights) {
					index = i - 1;
					break;
				} else {
					lastWeights = weights;
					weights += wheelElements [i].Weight;
				}
			}
			if (j == 0 || (wheelElements [index].Pay > wheelElements [awardIndex].Pay)) 
			{
				awardIndex = index;
			}
		}
		HourlyBonusCollectTimes = collect_times + 1;
		awardInfo.awardValue = wheelElements[awardIndex].Pay / ScalFactor;
	
		return wheelElements[awardIndex];
	}


	/// <summary>
	/// 根据服务器返回的信息设置awardIndex以及awardInfo.awardValut
	/// </summary>
	/// <param name="awardIndexFromServer">Award index from server.</param>
	public void SetAwardIndexFromServer(int awardIndexFromServer)
	{
		if (awardIndexFromServer >= 0 && awardIndexFromServer < wheelElements.Count){
			awardIndex = awardIndexFromServer;
			awardInfo.awardValue = wheelElements[awardIndex].Pay;
		}
	}
}
