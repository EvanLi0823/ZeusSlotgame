namespace Libs
{
    public class BaseAwardItem
    {
        public AwardType type;
        public int count;
    
        public virtual void AwardPrizes()
        {
            
        }

        public virtual string GetAwardCountDesc()
        {
            return count.ToString();
        }
    }
}