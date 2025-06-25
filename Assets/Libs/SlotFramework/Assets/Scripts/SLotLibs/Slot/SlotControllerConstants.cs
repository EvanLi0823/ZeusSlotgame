using UnityEngine;
using System.Collections;


public class SlotControllerConstants
{
    public const string ClassicSlotController = "SlotController";

    public const string InitSlotScene = "InitSlotScene";
    
    public const string OnSpinStart = "OnSpinStart";
    public const string OnSpinEnd = "OnSpinEnd";
    public const string SendSpinEvent = "SendSpinEvent";

	public const string EndCheckResult = "EndCheckResult";
    public const string OnRechargeableSpinWillStart = "OnRechargeableSpinWillStart";
    public const string OnRechargeableSpinWillEnd = "OnRechargeableSpinWillEnd";

    public const string OnAnimationSpinEnd = "OnAnimationSpinEnd";

    public const string OnBlanceChange = "OnBlanceChange";
    public const string OnBetChange = "OnBetChange";
    public const string OnBetRefresh = "OnBetRefresh";
	public const string InCommonJackPotPanel = "InCommonJackPotPanel";
	public const string OnBlanceChangeForDisPlay = "OnBlanceChangeForDisPlay"; 
	public const string OnCashChangeForDisPlay = "OnCashChangeForDisPlay"; 
	public const string OnPopLevelChange = "OnPopLevelChange"; 

	public const string OnBetChangeByUser = "OnBetChangeByUser";  

    public const string OnWinCoins = "OnWinCoins";
    public const string OnChangeWinText = "OnChangeWinText";
    public const string OnChangeWinTextSilence = "OnChangeWinTextSilence";
    public const string OnWinCash = "OnWinCash";
    public const string OnChangeCashText = "OnChangeCashText";
    public const string OnChangeCashTextSilence = "OnChangeCashTextSilence";
    public const string RefreshCashState = "RefreshCashState";

    public const string OnAverageBet = "OnAverageBet";
	public const string OnInterruptRollingEffect = "OnInterruptRollingEffect";
    public const string OnChangeWinTextEx = "OnChangeWinTextEx";
    public const string DisactiveButtons = "DisactiveButtons";
    public const string DisableBetButtons = "DisableBetButtons";
	public const string DisableSpinButton = "DisableSpinButton";
    public const string ActiveButtons = "ActiveButtons";
    public const string EnableBetButtons = "EnableBetButtons ";
	public const string MAX_BET_TIP = "MaxBetTip";
	public const string OnRefreshRewardSpin = "OnRefreshRewardSpin";
    public const string OnGainExperience = "OnGainExperience";

    public const string OnEnterFreespin = "OnEnterFreespin";
    public const string OnFreespinTimesChanged = "OnFreespinTimesChanged";
    public const string OnQuitFreespin = "OnQuitFreespin";

	public const string OnSpinStartPiggyCoins = "OnSpinStartPiggyCoins";


	public const string EnableDisableSpin = "EnableDisableSpin";


    public const string ShowOrHideBottomCanvasCustomPanel = "ShowOrHideBottomCanvasCustomPanel";
    public const string AssignBottomCanvasCustomPanel = "AssignBottomCanvasCustomPanel";

	public const string OnWinCoinsForDisplay = "OnWinCoinsForDisplay";
	public const string HideDoubleUpButton = "HideDoubleUpButton";

	public const string ShowAwardAnimation = "ShowAwardAnimation";
	public const string ChangeMiddleMessage = "ChangeMiddleMessage";

	public const string FlurryNoMonedyKey="GameTime";
	public const string FlurryOnSpinKey="SpinTimes";

	public const string SLOTS_CURRENT_BET ="CURRENT_BET";

	public const string AUTO_SPIN_SUSPEND ="AUTO_SPIN_SUSPEND";
	public const string AUTO_SPIN_RESUME ="AUTO_SPIN_RESUME";

