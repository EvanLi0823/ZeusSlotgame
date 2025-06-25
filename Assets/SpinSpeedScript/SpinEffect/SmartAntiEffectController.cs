using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Classic;
/// <summary>
/// Spin effect controller.
/// Include Function:
/// 1.Aniticipatioin
/// 2.Blink
/// 3.SmartSound
/// 4.Bonus Animation
/// 设计目标实现不依赖其他控制器类，比如ReelManager
/// 需要配置不同的组，组里包括symbols、可能性列表、触发个数，类型
/// smartSound的音效需要从索引0开始，如音效的名字为smartSound0
/// antis的动画播放其实很简单，如果播放smartSound了，则在smartSound的当前reel加1（除非是最后一个轮子），当然是在存在的轮子上
/// </summary>
public class SmartAntiEffectController : MonoBehaviour {
    //	public const string SMART_GROUP_ID = "SmartGroupID";
    public enum SmartAntiType
    {
        Continuous,//第一轴起，连续出现等价图标
        Scatter,//不需要连续出现
        SameLine //20190724 ，同一行的播放animation按照出现几个才做动画，后续扩展
    }

    [Serializable]
    public class SmartConfigs {
        [Header("同一组SmartSymbol的名字(要求与SymbolMap里的名字相同)")]
        public List<string> SmartSymbolName = new List<string>();
        [Header("每轴可能出现的最大SmartSymbol数量")]
        public List<int> ReelMaxAppearCountList = new List<int> { 1, 1, 1, 1, 1 };
        [Header("触发Feature的最大SmartSymbol数量")]
        public int TriggerCount = 3;
        [Header("SmartSymbol出现方式(是否要求连续轴出现SmartSymbol)")]
        public SmartAntiType SmartType = SmartAntiType.Scatter;
        [Header("触发Anticipation前，出现SmartSymbol的轴数")]
        public int SmartTriggerAntiReelCount = 1;//出现几个SmartSymbol才会触发Anticipation
        [HideInInspector]
        public int AllPossibleNum;
    }

    void Awake() {
        if (SmartConfig == null || SmartConfig.Count == 0)
            return;

        for (int i = 0; i < SmartConfig.Count; i++) {
            SmartConfigs smartConfig = SmartConfig[i];
            //记录总的出现个数，然后减去当前可能的总个数即为将来可能出现的个数
            int allPossibleNum = 0;
            smartConfig.ReelMaxAppearCountList.ForEach(delegate (int obj) {
                allPossibleNum += obj;
            });
            smartConfig.AllPossibleNum = allPossibleNum;
        }
    }
    [Header("Unity动画清除时间，慎用")]
    public float ClearAnimationTime = -1;
    public List<SmartConfigs> SmartConfig;
    protected List<int> AntiReels = new List<int>();
    protected Dictionary<int, Dictionary<int, bool>> SmartPositions = new Dictionary<int, Dictionary<int, bool>>();
    public virtual List<int> CheckSmartPositionAndAnticipation(List<ResultContent.ReelResult> reelsSpinData, SymbolMap symbolMap, ReelManager reelManager)
    {
        StopAllSmartAndAntiEffectAnimation(reelManager);
        SmartPositions.Clear();
        AntiReels.Clear();
        stopBlinkAniList.Clear();

        if (SmartConfig == null || SmartConfig.Count == 0)
            return AntiReels;

       

        for (int i = 0; i < SmartConfig.Count; i++) 
        {
            SmartConfigs smartConfig = SmartConfig[i];
         
            if (smartConfig.AllPossibleNum == 0) {
                continue;
            }

            if (smartConfig.SmartType == SmartAntiType.Continuous || smartConfig.SmartType == SmartAntiType.Scatter)
            {
                ContinuousScatterAnticipation(smartConfig, reelsSpinData, symbolMap);
            }
            else if(smartConfig.SmartType == SmartAntiType.SameLine)
            {
                SameLineAnticipation(smartConfig, reelsSpinData, symbolMap);
            }

        }
        PreloadSmartWebm(reelsSpinData, reelManager);
        //		Log.LogDictionary (AntiReels);
        return AntiReels;
    }

