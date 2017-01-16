using UnityEngine;
using System.Collections;

[RequireComponent(typeof(TagPursuit))]
public class SkeletonController : Fragmental
{
	public GameObject Grip, Light;
	public GameObject[] Weapons;

	private CharacterController control;
	private TagPursuit pursuit;
	private Animator anim;
	private GameObject weapon;

	void Start()
	{
		if (!_mobInitialized) InitializeMob();
		control = GetComponent<CharacterController>();
		pursuit = GetComponent<TagPursuit>();
		anim = GetComponent<Animator>();
		arm();
		InitializeFragments();
		preparePursuit();
		OnAnimateStateChange += toggleActive;
	}

	void Update()
	{
		CullAnimations(anim);
		LingerInvincibility();
		Die();
	}

	private void arm()
	{
		if (Weapons != null && Weapons.Length > 0)
		{
			int index = Random.Range(0, Weapons.Length);
			GameObject chosenWeapon = Weapons[index];
			weapon = Instantiate(chosenWeapon, Grip.transform.position, Grip.transform.rotation) as GameObject;
			weapon.transform.parent = Grip.transform;
		}
	}
	
	private void preparePursuit()
	{
		pursuit.TargetLostEvent += () => anim.SetBool("Run", false);
		pursuit.AggroEvent += () => anim.SetBool("Run", true);
		pursuit.EnterMeleeEvent += () => anim.SetBool("Attack", true);
		pursuit.LeaveMeleeEvent += () => anim.SetBool("Attack", false);
		pursuit.InCloseRange += () => anim.SetBool("Run", false);
		pursuit.ExitCloseRange += () => anim.SetBool("Run", true);
	}

	private void toggleActive(bool active)
	{
		Light.SetActive(active);
		pursuit.SetPursuit(active);
		control.enabled = active;
		Grip.GetComponent<BoxCollider>().enabled = active;
		if (!active)
		{
			anim.SetBool("Run", false);
			anim.SetBool("Attack", false);
		}
	}
}
