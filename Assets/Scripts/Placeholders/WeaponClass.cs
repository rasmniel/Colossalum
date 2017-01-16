using UnityEngine;
using System.Collections;

public class WeaponClass : Placeholder
{
	public GameObject[] DesiredWeapons;

	protected override void SetDesiredValues()
	{
		DesiredRotation = Quaternion.Euler(transform.eulerAngles);
		DesiredObject = getRandom(DesiredWeapons);
	}
}
