using System.Collections.Generic;

namespace Libs
{
    public class ConfigRuleAction:IRuleAction
    {
        public string Snippet;
        public bool CheckdResultSkip(SpinResultData data) //是否跳过这个结果
        {
            return false;
        }

        public RuleActionType GetRuleType()
        {
            return RuleActionType.REPACE_CONFIG;
        }

        public ConfigRuleAction(Dictionary<string, object> dic)
        {
            if (dic.ContainsKey("a8") && dic["a8"] != null)
            {
                Snippet = dic["a8"].ToString();
            }
        }
    }
}