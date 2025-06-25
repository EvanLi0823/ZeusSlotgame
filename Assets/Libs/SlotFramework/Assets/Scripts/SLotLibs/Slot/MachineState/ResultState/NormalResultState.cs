using UnityEngine;
using System.Collections;
using Libs;

namespace Classic
{
	public class NormalResultState : ResultState
	{
        public override void Init()
        {
            action = new BaseActionNormal();
            action.AutoPlayNextAction = false;
            action.PlayCallBack.AddStartMethod(Run2);
        }
        public void Run2()
		{
			base.Run ();
			if (!ResultStateManager.Instante.slotController.reelManager.IsNewProcess)
			{
				ResultStateManager.Instante.slotController.CheckResult();
			}
            ResultStateManager.Instante.slotController.PlayAnimation();
		}
	
		public override void End ()
		{
			base.End ();
		}
	}
}
