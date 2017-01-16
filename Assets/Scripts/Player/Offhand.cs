using UnityEngine;
using System.Collections.Generic;

public class Offhand : MonoBehaviour
{
	public GameObject Grip;
	public ParticleSystem Sparkle;
	public GameObject[] Items;

	private Animator anim;
	private static GameObject[] _inventory;
	public const int Capacity = 5;

	void Start()
	{
		anim = GetComponent<Animator>();
		_inventory = new GameObject[Capacity];
		refillInventory();
	}

	void Update()
	{
		attemptConsume();
		attemptDrop();
		altarSparkle();
	}

	private GameObject _nearbyAltar = null;
	private string _currentType = null;
	void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Altar"))
			_nearbyAltar = other.gameObject;
		else if (other.CompareTag("Pickup"))
			attemptPickup(other.gameObject);
	}

	void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Altar"))
			_nearbyAltar = null;
	}

	private void refillInventory()
	{
		string[] inventoryToCopy = GameModel.Inventory;
		GameModel.Inventory = new string[Capacity];
		foreach (string item in inventoryToCopy)
		{
			GameObject itemObject = getItem(item);
			if (itemObject != null)
				attemptPickup(Instantiate(itemObject, Vector3.zero, Quaternion.identity) as GameObject);
		}
	}

	private GameObject getItem(string itemType)
	{
		foreach (GameObject item in Items)
			if (item.GetComponent<Item>().Type == itemType)
				return item;
		return null;
	}

	#region Consume methods.

	private const string
		MainAnim = "Consume",
		DropAnim = "Drop";
	private void attemptConsume()
	{
		// Look for player input.
		bool doAlt = Input.GetAxis("Alternative") > 0;
		if (_nearbyAltar != null && doAlt)
		{
			// Do not consume if near altar!
			if (canTransmute())
			{
				// Transmute!
				anim.Play(DropAnim);
				anim.SetBool(DropAnim, false);
			}
		}
		else if (_nextIndex > 0)
		{
			// If not near altar or can't transmute, attempt to consume inventory.
			bool isActive = anim.GetCurrentAnimatorStateInfo(1).IsName(MainAnim);
			bool setActive = false;
			if (doAlt && !isActive)
				setActive = true;
			anim.SetBool(MainAnim, setActive);
		}
	}

	public void ConsumeInventory()
	{
		_nextIndex--;
		consume(_inventory[_nextIndex]);
		_inventory[_nextIndex] = null;
		if (_nextIndex == 0)
			resetInventory();
	}

	private const int
		Berry = 1,
		Potion = 10;
	private void consume(GameObject itemObject)
	{
		if (itemObject != null)
		{
			Item item = itemObject.GetComponent<Item>();
			switch (item.Type)
			{
			case "Berry":
				PlayerData.PlayerHeal(Berry);
				break;
			case "Potion":
				PlayerData.PlayerHeal(Potion);
				break;
			case "Crystal":
				PlayerData.PlayerInfuse();
				break;
			case "Teleport":
				PlayerData.Whiteout();
				break;
			}
			Destroy(itemObject);
		}
	}

	#endregion Consume

	#region Drop methods.

	private void attemptDrop()
	{
		bool doDrop = Input.GetAxis("Toss") > 0;
		bool activeMain = anim.GetCurrentAnimatorStateInfo(1).IsName(MainAnim);
		bool activeDrop = anim.GetCurrentAnimatorStateInfo(1).IsName(DropAnim);
		bool setActive = false;
		if (doDrop && activeMain && !activeDrop)
		{
			anim.Play(DropAnim);
			setActive = true;
		}
		anim.SetBool(DropAnim, setActive);
	}

	public void DropInventory()
	{
		if (_nearbyAltar != null && canTransmute())
		{
			Altar altar = _nearbyAltar.GetComponent<Altar>();
			if (altar.Transmute(_inventory))
				resetInventory();
		}
		else
		{
			for (int i = 0; i < _inventory.Length; i++)
				if (_inventory[i] != null)
					drop(_inventory[i]);
			resetInventory();
		}
	}

	private const float DropForce = 4.0f;
	private void drop(GameObject item)
	{
		if (item != null)
		{
			Rigidbody body = item.GetComponent<Rigidbody>();
			body.useGravity = true;
			body.isKinematic = false;
			Vector3 force = (Camera.main.transform.forward * DropForce) + (-Camera.main.transform.right * DropForce);
			body.AddForce(force, ForceMode.Impulse);
			item.transform.parent = null;
			item.layer = LayerMask.NameToLayer("Env");
			item.GetComponent<Item>().ToggleScale(true);
		}
	}

	#endregion Drop

	#region Pickup methods.

	private bool _carryingSingle = false;
	private void attemptPickup(GameObject itemObject)
	{
		Item item = itemObject.GetComponent<Item>();
		if (_nextIndex < _inventory.Length)
		{
			if (_nextIndex == 0)
				pickupSingle(item);
			if (!_carryingSingle)
				pickupMultiples(item);
		}
	}

	private void pickupSingle(Item item)
	{
		if (item.isSingle)
		{
			_carryingSingle = true;
			pickup(item.gameObject);
		}
	}

	private void pickupMultiples(Item item)
	{
		if (_currentType == null || item.Type == _currentType)
		{
			pickup(item.gameObject);
			rearrangeInventory();
		}
	}

	private int _nextIndex = 0;
	private void pickup(GameObject itemObject)
	{
		// Activate the pickup's rigidbody to make it fall.
		Rigidbody body = itemObject.GetComponent<Rigidbody>();
		body.useGravity = false;
		body.isKinematic = true;
		// Claim the pickup with the offhand.
		itemObject.layer = LayerMask.NameToLayer("Eq");
		int index = _nextIndex++;
		Transform slot;
		if (_carryingSingle)
			slot = Grip.transform;
		else
			slot = Grip.transform.GetChild(index);
		itemObject.transform.parent = slot;
		itemObject.transform.position = slot.position;
		_inventory[index] = itemObject;
		Item item = itemObject.GetComponent<Item>();
		GameModel.Inventory[index] = item.Type;
		_currentType = item.Type;
		item.ToggleScale(false);
	}

	private void rearrangeInventory()
	{
		GameObject[] newInventory = new GameObject[5];
		int children = 0;
		// Tally children in grip slots.
		foreach (Transform slot in Grip.transform)
		{
			// If there's exactly 1 item in the slot.
			if (slot.childCount == 1)
			{
				// Keep it.
				Transform item = slot.GetChild(0);
				newInventory[children++] = item.gameObject;
			}
		}
		// If counted a different amount of children than expected.
		if (children != _nextIndex)
		{
			// Rearrange inventory items.
			for (int i = 0; i < children; i++)
			{
				// One by one.
				GameObject item = newInventory[i];
				Transform slot = Grip.transform.GetChild(i);
				item.transform.parent = slot;
				item.transform.position = slot.position;
			}
			// Keep new values in instance variables.
			_nextIndex = children;
			_inventory = newInventory;
		}
	}

	#endregion Pickup

	private void altarSparkle()
	{
		ParticleSystem.EmissionModule em = Sparkle.emission;
		if (!em.enabled && _nearbyAltar != null && canTransmute())
		{
			// If the player can transmute and is close to an altar, start sparkle.
			Item item = _inventory[0].GetComponent<Item>();
			Sparkle.startColor = item.Color;
			em.enabled = true;
		}
		else em.enabled = false;
	}

	private void resetInventory()
	{
		// Reset the inventory to empty.
		GameModel.Inventory = new string[Capacity];
		_inventory = new GameObject[Capacity];
		_currentType = null;
		_carryingSingle = false;
		_nextIndex = 0;
	}

	private bool canTransmute()
	{
		// If the player is holding no items, the transmutation is impossible.
		if (_nextIndex == 0) return false;
		// Does the player's item have a craftable recipe?
		bool canCraft = _inventory[0].GetComponent<Item>().Craftable != null;
		// Does the player have a full inventory to transmute?
		return canCraft && _nextIndex == _inventory.Length;
	}
}
