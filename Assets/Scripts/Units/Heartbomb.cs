using UnityEngine;
using System.Collections;

public class Heartbomb : MonoBehaviour
{
	public int Rings = 4, Size = 10;
	[Range(0, 1)]
	public float Radius = 0.1f;
	public float Power = 200.0f;
	public bool ExplodeOnStart = false;
	public Material OrbMaterial;

	private ArrayList orbs = new ArrayList();
	private bool _detonated = false;

	void Start()
	{
		if (ExplodeOnStart) Explode(false);
	}

	void Update()
	{
		if (_detonated) dissipate();
	}

	public void ExplodePercentage(bool detonated, float percentage)
	{
		int rings = Mathf.CeilToInt(Rings * percentage);
		int size = Mathf.CeilToInt(Size * percentage);
		ExplodeExact(detonated, rings, size);
	}

	public void ExplodeExact(bool detonated, int customRings, int customSize)
	{
		int r = Rings, s = Size;
		Rings = customRings;
		Size = customSize;
		Explode(detonated);
		Rings = r;
		Size = s;
	}

	public void Explode(bool detonated)
	{
		spawnOrbs();
		// Acquire blast center from transform.
		Vector3 center = transform.position;
		foreach (GameObject orb in orbs)
		{
			Rigidbody body = orb.GetComponent<Rigidbody>();
			if (body != null)
			{
				// Add explosive force to the orb's rigidbody.
				body.useGravity = true;
				body.isKinematic = false;
				body.AddExplosionForce(Power, center, Radius);
			}
			orb.transform.parent = null;
		}
		_detonated = detonated;
	}

	private void spawnOrbs()
	{
		// Iterate over the number of vertical rings.
		for (int i = 0; i < Rings; i++)
		{
			// Reduce the radius as rings increase, making the bomb top-heavy.
			float spawnRadius = Radius - (Radius / Rings * i);
			// Iterate over the amount of orbs per ring.
			for (int j = 0; j < Size; j++)
			{
				float angle = (j * Mathf.PI * 2) / Size;
				float x = Mathf.Cos(angle) * spawnRadius;
				float y = Radius - spawnRadius;
				float z = Mathf.Sin(angle) * spawnRadius;
				createOrb(new Vector3(x, y, z));
			}
		}
	}

	private GameObject createOrb(Vector3 position)
	{
		// Create an orb.
		GameObject orb = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		orb.layer = LayerMask.NameToLayer("Env");
		orbs.Add(orb);

		// Manipulate it's transform.
		orb.transform.parent = transform.parent;
		orb.transform.localScale = transform.localScale;
		orb.transform.localPosition = position;

		// Alter components.
		Destroy(orb.GetComponent<SphereCollider>());
		Rigidbody body = orb.AddComponent<Rigidbody>();
		body.useGravity = false;
		body.isKinematic = true;
		orb.AddComponent<BoxCollider>();

		// Change appearance.
		Renderer renderer = orb.GetComponent<Renderer>();
		renderer.material = OrbMaterial;
		renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

		return orb;
	}

	public int DissipationTimer = 250;
	private void dissipate()
	{
		if (DissipationTimer >= 0) DissipationTimer--;
		else
		{
			foreach (GameObject orb in orbs) Destroy(orb);
			Destroy(gameObject);
		}
	}
}
