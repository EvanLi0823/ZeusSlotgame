using System.Collections.Generic;

namespace Libs
{
    public class ConfigDataParser
    {
        public static Dictionary<string, object> ExtractConfigData(byte[] data)
        {
            string dataStr = System.Text.Encoding.UTF8.GetString(data);
            if (string.IsNullOrEmpty(dataStr)) return null;
            return MiniJSON.Json.Deserialize(dataStr) as Dictionary<string,object>;
        }
    }
}