using UnityEngine;
using System.Collections;

public class DaggerSpread : MonoBehaviour
{
	public GameObject[] Daggers;
	void Start()
	{
		foreach (GameObject dagger in Daggers)
			dagger.SetActive(true);
	}
}
