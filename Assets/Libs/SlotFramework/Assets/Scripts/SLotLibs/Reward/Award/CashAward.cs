namespace Libs
{
    public class CashAwardItem:BaseAwardItem
    {
        public override string GetAwardCountDesc()
        {
            return OnLineEarningMgr.Instance.GetMoneyStr(count, 0, false, true);
        }
    }
}