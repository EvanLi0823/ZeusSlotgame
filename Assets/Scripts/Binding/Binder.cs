/********************************************************************************************
    RealYou Framework
    Module: Core/Binding
    Author: HU QIWEI
********************************************************************************************/
using System.Collections.Generic;
using UnityEngine;
using System;


namespace Binding
{
    /// <summary>
    /// binder object for 1-way or 2-way data binding
    /// </summary>
    public class Binder : MonoBehaviour
    {
        static List<Binder> allBinders = new List<Binder>();
        static List<Bindable> syncList = new List<Bindable>();
        static int lastFrameCount = -1;

        Binder()
        {
            if (allBinders.IndexOf(this) == -1)
                allBinders.Add(this);
        }

        /// <summary>
        /// Get Binder component from a GameObject. Create the Binder component if not yet exist
        /// bindings on the Binder object will be dismissed if the GameObject is destroyed
        /// </summary>
        /// <param name="gObject">GameObject to hold the binder</param>
        /// <returns></returns>
        public static Binder GetBinder(GameObject gObject)
        {
            Binder binder;
            if ((binder = gObject.GetComponent<Binder>()) == null)
                binder = gObject.AddComponent<Binder>();
            return binder;
        }


        Dictionary<Bindable, List<Bindable>> bindings = new Dictionary<Bindable, List<Bindable>>();

        /// <summary>
        /// Bind value from obj1 to obj2 (1way)
        /// </summary>
        /// <param name="obj1">get value from obj1</param>
        /// <param name="obj2">set value to obj2</param>
        public void Bind(Bindable obj1, Bindable obj2)
        {
            // find object pair
            if (obj1 == null || obj2 == null)
                return;

            // check bindflag
            if ((obj1._bindFlag & Bindable.BindFlags.Get) == 0)
                throw new UnityException("Bind from unreadable object");

            if ((obj2._bindFlag & Bindable.BindFlags.Set) == 0)
                throw new UnityException("Bind to unwritable object");

            List<Bindable> bindlist;
            if (!bindings.TryGetValue(obj1, out bindlist))
            {
                bindlist = new List<Bindable>();
                bindings.Add(obj1, bindlist);
            }

            if (bindlist.IndexOf(obj2) > -1)
                throw new UnityException("Duplicate binding");

            bindlist.Add(obj2);

            obj1.AddRef();
            obj2.AddRef();
            if (obj1._type != null && ((obj1._bindFlag & Bindable.BindFlags.NoInit) == 0))
            {
                if (obj1._marked)
                {
                    SyncData(obj1, null);
                    syncList.Remove(obj1);
                    obj1._marked = false;
                }
                else
                    SyncData(obj1, obj2);
            }
        }

        /// <summary>
        /// Bind value obj1 and obj2 (2-way)
        /// </summary>
        /// <param name="obj1">the first object</param>
        /// <param name="obj2">the second object</param>
        public void Bind2(Bindable obj1, Bindable obj2)
        {
            Bind(obj1, obj2);
            Bind(obj2, obj1);
        }

        /// <summary>
        /// Remove bindings between obj1 and obj2. These two object must be in the exactly same order when calling bind()
        /// </summary>
        /// <param name="obj1">the first object</param>
        /// <param name="obj2">the second object</param>
        public void RemoveBinding(Bindable obj1, Bindable obj2 = null)
        {
            if (obj1 == null)
                return;

            List<Bindable> bindlist;
            if (!bindings.TryGetValue(obj1, out bindlist))
                return;

            if (obj2 != null)
            {
                int n = bindlist.IndexOf(obj2);
                if (n == -1)
                    return;

                bindlist.RemoveAt(n);
                obj2.Release();
            }
            else
            {
                for (int i = 0; i < bindlist.Count; i++)
                    bindlist[i].Release();
                bindings.Remove(obj1);
                obj1.Release();
            }
        }

        /// <summary>
        /// Remove all bindings on the Binder
        /// </summary>
        public void RemoveAllBindings()
        {
            foreach (var kv in bindings)
            {
                List<Bindable> bindlist = kv.Value;
                for (int i = 0; i < bindlist.Count; i++)
                    bindlist[i].Release();
                kv.Key.Release();
            }
            bindings.Clear();
        }

        /// <summary>
        /// mark Bindable object value dirty to synchronize value.
        /// </summary>
        /// <param name="obj">the object to get value from</param>
        public static void MarkObject(Bindable obj)
        {
            if (!obj._marked) {
                obj._marked = true;
                syncList.Add(obj);
            }
        }

        void UpdateBindables()
        {
            var enumerator = bindings.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if ((enumerator.Current.Key._bindFlag & Bindable.BindFlags.NeedFrameUpdate) > 0)
                    enumerator.Current.Key.Update();
            }
            enumerator.Dispose();
        }