    protected virtual void ContinuousScatterAnticipation(SmartConfigs smartConfig, List<ResultContent.ReelResult> reelsSpinData, SymbolMap symbolMap)
    {
        //当前轮子之前已经出现的个数,剩余最大数
        int appearSymbolNum = 0;
        int maxLastCount = smartConfig.AllPossibleNum;
        int appearSmartReelNum = 0;//每一组会对其进行重新检测
                                   //是否存在不连续的轮子
        bool existDiscontinuousReel = false;
        List<string> smartSymbols = smartConfig.SmartSymbolName;
        //j:轮子id
        for (int j = 0; j < smartConfig.ReelMaxAppearCountList.Count; j++)
        {
            //不存在，则接下去
            if (smartConfig.ReelMaxAppearCountList[j] == 0)
            {
                continue;
            }

            if (smartConfig.SmartType == SmartAntiType.Continuous && existDiscontinuousReel)
            {
                break;
            }

            //当前轮子是否存在smart
            bool _existSmartOnReel = false;
            List<int> symbols = reelsSpinData[j].SymbolResults;
            //只有当前组内，且前面满足Smart出现条件才会进入此处判定
            if (appearSmartReelNum >= smartConfig.SmartTriggerAntiReelCount)
            {
                if (!AntiReels.Contains(j)) AntiReels.Add(j);
            }
            //first add current reel smart number
            for (int k = 0; k < symbols.Count; k++)
            {
                if (SymbolIsConformInContinuousScatterAnticipation(smartSymbols,symbolMap,symbols,k,j))
                {

                    appearSymbolNum += 1;
                    if (!_existSmartOnReel)
                    {
                        _existSmartOnReel = true;
                        appearSmartReelNum++;
                    }
                }
            }

            maxLastCount -= smartConfig.ReelMaxAppearCountList[j];

            //超出了则直接退出
            //              Debug.Log(maxLastCount);
            if (appearSymbolNum + maxLastCount < smartConfig.TriggerCount)
            {
                break;
            }

            for (int k = 0; k < symbols.Count; k++)
            {
                if (SymbolIsConformInContinuousScatterAnticipation(smartSymbols,symbolMap,symbols,k,j))
                {
                    if (!SmartPositions.ContainsKey(j))
                    {
                        SmartPositions[j] = new Dictionary<int, bool>();
                    }
                    SmartPositions[j][k] = true;
                }
            }

            if (smartConfig.SmartType == SmartAntiType.Continuous)
            {
                //之前一直连续，并且当前reel不存在smart，则标记不连续
                if (existDiscontinuousReel == false && _existSmartOnReel == false)
                {
                    existDiscontinuousReel = true;
                }
            }
        }
    }

    
    /// <summary>
    /// Symbol是否符合在ContinuousScatterAnticipation类型判断中
    /// </summary>
    /// <param name="smartSymbolNames">Anticipation面板设置的Smart图标名称</param>
    /// <param name="symbolMap">SymbolMap</param>
    /// <param name="symbolResults">该列Symbol结果</param>
    /// <param name="symbolResultIndex">判断的SymbolIndex</param>
    /// <param name="reelIndex">列索引</param>d
    /// <returns></returns>
    protected virtual bool SymbolIsConformInContinuousScatterAnticipation(List<string> smartSymbolNames,SymbolMap symbolMap,List<int> symbolResults,int symbolResultIndex,int reelIndex)
    {
        
        if (smartSymbolNames == null || symbolMap == null || symbolResults == null ||
            symbolMap.getSymbolInfo(symbolResults[symbolResultIndex]).name == null) return false;
        return smartSymbolNames.Contains(symbolMap.getSymbolInfo(symbolResults[symbolResultIndex]).name);
    }

    //单独处理，不与scatter和continue混淆 , 如奢华男则只处理antiReels，播放smart动画另处理
    protected void SameLineAnticipation(SmartConfigs smartConfig, List<ResultContent.ReelResult> reelsSpinData, SymbolMap symbolMap)
    {
        //当前轮子之前已经出现的个数,剩余最大数
        int appearLineMaxSymbolNum = 0;
        int maxLastCount = smartConfig.AllPossibleNum;
        List<string> smartSymbols = smartConfig.SmartSymbolName;
        Dictionary<int, int> lineMaxSymbolCountDict = new Dictionary<int, int>();
        //j:轮子id
        for (int j = 0; j < smartConfig.ReelMaxAppearCountList.Count; j++)
        {
            //不存在，则接下去
            if (smartConfig.ReelMaxAppearCountList[j] == 0)
            {
                continue;
            }
            List<int> symbols = reelsSpinData[j].SymbolResults;
            //只有当前组内，且前面满足Smart出现条件才会进入此处判定
            if (appearLineMaxSymbolNum >= smartConfig.SmartTriggerAntiReelCount)
            {
                if (!AntiReels.Contains(j)) AntiReels.Add(j);
            }
            //first add current reel smart number
            for (int line = 0; line < symbols.Count; line++)
            {
                if (smartSymbols.Contains(symbolMap.getSymbolInfo(symbols[line]).name))
                {
                    if(!lineMaxSymbolCountDict.ContainsKey(line))
                    {
                        lineMaxSymbolCountDict[line] = 0;
                    }
                    lineMaxSymbolCountDict[line]++;
                
                    appearLineMaxSymbolNum = Mathf.Max(appearLineMaxSymbolNum, lineMaxSymbolCountDict[line]);
                }
            }

            maxLastCount -= smartConfig.ReelMaxAppearCountList[j];

            //超出了则直接退出

            if (appearLineMaxSymbolNum + maxLastCount < smartConfig.TriggerCount)
            {
                break;
            }

            //SmartPositions可能不是很统一，关卡里面单独处理

            //for (int k = 0; k < symbols.Count; k++)
            //{
            //    if (smartSymbols.Contains(symbolMap.getSymbolInfo(symbols[k]).name))
            //    {
            //        if (!SmartPositions.ContainsKey(j))
            //        {
            //            SmartPositions[j] = new Dictionary<int, bool>();
            //        }
            //        SmartPositions[j][k] = true;
            //    }
            //}
        }
    }

