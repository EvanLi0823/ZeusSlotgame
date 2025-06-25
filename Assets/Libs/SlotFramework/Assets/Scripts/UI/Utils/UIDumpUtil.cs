using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using Core;

namespace UI.Utils
{
    public class UIDumpUtil
    {
        public UIDumpUtil ()
        {
        }

        private static void Log(string message, bool force = false) {
            if (EnableDebugLog || force) {
                UnityEngine.Debug.Log(message);
            }
        }
        
        private static void LogError(string error, bool force = false)
        {
            if (EnableDebugError || force) {
                UnityEngine.Debug.Log (error);
            }
        }
        private static void LogWarning(string warning, bool force = false)
        {
            if (EnableDebugWarning || force) {
                UnityEngine.Debug.LogWarning(warning);
            }
        }
        
        private static readonly bool EnableDebugLog = false;
        private static readonly bool EnableDebugError = true;
        private static readonly bool EnableDebugWarning = true;
    }
}

