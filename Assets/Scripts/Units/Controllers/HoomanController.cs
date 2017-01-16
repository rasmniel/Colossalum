using UnityEngine;
using System.Collections;

public class HoomanController : MonoBehaviour
{
	public GameObject
		Heartbomb,
		Head;

	private Animator anim;
	
	void Start()
	{
		anim = GetComponent<Animator>();
	}

	void Update()
	{
		if (!dead)
		{
			cheer();
			excite();
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Weapon"))
			die();
	}

	[Range(0, 1)]
	public float CheerFrequency;
	private void cheer()
	{
		// Cancel any current cheering.
		if (anim.GetBool("Cheering"))
			anim.SetBool("Cheering", false);
		// Do another cheer, maybe?
		if (Random.value <= CheerFrequency)
			anim.SetBool("Cheering", true);
	}

	[Range(0, 1)]
	public float ExcitementChance = 0.99f;
	private int ExcitementCap = 3;
	private int _excitement = 0;
	private void excite()
	{
		if (Random.value > ExcitementChance)
		{
			if (Random.value > 0.5f)
				_excitement++;
			else _excitement--;
			_excitement = Mathf.Clamp(_excitement, 0, ExcitementCap);
			anim.SetInteger("ExcitementLevel", _excitement);
		}
	}

	private bool dead = false;
	private void die()
	{
		Head.layer = LayerMask.NameToLayer("Env");
		Rigidbody body = Head.GetComponent<Rigidbody>();
		body.useGravity = true;
		body.isKinematic = false;
		Heartbomb.GetComponent<Heartbomb>().Explode(true);
		GetComponent<CapsuleCollider>().enabled = false;
		// The hooman is now dead:
		anim.SetBool("Cheering", false);
		_excitement = 0;
		dead = true;
	}

}
