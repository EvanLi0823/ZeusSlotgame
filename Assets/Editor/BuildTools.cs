using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using Utils;

[System.Serializable]
public class PackageData
{
   public string PackageName;
   public string UToAMethodName;
   public string ResourceName;
   public string SceneName;
   public bool IsPortrait;
   public string Language;
   public bool isSelect = false;
   public PackageData(string packageName,string methodName,string resourceName,string sceneName,string language,bool isPortrait = false,bool select = false)
   {
      PackageName = packageName;
      UToAMethodName = methodName;
      ResourceName = resourceName;
      SceneName = sceneName;
      IsPortrait = isPortrait;
      Language = language;
      isSelect = select;
   }
}
public class BuildTools : EditorWindow
{
   private const string PackageNameKey = "PackageName";
   private const string UToAMethodNameKey = "UToAMethodName";
   private const string ResourceNameKey = "ResourceName";
   private const string SceneNameKey = "SceneName";
   private const string  IsPortraitKey= "IsPortrait";
   private const string  IsSelectedKey= "isSelect";
   private const string  LanguageKey= "Language";
   private bool initOver = false;
   private string filePath = "/Editor/PackageConfig.json";
   private string jsonPath = "";

   private Dictionary<string, PackageData> packageDatas = new Dictionary<string, PackageData>();
   PackageData[] packageDataList;
   private Vector2 scrollPos = Vector2.zero;

   private AnimBool m_ShowExtraFields;
   
   private PackageData curPackage;
   
   [MenuItem("Tools/BuildTools",false,10)]
   static void ShowChangeGUIDToolsWindox()
   {
      BuildTools editor = (BuildTools)GetWindowWithRect (typeof(BuildTools), new Rect (100, 100, 500, 800), false, "BuildTools");
      editor.Show ();
   }
   
   void Init()
   {
      jsonPath = Application.dataPath + filePath;
      if (!initOver)
      {
         GetPackageInfo();
      }
      initOver = true;
   }

   private void OnEnable()
   {
      m_ShowExtraFields = new AnimBool(true);
      m_ShowExtraFields.valueChanged.AddListener(Repaint);
   }

   //读取PackageConfig.json文件，
   void GetPackageInfo()
   {
      try
      {
         if(File.Exists(jsonPath))
         {
            string jsonContent = File.ReadAllText(jsonPath, System.Text.Encoding.UTF8);
            Dictionary<string,object> datas= MiniJSON.Json.Deserialize(jsonContent) as Dictionary<string,object>;
            if (datas==null || datas.Count==0)
            {
               return;
            }
            foreach (var data in datas)
            {
               Dictionary<string, object> dict = data.Value as Dictionary<string, object>;
               string packageName = Utils.Utilities.GetString(dict, PackageNameKey, null);
               string methodName = Utils.Utilities.GetString(dict, UToAMethodNameKey, null);
               string sceneName = Utils.Utilities.GetString(dict, SceneNameKey, null);
               string resourceName = Utils.Utilities.GetString(dict, ResourceNameKey, null);
               string language = Utils.Utilities.GetString(dict, LanguageKey, null);
               bool isPortrait = Utilities.GetBool(dict, IsPortraitKey, false);
               bool isSelect = Utilities.GetBool(dict, IsSelectedKey, false);
               PackageData packageData = new PackageData(packageName,methodName,resourceName,sceneName,language,isPortrait,isSelect);
               packageDatas.Add(data.Key,packageData);
            }
            //Debug.Log(packageDatas.Count);
         }
      }
      catch (Exception e)
      {
         Console.WriteLine(e);
         throw;
      }
   }

