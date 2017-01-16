using UnityEngine;
using System.Collections;

public class Breakable : MonoBehaviour
{
	public int Threshold = 1;
	public int TileCountToSpawn = 0;
	[Range(0, 1)]
	public float Rarity = 1;
	public GameObject[] ColliderObjects;
	public GameObject Effect;

	void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Weapon"))
		{
			if ((int)other.GetComponent<Weapon>().Type >= Threshold)
				doBreak();
		}
	}
		
	public float BreakForce = 250.0f;
	private const float
		ForceDeviation = 0.25f,
		BreakableSize = 2.5f;
	private void doBreak()
	{
		if (Effect != null)
			Effect.SetActive(false);
		foreach (Transform t in GetComponentsInChildren<Transform>())
		{
			Rigidbody body = t.GetComponent<Rigidbody>();
			if (body != null)
			{
				// Active rigidbody for using gravity.
				body.useGravity = true;
				body.isKinematic = false;
				// Calculate force.
				float deviation = BreakForce * ForceDeviation;
				float force = Random.Range(BreakForce - deviation, BreakForce + deviation);
				// Calculate explosion position and offset.
				Vector3 offset =
					Vector3.forward * Random.Range(-ForceDeviation, ForceDeviation) +
					Vector3.right * Random.Range(-ForceDeviation, ForceDeviation);
				Vector3 position = transform.position + offset;
				// Break!
				body.AddExplosionForce(force, position, BreakableSize);
				Item item = t.GetComponent<Item>();
				if (item != null) item.ToggleScale(true);
			}
		}
		foreach (GameObject obj in ColliderObjects)
			Destroy(obj);
		Destroy(GetComponent<Collider>());
	}
}