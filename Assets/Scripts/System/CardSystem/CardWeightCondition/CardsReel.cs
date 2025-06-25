using System.Collections.Generic;
using UnityEngine;

namespace CardSystem
{
    
    
    public class CardsReel
    {
        public string Name { get; private set; }
        private List<BaseCardReelItem> cardReel;
        public int TotalWeight{ get; private set; }
        
        public CardsReel(string name, List<object> data)
        {
            this.Name = name;
            ParseCardsData(data);
        }
        
        void ParseCardsData(List<object> data)
        {
            cardReel = new List<BaseCardReelItem>();
            TotalWeight = 0;
            if (data == null || data.Count <= 0)
            {
                return;
            }
            for (int i = 0; i < data.Count; i++)
            {
                Dictionary<string, object> info = data[i] as Dictionary<string, object>;
                if (info == null)
                {
                    continue;
                }
                BaseCardReelItem item = new BaseCardReelItem(info);
                if (item.Index >= 0)
                {
                    cardReel.Add(item);
                    TotalWeight += item.Weight;
                }
            }
        }
        
        public List<BaseCardReelItem> GetCards()
        {
            return cardReel;
        }
        
        public int GetResultRandomByWeight()
        {
            if (TotalWeight <= 0 || cardReel == null || cardReel.Count <= 0)
            {
                return -1;
            }
            int randomWeight = Random.Range(0, TotalWeight);
            int weightAccumulated = 0;
            for (int i = 0; i < cardReel.Count; i++)
            {
                BaseCardReelItem item = cardReel[i];
                int lastWeightAccumulated = weightAccumulated;
                weightAccumulated += item.Weight;
                if (weightAccumulated > randomWeight && lastWeightAccumulated <= randomWeight)
                {
                    return item.Index;
                }
            }
            return -1;
        }
    }
}