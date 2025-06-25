using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Libs;
using TMPro;

namespace Classic
{
	public class RateMachineDialog : RateMachineDialogBase 
	{
		private string slotName;
		RateMachineRender render1;
		RateMachineRender render2;
		RateMachineRender render3;
		RateMachineRender render4;
		RateMachineRender render5;
		private List<RateMachineRender> renders;
		private int m_RateLevel = 0;
		private int awardCoins = 500;
		private string titleText1, titleText2, okButtonText;
		private const string PIC_PATH ="Prefab/Atlas/MachineUnlock/";
       
     	protected override void Awake()
    	{
       		base.Awake();
			awardCoins = Plugins.Configuration.GetInstance().GetValueWithPath<int>("Module/RateMachineConfig/AwardCoin", 500);
			titleText1 = Plugins.Configuration.GetInstance().GetValueWithPath<string>("Module/RateMachineConfig/TitleText1", "How much fun was");
			titleText2 = Plugins.Configuration.GetInstance().GetValueWithPath<string>("Module/RateMachineConfig/TitleText2", "this slot machine?");
			okButtonText = Plugins.Configuration.GetInstance().GetValueWithPath<string>("Module/RateMachineConfig/OkButtonText", "<sprite=\"goldCoin\" index=0>Collect ");

            text_up.text = titleText1;
			text_down.text = titleText2;

			render1 = Button1.GetComponent<RateMachineRender> ();
			render2 = Button2.GetComponent<RateMachineRender> ();
			render3 = Button3.GetComponent<RateMachineRender> ();
			render4 = Button4.GetComponent<RateMachineRender> ();
			render5 = Button5.GetComponent<RateMachineRender> ();

			renders = new List<RateMachineRender>();
			renders.Add (render1);
			renders.Add (render2);
			renders.Add (render3);
			renders.Add (render4);
			renders.Add (render5);
			this.MoveSlider.onValueChanged.AddListener (delegate(float v) {
				int rateNumber = Mathf.Min(5, Mathf.Max (1, (int)Mathf.Floor (v * 5+1)));
				ClickStarHandler (rateNumber);
			});
			Messenger.Broadcast (SlotControllerConstants.AUTO_SPIN_SUSPEND);
        }
    
        bool register = false;
		private void ClickStarHandler(int currentRateLevel)
		{
			this.m_RateLevel = currentRateLevel;
			this.BtnOk.interactable = true;
			UIButtonScale buttonScale = this.BtnOk.GetComponent<UIButtonScale>();
			if (buttonScale) {
				buttonScale.enabled = true;
				buttonScale.scaleXOnPressed = buttonScale.scaleYOnPressed = 1.1f;
			}

			for (int i = 0; i < currentRateLevel; i++) {
				renders [i].ShowRate (true);
			}
			for (int i = currentRateLevel; i < renders.Count; i++) {
				renders [i].ShowRate (false);
			}
		}

   		public override void OnButtonClickHandler(GameObject go)
    	{
    	    base.OnButtonClickHandler(go);
			if (go == BtnOk.gameObject) {
                if (BtnOk.IsInteractable()) {
                    Dictionary<string, object> analyticsParameters = new Dictionary<string, object>();
					analyticsParameters[Analytics.RateMachineName] = slotName;
					analyticsParameters[Analytics.RateLevel] = this.m_RateLevel;
                    Analytics.GetInstance().LogEvent(Analytics.RateMachine, analyticsParameters);
					UserManager.GetInstance().UserProfile().RateMachine(slotName);

                    Messenger.Broadcast<Transform, Libs.CoinsBezier.BezierType, System.Action>(GameConstants.CollectBonusWithType, BtnOk.transform, Libs.CoinsBezier.BezierType.EmailBonus, null);


					//Give coin
                    UserManager.GetInstance().IncreaseBalanceAndSendMessage(awardCoins);

					//Record coin-giving event
					Dictionary<string,object> dict = new Dictionary<string,object>();
					dict.Add("increaseCoins", awardCoins);
					dict.Add(Analytics.RateMachineName,slotName);
					dict.Add(Analytics.RateLevel,this.m_RateLevel);
					BaseGameConsole.singletonInstance.LogBaseEvent("RateMachine",dict);

					//Record poped time
					UserManager.GetInstance().UserProfile().SetRatingDialogPopedTime(slotName, UserManager.GetInstance().UserProfile().GetRatingDialogPopedTime(slotName) + 1);

					//Visual&Audio effect
					AudioEntity.Instance.PlayCoinCollectionEffect ();
					Messenger.Broadcast (SlotControllerConstants.AUTO_SPIN_RESUME);
                    this.Close();
                }
			}
			else if (go == BtnClose1.gameObject) {
				//Record poped time
				UserManager.GetInstance().UserProfile().SetRatingDialogPopedTime(slotName, UserManager.GetInstance().UserProfile().GetRatingDialogPopedTime(slotName) + 1);

				this.Close();
			}
			else {
				m_RateLevel = renders.IndexOf (go.GetComponent<RateMachineRender> ()) +1;
				ClickStarHandler (m_RateLevel);
			}
    	}

		public override void Refresh ()
		{
			base.Refresh ();
			this.slotName = this.m_Data as string;
            this.BtnOk.interactable = false;
            UIButtonScale buttonScale = this.BtnOk.GetComponent<UIButtonScale>();
            if (buttonScale) {
                buttonScale.enabled = false;
				buttonScale.scaleXOnPressed = buttonScale.scaleYOnPressed = 1;
            }
			this.LabelAward.text = okButtonText +" "+ Utils.Utilities.ThousandSeparator (this.awardCoins);
        }
	}
}
