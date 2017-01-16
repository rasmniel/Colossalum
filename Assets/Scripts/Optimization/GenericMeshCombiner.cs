using UnityEngine;
using System.Collections.Generic;

public class GenericMeshCombiner : MonoBehaviour
{
	[Tooltip("Tag of prefabs to optimize.")]
	public string PrefabTag;
	[Tooltip("Fetch prefabs recursively?")]
	public bool R = true;
	[Tooltip("Determines whether to detect materials to optimize or only use specified list.")]
	public bool AutoFetchMats = true;
	[Tooltip("Specified materials to use. Ignored if AutoFetchMats is true.")]
	public Material[] Materials;

	private List<GameObject> prefabs;
	private List<MeshObjectCombiner> combiners;

	void Start()
	{
		Invoke("Streamline", 0.5f);
	}

	public void Streamline()
	{
		// Fetch the prefabs.
		prefabs = new List<GameObject>();
		fetchPrefabs(transform);
		// Prepare combiners.
		combiners = new List<MeshObjectCombiner>();
		if (!AutoFetchMats) prepareCombiners();
		populateCombiners();
		// Combine meshes and destroy.
		combineMeshes();
		Destroy(this);
	}

	private void fetchPrefabs(Transform t)
	{
		// Acquire all prefabs with the specified tag.
		foreach (Transform child in t)
		{
			if (child.CompareTag(PrefabTag))
				prefabs.Add(child.gameObject);
			else if (R && child.childCount > 0)
				fetchPrefabs(child);
		}
	}

	private void prepareCombiners()
	{
		// Prepare combiners for optimization without material-detection.
		foreach (Material m in Materials)
			combiners.Add(new MeshObjectCombiner(transform, m));
	}

	private void populateCombiners()
	{
		for (int i = 0; i < prefabs.Count; i++)
		{
			// For each prefab to combine, set the original inactive. 
			GameObject current = prefabs[i];
			// Get children's mesh filters and iterate over them.
			MeshFilter[] childFilters = current.GetComponentsInChildren<MeshFilter>(true);
			for (int j = 0; j < childFilters.Length; j++)
			{
				// Set a new combine instance to the mesh filter values.
				MeshFilter filter = childFilters[j];
				CombineInstance instance = new CombineInstance();
				instance.mesh = filter.mesh;
				instance.transform = filter.transform.localToWorldMatrix;
				// Get the material name.
				MeshRenderer renderer = filter.GetComponent<MeshRenderer>();
				string materialName = renderer.material.name;
				// Create new combiner for current material if it does not match an existing combiner's material.
				if (!addInstance(instance, materialName) && AutoFetchMats) // Provided AutoFetchMats is checked.
					addNewMaterialWithInstance(renderer.material, instance);
				// Destroy the old mesh components so that only the collider remains.
				Destroy(renderer);
				Destroy(filter);
			}
		}
	}

	private bool addInstance(CombineInstance instance, string material)
	{
		// Add instance and return true if material exist.
		foreach (MeshObjectCombiner combiner in combiners)
		{
			if (combiner.Material.name == material)
			{
				combiner.AddCombineInstance(instance);
				return true;
			}
		}
		// Return false, if it does not.
		return false;
	}

	private void addNewMaterialWithInstance(Material material, CombineInstance instance)
	{
		MeshObjectCombiner newCombiner = new MeshObjectCombiner(transform, material);
		newCombiner.AddCombineInstance(instance);
		combiners.Add(newCombiner);
	}

	private void combineMeshes()
	{
		foreach (MeshObjectCombiner combiner in combiners)
			combiner.Combine();
	}

	private class MeshObjectCombiner
	{
		public Material Material;
		public Transform transform;

		private GameObject meshObject;
		private List<CombineInstance> combineInstances;

		public MeshObjectCombiner(Transform t, Material m)
		{
			combineInstances = new List<CombineInstance>();
			transform = t;
			Material = m;
			// Instantiate an empty game object with a reset transform to hold the combined mesh at an offset.
			meshObject = Instantiate(new GameObject(), Vector3.zero, Quaternion.identity) as GameObject;
			meshObject.name = m.name;
		}

		public void AddCombineInstance(CombineInstance instance)
		{
			combineInstances.Add(instance);
		}

		public void Combine()
		{
			if (combineInstances.Count > 0)
			{
				meshObject.transform.parent = transform;
				// Add a mesh renderer and set the material.
				MeshRenderer renderer = meshObject.AddComponent<MeshRenderer>();
				renderer.material = Material;
				// Add a mesh filter and combine.
				MeshFilter filter = meshObject.AddComponent<MeshFilter>();
				filter.mesh = new Mesh();
				filter.mesh.CombineMeshes(combineInstances.ToArray());
				filter.mesh.Optimize();
				// Destroy all meshes used to build combined mesh.
				foreach (CombineInstance instance in combineInstances)
					Destroy(instance.mesh);
				// Note: Unity does not always free meshes from memory automatically!
			}
		}
	}
}
