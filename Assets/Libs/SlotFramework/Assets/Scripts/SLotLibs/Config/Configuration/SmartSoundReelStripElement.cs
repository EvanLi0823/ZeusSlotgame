using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Classic;
using Utils;
using System;
/// <summary>
/// Base smart sound reel strip element.LQ_SMART_SOUND
/// </summary>
public class SmartSoundReelStripElement {
   
    public List<int> smartSoundSymbolNameIndexs = null;
    public int requireAppearedSmartSymbolMinCount = 0;
    public bool needToSatisfyPayline= false;
    public int playSmartSoundIndex = -1;
    public int smartSoundSymbolGroupIndex = 0;
	public bool EnableSmartSoundAnticipationAnimation = false;
	public int RequireAppearedMinCountForAnticipationAnimation = 0;
	public string AnticipationStripsExistFlag = "1111111111";
    public SmartSoundReelStripElement(SymbolMap symbolMap,Dictionary<string,object> elementMembersInfos){
       
        if (elementMembersInfos==null) {
            return;
        }
        smartSoundSymbolNameIndexs = new List<int> ();
        if (elementMembersInfos.ContainsKey(SmartSoundReelStripsController.SMART_SOUND_SYMBOL_NAME)) {
            IList smartSoundsymbolNames = (elementMembersInfos [SmartSoundReelStripsController.SMART_SOUND_SYMBOL_NAME]) as IList;
            for (int i = 0; i < smartSoundsymbolNames.Count; i++) {
                string SymbolName = smartSoundsymbolNames [i].ToString();
                int symbolIndex = symbolMap.getSymbolIndex (SymbolName);
                if (!smartSoundSymbolNameIndexs.Contains(symbolIndex)) {
                    smartSoundSymbolNameIndexs.Add (symbolIndex);
                }

            }
        }
        if (elementMembersInfos.ContainsKey(SmartSoundReelStripsController.SMART_SOUND_SYMBOL_GROUP_INDEX)) {
            smartSoundSymbolGroupIndex = Convert.ToInt32(elementMembersInfos[SmartSoundReelStripsController.SMART_SOUND_SYMBOL_GROUP_INDEX]);
        }
        if (elementMembersInfos.ContainsKey(SmartSoundReelStripsController.REQUIRE_APPEARED_MIN_COUNT)) {
            requireAppearedSmartSymbolMinCount = Convert.ToInt32(elementMembersInfos [SmartSoundReelStripsController.REQUIRE_APPEARED_MIN_COUNT]);
        }
        if (elementMembersInfos.ContainsKey(SmartSoundReelStripsController.NEED_TO_SATISFY_PAYLINE)) {
            needToSatisfyPayline = Convert.ToBoolean (elementMembersInfos[SmartSoundReelStripsController.NEED_TO_SATISFY_PAYLINE]);
        }
        if (elementMembersInfos.ContainsKey(SmartSoundReelStripsController.PLAY_SMART_SOUND_INEX)) {
            playSmartSoundIndex = Convert.ToInt32 (elementMembersInfos[SmartSoundReelStripsController.PLAY_SMART_SOUND_INEX]);
        }
		if (elementMembersInfos.ContainsKey(SmartSoundReelStripsController.ENABLE_SMART_SOUND_ANTICIPATION_ANIMATION)) {
			EnableSmartSoundAnticipationAnimation = Convert.ToBoolean (elementMembersInfos [SmartSoundReelStripsController.ENABLE_SMART_SOUND_ANTICIPATION_ANIMATION]);
		}
		if (elementMembersInfos.ContainsKey(SmartSoundReelStripsController.REQUIRE_APPEARED_MIN_COUNT_FOR_ANTICIPATION_ANIMATION)) {
			RequireAppearedMinCountForAnticipationAnimation = Convert.ToInt32 (elementMembersInfos [SmartSoundReelStripsController.REQUIRE_APPEARED_MIN_COUNT_FOR_ANTICIPATION_ANIMATION]);
		}
		if (elementMembersInfos.ContainsKey(SmartSoundReelStripsController.ANTICIPATION_STRIPS_EXIST_FLAG)) {
			AnticipationStripsExistFlag = elementMembersInfos [SmartSoundReelStripsController.ANTICIPATION_STRIPS_EXIST_FLAG].ToString ();
		}
    }
}
