using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using UnityEngine;
namespace Classic{
	/// <summary>
	/// Singleton.
	/// <description>
	/// 此代码引用位置
	/// https://www.codeproject.com/Articles/14026/Generic-Singleton-Pattern-using-Reflection-in-C
	/// http://www.cnblogs.com/zhili/p/SingletonPatterm.html
	/// 所有单例在初始化时，都需要对数据成员进行重置处理,因为可能单例的配置数据会进行重置更新处理
	/// </description>
	/// </summary>
	public static class Singleton<T> where T:class  {
		// 定义一个静态变量来保存类的实例
		private static volatile T uniqueInstance;

		// 定义一个标识确保线程同步
		private static readonly object locker = new object();

		// 定义私有构造函数，使外界不能创建该类实例
		static Singleton()
		{
		}

		/// <summary>
		/// 定义公有方法提供一个全局访问点,同时你也可以定义公有属性来提供全局访问点
		/// </summary>
		/// <returns></returns>
		public static T Instance
		{
			// 当第一个线程运行到这里时，此时会对locker对象 "加锁"，
			// 当第二个线程运行该方法时，首先检测到locker对象为"加锁"状态，该线程就会挂起等待第一个线程解锁
			// lock语句运行完之后（即线程运行完之后）会对该对象"解锁"
			// 双重锁定只需要一句判断就可以了
			get{
				if (uniqueInstance == null)
				{
					lock (locker)
					{
						// 如果类的实例不存在则创建，否则直接返回
						if (uniqueInstance == null)
						{
							ConstructorInfo constructor = null;

							try
							{
								// Binding flags exclude public constructors.
								constructor = typeof(T).GetConstructor(BindingFlags.Instance | 
									BindingFlags.NonPublic, null, new Type[0], null);
							}
							catch (Exception exception)
							{
								Debug.LogError (exception.Message);
							}

							if (constructor == null || constructor.IsAssembly)
								// Also exclude internal constructors.
								Debug.LogError (string.Format("A private or " + 
									"protected constructor is missing for '{0}'.", typeof(T).Name));

							uniqueInstance = (T)constructor.Invoke(null);
						}
					}
				}
				return uniqueInstance;
			}
		}

	}
}

