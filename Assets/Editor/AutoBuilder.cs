using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class AutoBuilder : MonoBehaviour
{
	// init build params
	static string buildPath = "build";
	static string appName = GetAppName(PlayerSettings.bundleIdentifier);
	static string[] scenes;
	
	// Menu Items
	[MenuItem("Build/Build Web")]
	static void BuildWeb ()
	{
		BuildProject ("web");
	}
	
	[MenuItem("Build/Build iOS")]
	static void BuildIOS ()
	{
		BuildProject ("ios");
	}
	
	[MenuItem("Build/Build Android")]
	static void BuildAndroid ()
	{
		BuildProject ("android");
	}
	
	// Build Logic
	private static void BuildProject (string platform)
	{
        // set vals
        scenes = GetScenes();
		BuildTarget target = BuildTarget.StandaloneOSXUniversal;
		string deployPath = buildPath + "-" + platform + "/";

		// remove previous builds / directory
		ClearBuildPath (deployPath);
		
		// setup build target
		switch (platform) {
		case "ios":
			target = BuildTarget.iOS;
			break;
		case "android":
			target = BuildTarget.Android;
			deployPath += appName + ".apk";
			break;
		case "web":
		case "default":
			target = BuildTarget.WebPlayerStreamed;
			break;
		}
		
		// build
		BuildPipeline.BuildPlayer (scenes, deployPath, target, BuildOptions.None);
	}
	
	private static string[] GetScenes ()
	{
		List<string> s = new List<string>(); 
		
		for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
		{
			if (EditorBuildSettings.scenes[i].enabled)
			{
				s.Add (EditorBuildSettings.scenes[i].path);
			}
		}
		
		return s.ToArray();
	}

	private static string GetAppName(string s)
	{
		int position = s.LastIndexOf('.');
		if (position > -1)
			s = s.Substring(position + 1);

		return s;
	}
	
	private static void ClearBuildPath(string path)
	{
		if (Directory.Exists(path))
		{
			Directory.Delete(path, true);
		}
		
		Directory.CreateDirectory(path);
	}
}