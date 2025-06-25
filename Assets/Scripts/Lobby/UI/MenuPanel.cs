using System.Collections.Generic;
using Libs;
using Classic;
using Core;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuPanel : MonoBehaviour
{
	public Button m_MenuBtn;
	public Transform menuPanel;
	public Animator m_MenuBtnAnimator;
	public GameObject m_PayTableBtn;
	public Image menuBg;
	public Sprite m_SlostMenuSprite;

	private Vector3 closeVector = Vector3.zero;
	private Vector3 showVector = new Vector3(1f, 1f, 1f);
	public bool IsShowed;
	private bool IsClosing;

	private Tweener tweener;
	private Vector3 m_FAQBtnInitPosition;
	private Vector3 m_LiveChatBtnInitposition;

	void Awake()
	{
		Messenger.AddListener (GameConstants.OnSpinCloseSetting, SpinClose);
		Messenger.AddListener(GameConstants.OnSlotMachineSceneInit, InitMachine);
	}

	void OnDestroy()
	{
		Messenger.RemoveListener (GameConstants.OnSpinCloseSetting, SpinClose);
		Messenger.RemoveListener(GameConstants.OnSlotMachineSceneInit, InitMachine);
	}

	void Start()
	{
		menuPanel.localScale = closeVector;
		CommandManager.Instance.OnCommandUpdateEnd();
	}

	private void InitMachine()
	{
		menuBg.sprite = m_SlostMenuSprite;
		menuBg.SetNativeSize();
		m_PayTableBtn.SetActive(true);
	}

	public void OnClickSetting()
	{
		bool isPortrait = BaseGameConsole.ActiveGameConsole().IsInSlotMachine() &&
		                  SkySreenUtils.CurrentOrientation == ScreenOrientation.Portrait;
		OpenConfigParam<SettingDialog> param = new OpenConfigParam<SettingDialog>(isPortrait,0,OpenType.Normal,"",new SystemUIPopupStrategy());
		UIManager.Instance.OpenSystemDialog(param);
		AudioEntity.Instance.PlayClickEffect();
		Close();
	}

	public void OnClickPayTable()
    {
        Messenger.Broadcast(GameDialogManager.OpenPaytablePanel);
        AudioEntity.Instance.PlayEffectAudio("paytable_open");
        Close();
    }

    public void Close()
	{
        if (IsShowed) 
		{
			Messenger.Broadcast(TopPanel.CLICK_SETTING_CLOSE);
			m_MenuBtnAnimator.SetTrigger("Close");
		}

		Vector3 scaleVector = showVector;

		if (tweener != null) 
		{ 
			tweener.Kill ();
		}

		tweener = DOTween.To(() => menuPanel.localScale, x => scaleVector = x, closeVector, 0.3f).OnUpdate(() =>
				{
					menuPanel.localScale = scaleVector;
				}).OnComplete(() =>
				{
					IsShowed = false;
					IsClosing = false;
					m_MenuBtn.interactable = true;
				}).SetUpdate(true);
	}

    public void Show()
	{
        m_MenuBtn.interactable = false;

        if (IsShowed)
		{
			if(IsClosing) return;
			IsClosing = true;
            Close();
		}
		else
		{
			Messenger.Broadcast(TopPanel.CLICK_SETTING_OPEN);
            m_MenuBtnAnimator.SetTrigger("Open");

			Vector3 scaleVector = closeVector;

			if (tweener != null) 
			{ 
				tweener.Kill ();
			}

			tweener = DOTween.To(() => menuPanel.localScale, x => scaleVector = x, showVector, 0.3f).OnUpdate(() =>
					{
						menuPanel.localScale = scaleVector;
					}).OnComplete(() =>
					{
						IsShowed = true;
						m_MenuBtn.interactable = true;
					}).SetUpdate(true);
		}
	}

	public void SpinClose()
	{
		if (IsShowed)
		{
			if(IsClosing) return;
			IsClosing = true;
            Close();
		}
	}
	
	void Update()
	{
		if (Input.touchCount > 0)
		{
			if (Input.GetTouch(0).phase == TouchPhase.Ended)
			{
				if (m_MenuBtn != null) 
				{
					if (EventSystem.current!= null && EventSystem.current.currentSelectedGameObject == m_MenuBtn) 
					{
						return;
					}
				}
				try{
					Libs.CoroutineUtil.Instance.StartCoroutine(UI.Utils.UIUtil.DelayAction(.1f, delegate
						{
							if (IsShowed && !IsClosing)
							{
								IsClosing = true;
								Close();
							}
						}));
				}
				catch(System.Exception  e)
				{
					Debug.LogError(e);
				}
			}

		}
	}

	public void ResetMenuButton()
	{
		IsShowed = false;
		IsClosing = false;
		m_MenuBtn.interactable = true;
		menuPanel.localScale = Vector3.zero;
		m_MenuBtnAnimator.SetTrigger("Reset");
	}
}
