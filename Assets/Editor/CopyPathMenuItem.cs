﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class CopyPathMenuItem
{
    [MenuItem("GameObject/Copy Path", false, 11)]
    static void CopyPath()
    {
        GameObject currentGameObject = Selection.activeGameObject;

        if (currentGameObject == null)
            return;

        string path = currentGameObject.name;

        while (currentGameObject.transform.parent != null)
        {
            currentGameObject = currentGameObject.transform.parent.gameObject;

            path = $"{currentGameObject.name}/{path}";
        }

        EditorGUIUtility.systemCopyBuffer = path;
    }

    /// <summary>
    /// Only allow path copying if 1 object is selected.
    /// </summary>
    [MenuItem("GameObject/Copy Path", true)]
    static bool CopyPathValidation() => Selection.gameObjects.Length == 1;
}
