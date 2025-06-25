using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Libs;
using TMPro;
using App.SubSystems;

namespace Classic
{
public class SecondChanceDialog : SecondChanceDialogBase 
{
        /*
		private CoinOrderMeta orderMeta;

     	protected override void Awake()
    	{
			List<CoinOrderMeta> results = Plugins.Configuration.GetInstance ().ConfigurationParseResult ().ExtraCoinOrderMetas ();
			List<CoinOrderMeta> resultsTemp = new List<CoinOrderMeta> ();
			foreach (CoinOrderMeta meta in results) {
				if (meta.Tag ==6) {
					resultsTemp.Add (meta);
				}
			}
			orderMeta = resultsTemp[Random.Range(0,resultsTemp.Count)];
			base.Awake ();
    	}

   		public override void OnButtonClickHandler(GameObject go)
    	{
			base.OnButtonClickHandler (go);
			Analytics.GetInstance().LogEvent(Analytics.SecondChange, "pay", this.orderMeta.ProductIdentifier);
            StartCoroutine( Payment.GetInstance ().BuyCoins (orderMeta.Coins, orderMeta.Price, orderMeta.ProductIdentifier,0,"NONE"));
    	}

		protected override void Start ()
		{
			base.Start ();
			Messenger.AddListener (GameConstants.OnCoinPaymentSucceeded, AddExtraBalance);
		}

		public override void Refresh ()
		{
			base.Refresh ();
			this.DescText.text = " Make a $" +this.orderMeta.Price +  " purchase to unlock 2nd Chance Double Up for 1 hour!";
			this.CoinsText.text = string.Format("{0:0,0}",this.orderMeta.Coins);
			this.ForMoney.text = string.Format("for $"+this.orderMeta.Price);
			this.PlusText.text = this.orderMeta.Addition*100 + "%";
			Dictionary<string,object> parameters = new Dictionary<string,object> ();
			parameters.Add (PAY, this.orderMeta.ProductIdentifier);
			parameters.Add (TYPE,Analytics.SecondChange);
			parameters.Add (GameConstants.CPM_SHOW_TIMES, UserManager.GetInstance ().UserProfile ().SpecialOfferShowTime);
			BaseGameConsole.ActiveGameConsole ().LogBaseEvent (Analytics.SpecialOffer,parameters);
			Analytics.GetInstance().LogEvent(Analytics.SpecialOffer,parameters);
			UserManager.GetInstance ().UserProfile ().AddSpecialOfferShowTime ();
		}

		protected override void OnDestroy (){
			Messenger.RemoveListener (GameConstants.OnCoinPaymentSucceeded, AddExtraBalance);
			base.OnDestroy ();
		}

		public void AddExtraBalance()
		{
			//			UserManager.GetInstance ().UserProfile ().IncreaseBalance (orderMeta.Coins);
//			Messenger.Broadcast<Transform>(GameConstants.CollectBonus, GoToStoreButton.transform);
            Messenger.Broadcast<Transform, Libs.CoinsBezier.BezierType, System.Action>(GameConstants.CollectBonusWithType, GoToStoreButton.transform, Libs.CoinsBezier.BezierType.Test, null);
			Dictionary<string,object> parameters = new Dictionary<string,object> ();
			parameters.Add (PAY, this.orderMeta.ProductIdentifier);
			parameters.Add (TYPE, "SecondChange");
			parameters.Add ("price", this.orderMeta.Price);
			parameters.Add ("CurrentBet", UserManager.GetInstance().UserProfile().CurrentBet);
			parameters.Add (GameConstants.CPM_SHOW_TIMES, UserManager.GetInstance ().UserProfile ().SpecialOfferShowTime);
			BaseGameConsole.ActiveGameConsole ().LogBaseEvent (Analytics.SpecialOfferBuySucceed,parameters);
			Analytics.GetInstance().LogEvent(Analytics.SpecialOfferBuySucceed,parameters);
			DoubleUpManager.isBuySucceeded = true;
			this.Close();
		}

		private  readonly string PAY = "pay";
		private  readonly string TYPE = "type";
		*/
	}
}