   private void OnGUI()
   {
      Init();
      //画一个ScrollView，里面每个元素是 toggle
      // 滚动视图开始 
      EditorGUILayout.LabelField("打包模块");
      packageDataList = packageDatas.Values.ToArray();
      float itemHeight = 30; // 每个选项的高度
      float totalHeight = itemHeight * packageDataList.Length;
      float maxHeight = 800 * 0.5f;
      float weight = EditorGUIUtility.currentViewWidth;
      float height = Mathf.Min(maxHeight, totalHeight);
      Rect bgRect = new Rect(0, 20, weight, height + 5);
      GUI.color = new Color(0.2f, 0.2f, 0.2f); // 设置背景颜色（深灰色）
      GUI.Box(bgRect, ""); // 绘制纯色背景
      GUI.color = Color.white; // 恢复默认颜色
      
      scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(weight), GUILayout.Height(height));
      for (int i = 0; i < packageDataList.Length; i++)
      {
         packageDataList[i].isSelect = EditorGUILayout.ToggleLeft($""+packageDataList[i].SceneName, packageDataList[i].isSelect);
         if (packageDataList[i].isSelect)
         {
            curPackage = packageDataList[i];
            for (int j = 0; j < packageDataList.Length; j++)
            {
               if (j!=i)
               {
                  packageDataList[j].isSelect = false;
               }
            }
         }
      }
      EditorGUILayout.EndScrollView();
      GUILayout.BeginArea(new Rect(weight-80, height+30, 80, 50));
      GUILayout.BeginHorizontal();
      if (GUILayout.Button("+", GUILayout.Width(30))) {
         Debug.Log("添加模块");
      }
      if (GUILayout.Button("-", GUILayout.Width(30)) && packageDataList.Length > 0) {
         Debug.Log("删除模块"); // 删除选中模块
      }
    
      GUILayout.EndHorizontal();
      GUILayout.EndArea();
      EditorGUILayout.Space(20);
      m_ShowExtraFields.target = EditorGUILayout.ToggleLeft("显示详情", m_ShowExtraFields.target);
      if (EditorGUILayout.BeginFadeGroup(m_ShowExtraFields.faded))
      {
         EditorGUI.indentLevel++;
         RefreshExtraInfo();
         EditorGUI.indentLevel--;
      }
      EditorGUILayout.EndFadeGroup();
      if (GUILayout.Button("清理未选中包资源", GUILayout.Width(300)))
      {
         Debug.Log("清理未选中包资源");
      }
      EditorGUILayout.BeginHorizontal();
      if (GUILayout.Button("保存配置", GUILayout.Width(200)))
      {
         Debug.Log("保存配置");
         SaveSettings();
      }
      EditorGUILayout.Space(50);
      if (GUILayout.Button("打包", GUILayout.Width(200)))
      {
         Debug.Log("开始打包");
      }
      EditorGUILayout.EndHorizontal();
   }

   private void RefreshExtraInfo()
   {
      if (curPackage!=null)
      {
         EditorGUILayout.LabelField(PackageNameKey+" : "+curPackage.PackageName);
         EditorGUILayout.LabelField(UToAMethodNameKey+" : "+curPackage.UToAMethodName);
         EditorGUILayout.LabelField(ResourceNameKey+" : "+curPackage.ResourceName);
         EditorGUILayout.LabelField(SceneNameKey+" : "+curPackage.SceneName);
         EditorGUILayout.LabelField(LanguageKey+" : "+curPackage.Language);
         curPackage.IsPortrait = EditorGUILayout.Toggle(IsPortraitKey+" : ",curPackage.IsPortrait);
      }
      else
      {
         EditorGUILayout.LabelField(PackageNameKey+" : ");
         EditorGUILayout.LabelField(UToAMethodNameKey+" : ");
         EditorGUILayout.LabelField(ResourceNameKey+" : ");
         EditorGUILayout.LabelField(SceneNameKey+" : ");
         EditorGUILayout.LabelField(LanguageKey+" : ");
      }
   }
   //切换至当前选择的场景的配置
   private void SaveSettings()
   {
      //刷新 buildsettings场景
      string machinePath = "Assets/Scenes/" + curPackage.SceneName + ".unity";
      //loading 场景必需
      string loadingPath = "Assets/Scenes/Loading.unity";
      //过渡空场景必需
      string emptyScenePath = "Assets/Scenes/EmptyScene.unity";
     
      EditorBuildSettingsScene machineScene = new EditorBuildSettingsScene(machinePath,true);
      EditorBuildSettingsScene loadingScene = new EditorBuildSettingsScene(loadingPath, true);
      EditorBuildSettingsScene emptyScene = new EditorBuildSettingsScene(emptyScenePath, true);
      //场景顺序不能改动
      EditorBuildSettings.scenes = new EditorBuildSettingsScene[]{loadingScene,emptyScene,machineScene};
      
      //将 packageData数据存入 json数据中去
      try
      {
         string path = Application.dataPath + filePath;
         if (File.Exists(path))
         {
            File.Delete(path);
         }
         string dataInfo = JsonConvert.SerializeObject(packageDatas);
         File.WriteAllText(path,dataInfo,System.Text.Encoding.UTF8);
      }
      catch (Exception e)
      {
         Console.WriteLine(e);
         throw;
      }
      AssetDatabase.Refresh();
      AssetDatabase.SaveAssets();
   }
}
