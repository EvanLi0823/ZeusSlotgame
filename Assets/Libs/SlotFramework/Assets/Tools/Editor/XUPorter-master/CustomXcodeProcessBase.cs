using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.XCodeEditor;
using System.IO;
using System.Collections;
using System.Xml;
using System.Text;
#endif
public class CustomXcodeProcessBase  {
	protected XCProject  project;
	protected string pathToBuiltXcodeProject;
	public static CustomXcodeProcessBase _instance;
	public static CustomXcodeProcessBase Instance{
		get{
			if(_instance == null)
			{
				_instance = new CustomXcodeProcessBase();
			}
			return _instance;
		}
	}

	public static void SetXcodeProgressClass<T>() where T: CustomXcodeProcessBase
	{
		CustomXcodeProcessBase._instance = Activator.CreateInstance<T> ();
	}

	public virtual void DealXcodeFile(XCProject _xcProject,string _pathToBuiltXcodeProject)
	{
		this.project = _xcProject;
		this.pathToBuiltXcodeProject = _pathToBuiltXcodeProject;

		this.SetXcodeIdentity ();
		this.ModifyXcodeFile ();
		this.ReplaceFiles();
		this.EditorImage ();
		this.ModifyInfoPlist ();

//		this.ModifyYamlFile();
	}

	protected virtual void SetXcodeIdentity()
	{
//		project.overwriteBuildSetting ("CODE_SIGN_IDENTITY", "iPhone Developer: Yangqing Huang (XF37LMDD9P)", "Debug");

//		project.overwriteBuildSetting("PROVISIONING_PROFILE","AppTemplate Development Profile","Debug");
		//		project.overwriteBuildSetting ("CODE_SIGN_IDENTITY", "iPhone Developer: Yangqing Huang (XF37LMDD9P)", "Release");
	}


	protected virtual void EditorImage()
	{
		string appIconContents = pathToBuiltXcodeProject + "/Unity-iPhone/Images.xcassets/AppIcon.appiconset/Contents.json";
		StreamReader reader = new StreamReader (appIconContents);
		Hashtable json = MiniJSON.Json.Deserialize (reader.ReadToEnd ()) as Hashtable;
		reader.Close ();
		if (json != null) {
			if (!json.ContainsKey ("properties")) {
				json ["properties"] = new Hashtable ();
			}
			Hashtable props = (Hashtable)json ["properties"];
			props ["pre-rendered"] = true;
			StreamWriter writer = new StreamWriter (appIconContents);
			writer.Write ( MiniJSON.Json.Serialize (json));
			writer.Close ();
		}
	}

	protected virtual void ReplaceFiles()
	{
		//modify Constants
		XClass UnityIOSConstants = new XClass(Application.dataPath+"/Platforms/IOS/Plugins/iOS/Constants.m");
		UnityIOSConstants.ReplaceWithRegularExpresssion("const NSString \\* global_bundleVersion = @\".*\";","const NSString * global_bundleVersion = @\""+ PlayerSettings.bundleVersion +"\";");
		//		UnityIOSConstants.ReplaceWithRegularExpresssion("const NSString \\* global_bundleVersion = @\"0\";","const NSString * global_bundleVersion = @\""+ PlayerSettings.iOS.buildNumber +"\";");

//		UnityIOSConstants.Replace("const NSString * global_bundleVersion = @\"0\";","const NSString * global_bundleVersion = @\""+ PlayerSettings.bundleVersion +"\";");

		/*XClass AppsFlyerIOSController = new XClass(Application.dataPath+"/AppsFlyer/Plugins/iOS/AppsFlyerAppController.mm");
		AppsFlyerIOSController.Replace("-(void)didBecomeActive:(NSNotification*)notification {","/*-(void)didBecomeActive:(NSNotification*)notification {");
		AppsFlyerIOSController.Replace("didEnteredBackGround = NO;\n}\n}","didEnteredBackGround = NO;\n}\n}#1#");
		AppsFlyerIOSController.Replace("- (void)didEnterBackground:(NSNotification*)notification {","/*- (void)didEnterBackground:(NSNotification*)notification {");
		AppsFlyerIOSController.Replace("didEnteredBackGround = YES;\n}","didEnteredBackGround = YES;\n}#1#");
		AppsFlyerIOSController.Replace("- (void)onOpenURL:(NSNotification*)notification {","/*- (void)onOpenURL:(NSNotification*)notification {");
		AppsFlyerIOSController.Replace("[[AppsFlyerLib shared] handleOpenURL:url sourceApplication:sourceApplication withAnnotation:nil];\n}\n\n}","[[AppsFlyerLib shared] handleOpenURL:url sourceApplication:sourceApplication withAnnotation:nil];\n}\n\n}#1#");
		AppsFlyerIOSController.Replace("- (void)didReceiveRemoteNotification:(NSNotification*)notification {","/*- (void)didReceiveRemoteNotification:(NSNotification*)notification {");
		AppsFlyerIOSController.Replace("[[AppsFlyerLib shared] handlePushNotification:notification.userInfo];\n}","[[AppsFlyerLib shared] handlePushNotification:notification.userInfo];\n}#1#");
		AppsFlyerIOSController.Replace("IMPL_APP_CONTROLLER_SUBCLASS(AppsFlyerAppController)","/*IMPL_APP_CONTROLLER_SUBCLASS(AppsFlyerAppController)#1#");*/
	}


