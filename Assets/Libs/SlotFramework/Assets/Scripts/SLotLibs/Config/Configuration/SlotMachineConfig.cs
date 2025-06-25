using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Classic;
using Core;
using Utils;
using Libs;
using System.IO;



public class SlotMachineConfig :IData
{
	//private static string SLOT_MACHINE_PLIST_NAME="Machine.plist.xml";

	public const string WILD_ACCUMULATION = "WildAccumulation";
	public const string HAS_SPECIAL_SLOT_SOUND = "HasSpecialSlotSound";
	public const string RATE_ALERT_NAME = "MachineName";
	public const string SPIN_SAVE_LOCAL_TIME = "_SpinAllCount";
	public const string FEATURED_MACHINE = "FeaturedMachine";
	public const string MIN_BET = "MinBet";
	public const string BET_LIMIT = "BetLimit";
    public const string SUB_BOARDS = "SubBoards";
    public const string USE_SPINE = "UseSpine";
    public const string FAST_TIME_SCALE = "FastTimeScale";

    #region High Roller
    public readonly static string HIGH_ROLLER_CONFIG= "HighRollerConfig";
	public readonly static string IS_HIGH_ROLLER_MACHINE ="IsHighRollerMachine";
	public readonly static string DIS_PLAY_COUNT ="DisplayCount";
	public readonly static string DISPLAY_INTERVAL = "DisplayInterval";
	public readonly static string DISPLAY_SHOW_TIME = "DisplayTime";
	#endregion


	public const long DEFAULT_MIN_BET = 100;
	#if UNITY_EDITOR
	static int m_ForceLoadPlistFromInPackageInEditor = -1;
	const string kForceLoadPlistFromInPackage = "ForceLoadPlistFromInPackage";
	static bool ForceLoadPlistFromInPackageInEditor{
		get{
			if (m_ForceLoadPlistFromInPackageInEditor == -1)
				m_ForceLoadPlistFromInPackageInEditor = EditorPrefs.GetBool(kForceLoadPlistFromInPackage, true) ? 1 : 0;

			return m_ForceLoadPlistFromInPackageInEditor != 0;
		}	
		set
		{
			int newValue = value ? 1 : 0;
			if (newValue != m_ForceLoadPlistFromInPackageInEditor)
			{
				m_ForceLoadPlistFromInPackageInEditor = newValue;
				EditorPrefs.SetBool(kForceLoadPlistFromInPackage, value);
			}
		}
	}
	public static bool ForceLoadPlistFromInPackage{
		get{ 
			return ForceLoadPlistFromInPackageInEditor;
		}
		set{ 
			ForceLoadPlistFromInPackageInEditor = value;
		}
	}
#endif

  
    public BecomeWildStripsProbabilityTable  becomeWildStripsProbabilityTable
	{
		get;
		set;
	}
	//origin的plist的data信息
	public Dictionary<string, object> OrderInfo {
		get;
		set;
	}

	public bool isRewardSpin
	{
		get;
		set;
	}

	public bool UseSpine
	{
		get;
		set;
	}

	public float FastTimeScale = 1;

	#region ClubSystem

	public bool isInClub
	{
		get;
		set;
	}
	
	public bool isExclusive
	{
		get;
		set;
	}
	
	public bool isSneakPeak
	{
		get;
		set;
	}
	
	public bool isHighRoller
	{
		get;
		set;
	}

	public string AssetName
	{
		get;
		set;
	}
	#endregion
	
	
	public ExtroInfos extroInfos
	{
		get;
		set;
	}
	
	public FakeStrip FakeStrip
	{
		get;
		set;
	}

	public bool wildAccumulation
	{
		get;
		set;
	}

	public bool hasSpecialSlotSound
	{
		get ;
		set ;
	}

	public ReelManagerDataConfig reelManagerDataConfig
	{
		get;
		set;
	}

	public MachineTestDataFromPlistConfig machineTestDataFromPlistConfig
	{
		get;
		set;
	}

	public string GetCardName()
	{
		return string.Format("{0}_card", Name());
	}

    public string PlistName
    {
        set;
        get;
    }

	public bool IsCardInPackage
	{
		set;
		get; 
	}

	public bool IsPortrait
    {
        get;
        set;
    }

    public string CardType
    {
    	set;
    	get; 
    }
	public bool IsShakeStop { get; set;}
	public bool IsTempUnlock { get; set;}
	public bool IsNeedUpdateApp { get; set;}
    public bool EnableRollingUp { get; set; }
    
