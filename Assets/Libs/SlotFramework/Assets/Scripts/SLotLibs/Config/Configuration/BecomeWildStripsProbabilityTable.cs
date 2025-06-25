using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Classic{
    
    public class BecomeWildStripsProbabilityTable {
        public readonly static string BECOME_WILD_STRIPS_PROBABILITY_TABLE = "GroupRandomBecomeWildStripsProbability";
      
        public int totalWeights = 0;
        public List<BecomeWildStripsGroup> becomeWildStripsGroupList = new List<BecomeWildStripsGroup> ();
        public BecomeWildStripsProbabilityTable( List<object> infos){

            if (infos == null)
                return;
            totalWeights = 0;
            foreach (object item in infos) {
                BecomeWildStripsGroup becomeWildStripsGroup = new BecomeWildStripsGroup (item);
                totalWeights += becomeWildStripsGroup.weight;
                becomeWildStripsGroupList.Add (becomeWildStripsGroup);
            }
        }
       
        public List<int> GetCalculatedRandomWildStripsIndex(){
            
            if (totalWeights==0||becomeWildStripsGroupList.Count==0) {
                return null;
            }
            int currentWeight = 0;
            int bothCondition = 0;
            int randomResult = UnityEngine.Random.Range (0, totalWeights);
            for (int i = 0; i < becomeWildStripsGroupList.Count; i++) {
                if (randomResult >= currentWeight) {
                    bothCondition++;
                }
                currentWeight += becomeWildStripsGroupList [i].weight;
                if (randomResult < currentWeight) {
                    bothCondition++;
                }
                if (bothCondition==2) {
                    
                    return becomeWildStripsGroupList [i].stripsIndex;
                }
                bothCondition = 0;
            }

            return null;
        }
    }
}

