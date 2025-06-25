using System.Collections;
using System.Collections.Generic;
using Classic;
using UnityEngine;
using Libs;

/// <summary>
/// 用于存储与应用程序状态相关的信息
/// </summary>
public class ApplicationContext
{
   
   private ApplicationContext()
   {
      RegisterEvent();
   }
   
   ~ApplicationContext()
   {
      UnregisterEvent();
   }

   
   private void RegisterEvent()
   {
      
   }

   private void UnregisterEvent()
   {
      
   }
   
   public static ApplicationContext Instance{ get { return Classic.Singleton<ApplicationContext>.Instance; } }

}
