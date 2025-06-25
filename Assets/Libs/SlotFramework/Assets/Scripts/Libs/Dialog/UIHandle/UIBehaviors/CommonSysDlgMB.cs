using RealYou.Core.UI;
using UniRx.Async;
using System;

namespace Libs
{
    public class CommonSysDlgMB : IMachineBehaviour
    {
        protected Action<Action<bool>> CallBack { get; set; }
        protected bool finished = false;
        protected bool unregister = false;

        public CommonSysDlgMB(Action<Action<bool>> callback)
        {
            CallBack = callback;
        }

        public virtual int Priority
        {
            get { return (int)UIMBPriority.CommonSysDlg; }
        }

        public virtual async UniTask OnPreEnterMachine(bool isRestore)
        {
            PopUpUILogES.SendLog2ES($"CommonSysDlgMB:OnPreEnterMachine 1 Priority:{Priority} finished:{finished}");
            if (isRestore) return;
            PopUpUILogES.SendLog2ES($"CommonSysDlgMB:OnPreEnterMachine 2 Priority:{Priority} finished:{finished}");

            if (CallBack != null)
            {
                PopUpUILogES.SendLog2ES($"CommonSysDlgMB:OnPreEnterMachine 3 Priority:{Priority} finished:{finished}");
                CallBack((status) => { finished = true; });
                await UniTask.WaitUntil(() => finished);
                PopUpUILogES.SendLog2ES($"CommonSysDlgMB:OnPreEnterMachine 4 Priority:{Priority} finished:{finished}");
                MachineUtility.Instance.UnRegisterMachineEvent(this);
                unregister = true;
            }
        }

        public virtual async UniTask OnLateEnterMachine(bool isRestore)
        {
            PopUpUILogES.SendLog2ES($"CommonSysDlgMB:OnLateEnterMachine 1 Priority:{Priority} finished:{finished}");
            if (isRestore) return;
            PopUpUILogES.SendLog2ES($"CommonSysDlgMB:OnLateEnterMachine 2 Priority:{Priority} finished:{finished}");

            if (CallBack != null)
            {
                PopUpUILogES.SendLog2ES($"CommonSysDlgMB:OnLateEnterMachine 3 Priority:{Priority} finished:{finished}");
                CallBack((status) => { finished = true; });
                await UniTask.WaitUntil(() => finished);
                PopUpUILogES.SendLog2ES($"CommonSysDlgMB:OnLateEnterMachine 4 Priority:{Priority} finished:{finished}");
                MachineUtility.Instance.UnRegisterMachineEvent(this);
                unregister = true;
            }
        }

        public virtual async UniTask OnSpinEnd()
        {
            //像respin link freespin都要返回不执行
            PopUpUILogES.SendLog2ES($"CommonSysDlgMB:OnSpinEnd 1 Priority:{Priority} finished:{finished}");

            if (MachineUtility.Instance.IsFreespinBonus()) return;
            PopUpUILogES.SendLog2ES($"CommonSysDlgMB:OnSpinEnd 2 Priority:{Priority} finished:{finished}");
            if (CallBack != null)
            {
                PopUpUILogES.SendLog2ES($"CommonSysDlgMB:OnSpinEnd 3 Priority:{Priority} finished:{finished}");
                CallBack((status) => { finished = true; });
                await UniTask.WaitUntil(() => finished);
                PopUpUILogES.SendLog2ES($"CommonSysDlgMB:OnSpinEnd 4 Priority:{Priority} finished:{finished}");
                MachineUtility.Instance.UnRegisterMachineEvent(this);
                unregister = true;
            }
        }

        public virtual async UniTask OnExitMachine()
        {
            finished = true;
            PopUpUILogES.SendLog2ES($"CommonSysDlgMB:OnExitMachine 3 Priority:{Priority} finished:{finished}");

            if (!unregister) MachineUtility.Instance.UnRegisterMachineEvent(this);
        }
    }
}