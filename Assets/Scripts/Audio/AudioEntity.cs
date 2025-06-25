using UnityEngine;
using System.Collections;
using System;
using Classic;

namespace Libs
{
    public class AudioEntity
    {
        private static AudioEntity _instance;

        public static AudioEntity Instance 
        {
            get 
            {
                if (_instance == null) _instance = new AudioEntity ();
                return _instance;
            }
        }

        public static AudioEntity ActiveAudioEntity ()
        {
            return SetAudioEntityClass<AudioEntity> ();
        }

		public static T SetAudioEntityClass<T> () where T: AudioEntity
		{
			if (_instance==null)  _instance = Activator.CreateInstance<T> ();
			return _instance as T;
		}

        public bool EnableSound 
        {
            get
            {
                if (AudioManager.Instance != null)
                {
                    return AudioManager.Instance.EnableSound;
                }
                else return true;
            }
            set 
            {
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.EnableSound = value;
                }
            }
        }
        
        protected string audioOpenClick = "open_dialog";                                          //LQ 点击设置等相关按钮，在弹出的窗口中点击相应按钮如关闭按钮等触发的声音
        protected string audioCloseClick = "close_dialog";   
		protected string audioBet ="Bet";
		protected string audioLobbyBg = "Hall";                                         //LQ Lobby大厅内的背景音乐
        protected string audioCoinCollection = "CoinCollection";                        //LQ 点击购买按钮，在弹出的信用商店，点击购买条目时，播放的声音 Hourly Bonus 金币收集声效
        protected string audioBonus = "Coin2";                                          //LQ Lobby Hourly Bonus 准备好收集金币时，自动触发的声音事件
        protected string audioLevelup = "Levelup";                                      //LQ 用户玩家升级时，播放的声效
        //machine
        public static string audioSpin = "Spin";                                            //LQ 单击Spin按钮时，播放的音效
        protected string audioRollingUp = "RollingUp";                                  //LQ 用户中奖后，金币累加上涨的时候，播放的声效。
        protected string audioRollingEnd = "RollingEnd";                                //LQ 用户中奖后，金币涨停的时候，播放的声效。
        protected string audioReelStop = "ReelStop";                                    //LQ 转轮停止旋转的时候播放的音效
//        protected string audioReelStop1 = "ReelStop1";                                    //LQ 转轮停止旋转的时候播放的音效
//        protected string audioReelStop2 = "ReelStop2";                                    //LQ 转轮停止旋转的时候播放的音效
//        protected string audioReelStop3 = "ReelStop3";                                    //LQ 转轮停止旋转的时候播放的音效
//        protected string audioReelStop4 = "ReelStop4";                                    //LQ 转轮停止旋转的时候播放的音效
//        protected string audioReelStop5 = "ReelStop5";                                    //LQ 转轮停止旋转的时候播放的音效
        protected string audioAnticipationSound = "Anticipation-sound";                 //LQ 同smartsound
        protected string audioBonusTrigger = "BonusTrigger";                             //LQ 触发Bonus时，播放的音效

        public static string audioSpecial = "Special";                                      //LQ 在FreeSpin界面下，播放的背景音乐。
        protected string audioMultiplier = "Multiplier"; //scene 4                      //LQ goldslots 没有调用
        protected string audioWhelStop9 = "WheelWin"; //9#                              //LQ goldslots 没有调用
        protected string audioWheelStopAndWin = "WheelStopAndWin";                      //LQ Add 在用户中奖的时候，和中奖Symbol一起播放的音效，只播一遍。

        protected string audioSmallWin = "SmallWin";
        protected string audioMidWin = "MidWin";
        protected string audioBigWin = "BigWin";
        protected string audioTopWin = "TopWin";

