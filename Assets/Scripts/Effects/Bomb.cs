using UnityEngine;
using System.Collections;

public class Bomb : MonoBehaviour
{
	public float Radius = 0.1f;
	public float Power = 10.0f;
	public float PowerDeviation = 5.0f;

	private ArrayList destroyedEnv;
	private bool _detonated = false;

	void Update()
	{
		if (_detonated) dissipate();
	}

	public void Explode()
	{
		// Calculate power deviation.
		float deviation = Random.Range(-PowerDeviation, PowerDeviation);
		Power += deviation;
		// Acquire blast center from transform.
		Vector3 center = transform.position;
		// Count colliders hit by the blast.
		Collider[] impactedColliders = Physics.OverlapSphere(center, Radius);
		destroyedEnv = new ArrayList();
		// Cycle colliders hit by the blast.
		foreach (Collider hit in impactedColliders)
		{
			// Affectable objects are in the environmental layer.
			string layer = LayerMask.LayerToName(hit.gameObject.layer);
			if (layer == "Env")
			{
				// ... and have a rigidbody.
				Rigidbody body = hit.GetComponent<Rigidbody>();
				if (body != null)
				{
					destroyedEnv.Add(hit.gameObject);
					// Add explosive force to the colliders rigidbody.
					body.useGravity = true;
					body.isKinematic = false;
					body.AddExplosionForce(Power, center, Radius);
				}
			}
		}
		_detonated = true;
	}

	public int DissipationTimer = 500;
	private void dissipate()
	{
		if (DissipationTimer >= 0) DissipationTimer--;
		else
		{
			foreach (GameObject hit in destroyedEnv) Destroy(hit);
			Destroy(gameObject);
		}
	}
}
