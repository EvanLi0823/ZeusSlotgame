using System.Diagnostics;
using Debug = UnityEngine.Debug;
using UnityEngine;
public class Log{
	[Conditional("DEBUG_LOG")]
	public static void Trace(params object[] msgs)
	{
		var s = "";
		foreach (var tmpS in msgs)
		{
			s += tmpS + "\t";
		}

		Debug.Log(s);
	}
	
	[Conditional("DEBUG_LOG")]
    public static void Warn(string msg)
	{
		Debug.LogWarning (msg);
    }

    [Conditional("DEBUG")]
    public static void Error(object msg)
	{
		Debug.LogError (msg.ToString());
    }

    [Conditional("DEBUG")]
    public static void LogLimeColor(object msg)
    {
	    Debug.Log( $"<color=lime> {msg}</color>"); 
    }
    
    [Conditional("DEBUG")]
    public static void LogYellowColor(object msg)
    {
        Debug.Log($"<color=yellow> {msg}</color>");
    }
    
    [Conditional("DEBUG")]
    public static void LogRedColor(object msg)
    {
	    Debug.Log( $"<color=red> {msg}</color>"); 
    }
    
    [Conditional("DEBUG")]
    public static void LogWhiteColor(object msg)
    {
	    Debug.Log( $"<color=white> {msg}</color>"); 
    }
    
    [Conditional("DEBUG")]
    public static void LogDictionary(object msg)
    {
		Debug.Log (UnityEngine.JsonUtility.ToJson(msg));
	}
    
    [Conditional("DEBUG_Log")]
    public static void TraceWithColor (object msg, Color color)
	{
		string htmlColor = ColorUtility.ToHtmlStringRGB (color);
		Debug.Log ("<color=#" + htmlColor + ">--------------------------------------->" + msg.ToString () + "</color>");
    }

    [Conditional("DEBUG_Log")]
    public static void TraceWithColor (object msg, string htmlColor)
	{
		Debug.Log ("<color=" + htmlColor + ">--------------------------------------->" + msg.ToString () + "</color>");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="colorIndex">0~10</param>
    [Conditional("DEBUG_Log")]
    public static void TraceWithColor (object msg, int colorIndex)
	{
		string htmlColor = "";
		switch (colorIndex)
		{
			case 0:
				htmlColor = "#00EEEE";
				break;
			case 1:
				htmlColor = "#FFFF00";
				break;
			case 2:
				htmlColor = "#FF4040";
				break;
			case 3:
				htmlColor = "#FFFFFF";
				break;
			case 4:
				htmlColor = "#FF6A6A";
				break;
			case 5:
				htmlColor = "#A0522D";
				break;
			case 6:
				htmlColor = "#EE6A50";
				break;
			case 7:
				htmlColor = "#CDC5BF";
				break;
			case 8:
				htmlColor = "#8E388E";
				break;
			case 9:
				htmlColor = "#7A7A7A";
				break;
			case 10:
				htmlColor = "#9400D3";
				break;
			default:
				htmlColor = ColorUtility.ToHtmlStringRGB (Color.green);
				break;

		}

		Debug.Log ("<color=" + htmlColor + ">--------------------------------------->" + msg.ToString () + "</color> ");
    }

    [Conditional("UNITY_EDITOR"), Conditional("DEBUG"), Conditional("DEVELOPMENT_BUILD")]
    public static void ErrorWithColor (object msg, int colorIndex = 0)
    {
		string htmlColor = "";
		switch (colorIndex)
		{
			case 0:
				htmlColor = "#00EEEE";
				break;
			case 1:
				htmlColor = "#FFFF00";
				break;
			case 2:
				htmlColor = "#FF4040";
				break;
			case 3:
				htmlColor = "#FFFFFF";
				break;
			case 4:
				htmlColor = "#FF6A6A";
				break;
			case 5:
				htmlColor = "#A0522D";
				break;
			case 6:
				htmlColor = "#EE6A50";
				break;
			case 7:
				htmlColor = "#CDC5BF";
				break;
			case 8:
				htmlColor = "#8E388E";
				break;
			case 9:
				htmlColor = "#7A7A7A";
				break;
			case 10:
				htmlColor = "#9400D3";
				break;
			default:
				htmlColor = ColorUtility.ToHtmlStringRGB (Color.green);
				break;

		}
		Debug.LogError ("<color=" + htmlColor + ">---------------------->滴： " + msg.ToString () + "</color> ");
    }

    [Conditional("DEBUG_Log")]
    public static void ErrorWithColor (object msg, Color color)
	{
		string htmlColor = ColorUtility.ToHtmlStringRGB (color);
		Debug.LogError ("<color=#" + htmlColor + ">--------------------------------------->" + msg.ToString () + "</color>");
    }
}
