﻿using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
/// <summary>
/// Game view utils.
/// 1.先查找当前GameView的分辨率
/// 2.反转宽高后，进行查找是否包含指定分辨率设置：有的话，直接设置；没有的话，进行添加，再设置
/// </summary>
public static class GameViewUtils
{
    static object gameViewSizesInstance;
    static MethodInfo getGroup;

    static GameViewUtils()
    {
        // gameViewSizesInstance  = ScriptableSingleton<GameViewSizes>.instance;
        var sizesType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizes");
        var singleType = typeof(ScriptableSingleton<>).MakeGenericType(sizesType);
        var instanceProp = singleType.GetProperty("instance");
        getGroup = sizesType.GetMethod("GetGroup");
        gameViewSizesInstance = instanceProp.GetValue(null, null);
    }

    public enum GameViewSizeType
    {
        AspectRatio, FixedResolution
    }

    public static void AdjustGameViewResolution()
    {
        GameViewSizeGroupType currentGroupType = GameViewSizeGroupType.Standalone;
#if UNITY_IOS
        currentGroupType = GameViewSizeGroupType.iOS;
#elif UNITY_ANDROID
         currentGroupType = GameViewSizeGroupType.Android;
#endif
        if (SkySreenUtils.CurrentOrientation == ScreenOrientation.Portrait)
        {
            if (Screen.width > Screen.height)
            {
                //Toggle to Portrait
                SetGameViewResolution(currentGroupType, SkySreenUtils.DEVICE_HEIGHT, SkySreenUtils.DEVICE_WIDTH);
            }
           
        }
        else if (SkySreenUtils.CurrentOrientation == ScreenOrientation.LandscapeLeft)
        {
            if (Screen.width<Screen.height)
            {
                //Toggle to Landscape
                SetGameViewResolution(currentGroupType, SkySreenUtils.DEVICE_WIDTH, SkySreenUtils.DEVICE_HEIGHT);
            }
        }
    }

    private static void SetGameViewResolution(GameViewSizeGroupType groupType,int width,int height)
    {
        int id = FindSize(groupType, width, height);
        if (id == -1)
        {
            string name = width + "x" + height;
            AddCustomSize(GameViewSizeType.FixedResolution, groupType, width,height, name);
            id = FindSize(groupType, width, height);
            if (id != -1)
            {
                SetSize(id);
            }
        }
        else
        {
            SetSize(id);
        }

    }
    //public static void AddTestSize()
    //{
    //    AddCustomSize(GameViewSizeType.AspectRatio, GameViewSizeGroupType.Standalone, 123, 456, "Test size");
    //}

    //public static void SizeTextQueryTest()
    //{
    //    Debug.Log(SizeExists(GameViewSizeGroupType.Standalone, "Test size"));
    //}

    //public static void WidescreenQueryTest()
    //{
    //    Debug.Log(SizeExists(GameViewSizeGroupType.Standalone, "16:9"));
    //}

    //public static void SetWidescreenTest()
    //{
    //    SetSize(FindSize(GameViewSizeGroupType.Standalone, "16:9"));
    //}

    //public static void SetTestSize()
    //{
    //    int idx = FindSize(GameViewSizeGroupType.Standalone, 123, 456);
    //    if (idx != -1)
    //        SetSize(idx);
    //}

