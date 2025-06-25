
using System.Collections.Generic;

public class ADTimeCondition:ADCondition
{
    private int TargetTime;
    private int HasCollectTime;
    public ADTimeCondition(Dictionary<string, object> data) : base(data)
    {
        TargetTime = Utils.Utilities.GetInt(data, "Time", 0);
    }

    public override void ResetCondition()
    {
        HasCollectTime = 0;
    }
}
