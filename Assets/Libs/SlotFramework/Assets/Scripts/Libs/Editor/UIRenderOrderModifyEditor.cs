using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System.IO;
using Classic;

namespace Libs
{
    public class UIRenderOrderModifyEditor : EditorWindow
    {
        private static string assetPath;
        private static string modifyNodePath;
        private string targetOrder;
        private static bool isPrefabAsset = true;
        private static bool isPrefabMode = false;
        Regex pattern = new Regex(@"^\d{3,5}$");

        [MenuItem("Assets/UI渲染层级修改工具", false, 10)]
        static void ShowUIRenderOrderModifyWindow()
        {
            assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (string.IsNullOrEmpty(assetPath))
            {
                string clickedAssetGuid = Selection.assetGUIDs[0];
                string clickedPath = AssetDatabase.GUIDToAssetPath(clickedAssetGuid);
                assetPath = clickedPath;
            }

            UIRenderOrderModifyEditor editor =
                (UIRenderOrderModifyEditor)GetWindowWithRect(typeof(UIRenderOrderModifyEditor),
                    new Rect(100, 100, 600, 200), true, "UI渲染层级修改工具");
            editor.Show();
            isPrefabAsset = true;
        }

        [MenuItem("GameObject/UI渲染层级修改工具", false, 11)]
        static void ModifyGameObjectRenderOrder(MenuCommand menuCommand)
        {
            //EditorApplication.ExecuteMenuItem ("GameObject/UI渲染层级修改工具");
            assetPath = Util.GetGameObjectPath(Selection.activeGameObject.transform);
            if (assetPath.StartsWith("Canvas (Environment)/"))
            {
                modifyNodePath = assetPath.Replace("Canvas (Environment)/", "");
                assetPath = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage()
                    .prefabAssetPath;
                isPrefabMode = true;
            }

            UIRenderOrderModifyEditor editor =
                (UIRenderOrderModifyEditor)GetWindowWithRect(typeof(UIRenderOrderModifyEditor),
                    new Rect(100, 100, 600, 200), true, "UI渲染层级修改工具");
            editor.Show();
            isPrefabAsset = false;
        }

        void OnGUI()
        {
            if (isPrefabAsset)
            {
                DrawPrefabUI();
            }
            else
            {
                DrawGameObjectUI();
            }
        }

        void DrawGameObjectUI()
        {
            EditorGUILayout.BeginVertical();
            if (isPrefabMode)
            {
                EditorGUILayout.LabelField("修复当前Prefab的路径:" + assetPath);
                EditorGUILayout.LabelField("修复当前Prefab内GameObject所在的节点位置:" + modifyNodePath);
            }
            else
            {
                EditorGUILayout.LabelField("修复场景内GameObject所在的Path:" + assetPath);
            }

            Rect rect = EditorGUILayout.GetControlRect(GUILayout.Width(300));
            //isPrefabMode = EditorGUI.Toggle(rect,"是否处于Prefab Mode模式下",isPrefabMode);
            EditorGUILayout.LabelField("目标根层级指定(生效位数为:X00)");
            //rect = EditorGUILayout.GetControlRect (GUILayout.Width (300));

            targetOrder = EditorGUI.TextField(rect, targetOrder);
            EditorGUILayout.Space();

            if (GUILayout.Button("修改", GUILayout.Width(100)))
            {
                CheckModfiyGameObjectUIRenderOrder();
            }

            EditorGUILayout.EndVertical();
        }

        void DrawPrefabUI()
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Prefab资源路径:" + assetPath);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("目标根层级指定(生效位数为:X00)");
            Rect rect = EditorGUILayout.GetControlRect(GUILayout.Width(300));
            targetOrder = EditorGUI.TextField(rect, targetOrder);
            EditorGUILayout.Space();

            if (GUILayout.Button("修改", GUILayout.Width(100)))
            {
                CheckModfiyPrefabUIRenderOrder();
            }

            EditorGUILayout.EndVertical();
        }

        void CheckModfiyGameObjectUIRenderOrder()
        {
            if (isPrefabMode)
            {
                bool changed =
                    RebuildAssetComRenderOrder(assetPath, Convert.ToInt16(targetOrder), true, modifyNodePath);
                if (changed)
                {
                    ShowInfo("修复成功！");
                }
                else
                {
                    ShowInfo("资源满足需求，不需要修复！");
                }
            }
            else
            {
                GameObject go = Selection.activeGameObject;
                bool changed = ScanObject(go, Convert.ToInt16(targetOrder));
                if (changed)
                {
                    //Unity Scene 模式下
                    UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();
                    AssetDatabase.Refresh();
                    ShowInfo("修复成功！");
                }
                else
                {
                    ShowInfo("资源满足需求，不需要修复！");
                }
            }
        }

