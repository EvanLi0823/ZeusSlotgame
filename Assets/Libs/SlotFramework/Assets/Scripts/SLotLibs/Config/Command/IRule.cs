using System.Collections.Generic;

namespace LuaFramework
{
    /// <summary>
    /// 桥接 C# BaseCommandItem 与 Lua Rule 的接口
    /// </summary>
    public interface IRule
    {
        /// <summary>
        /// 执行动作
        /// </summary>
        /// <param name="contextInfo">
        ///    执行时的上下文信息，一般用于上报ES
        /// </param>
        void DoCommandOperation(Dictionary<string, object> contextInfo = null);

        int Priority();
        string Token();//Lua层的instance_id
    }
}