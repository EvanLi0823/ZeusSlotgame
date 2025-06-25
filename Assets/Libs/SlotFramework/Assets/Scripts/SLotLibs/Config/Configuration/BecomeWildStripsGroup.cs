using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
namespace Classic{
    
    public class BecomeWildStripsGroup {
        
        public readonly static string BLACK_OUT_PAYTABLE = "StripsIndex";
        public readonly static string SYMBOL_LIST = "Weight";

        public List<int> stripsIndex;
        public int weight=0;
        public BecomeWildStripsGroup(object wildGroup){

            if (wildGroup==null) {
                return;
            }
            stripsIndex = new List<int> ();
           
            string[] indexs = (wildGroup as Dictionary<string,object>) [BLACK_OUT_PAYTABLE].ToString ().Split (',');
            for (int i = 0; i < indexs.Length; i++) {
                stripsIndex.Add(Convert.ToInt32(indexs [i]));
            }
            weight = Convert.ToInt32 ((wildGroup as Dictionary<string,object>) [SYMBOL_LIST]);
        }
    }
}
