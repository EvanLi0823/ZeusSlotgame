using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Classic;
using System.IO;
public class DeletePrefsCommand : NormalCommandItem
{    
    /** 删除关卡数据 */
    private const string DeleteSceneProgressData = "DeleteSceneProgressData";
    /** 删除APP下载数据 */ 
    private const string DeleteAllDownloadData = "DeleteAllDownloadData";
    
    public DeletePrefsCommand(string cmdToken, Dictionary<string, object> dict) : base(cmdToken, dict) { }

    public override void OnCommandItemCreateEnd() {
        base.OnCommandItemCreateEnd();
        DoCommandOperation();
        
        //   测试   "{\"name\":\"DeletePrefsCommand\",\"type\":\"DeletePrefsCommand\",\"paraDict\":{\"name\":\"DeletSceneProgressData\"},\"ESMsgDict\":{\"name\":\"DeletSceneProgressData\"}}"
    }

    public override void DoCommandOperation(Dictionary<string, object> contextInfo = null) {
        base.DoCommandOperation();
        var paraDict = Utils.Utilities.GetValue<Dictionary<string, object>>(ItemDict, GameConstants.ParaDict_Key, null);
        if (paraDict == null) {
            OnAccept();
            return;
        }
        
        string cmdName = Utils.Utilities.GetValue<string>(paraDict, GameConstants.name_Key, string.Empty);
        List<object> fileList = Utils.Utilities.GetValue<List<object>>(paraDict, "fileList", new List<object>());
        List<string> deleteFileList = new List<string>();
        switch (cmdName)
        {
            case DeleteSceneProgressData: 
                string folderPath = Path.Combine (Application.persistentDataPath, SceneProgressManager.DataFolderName);
                if (Directory.Exists(folderPath))
                {
                    DirectoryInfo dir = new DirectoryInfo(folderPath);
                    foreach (FileInfo file in dir.GetFiles())
                    {
                        if (fileList.Exists(p=>
                        {
                            string str = p as string;
                            if (string.IsNullOrEmpty(str)) return false;
                            return file.Name.Contains(str);
                        }))
                        {
                            deleteFileList.Add((file.Name));
                            file.Delete();
                        }
                    }
                }
                break;
            case DeleteAllDownloadData:
                string path = Application.persistentDataPath;
                if (Directory.Exists(path))
                {
                    DirectoryInfo dir = new DirectoryInfo(path);
                    dir.Delete(true);
                }
                Debug.Log("delete download data success");
                break;
            default:
                #if UNITY_EDITOR 
                string msg = string.Format("error Command {0}.paraDict.{1}", this.Name, cmdName);
                Debug.LogError(msg);
                #endif    
                break;
        }
        
        Dictionary<string,object> esMsgDict = Utils.Utilities.GetValue<Dictionary<string,object>>(ItemDict,GameConstants.ESMsgDict_Key,new Dictionary<string,object>());
        esMsgDict.Add("paraDict", paraDict);
        esMsgDict.Add("deleteFileList", deleteFileList);
        BaseGameConsole.ActiveGameConsole().LogBaseEvent(Analytics.ExecuteGM_Key, esMsgDict);
        
        OnAccept();
    }
}
