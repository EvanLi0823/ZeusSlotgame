/********************************************************************************************
    RealYou Framework
    Module: Core/Binding
    Author: HU QIWEI
********************************************************************************************/
using UnityEngine;
using System;

namespace Binding
{
    public class BindableValue<T> : Bindable<T>
    {
        [SerializeField]
        public T Value
        {
            get { return _value; }
            set
            {
                this._value = value;
                Mark();
            }
        }

        public BindableValue(T initval)
        {
            _value = initval;
        }

        internal override void Set<TValue>(TValue val)
        {
            T tmp = (T)Convert.ChangeType(val, typeof(T));
            if (!_value.Equals(tmp)) {
                _value = tmp;
                Mark();
            }
        }        
    }
}
