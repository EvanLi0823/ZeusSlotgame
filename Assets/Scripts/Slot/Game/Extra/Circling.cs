using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using Classic;
[Serializable]
public class Circling : MonoBehaviour {

	private Transform  Wheel;
	public Image[] AllNumImages;
	public WheelElement[] WheelElements;
	private Action<Circling> CallBack;

	[HideInInspector]
	public bool IsLastCircle = true;
	[HideInInspector]
	public bool IsSmallCircle = false;
	[HideInInspector]
	public  int AwardIndex = 0;
	[HideInInspector]
	public  float AwardValue = 0;
	[HideInInspector]
	public int CircleIndex = 0;
	private bool bShowAwardTxt = false;


	protected float timeStep {
		get { return GoldSlotController.Instance.averageFrame.averageTime;}
	}
	public SuperWheelController superWheelController;
	public GameObject m_BlackMask;

	void Awake()
	{
		Wheel = this.transform;
		superWheelController.OnInitGameConfig (Wheel.gameObject.GetComponent<Image>(),OnWheelStartSpin,OnWheelEndSpin);
	}

	public void StartRoll (int resultIndex,Action<Circling> _callBack=null,bool isLastCircle = true,bool IsSmallCircle = false ,float AwardValue = 0,bool bShowAwardTxt = false,int circleIndex = 0)
	{
		this.bShowAwardTxt = bShowAwardTxt;
		this.AwardValue = AwardValue;
		this.AwardIndex = resultIndex;
		this.CallBack = _callBack;
		this.IsLastCircle = isLastCircle;
		this.IsSmallCircle = IsSmallCircle;
		this.CircleIndex = circleIndex;
		//superWheelController.GenResult ();
		//AwardIndex = superWheelController.awardIndex;
		//AwardValue = superWheelController.awardInfo.awardValue;
		superWheelController.awardIndex = AwardIndex;

		HideBlackMask ();

		superWheelController.StartRoll ();
	}

	protected virtual void OnWheelStartSpin()
	{

	}

	protected virtual void OnWheelEndSpin()
	{
		this.ShowBlackMask ();
		if (bShowAwardTxt) {
			this.ShowAwardTxt ();
		}
		else if (this.CallBack != null) {
			CallBack(this);
		}
	}

	public void ShowBlackMask()
	{
		if (m_BlackMask != null) {
			m_BlackMask.SetActive (true);
		}
	}

	public void HideBlackMask()
	{
		if (m_BlackMask != null) {
			m_BlackMask.SetActive (false);
		}
	}

	private void ShowAwardTxt()
	{
		GameObject temp = transform.parent.Find ("WinText").gameObject;
		temp.SetActive (true);
		temp.transform.localScale = Vector3.one;
		temp.GetComponent<TextMeshProUGUI> ().SetText (AwardValue.ToString ());
		temp.transform.DOScale (Vector3.zero, 2f).From ().SetEase (Ease.OutBack).OnComplete (() => {
			temp.SetActive (false);
			if (this.CallBack != null) {
				CallBack(this);
			}
		});
	}

	#region change color
	public void ChangeBgColor(bool isGrey)
	{
		float rgb = isGrey ?  0.5f : 1f;
		Image[] images = transform.parent.GetComponentsInChildren<Image> ();
		foreach (Image image in images) {
			image.color = new Color (rgb, rgb, rgb, 1f);
		}
	}

	#endregion

	public class CircleAward{
		public int CircleAwardIndex = 0;
		public int AwardNum = 0;
		public int FreeSpinTimes = 0;
		public int Times = 1;
		public void Reset()
		{
			this.CircleAwardIndex = 0;
			this.AwardNum = 0;
			Times = 1;
			FreeSpinTimes = 0;
		}
	}

}
