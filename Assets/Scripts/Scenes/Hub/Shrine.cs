using UnityEngine;
using System.Collections;

public class Shrine : MonoBehaviour
{
	public GameObject 
		Item, 
		Glow,
		Exit;

	private GameObject _w, arenaExit;
	private Animation anim;

	void Start()
	{
		anim = Exit.GetComponent<Animation>();
		fetchWeapon();
		arenaExit = Exit.transform.Find("Exit").gameObject;
		arenaExit.SetActive(false);
	}

	void Update()
	{
		spin();
		if (_collisionTimer > 0) _collisionTimer--;
	}

	private const float CollisionTime = 10.0f;
	private float _collisionTimer = 0;
	void OnTriggerEnter(Collider other)
	{
		if (_collisionTimer <= 0)
		{
			swapItem(other.gameObject);
		}
	}

	private void fetchWeapon()
	{
		if (!string.IsNullOrEmpty(GameModel.ShrineWeapon))
		{
			GameObject prefab = Combat.GetWeapon(GameModel.ShrineWeapon);
			GameObject weapon = Instantiate(prefab, transform.position, Quaternion.identity) as GameObject;
			place(weapon);
		}
	}

	public float SpinSpeed = 3.0f;
	private void spin()
	{
		Item.transform.Rotate(Vector3.up * SpinSpeed);
	}

	private void swapItem(GameObject other)
	{
		if (_w == null && other.CompareTag("Weapon"))
		{
			GameObject playerWeapon = Combat.GetPlayerWeapon();
			if (playerWeapon != null && other.GetInstanceID() == playerWeapon.GetInstanceID())
				Combat.PlayerDropWeapon();
			place(other);
			_collisionTimer = CollisionTime;
		}
		else if (_w.GetInstanceID() != other.gameObject.GetInstanceID() && other.CompareTag("Weapon"))
			drop();
	}
		
	public float Slant = 25.0f;
	private const string AnimationName = "ArenaDoors";
	private void place(GameObject weapon)
	{
		// Place weapon in the shrine.
		_w = weapon;
		GameModel.ShrineWeapon = weapon.name;
		Rigidbody body = weapon.GetComponent<Rigidbody>();
		body.useGravity = false;
		body.isKinematic = true;
		weapon.transform.parent = Item.transform;
		weapon.transform.position = Item.transform.position;
		weapon.transform.rotation = Quaternion.Euler(0, 0, Slant);

		// Apply effect and open door.
		Glow.SetActive(true);
		arenaExit.SetActive(true);
		if (!anim.isPlaying) anim.Play();
		AnimationState state = anim[AnimationName];
		state.time = 0;
		state.speed = 1;
	}

	private void drop()
	{
		// Drop weapon from the shrine.
		GameModel.ShrineWeapon = string.Empty;
		Rigidbody body = _w.GetComponent<Rigidbody>();
		body.useGravity = true;
		body.isKinematic = false;
		_w.transform.parent = null;
		_w = null;
	
		// Remove effect and close door.
		Glow.SetActive(false);
		arenaExit.SetActive(false);
		AnimationState state = anim[AnimationName];
		state.time = 1;
		state.speed = -1;
	}
}
