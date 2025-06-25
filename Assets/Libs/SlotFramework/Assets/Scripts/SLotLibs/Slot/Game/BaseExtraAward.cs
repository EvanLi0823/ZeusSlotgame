using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Libs;
namespace Classic
{
	public class BaseExtraAward : MonoBehaviour,IExtraGame
	{
		public float Duration;
		[HideInInspector]
		public bool NeedRecoveryData = false;
		protected virtual void Awake ()
		{
			AwardInfo = new BaseAward ();
            OnGameAwake();
		}

        protected virtual void OnDestroy()
        {
            OnGameDestroy();
        }

        public virtual void OnGameAwake() {
        }

        public virtual void OnGameDestroy() {
        }

		public virtual void Init (Dictionary<string,object> infos = null, GameCallback onGameOver = null)
		{
			this.OnGameOver = null;
			if (onGameOver != null)
				this.OnGameOver += onGameOver;
//			this.OnGameOver += delegate() {
//				Messenger.Broadcast<bool> (GameConstants.ENABLE_SHOW_TOURNAMENT, true);
//			};
			AwardInfo = new BaseAward ();
		}

        public virtual void OnEnterGame (ReelManager reelManager){
		}

        public virtual void OnQuitGame(ReelManager reelManager){
		}

		public virtual void OnSpin ()
		{
		}

		public virtual BaseAward GetAwardResult ()
		{
			return AwardInfo;
		}
		
		public virtual void PlayAnimation ()
		{
			if (HasAnimation) {
				DelayAction delayAction = new DelayAction (AnimationDuration, null, OnGameEnd);
				delayAction.Play ();
			} else {
				if (OnGameOver != null) {
					OnGameOver ();
				}
			}
		}
		public virtual void StopAudio(){
			AudioEntity.Instance.StopRollingUpEffect ();
			AudioEntity.Instance.StopBigWinRollUpEffect ();
		}
		public virtual void  OnGameEnd ()
		{
			NeedRecoveryData = false;
			if (OnGameOver != null) {
				OnGameOver ();
			}
		}

		public virtual void OnBetChanger (long bet)
		{
		}
	
		public virtual void OnSpinEnd(){
		}

		public GameCallback OnGameOver {
			get;
			set;
		}
		
		public float AnimationDuration {
			get{ return Duration;}
			set{ Duration = value;}
		}
		
		public bool HasAnimation {
			get
			{ return (AnimationDuration != 0) && (AwardInfo.isAwarded); }
		}

		public BaseAward AwardInfo {
			get;
			set;
		}

		/// <summary>
		/// Gets the test log.
		/// testController使用，输出test数据
		/// </summary>
		/// <returns>The test log.</returns>
		public virtual string GetTestLog()
		{
			return "";
		}

		public void Deactivate3DEffect() {
			if (TwistEffect.instance != null) {
				TwistEffect.instance.enabled = false;
			}
			GameObject bannerCameraGO = GameObject.Find ("Main Camera/UICanvas/UIPanel/BannerCanvas/BannerPrefab(Clone)/BannerCamera");
			if (bannerCameraGO != null) {
				bannerCameraGO.SetActive (false);
			}
		}

		public void Reset3DEffect() {
			if (TwistEffect.instance != null) {
				TwistEffect.instance.enabled = true;
			}
			GameObject bannerCameraGO = GameObject.Find ("Main Camera/UICanvas/UIPanel/BannerCanvas/BannerPrefab(Clone)/BannerCamera");
			if (bannerCameraGO != null) {
				bannerCameraGO.SetActive (true);
			}
		}
	}
}