    public int LobbyShowJackpotGrand { get; set; }
    public int LobbyShowJackpotIncrease { get; set; }

    public int SortIndex = 0;

	public bool Activate3DEffect
	{
		get;
		set;
	}

	public bool useFeatureOpenBet { get; set; }
	
	#region spinCount

	public bool isNewForUser(){
		return !SharedPlayerPrefs.HasPlayerPrefsKey (Name() + SPIN_SAVE_LOCAL_TIME);
	}


	private int[] LoadLocalSpinCount()
	{
		int[] result = new int[MAX_SPIN_COUNT_DAYS];
		string str = SharedPlayerPrefs.GetPlayerPrefsStringValue(Name() + SPIN_SAVE_LOCAL_TIME, "");
		if (!string.IsNullOrEmpty(str))
		{
			string[] tmp = str.Split(new[] { '_' });
			for (int i = 0; i < tmp.Length; i++)
			{
				result[i] = int.Parse(tmp[i]);
			}
		}
		return result;
	}

	private void SaveLocalSpinCount(int[] OldSpinArray)
	{
		string[] tmp = new string[OldSpinArray.Length];
		for (int i = 0; i < OldSpinArray.Length; i++)
		{
			tmp[i] = OldSpinArray[i].ToString();
		}
		string str = String.Join("_", tmp);
		SharedPlayerPrefs.SetPlayerPrefsStringValue(Name() + SPIN_SAVE_LOCAL_TIME, str);
//		for (int i = 0; i < MAX_SPIN_COUNT_DAYS; i++) {
//			SharedPlayerPrefs.SetPlayerPrefsIntValue (Name () + SPIN_SAVE_LOCAL_TIME+i,OldSpinArray[i]);
//		}
		SharedPlayerPrefs.SavePlayerPreference();
	}

	private long _allSpinCount = 0;
	private const int MAX_SPIN_COUNT_DAYS = 31;

	public long MachineSpinCount
	{
		get{ return _allSpinCount; }
	}

	//当前机器的所有spin次数
	public int GetAllSpinCount()
	{
		int[] lastDaysSpinCountArray = LoadLocalSpinCount();//PlayerPrefsX.GetIntArray (Name () + SPIN_SAVE_LOCAL_TIME, 0, MAX_SPIN_COUNT_DAYS);
		int daysSinceLastLaunch = UserManager.GetInstance().UserProfile().DaysFromLastLaunch();

		if (daysSinceLastLaunch > 0)
		{
			daysSinceLastLaunch = Math.Min(daysSinceLastLaunch, MAX_SPIN_COUNT_DAYS);
			//save old data
			int allOldSpinCounts = 0;
			for (int j = 0; j < daysSinceLastLaunch; j++)
			{
				allOldSpinCounts += lastDaysSpinCountArray[MAX_SPIN_COUNT_DAYS - j - 1];
			}

			int[] exactSpinArray = new int[MAX_SPIN_COUNT_DAYS];
			Array.Copy(lastDaysSpinCountArray, 0, exactSpinArray, daysSinceLastLaunch, MAX_SPIN_COUNT_DAYS - daysSinceLastLaunch);
			SaveLocalSpinCount(exactSpinArray);
			exactSpinArray[MAX_SPIN_COUNT_DAYS - 1] = allOldSpinCounts;
			lastDaysSpinCountArray = exactSpinArray;
		}

		int dayCounts = 31;//DataManager.GetInstance().FavoriteLocalDays;
		if (daysSinceLastLaunch > dayCounts)
		{
			dayCounts = MAX_SPIN_COUNT_DAYS;
		}
		dayCounts = Math.Min(dayCounts, MAX_SPIN_COUNT_DAYS); //max
		int result = 0;
		for (int i = 0; i < dayCounts; i++)
		{
			result += lastDaysSpinCountArray[i];
		}
		return result;
	}

	public void CaculateMachineSpinCount()
	{
		_allSpinCount = GetAllSpinCount();
	}

	public void AddTodaySpinCount(int addCount)
	{
		int[] originArray = LoadLocalSpinCount();
		originArray[0] += addCount;
		SaveLocalSpinCount(originArray);
	}

	#endregion

	public void SetName(string name)
	{
		this.name = name;
	}

	public void SetComparePath(string _comparePath = "")
	{
		if(string.IsNullOrEmpty(_comparePath))
		{
			return;
		}
		this.comparePath = _comparePath + "/";
	}

	public string Name()
	{
		return name;
	}

	#region ClubSystem
	public string BundlePath()
	{
		return comparePath + AssetName.ToLower();
	}