        protected string audioLevelUpCoinsCollection = "LevelUpCoinsCollection";        //LQ Add 升级之后点领取金币奖励的音效
        protected string audioBonusOrFreeSpinGainCoins = "BonusOrFreeSpinGainCoins";    //LQ Add BONUS，FREESPIN弹出结算时音效
        //protected string audioBigWin = "BigWin";                                        //LQ Add BONUS，BigWin

        //goldslots 3Tiggers
        protected string audioEyeShoot = "eye_shoot";
        protected string audioBonusReTrigger = "BonusReTrigger";
        protected string audioFreespinPopupBackground_loop = "free_spins_pop_up_background_loop";
        protected string audioFreespinEnd = "free_spins_end";
		protected const string audioCollectGenerate = "collect_generate";

        protected string audioFreespinSelectionMade = "free_spins_selection_made";
        protected string audioProgressivePopup = "progressive_pop_up";
		protected string AUDIO_VIP_ROOM = "VIP_Background";
		protected string AUDIO_FREESPIN_ADD = "FreespinAdd";
		protected string AUDIO_JACKPOT_RETRIGER = "JackpotTrigger";
        protected string COMMON_JACKPOT_WIN = "CommonJackpotWin";

        public const string audioGiftBoxFly = "GiftBox_fly";
        public const string audioGiftBoxDown = "GiftBox_down";
        public const string audioGiftBoxOpen = "GiftBox_open";
        public const string audioGiftBoxCollectCoin = "giftboxcolletcoin";
        public const string audioGiftBoxCollectBtn = "giftboxcollectbtn";
        public const string audioTaskUltimateAward = "taskUltimateAward";
        public const string audioDailyQuestRewardShow = "DailyQuestRewardShow";
        public const string audioDailyQuestRewardFly = "DailyQuestRewardFly";

        public const string audioQiandaiEnd = "qiandai_end";
        public const string audioQiandaiStart = "qiandai_start";

        public const string audio_gliding = "audio_gliding";


        public void PlayJackpotTrigger()
		{
			AudioManager.Instance.AsyncPlayEffectAudio (AUDIO_JACKPOT_RETRIGER);
		}
		public void PlayFreeSpinAddEffect()
		{
			AudioManager.Instance.AsyncPlayEffectAudio(AUDIO_FREESPIN_ADD);
		}
        public void PlayLevelUpCollectCoinsEffect()
        {
            AudioManager.Instance.AsyncPlayEffectAudio (audioLevelUpCoinsCollection);
        }

		public void StopSpinAudio(float fadeOut=0){
			AudioManager.Instance.StopMusicAudio (audioSpin,fadeOut);
		}
        public void PlayBonusOrFreeSpinGainCoinsEffect()
        {
            AudioManager.Instance.AsyncPlayEffectAudio (audioBonusOrFreeSpinGainCoins);
        }
      
        public void  PlayClickEffect ()
        {
            AudioManager.Instance.AsyncPlayEffectAudio (audioOpenClick);
        }

        public void  PlayCloseClickEffect ()
        {
            AudioManager.Instance.AsyncPlayEffectAudio (audioCloseClick);
        }

        public void StopSlotsBackGroundMusic()
        {
            AudioManager.Instance.StopMusicAudio (audioSpin);
            AudioManager.Instance.StopMusicAudio (audioSpecial);
        }

		public void PlayChangeBetEffect(int level){
			if (level==0) {
				AudioManager.Instance.AsyncPlayEffectAudio (audioBet+"-no");
			}
			else if (level >= 1 && level <= 11) {
				AudioManager.Instance.AsyncPlayEffectAudio (audioBet+level.ToString());
			} else if (level>11) {
				AudioManager.Instance.AsyncPlayEffectAudio (audioBet+"-max");
			} 
		}

        public void PlayLevelupEffect()
        {
            AudioManager.Instance.AsyncPlayEffectAudio (audioLevelup);//LQ 忽略循环延迟
        }

        public void PauseBackGroundAudio(string name, float time = 0, float value = 1f)
		{
            AudioManager.Instance.PauseMusicAudio(name, time, value);
		}

