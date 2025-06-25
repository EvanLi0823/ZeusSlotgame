using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Classic;
using Utils;

public class ReelManagerDataConfig
{
	public static readonly string REEL_MANAGER_DATA_CONFIG = "ReelManagerDataConfig";
	static readonly string BASEDATA_CONFIG = "BaseDataConfig";
	static readonly string SHOW_ALL_SYMBOL_ANIMATION_CONFIG = "ShowAllSymbolAnimationConfig";
	static readonly string FREESPIN_AWARD_CONFIG = "FreeSpinAwardConfig";
	static readonly string BONUSGAME_AWARD_CONFIG = "BonusGameAwardConfig";
	static readonly string ONCE_MORE_GAME_CONFIG = "OnceMoreGameConfig";


	public BaseDataConfig baseDataConfig = null;

	public ShowAllSymbolAnimationConfig showAllSymbolAnimationConfig = null;

	public FreeSpinAwardConfig freeSpinAwardConfig = null;

	public BonusGameAwardConfig bonusGameAwardConfig = null;

	public OnceMoreGameConfig onceMoreGameConfig = null;

	public ReelManagerDataConfig (Dictionary<string,object> config)
	{
		if (config == null) {
			return;
		}

		if (config.ContainsKey (BASEDATA_CONFIG)) {
			baseDataConfig = new BaseDataConfig (config [BASEDATA_CONFIG] as Dictionary<string,object>);
		}

		if (config.ContainsKey (SHOW_ALL_SYMBOL_ANIMATION_CONFIG)) {
			showAllSymbolAnimationConfig = new ShowAllSymbolAnimationConfig (config [SHOW_ALL_SYMBOL_ANIMATION_CONFIG] as Dictionary<string,object>);
		}

		if (config.ContainsKey (FREESPIN_AWARD_CONFIG)) {
			freeSpinAwardConfig = new FreeSpinAwardConfig (config [FREESPIN_AWARD_CONFIG] as Dictionary<string,object>);
		}

		if (config.ContainsKey (BONUSGAME_AWARD_CONFIG)) {
			bonusGameAwardConfig = new BonusGameAwardConfig (config [BONUSGAME_AWARD_CONFIG] as Dictionary<string,object>);
		}

		if (config.ContainsKey (ONCE_MORE_GAME_CONFIG)) {
			onceMoreGameConfig = new OnceMoreGameConfig (config [ONCE_MORE_GAME_CONFIG] as Dictionary<string,object>);
		}
	}


	public class BaseDataConfig
	{
		static readonly string SCAL_FACTOR = "scalFactor";
		static readonly string LINE_ANIMATION_TAG = "lineAnimationTag";
		static readonly string HAS_BIG_ANIMATION = "hasBigAnimation";
		static readonly string BIG_ANIMATION_TAG = "bigAnimationTag";

		public float scalFactor = 1f;
		public int lineAnimationTag = 20;
		public bool hasBigAnimation = false;
		public int bigAnimationTag = 10;

		public BaseDataConfig (Dictionary<string,object> config)
		{
			if (config.ContainsKey (SCAL_FACTOR)) {
				scalFactor = Utilities.CastValueFloat (config [SCAL_FACTOR], scalFactor);
			}

			if (config.ContainsKey (LINE_ANIMATION_TAG)) {
				lineAnimationTag = Utilities.CastValueInt (config [LINE_ANIMATION_TAG], lineAnimationTag);
			}

			if (config.ContainsKey (HAS_BIG_ANIMATION)) {
				hasBigAnimation = Utilities.CastValueBool (config [HAS_BIG_ANIMATION], hasBigAnimation);
			}

			if (config.ContainsKey (BIG_ANIMATION_TAG)) {
				bigAnimationTag = Utilities.CastValueInt (config [BIG_ANIMATION_TAG], bigAnimationTag);
			}
		}

	}


	public class ShowAllSymbolAnimationConfig
	{
		static readonly string SHOW_ALL_ANIMATION = "showAllAnimation";
		static readonly string SHOW_ALL_ANIMATION_TAG = "showAllAnimationTag";

		public bool showAllAnimation = false;
		public float showAllAnimationTag = 3f;

		public ShowAllSymbolAnimationConfig (Dictionary<string,object> config)
		{
			if (config.ContainsKey (SHOW_ALL_ANIMATION)) {
				showAllAnimation = Utilities.CastValueBool (config [SHOW_ALL_ANIMATION], showAllAnimation);
			}

			if (config.ContainsKey (SHOW_ALL_ANIMATION_TAG)) {
				showAllAnimationTag = Utilities.CastValueFloat (config [SHOW_ALL_ANIMATION_TAG], showAllAnimationTag);
			}
		}
	}

	public class FreeSpinAwardConfig
	{
		static readonly string HAS_FREESPINGAME = "hasFreespinGame";
		static readonly string FREE_SPIN_TIMES = "freeSpinTimes";
		static readonly string AWARD_SYMBOL_COUNT = "awardSymbolCount";

		public bool hasFreespinGame = false;
		public int freeSpinTimes = 7;
		public int awardSymbolCount = 3;

		public FreeSpinAwardConfig (Dictionary<string,object> config)
		{

			if (config.ContainsKey (HAS_FREESPINGAME)) {
				hasFreespinGame = Utilities.CastValueBool (config [HAS_FREESPINGAME], hasFreespinGame);
			}

			if (config.ContainsKey (FREE_SPIN_TIMES)) {
				freeSpinTimes = Utilities.CastValueInt (config [FREE_SPIN_TIMES], freeSpinTimes);
			}

			if (config.ContainsKey (AWARD_SYMBOL_COUNT)) {
				awardSymbolCount = Utilities.CastValueInt (config [AWARD_SYMBOL_COUNT], awardSymbolCount);
			}
		}
	}

	public class BonusGameAwardConfig
	{
		static readonly string HAS_BONUS_GAME = "hasBonusGame";
		static readonly string AWARD_SYMBOL_COUNT = "awardSymbolCount";
		static readonly string BONUS_NEED_CHECK_RESULT = "bonusNeedCheckResult";

		public bool hasBonusGame = false;
		public int awardSymbolCount = 3;
		public bool bonusNeedCheckResult = true;

		public BonusGameAwardConfig (Dictionary<string,object> config)
		{

			if (config.ContainsKey (HAS_BONUS_GAME)) {
				hasBonusGame = Utilities.CastValueBool (config [HAS_BONUS_GAME], hasBonusGame);
			}

			if (config.ContainsKey (AWARD_SYMBOL_COUNT)) {
				awardSymbolCount = Utilities.CastValueInt (config [AWARD_SYMBOL_COUNT], awardSymbolCount);
			}

			if (config.ContainsKey (BONUS_NEED_CHECK_RESULT)) {
				bonusNeedCheckResult = Utilities.CastValueBool (config [BONUS_NEED_CHECK_RESULT], bonusNeedCheckResult);
			}
		}
	}

	public class OnceMoreGameConfig
	{
		static readonly string HAS_ONECE_MORE = "hasOnceMore";
		static readonly string GEN_RATE = "genRate";

		public bool hasOnceMore = false;
		public float genRate = 0.3f;

		public OnceMoreGameConfig (Dictionary<string,object> config)
		{
			if (config.ContainsKey (HAS_ONECE_MORE)) {
				hasOnceMore = Utilities.CastValueBool (config [HAS_ONECE_MORE], hasOnceMore);
			}

			if (config.ContainsKey (GEN_RATE)) {
				genRate = Utilities.CastValueFloat (config [GEN_RATE], genRate);
			}
		}
	}
}
