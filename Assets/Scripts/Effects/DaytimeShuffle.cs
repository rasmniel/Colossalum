using UnityEngine;
using System.Collections;

public class DaytimeShuffle : MonoBehaviour
{
	void Start()
	{
		float rotation = Random.value * 180;
		transform.rotation = Quaternion.Euler(rotation, 0, 0);
	}
}