	protected virtual void ModifyXcodeFile()
	{
		//prefixPath
		XClass prefixPach = new XClass (pathToBuiltXcodeProject + "/Classes/Prefix.pch");
		if (prefixPach.isContainsText ("#import <HSAppFramework/HSAppFramework.h>")) {
			//has added ,dont do next
			return;
		}

		prefixPach.WriteBelow ("#import <UIKit/UIKit.h>\n", "	#import <HSAppFramework/HSAppFramework.h>\n");
		if(Debug.isDebugBuild)
		{
			prefixPach.WriteBelow("#endif\n","#define DEBUG 1");
		}
		//controller.h
		XClass UnityAppControllerH = new XClass (pathToBuiltXcodeProject + "/Classes/UnityAppController.h");
		UnityAppControllerH.Replace (" NSObject<UIApplicationDelegate>", "BFApplication");
		UnityAppControllerH.WriteBelow("@property (retain, nonatomic) UIWindow* window;", 
            "\n@property (assign, nonatomic) BOOL rtotMgrGetTaskConnectionDidFinish;\n");

#if UNITY_2019_3_OR_NEWER
		UnityAppControllerH.WriteBelow("#include \"RenderPluginDelegate.h\"", "#import <HSAppFramework/BFApplication.h>");

		//main.mm
		XClass mainMM = new XClass(pathToBuiltXcodeProject + "/MainApp/main.mm");
		mainMM.Replace("#include <UnityFramework/UnityFramework.h>", "#include \"UnityFramework.h\"");
#endif

		//appController.m
		XClass UnityAppController = new XClass(pathToBuiltXcodeProject + "/Classes/UnityAppController.mm");
		UnityAppController.WriteBelow ("#include <mach/mach_time.h>\n", "#import <FBSDKCoreKit/FBSDKCorekit.h>\n#include \"NativeAPI.h\"\n#import <HSAppFramework/BFRtotMgr.h>\n#import \"Firebase.h\"\n");
		UnityAppController.WriteBelow ("didReceiveLocalNotification:(UILocalNotification*)notification\n{\n"," [super application:application didReceiveLocalNotification:notification];\n");
		UnityAppController.WriteBelow ("didReceiveRemoteNotification:(NSDictionary*)userInfo\n{\n","    [super application:application didReceiveRemoteNotification:userInfo];\n");
		UnityAppController.WriteBelow ("UnitySendRemoteNotification(userInfo);\n}\n",getNewFunctionCode());
		UnityAppController.WriteBelow ("didRegisterForRemoteNotificationsWithDeviceToken:(NSData*)deviceToken\n{\n","    [super application:application didRegisterForRemoteNotificationsWithDeviceToken:deviceToken];\n");
		UnityAppController.WriteBelow ("didFailToRegisterForRemoteNotificationsWithError:(NSError*)error\n{\n"," [super application:application didFailToRegisterForRemoteNotificationsWithError:error];\n");
		//UnityAppController.WriteBelow ("openURL: (NSURL*)url options: (NSDictionary<NSString*, id>*)options\n{\n", " [super application:application openURL:url sourceApplication:sourceApplication annotation:annotation];");
                                        

        UnityAppController.WriteBelow ("applicationDidEnterBackground:(UIApplication*)application\n{\n","   [super applicationDidEnterBackground:application];\n   glFinish();");
		UnityAppController.WriteBelow ("applicationWillEnterForeground:(UIApplication*)application\n{\n","    [super applicationWillEnterForeground:application];\n    glFinish();");
		UnityAppController.WriteBelow ("applicationDidBecomeActive:(UIApplication*)application\n{\n"," [super applicationDidBecomeActive:application];\n" +
			"    //[[HSAnalytics sharedInstance] setFBEventNamePurchase:FBSDKAppEventParameterNameContentID registration:FBSDKAppEventNameCompletedRegistration spentCredits:FBSDKAppEventNameSpentCredits];\n");

		UnityAppController.WriteBelow ("applicationWillResignActive:(UIApplication*)application\n{\n","    [super applicationWillResignActive:application];\n    glFinish();");
		UnityAppController.WriteBelow ("applicationWillTerminate:(UIApplication*)application\n{\n","    [super applicationWillTerminate:application];\n" +
			"    [[NativeAPI sharedInstance] tearDown];\n");
		UnityAppController.WriteBelow ("idFinishLaunchingWithOptions:(NSDictionary*)launchOptions\n{\n",getLaunchCode());

	//local push 
		UnityAppController.WriteBelow("[super application:application didReceiveLocalNotification:notification];\n"," if (notification) {\n       " +
			"NSDictionary *dic =  notification.userInfo;\n        " +
			"         if( [dic count] ) {\n"+
			"NSData *data = [NSJSONSerialization dataWithJSONObject:dic options:NSJSONWritingPrettyPrinted error:nil];\n        " +
			"NSString *jsonStr = [[NSString alloc]initWithData:data encoding:NSUTF8StringEncoding];\n        " +
			"[NativeAPI sharedInstance].userInfo = jsonStr;\n        [[NativeAPI sharedInstance] sendLocalNotifiy];\n        " +
			"//[[HSAnalytics sharedInstance] logEvent:@\"LaunchWithLocalNotification\" withParameters:dic];\n " +
			"    }\n"+
			"}");
		// UIInterfaceOrientation
		UnityAppController.Replace("return [[window rootViewController] supportedInterfaceOrientations] | _forceInterfaceOrientationMask;",
			"return UIInterfaceOrientationMaskPortrait |UIInterfaceOrientationMaskLandscapeLeft|UIInterfaceOrientationMaskLandscapeRight;");

//		#if GAME_SLOTS
		//iphoneX fit to 16/9， 简单的根据width 大于 screen 两倍判断
//		string scrIphoneCode="_window         = [[UIWindow alloc] initWithFrame: [UIScreen mainScreen].bounds]";
//		string dstIphoneCode = "    CGRect winSize = [UIScreen mainScreen].bounds;\n" +
//			"    if (winSize.size.width / winSize.size.height > 2) {\n" +
//			"        NSLog(@\"width:%f\",winSize.size.width);\n" +
//			"        NSLog(@\"height:%f\",winSize.size.height);\n" +
//			"        winSize.size.width -= 144;\n" +
//			"        winSize.origin.x = 72;\n" +
//			"        ::printf(\"-> is iphonex hello world\\n\");\n" +
//			"    } else {\n" +
//			"        ::printf(\"-> is not iphonex hello world\\n\");\n" +
//			"    }\n" +
//			"    _window = [[UIWindow alloc] initWithFrame: winSize];\n";
//		UnityAppController.Replace(scrIphoneCode,dstIphoneCode);
//		#endif

		//splashScreen.mm
		//var OrginFilePath = "Assets/Platforms/IOS/Plugins/iOSModifiedUnityCode/SplashScreen.mm" ;
		//string text_all = File.ReadAllText(OrginFilePath);
		//StreamWriter streamWriter = new StreamWriter(pathToBuiltXcodeProject+"/Classes/UI/SplashScreen.mm");
		//streamWriter.Write(text_all);
		//streamWriter.Close();
/*      //隐藏Home Indicator
        //UnityViewControllerBaseiOS.h
        XClass UnityViewControllerBaseiOSH = new XClass(pathToBuiltXcodeProject + "/Classes/UI/UnityViewControllerBaseiOS.h");
        UnityViewControllerBaseiOSH.WriteBelow("- (NSUInteger)supportedInterfaceOrientations;",
                                                                                       "\n- (UIRectEdge)preferredScreenEdgesDeferringSystemGestures;\n - (BOOL)prefersHomeIndicatorAutoHidden;\n ");
        //UnityViewControllerBaseiOS.mm
        XClass UnityViewControllerBaseiOS = new XClass(pathToBuiltXcodeProject + "/Classes/UI/UnityViewControllerBaseiOS.mm");
        UnityViewControllerBaseiOS.Replace("return UnityGetHideHomeButton();", "return YES;");
 */  
        //UnityViewControllerBaseiOS.mm   Edge Protection
        //XClass UnityViewControllerBaseiOS = new XClass(pathToBuiltXcodeProject + "/Classes/UI/UnityViewControllerBaseiOS.mm");
        //UnityViewControllerBaseiOS.Replace("- (UIRectEdge)preferredScreenEdgesDeferringSystemGestures\n{\n"+
                                           //"    UIRectEdge res = UIRectEdgeNone;\n"+
                                           //"    if (UnityGetDeferSystemGesturesTopEdge())\n"+
                                           //"        res |= UIRectEdgeTop;\n"+
                                           //"    if (UnityGetDeferSystemGesturesBottomEdge())\n"+
                                           //"        res |= UIRectEdgeBottom;\n"+
                                           //"    if (UnityGetDeferSystemGesturesLeftEdge())\n"+
                                           //"        res |= UIRectEdgeLeft;\n"+
                                           //"    if (UnityGetDeferSystemGesturesRightEdge())\n"+
                                           //"        res |= UIRectEdgeRight;\n"+
                                           //"    return res;\n"+
                                           //"}\n",
                                           //"- (UIRectEdge)preferredScreenEdgesDeferringSystemGestures\n{\n" +
                                           //"    return UIRectEdgeAll;\n"+
                                           //"}\n");
	}
	
