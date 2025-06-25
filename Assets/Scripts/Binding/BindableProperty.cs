/********************************************************************************************
    RealYou Framework
    Module: Core/Binding
    Author: HU QIWEI
********************************************************************************************/
using UnityEngine.Events;
using UnityEngine;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Binding
{
    public class BindableProperty<T> : Bindable<T>
	{
		protected object boundObject;
        protected PropertyInfo pinfo;
        protected FieldInfo finfo;

        public BindableProperty(object obj)
		{
			boundObject = obj;
            _bindFlag = BindFlags.Both | BindFlags.NeedFrameUpdate;
		}

        internal override TValue Get<TValue>()
		{
            return (TValue)(pinfo != null ? pinfo.GetValue(boundObject) : finfo.GetValue(boundObject));
		}

		internal override void Set<TValue>(TValue val)
		{
            if (pinfo != null)
                pinfo.SetValue(boundObject, (T)Convert.ChangeType(val, typeof(T)));
            else
                finfo.SetValue(boundObject, (T)Convert.ChangeType(val, typeof(T)));
        }

        internal void ParseExpression(LambdaExpression expression)
        {
            var body = expression.Body as MemberExpression;
            if (body == null)
                throw new ArgumentException(string.Format("Invalid expression:{0}", expression));

            if (!(body.Expression is ParameterExpression))
                throw new ArgumentException(string.Format("Invalid expression:{0}", expression));

            pinfo = body.Member as PropertyInfo;
            if (pinfo == null)
            {
                finfo = body.Member as FieldInfo;
                if (finfo == null)
                    throw new ArgumentException(string.Format("Invalid expression:{0}", expression));
                else
                    _value = (T)finfo.GetValue(boundObject);
            }
            else
                _value = (T)pinfo.GetValue(boundObject);
        }

        internal override void Update()
        {
            T newvalue = (T)(pinfo != null ? pinfo.GetValue(boundObject) : finfo.GetValue(boundObject));
            if (!newvalue.Equals(_value))
            {
                _value = newvalue;
                Mark();
            }
        }

        void Mark1(T val)
        {
            Mark();
        }

        UnityEvent _event;
        /// <summary>
        /// When event triggered, property value will be synchronized
        /// </summary>
        /// <param name="evt">event object</param>
        /// <returns>BindableProperty</returns>
        public BindableProperty<T> When(UnityEvent evt)
        {
            _bindFlag = BindFlags.Both;
            _event = evt;
            return this;
        }

        UnityEvent<T> _event1;
        /// <summary>
        /// When event triggered, property value will be synchronized
        /// </summary>
        /// <param name="evt"></param>
        /// <returns>BindableProperty</returns>
        public BindableProperty<T> When(UnityEvent<T> evt)
        {
            _bindFlag = BindFlags.Both;
            _event1 = evt;
            return this;
        }

        internal override void AddRef()
        {
            base.AddRef();
            if (_refCount == 1)
            {
                _event?.AddListener(Mark);
                _event1?.AddListener(Mark1);
            }
        }

        internal override void Release()
        {
            base.Release();
            if (_refCount == 0)
            {
                _event?.RemoveListener(Mark);
                _event1?.RemoveListener(Mark1);
            }
        }
    }

    public class BindableProperty<T, TOther> : BindableProperty<T>
    {
        public BindableProperty(object obj): base(obj)
        {
        }

        Func<T, TOther> getFunc = null;
        internal BindableProperty<T> GetWith(Expression<Func<T, TOther>> converter)
        {
            getFunc = converter.Compile();
            return this;
        }

        Func<TOther, T> setFunc = null;
        internal BindableProperty<T> SetWith(Expression<Func<TOther, T>> converter)
        {
            setFunc = converter.Compile();
            return this;
        }

        internal override TValue Get<TValue>()
        {
            object ret = pinfo != null ? pinfo.GetValue(boundObject) : finfo.GetValue(boundObject);
            if (getFunc != null)
                return (TValue)Convert.ChangeType(getFunc((T)ret), typeof(TValue));
            else
                return (TValue)ret;
        }

        internal override void Set<TValue>(TValue val)
        {
            T sval = setFunc != null ?
                setFunc((TOther)Convert.ChangeType(val, typeof(TOther))) :
                (T)Convert.ChangeType(val, typeof(T));

            if (setFunc != null)
                pinfo.SetValue(boundObject, sval);
            else
                finfo.SetValue(boundObject, sval);
        }
    }

    public static class ObjectBinderExtension
    {
        /// <summary>
        /// make property bindable from an object
        /// </summary>
        /// <typeparam name="T">object type</typeparam>
        /// <typeparam name="TProp">property type</typeparam>
        /// <param name="obj">object</param>
        /// <param name="memberExpression">expression to extract the property</param>
        /// <returns>BindableProperty</returns>
        public static BindableProperty<TProp> For<T, TProp>(this T obj, Expression<Func<T, TProp>> memberExpression) where T: class
        {
            if (memberExpression == null)
                return null;

            BindableProperty<TProp> prop = new BindableProperty<TProp>(obj);
            prop.ParseExpression(memberExpression);
            return prop;
        }

        /// <summary>
        /// make property bindable from an object, with type conversion
        /// </summary>
        /// <typeparam name="T">object type</typeparam>
        /// <typeparam name="TProp">property type</typeparam>
        /// <typeparam name="TOther">type to convert</typeparam>
        /// <param name="obj">object</param>
        /// <param name="memberExpression">expression to extract the property</param>
        /// <param name="getWith">convert property type to other type</param>
        /// <param name="setWith">convert other type to property type</param>
        /// <returns></returns>
        public static BindableProperty<TProp, TOther> For<T, TProp, TOther>
            (this T obj,
            Expression<Func<T, TProp>> memberExpression,
            Expression<Func<TProp, TOther>> getWith,
            Expression<Func<TOther, TProp>> setWith)
            where T : class
        {
            if (memberExpression == null)
                return null;

            BindableProperty<TProp, TOther> prop = new BindableProperty<TProp, TOther>(obj);
            prop.ParseExpression(memberExpression);
            if (getWith != null)
                prop.GetWith(getWith);
            if (setWith != null)
                prop.SetWith(setWith);
            return prop;
        }
    }
}
