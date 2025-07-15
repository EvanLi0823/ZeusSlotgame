using System.Collections.Generic;

namespace CardSystem
{
    public class BaseCard
    {
        public string Name{ get; private set;}
        public int Index { get; private set; }
        public int Level{ get; private set; }

        public int Count{ get; private set; }
        public BaseCard(string name,Dictionary<string,object> data)
        {
            Name = name;
            Index = Utils.Utilities.GetInt(data, CardSystemConstants.index, -1);
            Level = Utils.Utilities.GetInt(data, CardSystemConstants.level, 1);
            Count = CardSystemManager.Instance.GetCardCount(Index);
        }
        
        public void SetCount(int count)
        {
            Count = count;
        }
    }
}