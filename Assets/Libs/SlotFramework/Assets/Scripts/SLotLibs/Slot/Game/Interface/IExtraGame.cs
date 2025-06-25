using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Classic
{
	public interface IExtraGame
	{
		BaseAward GetAwardResult ();
	    
		void PlayAnimation ();

		void Init(Dictionary<string,object> infos,GameCallback onGameOver = null);

        void OnEnterGame (ReelManager reelManager);

        void OnQuitGame(ReelManager reelManager);

		void OnSpin();

		void OnGameEnd();

		void OnBetChanger(long bet);

		void OnSpinEnd();

		GameCallback OnGameOver {
			get;
			set;
		}

		float AnimationDuration {
			get;
			set;
		}

		bool HasAnimation {
			get;
		}

		BaseAward AwardInfo {
			get;
			set;
		}
	}
}