	#endregion
	
	
	public int SlotIndex
	{
		get;
		set;
	}


	public bool IsFavorate = false;
	
	public bool EnableTournament = false;

	public bool IsFeatureSlot = false;

	public bool IsNewSlot = false;

	public bool IsCommingSoon {
		set;
		get;
	}

	/// <summary>
	/// 是否在维护中
	/// </summary>
	public bool IsUnderMaintenance { get; set; }

	public int VipLockLevel
	{
		get;
		set;
	}

	public  string RemoteBundleURL()
	{
		return remoteBundleURL;
	}

	public float CardDownLoadProgress
	{
		set;
		get;
	}

	public  string RateShowName
	{
		get;
		set;
	}

	public void SetRemoteBundleURL(string url)
	{
		if (!string.IsNullOrEmpty(url))
		{
			remoteBundleURL = Uri.UnescapeDataString(url);
		}
		else
		{
			remoteBundleURL = url;
		}
	}

	public void SetRemoteCardBundleURL(string url)
	{
		if (!string.IsNullOrEmpty(url))
		{
			remoteCardVersion = Uri.UnescapeDataString(url);
		}
		else
		{
			remoteCardVersion = url;
		}
	}

	public bool IsMachineInPackage
	{
		set;
		get;
	}

	public bool EnableShowJackPotInfo {
		set;
		get;
	}

	public bool IsHighRollerMachine = false;
	public int HighRollerDisPlayCount = 2;
	public int HighRollerDisplayInterval =86400;
	public int HighRollerDisplayTime = 2;

	public long MinBet
	{
		set{ minBet = value;}
		get{ return minBet;}
	}
	//机器内暂时不包含BetConfig解析--即不支持这块BetConfig切换 如果需要支持--需要添加功能
    public Dictionary<long, int> MachineBetLevelDict = new Dictionary<long, int>();
    public List<long> MachineBetList = new List<long>();
	private long minBet=0;   
	
	/***
	 * 是否打开触发freegame看广告spin次数加一
	 */
	public bool IsOpenAdFreespinCountAdd
	{
		get;
		set;
	}

	public bool EnableCommonJackpot
	{
		set;
		get;
	}

    public int NoWinRequestPossibility
    {
        get;
        set;
    }

	public int MachineId {
		get;
		set;
	}

	public string RemoteCardVersion()
	{
		return remoteCardVersion;
	}

	public  int RequiredLevel()
	{
		return requiredLevel;
	}

	public int PrenoticeUnlockLevel()
	{
		return prenoticeUnlockLevel;
	}

	public void SetPrenoticeUnlockLevel(int level)
	{
		prenoticeUnlockLevel = level;
	}
	public void SetRequiredLevel(int level)
	{
		requiredLevel = level;
	}

	public bool IsUseTestSpeed { set; get; } = false;

	public RuleSwitch RuleSwitch { set; get; } = new RuleSwitch("0");

	public bool SpinUseNetwork { get; set; }
	
	public bool SpinUseFakeStrip { get; set; }

	public bool SpinButtonWaitNet { get; set; }

	public bool UseFeatureRule { set; get; } = false;
	public  Dictionary<string, object> Properties()
	{
		return properties;
	}

	public void SetProperties(Dictionary<string,object> newProps)
	{
		properties.Clear();
		if (newProps != null)
		{
			foreach (string key in newProps.Keys)
			{
				properties.Add(key, newProps[key]);
			}
		}
	}
	public List<JackPotIncreaseMachineConfig> JackPotIncreaseConfigs = new List<JackPotIncreaseMachineConfig>();
	//increase
	public List<JackPotPrizePool> JackPotPrizeInfos = new List<JackPotPrizePool>();
	//所有的配置，其中有的部分会increase


	
	#region spin数据计算相关
    //放到关卡里面的linetable
    public Dictionary<string, LineTable> LineTableDict = new Dictionary<string, LineTable>();

    public BoardStripConfig MainBoardStripConfig = new BoardStripConfig() ;//Strip，SymbolMap，PayTable，PayLine

    //fs的strip,注意需要加入linetable，以及需要手动初始化
    public BoardStripConfig FSStripConfig;
    
    public List<BoardStripConfig> SubBoardStripConfigList;

    public object ServerReelData;

    //0 普通机器 1 power feature machine 2 high roller feature machine
    public int IsPowerMachine
    {
	    get;
	    set;
    }

