/********************************************************************************************
    RealYou Framework
    Module: Core/Binding
    Author: HU QIWEI
********************************************************************************************/
using System;

namespace Binding
{
    public class BindableAction : Bindable<bool>
    {
        Action _action;
        public BindableAction(Action action)
        {
            _action = action;
            _bindFlag = Bindable<bool>.BindFlags.Set;
        }

        internal override void Set<TValue>(TValue val)
        {
            _action.Invoke();
        }
    }


    public class BindableAction<T> : Bindable<T>
    {
        Action<T> _action;
        T _param;
        public BindableAction(Action<T> action)
        {
            _action = action;
            _param = default(T);
            _bindFlag = Bindable<T>.BindFlags.Set;
        }

        public BindableAction(Action<T> action, T val)
        {
            _action = action;
            _param = val;
            _bindFlag = Bindable<T>.BindFlags.Set;
        }

        internal override void Set<TValue>(TValue val)
        {
            if (typeof(TValue) == typeof(object) && (object)val == null)
                _action.Invoke(_param);
            else
                _action.Invoke((T)Convert.ChangeType(val, typeof(T)));
        }
    }

    public partial class Bindable
    {
        /// <summary>
        /// Wrap an action as Bindable object
        /// </summary>
        /// <param name="action">the action to wrap up</param>
        /// <returns>BindableAction</returns>
        public static BindableAction Wrap(Action action)
        {
            return new BindableAction(action);
        }

        /// <summary>
        /// Wrap an action with parameter type T as Bindable object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action">the action to wrap up</param>
        /// <returns>BindableAction</returns>
        public static BindableAction<T> Wrap<T>(Action<T> action)
        {
            return new BindableAction<T>(action, default(T));
        }

        /// <summary>
        /// Wrap an action with parameter type T and its default value as Bindable object
        /// </summary>
        /// <typeparam name="T">action's parameter type</typeparam>
        /// <param name="action">the action to wrap up</param>
        /// <param name="defVal">default parameter value</param>
        /// <returns>BindableAction</returns>
        public static BindableAction<T> Wrap<T>(Action<T> action, T defVal)
        {
            return new BindableAction<T>(action, defVal);
        }
    }
}
