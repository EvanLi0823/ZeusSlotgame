using System;
using System.Collections.Generic;
using UniRx.Async;

namespace RealYou.Core.UI
{
    public partial class MachineUtility
    {

        private List<IMachineBehaviour> _machineBehaviours = new List<IMachineBehaviour>();
        
        private readonly List<IMachineBehaviour> _machineBehavioursRemoveList = new List<IMachineBehaviour>();
        
        public void RegisterMachineEvent(IMachineBehaviour behaviour)
        {
            if(behaviour == null)
                return;
            _machineBehaviours.Add(behaviour);
            _machineBehaviours.Sort(MachineBehavioursCompare);
        }
        
        public void UnRegisterMachineEvent(IMachineBehaviour behaviour)
        {
            if(behaviour == null)
                return;

            var index = _machineBehaviours.IndexOf(behaviour);
            if (index != -1)
            {
                _machineBehavioursRemoveList.Add(behaviour);
            }
        }

        internal async UniTask InvokePreEnterMachineEvent(bool isRestore)
        {
            RemoveMachineUnusedEvent();
            
            IMachineBehaviour behaviour = null;
            for (int i = 0; i < _machineBehaviours.Count; i++)
            {
                try
                {
                    behaviour = _machineBehaviours[i];
                    if(behaviour == null || MachineEventIsRemoved(behaviour))
                        continue;
                    
                    await behaviour.OnPreEnterMachine(isRestore);

                }
                catch (Exception e)
                {
                    BaseGameConsole.singletonInstance.LogError(e.ToString(), "EnterMachineEvent");
                }
            }
            
            RemoveMachineUnusedEvent();
        }
        
        internal async UniTask InvokeLateEnterMachineEvent(bool isRestore)
        {
            RemoveMachineUnusedEvent();
            
            IMachineBehaviour behaviour = null;
            for (int i = 0; i < _machineBehaviours.Count; i++)
            {
                try
                {
                    behaviour = _machineBehaviours[i];
                    if(behaviour == null || MachineEventIsRemoved(behaviour))
                        continue;
                    
                    await behaviour.OnLateEnterMachine(isRestore);

                }
                catch (Exception e)
                {
                    BaseGameConsole.singletonInstance.LogError(e.ToString(), "EnterMachineEvent");
                }
            }
            
            RemoveMachineUnusedEvent();
        }
        
        internal async UniTask InvokeSpinEndEvent()
        {
            RemoveMachineUnusedEvent();

            IMachineBehaviour behaviour = null;
            for (int i = 0; i < _machineBehaviours.Count; i++)
            {
                try
                {
                    behaviour = _machineBehaviours[i];
                    if(behaviour == null || MachineEventIsRemoved(behaviour))
                        continue;

                    await behaviour.OnSpinEnd();
                }
                catch (Exception e)
                {
                    BaseGameConsole.singletonInstance.LogError(e.ToString(), "SpinEndEvent");
                }
            }

            RemoveMachineUnusedEvent();
        }
        
        internal async UniTask InvokeExitMachineEvent()
        {
            RemoveMachineUnusedEvent();

            IMachineBehaviour behaviour = null;
            for (int i = 0; i < _machineBehaviours.Count; i++)
            {
                try
                {
                    behaviour = _machineBehaviours[i];
                    if(behaviour == null || MachineEventIsRemoved(behaviour))
                        continue;

                    await behaviour.OnExitMachine();
                }
                catch (Exception e)
                {
                    BaseGameConsole.singletonInstance.LogError(e.ToString(), "ExitMachineEvent");
                }
            }

            RemoveMachineUnusedEvent();
        }

        private bool MachineEventIsRemoved(IMachineBehaviour machineBehaviour)
        {
            if (machineBehaviour == null)
                return true;

            return _machineBehavioursRemoveList.Contains(machineBehaviour);
        }

        private void RemoveMachineUnusedEvent()
        {
            for (int i = 0; i < _machineBehavioursRemoveList.Count; i++)
            {
                if(_machineBehavioursRemoveList[i] == null)
                    continue;
                
                _machineBehaviours.Remove(_machineBehavioursRemoveList[i]);
            }
            
            _machineBehavioursRemoveList.Clear();
        }

        private static int MachineBehavioursCompare(IMachineBehaviour left, IMachineBehaviour right)
        {
            if (left == null || right == null)
                return 0;
            
            return right.Priority.CompareTo(left.Priority);
        }
        
    }

    public interface IMachineBehaviour
    {
        /// <summary>
        /// 数字越大优先级越高.
        /// </summary>
        int Priority { get; }
        
        /// <summary>
        /// 进入机器.
        /// </summary>
        /// <param name="isRestore">true:表示是由上次保存恢复进入的.</param>
        /// <returns></returns>
         UniTask OnPreEnterMachine(bool isRestore);
        
         UniTask OnLateEnterMachine(bool isRestore);

        /// <summary>
        /// spin结束后.
        /// </summary>
        /// <returns></returns> 
        UniTask OnSpinEnd();
        /// <summary>
        /// 退出机器前，可以将不需要的IMachineBehaviour对象移除
        /// </summary>
        /// <returns></returns>
        UniTask OnExitMachine();
    }
}