using UnityEngine;
using System.Collections;

public abstract class Mob : MonoBehaviour
{
	public int TileCountToSpawn;
	[Range(0, 1)]
	public float Rarity = 1;

	protected static GameObject Player;

	protected bool _mobInitialized = false;
	protected void InitializeMob()
	{
		Player = GameObject.FindGameObjectWithTag("Player");
		setSize();
		_mobInitialized = true;
	}

	void Start()
	{
		if (!_mobInitialized) InitializeMob();
	}

	void Update()
	{
		LingerInvincibility();
		Die();
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Weapon"))
			takeHit(other.gameObject);
	}

	[Range(0, 1)]
	public float SizeDeviation = 0.05f;
	public bool AbsoluteDeviation = false;
	private const float SuperSizeChance = 0.025f;
	private void setSize()
	{
		float size;
		if (AbsoluteDeviation)
			size = 1 + SizeDeviation;
		else
			size = Random.Range(1 - SizeDeviation, 1 + SizeDeviation);
		if (Random.value < SuperSizeChance)
			size += size;
		transform.localScale = Vector3.one * size;
	}

	private void takeHit(GameObject other)
	{
		if (IsAlive() && _invincibility <= 0)
		{
			GetWrecked(other.GetComponent<Weapon>());
			_invincibility = InvincibilityPeriod;
		}
	}

	private bool _killed = false;
	protected void Die()
	{
		if (!IsAlive() && !_killed)
		{
			_killed = true;
			PlayerData.PlayerKill();
		}
	}

	private const float InvincibilityPeriod = 5.0f;
	private float _invincibility = 0;
	protected void LingerInvincibility()
	{
		if (_invincibility > 0) _invincibility--;
	}

	private const float cullingDistance = 10.0f;
	protected void CullAnimations(Animator anim)
	{
		float distanceFromPlayer = Vector3.Distance(Player.transform.position, transform.position);
		if (distanceFromPlayer > cullingDistance)
			anim.cullingMode = AnimatorCullingMode.CullCompletely;
		else
			anim.cullingMode = AnimatorCullingMode.AlwaysAnimate;
	}

	protected abstract bool IsAlive();

	protected abstract void GetWrecked(Weapon weapon);
}
