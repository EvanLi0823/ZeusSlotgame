using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * 参考连接:http://www.cocoachina.com/ios/20171011/20737.html
 if ([deviceString isEqualToString:@"iPhone10,1"])   return @"国行(A1863)、日行(A1906)iPhone 8";
if ([deviceString isEqualToString:@"iPhone10,4"])   return @"美版(Global/A1905)iPhone 8";
if ([deviceString isEqualToString:@"iPhone10,2"])   return @"国行(A1864)、日行(A1898)iPhone 8 Plus";
if ([deviceString isEqualToString:@"iPhone10,5"])   return @"美版(Global/A1897)iPhone 8 Plus";
if ([deviceString isEqualToString:@"iPhone10,3"])   return @"国行(A1865)、日行(A1902)iPhone X";
if ([deviceString isEqualToString:@"iPhone10,6"])   return @"美版(Global/A1901)iPhone X";
更多新设备信息详见Github-iOS-getClientInfo
iPhone X 中文官方适配文档
链接：http://www.jianshu.com/p/f5ee206c7df0
搜索CocoaChina微信公众号：CocoaChina 微信扫一扫
订阅每日移动开发及APP推广热点资讯
公众号：CocoaChina

*/
public class IphoneXAdapter {
  
    public static readonly int Width = 1878;
    public static readonly int Height = 1056;
    public static readonly int Left = 279;
    public static readonly int Top = 11;
    public static readonly int Right = 279;
    public static readonly int Bottom = 58;
    public static void AdapterCamera(Camera cam){
        
        if (IsIphoneX())
        {
            if (cam == null)
            {
                return;
            }
			cam.rect = new Rect((float)Left/SkySreenUtils.DEVICE_WIDTH, (float)Bottom/SkySreenUtils.DEVICE_HEIGHT, (float)Width/SkySreenUtils.DEVICE_WIDTH, (float)Height/SkySreenUtils.DEVICE_HEIGHT);
        }
    }
    public static bool IsIphoneX(){
		
        string deviceType = SystemInfo.deviceModel;
		#if UNITY_EDITOR
		return (SkySreenUtils.DEVICE_WIDTH ==2436&&SkySreenUtils.DEVICE_HEIGHT ==1125)||(SkySreenUtils.DEVICE_WIDTH==1792&&SkySreenUtils.DEVICE_HEIGHT==828)||(SkySreenUtils.DEVICE_WIDTH==2688&&SkySreenUtils.DEVICE_HEIGHT==1242)||
            (SkySreenUtils.DEVICE_WIDTH == 1125 && SkySreenUtils.DEVICE_HEIGHT == 2436) || (SkySreenUtils.DEVICE_WIDTH == 828 && SkySreenUtils.DEVICE_HEIGHT == 1792) || (SkySreenUtils.DEVICE_WIDTH == 1242 && SkySreenUtils.DEVICE_HEIGHT ==2688 );
		#elif PLATFORM_GOLDS //gold的x系列都按照iphoneX来算，classic的先不用改
		//参考网址：https://gist.github.com/adamawolf/3048717
	    return (deviceType == "iPhone10,3" || deviceType == "iPhone10,6")
	           || deviceType == "iPhone11,8" //XR
	           || deviceType == "iPhone11,2" //XS
	           || deviceType == "iPhone11,4" //XS Max
	           || deviceType == "iPhone11,6" //XS Max
	           || deviceType == "iPhone12,1" //iPhone 11
	           || deviceType == "iPhone12,3" //iPhone 11 Pro
	           || deviceType == "iPhone12,5" //iPhone 11 Pro Max
	           || deviceType == "iPhone13,1" // iPhone 12 Mini
	           || deviceType == "iPhone13,2" // iPhone 12
	           || deviceType == "iPhone13,3" // iPhone 12 Pro
	           || deviceType == "iPhone13,4" // iPhone 12 Pro Max
	           || deviceType == "iPhone14,2" //iPhone 13 Pro
	           || deviceType == "iPhone14,3" //iPhone 13 Pro Max
	           || deviceType == "iPhone14,4" //iPhone 13 Mini
	           || deviceType == "iPhone14,5"; //iPhone 13
#else
		return deviceType == "iPhone10,3" || deviceType == "iPhone10,6";
#endif

//		iPhoneX: “iPhone10,3”, “iPhone10,6”
//		iPhoneXR: “iPhone11,8”
//		iPhoneXS: “iPhone11,2”
//		iPhoneXS Max: “iPhone11,6”
    }
}