        public void UnPauseBackGroundAudio(string name, bool loop = true, float value = 1, float time = 0)
		{
            AudioManager.Instance.UnPauseMusicAudio(name, loop, value, time);
		}
        public void SetBackGroundAudioVolume(string name, float time = 0, float value = 1f)
        {
            AudioManager.Instance.SetMusicAudioVolum(name, time, value);
        }
        
        public void PlayAwardSymbolAudio(string name)
        {
            AudioManager.Instance.AsyncPlayEffectAudio(name, false);
        }

		public void StopAwardSymbolAudio()
        {
            AudioManager.Instance.StopEffectAudio ("bet_1");
            AudioManager.Instance.StopEffectAudio ("bet_1_5");
            AudioManager.Instance.StopEffectAudio ("bet_5");
        }

        public void PlayCollisionGenerateEffect()
		{
			AudioManager.Instance.AsyncPlayEffectAudio (audioCollectGenerate);
		}

        public void PlayCoinCollectionEffect()
        {
            AudioManager.Instance.AsyncPlayMusicAudio(audioCoinCollection, false);
        }

        public void StopCoinCollectionEffect()
        {
            AudioManager.Instance.StopMusicAudio(audioCoinCollection, 0.5f);
        }

        public void PlayEnterRoomEffect()
        {
			AudioManager.Instance.AsyncPlayEffectAudio (audioOpenClick);
        }

        public void PlayBonusReadyEffect()
        {
            AudioManager.Instance.AsyncPlayEffectAudio (audioBonus);
        }

		public void PlayRollUpEffect(float volume = 1f,bool loop = true)
        {
			AudioManager.Instance.AsyncPlayEffectAudio(audioRollingUp,loop,volume);
        }

		public virtual void StopBigWinRollUpEffect(){
                StopRollingUpEffect ();
		}

        public void PlayRollEndEffect()
        {
            AudioManager.Instance.AsyncPlayEffectAudio (audioRollingEnd);
        }

		public void PlayReelStopEffect(int reelIndex = -1,bool loop = false,float volume = 1.0f)
        {
            //20191226 每个reel的音效播放相同
            AudioManager.Instance.AsyncPlayEffectAudio (audioReelStop,loop,volume,false);
        }

        public void PlaySmartSoundEffect(int index , float volume = 1.0f)
        {
			PlayEffect (string.Format("SmartSound{0}",index), false, volume);
        }

        public void PlayAnticipationSoundEffect()
        {
            AudioManager.Instance.AsyncPlayEffectAudio(audioAnticipationSound);
        }
        public void StopAnticipationSoundEffect()
        {
            AudioManager.Instance.StopEffectAudio(audioAnticipationSound);
        }
        public void PlayBonusTriggerEffect()
        {
            AudioManager.Instance.AsyncPlayEffectAudio (audioBonusTrigger);
        }
        public void PlayBonusRetriggerEffect(){
            AudioManager.Instance.AsyncPlayEffectAudio (audioBonusReTrigger);
        }

        public void PlayEyeShootEffect(){
            AudioManager.Instance.AsyncPlayEffectAudio (audioEyeShoot);
        }
        public void PlayBonusGameBackgroundLoopEffect(){
            AudioManager.Instance.AsyncPlayEffectAudio (audioFreespinPopupBackground_loop);
        }
        public void PlayFreespinEndEffect(){
            AudioManager.Instance.AsyncPlayEffectAudio (audioFreespinEnd);
        }
        public void PlayEnterFreeSpinClickEffect(){
            AudioManager.Instance.AsyncPlayEffectAudio (audioFreespinSelectionMade);
        }
        public void PlayProgressivePopupEffect(){
            AudioManager.Instance.AsyncPlayEffectAudio (audioProgressivePopup);
        }

        public void PlayMultiplierEffect()
        {
            AudioManager.Instance. AsyncPlayEffectAudio (audioMultiplier);
        }

