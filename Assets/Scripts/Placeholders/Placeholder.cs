using UnityEngine;
using System.Collections;

public class Placeholder : MonoBehaviour
{
	[Range(0, 1)]
	public float SpawnChance = 1.0f;
	public GameObject DesiredObject;
	public bool FreeRoam;
	public bool StayAlive;
	void Start()
	{
		SetDesiredValues();
		DestroyIndicator();
		InstantiateDesiredObject();
	}

	protected virtual void SetDesiredValues()
	{
		DesiredPosition = DesiredScale = null;
		DesiredRotation = null;
	}

	protected Vector3? DesiredPosition, DesiredScale;
	protected Quaternion? DesiredRotation;
	protected void InstantiateDesiredObject()
	{
		bool doInstantiate = Random.value <= SpawnChance;
		if (DesiredObject != null && doInstantiate)
		{
			// Instantiate desired object.
			GameObject obj = Instantiate(DesiredObject, Vector3.zero, Quaternion.identity) as GameObject;
			obj.transform.position = DesiredPosition ?? transform.position;
			obj.transform.rotation = DesiredRotation ?? transform.rotation;
			obj.transform.localScale = DesiredScale ?? transform.localScale;
			if (FreeRoam) obj.transform.parent = null;
			else if (StayAlive) obj.transform.parent = transform;
			else
			{
				obj.transform.parent = transform.parent;
				Destroy(gameObject);
			}
		}
		else Destroy(gameObject);
	}

	protected void DestroyIndicator()
	{
		// Destroy placeholder indicator.
		if (transform.childCount > 0)
		{
			GameObject indicator = transform.GetChild(0).gameObject;
			if (indicator != null)
				Destroy(indicator);
		}
	}

	protected GameObject getRandom(GameObject[] objects)
	{
		return objects[Random.Range(0, objects.Length)];
	}
}
