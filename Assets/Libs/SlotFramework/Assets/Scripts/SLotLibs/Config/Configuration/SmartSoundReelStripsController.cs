using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Utils;
using Core;
/// <summary>
/// Smart sound reel strips config. LQ_SMART_SOUND
/// </summary>
namespace Classic
{
    public class SmartSoundReelStripsController
    {
        public readonly static string SMART_SOUND_REEL_STRIPS_CONFIG = "SmartSounds";
        public readonly static string SMART_SOUND_SYMBOL_NAME = "SmartSoundSymbolName";                                 //LQ Array
        public readonly static string SMART_SOUND_SYMBOL_GROUP_INDEX = "SmartSoundSymbolGroupIndex"; 
        public readonly static string REQUIRE_APPEARED_MIN_COUNT = "RequireAppearedMinCount";                           //LQ Number
        public readonly static string NEED_TO_SATISFY_PAYLINE = "NeedToSatisfyPayLine";                                 //LQ Boolean
        public readonly static string PLAY_SMART_SOUND_INEX = "PlaySmartSoundIndex"; 
		public readonly static string ENABLE_SMART_SOUND_ANTICIPATION_ANIMATION = "EnableSmartSoundAnticipationAnimation";
		public readonly static string REQUIRE_APPEARED_MIN_COUNT_FOR_ANTICIPATION_ANIMATION = "RequireAppearedMinCountForAnticipationAnimation";
		public readonly static string ANTICIPATION_STRIPS_EXIST_FLAG ="AnticipationStripsExistFlag";
        public List<SmartSoundReelStrip> smartSoundReelStrips;
        public Dictionary<int,Dictionary<int,int>> smartSoundGroupSymbolAppearedCount =null;//LQ 线上与非线上都用此结构进行处理 依次分别为组号、所在中奖线号、线上出现次数
        public SmartSoundReelStripsController (SymbolMap symbolMap,List<object> infos = null)
        {
           
			if (infos != null) {
                smartSoundReelStrips = new List<SmartSoundReelStrip> ();

                for (int i = 0; i < infos.Count; i++) {
                    SmartSoundReelStrip baseSmartSoundReelStrip = new SmartSoundReelStrip (symbolMap,infos[i] as List<object> );
					smartSoundReelStrips.Add (baseSmartSoundReelStrip);
                }

                //LQ 解析完后，对其SmartSoundSymbolGroup进行统计，来为后续使用计数做准备
                RecordSmartSoundSymbolInfos ();
            }
        }

        public void ClearSmartSoundGroupSymbolAppearedCount(){
            if (smartSoundGroupSymbolAppearedCount!=null) {
                foreach (int key in smartSoundGroupSymbolAppearedCount.Keys) {
                    smartSoundGroupSymbolAppearedCount[key].Clear();
                }
            }
        }
        public void RecordSmartSoundSymbolInfos(){
			
            if (smartSoundReelStrips==null) {
                return;
            }
            smartSoundGroupSymbolAppearedCount = new Dictionary<int, Dictionary<int, int>> ();
            for (int i = 0; i < smartSoundReelStrips.Count; i++) {
                List<SmartSoundReelStripElement> baseSmartSoundReelStripElements = smartSoundReelStrips [i].smartSoundReelStripElements;
                if (baseSmartSoundReelStripElements!=null) {
                    for (int j = 0; j < baseSmartSoundReelStripElements.Count; j++) {
                        {
                            int symbolGroupIndex = baseSmartSoundReelStripElements [j].smartSoundSymbolGroupIndex;
                            if (!smartSoundGroupSymbolAppearedCount.ContainsKey(symbolGroupIndex)) {
                                smartSoundGroupSymbolAppearedCount.Add (symbolGroupIndex,new Dictionary<int, int>());//LQ 存储指定SmartSoundSymbolGroup的索引信息，方便ReelManager对指定Symbol所在组，所在线进行计数
                            }
                        }
                    }
                }
            }
        }