    private void InitAllLineTables()
    {
        Dictionary<string, object> dictObj = Utils.Utilities.GetValue<Dictionary<string, object>>(this.PlistDict, LineTable.LINE_TABLES_KEY, null);
        if (dictObj == null)
        {
            return;
        }

		LineTableDict.Clear();

        foreach (string key in dictObj.Keys)
        {
            Dictionary<string, object> dict = (Dictionary<string, object>)dictObj[key];

            LineTable lineTable = LineTable.ParseLineTable(key, dict, null);

            if(lineTable != null)
            {
                LineTableDict.Add(key, lineTable);
            }
        }

    }

    public LineTable GetLineTable(string lineTableName)
    {
	    if (LineTableDict.ContainsKey(lineTableName))
	    {
		    return LineTableDict[lineTableName];
	    }

	    return null;
    }


    #endregion

    #region 初始化config，将此内容放入到加载场景的loading中

    public bool IsDataInit = false;
    //是否初始化完毕
    public Dictionary<string, object> PlistDict = null;
    //plist中的原始数据
    public void ParseDict()
	{
//		if (IsDataInit) return;
		IsDataInit = false;
		Dictionary<string ,object> configMachineDict = Plugins.Configuration.GetInstance().GetValueWithPath<Dictionary<string ,object>>("/" + SlotMachineConfigParse.SLOT_MACHINES + "/" + this.name, null);
		if (configMachineDict != null)
		{
			PlistDict = configMachineDict;
		}
		else
		{

			byte[] plistData = GetPlistTextConfig(this.AssetName);
			if (plistData != null && plistData.Length > 0)
			{
				Dictionary<string ,object> machinePlistData = CSharpUtil.ParsePlistByteArray(plistData);
				this.PlistDict = (Dictionary<string,object>)machinePlistData[this.AssetName];
			}
			else
			{
				throw new Exception(this.name + ".plist.xml is Empty");
			}
		}
		
        //整合关卡自带的linetables
        InitAllLineTables();

        //主的strip等设置
        this.MainBoardStripConfig.InitStripConfig(PlistDict,this);
        
        //加载Spine动画
        this.UseSpine = Utilities.GetBool(PlistDict, USE_SPINE, false);

		// CoroutineUtil.Instance.StartCoroutine(this.InitSpineAsset());
        
        if(PlistDict.ContainsKey(SUB_BOARDS))
        {
            this.SubBoardStripConfigList = new List<BoardStripConfig>();
            List<object> list = PlistDict[SUB_BOARDS] as List<object>;
            for(int i =0; i < list.Count; i++)
            {
                Dictionary<string, object> o = list[i] as Dictionary<string, object>;
                BoardStripConfig config = new BoardStripConfig();
                config.InitStripConfig(o,this);
                this.SubBoardStripConfigList.Add(config);
            }
        }

		// #if DEBUG || UNITY_EDITOR
        if (PlistDict.ContainsKey(MachineTestDataFromPlistConfig.TEST_RESULT_DATA_LIST_CONFIG))
		{
			this.machineTestDataFromPlistConfig = new MachineTestDataFromPlistConfig(this.MainBoardStripConfig.SymbolMap, PlistDict[MachineTestDataFromPlistConfig.TEST_RESULT_DATA_LIST_CONFIG] as Dictionary<string,object>);
		}
		// #endif
		if (PlistDict.ContainsKey(ExtroInfos.EXTRA_INFOS))
		{
			this.extroInfos = new ExtroInfos(PlistDict[ExtroInfos.EXTRA_INFOS] as Dictionary<string,object>);
		}
		else
		{
			this.extroInfos = new ExtroInfos(null);
		}
		if (PlistDict.ContainsKey(FakeStrip.ReelData_INFOS))//&&SpinUseFakeStrip)
		{
			this.FakeStrip = new FakeStrip(PlistDict[FakeStrip.ReelData_INFOS] as Dictionary<string,object>,MainBoardStripConfig.SymbolMap);
		}
		else
		{
			this.FakeStrip = new FakeStrip(null,MainBoardStripConfig.SymbolMap);
		}

		List<object> blackOut = null;

		if (PlistDict.ContainsKey(ClassicPaytable.BLACK_OUT_PAYTABLE))
		{
			blackOut = PlistDict[ClassicPaytable.BLACK_OUT_PAYTABLE] as List<object>;
		}

	

		if (PlistDict.ContainsKey(BecomeWildStripsProbabilityTable.BECOME_WILD_STRIPS_PROBABILITY_TABLE))
		{
			this.becomeWildStripsProbabilityTable = new BecomeWildStripsProbabilityTable(PlistDict[BecomeWildStripsProbabilityTable.BECOME_WILD_STRIPS_PROBABILITY_TABLE] as List<object>);
		}
		else
		{
			this.becomeWildStripsProbabilityTable = new BecomeWildStripsProbabilityTable(null);
		}

		if (PlistDict.ContainsKey(ReelManagerDataConfig.REEL_MANAGER_DATA_CONFIG))
		{
			this.reelManagerDataConfig = new ReelManagerDataConfig(PlistDict[ReelManagerDataConfig.REEL_MANAGER_DATA_CONFIG] as Dictionary<string,object>);
		}
		else
		{
			this.reelManagerDataConfig = new ReelManagerDataConfig(null);
		}

		if (PlistDict.ContainsKey(WILD_ACCUMULATION))
		{
			this.wildAccumulation = Utilities.CastValueBool(PlistDict[WILD_ACCUMULATION], true);
		}
		else
		{
			this.wildAccumulation = true;
		}

		if (PlistDict.ContainsKey(HAS_SPECIAL_SLOT_SOUND))
		{
			this.hasSpecialSlotSound = Utilities.CastValueBool(PlistDict[HAS_SPECIAL_SLOT_SOUND], true);
		}
		else
		{
			this.hasSpecialSlotSound = false;
		}
		if (PlistDict.ContainsKey(RATE_ALERT_NAME))
		{
			this.RateShowName = PlistDict[RATE_ALERT_NAME] as string;
		}

		long defaultBet =  DEFAULT_MIN_BET;

		if (PlistDict.ContainsKey(BET_LIMIT)) {
			defaultBet = Utilities.GetLong(PlistDict[BET_LIMIT] as Dictionary<string,object>, MIN_BET, defaultBet);
		}
        
        if (minBet < defaultBet)
        {
        	minBet = defaultBet;
        }

		#region High Roller
		if (PlistDict.ContainsKey(HIGH_ROLLER_CONFIG)) {
			Dictionary<string,object> highrollerDict = PlistDict[HIGH_ROLLER_CONFIG] as Dictionary<string,object>;
			if (highrollerDict.ContainsKey(IS_HIGH_ROLLER_MACHINE)) {
				this.IsHighRollerMachine = Utilities.CastValueBool(highrollerDict[IS_HIGH_ROLLER_MACHINE], false);
			}

			if (highrollerDict.ContainsKey(DIS_PLAY_COUNT)) {
				this.HighRollerDisPlayCount = Utilities.CastValueInt(highrollerDict[DIS_PLAY_COUNT],2);
			}

			if (highrollerDict.ContainsKey(DISPLAY_INTERVAL)) {
				this.HighRollerDisplayInterval = Utilities.CastValueInt(highrollerDict[DISPLAY_INTERVAL],86400);
			}

			if (highrollerDict.ContainsKey(DISPLAY_SHOW_TIME)) {
				this.HighRollerDisplayTime = Utilities.CastValueInt(highrollerDict[DISPLAY_SHOW_TIME],2);
			}

		}
		#endregion
		
		//后端化的带子数据
		if (PlistDict.ContainsKey(SlotControllerConstants.SERVER_SPIN_DATA_PARENT))
		{
			ServerReelData = PlistDict[SlotControllerConstants.SERVER_SPIN_DATA_PARENT];
		}
	
		IsDataInit = true;
		
	}

