using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

static class ArrayExtension
{
	public static void SwapArrayElements<T>(this T[] inputArray, int index1, int index2)
	{
		T temp = inputArray[index1];
		inputArray[index1] = inputArray[index2];
		inputArray[index2] = temp;
	}
}

public class Export
{
	static IEnumerable<BuildTarget> GetTargets()
	{
		BuildTarget[] targets = {BuildTarget.WebPlayer,
								 BuildTarget.FlashPlayer,
								 BuildTarget.Android,
								 BuildTarget.iPhone};


		int current = Array.IndexOf(targets, EditorUserBuildSettings.activeBuildTarget);
		if (current != -1)
			targets.SwapArrayElements(0, current);

		foreach (BuildTarget target in targets)
			yield return target;
	}

	[MenuItem("Assets/Build AssetBundles")]
	static void BuildAssetBundles ()
	{
		IEnumerable<BuildTarget> targets = GetTargets();

		foreach (BuildTarget target in targets)
		{
			EditorUserBuildSettings.SwitchActiveBuildTarget(target);
			foreach (string input in Directory.GetFiles(@"Assets/Prefabs/", @"*.prefab"))
			{
				string output = "public/assetbundles/";
				switch (EditorUserBuildSettings.activeBuildTarget)
				{
				case BuildTarget.WebPlayer:
				case BuildTarget.WebPlayerStreamed:
					output = Path.Combine(output, "web");
					break;
				case BuildTarget.FlashPlayer:
					output = Path.Combine(output, "swf");
					break;
				case BuildTarget.Android:
					output = Path.Combine(output, "android");
					break;
				case BuildTarget.iPhone:
					output = Path.Combine(output, "ios");
					break;
				}

				output = Path.Combine(output, Path.GetFileName(input));
				output = Path.ChangeExtension(output, ".assetbundle");

				FileInfo info = new FileInfo(output);
				if (!info.Directory.Exists)
					Directory.CreateDirectory(info.Directory.FullName);

				UnityEngine.Object main = AssetDatabase.LoadMainAssetAtPath(input);
				BuildPipeline.BuildAssetBundle(main, null, output, BuildAssetBundleOptions.CollectDependencies, EditorUserBuildSettings.activeBuildTarget);		
			}
		}
	}

	[MenuItem("Assets/Build Players")]
	static void BuildPlayers ()
	{
		string[] scenes = new string[] { "Assets/scene.unity" };
		string outdir = "";
		string outname = "";

		IEnumerable<BuildTarget> targets = GetTargets();
		foreach (BuildTarget target in targets)
		{
			switch (target)
			{
			case BuildTarget.WebPlayer:
			case BuildTarget.WebPlayerStreamed:
				outname = "web";
				outdir = "public/";
				break;

			case BuildTarget.FlashPlayer:
				outname = "flash.swf";
				outdir = "public/flash/";
				break;

			case BuildTarget.Android:
				outname = "android.apk";
				outdir = "public/android/";
				break;

			case BuildTarget.iPhone:
				outname = "iOS";
				outdir = "";
				break;
			}

			string start = "";
			foreach (string part in outdir.Split('/'))
			{
				start += part + "/";
				if (!Directory.Exists(start))
					Directory.CreateDirectory(start);
			}

			string res = BuildPipeline.BuildPlayer(scenes, Path.Combine(outdir, outname), target, BuildOptions.None);
			if (res.Length > 0)
				throw new System.Exception("BuildPlayer failure: " + res);
		}
	}
}