	protected virtual string getLaunchCode()
	{
		string userInfoDebugStr = "";
//		if(Debug.isDebugBuild){
//		userInfoDebugStr = "        // 显示推送内容（按需求是否显示）\n" +		
//			"        UIAlertView *alert = [[UIAlertView alloc] initWithTitle:@\"提示\" message:jsonStr delegate:nil cancelButtonTitle:@\"确定\" otherButtonTitles:nil];\n" +
//			"        [alert show];\n";
//		}

		StringBuilder appendLaunchCode = new StringBuilder ();
		appendLaunchCode.Append ("#ifdef DEBUG\n" +
		                         "    [BFApplication setDebugEnabled:YES];\n" +
		                         "#endif\n" +
		                         "    [super application:application didFinishLaunchingWithOptions:launchOptions];\n" +
		                         "    DebugLog(@\"data = %@\\n\", [BFConfig sharedInstance].data);\n" +
		                         "    NSString *dataStr = [NSString stringWithFormat:@\"data = %@\",[BFConfig sharedInstance].data];\n" +
		                         "    printf_console(\"%s\\n\",(char*)dataStr.UTF8String);\n");
        appendLaunchCode.Append ("[FIRApp configure];\n");
		appendLaunchCode.Append ("\n  [[NativeAPI sharedInstance] setupNatvieAPI];\n");
//		appendLaunchCode.Append (" if([UIApplication instancesRespondToSelector:@selector(registerUserNotificationSettings:)]) {\n" +
//		                         "        UIUserNotificationSettings *notificationSettings = [UIUserNotificationSettings settingsForTypes:(UIUserNotificationTypeBadge\n" +
//		                         "                                                                |UIUserNotificationTypeSound\n" +
//		                         "                                                                 |UIUserNotificationTypeAlert) categories:nil];\n" +
//		                         "        [[UIApplication sharedApplication] registerUserNotificationSettings:notificationSettings];\n" +
//		                         "    }\n");
		appendLaunchCode.Append ("\n    UILocalNotification *notification = launchOptions[UIApplicationLaunchOptionsLocalNotificationKey];\n");
		appendLaunchCode.Append ("\n    if (notification) {\n" +
			"         NSDictionary *dic =  notification.userInfo;\n" + 
			"         if( [dic count] ) {\n"+
			"        NSData *data = [NSJSONSerialization dataWithJSONObject:dic options:NSJSONWritingPrettyPrinted error:nil];\n" +
			"        NSString *jsonStr = [[NSString alloc]initWithData:data encoding:NSUTF8StringEncoding];\n" +
			"        [NativeAPI sharedInstance].userInfo = jsonStr;\n"+
			userInfoDebugStr +
			"        //[[HSAnalytics sharedInstance] logEvent:@\"LaunchWithLocalNotification\" withParameters:dic];\n" +
			"    }\n");
		appendLaunchCode.Append ("\n  NSDictionary* remoteNotificationPayload = launchOptions[UIApplicationLaunchOptionsRemoteNotificationKey];\n" +
			"    if (remoteNotificationPayload)    {\n" +
			"        //[[HSAnalytics sharedInstance] logEvent:@\"LaunchWithRemoteNotification\"];\n" +
			"    }\n"+
			"    }\n");
		return appendLaunchCode.ToString();
	}