    public void InitFreeSpinStrip()
    {
	    FSStripConfig = new BoardStripConfig();
	    Dictionary<string, object> o = extroInfos.GetSubInfos(FreespinGame.FreespinSlotMachine);
	    if (o != null)
	    {
			FSStripConfig.InitStripConfig(o,this);
	    }
    }

	#endregion

	public bool IsUseDefaultPlist { get; private set;}
	public string CurSlotPlistFileName{ get; private set;}

  
    public ServerData serverMath;
	public byte[] GetPlistTextConfig(string sceneName)
	{
        CurSlotPlistFileName = sceneName + ".plist";
		return GetPkgRawPlistData(sceneName);
    }

    public static Dictionary<string,object> GetMachinePlistDataInEditor(string sceneName){
        byte[] plistData= GetPkgRawPlistData(sceneName);
        Dictionary<string, object> plistInfo=null;
        if (plistData != null && plistData.Length > 0)
        {
            Dictionary<string, object> machinePlistData = CSharpUtil.ParsePlistByteArray(plistData);
            plistInfo = (Dictionary<string, object>)machinePlistData[sceneName];
        }

        return plistInfo;
    }

    private static byte[] GetPkgRawPlistData(string fileName){
        byte[] bufferData = null;
        try
        {
            string filePath = "";
            TextAsset textAsset = null;
            Action fileLoadAction = () => {
                filePath = AssetsPathManager.GetPlistResourcePathInPackageOnMobile() + fileName + ".plist";
                textAsset = Resources.Load(filePath) as TextAsset;
            };

            fileLoadAction();
            if (textAsset == null)
            {
                fileLoadAction();
            }
            if (textAsset == null || textAsset.bytes == null)
            {
                return null;
            }
            bufferData = textAsset.bytes;
        }
        catch (Exception ex)
        {
            Utils.Utilities.LogSwapSceneException("Exception: Read Package Raw Plist Data is Failed!");
        }
        return bufferData;
    }
    
