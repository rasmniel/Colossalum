using UnityEngine;
using System.Collections;

[RequireComponent(typeof(TagPursuit))]
public class GoblinController : Mob
{
	public GameObject Heartbomb,
		Grip, 
		Head, 
		Torso, 
		Hips;
	public GameObject[]
		Weapons,
		Hats,
		Shirts, 
		Pants;

	private CharacterController control;
	private TagPursuit pursuit;
	private Animator anim;
	private Heartbomb heart;
	private GameObject w;

	void Start()
	{
		if (!_mobInitialized) InitializeMob();
		// Acquire components.
		control = GetComponent<CharacterController>();
		pursuit = GetComponent<TagPursuit>();
		anim = transform.GetChild(0).GetComponent<Animator>();
		heart = Heartbomb.GetComponent<Heartbomb>();

		// Initialize.
		arm();
		suit();
		preparePursuit();
	}

	[Range(0, 0.1f)]
	public float
		FaceAnimationFrequency = 0.02f,
		BodyAnimationFrequency = 0.005f;
	void Update()
	{
		LingerInvincibility();
		playEitherAnimation("Blink", "Leer", FaceAnimationFrequency);
		playEitherAnimation("Crouch", "Stretch", BodyAnimationFrequency);
		CullAnimations(anim);
		Die();
	}

	private void arm()
	{
		if (Weapons.Length > 0)
		{
			// Instantiate a weapon.
			GameObject weapon = null;
			while (weapon == null)
				weapon = InstantiateRandomEquipment(Weapons, Grip);
			w = weapon;
			
			// Adjust weapon.
			weapon.transform.rotation = Grip.transform.rotation;
			weapon.transform.localScale = transform.localScale;
			weapon.transform.parent = Grip.transform;
		}
	}

	private void suit()
	{
		if (Hats.Length > 0) // Instantiate a hat.
			InstantiateRandomEquipment(Hats, Head);
		if (Shirts.Length > 0) // Instantiate a shirt.
			InstantiateRandomEquipment(Shirts, Torso);
		if (Pants.Length > 0) // Instantiate pants.
			InstantiateRandomEquipment(Pants, Hips);
	}

	private GameObject InstantiateRandomEquipment(GameObject[] equipment, GameObject slot)
	{
		int index = Random.Range(-1, equipment.Length);
		// If -1, instantiate no equipment for this slot.
		if (index > -1)
		{
			// Instantiate a piece of equipment to the desired slot.
			GameObject obj = Instantiate(equipment[index], slot.transform.position, slot.transform.rotation) as GameObject;
			obj.transform.localScale = transform.localScale;
			obj.transform.parent = slot.transform;
			return obj;
		}
		return null;
	}

	private void preparePursuit()
	{
		pursuit.AggroEvent += () => anim.SetBool("Run", true);
		pursuit.TargetLostEvent += () => anim.SetBool("Run", false);
		pursuit.InCloseRange += attack;
		pursuit.ExitCloseRange += stopAttack;
	}

	private void attack()
	{
		anim.SetBool("Run", false);
		anim.SetBool("Attack", true);
		pursuit.SetPursuit(false);
	}

	private void stopAttack()
	{
		anim.SetBool("Run", true);
		anim.SetBool("Attack", false);
		pursuit.SetPursuit(true);
	}

	private void playEitherAnimation(string anim1, string anim2, float frequency)
	{
		anim.SetBool(anim1, false);
		anim.SetBool(anim2, false);
		if (!anim.GetBool("Run") && Random.value < frequency)
		{
			if (Random.value < 0.5f) // 50 % chance.
				anim.SetBool(anim1, true);
			else
				anim.SetBool(anim2, true);
		}
	}

	protected override bool IsAlive()
	{
		return _hp > 0;
	}

	private int _hp = 4;
	private const int DamageDeviation = 1;
	private const float
		BleedDeviation = 0.1f,
		DamageTypeModifier = 0.25f;
	protected override void GetWrecked(Weapon weapon)
	{
		// If dead, return.
		if (!IsAlive()) return;

		// Calculate damage equal to weapon type +/- 1;
		int damage = (int)weapon.Type;
		int minDmg = damage - DamageDeviation;
		int maxDmg = damage + DamageDeviation;
		// Add 1 to account for max value being exclusive.
		_hp -= Random.Range(minDmg, maxDmg + 1);

		// Calculate blood.
		float bleed = damage * DamageTypeModifier;
		float percentage = Random.Range(bleed - BleedDeviation, bleed + BleedDeviation);

		// Evaluate hp.
		if (!IsAlive()) perish(weapon, percentage);
		else heart.ExplodePercentage(false, percentage);
	}

	[Range(0, 1)]
	public float MaimChance;
	public GameObject[] Detatchables;
	private void perish(Weapon weapon, float bleedPercentage)
	{
		// Die...
		maim(w);
		Destroy(Grip);
		anim.enabled = false;
		control.enabled = false;
		gameObject.layer = LayerMask.NameToLayer("Env");

		// Enable rigidbody and capsule colliders.
		Rigidbody body = GetComponent<Rigidbody>();
		body.useGravity = true;
		body.isKinematic = false;
		CapsuleCollider[] colliders = GetComponents<CapsuleCollider>();
		foreach (CapsuleCollider collider in colliders)
			collider.enabled = true;
		
		// Calculate the chance for maiming limbs.
		float chance = (int)weapon.Type * MaimChance;
		foreach (GameObject bodypart in Detatchables)
			if (Random.value <= chance)
				maim(bodypart);
		
		// Explode the heart for good.
		heart.ExplodePercentage(true, bleedPercentage);
	}

	private void maim(GameObject bodypart)
	{
		// Remove parent and become part of environment.
		bodypart.transform.parent = null;
		bodypart.layer = LayerMask.NameToLayer("Env");

		// Activate rigidbody.
		Rigidbody body = bodypart.GetComponent<Rigidbody>();
		body.useGravity = true;
		body.isKinematic = false;

		// Activate all colliders.
		Collider[] colliders = bodypart.GetComponentsInChildren<Collider>();
		foreach (Collider collider in colliders)
			collider.enabled = true;
	}
}
