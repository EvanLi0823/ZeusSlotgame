using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.XCodeEditor;

#endif
using System.IO;

public static class XCodePostProcess
{

#if UNITY_EDITOR
	[PostProcessBuild(999)]
	public static void OnPostProcessBuild( BuildTarget buildTarget, string pathToBuiltXcodeProject )
	{
		if (buildTarget != BuildTarget.iOS) {
			Debug.LogWarning("Target is not iPhone. XCodePostProcess will not run");
			return;
		}

		// Create a new project object from build target
		XCProject project = new XCProject( pathToBuiltXcodeProject );

		// Find and run through all projmods files to patch the project.
		// Please pay attention that ALL projmods files in your project folder will be excuted!
		string[] files = Directory.GetFiles( Application.dataPath, "*.projmods", SearchOption.AllDirectories );
		foreach( string file in files ) {
			UnityEngine.Debug.Log("ProjMod File: "+file);
			project.ApplyMod( file );
		}
		// Finally save the xcode project
		if (files.Length > 0) {
			CustomXcodeProcessBase.Instance.DealXcodeFile (project, pathToBuiltXcodeProject);
		}
		project.overwriteBuildSetting("ENABLE_BITCODE", "NO", "Release");
		project.overwriteBuildSetting("ENABLE_BITCODE", "NO", "Debug");
		project.overwriteBuildSetting("SWIFT_VERSION", "5.0");//对所有的环境进行设置
		
		project.Save();	

//		string projPath = pathToBuiltXcodeProject + "/Unity-iPhone.xcodeproj/project.pbxproj";
//
//		UnityEditor.iOS.Xcode.PBXProject proj = new UnityEditor.iOS.Xcode.PBXProject();
//		proj.ReadFromString(File.ReadAllText(projPath));
//		string target = proj.TargetGuidByName("Unity-iPhone");
//
//		// GoogleMobileAds (aka AdMob):
//		proj.AddBuildProperty(target, "CLANG_ENABLE_MODULES", "YES");
//		proj.AddBuildProperty(target, "OTHER_LDFLAGS", "$(inherited)");
//
//		proj.RemoveFrameworkFromProject (target, "HSBasicAppFramework.framework");
//
//		File.WriteAllText(projPath, proj.WriteToString());
	}
#endif

	public static void Log(string message)
	{
		UnityEngine.Debug.Log("PostProcess: "+message);
	}


}