    private static byte[] GetPlistData(string fileName){
	    byte[] bufferData = null;
	    try
	    {
		    string filePath = "";
		    TextAsset textAsset = null;
		    
		    Action fileLoadAction = () => {
			    textAsset = ResourceLoadManager.Instance.LoadResource<TextAsset>(fileName);
		    };

		    fileLoadAction();
		    if (textAsset == null)
		    {
			    fileLoadAction();
		    }
		    if (textAsset == null || textAsset.bytes == null)
		    {
			    return null;
		    }
		    bufferData = textAsset.bytes;
	    }
	    catch (Exception ex)
	    {
		    Utils.Utilities.LogSwapSceneException("Exception: Read Package Raw Plist Data is Failed!");
	    }
	    return bufferData;
    }
    private byte[] GetLocalPlistData(string filePath,bool IsEncrypt) {
        byte[] bufferData = null;
        if (!File.Exists(filePath))
        {
            return bufferData;
        }
        try
        {
            byte[] data = null;
            Action loadData = () => {
                data = File.ReadAllBytes(filePath);
            };
            loadData();
          
            if (data==null)
            {
                return null;
            }
            if (IsEncrypt)
            {
                bufferData = FileUtils.GetRawData(data);
            }
            else
            {
                bufferData = FileUtils.DecompressData(data);
            }
            if (bufferData==null)
            {
                Utils.Utilities.LogSwapSceneException("Exception: Read Local Plist Data is Failed, Slot Plist File And ReadConfig not match EncryptOrCompress!");
            }

        }
        catch (Exception ex)
        {
            Utils.Utilities.LogSwapSceneException("Exception: Read Local Plist Data is Failed!" + ex.Message);
        }
        return bufferData;
    }
    private byte[] GetPkgPlistData(string fileName,string defaultFileName,bool IsEncryptData){
        byte[] bufferData = null;
        try
        {
            string filePath = "";
            TextAsset textAsset = null;
            Action<string> fileLoadAction = (slotName)=>{
                filePath = AssetsPathManager.GetPlistFinalResourcePathInPackageOnMobile() + slotName + ".plist.zip";
                textAsset = Resources.Load(filePath) as TextAsset;
            };

            fileLoadAction(fileName);
          
            if (textAsset==null)
            {
                IsUseDefaultPlist = true;
                fileLoadAction(defaultFileName);
            }
            if (textAsset==null||textAsset.bytes==null)
            {
                return null;
            }

            if (ApplicationConfig.GetInstance().EnableReadEncryptPlistInPackage ||IsEncryptData)
            {
                bufferData = FileUtils.GetRawData(FileUtils.MergeByte(textAsset.bytes));
            }
            else
            {
                bufferData = FileUtils.DecompressData(FileUtils.MergeByte(textAsset.bytes));
            }
            if (bufferData == null)
            {
                Utils.Utilities.LogSwapSceneException("Exception: Read Package Plist Data is Failed, Slot Plist File And ReadConfig not match EncryptOrCompress!");
            }
        }
        catch (Exception ex)
        {
            Utils.Utilities.LogSwapSceneException("Exception: Read Package Plist Data is Failed!");
        }
        return bufferData;
    }
    

	public void InitJackPotIncrease(List<object> originJackpotsIncreaseDatas)
	{
		if (originJackpotsIncreaseDatas == null) return;
		for (int i = 0; i < originJackpotsIncreaseDatas.Count; i++)
		{
			Dictionary<string,object> jackpot = originJackpotsIncreaseDatas[i] as Dictionary<string,object>;
			JackPotIncreaseMachineConfig config = new JackPotIncreaseMachineConfig(jackpot);
			this.JackPotIncreaseConfigs.Add(config);
		}
	}

