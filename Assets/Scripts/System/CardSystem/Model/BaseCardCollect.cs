using System.Collections.Generic;

namespace CardSystem
{
    public class BaseCardCollect
    {
        private string name;
        private List<BaseCard> cards;
        private long coins;
        private int cash;

        public string Name
        {
            get { return name; }
            private set { name = value; }
        }
        public List<BaseCard> Cards
        {
            get { return cards; }
            private set { cards = value; }
        }
        public long Coins
        {
            get { return coins; }
            private set { coins = value; }
        }
        public int Cash
        {
            get { return cash; }
            private set { cash = value; }
        }
        
        public BaseCardCollect(string name,int coins, Dictionary<string,object> cards)
        {
            this.name = name;
            this.Cash = coins;
            InitCards(cards);
        }

        public void InitCards(Dictionary<string,object> data)
        {
            cards = new List<BaseCard>();
            if (data == null || data.Count <= 0)
            {
                return;
            }
            foreach (var item in data)
            {
                if (item.Value is Dictionary<string, object> cardData)
                {
                    BaseCard card = new BaseCard(item.Key, cardData);
                    cards.Add(card);
                }
            }
        }
    }
}