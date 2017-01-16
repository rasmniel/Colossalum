using UnityEngine;
using System.Collections;

public class Scenery : Placeholder
{
	[Range(0, 1)]
	public float SizeVariation;
	public GameObject[] DesiredObjects;

	protected override void SetDesiredValues()
	{
		DesiredRotation = Quaternion.Euler(0, Random.value * 360, 0);
		DesiredScale = Vector3.one * Random.Range(1 - SizeVariation, 1 + SizeVariation);
		DesiredObject = getRandom(DesiredObjects);
	}
}
