using UnityEngine;
using System.Collections;

public class SubstanceTest : MonoBehaviour
{
	public string base_url = "";
	public GameObject go = null;
	bool loading = false;

	protected static string PlatformString
	{
		get
		{
#if UNITY_FLASH
			return "swf";
#else
			switch (Application.platform) {
				case RuntimePlatform.Android:		return "android";
				case RuntimePlatform.IPhonePlayer:	return "ios";
				default:							return "web";
			}
#endif
		}
	}

	IEnumerator LoadAssetbundle(string name)
	{
		loading = true;
		if (go != null)
		{
			Destroy (go);
			go = null;
		}

		var www = new WWW(System.String.Format("{0}/assetbundles/{1}/{2}.assetbundle", base_url, PlatformString, name));
		Debug.Log(www.url);
		yield return www;

		loading = false;
		if (www.error != null)
		{
			Debug.LogError(www.error);
			yield break;
		}

		go = Instantiate (www.assetBundle.mainAsset, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
		www.assetBundle.Unload(false);
	}

	void OnGUI ()
	{
		GUI.enabled = !loading;
		GUILayout.BeginArea(new Rect(0, 0, 100, 500));
		if (GUILayout.Button("A", GUILayout.Width(60)))
			StartCoroutine(LoadAssetbundle("A"));
		if (GUILayout.Button("B", GUILayout.Width(60)))
			StartCoroutine(LoadAssetbundle("B"));
		if (GUILayout.Button("C", GUILayout.Width(60)))
			StartCoroutine(LoadAssetbundle("C"));
		GUILayout.EndArea();
		GUI.enabled = true;
	}

	void Update () {
		if (go != null)
			go.transform.Rotate(5*Time.deltaTime, 10*Time.deltaTime, 0);
	}
}
