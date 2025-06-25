using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonMobileDeviceLandscapeAdapter {

	private const float WIDTH_HEIGHT_RATIO_16_9 = 0.5625f;
	private const float WIDTH_HEIGHT_RATIO_4_3 = 0.75f;

	public static void AdapterCamera(Camera cam){
//        if (SkySreenUtils.CurrentOrientation != ScreenOrientation.Landscape) return;
//        if (cam.name.StartsWith("NoController")) return;
//        if (IphoneXAdapter.IsIphoneX()) {
//			return;
//		}
//		if (IsWideScreen()) {
//			int width = (int)(SkySreenUtils.DEVICE_HEIGHT/WIDTH_HEIGHT_RATIO_16_9);
//			int height = SkySreenUtils.DEVICE_HEIGHT;
//			int x = (SkySreenUtils.DEVICE_WIDTH - width)/ 2;
//			int y = 0;
//			cam.rect = new Rect ((float)x/SkySreenUtils.DEVICE_WIDTH,(float)y/SkySreenUtils.DEVICE_HEIGHT,(float)width/SkySreenUtils.DEVICE_WIDTH,(float)height/SkySreenUtils.DEVICE_HEIGHT);
//		}
//		else if (IsSquareScreen()) {
//			int width = SkySreenUtils.DEVICE_WIDTH;
//			int height = (int)(SkySreenUtils.DEVICE_WIDTH*WIDTH_HEIGHT_RATIO_4_3);
//			int x = 0;
//			int y = (SkySreenUtils.DEVICE_HEIGHT-height)/2;
//			cam.rect = new Rect ((float)x/SkySreenUtils.DEVICE_WIDTH,(float)y/SkySreenUtils.DEVICE_HEIGHT,(float)width/SkySreenUtils.DEVICE_WIDTH,(float)height/SkySreenUtils.DEVICE_HEIGHT);
//		}

	}

	/// <summary>
	/// Determines if is wide screen.
	/// 比16:9的屏幕要窄的屏幕
	/// </summary>
	/// <returns><c>true</c> if is wide screen; otherwise, <c>false</c>.</returns>
	public static bool IsWideScreen(){ 
		return ((float)SkySreenUtils.DEVICE_HEIGHT/SkySreenUtils.DEVICE_WIDTH) < WIDTH_HEIGHT_RATIO_16_9;
	}
	/// <summary>
	/// Determines if is square screen.
	/// 比4:3的屏幕要宽的屏幕，向方屏幕接近
	/// </summary>
	/// <returns><c>true</c> if is square screen; otherwise, <c>false</c>.</returns>
	public static bool IsSquareScreen(){
		return ((float)SkySreenUtils.DEVICE_HEIGHT / SkySreenUtils.DEVICE_WIDTH) > WIDTH_HEIGHT_RATIO_4_3;
	}

	public static int GetViewWidth(){ 
		if (IsWideScreen()) {
			return (int)(SkySreenUtils.DEVICE_HEIGHT/WIDTH_HEIGHT_RATIO_16_9);
		}
		else if (IsSquareScreen()) {
			return SkySreenUtils.DEVICE_WIDTH;
		}
		return SkySreenUtils.DEVICE_WIDTH;
	}

	public static int GetViewHeight(){
		if (IsWideScreen()) {
			return SkySreenUtils.DEVICE_HEIGHT;
		}
		else if (IsSquareScreen()) {
			return (int)(SkySreenUtils.DEVICE_WIDTH*WIDTH_HEIGHT_RATIO_4_3);
		} 
		return SkySreenUtils.DEVICE_HEIGHT;
	}

	public static int GetViewTopMargin(){
		if (IsWideScreen()) {
			return 0;
		}
		else if (IsSquareScreen()) {
			int height = (int)(SkySreenUtils.DEVICE_WIDTH*WIDTH_HEIGHT_RATIO_4_3);
			return (SkySreenUtils.DEVICE_HEIGHT-height)/2;
		}
		return 0;
	}
	public static int GetViewBottomMargin(){
		if (IsWideScreen()) {
			return 0;
		}
		else if (IsSquareScreen()) {
			int height = (int)(SkySreenUtils.DEVICE_WIDTH*WIDTH_HEIGHT_RATIO_4_3);
			return (SkySreenUtils.DEVICE_HEIGHT-height)/2;
		}
		return 0;
	}
	public static int GetViewLeftMargin(){
		if (IsWideScreen()) {
			int width = (int)(SkySreenUtils.DEVICE_HEIGHT/WIDTH_HEIGHT_RATIO_16_9);
			return (SkySreenUtils.DEVICE_WIDTH - width)/ 2;
		}
		else if (IsSquareScreen()) {
			return 0;
		}
		return 0;
	}
	public static int GetViewRightMargin(){
		if (IsWideScreen()) {
			int width = (int)(SkySreenUtils.DEVICE_HEIGHT/WIDTH_HEIGHT_RATIO_16_9);
			return (SkySreenUtils.DEVICE_WIDTH - width)/ 2;
		}
		else if (IsSquareScreen()) {
			return 0;
		}
		return 0;
	}
}
