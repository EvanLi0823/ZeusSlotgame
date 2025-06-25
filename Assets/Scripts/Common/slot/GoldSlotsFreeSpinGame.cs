using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Libs;
namespace Classic{
	public class GoldSlotsFreeSpinGame : FreespinGame {

		protected override void ShowWinDialogHandler(float delayTime,long winCoins,System.Action callback,int cash){
			DelayAction delayAction = new DelayAction(delayTime, null,
				() => {
					PlayExitAudio();
					Dictionary<string, object> data = new Dictionary<string, object>();
					data[GameConstants.WinCoins_key] = winCoins;
					data[GameConstants.WinCash_key] = cash;
					data[GameConstants.FreeSpinCount_key] = TotalTime;
					Messenger.Broadcast<Dictionary<string, object>, System.Action,bool>(GameDialogManager.OpenFreeSpinEndDialog,data, () => {
						callback();
					},true);
                    BeforeShowWinDialogHandler();
                });

			delayAction.Play();
		}

        protected virtual void BeforeShowWinDialogHandler()
        {

        }


        public override void showFirstWinFreespin (int times, System.Action callback)
		{
			PlayEnterAudio ();
			DelayAction delayAction = new DelayAction (1f, null, BaseSlotMachineController.Instance.StopAllAnimation);
			delayAction.Play ();
			Messenger.Broadcast<int,System.Action,bool,bool> (GameDialogManager.OpenAutoSpinWinDialog, times, () => {
				callback ();
			}, true,true);
		}

		public override void showNextWinFreespin (int times, System.Action callback)
		{
			DelayAction delayAction = new DelayAction (1f, null, BaseSlotMachineController.Instance.StopAllAnimation);
			delayAction.Play ();
			AudioEntity.Instance.PlayFreeSpinAddEffect ();
			Messenger.Broadcast<int,System.Action,bool> (GameDialogManager.OpenAutoSpinAdditionalDialog, times, () => {
				callback ();
			},true);
		}
	}
}


