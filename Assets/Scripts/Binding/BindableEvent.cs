/********************************************************************************************
    RealYou Framework
    Module: Core/Binding
    Author: HU QIWEI
********************************************************************************************/
using UnityEngine;
using UnityEngine.Events;

namespace Binding
{
    public class BindableEvent : Bindable<UnityEvent>
    {
        public BindableEvent(UnityEvent evt)
        {
            _bindFlag = BindFlags.Get | BindFlags.NoInit ;
            _value = evt;
            _type = null;
        }

        void BindEventHandler()
        {
            Mark();
        }

        internal override void AddRef()
        {
            base.AddRef();
            if (_refCount == 1)
                _value.AddListener(BindEventHandler);
        }

        internal override void Release()
        {
            base.Release();
            if (_refCount == 0)
                _value.RemoveListener(BindEventHandler);
        }

        internal override void Set<TValue>(TValue val)
        {
            Debug.Log("denied: set to BindableEvent");
        }
    }


    public class BindableEvent<T> : Bindable<T>
    {
        private UnityEvent<T> _event;
        public BindableEvent(UnityEvent<T> evt)
        {
            _bindFlag = BindFlags.Get | BindFlags.NoInit;
            _event = evt;
        }

        void BindEventHandler(T val)
        {
            _value = val;
            Mark();
        }


        internal override void AddRef()
        {
            base.AddRef();
            if (_refCount == 1)
                _event.AddListener(BindEventHandler);
        }

        internal override void Release()
        {
            base.Release();
            if (_refCount == 0)
                _event.RemoveListener(BindEventHandler);
        }

        internal override void Set<TValue>(TValue val)
        {
            Debug.Log("denied: set to BindableEvent<" + typeof(T).Name + ">");
        }
    }

    public partial class Bindable
    {
        /// <summary>
        /// Wrap a UnityEvent object as Bindable
        /// </summary>
        /// <param name="evt">event object</param>
        /// <returns>BindableEvent</returns>
        public static BindableEvent Wrap(UnityEvent evt)
        {
            return new BindableEvent(evt);
        }

        /// <summary>
        /// Wrap a UnityEvent object with event paramter type T as Bindable
        /// </summary>
        /// <typeparam name="T">event parameter type</typeparam>
        /// <param name="evt">event object</param>
        /// <returns>BindableEvent</returns>
        public static BindableEvent<T> Wrap<T>(UnityEvent<T> evt)
        {
            return new BindableEvent<T>(evt);
        }
    }
}