    public static void SetSize(int index)
    {
        var gvWndType = typeof(Editor).Assembly.GetType("UnityEditor.GameView");
        var selectedSizeIndexProp = gvWndType.GetProperty("selectedSizeIndex",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        var gvWnd = EditorWindow.GetWindow(gvWndType);
        selectedSizeIndexProp.SetValue(gvWnd, index, null);
    }

    //public static void SizeDimensionsQueryTest()
    //{
    //    Debug.Log(SizeExists(GameViewSizeGroupType.Standalone, 123, 456));
    //}

    public static void AddCustomSize(GameViewSizeType viewSizeType, GameViewSizeGroupType sizeGroupType, int width, int height, string text)
    {
        // GameViewSizes group = gameViewSizesInstance.GetGroup(sizeGroupTyge);
        // group.AddCustomSize(new GameViewSize(viewSizeType, width, height, text);

        var group = GetGroup(sizeGroupType);
        var addCustomSize = getGroup.ReturnType.GetMethod("AddCustomSize"); // or group.GetType().
        var gvsType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSize");
        var ctor = gvsType.GetConstructor(new Type[] { typeof(int), typeof(int), typeof(int), typeof(string) });
        var newSize = ctor.Invoke(new object[] { (int)viewSizeType, width, height, text });
        addCustomSize.Invoke(group, new object[] { newSize });
    }

    public static bool SizeExists(GameViewSizeGroupType sizeGroupType, string text)
    {
        return FindSize(sizeGroupType, text) != -1;
    }

    public static int FindSize(GameViewSizeGroupType sizeGroupType, string text)
    {
        // GameViewSizes group = gameViewSizesInstance.GetGroup(sizeGroupType);
        // string[] texts = group.GetDisplayTexts();
        // for loop...

        var group = GetGroup(sizeGroupType);
        var getDisplayTexts = group.GetType().GetMethod("GetDisplayTexts");
        var displayTexts = getDisplayTexts.Invoke(group, null) as string[];
        for (int i = 0; i < displayTexts.Length; i++)
        {
            string display = displayTexts[i];
            // the text we get is "Name (W:H)" if the size has a name, or just "W:H" e.g. 16:9
            // so if we're querying a custom size text we substring to only get the name
            // You could see the outputs by just logging
            // Debug.Log(display);
            int pren = display.IndexOf('(');
            if (pren != -1)
                display = display.Substring(0, pren - 1); // -1 to remove the space that's before the prens. This is very implementation-depdenent
            if (display == text)
                return i;
        }
        return -1;
    }

    public static bool SizeExists(GameViewSizeGroupType sizeGroupType, int width, int height)
    {
        return FindSize(sizeGroupType, width, height) != -1;
    }

    public static int FindSize(GameViewSizeGroupType sizeGroupType, int width, int height)
    {
        // goal:
        // GameViewSizes group = gameViewSizesInstance.GetGroup(sizeGroupType);
        // int sizesCount = group.GetBuiltinCount() + group.GetCustomCount();
        // iterate through the sizes via group.GetGameViewSize(int index)

        var group = GetGroup(sizeGroupType);
        var groupType = group.GetType();
        var getBuiltinCount = groupType.GetMethod("GetBuiltinCount");
        var getCustomCount = groupType.GetMethod("GetCustomCount");
        int sizesCount = (int)getBuiltinCount.Invoke(group, null) + (int)getCustomCount.Invoke(group, null);
        var getGameViewSize = groupType.GetMethod("GetGameViewSize");
        var gvsType = getGameViewSize.ReturnType;
        var widthProp = gvsType.GetProperty("width");
        var heightProp = gvsType.GetProperty("height");
        var indexValue = new object[1];
        for (int i = 0; i < sizesCount; i++)
        {
            indexValue[0] = i;
            var size = getGameViewSize.Invoke(group, indexValue);
            int sizeWidth = (int)widthProp.GetValue(size, null);
            int sizeHeight = (int)heightProp.GetValue(size, null);
            if (sizeWidth == width && sizeHeight == height)
                return i;
        }
        return -1;
    }

    static object GetGroup(GameViewSizeGroupType type)
    {
        return getGroup.Invoke(gameViewSizesInstance, new object[] { (int)type });
    }

    //public static void LogCurrentGroupType()
    //{
    //    Debug.Log(GetCurrentGroupType());
    //}

    public static GameViewSizeGroupType GetCurrentGroupType()
    {
        var getCurrentGroupTypeProp = gameViewSizesInstance.GetType().GetProperty("currentGroupType");
        return (GameViewSizeGroupType)(int)getCurrentGroupTypeProp.GetValue(gameViewSizesInstance, null);
    }
}
#endif