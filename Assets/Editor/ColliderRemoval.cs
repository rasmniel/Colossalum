using UnityEngine;
using UnityEditor;
using System.Collections;

public class ColliderRemoval : ScriptableWizard
{
	public GameObject Target;
	[MenuItem("Custom/Collider Removal")]

	static void CreateWizard()
	{
		ScriptableWizard.DisplayWizard("Collider Removal", typeof(ColliderRemoval), "Remove");
	}

	void OnWizardCreate()
	{
		removeColliders(Target.transform);
	}

	private void removeColliders(Transform t)
	{
		foreach (Transform child in t)
		{
			Collider collider = child.GetComponent<Collider>();
			if (collider != null)
				DestroyImmediate(collider);
			if (child.childCount > 0)
				removeColliders(child);
		}
	}
}
