using UnityEngine;
using System.Collections;

// Rigidbody for falling, box collider for landing, capsule collider for detection.
[RequireComponent(typeof(Rigidbody), typeof(BoxCollider), typeof(CapsuleCollider))]
public class Weapon : MonoBehaviour
{
	public enum WeaponType
	{
		Zero,
		Light,
		Medium,
		Heavy,
		Devastation}

	;

	public enum AttackMotion
	{
		Punch,
		Stab,
		Chop,
		Jab,
		Sweep}

	;
	
	public WeaponType Type;
	public AttackMotion Attack;
	public GameObject Layout;

	private bool isLoot = true;
	private float _reversionTimer;
	void Update()
	{
		if (_reversionTimer > 0) _reversionTimer--;
		else if (isLoot) gameObject.layer = LayerMask.NameToLayer("Loot");
	}

	private const float reversionTime = 100.0f;
	public void SetLootable(bool lootable)
	{
		isLoot = lootable;
		// Alter the weapon's behaviour in the world.
		Rigidbody body = GetComponent<Rigidbody>();
		body.useGravity = isLoot;
		body.isKinematic = !isLoot;
		GetComponent<BoxCollider>().enabled = isLoot;
		// Determine whether the weapon has just been thrown or picked up.
		if (!isLoot) gameObject.layer = LayerMask.NameToLayer("Eq");
		else _reversionTimer = reversionTime;
	}
}
