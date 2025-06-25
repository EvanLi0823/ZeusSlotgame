using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Libs;
using UnityEngine.UI;
using TMPro;
using Random = UnityEngine.Random;

public class FirstToLobbyDialog : UIDialog {

	public Slider slider;
	public Text percentTxt;
	private TextMeshProUGUI showText;

	private bool updateFailed = false;
	public double appLoadTime{ set; get;}
	private int currentProgress;
	private GameObject landscapePanel;
	private GameObject portraitPanel;
	private GameObject activePanel;
	protected override void Awake()
	{
		updateFailed = false;
		
		portraitPanel = Util.FindObject<GameObject>(transform, "Anchor_Portrait/");
		landscapePanel = Util.FindObject<GameObject>(transform, "Anchor_Landscape/");
		portraitPanel.SetActive(Screen.orientation == ScreenOrientation.Portrait);
		landscapePanel.SetActive(Screen.orientation != ScreenOrientation.Portrait);

		if (Screen.orientation == ScreenOrientation.Portrait)
		{
			activePanel = portraitPanel;
		}
		else
		{
			activePanel = landscapePanel;
		}
		appLoadTime = TimeUtils.GetStartUpRunTime();
		slider = Util.FindObject<Slider>(activePanel.transform, "Panel/Slider/");
		slider.value = 0f;
		percentTxt = Util.FindObject<Text>(activePanel.transform,  "Panel/TextPercent/");
		percentTxt.text = "0%";
		base.Awake();
		BaseGameLoadManager.Instance.Init();
		Messenger.AddListener<int>(GameConstants.FIRST_GO_LOBBY_PROGRESS, loadingHandler);
		Messenger.Broadcast(GameConstants.HIDE_DEFAULT_LOAING_IMAGE);
	}
	

	IEnumerator RefreshShowText()
	{
		showText = Util.FindObject<TextMeshProUGUI>(transform, GameConstants.LOADING_BASE_PATH + "ShowText/");
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
	

	private void loadingHandler(int progress)
	{
		if(updateFailed)
			return;
		currentProgress = progress;
		slider.value = progress / 100f;
		percentTxt.text = string.Format ("{0}%", currentProgress);
		if (progress == 100) {
			Messenger.RemoveListener<int> (GameConstants.FIRST_GO_LOBBY_PROGRESS, loadingHandler);
			new DelayAction(0.5f,null, () =>
			{
				this.Close();
			}).Play();
		}
	}

	protected override void OnDestroy ()
	{
		Messenger.RemoveListener<int> (GameConstants.FIRST_GO_LOBBY_PROGRESS, loadingHandler);
		base.OnDestroy ();
		if(updateFailed)
			return;
		double loadTime = TimeUtils.GetStartUpRunTime() - appLoadTime;
		Dictionary<string,object> parameters1 = new Dictionary<string, object>();
		parameters1.Add("duration",loadTime);
		BaseGameConsole.ActiveGameConsole().LogBaseEvent("AppLoadTime",parameters1);
		// RenderLevelSwitchMgr.Instance.SetRenderLevel(loadTime);
		// CommandTriggerManager.Instance.CheckMomentConditions (GameConstants.ENTER_LOBBY);
	}

	public readonly static string[] LoadingShowtextArry =
	{
		"Cleaning the volcanic ash","Talking to the Pharaoh","Hunting the Mammoths","Making a wish to the Genie","Cracking the Bank Safe",
		"Climbing the Fortune Tree","Preparing for Diamond Rush","Mowing in the farm","Tanning with Hawaii Girls","Feeding the Puppies",
		"Training the Dragons","Starting the Monster Party","Chasing the Buffaloes","Looking for Pirate’s Treasure","Planting more Mexico Chili",
		"Releasing the 3 Dragons","Locating the Mayan Temple","Finding out the Sakura Secret","Applying Passport for Cat World","Dressing for Samba Dance",
		"Saying Hi to Frog Prince","Arresting the Crazy Scientist"
	};
}