        void CheckModfiyPrefabUIRenderOrder()
        {
            if (string.IsNullOrEmpty(assetPath) ||
                (!File.Exists(Path.Combine(Environment.CurrentDirectory, assetPath)) &&
                 !Directory.Exists(Path.Combine(Environment.CurrentDirectory, assetPath))))
            {
                ShowInfo("资源路径不对");
                return;
            }

            if (!assetPath.EndsWith(".prefab") &&
                !Directory.Exists(Path.Combine(Environment.CurrentDirectory, assetPath)))
            {
                ShowInfo("资源类型不对，必须为prefab或者prefab目录");
                return;
            }

            if (string.IsNullOrEmpty(targetOrder) || !pattern.IsMatch(targetOrder) ||
                Int16.MinValue > Convert.ToInt32(targetOrder) || Convert.ToInt32(targetOrder) > Int16.MaxValue)
            {
                ShowInfo("目标层级不对");
                return;
            }

            bool changed = false;
            if (Directory.Exists(Path.Combine(Environment.CurrentDirectory, assetPath)))
            {
                changed = RebuildAssetComRenderOrderInSpecialFolder(assetPath, Convert.ToInt16(targetOrder));
            }
            else
            {
                //加载指定路径的prefab
                changed = ModifyPrefab();
            }

            if (changed)
            {
                ShowInfo("修复成功！");
            }
            else
            {
                ShowInfo("资源满足需求，不需要修复！");
            }
        }

        void ShowInfo(string errorMsg)
        {
            EditorUtility.DisplayDialog("修复状态", errorMsg, "Ok");
        }

        bool ModifyPrefab()
        {
            return RebuildAssetComRenderOrder(assetPath, Convert.ToInt16(targetOrder));
        }

        bool RebuildAssetComRenderOrderInSpecialFolder(string target, int sortingOrderPrefix)
        {
            Debug.Log("======================= ReBUILD =====================");
            string[] files = Directory.GetFiles(Path.Combine(Environment.CurrentDirectory, target), "*.prefab",
                SearchOption.AllDirectories);
            bool changed = false;
            for (int i = 0; i < files.Length; i++)
            {
                bool ret = RebuildAssetComRenderOrder(files[i].Replace(Environment.CurrentDirectory + "/", ""),
                    sortingOrderPrefix, false);
                if (!changed && ret) changed = true;
            }

            AssetDatabase.Refresh();
            return changed;
        }

        bool RebuildAssetComRenderOrder(string target, int sortingOrderPrefix, bool refresh = true,
            string repairNodePath = "")
        {
            Debug.Log("======================= ReBUILD =====================");
            Debug.Log("file=" + target);
            bool changed = false;
            GameObject gobj = PrefabUtility.LoadPrefabContents(target);
            if (gobj == null)
                Debug.LogError("load prefab failed: " + target);
            else
            {
                GameObject modifyObj = gobj;
                if (!string.IsNullOrEmpty(repairNodePath))
                {
                    repairNodePath = repairNodePath.Substring(repairNodePath.IndexOf('/') + 1);
                    modifyObj = Util.FindObjectEx<GameObject>(gobj.transform, repairNodePath);
                }

                changed = ScanObject(modifyObj, sortingOrderPrefix);
                if (changed)
                {
                    PrefabUtility.SaveAsPrefabAsset(gobj, target);
                    Debug.Log("SAVE: " + target);
                }

                DestroyImmediate(gobj);
            }

            if (refresh) AssetDatabase.Refresh();
            return changed;
        }

        /// <summary>
        /// 可以扫描Prefab也可以扫描场景对象节点
        /// </summary>
        /// <param name="gobj"></param>
        /// <param name="orderPreFix"></param>
        /// <returns></returns>
        bool ScanObject(GameObject gobj, int orderPreFix)
        {
            bool changed = false;

            Canvas canvas = gobj.GetComponent<Canvas>();
            if (canvas != null)
            {
                //if (canvas.overrideSorting)
                {
                    int initOrder, rawOrder;
                    initOrder = rawOrder = canvas.sortingOrder;
                    rawOrder %= 100;
                    orderPreFix /= 100;
                    orderPreFix *= 100;
                    canvas.sortingOrder = rawOrder + orderPreFix;
                    if (canvas.sortingOrder != initOrder)
                    {
                        changed = true;
                    }
                }
            }

            SpriteRenderer spriteRenderer = gobj.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                int initOrder, rawOrder;
                initOrder = rawOrder = spriteRenderer.sortingOrder;
                rawOrder %= 100;
                orderPreFix /= 100;
                orderPreFix *= 100;
                spriteRenderer.sortingOrder = rawOrder + orderPreFix;
                if (spriteRenderer.sortingOrder != initOrder)
                {
                    changed = true;
                }
            }

            ParticleSystem particleSystem = gobj.GetComponent<ParticleSystem>();
            if (particleSystem != null)
            {
                Renderer render = particleSystem.GetComponent<Renderer>();
                if (render != null && render.enabled)
                {
                    int initOrder, rawOrder;
                    initOrder = rawOrder = render.sortingOrder;
                    rawOrder %= 100;
                    orderPreFix /= 100;
                    orderPreFix *= 100;
                    render.sortingOrder = rawOrder + orderPreFix;
                    if (render.sortingOrder != initOrder)
                    {
                        changed = true;
                    }
                }
            }

            for (int i = 0; i < gobj.transform.childCount; i++)
            {
                if (ScanObject(gobj.transform.GetChild(i).gameObject, orderPreFix))
                    changed = true;
            }

            return changed;
        }
    }
}