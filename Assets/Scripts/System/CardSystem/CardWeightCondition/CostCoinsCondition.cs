using System.Collections.Generic;
using Classic;

namespace CardSystem
{
    public class CostCoinsCondition:BaseWeightCondition
    {
        public int Limit{ get; private set;}
        public int Cost{ get; private set;}

        public override void ParseConditionConfig(Dictionary<string, object> data)
        {
            base.ParseConditionConfig(data);
            Limit = Utils.Utilities.GetInt(data, CardSystemConstants.limit, 0);
            Cost = Utils.Utilities.GetInt(data, CardSystemConstants.cost, 0);
        }

        public override bool CheckCondition()
        {
            //用户
            return UserManager.GetInstance().UserProfile().Balance() >= Limit;
        }

        public override void Execute()
        { 
            UserManager.GetInstance().IncreaseBalance(-Cost);
            Messenger.Broadcast(SlotControllerConstants.OnBlanceChangeForDisPlay);
        }
    }
}