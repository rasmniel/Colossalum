using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour
{
	public string Type;
	public bool isSingle;
	public Color Color;
	public GameObject Craftable;

	private SphereCollider sphereCollider;
	void Start()
	{
		sphereCollider = GetComponent<SphereCollider>();
	}

	private bool _upscale = false;
	public void ToggleScale(bool upscale)
	{
		if (upscale == _upscale) return;
		if (_upscale)
		{
			sphereCollider.radius = sphereCollider.radius * 2;
			transform.localScale = transform.localScale / 2;
			_upscale = false;
		}
		else
		{
			sphereCollider.radius = sphereCollider.radius / 2;
			transform.localScale = transform.localScale * 2;
			_upscale = true;
		}
	}
}
