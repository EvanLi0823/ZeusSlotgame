using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Classic;
public class ConditionContentCellFactory {
	public const string IN_MACHINE = "InMachine";
	public const string PLAYER_LEVEL = "PlayerLevel";
	public const string BET = "Bet";
	public const string BALANCE = "Balance";
	public const string SPIN_COUNT = "SpinCount";
	public const string BALANCE_BET_RATIO = "BalanceBetRatio";
	public const string MAX_BALANCE_CURRENT_BALANCE_RATIO = "MaxBalanceCurrentBalanceRatio";
	public const string SESSION_SPIN_COUNT_INTERVAL = "SessionSpinCountInterval";
	public const string FIRST_COOL_DOWN_SESSION_SPIN_COUNT_INTERVAL = "FirstCoolDownSessionSpinCountInterval";
	public const string SESSION_EVENT_ACTION_TRIGGER_COUNT = "SessionEventActionTriggerCount";
	public const string GLOBAL_EVENT_ACTION_TRIGGER_COUNT = "GlobalEventActionTriggerCount";
	public const string GLOBAL_SPIN_COUNT_INTERVAL = "GlobalSpinCountInterval";
	public const string FIRST_COOL_DOWN_GLOBAL_SPIN_COUNT_INTERVAL = "FirstCoolDownGlobalSpinCountInterval";
	public const string TRIGGER_MOMENT = "TriggerMoment";
	public const string SWITCH_ON = "SwitchOn";
	public const string PROBALITY = "Probality";
	public const string POP_UP_SUSPEND = "PopUpSuspend";
	public static ConditionContentCell CreateContentCell(object value,string contentCellTypeFlag){

		switch (contentCellTypeFlag) {
			case PLAYER_LEVEL:
				{
					return new PlayerLevelContentCell (value);
				}
			case BET:
				{
					return new BetContentCell (value);
				}
				
			case BALANCE:
				{
					return new BalanceContentCell (value);
				}
				
			case SPIN_COUNT:
				{
					return new SpinCountContentCell (value);
				}

			case BALANCE_BET_RATIO:
				{
					return new BalanceBetRatioContentCell (value);
				}
				
			case SESSION_SPIN_COUNT_INTERVAL:
				{
					return new SessionSpinCounIntervaltConditionContentCell (value);
				}
				
			case MAX_BALANCE_CURRENT_BALANCE_RATIO:
				{
					return new MaxBalanceCurrentBalanceRatioConditionContentCell (value);
				}
				
			case SESSION_EVENT_ACTION_TRIGGER_COUNT:
				{
					return new SessionEventActionTriggerCountConditionContentCell (value);
				}
			case GLOBAL_EVENT_ACTION_TRIGGER_COUNT:
				{
					return new GlobalEventTriggerCountConditionContentCell (value);
				}
			case GLOBAL_SPIN_COUNT_INTERVAL:
				{
					return new GlobalSpinCountIntervalConditionContentCell (value);
				}
			case SWITCH_ON:
				{
					return new SwitchOnConditionContentCell (value);
				}
			case PROBALITY:
				{
					return new ProbalityConditionContentCell (value);
				}
			case FIRST_COOL_DOWN_GLOBAL_SPIN_COUNT_INTERVAL:
				{
					return new FirstCoolDownGlobalSpinCountIntervalConditionContentCell (value);
				}
			case FIRST_COOL_DOWN_SESSION_SPIN_COUNT_INTERVAL:
				{
					return new FirstCoolDownSessionSpinCounIntervaltConditionContentCell (value);
				}
			case POP_UP_SUSPEND:
				{
					return new PopUpSuspendFactorsConditionContentCell (value);
				}
			case TRIGGER_MOMENT:
				{
					return new TriggerMomentContentCell (value);
				}
			case IN_MACHINE:
				{
					return new InMachineContentCell (value);
				}
			case GameConstants.AssetMultipler_Key:
				{
					return new UserMultiplerConditionContentCell(value);
				}
            case GameConstants.ShopIsNew_Key:
                {
                    return new UserIsNewShopConditionContentCell(value);
                }
			case GameConstants.IsShowWelcomeOnce_Key:
				{
					return new IsShowWelcomeOnceConditionContentCell(value);
				}
			case GameConstants.IsCheckFeature_Key:
				return new IsRateFeatureSlotConditionContentCell(value);
			case GameConstants.RequireSpinNum_Key:
				return new RateSlotRequireSpinNumConditionContentCell(value);
			default:
				{
					Utils.Utilities.LogPlistError ("ConditionContentCell:" + contentCellTypeFlag + " Parse not right");
				}
			break;
		}
		return new ConditionContentCell();
	}
}
