using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LuaFramework
{
    public interface IConditionContent 
    {
        bool CheckCondition();
        void SetExtraData(Dictionary<string,object> info);
    }
}