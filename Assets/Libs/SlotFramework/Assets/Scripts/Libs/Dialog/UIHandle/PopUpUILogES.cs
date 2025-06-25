using System.Collections.Generic;
using UnityEngine;

namespace Libs
{
    public class PopUpUILogES
    {
        private const string LOG_UI_ERROR_TYPE_KEY =  "NewPopupUI";
        private const string LOG_UI_TRACK_KEY = "DEBUG_LOG";
        private const string KEY_PATH = "ApplicationConfig/UILogSwitch";
        private const string ERROR_KEY_PATH = "ApplicationConfig/UIErrorSwitch";
        private static bool sendLog  = false;
        private static bool SendError = false;
        public static void SendError2ES(string msg)
        {
            if (!SendError) return;

            #if UNITY_EDITOR
            Debug.LogError("NewPopUpUI:"+msg);
            #endif
            
            BaseGameConsole.ActiveGameConsole().LogError(msg,LOG_UI_ERROR_TYPE_KEY);
        }

        public static void SetSwitch()
        {
            sendLog = Plugins.Configuration.GetInstance().GetValueWithPath<bool>(KEY_PATH, false);
            SendError = Plugins.Configuration.GetInstance().GetValueWithPath<bool>(ERROR_KEY_PATH, false);
        }
        public static void SendLog2ES(string msg,string tag = "DEBUG")
        {
            if (!sendLog) return;
            #if UNITY_EDITOR
            Debug.Log($"<color=red>Log: {msg}</color>");
            #endif
            if(string.IsNullOrEmpty(msg))
                return;

            if (string.IsNullOrEmpty(tag))
            {
                tag = "none";
            }
            Dictionary<string, object> eventParams = new Dictionary<string, object>();
            eventParams.Add("msg",msg);
            eventParams.Add("tag",tag);
            
            BaseGameConsole.ActiveGameConsole().LogBaseEvent(LOG_UI_TRACK_KEY, eventParams);
        }
    }
}