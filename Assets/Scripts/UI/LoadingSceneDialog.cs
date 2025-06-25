using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Libs;
using UnityEngine.UI;
using TMPro;
using Random = UnityEngine.Random;
public class LoadingSceneDialog : MonoBehaviour
{
  	public Slider slider;
	public Text percentTxt;
	private TextMeshProUGUI showText;

	private bool updateFailed = false;
	public double appLoadTime{ set; get;}
	private int currentProgress;
	private GameObject landscapePanel;
	private GameObject portraitPanel;
	private GameObject activePanel;
	void Awake()
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
		BaseGameLoadManager.Instance.Init();
		Messenger.AddListener<int>(GameConstants.FIRST_GO_LOBBY_PROGRESS, loadingHandler);
		Messenger.Broadcast(GameConstants.HIDE_DEFAULT_LOAING_IMAGE);
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
				this.OnDestroy();
			}).Play();
		}
	}
	void OnDestroy ()
	{
		Messenger.RemoveListener<int> (GameConstants.FIRST_GO_LOBBY_PROGRESS, loadingHandler);
		Destroy(this);
		if(updateFailed)
			return;
		double loadTime = TimeUtils.GetStartUpRunTime() - appLoadTime;
		Dictionary<string,object> parameters1 = new Dictionary<string, object>();
		parameters1.Add("duration",loadTime);
		BaseGameConsole.ActiveGameConsole().LogBaseEvent("AppLoadTime",parameters1);
		// RenderLevelSwitchMgr.Instance.SetRenderLevel(loadTime);
		// CommandTriggerManager.Instance.CheckMomentConditions (GameConstants.ENTER_LOBBY);
	}
}
