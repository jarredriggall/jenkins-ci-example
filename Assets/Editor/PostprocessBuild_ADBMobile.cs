#if UNITY_IOS
using UnityEditor.Callbacks;
using UnityEditor;
using System.IO;
using UnityEditor.iOS.Xcode;
using UnityEngine;

public class PostprocessBuild_ADBMobile : UnityEngine.MonoBehaviour
{
	[PostProcessBuild]
	public static void OnPostprocessBuild(BuildTarget buildTarget, string path)
	{
		if (buildTarget == BuildTarget.iOS)
		{
			string projPath = path + "/Unity-iPhone.xcodeproj/project.pbxproj";

			//Copy config vars
			string configPathUnity = "/Plugins/iOS/ADBMobile/ADBMobileConfig.json";
			string configPathUnityFull = Application.dataPath + configPathUnity;
			string configPathXcode = "/Libraries/Plugins/iOS/ADBMobile/ADBMobileConfig.json";
			string configPathXcodeFull = path + configPathXcode;

			PBXProject proj = new PBXProject();
			proj.ReadFromString(File.ReadAllText(projPath));

			string target = proj.TargetGuidByName("Unity-iPhone");

			proj.AddFileToBuild(target, proj.AddFile("usr/lib/libsqlite3.tbd", "Frameworks/libsqlite3.tbd", PBXSourceTree.Sdk));
			proj.SetBuildProperty(target, "ENABLE_BITCODE", "false");

			//Copy file from Unity to Xcode
			File.Copy(configPathUnityFull, configPathXcodeFull);

			//Add file to the project, and make sure it is included in the Xcode build
			string fileToAdd = proj.AddFile(configPathXcodeFull, configPathXcode);
			proj.AddFileToBuild(target, fileToAdd);

			File.WriteAllText(projPath, proj.WriteToString());
		}
	}
}
#endif
