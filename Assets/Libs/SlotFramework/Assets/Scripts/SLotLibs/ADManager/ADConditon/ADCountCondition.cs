
using System.Collections.Generic;

namespace Ads
{
    public class ADCountCondition:ADCondition
    {
        private int TargetTime = 0;
        private int HasCollectTime = 0;
        public ADCountCondition(Dictionary<string, object> data) : base(data)
        {
            TargetTime = Utils.Utilities.GetInt(data, "target", 0);
        }
    
        public override void ResetCondition()
        {
            HasCollectTime = 0;
        }
    
        public override void UpdateCondition()
        {
            HasCollectTime++;
        }
    
        public override bool isMeetCondition()
        {
            return HasCollectTime >= TargetTime;
        }

        public override void Clone(ADCondition adCondition)
        {
            base.Clone(adCondition);
            if (adCondition is ADCountCondition adCountCondition)
            {
                HasCollectTime = adCountCondition.HasCollectTime;
            }
        }
    }
}