        public void PlayWhellStop9Effect()
        {
            AudioManager.Instance.AsyncPlayEffectAudio (audioWhelStop9);
        }

		public void StopMusicAudio(string musicName,float fadeoutTime = 0f){
			AudioManager.Instance.StopMusicAudio (musicName,fadeoutTime);
		}
		public void PlayEffectAudio(string effectName){
			AudioManager.Instance.AsyncPlayEffectAudio (effectName);
		}
		public void StopEffectAudio(string effectName){
			AudioManager.Instance.StopEffectAudio (effectName);
		}
        public void PlayFreeSpinBackgroundMusic(float volume = 1.0f)
        {
            if (BaseGameConsole.singletonInstance.IsInLobby()) {
                return;
            }
            AudioManager.Instance.AsyncPlayMusicAudio (audioSpecial, true, volume);
        }
		public void StopFreeSpinBackgroundMusic()
		{
			AudioManager.Instance.StopMusicAudio (audioSpecial);
		}
		public void PlayMuisc(string audioName,bool loop = true,float volume = 1.0f){
			AudioManager.Instance.AsyncPlayMusicAudio (audioName,loop,volume);
        }

		public void PlayEffect(string audioName,bool loop = false,float volume = 1.0f){
			AudioManager.Instance.AsyncPlayEffectAudio (audioName,loop,volume);
        }

        public void PlayWheelStopAndWinSoundEffect(int winType = -1)
        {
            //LQ 声音文件属于音效标准没定好，在此临时使用此方法代码应急处理
            string audioScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene ().name;
            float volume = 1f;
            switch (audioScene) {
            case "KingArthur":
                {volume = 0.8f;}
                break;
            case "TreasuresOfTory":
                {volume = 0.8f;}
                break;
            default:
                break;
            }
            switch (winType) {
            case 0:
                AudioManager.Instance.AsyncPlayEffectAudio (audioSmallWin);
                break;
            case 1:
                AudioManager.Instance.AsyncPlayEffectAudio (audioMidWin);
                break;
            case 2:
                AudioManager.Instance.AsyncPlayEffectAudio (audioBigWin);
                break;
            case 3:
                AudioManager.Instance.AsyncPlayEffectAudio (audioTopWin);
                break;
            default:
                AudioManager.Instance.AsyncPlayEffectAudio (audioWheelStopAndWin,false,volume);
            break;
            }
           
        }

        public void StopAllEffect()
        {
            if (AudioManager.GetInstance()==null) {
                return;
            }
            AudioManager.Instance.StopAllEffectAudio ();
        }
		public void StopRollingUpEffect(){
			AudioManager.Instance.StopEffectAudio (audioRollingUp);
		}
        public void StopAllMusic()
        {
            if (AudioManager.GetInstance()==null) {
                return;
            }
            AudioManager.Instance.StopAllMusicAudio ();
        }
        public void StopAllAudio()
        {
            StopAllMusic ();
            StopAllEffect ();
        }


        public void PlayFeatureBtnEffect()
        {
            AudioManager.Instance.AsyncPlayEffectAudio("feature_dialog_btn");
        }

        #region FreeGame音效

        //暂停FreeGame背景音乐
        public void PauseFreeGameBgMusic()
        {
            AudioManager.Instance.PauseMusicAudio(audioSpecial);
        }

        public void UnPauseFreeGameBgMusic()
        {
            AudioManager.Instance.UnPauseMusicAudio(audioSpecial);
        }

        //播放FreeSpin弹窗音乐
        public void PlayFreeGameStartDialogMusic(float volume = 1.0f)
        {
            AudioManager.Instance.AsyncPlayMusicAudio("freegame_start_dialog", false, volume);
        }

        public void StopFreeGameStartDialogMusic()
        {
            AudioManager.Instance.StopMusicAudio("freegame_start_dialog");
        }