	public void InitJackPotPrize(List<object> jackPotPrizes)
	{
		if (jackPotPrizes == null) return;
		for (int i = 0; i < jackPotPrizes.Count; i++)
		{
			Dictionary<string , object> jackpotPrize = jackPotPrizes[i] as Dictionary<string,object>;
			JackPotPrizePool prizePool = new JackPotPrizePool(jackpotPrize, this.name);
			this.JackPotPrizeInfos.Add(prizePool);
		}
	}

	public void InitLobbyShowJackInfo(Dictionary<string, object> LobbyShowJackpotInfo)
	{
		if (LobbyShowJackpotInfo != null)
		{
			LobbyShowJackpotGrand = Utils.Utilities.GetInt(LobbyShowJackpotInfo,"GrandValue",0);
			LobbyShowJackpotIncrease = Utils.Utilities.GetInt(LobbyShowJackpotInfo,"IncreaseMaxEverySeconds",0);
		}
		else
		{
			// 小于零代表本主题没有大厅蓝色Jackpot
			LobbyShowJackpotGrand = -1;
			LobbyShowJackpotIncrease = -1;
		}
	}
	
	public JackPotIncreaseMachineConfig GetJackPotIncreaseConfig(string awardName)
	{
		for (int i = 0; i < JackPotIncreaseConfigs.Count; i++)
		{
			if (!string.IsNullOrEmpty(awardName) && awardName.Equals(JackPotIncreaseConfigs[i].JackPotType))
			{
				return JackPotIncreaseConfigs[i];
			}
		}
		return null;
	}
	public JackPotPrizePool GetJackPotPool(string awardName)
	{
		for (int i = 0; i < JackPotPrizeInfos.Count; i++)
		{
			if (!string.IsNullOrEmpty(awardName) && awardName.Equals(JackPotPrizeInfos[i].AwardName))
			{
				return JackPotPrizeInfos[i];
			}
		}
		return null;
	}
	/// <summary>
	/// 2.1.0只会有一种类型出现
	/// Maxs the jackpot number increase config.
	/// </summary>
	/// <returns>The jackpot number increase config.</returns>
	public JackPotIncreaseMachineConfig MaxJackpotNumberIncreaseConfig()
	{
//		JackPotIncreaseMachineConfig result = null;
		for (int i = 0; i < JackPotIncreaseConfigs.Count; i++)
		{
			if (!string.IsNullOrEmpty(JackPotIncreaseConfigs[i].JackPotType))
			{
				return JackPotIncreaseConfigs[i];
			}
		}
		return null;
	}
	
	public class ServerData{
        public string plistName = "";
        public float GameConfigRunV = 0f;
        public int noWinRequestPossibility = -1;
        public ServerData(Dictionary<string,object> dict){
            if (dict == null) return;

            plistName = CSharpUtil.GetValueWithPath<string>(dict, "math/plist_name", "");
            noWinRequestPossibility = CSharpUtil.GetValueWithPath<int>(dict,"math/nm_sample_rate",-1);
            GameConfigRunV = CSharpUtil.GetValueWithPath<float>(dict, "display_config/runv", 0f);
        }
    }

	private string remoteBundleURL;
	private string remoteCardVersion;

	private string name = "SlotMachineConfig";

	private string comparePath = "";

	private int requiredLevel = 0;

	private int prenoticeUnlockLevel = 0;

	private Dictionary<string,object> properties = new Dictionary<string, object>();
	 
	#region 加载spine动画
	public Dictionary<int, List<SpineResourceAsset>> SpineAsset = new Dictionary<int, List<SpineResourceAsset>>();
	
