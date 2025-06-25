using System.Collections.Generic;
public static class DsExtension
{

    public static int GetIntValue (this Dictionary<string, object> dict, string key, int defaultValue)
    {
        return Utils.Utilities.GetInt (dict, key, defaultValue);
    }
    
    
    public static long GetLongValue (this Dictionary<string, object> dict, string key, long defaultValue)
    {
        return Utils.Utilities.GetLong (dict, key, defaultValue);
    }

    public static float GetFloatValue (this Dictionary<string, object> dict, string key, float defaultValue)
    {
        return Utils.Utilities.GetFloat (dict, key, defaultValue);
    }

    public static T GetValue<T> (this Dictionary<string, object> dict, string key, T defaultValue)
    {
        return Utils.Utilities.GetValue (dict, key, defaultValue);
    }

    public static bool GetBool (this Dictionary<string, object> dict, string key, bool defaultValue)
    {
        return Utils.Utilities.GetBool (dict, key, defaultValue);
    }

}
