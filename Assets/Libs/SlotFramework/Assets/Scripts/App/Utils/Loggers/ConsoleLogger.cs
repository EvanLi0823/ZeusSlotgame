using System;
using System.Collections;

using Utils;

namespace Utils.Loggers
{
    public class ConsoleLogger : Logger
    {

        public ConsoleLogger () : base()
        {

        }

        public override void Log (string message, bool enabled = true)
        {
            if (EnableDebugLog && enabled) {
                Console.Out.WriteLine (string.Format ("{1} Info:{0}", message, DateTime.Now.ToString(TimeHeaderFormat)));
            }
        }
        
        public override void LogError (string error, bool enabled = true)
        {
            if (EnableDebugError && enabled) {
                Console.Error.WriteLine (string.Format ("{1} Error!!!:{0}", error, DateTime.Now.ToString(TimeHeaderFormat)));
            }
        }
        
        public override void LogWarning (string warning, bool enabled = true)
        {
            if (EnableDebugWarning && enabled) {
                Console.Error.WriteLine (string.Format ("{1} Warning:{0}", warning, DateTime.Now.ToString(TimeHeaderFormat)));
            }
        }
    }
}