	protected virtual string getNewFunctionCode()
	{
		StringBuilder appendFunctinoCode = new StringBuilder();
//		appendFunctinoCode.Append("- (void)application:(UIApplication *)application didReceiveRemoteNotification:(NSDictionary *)userInfo fetchCompletionHandler:(void (^)(UIBackgroundFetchResult result))completionHandler\n{\n" +
//			"    [super application:application didReceiveRemoteNotification:userInfo fetchCompletionHandler:completionHandler];\n}\n");

		appendFunctinoCode.Append ("- (void)application:(UIApplication *)application didRegisterUserNotificationSettings:(UIUserNotificationSettings *)notificationSettings {\n" +
			"    [super application:application didRegisterUserNotificationSettings:notificationSettings];\n}\n");

		appendFunctinoCode.Append ("- (NSString *)configFileName{\n" +
			"#ifdef DEBUG\n"+
			"return @\"config-d.ya\";\n"+
			"#else\n"+
			"return @\"config-r.ya\";\n"+
		                           "#endif\n}\n");

        StringBuilder codeStringBuilder = new StringBuilder();
        codeStringBuilder.AppendLine("\n");
        codeStringBuilder.AppendLine("-(void)onApplicationStart {");
        codeStringBuilder.AppendLine("    [super onApplicationStart];");
        codeStringBuilder.AppendLine("    [[NSNotificationCenter defaultCenter]");
        codeStringBuilder.AppendLine("        addObserverForName:BFRtotMgrGetTaskConnectionDidFinish object:nil queue:nil usingBlock:^(NSNotification* _Nonnull note) { ");
        codeStringBuilder.AppendLine("            self.rtotMgrGetTaskConnectionDidFinish = YES;");
        codeStringBuilder.AppendLine("        }];");
		codeStringBuilder.AppendLine("    [[BFConfig sharedInstance] fetchRemote];");
		codeStringBuilder.AppendLine("    [[NSNotificationCenter defaultCenter]");
		codeStringBuilder.AppendLine("        addObserverForName:kBFNotificationName_RemoteFetchFinished object:nil queue:nil usingBlock:^(NSNotification* _Nonnull note) { ");
		codeStringBuilder.AppendLine("            UnitySendMessage(\"GameConsole\", \"RemoteConfigDidFinishInit\", \"\");");
		codeStringBuilder.AppendLine("        }];");
        codeStringBuilder.AppendLine("}");


        //[[HSRtotMgr sharedInstance] sendEventToServer:@"ClassicSlotABTask4" action:@"Vote2_1"];
        appendFunctinoCode.Append(codeStringBuilder.ToString());

		return appendFunctinoCode.ToString();
	}

