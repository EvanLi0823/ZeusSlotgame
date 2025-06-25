//#define TRACE_METHOD_INVOCATION
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Diagnostics;

using Utils.Loggers;

namespace Utils
{
    public abstract class Logger
    {
        public static readonly string TimeHeaderFormat = "h:mm:ss.fff";
      
        public static Logger GetInstance (Type clazz, Type loggerClazz)
        {
            return GetInstance(clazz,loggerClazz,true,true,true);
        }

        public static Logger GetInstance (Type clazz, Type loggerClazz, bool enableDebugLog, bool enableDebugWarning, bool enableDebugError)
        {
            if (loggers.ContainsKey (clazz) && loggers[clazz].GetType() == loggerClazz) {
                global::Utils.Logger logger = loggers[clazz];

                logger.EnableDebugLog =  enableDebugLog;
                logger.EnableDebugWarning = enableDebugWarning;
                logger.EnableDebugError = enableDebugError;

                return logger;
            } 
            if (loggerClazz != null) {
                global::Utils.Logger logger = (Logger)Activator.CreateInstance(loggerClazz);

                logger.EnableDebugLog =  enableDebugLog;
                logger.EnableDebugWarning = enableDebugWarning;
                logger.EnableDebugError = enableDebugError;

                loggers.Add (clazz, logger);
                return logger;
            }
            
            return DefaultLogger;
        }

        public static Logger GetConsoleLogger(Type clazz,bool enableDebugLog = true, bool enableDebugWarning = true, bool enableDebugError = true)
        {
            return GetInstance(clazz, typeof(ConsoleLogger), enableDebugLog,enableDebugWarning,enableDebugError);
        }

        public static Logger GetUnityDebugLogger(Type clazz,bool enableDebugLog = true, bool enableDebugWarning = true, bool enableDebugError = true)
        {
            return GetInstance(clazz, typeof(UnityDebugLogger), enableDebugLog,enableDebugWarning,enableDebugError);
        }

        protected Logger ()
        {
            EnableDebugLog = true;
            EnableDebugError = true;
            EnableDebugWarning = true;
        }

        public void Assert (bool b, string message = "", bool enabled = true)
        {
            if (!b) {
                LogError(message,enabled);
            }
        }

		public void AssertF (bool b, string messageFormat, params object[] parms)
		{
			if (!b) {
				LogErrorF(messageFormat,parms);
			}
		}

        public abstract void Log (string message, bool enabled = true);
    
        public abstract void LogError (string error, bool enabled = true);

        public abstract void LogWarning (string warning, bool enabled = true);
    
		public void LogF (string messageFormat, params object[] parms)
		{
			if (EnableDebugLog) {
				Log(string.Format(messageFormat,parms));
			}
		}

		public void LogErrorF (string errorFormat, params object[] parms)
		{
			if (EnableDebugError) {
				LogError(string.Format(errorFormat,parms));
			}
		}
		
		public void LogWarningF (string warningFormat, params object[] parms)
		{
			if (EnableDebugWarning) {
				LogWarning(string.Format(warningFormat,parms));
			}
		}

        public void LogMethodInvoked(string message = "", bool enabled = true)
        {
#if TRACE_METHOD_INVOCATION
            StackTrace st = new StackTrace (true);
            StackFrame sf = st.GetFrame (1);
            
            string methodName = sf.GetMethod ().Name;

            StackFrame caller = st.GetFrame(2);
            int line = 0;
            string callerMethodName = "no caller";
            if (caller != null) {
                line = caller.GetFileLineNumber();
                callerMethodName = caller.GetMethod().Name;
            }

            Log (string.Format("{4} Method {0} is invoked. caller {1} ,line:{2}. {3}",methodName,callerMethodName,line, message,sf.GetMethod().DeclaringType),enabled);
#endif
        }

		public void LogMethodInvokedF(string messageFormat = "", params object[] args)
		{
#if TRACE_METHOD_INVOCATION
			LogMethodInvoked(string.Format(messageFormat,args));
#endif
		}
        public void LogException(Exception ex) 
        {
            LogError(string.Format ("{0}",ex));
        }
        private DateTime methodInvokedTime;
      
        public void StartTimeProfile()
        {
            methodInvokedTime = DateTime.Now;
        }

        public void CompleteTimeProfile(string message = "")
        {
            Log (string.Format("{1}: execution {0} seconds",(DateTime.Now - methodInvokedTime).TotalSeconds,message),true);
        }

        public bool EnableDebugLog { get; set; }

        public bool EnableDebugError { get; set; }

        public bool EnableDebugWarning { get; set; }


        private static Logger DefaultLogger = new ConsoleLogger();
        private static readonly Dictionary<Type,Logger> loggers = new Dictionary<Type,Logger> ();
    }
}