    public virtual void PlaySmartSoundAndAnimation(int reelIndex, ReelController controller)
    {
        if (SmartPositions.ContainsKey(reelIndex)) {
            foreach (int position in SmartPositions[reelIndex].Keys) {
                BaseElementPanel render = controller.GetSymbolRender(reelIndex, position);
                if (render != null) {
                    render.PlayAnimation((int)BaseElementPanel.AnimationID.AnimationID_SmartSymbolReminder, false, null, () =>
                    {
                        render.StopAnimation();
                    }, 0f, true);
                    Messenger.Broadcast<int>(SlotControllerConstants.PLAY_ANTICIPATION_EFFECT, reelIndex);
                    //TODO:smart的音效优化

                    PlaySmartSound(reelIndex,controller);

                    if (ClearAnimationTime == -1) continue;
                    Libs.DelayAction da = new Libs.DelayAction(ClearAnimationTime, null, () => {
                        render.StopAnimation();
                        Messenger.Broadcast<int>(SlotControllerConstants.STOP_ANTICIPATION_EFFECT, reelIndex);
                    });
                    da.Play();
                    stopBlinkAniList.Add(da);
                }
            }

        }
    }

    public virtual void PlaySmartSound(int reelIndex, ReelController controller)
    {
        Libs.AudioEntity.Instance.PlaySmartSoundEffect(reelIndex);
    }

    protected void StopSmartAnimation(int reelIndex, ReelController controller, int symbolIndex)
    {
        if (SmartPositions.ContainsKey(reelIndex))
        {
            foreach (int position in SmartPositions[reelIndex].Keys)
            {
                BaseElementPanel render = controller.GetSymbolRender(reelIndex, position);
                if (render != null && render.SymbolIndex == symbolIndex)
                    render.StopAnimation();
            }
        }

    }

    protected virtual void PreloadSmartWebm(List<ResultContent.ReelResult> reelsSpinData, ReelManager reelManager)
    {
        
    }

    protected List<Libs.DelayAction> stopBlinkAniList = new List<Libs.DelayAction>();

    public virtual void StopAllBlinkAnimation(ReelManager reelManager) {
        if (stopBlinkAniList.Count==0)
        {
            return;
        }
        for (int i = 0; i < stopBlinkAniList.Count; i++)
        {
            if (stopBlinkAniList[i] != null)
                stopBlinkAniList[i].Stop(true);
        }

    }

    public virtual void StopAllSmartAndAntiEffectAnimation(ReelManager reelManager){
		foreach (int key in SmartPositions.Keys) {
			foreach (int position in SmartPositions[key].Keys) {
				BaseElementPanel render = reelManager.GetSymbolRender (key, position);
				if (render != null) render.StopAnimation();
			}
		}
	}

	public bool IsContainBlinkSymbol(int reelIndex,int positionId)
	{
		return SmartPositions.ContainsKey (reelIndex) && SmartPositions [reelIndex].ContainsKey (positionId);
	}
    
    public List<int> GetReelSmartPositionId(int reelIndex)
	{
        List<int> symbolPositionId = new List<int>();
        if(SmartPositions.ContainsKey(reelIndex))
        {
            foreach (var positionId in SmartPositions[reelIndex].Keys)
            {
                if(SmartPositions[reelIndex][positionId]) symbolPositionId.Add(positionId);
            }
        }
        return symbolPositionId;
	}

    /*
	protected List<int> AntiReels {
		get;
		set;
	}
	public List<int> GenerateSpinEffectResult(List<List<int>> results,SymbolMap symbolMap){
		CheckSmartSoundSymbol (results,symbolMap);
		SetSmartSoundEffect ();
		SetAnticipationEffect ();

		return AntiReels;
	}

	protected virtual void SetSmartSoundEffect(){
		
	}

	protected virtual void SetAnticipationEffect(){
		
	}

	protected virtual void CheckSmartSoundSymbol(List<List<int>> results,SymbolMap symbolMap){
		if (results == null||symbolMap==null||SmartConfig==null||SmartConfig.Count==0||results.Count==0) return;

		InitSpinData (results);


	}

	protected void HandleContinuousSmartEffect(List<List<int>> results,SymbolMap symbolMap,int groudID){
		
	}

	protected void HandleScatterSmartEffect(List<List<int>> results,SymbolMap symbolMap){
		
	}

	protected void InitSpinData(List<List<int>> results){
		smartSoundMap.Clear ();
		for (int i = 0; i < results.Count; i++) {
			List<bool> smartSoundReel = new List<bool> ();
			for (int j = 0; j < results[i].Count; j++) {
				smartSoundReel.Add(false);
			}
			smartSoundMap.Add (smartSoundReel);
		}
	}

*/
}