	protected virtual void ModifyInfoPlist()
	{
		XPlist plist = new XPlist(pathToBuiltXcodeProject);
		
		System.Text.StringBuilder sb = new System.Text.StringBuilder();

		string szFormat = @"
           	<key>CFBundleVersion</key>
	<string>";
		string bundleVersion =	PlayerSettings.bundleVersion;
		string bundleLastKey =		"</string>";
		sb.Append(szFormat);
		sb.Append(bundleVersion);
		sb.Append(bundleLastKey);

		plist.AddKey(sb.ToString());
//		plist.ReplaceKey("<string>en</string>","<string>zh_CN</string>");
		plist.Save();
	}


	//less use ,substitution of ya configuration
	protected virtual void ModifyYamlFile()
	{
        string projPath = UnityEditor.iOS.Xcode.PBXProject.GetPBXProjectPath(pathToBuiltXcodeProject);
		UnityEditor.iOS.Xcode.PBXProject proj = new UnityEditor.iOS.Xcode.PBXProject();
		
		proj.ReadFromString(File.ReadAllText(projPath));
#if UNITY_2019_3_OR_NEWER
		string target = proj.GetUnityMainTargetGuid();
#else
		string target = proj.TargetGuidByName("Unity-iPhone");
#endif

		string fileName = "config.ya";
		var filePath =Path.Combine(Application.dataPath, Path.Combine("Platforms/IOS/Plugins/iOS/Res", fileName));
		File.Copy(filePath, Path.Combine(pathToBuiltXcodeProject, fileName));
		proj.AddFileToBuild(target, proj.AddFile(fileName, fileName,UnityEditor.iOS.Xcode.PBXSourceTree.Source));
		
		File.WriteAllText(projPath, proj.WriteToString());
    }
}
