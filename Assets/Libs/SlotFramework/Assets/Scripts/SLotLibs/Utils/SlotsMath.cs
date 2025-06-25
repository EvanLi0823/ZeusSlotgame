using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public class SlotsMath  {
	private static readonly string PAY= "pay";
	private static readonly string WEIGHT= "weight";

	public static int GetWeightIndexInArray(WheelElement[] WheelElements)
	{
		float totalWeight = 0;
		
		for (int i=0; i<WheelElements.Length; i++) {
			totalWeight += WheelElements[i].Weight;
		}
		
		int temp =UnityEngine.Random.Range (0,(int) totalWeight);
		int AwardIndex = WheelElements.Length-1;
		
		float lastWeights = 0;
		float weights = WheelElements [0].Weight;
		for (int i=1; i<WheelElements.Length; i++) {
			if (lastWeights <= temp && temp < weights) {
				AwardIndex = i - 1;
				return AwardIndex;
			} else {
				lastWeights = weights;
				weights += WheelElements[i].Weight;
			}
		}
		return AwardIndex;
	}


	public static List<WheelElement> CreateWheelElements(object o)
	{
		List<WheelElement> wheelElements = new List<WheelElement>();
		if (o == null) {
			return wheelElements;
		}
		List<object> list = o as List<object>;
		for (int i = 0; i < list.Count; i++)
		{
			Dictionary<string, object> dic = list[i] as Dictionary<string, object>;
			float _pay = Utils.Utilities.CastValueFloat(dic[PAY]);
			float _weight = Utils.Utilities.CastValueFloat(dic[WEIGHT]);
			WheelElement e = new WheelElement(_weight, _pay);
			wheelElements.Add(e);
		}
		return wheelElements;
	}
	// /// <summary>
	/// weight 相关
	/// </summary>
	/// <returns>The weight index in array.</returns>
	/// <param name="WeightElements">Weight elements.</param>
	public static WeightElement GetWeightIndexInArray(List<WeightElement> WeightElements)
	{
		float totalWeight = 0;

		for (int i=0; i<WeightElements.Count; i++) {
			totalWeight += WeightElements[i].Weight;
		}

		float temp =UnityEngine.Random.Range (0f, totalWeight);
		int AwardIndex = WeightElements.Count-1;

		float lastWeights = 0;
		float weights = WeightElements [0].Weight;
		for (int i=1; i<WeightElements.Count; i++) {
			if (lastWeights <= temp && temp < weights) {
				AwardIndex = i - 1;
				break;
			} else {
				lastWeights = weights;
				weights += WeightElements[i].Weight;
			}
		}
        WeightElements[AwardIndex].index = AwardIndex;
        return  WeightElements[AwardIndex];
	}

	public static List<WeightElement> CreateWeightElements(object o,string SymbolKey = "Symbol",string WeightsKey = "Weights")
	{
		List<WeightElement> weightElements = new List<WeightElement>();
		if (o == null) {
			return weightElements;
		}
		List<object> list = o as List<object>;
		for (int i = 0; i < list.Count; i++)
		{
			Dictionary<string, object> dic = list[i] as Dictionary<string, object>;
			string _name = dic[SymbolKey].ToString();
			float _weight = Utils.Utilities.CastValueFloat(dic[WeightsKey]);
			WeightElement e = new WeightElement(_name,_weight );
			weightElements.Add(e);
		}
		return weightElements;
	}
}


[Serializable]
public class WheelElement
{
	public const string WEIGHT = "weight";
	public const string PAY = "pay";
	public const string IS_FREESPIN = "IsFreeSpin";
	public const string IS_MAX_VALUE = "IsMaxValue";
	
	public int WheelIndex = 0;
    public int Extension = 0;
	public float Weight;
	public float Pay;
	public bool isFreeSpin = false;
	public bool isMaxValue = false;
	public WheelElement(float _weight, float _pay)
	{
		this.Weight = _weight;
		this.Pay = _pay;
	}

	public WheelElement(int _wheelIndex,float _weight, float _pay,int _extension)
    {
        this.WheelIndex = _wheelIndex;
        this.Weight = _weight;
        this.Pay = _pay;
        this.Extension = _extension;
    }
	
	public WheelElement(Dictionary<string,object> dict){
		Pay = Utils.Utilities.GetFloat(dict,PAY,0);
		Weight = Utils.Utilities.GetFloat(dict,WEIGHT,0);
		isFreeSpin = Utils.Utilities.GetBool(dict,IS_FREESPIN,false);
		isMaxValue = Utils.Utilities.GetBool(dict, IS_MAX_VALUE, false);
	}
}

[Serializable]
public class WeightElement
{
	public float Weight;
	public string name;
    public int index;
	public WeightElement(string _name,float _weight)
	{
		this.name = _name;
		this.Weight = _weight;
	}

	public WeightElement(Dictionary<string,object> dict,string SymbolKey="Symbol",string WeightsKey ="Weights")
	{
		 name = dict[SymbolKey].ToString();
		 Weight = Utils.Utilities.CastValueFloat(dict[WeightsKey]);
	}
}
