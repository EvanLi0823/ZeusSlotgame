using UnityEngine;
using System.Collections.Generic;
using System.Text;

public class SharedPlayerPrefs
{
    public static bool HasPlayerPrefsKey(string key)
    {
        return PlayerPrefs.HasKey(key);
    }

    public static void DeletePlayerPrefsKey(string key)
    {
        PlayerPrefs.DeleteKey(key);
    }

    public static void DeleteLocalDataBykey(string key)
    {
        if (PlayerPrefs.HasKey(key)) PlayerPrefs.DeleteKey(key);
    }

    public static string GetPlayerPrefsStringValue(string key, string defaultValue = null)
    {
        return PlayerPrefs.GetString(key, defaultValue);
    }

    public static int GetPlayerPrefsIntValue(string key, int defaultValue = 0)
    {
        if (PlayerPrefs.HasKey(key))
        {
            return PlayerPrefs.GetInt(key, defaultValue);
        }

        return defaultValue;
    }

    public static float GetPlayerPrefsFloatValue(string key, float defaultValue = 0f)
    {
        return PlayerPrefs.GetFloat(key, defaultValue);
    }

    public static void SetPlayerPrefsStringValue(string key, string defaultValue)
    {
        PlayerPrefs.SetString(key, defaultValue);
    }

    public static void SetPlayerPrefsIntValue(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
    }

    public static void SetPlayerPrefsFloatValue(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
    }

    public static void SetPlayerPrefsBoolValue(string key, bool value)
    {
        int boolInt = value ? 1 : 0;
        PlayerPrefs.SetInt(key, boolInt);
    }

    public static bool GetPlayerBoolValue(string key, bool defaultValue)
    {
        int boolInt = defaultValue ? 1 : 0;
        return (PlayerPrefs.GetInt(key, boolInt) >= 1);
    }

    public static float LoadPlayerPrefsFloat(string key, float defaultValue = 0)
    {
        string strValue = SharedPlayerPrefs.GetPlayerPrefsStringValue(key, null);
        if (strValue == null)
        {
            return defaultValue;
        }

        float result = defaultValue;
        if (float.TryParse(strValue, out result))
        {
            return result;
        }
        else
        {
            return defaultValue;
        }
    }

    public static void SavePlayerPrefsFloat(string key, float value)
    {
        SharedPlayerPrefs.SetPlayerPrefsStringValue(key, value.ToString());
        SharedPlayerPrefs.SavePlayerPreference();
    }

    public static long LoadPlayerPrefsLong(string key, long defaultValue = 0)
    {
        string strValue = SharedPlayerPrefs.GetPlayerPrefsStringValue(key, null);
        if (strValue == null)
        {
            return defaultValue;
        }

        long result = defaultValue;
        if (long.TryParse(strValue, out result))
        {
            return result;
        }
        else
        {
            return defaultValue;
        }
    }

    public static void SavePlayerPrefsLong(string key, long value)
    {
        SharedPlayerPrefs.SetPlayerPrefsStringValue(key, value.ToString());
        SharedPlayerPrefs.SavePlayerPreference();
    }

    public static void SavePlayerPreference()
    {
//		PlayerPrefs.Save();
    }

    public static void SavePlayerPrefsListInt(string key, List<int> value)
    {
        if (value == null || value.Count == 0)
        {
            return;
        }

        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < value.Count; i++)
        {
            sb.Append(value[i].ToString()).Append('|');
        }

        SetPlayerPrefsStringValue(key, sb.ToString());
    }

    public static List<int> LoadPlayerPrefsListInt(string key)
    {
        string[] mixture = GetPlayerPrefsStringValue(key, string.Empty).Split('|');
        if (mixture.Length == 0)
        {
            return new List<int>();
        }

        var ret = new List<int>();
        int parseResult;
        for (int i = 0; i < mixture.Length; i++)
        {
            if (int.TryParse(mixture[i], out parseResult))
            {
                ret.Add(parseResult);
            }
            else
            {
                continue;
            }
        }

        return ret;
    }

    public static void SavePlayerPrefsListString(string key, List<string> value)
    {
        if (value == null || value.Count == 0)
        {
            return;
        }

        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < value.Count; i++)
        {
            sb.Append(value[i]).Append(';');
        }

        SetPlayerPrefsStringValue(key, sb.ToString());
    }

    public static List<string> GetPlayerPrefsListString(string key)
    {
        string[] mixture = GetPlayerPrefsStringValue(key, string.Empty).Split(';');
        if (mixture.Length == 0)
        {
            return new List<string>();
        }

        var ret = new List<string>();
        for (int i = 0; i < mixture.Length; i++)
        {
            ret.Add(mixture[i]);
        }

        return ret;
    }
}