        public void PlayFreeGameRetriggerDialogMusic()
        {
            AudioManager.Instance.AsyncPlayMusicAudio("freegame_retrigger_dialog", false);
        }

        public void StopFreeGameRetriggerDialogMusic()
        {
            AudioManager.Instance.StopMusicAudio("freegame_retrigger_dialog");
        }

        public void PlayFreeGameEndDialogMusic(float volume = 1.0f)
        {
            AudioManager.Instance.AsyncPlayMusicAudio("freegame_end_dialog", false, volume);
        }

        public void StopFreeGameEndDialogMusic()
        {
            AudioManager.Instance.StopMusicAudio("freegame_end_dialog");
        }


        //播放ReSpin弹窗音乐
        public void PlayReSpinStartDialogMusic(float volume = 1.0f)
        {
            AudioManager.Instance.AsyncPlayMusicAudio("respin_start_dialog", false, volume);
        }

        public void StopReSpinStartDialogMusic()
        {
            AudioManager.Instance.StopMusicAudio("respin_start_dialog");
        }

        public void PlayReSpinEndDialogMusic(float volume = 1.0f)
        {
            AudioManager.Instance.AsyncPlayMusicAudio("respin_end_dialog", false, volume);
        }

        public void StopReSpinEndDialogMusic()
        {
            AudioManager.Instance.StopMusicAudio("respin_end_dialog");
        }


        //gliding 上下移动的音效
        public void PlayGlidingEffect(float volume = 1f)
        {
            AudioManager.Instance.AsyncPlayEffectAudio(audio_gliding, false, volume);
        }
        #endregion

        public const string COMMON_JACKPOT_WHEEL_TRIGGER = "JackpotWheelTrigger";
        public const string COMMON_JACKPOT_WHEEL_TURN = "JackpotWheelTurn";
        public const string COMMON_JACKPOT_WHEEL_WIN = "JackpotWheelWin";
        public const string COMMON_JACKPOT_WHEEL_WINBASE = "JackpotWheelWinBase";
        public const string COMMON_JACKPOT_WHEEL_DING = "JackpotWheelDing";
        /// <summary>
        /// jackpot 弹出转轮的音效
        /// </summary>
        public void PlayCommonJackpotWheelTrigger()
        {
            AudioManager.Instance.AsyncPlayEffectAudio(COMMON_JACKPOT_WHEEL_TRIGGER);
        }
        /// <summary>
        /// jackpot 转轮转动的音效
        /// </summary>
        public void PlayCommonJackpotWheelTurn()
        {
            AudioManager.Instance.AsyncPlayEffectAudio(COMMON_JACKPOT_WHEEL_TURN);
        }
        /// <summary>
        /// jackpot 停止转轮转动的音效
        /// </summary>
        public void StopCommonJackpotWheelTurn()
        {
            AudioManager.Instance.StopEffectAudio(COMMON_JACKPOT_WHEEL_TURN);
        }
        /// <summary>
        /// jackpot 转轮停下，中奖那片播放粒子特效时音效
        /// </summary>
        public void PlayCommonJackpotWheelWin()
        {
            AudioManager.Instance.AsyncPlayEffectAudio(COMMON_JACKPOT_WHEEL_WIN);
        }

        /// <summary>
        /// jackpot 转轮中，播放缓慢停止Ding音效
        /// </summary>
        public void PlayCommonJackpotWheelDing()
        {
            AudioManager.Instance.AsyncPlayEffectAudio(COMMON_JACKPOT_WHEEL_DING);
        }

        /// <summary>
        /// jackpot 中基础奖励出弹板的音效
        /// </summary>
        public void PlayCommonJackpotWheelWinBase()
        {
            AudioManager.Instance.AsyncPlayEffectAudio(COMMON_JACKPOT_WHEEL_WINBASE);
        }

        public void PlayCommonJackPotWin()
        {
            AudioManager.Instance.AsyncPlayEffectAudio(COMMON_JACKPOT_WIN);
        }
    }
}
