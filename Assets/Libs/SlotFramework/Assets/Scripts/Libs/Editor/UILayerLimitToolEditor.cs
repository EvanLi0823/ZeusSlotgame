#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Libs.SlotFramework.Assets.Scripts.Libs.Editor
{
    public class UILayerLimitToolEditor : EditorWindow
    {
        enum EState
        {
            Normal = 0,
            Success = 1,
            Error = 2,
        }

        enum ELayerLevel
        {
            One = 15,
            Two = 25,
            Three = 35,
            Four = 45
        }

        Dictionary<int, string> stateDict = new Dictionary<int, string>()
        {
            [0] = ">>>>>>>>>>>>>>>>>>>>>>>>>>>>>><color=yellow>{0}</color><<<<<<<<<<<<<<<<<<<<<<<<<<<<<<",
            [1] = "==============================<color=green>{0}</color>==============================",
            [2] = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxx<color=red>{0}</color>xxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"
        };

        //private List<GameObject> objList = new List<GameObject>();
        private Dictionary<string, GameObject> goDict = new Dictionary<string, GameObject>();
        private string MaxLayer = "10";
        private string MinLayer = "-1";
        private int selectIndex = 1;
        private string[] layerList = { "1级弹窗", "2级弹窗", "3级弹窗", "4级弹窗" };

        public List<int> layerAbs = new List<int>();

        private Dictionary<int, List<Object>> layerObjDict = new Dictionary<int, List<Object>>();

        [MenuItem("Assets/UI层级Layer限制器", false, 15)]
        public static void OpenUILayerLimitWindow()
        {
            WindowShow();
        }

        private static void WindowShow()
        {
            UILayerLimitToolEditor window =
                GetWindowWithRect<UILayerLimitToolEditor>(new Rect(100, 100, 600, 200), true, "UI层级限制器");
            window.Show();
        }

        private void OnGUI()
        {
            DrawGameObjectUI();
        }

        void DrawGameObjectUI()
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("修复当前物体的路径:" + GetAssetName());
            EditorGUILayout.Space();
            selectIndex = EditorGUILayout.Popup(selectIndex, layerList);

            EditorGUILayout.Space();

            if (GUILayout.Button("修改", GUILayout.Width(100)))
            {
                ChangeGameObjectLayer();
            }


            EditorGUILayout.EndVertical();
        }

        private void ChangeGameObjectLayer()
        {
            LayerLog("Begin Change !");

            SetSelectGameObj();
            try
            {
                foreach (var go in goDict)
                {
                    ChangeLayer(go);
                }
            }
            catch (Exception e)
            {
                LayerLog($"改变层级发生错误 ：{e.Message}");
                return;
            }

            LayerLog("Change Layer Success !", EState.Success);
        }

        private void SetSelectGameObj()
        {
            goDict.Clear();
            int index = -1;
            Object[] list = Selection.objects;
            if (list == null)
            {
                LayerLog("你没有选中物体！", EState.Error);
                return;
            }

            if (!CheckContainFolder(list, ref index))
            {
                GetObjects(list);
            }
            else
            {
                Object folder = list[index];
                GetObjectsForFolder(folder);
            }
        }

        private bool CheckContainFolder(Object[] objects, ref int index)
        {
            int count = objects.Length;

            for (int i = 0; i < count; i++)
            {
                string path = AssetDatabase.GetAssetPath(objects[i]);
                if (!string.IsNullOrEmpty(path) && !Path.HasExtension(path))
                {
                    index = i;
                    Debug.Log("选择到了文件夹 : " + i);
                    return true;
                }
            }

            return false;
        }

        private string GetAssetName()
        {
            Object[] objects = Selection.objects;
            if (objects == null) return "";

            string name = "";
            int index = -1;

            if (CheckContainFolder(objects, ref index))
            {
                name = AssetDatabase.GetAssetPath(objects[index]);
            }
            else
            {
                List<string> nameList = new List<string>();
                for (int i = 0; i < objects.Length; i++)
                {
                    string path = AssetDatabase.GetAssetPath(objects[i]);
                    nameList.Add(Path.GetFileNameWithoutExtension(path));
                }

                name = string.Join(",", nameList);
            }

            return name;
        }

        private void GetObjects(Object[] objects)
        {
            foreach (var obj in objects)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                if (Path.GetExtension(path) == ".prefab")
                {
                    GameObject go = PrefabUtility.LoadPrefabContents(path);
                    //GameObject go = obj as GameObject;
                    if (go != null) goDict.Add(path, go);
                }
            }
        }

        private void GetObjectsForFolder(Object objects)
        {
            string objectPath = AssetDatabase.GetAssetPath(objects);
            string path = Path.Combine(System.Environment.CurrentDirectory, objectPath);
            string[] objsPath = Directory.GetFiles(path, ".prefab", SearchOption.AllDirectories);
            for (int i = 0; i < objsPath.Length; i++)
            {
                GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(objsPath[i]);
                if (go != null) goDict.Add(path, go);
            }
        }

        private void ChangeLayer(KeyValuePair<string, GameObject> kv)
        {
            if (kv.Value == null) return;
            GameObject go = kv.Value;
            ParticleSystem[] list = go.GetComponentsInChildren<ParticleSystem>(true);
            if (list != null)
            {
                foreach (var ps in list)
                {
                    Renderer renderer = ps.GetComponent<Renderer>();
                    if (renderer == null) continue;
                    AddLayerDictAndHash(renderer.sortingOrder, renderer);
                }
            }

            SpriteRenderer[] srs = go.GetComponentsInChildren<SpriteRenderer>(true);
            if (srs != null)
            {
                foreach (var ps in srs)
                {
                    if (ps == null) continue;
                    AddLayerDictAndHash(ps.sortingOrder, ps);
                }
            }

            Canvas[] cvses = go.GetComponentsInChildren<Canvas>(true);
            if (cvses != null)
            {
                foreach (var cvs in cvses)
                {
                    if (cvs == null) continue;
                    AddLayerDictAndHash(cvs.sortingOrder, cvs);
                }
            }

            List<int> tempList = CalculateLayer();
            if (selectIndex != 0)
            {
                Canvas cv = go.GetComponent<Canvas>();
                if (cv == null)
                {
                    cv = go.AddComponent<Canvas>();
                }

                cv.overrideSorting = true;

                if (tempList != null && tempList.Count > 0)
                {
                    cv.sortingOrder = tempList[0];
                }

                GraphicRaycaster graphicRay = go.GetComponent<GraphicRaycaster>();
                if (graphicRay == null)
                {
                    graphicRay = go.AddComponent<GraphicRaycaster>();
                }
            }

            PrefabUtility.SaveAsPrefabAsset(go, kv.Key);
        }

        private List<int> CalculateLayer()
        {
            if (layerAbs.Count != layerObjDict.Count)
            {
                LayerLog("插入Layer集合出错，请查看代码！", EState.Error);
                return null;
            }

            ELayerLevel level = ELayerLevel.Two;
            layerAbs.Sort();
            switch (selectIndex)
            {
                case 0:
                    level = ELayerLevel.One;
                    break;
                case 1:
                    level = ELayerLevel.Two;
                    break;
                case 2:
                    level = ELayerLevel.Three;
                    break;
                case 3:
                    level = ELayerLevel.Four;
                    break;
                default:
                    return null;
            }

            int centerLayer = (int)level;

            int count = layerAbs.Count;
            int left = centerLayer - ((count - 1) / 2);

            List<int> tempList = new List<int>();
            var sortCount = left + count;
            for (int i = left; i < sortCount; i++)
            {
                tempList.Add(i);
            }

            for (int i = 0; i < layerAbs.Count; i++)
            {
                List<Object> objT = layerObjDict[layerAbs[i]];
                int layerTemp = tempList[i];
                CheckAndSetLayer(objT, layerTemp, centerLayer);
            }

            if (tempList.Count == 0) tempList.Add(centerLayer);

            return tempList;
        }

        private void AddLayerDictAndHash(int layer, Object value)
        {
            if (!layerAbs.Contains(layer))
            {
                layerAbs.Add(layer);
            }

            if (layerObjDict.ContainsKey(layer))
            {
                layerObjDict[layer].Add(value);
            }
            else
            {
                List<Object> objects = new List<Object>() { value };
                layerObjDict.Add(layer, objects);
            }
        }

        private void CheckAndSetLayer(List<Object> objects, int layer, int centerLayer)
        {
            for (int i = 0; i < objects.Count; i++)
            {
                var item = objects[i];
                if (item is SpriteRenderer)
                {
                    (item as SpriteRenderer).sortingOrder = Mathf.Clamp(layer, centerLayer - 4, centerLayer + 5);
                }
                else if (item is Renderer)
                {
                    (item as Renderer).sortingOrder = Mathf.Clamp(layer, centerLayer - 4, centerLayer + 5);
                    ;
                }
                else if (item is Canvas)
                {
                    (item as Canvas).sortingOrder = Mathf.Clamp(layer, centerLayer - 4, centerLayer + 5);
                    ;
                }
            }
        }

        private void Clear()
        {
            MaxLayer = "10";
            MinLayer = "-1";
            goDict.Clear();
        }

        private void OnDestroy()
        {
            Clear();
        }

        private void LayerLog(string msg, EState state = EState.Normal)
        {
            Debug.LogFormat(stateDict[(int)state], msg);
            if (state == EState.Error) Clear();
        }
    }
}

#endif