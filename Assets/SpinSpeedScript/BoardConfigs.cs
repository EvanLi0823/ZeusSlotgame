using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BoardConstant
{
    public const string START_QUICK_STOP_HANDLER = "StartQuickStopHandler";
}

//棋盘
[Serializable]
public class BoardConfigs : MonoBehaviour {
   	[Header("整个棋盘的宽度")]
	public float ReelPanelWidth = 1374f;
	[Header("整个棋盘的高度")]
	public float ReelPanelHeight = 688f;

	[Header("单个轮子的宽度")]
	public float ReelWidth = 224f;
	[Header("单个symbol的高度")]
	public float SymbolHeight = 172f;

	[Header("棋盘与棋盘的空间")]
	public float ReelSpace =6f;

	public GameObject ReelPrefab;
	public GameObject SymbolRender;


	[Header("用于Classic含有Blank的移植机器")]
	public bool IsBlank = false;
	[Header("用于横向轮子旋转")]
	public bool IsLandspace = false;

	[Header("轮子的配置")]
	public ReelConfig[] ReelConfigs;

	[Header("是否是单独小的排版，如respin")]
	public bool IsSmallMode;

	[Header("同层symbol的层级配置，是否上压下")]
	public bool IsUpOverDown = false;

    [Header("当layout初始化的时候是否需要销毁animation动画层，如关卡奢华男是不需要的")]
    public bool IsReserveAnimation = false;

    //	public SymbolConfig[] SymbolConfigs;
    public float ReelShowHeight(int reelIndex)
	{
		if(IsLandspace) return this.SymbolHeight * ReelConfigs [reelIndex].RenderMultiple;

		if(IsBlank) return int.Parse(Math.Floor(ReelConfigs [reelIndex].ReelShowNum/2F).ToString())  * this.SymbolHeight * ReelConfigs [reelIndex].RenderMultiple;

		return ReelConfigs[reelIndex].ReelShowNum * this.SymbolHeight * ReelConfigs [reelIndex].RenderMultiple;    
	}

	public float ReelShowWidth(int reelIndex)
	{
		if(IsLandspace) return ReelConfigs[reelIndex].ReelShowNum * this.ReelWidth * ReelConfigs[reelIndex].RenderMultiple;

		return ReelWidth * ReelConfigs[reelIndex].RenderMultiple;
	}

	public float ReelPositionX()
	{
		if(IsLandspace) return 0;
		return ReelWidth/2 - ReelPanelWidth / 2f;
	}

//	private Dictionary<string,SymbolConfig> m_SymbolConfigDict = new Dictionary<string,SymbolConfig>();
//	public SymbolConfig GetSymbolConfigByName(string str)
//	{
//		if (m_SymbolConfigDict.ContainsKey (str)) {
//			return m_SymbolConfigDict [str];
//		}
//		Debug.LogError (str+"的symbol配置有问题");
//		return null;
//	}

	void Awake()
	{
		#if TEST_DEMO
		//1 初始化symbol名字对应的dict
		m_SymbolConfigDict.Clear ();
		for (int i = 0; i < this.SymbolConfigs.Length; i++) {
			this.m_SymbolConfigDict [this.SymbolConfigs [i].SymbolName] = this.SymbolConfigs [i];
		}

		//2. init symbol 带子
		for (int i = 0; i < this.ReelConfigs.Length; i++) {
			this.ReelConfigs [i].InitSymbolWeight ();
		}
		#endif
	}
}

//symbol
[Serializable]
public class SymbolConfig
{
	[Header("symbol图片")]
	public Sprite SymbolSprite;
	[Header("显示的层次")]
	public int DisplayLevel = 0;

//	[HideInInspector]
	[Header("占用的symbol高度倍数")]
	public int HeightMultiple = 1; //解决占用两个symbol高度的问题，将来解决吧。


	public string SymbolName
	{
		get{
			return SymbolSprite.name;
		}
	}
}

//轮子
[Serializable]
public class ReelConfig 
{
//	#if ART_DEMO
//	public int ComputeSymbolNum
//	{
//		get{
//			return ReelShowNum + 1;
//		}
//	}
//	#else
	[SerializeField]
	public int ComputeSymbolNum = 5;//计算的symbols
//	#endif
	[SerializeField]
	public int ReelShowNum=4; //显示的symbol个数
	[SerializeField]
	public int RenderMultiple = 1;

	[Header("牌面初始化时的position")]
	[SerializeField]
	public int BeginPosition = 0;
    [Header("带子相对居中位置在Y轴上的偏移")]
    public int ReelYOffset = 0;
    #if TEST_DEMO
	[Header("带子模拟数据")]
	[SerializeField]
	private string ReelSymbolSimulateData;

	[HideInInspector]
	public  List<WeightData> ReelWeightData;

	public void InitSymbolWeight()
	{
		ReelWeightData = GetReelSymbolData ();
	}

	private List<WeightData> GetReelSymbolData()
	{
			if (string.IsNullOrEmpty(ReelSymbolSimulateData)) {
				return null;
			}

		string[] array = ReelSymbolSimulateData.Trim().Split (',');
			if (array == null) {
				return null;
			}

			List<WeightData> result = new List<WeightData> ();
			for (int i = 0; i < array.Length; i++) {
				result.Add (new WeightData (-1,1,array[i]));
			}

			return result; //new List<string>( ReelSymbolSimulateData.Split (','));

	}

#endif
}