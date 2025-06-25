/********************************************************************************************
    RealYou Framework
    Module: Core/Binding
    Author: HU QIWEI
********************************************************************************************/
using UnityEngine;
using System;

namespace Binding
{
    /// <summary>
    /// Bindable object
    /// </summary>
    public partial class Bindable
    {
        internal int _refCount = 0;     // reference count for AddRef() and Release()
        internal Type _type;             // original data type

        [Flags]
        internal enum BindFlags
        {
            None,                       // no flags
            Get = 1,                    // data readable
            Set = 2,                    // data writable
            Both = 3,                   // data readable and writable
            NeedFrameUpdate = 4,        // need to update and check modified for every frame
            NoInit = 8                  // do not sync value when initializing
        };


        internal BindFlags _bindFlag = BindFlags.Both;
        internal bool _marked = false;

        internal virtual TValue Get<TValue>()
        {
            return default(TValue);
        }

        internal virtual void Set<TValue>(TValue val)
        {
        }

        internal virtual void AddRef()
        {
            _refCount++;
        }

        internal virtual void Release()
        {
            _refCount--;
            if (_refCount < 0)
                _refCount = 0;
        }

        /// <summary>
        /// when calling bind(), do not transfer value from this object.
        /// Synchronization after bind() is not affected.
        /// </summary>
        /// <returns>Bindable</returns>
        public Bindable NoInit()
        {
            _bindFlag |= BindFlags.NoInit;
            return this;
        }

        // update object status on every frame; check if it's "dirty"
        internal virtual void Update()
        {

        }

        /// <summary>
        /// mark object as "dirty" so it will be synchronized on next update
        /// </summary>
        public void Mark()
        {
            Binder.MarkObject(this);
        }
    }

    public class Bindable<T>: Bindable
	{
        [SerializeField]
        protected T _value;

        internal Bindable()
        {
            _type = typeof(T);
        }

        internal override TValue Get<TValue>()
        {
            return (TValue)Convert.ChangeType(_value, typeof(TValue));
        }

        internal override void Set<TValue>(TValue val)
        {
            //if (typeof(T) == typeof(TValue))
            _value = (T)Convert.ChangeType(_value, typeof(T));
        }
    }
}
