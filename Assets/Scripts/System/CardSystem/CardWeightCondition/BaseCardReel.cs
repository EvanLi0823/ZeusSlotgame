using System.Collections.Generic;
using Utils;

namespace CardSystem
{
    public class BaseCardReelItem
    {
        public string Name{ get; private set;}
        public int Weight{ get; private set; }
        public int Index { get; private set; }
        public int Level{ get; private set; }

        public BaseCardReelItem(Dictionary<string, object> info)
        {
            if (info.ContainsKey(CardSystemConstants.Card))
            {
                Name = (string)info[CardSystemConstants.Card];
                Index = CardSystemManager.Instance.GetCardIndex(Name);
            }
            else
            {
                Index = -1;
            }

            if (info.ContainsKey(CardSystemConstants.Weights))
            {
                Weight = Utilities.CastValueInt(info[CardSystemConstants.Weights]);
            }
            else
            {
                Weight = 0;
            }

            Level = CardSystemManager.Instance.GetCardLevel(Index);
        }
    }
}