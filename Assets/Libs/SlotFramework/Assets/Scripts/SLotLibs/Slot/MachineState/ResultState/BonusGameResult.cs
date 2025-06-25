using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Libs;

namespace Classic
{
	public class BonusGameResult : ResultState
	{
		List<List<int>> lastReuslt = new List<List<int>> ();
        public override void Init()
        {
            action = new BaseActionNormal();
            action.AutoPlayNextAction = false;
            action.PlayCallBack.AddStartMethod(Run2);
        }

        public void Run2()

        {
            base.Run ();
			SaveGameResultData ();

            ResultStateManager.Instante.slotController.reelManager.BonusGame.InitBonus(ResultStateManager.Instante.slotController.slotMachineConfig.extroInfos.infos ,OnEndBonusGame);
            
			// 新流程，先播放线奖，再播放Bonus中奖动画
			//有些流程会先处理bonusgame，为避免画线丢掉出现问题，则需要设置NeedRmBgAni为false
	        if (ResultStateManager.Instante.slotController.reelManager.IsNewProcess && ResultStateManager.Instante.slotController.reelManager.BonusGame.NeedRmBgAni)
	        {
		        float time = 0;
		        ReelManager rm = ResultStateManager.Instante.slotController.reelManager;
		        if (rm.ResultData.LineValue > 0) time = rm.gameConfigs.TileAnimationDuration;
		        new DelayAction(time, null, delegate
		        {
			        BaseSlotMachineController.Instance.StopAllAnimation();
			        PlayWinBonusGameAnimation();
					ResultStateManager.Instante.slotController.StopAwardSymbolAnimation();
		        }).Play();
	        }
			else
				PlayWinBonusGameAnimation ();
        }

        #region Restore BonusGame
        public void AddRestoreMethod()
        {
            action = new BaseActionNormal();//将默认的run方法剔除
            action.AutoPlayNextAction = false;
            action.PlayCallBack.AddStartMethod(Restore);
        }

        public BonusGameResult (bool restoreBonusState= false){
			if (restoreBonusState) {
				AddRestoreMethod ();
			} 
			else {
				base.Init ();
			}
		}
		public  void Restore(){
			base.Init ();

//			ResultStateManager.Instante.slotController.needCheckResult = ResultStateManager.Instante.slotController.reelManager.gameConfigs.BonusNeedCheckResult;
			ResultStateManager.Instante.slotController.reelManager.BonusGame.InitBonus(ResultStateManager.Instante.slotController.slotMachineConfig.extroInfos.infos ,OnEndBonusGame);
			ResultStateManager.Instante.slotController.reelManager.BonusGame.OnEnterGame (ResultStateManager.Instante.slotController.reelManager);
			//HIDE TOUR
//			if (ResultStateManager.Instante.slotController.reelManager.BonusGame.NeedHideBannerTournament) {
//				Messenger.Broadcast<bool> (GameConstants.ENABLE_SHOW_TOURNAMENT, false);
//			}
		}
		#endregion
		void SaveGameResultData(){
			lastReuslt.Clear ();
			for (int i=0; i< ResultStateManager.Instante.slotController.reelManager.resultContent.ReelResults.Count; i++) {
				List<int> temp = new List<int> ();
				temp.AddRange (ResultStateManager.Instante.slotController.reelManager.resultContent.ReelResults [i].SymbolResults);
				lastReuslt.Add (temp);
			}
			if (ResultStateManager.Instante.slotController.reelManager.isFreespinBonus) {
				//在reelmanager启动ReelsStartRun()方法内已经对当前结果进行保存了，但是可能有需求在最后转完后，替换symbol，所以双重保险，以切换时，为准；没有转完，
				//则使用在reelmanager启动ReelsStartRun方法里的数据恢复处理
				ResultStateManager.Instante.slotController.reelManager.freeSpinResult = lastReuslt;
			} 
			else {
				ResultStateManager.Instante.slotController.reelManager.baseGameResult = lastReuslt;
			}

		}

		public  override void End ()
		{
			base.End ();
		}

		void OnEndBonusGame()
		{
			ResultStateManager.Instante.slotController.IncreaseBonusGameAward();
			End ();
		}

        void PlayWinBonusGameAnimation()
        {
            ResultStateManager.Instante.slotController.reelManager.BonusGame.PlayWinBonusGameAnimation (()=>{
				ResultStateManager.Instante.slotController.reelManager.BonusGame.StopAudio();
                ResultStateManager.Instante.slotController.reelManager.BonusGame.OnEnterGame (ResultStateManager.Instante.slotController.reelManager);
			//HIDE TOUR
//				if(ResultStateManager.Instante.slotController.reelManager.BonusGame.NeedHideBannerTournament){
//					new DelayAction(0.5f,null,delegate() {
//						Messenger.Broadcast<bool>(GameConstants.ENABLE_SHOW_TOURNAMENT,false);	
//					}).Play();
//				}
			});
        }
	}
}
