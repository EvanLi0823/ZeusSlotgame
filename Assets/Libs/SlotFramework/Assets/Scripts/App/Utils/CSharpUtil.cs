using System;
using System.Collections.Generic;
using Plugins;
public class CSharpUtil
{
    public static long SECOND = 1L;
    public static long MINUTE = SECOND * 60;
    public static long HOUR = MINUTE * 60;
    public static long DAY = HOUR * 24;

    public static long ToTimeStamp(DateTime dateTime)
    {
        long ticks = DateTime.UtcNow.Ticks - dateTime.Ticks;
        ticks /= 10000000; //Convert windows ticks to seconds
        return ticks;
    }

    public static DateTime ToDateTimeFromTimeStamp(long timeStamp)
    {
        // Unix timestamp is seconds past epoch
        System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        dtDateTime = dtDateTime.AddSeconds(timeStamp).ToLocalTime();
        return dtDateTime;
    }

    public static string FixIntVersion(string version)
    {
        string fixedVersion = version;
        int intValue = 0;
        if (int.TryParse(version, out intValue))
        {
            fixedVersion = string.Format("{0}.0.0", version);
        }

        return fixedVersion;
    }

    public static string FixInvalidVersion(string version, string defaultVersion)
    {
        string fixedVersion = version;
        try
        {
            new Version(version);
        }
        catch (Exception)
        {
            fixedVersion = defaultVersion;
        }

        return fixedVersion;
    }


    public static T GetValueWithPath<T>(Dictionary<string, object> dict, string path, T defaultValue)
    {
        if (dict == null)
        {
            return defaultValue;
        }

        if (path.Contains("/"))
        {
            string[] pathComponents = path.Split(pathSeperatorChars, StringSplitOptions.RemoveEmptyEntries);
            Dictionary<string, object> node = dict;
            for (int i = 0; i < pathComponents.Length - 1; i++)
            {
                if (node.ContainsKey(pathComponents[i]))
                {
                    node = (Dictionary<string, object>)node[pathComponents[i]];
                }
                else
                {
                    node = null;
                    break;
                }
            }

            if (node != null && node.ContainsKey(pathComponents[pathComponents.Length - 1]))
            {
                return (T)Convert.ChangeType(node[pathComponents[pathComponents.Length - 1]], typeof(T));
            }
        }

        return GetValue<T>(dict, path, defaultValue);
    }

    public static bool SetValueWithPath<T>(Dictionary<string, object> dict, string path, T value)
    {
        if (dict == null) return false;

        if (path.Contains("/"))
        {
            string[] pathComponents = path.Split(pathSeperatorChars, StringSplitOptions.RemoveEmptyEntries);
            Dictionary<string, object> node = dict;

            for (int i = 0; i < pathComponents.Length - 1; i++)
            {
                if (!node.ContainsKey(pathComponents[i]))
                {
                    node = null;
                    break;
                }

                node = (Dictionary<string, object>)node[pathComponents[i]];
            }

            if (node != null && node.ContainsKey(pathComponents[pathComponents.Length - 1]))
            {
                node[pathComponents[pathComponents.Length - 1]] = value;
                return true;
            }
        }

        return SetValue<T>(dict, path, value);
    }

    public static bool SetValue<T>(Dictionary<string, object> dict, string key, T value)
    {
        if (dict == null) return false;
        if (!dict.ContainsKey(key)) return false;

        dict[key] = value;
        return true;
    }

    public static T GetValue<T>(Dictionary<string, object> dict, string key, T defaultValue)
    {
        if (dict == null)
        {
            return defaultValue;
        }

        if (dict.ContainsKey(key))
        {
            return (T)Convert.ChangeType(dict[key], typeof(T));
        }
        else
        {
            return defaultValue;
        }
    }

    public static void CheckNotNull(object obj, string errorMessage)
    {
        if (obj == null)
        {
            throw new System.ArgumentNullException(errorMessage);
        }
    }

    public static int[] ParseIntArray(string values)
    {
        string[] tokens = values.Split(',');
        int[] intArray = Array.ConvertAll<string, int>(tokens, int.Parse);
        return intArray;
    }

