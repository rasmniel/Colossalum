using UnityEngine;
using System.Collections;

public class AccessoryPlaceholder : Placeholder
{
	[Range(0, 1)]
	public float Chance;
	public GameObject Accessory;

	protected override void SetDesiredValues()
	{
		if (Random.value < Chance)
		{
			DesiredObject = Accessory;
			Quaternion rotation = Quaternion.Euler(Random.Range(-90, 90), Random.value * 360, Random.Range(-90, 90));
			DesiredRotation = rotation;
		}
	}
}
