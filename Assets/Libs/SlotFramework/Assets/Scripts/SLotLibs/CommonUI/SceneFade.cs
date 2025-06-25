using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using Random = UnityEngine.Random;

public class SceneFade : MonoBehaviour {

	public Image FadeImg;
	public Image ShowLoadingImg;  //fore
	public RectTransform LoadingPanel;
	private Slider SliderLoading;
	public float fadeSpeed = 0.5f;
	public Text txtPercent;
	public TextMeshProUGUI showText;

	public const string ON_SCENE_FADE_OVER = "OnSceneFadeOver";

	private bool isReady = false;

	public RectTransform loadingimage;
	void Awake()
	{
//		LoadingPanel.gameObject.SetActive (false);
		ShowLoadingImg.gameObject.SetActive (false);
		txtPercent.text = "0%";
		SliderLoading = this.transform.Find("Panel/Slider").GetComponent<Slider>();
		SliderLoading.value = 0f;
		SetLoadingImage();
		StartCoroutine(RefreshShowText());
	}
	
	IEnumerator RefreshShowText()
	{
		if (showText != null)
		{
			showText.gameObject.SetActive(true);
			int temp = -1;
			while (true)
			{
				int index = Random.Range(0, LoadingShowtextArry.Length);
				if (temp == index)
				{
					if (index == LoadingShowtextArry.Length-1)
					{
						index--;
					}
					else
					{
						index++;
					}
				}
				temp = index;
				Coroutine cor = StartCoroutine(AppendShowPoint(LoadingShowtextArry[index]));
				yield return GameConstants.FiveSecondWait;
				StopCoroutine(cor);
			}
		}
		
	}

	IEnumerator AppendShowPoint(string tip)
	{
		int count = 0;
		while (true)
		{
			string str = String.Empty;
			for (int i = 0; i < count; i++)
			{
				str += ".";
			}
			yield return GameConstants.FiveIn10SecondWait;
			count++;
			count = count % 4;
			showText.text = tip+str;
			yield return null;
		}
	}
	
//	void Update()
//	{
//		if (isReady) {	
//			isReady = false;
//			FadeImg.DOFade(1,fadeSpeed/2).OnComplete(ShowLogoPanel);	
//		}
//	}
	
	
	void FadeToClear(bool needFadeBack)
	{ 
		Tweener t1 = ShowLoadingImg.DOFade (1f, fadeSpeed/2).OnComplete ( ()=>{
			this.LoadingPanel.gameObject.SetActive(false);
			Libs.UIDialog dialog = transform.parent.GetComponent<Libs.UIDialog>();
			dialog.Close();
			
			if (!needFadeBack) {
				Messenger.Broadcast(ON_SCENE_FADE_OVER);
			}
		}).SetUpdate(true);
		if(needFadeBack){ 
			Tweener t2 = ShowLoadingImg.DOFade (0f, fadeSpeed/2).OnComplete ( ()=>{
				Libs.UIDialog dialog = transform.parent.GetComponent<Libs.UIDialog>();
				dialog.Close();
				Messenger.Broadcast(ON_SCENE_FADE_OVER);
			}).SetUpdate(true);
			
			Sequence seq = DOTween.Sequence();
			seq.Append(t1);
			seq.Append(t2);
			seq.Play();
		}
		else {
			t1.Play();
		}
	}
	
	void ShowLogoPanel()
	{
		FadeImg.gameObject.SetActive(false);
		LoadingPanel.gameObject.SetActive (true);
		ShowLoadingImg.gameObject.SetActive (true);

		ShowLoadingImg.color = new Color (0f, 0f, 0f, 0f); //.DOFade (0,fadeSpeed/2);
	}

	void FadeToBlack()
	{
		ShowLogoPanel ();
//		isReady = true;
//		isReady = false;
//		FadeImg.DOFade(1,fadeSpeed/2).OnComplete(ShowLogoPanel);	
	}
	public IEnumerator PlayerTurn (Image image,float aTime, bool isRevert)
	{
		for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
		{
			Color c = image.color;
			c.a = t;
			image.color = c;

			yield return null;
		}
		
		yield return null;
	}
	

	
	
	public void FadeSceneToBlack(string sceneName=null)
	{
		// Make sure the RawImage is enabled.
		FadeImg.enabled = true;
		
		// Start fading towards black.
		FadeToBlack();
	}


	public void OnCompletedLoad(bool needFadeBack = true)
	{
		this.SliderLoading.value = 1;
		this.txtPercent.text  = "100%";
		FadeToClear (false);
	}

	public void OnLoading (string levelName,float progress)
	{
		SliderLoading.value = progress;
		this.txtPercent.text = (int)(progress *100) + "%";
	}
	
	public readonly static string[] LoadingShowtextArry =
	{
		"Cleaning the volcanic ash","Talking to the Pharaoh","Hunting the Mammoths","Making a wish to the Genie","Cracking the Bank Safe",
		"Climbing the Fortune Tree","Preparing for Diamond Rush","Mowing in the farm","Tanning with Hawaii Girls","Feeding the Puppies",
		"Training the Dragons","Starting the Monster Party","Chasing the Buffaloes","Looking for Pirate’s Treasure","Planting more Mexico Chili",
		"Releasing the 3 Dragons","Locating the Mayan Temple","Finding out the Sakura Secret","Applying Passport for Cat World","Dressing for Samba Dance",
		"Saying Hi to Frog Prince","Arresting the Crazy Scientist"
	};

	public void SetLoadingImage()
	{
		if (loadingimage!=null)
		{
			if (SkySreenUtils.CurrentOrientation == ScreenOrientation.LandscapeLeft )
			{
				if (((float)Screen.width/(float)Screen.height)<2.1f)
				{
					loadingimage.sizeDelta=new Vector2(2339,1080);
				}
				else
				{
					loadingimage.sizeDelta=new Vector2(2460,1080);
				}
				
			}
			else
			{
				loadingimage.sizeDelta=new Vector2(2460,1080);
			}
		}
	}
}
