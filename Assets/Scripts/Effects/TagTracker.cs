using UnityEngine;
using System.Collections;

public class TagTracker : MonoBehaviour
{
	public string Tag;

	private GameObject _target;
	private bool _visible;

	void Update()
	{
		track();
	}

	private void track()
	{
		if (_target != null)
		{
			transform.LookAt(_target.transform.position);
			if (!_visible)
			{
				transform.GetChild(0).gameObject.SetActive(true);
				_visible = true;
			}
		}
		else
		{
			_target = GameObject.FindGameObjectWithTag(Tag);
			if (_visible)
			{
				transform.GetChild(0).gameObject.SetActive(false);
				_visible = false;
			}
		}
	}
}
