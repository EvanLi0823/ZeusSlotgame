using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Libs;
namespace Classic
{
    public class ResultInState : ResultState
    {
        public  void Run2()
        {
            base.Run();
            End();
        }

        public override void End()
        {
            base.End();
        }

        public override void Init()
        {
            action = new BaseActionNormal();
            action.AutoPlayNextAction = false;
            action.PlayCallBack.AddStartMethod(Run2);
        }
    }
}
