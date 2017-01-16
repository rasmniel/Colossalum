using UnityEngine;
using System.Collections;

public class Fragmental : Mob
{
	public bool
		InstaWreck = false,
		PaintFragments = false;
	public Color Color = Color.red;

	private ArrayList fragments;
	private bool isAnimate = true;

	void Start()
	{
		InitializeFragments();
	}

	protected void InitializeFragments()
	{
		fragments = new ArrayList();
		countFragments(transform);
		if (InstaWreck)
			GetWrecked(null);
		if (PaintFragments)
			paintFragments(Color);
	}

	private void countFragments(Transform t)
	{
		// Recursively, count all fragments in the transform.
		foreach (Transform child in t)
		{
			if (child.childCount > 0)
				countFragments(child);
			// Children with a rigidbody are considered fragments in this regard.
			else if (child.GetComponent<Rigidbody>() != null)
				fragments.Add(new FragmentEntry(child.gameObject, t));
		}
	}

	[Range(0, 1)]
	public float WreckChance = 0.12f;
	protected override void GetWrecked(Weapon weapon)
	{
		float chance = 1;
		if (weapon != null)
			chance = (int)weapon.Type * WreckChance;
		int fragmentsRemaining = 0;
		foreach (FragmentEntry entry in fragments)
		{
			if (!entry.IsLost)
			{
				if (Random.value <= chance)
					entry.ToggleLost();
				else fragmentsRemaining++;
			}
		}
		validateIntegrity(fragmentsRemaining);
	}

	public int IntegrityFraction = 4;
	private void validateIntegrity(int fragmentsRemaining)
	{
		if (isAnimate)
		{
			if (fragmentsRemaining < fragments.Count / IntegrityFraction)
				setAnimateState(false);
		}
	}

	private void reanimate()
	{
		foreach (FragmentEntry entry in fragments)
			entry.Reanimate();
		setAnimateState(true);
	}

	protected delegate void AnimateStateChange(bool animate);
	protected AnimateStateChange OnAnimateStateChange;
	private void setAnimateState(bool animate)
	{
		isAnimate = animate;
		if (OnAnimateStateChange != null)
			OnAnimateStateChange(animate);
		if (!animate) GetWrecked(null);
	}

	protected override bool IsAlive()
	{
		return isAnimate;
	}

	private void paintFragments(Color c)
	{
		foreach (FragmentEntry entry in fragments)
			entry.Fragment.GetComponent<MeshRenderer>().material.color = c;
	}

	private class FragmentEntry
	{
		public Vector3 InitialPosition;
		public Quaternion InitialRotation;
		public Vector3 InitialScale;
		public GameObject Fragment;
		public bool IsLost;

		private Transform parent;

		public FragmentEntry(GameObject fragment, Transform parent)
		{
			this.parent = parent;
			Fragment = fragment;
			IsLost = false;
			InitialPosition = fragment.transform.localPosition;
			InitialRotation = fragment.transform.localRotation;
			InitialScale = fragment.transform.localScale;
		}

		public void ToggleLost()
		{
			IsLost = !IsLost;
			Rigidbody body = Fragment.GetComponent<Rigidbody>();
			body.isKinematic = !body.isKinematic;
			body.useGravity = !body.useGravity;
			string layerStr = IsLost ? "Env" : "Unit";
			Fragment.layer = LayerMask.NameToLayer(layerStr);
			Fragment.transform.parent = IsLost ? null : parent;
		}

		public void Reanimate()
		{
			if (IsLost)
			{
				ToggleLost();
				Fragment.transform.localPosition = InitialPosition;
				Fragment.transform.localRotation = InitialRotation;
				Fragment.transform.localScale = InitialScale;
			}
		}
	}
}
