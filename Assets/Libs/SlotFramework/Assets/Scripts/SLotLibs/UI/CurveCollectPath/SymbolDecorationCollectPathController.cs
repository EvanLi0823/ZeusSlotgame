using UnityEngine;
using System.Collections;
using Classic;

public class SymbolDecorationCollectPathController : MonoBehaviour
{
	private CurveCollectPathController curveCollectPathController;
	private GameObject middlePanel;
	private GameObject middleReel;
	private ReelManager reelmanager;
	private BaseElementPanel middlePalelElement;

	public TaskType TaskType;
	public bool isNormal = false;
	public bool isWild = false;
	public bool isBonus = false;
	public bool isFreespin = false;

	void Awake()
	{
		Messenger.AddListener<BaseElementPanel>(GameConstants.CollectSymbolDecorationTask, SetCurrentElement);
	}
	void Destroy()
	{
		Messenger.RemoveListener<BaseElementPanel>(GameConstants.CollectSymbolDecorationTask, SetCurrentElement);
	}
	void Start()
	{
		if (curveCollectPathController == null)
			curveCollectPathController =  Util.FindObject<CurveCollectPathController>(transform,"CollectPathController/");
		SetPanel ();
	}

	private void SetPanel()
	{
		if (BaseSlotMachineController.Instance == null) return;
		if (reelmanager == null) reelmanager = BaseSlotMachineController.Instance.reelManager;
		if (middlePanel == null) middlePanel = reelmanager.gameObject;
		if (middleReel == null) middleReel = reelmanager.Reels [reelIndex].gameObject;
	}

	

	private void SetCurrentElement(BaseElementPanel elementPanel)
	{
		//Debug.Log ("TaskType  " + TaskType);
		SetPanel ();
		if (reelmanager == null || middleReel == null || middlePanel == null || curveCollectPathController == null) return;
		switch (TaskType)
		{
			case TaskType.MiddlePanle:
			case TaskType.MiddleReel:
				middlePalelElement = CreateElementPanel ();
				OnPlayCollectPathMsg (middlePalelElement);
				break;
			case TaskType.Normal:
				OnPlayCollectPathMsg (elementPanel);
				break;
			default:
				break;
		}
	}

	protected void OnPlayCollectPathMsg(BaseElementPanel elementPanel)
	{
	}

	private BaseElementPanel CreateElementPanel()
	{
		BaseElementPanel element = new BaseElementPanel ();
		element = reelmanager.Reels [reelIndex].GetMiddleElementRender ();

		return element;
	}
		
	int reelIndex
	{
		get
		{ 
			if (reelmanager.Reels.Count % 2 == 0) 
			{
				return (reelmanager.Reels.Count / 2) - 1;
			}
			else 
			{
				return reelmanager.Reels.Count / 2;
			}
		}
	}
}

public enum TaskType
{
	Normal,
	MiddlePanle,
	MiddleReel
}


