using System;
using UnityEngine;

public class Combat : MonoBehaviour
{
	private static Weapon _w;
	private GameObject _chosenWeapon = null;
	private Collider edgeCollider;
	private Animator anim;
	private const float WeaponRotation = 70.0f;

	void Start()
	{
		anim = GetComponent<Animator>();
		if (!string.IsNullOrEmpty(GameModel.PlayerWeapon))
			spawn(GetWeapon(GameModel.PlayerWeapon));
	}

	void Update()
	{
		attemptAttack();
		attemptToss();
//		devSpawnIn();
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Weapon"))
			equip(other.gameObject);
	}

	public GameObject[] Weapons;
	private static GameObject[] staticWeapons;
	public static GameObject GetWeapon(string name)
	{
		staticWeapons = GameObject.FindGameObjectWithTag("Player").GetComponent<Combat>().Weapons;
		if (!string.IsNullOrEmpty(name))
			foreach (GameObject weapon in staticWeapons)
				if (name.StartsWith(weapon.name))
					return weapon;
		return null;
	}

	public static GameObject GetPlayerWeapon()
	{
		if (_w == null) return null;
		return _w.gameObject;
	}

	public static void PlayerDropWeapon()
	{
		// Access toss method from object reference.
		Combat combat = GameObject.FindGameObjectWithTag("Player").GetComponent<Combat>();
		combat.doToss(Vector3.zero);
	}

	public GameObject Grip;
	private void spawn(GameObject weapon)
	{
		if (_w == null && weapon != null)
			weapon = Instantiate(weapon, Grip.transform.position, Grip.transform.rotation) as GameObject;
		equip(weapon);
	}

	private void equip(GameObject weapon)
	{
		// Equip the weapon, if none is wielded.
		if (_w == null)
		{
			_w = weapon.GetComponent<Weapon>();
			// If a weapon was equipped, adjust it firmly in your hand.
			if (_w != null)
			{
				edgeCollider = weapon.GetComponent<CapsuleCollider>();
				_w.SetLootable(false);
				weapon.transform.rotation = Grip.transform.rotation;
				weapon.transform.position = Grip.transform.position;
				weapon.transform.parent = Grip.transform;
				weapon.transform.Rotate(0, WeaponRotation, 0);
				// Change weapon variables each time a new one is picked up.
				_chosenWeapon = weapon;
				GameModel.PlayerWeapon = weapon.name;
			}
		}
	}

	#region Attack methods.

	private void attemptAttack()
	{
		// Get player's attack input.
		bool doAttack = Input.GetAxis("Attack") > 0;
		// Get weapon's attack enum, or use default.
		string attack = attackType();
		bool isActive = anim.GetCurrentAnimatorStateInfo(0).IsName(attack);
		bool setActive = false;
		if (doAttack && !isActive)
		{
			setActive = true;
			anim.SetBool(BaseAttack, false);
		}
		else if (!doAttack)
			resetAttackAnimation();
		anim.SetBool(attack, setActive);
		colliderCheck();
	}

	private const string BaseAttack = "Punch";
	private string attackType()
	{
		return _w != null ? _w.Attack.ToString() : BaseAttack;
	}

	[HideInInspector]
	public bool Attacking;
	private void colliderCheck()
	{
		if (edgeCollider != null)
		{
			if (edgeCollider.enabled != Attacking)
				edgeCollider.enabled = Attacking;
		}
	}

	#endregion

	private void resetAttackAnimation()
	{
		foreach (string attack in Enum.GetNames(typeof(Weapon.AttackMotion)))
			anim.SetBool(attack, false);
	}

	#region Toss methods

	private const string TossAnim = "Toss";
	private void attemptToss()
	{
		// Is the player throwing their weapon?
		bool doToss = Input.GetAxis(TossAnim) > 0;
		bool activeAttack = anim.GetCurrentAnimatorStateInfo(0).IsName(attackType());
		bool activeToss = anim.GetCurrentAnimatorStateInfo(0).IsName(TossAnim);
		bool setActive = false;
		if (doToss && activeAttack && !activeToss && _w != null)
		{
			anim.Play(TossAnim);
			setActive = true;
			anim.SetBool(BaseAttack, false);
		}
		anim.SetBool(TossAnim, setActive);
	}

	public float TossForce = 800.0f;
	public void Toss()
	{
		Vector3 forwardForce = Camera.main.transform.forward * TossForce;
		Vector3 upwardForce = Vector3.up * (TossForce / 2);
		doToss(forwardForce + upwardForce);
	}

	private void doToss(Vector3 force)
	{
		if (_w != null)
		{
			// Unequip the weapon.
			_w.SetLootable(true);
			edgeCollider.enabled = true;
			edgeCollider = null;

			// Toss the weapon!
			_w.gameObject.transform.parent = null;
			_w.GetComponent<Rigidbody>().AddForce(force);
			_w = null;
		}
	}

	#endregion

	private void devSpawnIn()
	{
		if (Input.GetAxis("Spawn") > 0)
			spawn(_chosenWeapon);
	}
}