        void Sync(Bindable obj1, Bindable obj2)
        {
            try
            {
                TypeCode typeCode = Type.GetTypeCode(obj1._type);
                switch (typeCode)
                {
                    case TypeCode.Empty:
                        obj2.Set<object>(null);
                        break;
                    case TypeCode.Boolean:
                        obj2.Set<bool>(obj1.Get<bool>());
                        break;
                    case TypeCode.Byte:
                        obj2.Set<byte>(obj1.Get<byte>());
                        break;
                    case TypeCode.Char:
                        obj2.Set<char>(obj1.Get<char>());
                        break;
                    case TypeCode.DateTime:
                        obj2.Set<DateTime>(obj1.Get<DateTime>());
                        break;
                    case TypeCode.Decimal:
                        obj2.Set<decimal>(obj1.Get<decimal>());
                        break;
                    case TypeCode.Double:
                        obj2.Set<double>(obj1.Get<double>());
                        break;
                    case TypeCode.Int16:
                        obj2.Set<Int16>(obj1.Get<Int16>());
                        break;
                    case TypeCode.Int32:
                        obj2.Set<Int32>(obj1.Get<Int32>());
                        break;
                    case TypeCode.Int64:
                        obj2.Set<Int64>(obj1.Get<Int64>());
                        break;
                    case TypeCode.SByte:
                        obj2.Set<sbyte>(obj1.Get<sbyte>());
                        break;
                    case TypeCode.Single:
                        obj2.Set<float>(obj1.Get<float>());
                        break;
                    case TypeCode.String:
                        obj2.Set<string>(obj1.Get<string>());
                        break;
                    case TypeCode.UInt16:
                        obj2.Set<UInt16>(obj1.Get<UInt16>());
                        break;
                    case TypeCode.UInt32:
                        obj2.Set<UInt32>(obj1.Get<UInt32>());
                        break;
                    case TypeCode.UInt64:
                        obj2.Set<UInt64>(obj1.Get<UInt64>());
                        break;
                    case TypeCode.Object:
                        if (obj1._type.Equals(typeof(Vector2)))
                            obj2.Set<Vector2>(obj1.Get<Vector2>());
                        else
                        if (obj1._type.Equals(typeof(Vector3)))
                            obj2.Set<Vector3>(obj1.Get<Vector3>());
                        else
                        if (obj1._type.Equals(typeof(Vector4)))
                            obj2.Set<Vector4>(obj1.Get<Vector4>());
                        else
                        if (obj1._type.Equals(typeof(Color)))
                            obj2.Set<Color>(obj1.Get<Color>());
                        else
                        if (obj1._type.Equals(typeof(Rect)))
                            obj2.Set<Rect>(obj1.Get<Rect>());
                        else
                        if (obj1._type.Equals(typeof(Quaternion)))
                            obj2.Set<Quaternion>(obj1.Get<Quaternion>());
                        else
                        if (obj1._type.Equals(typeof(Version)))
                            obj2.Set<Version>(obj1.Get<Version>());
                        break;
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        void SyncData(Bindable obj1, Bindable obj2 = null)
        {
            List<Bindable> bindlist;
            if (bindings.TryGetValue(obj1, out bindlist))
            {
                if (obj2 == null)
                {
                    for (int i = 0; i < bindlist.Count; i++)
                        Sync(obj1, bindlist[i]);
                }
                else
                {
                    Sync(obj1, obj2);
                }
            }
        }

        private void Update()
        {
            if (Time.frameCount != lastFrameCount)
            {
                lastFrameCount = Time.frameCount;
                for (int i = 0; i < allBinders.Count; i++)
                    allBinders[i].UpdateBindables();

                for (int i = 0; i < syncList.Count; i++) {
                    Bindable obj = syncList[i];
                    for (int j = 0; j < allBinders.Count; j++)
                    {
                        allBinders[j].SyncData(obj);
                    }
                }

                for (int i = 0; i < syncList.Count; i++)
                    syncList[i]._marked = false;
                
                syncList.Clear();
            }
        }

        private void OnDestroy()
        {
            RemoveAllBindings();
            allBinders.Remove(this);
        }
    }

    public static class BinderGameObjectExtensions
    {
        /// <summary>
        /// Get binder from GameObject. Create if not exist yet
        /// </summary>
        /// <param name="gObject">GameObject</param>
        /// <returns>Binder object</returns>
        public static Binding.Binder Binder(this GameObject gObject)
        {
            return Binding.Binder.GetBinder(gObject);
        }

        /// <summary>
        /// bind value from obj1 to obj2 (1-way)
        /// </summary>
        /// <param name="gObject">GameObject</param>
        /// <param name="obj1">get value from obj1</param>
        /// <param name="obj2">set value to obj2</param>
        public static void Bind(this GameObject gObject, Bindable obj1, Bindable obj2)
        {
            Binder(gObject).Bind(obj1, obj2);
        }

        /// <summary>
        /// bind obj1 with obj2 (2-way)
        /// </summary>
        /// <param name="gObject">GameObject</param>
        /// <param name="obj1">the first object</param>
        /// <param name="obj2">the second object</param>
        public static void Bind2(this GameObject gObject, Bindable obj1, Bindable obj2)
        {
            Binder(gObject).Bind2(obj1, obj2);
        }

        /// <summary>
        /// Remove binding of obj1 to obj2
        /// </summary>
        /// <param name="gObject">GameObject</param>
        /// <param name="obj1">get value from obj1</param>
        /// <param name="obj2">set value to obj2</param>
        public static void RemoveBinding(this GameObject gObject, Bindable obj1, Bindable obj2 = null)
        {
            Binding.Binder binder = gObject.GetComponent<Binding.Binder>();
            binder?.RemoveBinding(obj1, obj2);
        }

        /// <summary>
        /// Remove all bindings on GameObject
        /// </summary>
        /// <param name="gObject">GameObject</param>
        public static void RemoveAllBindings(this GameObject gObject)
        {
            Binding.Binder binder = gObject.GetComponent<Binding.Binder>();
            binder?.RemoveAllBindings();
        }
    }

}
