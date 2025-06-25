using System;
using UnityEditor;
using UnityEngine;

namespace Libs
{
    public class FindPrefabByGUIDEditor : EditorWindow
    {
        private string targetOrder;
        private string info;
        [MenuItem("Tools/根据Guid查找路径")]
        static void ShowFindPrefabByGUIDWindow()
        {
            FindPrefabByGUIDEditor editor =
                (FindPrefabByGUIDEditor)GetWindowWithRect(typeof(FindPrefabByGUIDEditor),
                    new Rect(100, 100, 600, 200), true, "根据Guid查找路径");
            editor.autoRepaintOnSceneChange = true;
            editor.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("请输入Guid:");
            Rect rect = EditorGUILayout.GetControlRect(GUILayout.Width(300));
            targetOrder = EditorGUI.TextField(rect, targetOrder);
            info = "";
            if (GUILayout.Button("开始查找", GUILayout.Width(100)))
            {
                info = LoadAssetByGUID(targetOrder);
                Debug.Log("开始查找信息"+info);
                if (!string.IsNullOrEmpty(info))
                {
                    UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(info);
                    EditorGUIUtility.PingObject(asset);
                }
            }
            EditorGUILayout.EndVertical();
        }
        public static string LoadAssetByGUID(string guid)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            return assetPath;
        }
        void OnInspectorUpdate()
        {
            //开启窗口的重绘，不然窗口信息不会刷新
            Repaint();
        }
    }
}