        public int CheckPlaySmartSoundCondition(ReelManager reelManager,SymbolMap.SymbolElementInfo info,int elementColumnIndex,int elementRowIndex)
        {
            //LQ 此方法，将不再划分多线还是单线（都是通过遍历是否在线上来实现），是baseGame还是FreeSpin（切换时，会保证数据使用各自数据），将会进行统一方式处理
            int playSmartSoundIndex = -1;
            bool enablePlaySmartSound = false;
            List<SmartSoundReelStripElement> baseSmartSoundReelStripElements =null;
            //20190416
            //if (reelManager.smartSoundReelStripsController.smartSoundGroupSymbolAppearedCount==null) {
            //    return -1;// 说明节点不存在 不播放声音 
            //}
            if (smartSoundReelStrips.Count<=elementColumnIndex) {
                return -1;// 说明节点不存在 不播放声音 
            }
            baseSmartSoundReelStripElements = smartSoundReelStrips[elementColumnIndex].smartSoundReelStripElements;
            if (baseSmartSoundReelStripElements==null) {
                return -1;
            }
            for (int i = 0; i < baseSmartSoundReelStripElements.Count; i++) {
                if (baseSmartSoundReelStripElements[i].smartSoundSymbolNameIndexs.Contains(info.Index)) {
                    if (baseSmartSoundReelStripElements[i].needToSatisfyPayline) {

                        for (int k = 0; k < reelManager.lineTable.TotalPayLineCount(); k++) {
                            PayLine payLine = reelManager.lineTable.GetPayLineAtIndex (k);
                            if (payLine.RowNumberAt (elementColumnIndex) ==elementRowIndex) {
                               
                                Dictionary<int,int> payLineSmartSoundSymbolCount = smartSoundGroupSymbolAppearedCount[baseSmartSoundReelStripElements[i].smartSoundSymbolGroupIndex];
                                if (!payLineSmartSoundSymbolCount.ContainsKey(k)) {
                                    payLineSmartSoundSymbolCount.Add (k, 0);
                                }
                                if (!enablePlaySmartSound){
                                    if (payLineSmartSoundSymbolCount.ContainsKey(k)&&payLineSmartSoundSymbolCount[k]>=baseSmartSoundReelStripElements[i].requireAppearedSmartSymbolMinCount) {
                                        enablePlaySmartSound = true;//LQ 只要满足条件，即只执行一次
                                        playSmartSoundIndex = payLineSmartSoundSymbolCount[k]+1;
                                        if (baseSmartSoundReelStripElements[i].playSmartSoundIndex !=-1) {
                                            playSmartSoundIndex = baseSmartSoundReelStripElements [i].playSmartSoundIndex;
                                        }
                                    }
                                }

                                payLineSmartSoundSymbolCount[k]++;
								#region AnticipationAnimationCheck
								if (baseSmartSoundReelStripElements[i].EnableSmartSoundAnticipationAnimation
									&& payLineSmartSoundSymbolCount[k] >= baseSmartSoundReelStripElements[i].RequireAppearedMinCountForAnticipationAnimation) {
									SetSmartSoundAnticipationAnimation (reelManager,elementColumnIndex,baseSmartSoundReelStripElements[i].AnticipationStripsExistFlag);
								}
								#endregion
                            }
                        }
                    }
                    else {
                        
                        Dictionary<int,int> smartSoundGroupIndexCount = smartSoundGroupSymbolAppearedCount [baseSmartSoundReelStripElements [i].smartSoundSymbolGroupIndex];
                        int totalCount = 0;
                        //非Payline判定，我们默认在键为0的对应计数器内累加
                        if (!smartSoundGroupIndexCount.ContainsKey(0)) {
                            smartSoundGroupIndexCount.Add (0,0);
                        }
                        foreach (int key in smartSoundGroupIndexCount.Keys) {
                            totalCount += smartSoundGroupIndexCount[key];
                        }

                        if (!enablePlaySmartSound && totalCount >= baseSmartSoundReelStripElements[i].requireAppearedSmartSymbolMinCount) {
                            enablePlaySmartSound = true;
							playSmartSoundIndex = smartSoundGroupIndexCount[0]+1;
                            if (baseSmartSoundReelStripElements[i].playSmartSoundIndex !=-1) {
                                playSmartSoundIndex = baseSmartSoundReelStripElements [i].playSmartSoundIndex;
                            }
                        }

                        smartSoundGroupIndexCount[0]++;
						#region AnticipationAnimationCheck
						if (baseSmartSoundReelStripElements[i].EnableSmartSoundAnticipationAnimation
							&& smartSoundGroupIndexCount[0] >= baseSmartSoundReelStripElements[i].RequireAppearedMinCountForAnticipationAnimation) {
							SetSmartSoundAnticipationAnimation (reelManager,elementColumnIndex,baseSmartSoundReelStripElements[i].AnticipationStripsExistFlag);
						}
						#endregion
                    }
                }
            }        

            return playSmartSoundIndex;
        }
    
