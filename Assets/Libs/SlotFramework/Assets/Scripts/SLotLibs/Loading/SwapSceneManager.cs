using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Classic;
using System;
using RealYou.Core.UI;
using Libs;
using UniRx.Async;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

/// <summary>
/// Exchange swap manager.
/// 目标致力于完成全局管理切换场景的任务,而不受切换场景被销毁的命运，同时便于集中统一管理
/// 目前已完成
/// </summary>
public class SwapSceneManager:IMachineBehaviour
{
	#region 接口实现
	public int Priority => (int)Libs.UIMBPriority.CommonSysDlg;
	public UniTask OnPreEnterMachine(bool isRestore)
	{
		return UniTask.CompletedTask;
	}
	public UniTask OnLateEnterMachine(bool isRestore)
	{
		if (finishedCB != null) finishedCB(eid);
		finishedCB = null;
		eid = 0;
		return UniTask.CompletedTask;
	}
	
	public UniTask OnSpinEnd()
	{
		return UniTask.CompletedTask;
	}
	public UniTask OnExitMachine()
	{
		return UniTask.CompletedTask;
	}
	#endregion
	
	#region 事件处理

	private DelayAction locateDelayAction;
	private const float locateAddTime = 0.5f;
	public const string StartMoveCardSlider = "StartMoveCardSlider";
	
	public void RegisterLockUIEvent(string slotName)
	{
		if (finishedCB != null) return;
		eid = UIManager.Instance.RegisterUIEventAtOnce(slotName, out finishedCB);
	}

	public void UnRegisterLockUIEvent()
	{
		UIManager.Instance.UnRegisterUIEventAtOnce(eid,finishedCB);
		eid = 0;
		finishedCB = null;
		UnRegisterLocateTimeCheck();
	}

	private Dictionary<string,Action<string>> SwapEventCBs= new Dictionary<string, Action<string>>();
	public void RegisterSwapEvent(string token,Action<string> DoCB)
	{
		if(!SwapEventCBs.ContainsKey(token))
			SwapEventCBs.Add(token,null);
		SwapEventCBs[token] = DoCB;
	}

	public void UnRegisterSwapEvent(string token)
	{
		if (SwapEventCBs.ContainsKey(token))
			SwapEventCBs.Remove(token);
	}
	
	/// <summary>
	/// 监听大厅/Club大厅 的卡牌拖动. 定位机器过程中如果主动拖动大厅卡牌滑动条, 则相当于机器无法下载主动调用UnRegisterLockUIEvent
	/// </summary>
	/// <param name="data"></param>
	private void LobbyCardBeginDrag(PointerEventData data)
	{
		if (eid <= 0) return;
		UnRegisterLockUIEvent();
	}

	private void RegisterLocateTimeCheck(float time)
	{
		if (locateDelayAction != null) return;
		locateDelayAction = new DelayAction(time + locateAddTime, null, () =>
		{
			UnRegisterLockUIEvent();
		});
	}
	private void UnRegisterLocateTimeCheck()
	{
		if (locateDelayAction != null)
		{
			locateDelayAction.Kill();
		}

		locateDelayAction = null;
	}

	#endregion
	
	public string EnterSlotType { get; set;}

	public bool IsFsAwardEnterMachine { get; set; } = false;

	#region ClubSystem

	private string CurLogicSceneName;

	private void SetLogicSceneName(string logicSceneName)
	{
		CurLogicSceneName = logicSceneName;
	}

	public string GetLogicSceneName(bool forceSceneName = false)
	{
		CurLogicSceneName = SceneManager.GetActiveScene().name;
		return CurLogicSceneName;
	}

	public AsyncOperation LoadSceneAsync(string sceneName,string logicName = "",LoadSceneMode mode=LoadSceneMode.Single)
	{
		if (!string.IsNullOrEmpty(logicName))
		{
			CurLogicSceneName = logicName;
		}
		AsyncOperation asyncOP = SceneManager.LoadSceneAsync(sceneName,mode);
		return asyncOP;
	}
	#endregion

	private SwapSceneManager()
	{
		Messenger.AddListener<float>(StartMoveCardSlider,RegisterLocateTimeCheck);
		MachineUtility.Instance.RegisterMachineEvent(this);
	}
	~SwapSceneManager()
	{
		Messenger.RemoveListener<float>(StartMoveCardSlider,RegisterLocateTimeCheck);
		MachineUtility.Instance.UnRegisterMachineEvent(this);
	}
	public void Init()
	{
		
	}
	private void Go2SlotMachine(string slotName)
	{
		if (GetLogicSceneName() == slotName)
			return;

		SwapGameScene(slotName, true, null, null);
	}

