using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Classic;
public class FreeSpinTxtPanel : MonoBehaviour {
	//0为normal，1为带有倍率，2为util停止
	enum FreeSpinShowType
	{
		Normal = 0,
		Multiple = 1,
		UtilOther = 2
	}

	private readonly string FREESPIN_TXT_MULTIPLE= "X{0}";
	public GameObject m_BG;
	public GameObject m_NormalPanel;
	public GameObject m_FullSpinPanel;
	public GameObject m_MultiplePanel;

	public TextMeshProUGUI m_NormalSpinTxt;
	public TextMeshProUGUI m_MultipleSpinTxt;
	public TextMeshProUGUI m_MultipleCountTxt;

	private int LeftTime,  TotalTime, Multiple, Type;
	void Awake()
	{
		m_BG.SetActive (false);
		Messenger.AddListener<int,int,int> (SlotControllerConstants.OnFreespinTimesChanged, RefreshUI);
		Messenger.AddListener (SlotControllerConstants.SLOT_REELMANAGER_INIT,Init);
		Messenger.AddListener (SlotControllerConstants.OnEnterFreespin, Show);
		Messenger.AddListener (SlotControllerConstants.OnQuitFreespin, Hide);
	}

	void OnDestroy()
	{
		Messenger.RemoveListener (SlotControllerConstants.SLOT_REELMANAGER_INIT,Init);
		Messenger.RemoveListener<int,int,int> (SlotControllerConstants.OnFreespinTimesChanged, RefreshUI);
		Messenger.RemoveListener (SlotControllerConstants.OnEnterFreespin, Show);
		Messenger.RemoveListener (SlotControllerConstants.OnQuitFreespin, Hide);
	}

	void Init()
	{
		GameConfigs _gameconfig = BaseSlotMachineController.Instance.reelManager.GetComponent<GameConfigs>();
		if (_gameconfig != null) {
			this.Type = _gameconfig.FreeSpinType;
			if (_gameconfig.FreeSpinType == (int)FreeSpinShowType.Normal) {
				this.m_NormalPanel.SetActive (true);
				this.m_MultiplePanel.SetActive (false);
				this.m_FullSpinPanel.SetActive (false);
			} else if (_gameconfig.FreeSpinType == (int)FreeSpinShowType.Multiple) {
				this.m_MultiplePanel.SetActive (true);
				this.m_FullSpinPanel.SetActive (false);
				this.m_NormalPanel.SetActive (false);
			} else if (_gameconfig.FreeSpinType == (int)FreeSpinShowType.UtilOther) {
				this.m_FullSpinPanel.SetActive (true);
				this.m_MultiplePanel.SetActive (false);
				this.m_NormalPanel.SetActive (false);
			}
		}
	}

	void RefreshUI(int _leftTime,int _totalTime,int _multiple)
	{
		this.LeftTime = Mathf.Max(0, _leftTime);
		this.TotalTime = _totalTime;
		this.Multiple = _multiple;

		if (this.Type == 0) {
			m_NormalSpinTxt.text = string.Format (GameConstants.StringFormat_0_1_Key, this.LeftTime, this.TotalTime);
		} else if (this.Type == 1) {
			this.m_MultipleSpinTxt.text = string.Format (GameConstants.StringFormat_0_1_Key, this.LeftTime, this.TotalTime);
			this.m_MultipleCountTxt.text = string.Format (FREESPIN_TXT_MULTIPLE, this.Multiple);
		}
	}

	void Show()
	{
		m_BG.SetActive (true);
	}

	void Hide()
	{
		m_BG.SetActive (false);
	}
}