        //此方法不会考虑中线一说  -特殊主题专用
        public void CheckIndependentSmartSoundElement(ReelManager reelManager,int ReelIndex,int SymbolIndex,int IndexOfDisplay){
            int mustShowMinCount = 0;
            int onlyPlayOneSmartSound = -1;
            int symbolGroupIndex = 0;
            int totalCount = 0;
			int currentIndex = 0;
            //20190416
            //if (reelManager.smartSoundReelStripsController.smartSoundGroupSymbolAppearedCount==null) {
            //    return;// 说明节点不存在 不播放声音 
            //}
            List<SmartSoundReelStripElement> baseSmartSoundReelStripElements = smartSoundReelStrips[ReelIndex].smartSoundReelStripElements;
            for (int i = 0; i < baseSmartSoundReelStripElements.Count; i++) {
                if (baseSmartSoundReelStripElements[i].smartSoundSymbolNameIndexs.Contains(SymbolIndex)) {
                    mustShowMinCount = baseSmartSoundReelStripElements [i].requireAppearedSmartSymbolMinCount;
                    onlyPlayOneSmartSound = baseSmartSoundReelStripElements [i].playSmartSoundIndex;
                    symbolGroupIndex = baseSmartSoundReelStripElements [i].smartSoundSymbolGroupIndex;
					currentIndex = i;
                    break;
                }
            }
            foreach (int key in smartSoundGroupSymbolAppearedCount[symbolGroupIndex].Keys) {
                totalCount += smartSoundGroupSymbolAppearedCount [symbolGroupIndex][key];
            }
            if ((IndexOfDisplay+totalCount)>= mustShowMinCount) {
                if (onlyPlayOneSmartSound!=-1) {
                    //Libs.SoundEntity.Instance.SmartSound (onlyPlayOneSmartSound);
                    Libs.AudioEntity.Instance.PlaySmartSoundEffect(onlyPlayOneSmartSound);
                } else {
                    //Libs.SoundEntity.Instance.SmartSound (totalCount);
                    Libs.AudioEntity.Instance.PlaySmartSoundEffect(totalCount);
                }

                //LQ 默认累加到0线上
                if (!smartSoundGroupSymbolAppearedCount[symbolGroupIndex].ContainsKey(0)) {
                    smartSoundGroupSymbolAppearedCount [symbolGroupIndex].Add (0,1);
                } else {
                    smartSoundGroupSymbolAppearedCount [symbolGroupIndex][0]++;
                }
                #region AnticipationAnimationCheck
				if (baseSmartSoundReelStripElements [currentIndex].EnableSmartSoundAnticipationAnimation && 
					smartSoundGroupSymbolAppearedCount [symbolGroupIndex][0]>=baseSmartSoundReelStripElements [currentIndex].RequireAppearedMinCountForAnticipationAnimation) {
					SetSmartSoundAnticipationAnimation (reelManager,ReelIndex,baseSmartSoundReelStripElements [currentIndex].AnticipationStripsExistFlag);
				}
				#endregion
            }  
        }

		#region AnticipationAnimationCheck
		private void SetSmartSoundAnticipationAnimation(ReelManager reelManager,int reelIndex,string anticipationFlag){
			if (reelManager==null) {
				return ;
			}
			char[] antiFlagArray = anticipationFlag.ToCharArray ();//默认最大支持10个带子
			int nextReelIdx = reelIndex+1;
			for (int i = nextReelIdx; i < reelManager.Reels.Count; i++)
			{
				if (antiFlagArray[i]=='1')
				{
					reelManager.Reels[i].needToPlayAnticipationAnimation = true;
				}
			}
		}
		#endregion
	}
}