using UnityEngine;
using System.Collections;

public class HueShuffler : MonoBehaviour
{
	public Material[] Materials;
	public GameObject[] Subjects;

	void Update()
	{
		if (!_shuffled)
			shuffleHue();
		else
			Destroy(this);
	}

	private bool _shuffled = false;
	private void shuffleHue()
	{
		try
		{
			Material m = Materials[Random.Range(0, Materials.Length)];
			foreach (GameObject subject in Subjects)
				subject.GetComponent<MeshRenderer>().material = m;
			_shuffled = true;
		}
		finally
		{
		}
	}
}
