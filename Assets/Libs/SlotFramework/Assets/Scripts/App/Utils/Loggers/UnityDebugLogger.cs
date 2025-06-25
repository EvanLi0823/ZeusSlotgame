using UnityEngine;
using System.Collections;
using System;
using Utils;

namespace Utils.Loggers
{
    public class UnityDebugLogger : Logger
    {
    
        public UnityDebugLogger () : base()
        {
        
        }
    
        public override void Log (string message, bool enabled = true)
        {
            if (EnableDebugLog && enabled) {
                Debug.Log(string.Format ("{1} Info:{0}", message, DateTime.Now.ToString(TimeHeaderFormat)));
            }
        }
    
        public override void LogError (string error, bool enabled = true)
        {
            if (EnableDebugError && enabled) {
                Debug.LogError(string.Format ("{1} Error!!!:{0}", error, DateTime.Now.ToString(TimeHeaderFormat)));
            }
        }
    
        public override void LogWarning (string warning, bool enabled = true)
        {
            if (EnableDebugWarning && enabled) {
                Debug.LogWarning(string.Format ("{1} Warning:{0}", warning, DateTime.Now.ToString(TimeHeaderFormat)));
            }
        }
    }
}
