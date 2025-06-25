using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuleUtils 
{
    public static  float CastValueFloat(object o,float defaultValue = 0){
        if (o == null) {
            return defaultValue;
        }

        if (o is int) {
            return (float)((int)o);
        } else if (o is long) {
            return (float)((long)o);
        } else if (o is double) {
            return (float)((double)o);
        } else if (o is float) {
            return ((float)o);
        } else {
            float result = defaultValue;
            if (float.TryParse(o.ToString(), out result)) {
                return result;
            }
        }
        return defaultValue;
    }
    
    public static int CastValueInt(object o,int defaultValue = 0){
        if (o == null) {
            return defaultValue;
        }

        if (o is int) {
            return ((int)o);
        } else if (o is long) {
            return (int)((long)o);
        } else if (o is double) {
            return (int)((double)o);
        } else if (o is float) {
            return (int)((float)o);
        } else {
            int result = defaultValue;
            if (int.TryParse(o.ToString(), out result)) {
                return result;
            }
        }
        return defaultValue;
    }
    
    public static  long CastValueLong(object o,long defaultValue = 0){
        if (o == null) {
            return defaultValue;
        }

        if (o is int) {
            return (long)((int)o);
        } else if (o is long) {
            return ((long)o);
        } else if (o is double) {
            return (long)((double)o);
        } else if (o is float) {
            return (long)((float)o);
        } else {
            long result = defaultValue;
            if (long.TryParse(o.ToString(), out result)) {
                return result;
            }
        }
        return defaultValue;
    }
    
    public static double ConvertToUnixTimestamp(DateTime date)
    {
        DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        TimeSpan diff = date.ToUniversalTime() - origin;
        return Math.Floor(diff.TotalSeconds);
    }

}
