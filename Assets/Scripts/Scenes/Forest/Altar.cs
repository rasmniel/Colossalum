using UnityEngine;
using System.Collections;

public class Altar : MonoBehaviour
{
	public GameObject Spawn;
	private GameObject _item;
	private const float
		RotationSpeed = 5.0f,
		Slant = 25.0f;

	void Update()
	{
		Spawn.transform.Rotate(0, RotationSpeed, 0);
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Weapon"))
			dropItem();
	}

	private void dropItem()
	{
		if (_item != null)
		{
			Rigidbody body = _item.GetComponent<Rigidbody>();
			body.useGravity = true;
			body.isKinematic = false;
			_item.transform.parent = null;
			_item = null;
		}
	}

	public bool Transmute(GameObject[] toTransmute)
	{
		if (_item == null)
		{
			if (toTransmute.Length != 5)
				throw new UnityException("Inventory size must be exactly 5 - please do not attempt to Transmute any untransmutable arrays.");
			// Transmute and spawn the appropriate item.
			GameObject craftable = toTransmute[0].GetComponent<Item>().Craftable;
			if (craftable == null)
				throw new UnityException("Item must be craftable - Do not Transmute uncraftable items.");
			_item = Instantiate(craftable, Spawn.transform.position, Quaternion.identity) as GameObject;
			// Place the item on the altar.
			Rigidbody body = _item.GetComponent<Rigidbody>();
			body.useGravity = false;
			body.isKinematic = true;
			_item.transform.position = Spawn.transform.position;
			_item.transform.parent = Spawn.transform;
			_item.transform.rotation = Quaternion.Euler(0, 0, Slant);
			// Destroy items used for transmutation.
			foreach (GameObject item in toTransmute)
				Destroy(item);
			return true;
		}
		return false;
	}
}
