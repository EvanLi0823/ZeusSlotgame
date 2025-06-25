using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
/// <summary>
/// Reel strip manager.
/// 需要考虑的特定关卡有goldslots的点金术和DoubleStar
/// classicSlots的doubleStar的rewin
///2020.2.28 LQ
///新版的管理器需要达到以下要求：
///1.提供统一的获取带子接口(无论是使用真实带子还是虚拟带子SPIN，对外部来说是透明的)
///2.提供统一的结果牌面接口(无论是服务器pattern还是从本地带子中创建产生或者直接选中产生，亦或者是其他的渠道)
///3.对带子进行拼接，由ReelController等UI层类来完成
///4.机器初始化时，必须使用数据带子初始化，防止初始牌面发生变化，转动时，在替换为虚拟带子
///5.在旋转过程中，拼接不更改显示带子数据，而是根据具体关卡，实时进行替换，可以参考PirateBonanzaReel和FortuneOfSantaReel 衔接点有三处:正常停止前，急停前，开始旋转时
///6.每个关卡根据自身特点，进行自定义实现衔接点
/// </summary>
namespace Classic
{
	public class ReelStripManager
	{
		public readonly static string MACHINE_MATH_NODE = "MachineMath";
		private RealReelStrip realReelStrip; //真实带子

		#region 机器数据解析

		public ReelStripManager(SymbolMap symbolMap, Dictionary<string, object> info)
		{
			if (info == null)
			{
				return;
			}

			if (realReelStrip == null)
			{
				realReelStrip = new RealReelStrip(symbolMap, info);
			}
		}

		#endregion

		#region 真实数据层接口

		public DataFilterResult selectedResult
		{
			get { return realReelStrip.selectedResult; }
			set { realReelStrip.selectedResult = value; }
		}

		public string defaultRName {
			get { return realReelStrip.defaultRName; }
			set { realReelStrip.defaultRName = value; }
		}

		public string defaultAName
		{
			get { return realReelStrip.defaultAName; }
			set { realReelStrip.defaultAName = value; }
		}

		public string GetCurrentUseRName(){
			return realReelStrip.GetCurrentUseRName();
		}
		public string GetCurrentUseAName(){
			return realReelStrip.GetCurrentUseAName();
		}
		public string GetCurrentUseCName(){
			return realReelStrip.GetCurrentUseCName();
		}
		public bool CanMeetTheCondition(List<ReelStrip> currentStripList)
		{
			return realReelStrip.CanMeetTheCondition(currentStripList);
		}

		public List<ReelStrip> GetSelectedStrips()
		{
			return realReelStrip.GetSelectedStrips();
		}

		public void InitStrips()
		{
			realReelStrip.InitStrips();
		}
		public List<ReelStrip> GetInitStrips()
		{
			return realReelStrip.GetInitStrips();
		}
		
		public void ResetRAData(string r, string a, List<List<int>> fixedResult)
		{
			realReelStrip.ResetRAData(r,a,fixedResult);
		}

		#endregion
		
	}
}
