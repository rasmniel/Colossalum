using UnityEngine;
using System.Collections;

public class CrowdControl : MonoBehaviour
{
	public Transform Hoomans;

	private ArrayList crowd;

	void Start()
	{
		gatherCrowd();
		determineAudience();
	}

	private void gatherCrowd()
	{
		crowd = new ArrayList();
		// Will search recursively for hoomans in the specified transform.
		if (Hoomans != null) countAudience(Hoomans);
		// ... or utilize own transform if none were specified.
		else countAudience(transform);
	}

	private void countAudience(Transform t)
	{
		foreach (Transform child in t)
		{
			if (child.CompareTag("Placeholder")) crowd.Add(child);
			if (child.childCount > 0) countAudience(child);
		}
	}

	[Range(0, 1)]
	public float AudienceDensity;
	private void determineAudience()
	{
		foreach (Transform hooman in crowd)
			if (Random.value >= AudienceDensity)
				hooman.gameObject.SetActive(false);
	}
}
