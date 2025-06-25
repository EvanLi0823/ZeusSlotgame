using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class RateMachineRender:MonoBehaviour
{
	private Image HasRateImage;
	private TextMeshProUGUI HasRateTxt;
	private TextMeshProUGUI HasUnRateTxt;
	void Awake()
	{			
		this.HasRateImage = Util.FindObject<Image> (transform, "ImageChoose/");
//		this.HasRateTxt = Util.FindObject<TextMeshProUGUI> (transform, "TxtHasChoose/");
//		this.HasUnRateTxt = Util.FindObject<TextMeshProUGUI> (transform, "TxtUnChoose/");
		Reset ();
	}

	public void Reset()
	{
		this.HasRateImage.gameObject.SetActive (false);
	}

	public void ShowRate(bool isShow)
	{
		if (isShow) {
			this.HasRateImage.gameObject.SetActive (true);
//			this.HasRateTxt.gameObject.SetActive (true);
//			this.HasUnRateTxt.gameObject.SetActive (false);
		} else {
			this.HasRateImage.gameObject.SetActive (false);
//			this.HasRateTxt.gameObject.SetActive (false);
//			this.HasUnRateTxt.gameObject.SetActive (true);
		}
	}
}