	public const string HIGH_ROLLER_CHECK_KEY = "HighRollerPopupDialogCheck";

	public const string PAYLINE_ANIMATION_SHOW = "PAYLINE_ANIMATION_SHOW";
    public const string PAYLINE_ANIMATION_SHOW_Slower = "PAYLINE_ANIMATION_SHOW_SLOWER";

	public const string PAYLINE_ANIMATION_SHOW_BOXPARTICLE = "PAYLINE_ANIMATION_SHOW_BOXPARTICLE";

	public const string PAYLINE_ANIMATION_HIDE = "PAYLINE_ANIMATION_HIDE";
	public const string PAYLINE_SYMBOL_ANIMATION_HIDE = "PAYLINE_SYMBOL_ANIMATION_HIDE";

    public const string WIN_COINS ="WinCoins";
	public const string JackpotType ="JackpotType";
	public const string PROBABILITY = "Probability";
	public const string NAME = "name";
	public const string THEME = "theme";
	public const string GAIN_WIN_TYPE ="GainWinType";
	public const string JACKPOT_TYPE_NORMAL_JACKPOT = "NormalJackpot";

	public const string SLOT_REELMANAGER_INIT="SlotReelManageInit";
	public const string CUSTOM_JACKPOT_FLAG= "JackFlag";

	public const string TOURNAMENT_SPIN_ENROLL_MSG = "TOURNAMENT_SPIN_ENROLL_MSG";
	public const string TOURNAMENT_ENABLE_ENROLL_MSG = "TOURNAMENT_ENABLE_ENROLL_MSG";
	public const string TOURNAMENT_DISABLE_ENROLL_MSG = "TOURNAMENT_DISABLE_ENROLL_MSG";

	public const string TOURNAMENT_ENABLE_INTERVAL_MSG="TOURNAMENT_ENABLE_INTERVAL_MSG";
	public const string TOURNAMENT_DISABLE_INTERVAL_MSG="TOURNAMENT_DISABLE_INTERVAL_MSG";
	public const string IS_COLLECTABlE = "IsCollectable";
	public const string WHEEL_LIST= "WheelList";

	public const string OnSwitchWaitServerResult = "OnSwitchWaitServerResult";
	
    //	public const string SLOT_FREESPIN_ENTER="SLOT_FREESPIN_ENTER";
    //	public const string SLOT_FREESPIN_EXIT="SLOT_FREESPIN_EXIT";

    #region 机器效果
    public const string PLAY_OUTER_BORDER_BIG_WIN_ANIMATION = "PlayOutBorderBigAnimation";
    #region anticipation blink
    public const string PLAY_ANTICIPATION_EFFECT = "PLAY_ANTICIPATION_EFFECT";
    public const string STOP_ANTICIPATION_EFFECT = "STOP_ANTICIPATION_EFFECT";

    public const string OnEnterLink = "OnEnterLink";
    public const string OnQuitLink = "OnQuitLink";
    public const string MachineSpeedFastMode = "MachineSpeedFastMode";
    #endregion

    public const string PLAY_BACKGROUND_MUSIC = "PLAY_BACKGROUND_MUSIC";
    public const string REFRESH_JACKPOT_UI_TYPE = "REFRESH_JACKPOT_UI_TYPE";
    public enum JACKPOT_TYPE
    {
	    GRAND,
	    MAJOR,
	    MINOR,
	    MINI,
	    NONE
    }
    #endregion

    public const string SERVER_SPIN_DATA_PARENT = "ReelData";
    public const string SERVER_SPIN_DATA_Init = "InitBaseData";
    public const string SERVER_SPIN_DATA_InitEXTRA = "InitExtraData";
    public const string SERVER_SPIN_DATA_BG = "BaseGame";
    public const string SERVER_SPIN_DATA_FS = "FSGame";
    public const string SERVER_SPIN_DATA_LK = "LinkGame";
}