	public static SwapSceneManager Instance{get{return Singleton<SwapSceneManager>.Instance;}}

	private void SwapScene(object slotMachineConfig,System.Action startCB,System.Action finishCB,bool isRewardSpin = false)
	{
		if (isLoading) {
			UnRegisterLockUIEvent();
			return;
		}
		UnRegisterLocateTimeCheck();
		isLoading = true;
		if (startCB!=null) {
			startCB ();
		}

		string slot_name = "";
		if (slotMachineConfig is SlotMachineConfig config)
		{
			SetLogicSceneName(config.Name());
			slot_name = config.Name();
		}
		//SetScreenOrientation(slotMachineConfig);
		//flycoinsPanel hide self
        Messenger.Broadcast(GameConstants.SWAP_SCENE_NEED_DO);
        CoinsBezier.Instance.KillAllCoins();
        AudioEntity.Instance.StopAllMusic();
        Messenger.Broadcast<System.Action,object>(GameDialogManager.OpenSceneExchange, ()=>{
			isLoading = false;
			if (finishCB!=null) {
				finishCB();
			}
		}, slotMachineConfig);
	}

    public void SetScreenOrientation(object config)
    {
        if (config!=null && config is SlotMachineConfig)
        {
            if (((SlotMachineConfig)config).IsPortrait)
            {
                SkySreenUtils.CurrentOrientation = ScreenOrientation.Portrait;
            }
            else
            {
                SkySreenUtils.CurrentOrientation = ScreenOrientation.LandscapeLeft;
            }
        }
        else
        {
		    SkySreenUtils.CurrentOrientation = ScreenOrientation.Portrait;
        }
    }

    public  ScreenOrientation GetScreenOrientation(object config)
    {
	    if (config==null)
	    {
		    return SkySreenUtils.CurrentOrientation;
	    }
	    if (config is SlotMachineConfig)
	    {
		    if (((SlotMachineConfig)config).IsPortrait)
		    {
			    return ScreenOrientation.Portrait;
		    }
		    else
		    {
			    return ScreenOrientation.LandscapeLeft;
		    }
	    }
	    else
	    {
		    return  ScreenOrientation.LandscapeLeft;
	    }
    }

    private void SwapSlotScene(SlotMachineConfig slotMachineConfig,System.Action startCB=null,System.Action finishCB=null){
		GoToSlotScene(slotMachineConfig,startCB,finishCB);
	}

	private void GoToSlotScene(SlotMachineConfig slotMachineConfig,System.Action startCB,System.Action finishCB)
	{
		Libs.AudioEntity.Instance.PlayEnterRoomEffect();
		SwapScene (slotMachineConfig,startCB,finishCB);
	}

	private bool isLoading = false;
	public bool IsLoading {
		get {
			return isLoading;
		}
	}
	
	private Action<int> finishedCB = null;
	private int eid = 0;
	public void SwapGameScene(string sceneName,bool needLocateSlot,System.Action startCB=null,System.Action finishCB=null,bool isRewardSpin = false)
	{
		if (isLoading) return;
		
		if (finishedCB != null) return;
	
		//1.添加锁定UIEvent的节点
		RegisterLockUIEvent(sceneName);
		#region 3.机器 
		SlotMachineConfig config = BaseGameConsole.ActiveGameConsole (true).SlotMachineConfig (sceneName);
		if (config!=null) {
			SwapSlotScene (config,startCB, () =>
			{
				UnRegisterLockUIEvent();
			});
		}
		#endregion
		
	}

	public bool EnterSlotScene(string sceneName, Action startCB = null, Action finishCB = null)
	{
		if (isLoading)
			return false;
		SlotMachineConfig config = BaseGameConsole.ActiveGameConsole (true).SlotMachineConfig (sceneName);
		if (config == null)
			return false;
		SwapScene(config, startCB, finishCB);
		return true;
	}
	public void ResetState(){
		isLoading = false;
        // SetScreenOrientation();
		UnRegisterLockUIEvent();
		SwapEventCBs.Clear();
    }
}
