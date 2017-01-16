using UnityEngine;
using UnityEditor;
using System.Collections;

public class Phyllotaxis : ScriptableWizard
{
	public GameObject Original;
	public int Amount;
	public float 
		ShrinkFactor = 0.03f,
		ElevationFactor = 0.25f;

	[MenuItem("Custom/Phyllotaxis")]
	static void CreateWizard()
	{
		ScriptableWizard.DisplayWizard("Phyllotaxis", typeof(Phyllotaxis), "Fib!");
	}

	void OnWizardCreate()
	{
		fib(); //!
	}

	private const float	GoldenAngle = 137.51f;
	private void fib()
	{
		Transform t = Original.transform;
		// Original object corresponds to i = 0
		for (int i = 1; i < Amount; i++)
		{
			Vector3 position = new Vector3(t.position.x, t.position.y + (ElevationFactor * i), t.position.z);
			Quaternion rotation = Quaternion.Euler(t.rotation.x, t.rotation.y + (GoldenAngle * i), t.rotation.z);				
			GameObject clone = Instantiate(Original, position, rotation) as GameObject;
			clone.name = clone.name.Remove(clone.name.Length - "(Clone)".Length);
			clone.transform.parent = t.parent;
			float downScale = 1 - (ShrinkFactor * i);
			clone.transform.localScale = clone.transform.localScale * downScale;
		}
	}
}
