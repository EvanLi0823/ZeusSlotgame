using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonMobileDeviceAdapter
{
    public static void AdapterCamera(Camera cam)
    {
//        switch (SkySreenUtils.CurrentOrientation)
//        {
//            case ScreenOrientation.Landscape:
//                CommonMobileDeviceLandscapeAdapter.AdapterCamera(cam);
//                break;
//            case ScreenOrientation.Portrait:
//                CommonMobileDevicePortraitAdapter.AdapterCamera(cam);
//                break;
//            default:
//                break;
//        }
    }

    /// <summary>
    /// Determines if is wide screen.
    /// 比16:9的屏幕要窄的屏幕
    /// </summary>
    /// <returns><c>true</c> if is wide screen; otherwise, <c>false</c>.</returns>
    public static bool IsWideScreen()
    {
        return CommonMobileDeviceLandscapeAdapter.IsWideScreen();
    }
    /// <summary>
    /// Determines if is square screen.
    /// 比4:3的屏幕要宽的屏幕，向方屏幕接近
    /// </summary>
    /// <returns><c>true</c> if is square screen; otherwise, <c>false</c>.</returns>
    public static bool IsSquareScreen()
    {
        return CommonMobileDeviceLandscapeAdapter.IsSquareScreen();
    }

    public static int GetViewWidth()
    {
        return CommonMobileDeviceLandscapeAdapter.GetViewWidth();
    }

    public static int GetViewHeight()
    {
        return CommonMobileDeviceLandscapeAdapter.GetViewHeight();
    }

    public static int GetViewTopMargin()
    {
        return CommonMobileDeviceLandscapeAdapter.GetViewTopMargin();
    }
    public static int GetViewBottomMargin()
    {
        return CommonMobileDeviceLandscapeAdapter.GetViewBottomMargin();
    }
    public static int GetViewLeftMargin()
    {
        return CommonMobileDeviceLandscapeAdapter.GetViewLeftMargin();
    }
    public static int GetViewRightMargin()
    {
        return CommonMobileDeviceLandscapeAdapter.GetViewRightMargin();
    }
}