    public static int[,] ParseTwoDimIntArray(List<object> rows)
    {
        int[,] table = null;
        int rowIndex = 0;
        foreach (object rowObj in rows)
        {
            string row = (string)rowObj;
            string[] tokens = row.Split(',');
            int[] rowNumbers = Array.ConvertAll<string, int>(tokens, int.Parse);
            if (table == null)
            {
                table = new int[rows.Count, rowNumbers.Length];
            }
            else
            {
                if (rowNumbers.Length != table.GetUpperBound(1) + 1)
                {
                    throw new Exception("Invalid PayTable row:" + rowIndex);
                }
            }

            for (int col = 0; col < rowNumbers.Length; col++)
            {
                table[rowIndex, col] = rowNumbers[col];
            }

            rowIndex++;
        }

        return table;
    }

    public static T GetPlatformValue<T>(Dictionary<string, object> dict, string key, T defaultValue)
    {
        if (dict == null)
        {
            return defaultValue;
        }

        if (dict.ContainsKey(key))
        {
            if (dict[key] is Dictionary<string, object>)
            {
                Dictionary<string, object> valueDict = dict[key] as Dictionary<string, object>;
                string os = Libs.AssetsPathManager.GetPlatformName();
                return CSharpUtil.GetValue<T>(valueDict, os, defaultValue);
            }
            else
            {
                return CSharpUtil.GetValue<T>(dict, key, defaultValue);
            }
        }

        return defaultValue;
    }


    public static string GetCoinsMultiplerValue(Dictionary<string, object> dict, string key, string defaultValue)
    {
        if (dict == null)
        {
            return defaultValue;
        }

        if (dict.ContainsKey(key))
        {
            if (dict[key] is Dictionary<string, object>)
            {
                Dictionary<string, object> valueDict = dict[key] as Dictionary<string, object>;
                string multipler = Core.ApplicationConfig.GetInstance().ShowCoinsMultiplier.ToString();
                return CSharpUtil.GetValue<string>(valueDict, multipler, defaultValue);
            }
            else
            {
                return CSharpUtil.GetValue<string>(dict, key, defaultValue);
            }
        }

        return defaultValue;
    }

    public static Dictionary<string, object> ParsePlistByteArray(byte[] byteArray)
    {
        Dictionary<string, object> plist = Plist.readPlist(byteArray) as Dictionary<string, object>;
        //            System.Text.UTF8Encoding enc = new System.Text.UTF8Encoding();
        //            string str = enc.GetString(byteArray);
        return plist;
    }

    public static bool IsHTTPURL(string url)
    {
        return !string.IsNullOrEmpty(url) &&
               (url.StartsWith("http://", StringComparison.CurrentCultureIgnoreCase) ||
                url.StartsWith("https://", StringComparison.CurrentCultureIgnoreCase));
    }

    public static long ConvertDateTimeLongInSecond(System.DateTime time)
    {
        return (time.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
    }

    public static DateTime ConvertLongToDateInSecond(long time)
    {
        System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
        return startTime.AddTicks(time * 10000000);
        // DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime (new DateTime (1970, 1, 1));
        // long lTime = 0L;
        // try {
        // 	lTime = long.Parse (time + "0000000");
        // } catch (Exception ex) {
        // 	UnityEngine.Debug.LogError ("please check time is seconds type,the param time not beyond 12bit");
        // }

        // TimeSpan toNow = new TimeSpan (lTime);
        // DateTime dtResult = dtStart.Add (toNow);
        // return dtResult;
    }

    public static byte[] ArrayCopy(byte[] bytes0, byte[] bytes1)
    {
        byte[] resultBytes = new byte[bytes0.Length + bytes1.Length];
        Array.Copy(bytes0, 0, resultBytes, 0, bytes0.Length);
        Array.Copy(bytes1, 0, resultBytes, bytes0.Length, bytes1.Length);
        return resultBytes;
    }

    private static readonly char[] pathSeperatorChars = new char[] { '/' };
}