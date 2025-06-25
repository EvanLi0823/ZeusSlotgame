using UnityEngine;

public class SplashLogger
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    private static void LogBeforeSplash()
    {
        Debug.Log("=== Splash Screen即将显示 ==="); // 
    }
}