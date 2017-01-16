using UnityEngine;
using System.Collections;

public class SceneJanitor : MonoBehaviour
{
	void Start()
	{
		Invoke("DoClean", 1.5f);
	}

	void DoClean()
	{
		SceneJanitor.Clean();
	}

	public static void Clean()
	{
		// Clean up Unity's mess of new game objects arising from creating objects for mesh combining.
		GameObject[] moot = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[];
		for (int i = 0; i < moot.Length; i++)
		{
			if (moot[i].name == "New Game Object")
				Destroy(moot[i]);
			else if (isEmptyGameObject(moot[i]))
				Destroy(moot[i]);
		}
		Resources.UnloadUnusedAssets();
	}

	private static bool isEmptyGameObject(GameObject go)
	{
		return go.transform.childCount == 0 &&
		go.GetComponents<Component>().Length == 1 &&
		go.transform.position == Vector3.zero &&
		go.transform.rotation == Quaternion.identity &&
		go.transform.localScale == Vector3.one;
	}
}
