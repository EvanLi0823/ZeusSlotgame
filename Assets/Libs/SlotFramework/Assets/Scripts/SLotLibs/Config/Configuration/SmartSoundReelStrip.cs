using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Classic;
using Utils;
/// <summary>
/// Base smart sound reel strip.LQ_SMART_SOUND
/// </summary>
public class SmartSoundReelStrip {
    public List<SmartSoundReelStripElement> smartSoundReelStripElements{
        get;
        private set;
    }

    public SmartSoundReelStrip(SymbolMap symbolMap,List<object> smartSoundReelStripElementInfos){
        if (smartSoundReelStripElementInfos==null) {
            return;
        }
        smartSoundReelStripElements = new List<SmartSoundReelStripElement> ();
        for (int i = 0; i < smartSoundReelStripElementInfos.Count; i++) {
            SmartSoundReelStripElement baseSmartSoundReelStripElement = new SmartSoundReelStripElement (symbolMap,smartSoundReelStripElementInfos[i] as Dictionary<string,object>);
            this.smartSoundReelStripElements.Add (baseSmartSoundReelStripElement);
        }
       
    }
}