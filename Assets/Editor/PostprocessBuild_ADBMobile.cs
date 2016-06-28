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
			configPathXcodeFull = configPathXcodeFull.Replace("//", "/");
			string dirTest = "/Libraries/Plugins/iOS/ADBMobile";

			PBXProject proj = new PBXProject();
			proj.ReadFromString(File.ReadAllText(projPath));

			string target = proj.TargetGuidByName("Unity-iPhone");
			proj.AddFileToBuild(target, proj.AddFile("usr/lib/libsqlite3.tbd", "Frameworks/libsqlite3.tbd", PBXSourceTree.Sdk));
			proj.SetBuildProperty(target, "ENABLE_BITCODE", "false");

			//Copy file from Unity to Xcode
			if(File.Exists(configPathUnityFull))
			{
				File.Copy(configPathUnityFull, configPathXcodeFull);
			}
				
			//Add file to the project, and make sure it is included in the Xcode build
			//Default building
			if(File.Exists(configPathXcodeFull))
			{
				Debug.LogError("FOUND JASON FILE IN XCODE HERE: " + configPathXcodeFull);

				if(Directory.Exists(path + dirTest))
				{
					string directoryString = path + dirTest;
					directoryString.Replace("//", "/");
					Debug.LogError("FOUND A DIRECTORY TO PUT THE JASON FILE HERE: " + directoryString);
					string fileToAdd = proj.AddFile(configPathXcodeFull, configPathXcode);
					proj.AddFileToBuild(target, fileToAdd);
				}
				else
				{
					Debug.LogError("INVALID DIRECTORY TO PUT THE JASON FILE: " + dirTest);
				}


			}
			// CN Auto Build system (FastLane) has some quirks
			else
			{
				Debug.LogError("NO JASON FILE");
				string fastLanePath = configPathXcodeFull.Replace("build-ios","");
				if(File.Exists(fastLanePath))
				{
					Debug.LogError("FOUND FASTLANE JASON FILE");
					string fileToAdd = proj.AddFile(fastLanePath, configPathXcode);
					proj.AddFileToBuild(target, fileToAdd);
				}
			}
	

			File.WriteAllText(projPath, proj.WriteToString());
		}
	}
}
#endif
