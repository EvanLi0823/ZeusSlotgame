using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Libs;
using System.IO;
public static class LoadingSceneUtil
{

    public static AssetBundle loadingBundle;

    public static Sprite GetSceneLoading(object data)
    {
        SlotMachineConfig slotMachineConfig;
        string sceneName;
        if (data is SlotMachineConfig)
        {
            slotMachineConfig = data as SlotMachineConfig;
            sceneName = slotMachineConfig.Name();
        }
        else
        {
            return null;
        }
        if (sceneName.Equals(GameConstants.LOBBY_NAME))
        {
            return null;
        }
        Sprite s = null;
        s = Resources.Load(sceneName + "/loading", typeof(Sprite)) as Sprite;
        return s;
    }

    private static string ConvertTmpPathToTargetSavePath(string tmpPath)
    {
        return tmpPath.Substring(0, tmpPath.Length - 4);
    }
}
