using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReelRunData
{
	//run的索引
	public int RunIndex;

	public bool IsAnticipation;

	public AnimationCurve ReelCurve;

	//	public GameObject AntiGameObject;
	public float AntiStartTime;
	public bool IsLastReel;

	//结果数据
	public List<int> ResultSymbols;

	//转动用的数据数组 结果数据目前没用到，没放入到里面
	public List<int> ReelSpinData;
//	public List<ResultContent.WeightData> RunStrip;

	//留作备用
	public int StartInData;

	//调整速度来用
	private float _RunTimeScale = 1f;
	private float _RunRouteScale = 1f;

	//是否是掉落模式，用在pixels关卡
	public bool IsAwardDropMode = false;
	//记录掉落时候的索引
	public List<int> AwardIndexList;

	public float RunTimeScale {
		private set{ }
		get{
			return _RunTimeScale;
		}
	}


	public float RunLengthScale {
		private set{ }
		get{
			return _RunRouteScale;
		}
	}

	public ReelRunData(int _index,bool _isAnti,AnimationCurve _curve,bool _isLast)
	{
		this.RunIndex = _index;
		this.IsAnticipation = _isAnti;
		this.ReelCurve = _curve;
		this.IsLastReel = _isLast;

		if (this.IsAnticipation) {
			AntiStartTime = this.ReelCurve.keys [2].time;
		}
	}

	public void SetFastScale(float _timeScale, float _routeScale)
	{
		_RunTimeScale = _timeScale;
		_RunRouteScale = _routeScale;
	}

}

//public class WeightData
//{
//	public int value;
//	public int weight;
//	public string SymbolName;
//	public WeightData (int value, int weight = 1,string _SymbolName="")
//	{
//		this.value = value;
//		this.weight = weight;
//		this.SymbolName = _SymbolName;
//	}
//}