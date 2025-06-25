/********************************************************************************************
    RealYou Framework
    Module: Core/Binding
    Author: HU QIWEI
********************************************************************************************/
using UnityEngine;
using System;

namespace Binding
{
    public class BindableObject<T> : Bindable<T>
    {
        BindableObject()
        {
            _value = (T)Convert.ChangeType(this, typeof(T));
            _bindFlag = Bindable<T>.BindFlags.Get;
        }

        internal override void Set<TValue>(TValue val)
        {
            Debug.Log("denied: set to BindableObject<" + typeof(T).Name + ">");
        }
    }
}