	public IEnumerator InitSpineAsset(Action<float> progressCallback = null)
	{
	    // 阶段1：加载plist文件 (20%进度)
	    bool loadSucceed = false;
	    TextAsset spineInfo = null;
	    string plistName = this.name + "/SpineAnimation/animation.plist.xml";
	    
	    CoroutineUtil.Instance.StartCoroutine(
	        Libs.ResourceLoadManager.Instance.AsyncLoadResource<TextAsset>(
	            plistName,
	            (path, asset) => {
	                loadSucceed = true;
	                spineInfo = asset;
	            }
	        )
	    );

	    while (!loadSucceed)
	    {
	        progressCallback?.Invoke(0.2f * Mathf.Clamp01(Time.time)); // 模拟进度
	        yield return null;
	    }

	    if(spineInfo == null)
	    {
	        Debug.LogError($"{this.name} InitSpineAsset Error: Failed to load plist");
	        progressCallback?.Invoke(1f); // 即使失败也标记完成
	        yield break;
	    }
	    progressCallback?.Invoke(0.2f);

	    // 阶段2：解析基础数据 (10%进度)
	    Dictionary<string,object> dataInfo = CSharpUtil.ParsePlistByteArray(spineInfo.bytes);
	    if(!dataInfo.ContainsKey("SymbolAnimationMap"))
	    {
	        Debug.LogError($"{this.name} InitSpineAsset Error: Missing SymbolAnimationMap");
	        progressCallback?.Invoke(1f);
	        yield break;
	    }
	    progressCallback?.Invoke(0.3f);

	    if(this.MainBoardStripConfig?.SymbolMap == null)
	    {
	        Debug.LogError($"{this.name} InitSpineAsset Error: Invalid SymbolMap");
	        progressCallback?.Invoke(1f);
	        yield break;
	    }

	    // 阶段3：加载所有Spine资源 (70%进度)
	    Dictionary<string,object> animationMap = dataInfo["SymbolAnimationMap"] as Dictionary<string,object>;
	    int totalAnimations = animationMap.Count;
	    int completedAnimations = 0;

	    foreach (var item in animationMap)
	    {
	        if(item.Value == null) continue;

	        List<object> itemInfo = item.Value as List<object>;
	        List<SpineResourceAsset> resourceList = new List<SpineResourceAsset>();

	        foreach (var animation in itemInfo)
	        {
	            if(animation == null) break;

	            Dictionary<string,object> aniInfo = animation as Dictionary<string,object>;
	            if(!aniInfo.ContainsKey("animation_id") || 
	               !aniInfo.ContainsKey("animation_name") || 
	               !aniInfo.ContainsKey("animation_spine"))
	            {
	                break;
	            }

	            // 创建资源对象
	            SpineResourceAsset resourceAsset = new SpineResourceAsset(
	                Utils.Utilities.CastValueInt(aniInfo["animation_id"]),
	                aniInfo["animation_name"].ToString(),
	                aniInfo.ContainsKey("animation_skin") ? aniInfo["animation_skin"].ToString() : "default",
	                aniInfo.ContainsKey("animation_clip") ? Utils.Utilities.CastValueBool(aniInfo["animation_clip"]) : false
	            );

	            // 加载所有骨架数据
	            foreach (var name in aniInfo["animation_spine"] as List<object>)
	            {
	                yield return LoadSkeletonDataAsset(name.ToString(), resourceAsset);
	            }
	            resourceList.Add(resourceAsset);
	        }

	        // 存储有效资源
	        if(resourceList.Count > 0)
	        {
	            int symbolIndex = this.MainBoardStripConfig.SymbolMap.getSymbolIndex(item.Key);
	            if(!SpineAsset.ContainsKey(symbolIndex))
	            {
	                SpineAsset.Add(symbolIndex, resourceList);
	            }
	        }

	        // 更新进度
	        completedAnimations++;
	        progressCallback?.Invoke(0.3f + 0.7f * (completedAnimations / (float)totalAnimations));
	    }

	    progressCallback?.Invoke(1f);
	}

	public IEnumerator LoadSkeletonDataAsset(string aniAsset, SpineResourceAsset resourceAsset)
	{
		bool loadSucceed = false;
		Spine.Unity.SkeletonDataAsset spineData = null;
		CoroutineUtil.Instance.StartCoroutine(Libs.ResourceLoadManager.Instance.AsyncLoadResource<Spine.Unity.SkeletonDataAsset> (
			this.name + "/SpineAnimation/" + aniAsset + "_SkeletonData.asset",(path,asset) =>
		{
			loadSucceed = true;
			spineData = asset;
		}));

		while (!loadSucceed)
		{
			yield return GameConstants.FrameTime;
		}

		if(spineData == null)
		{
			Debug.LogError(this.name + "LoadSkeletonDataAsset failed! Skeleton Name is: " + aniAsset);
			yield break;
		}
		resourceAsset.AddSkeletonDataAsset(spineData);
	}

	#endregion

	#region 清除关卡里的数据

	public void ClearSpineData()
	{
		SpineAsset.Clear();
		
	}

	public void ClearStaticData()
	{
		LineTableDict.Clear();
		this.MainBoardStripConfig.Clear();
		if (this.SubBoardStripConfigList != null)
		{
			this.SubBoardStripConfigList.Clear();
		}
		
		this.extroInfos = null;
		
		reelManagerDataConfig = null;
		
		if(this.PlistDict!=null)
		{
			this.PlistDict.Clear();
		}

		ServerReelData = null;
	}

	#endregion
}
