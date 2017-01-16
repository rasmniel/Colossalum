using UnityEngine;
using System.Collections;

public class VictoryEffect : MonoBehaviour
{
	public ParticleSystem Accent;
	public Color AccentColor;
	void Start()
	{
		Accent.startColor = AccentColor;
	}

	public float Speed = 0.025f;
	private const float Limit = 10000;
	private int momentum = 0;
	void Update()
	{
		float rotation = momentum++ * Speed;
		momentum = (int)Mathf.Clamp(momentum, 0, Limit * Speed);
		transform.Rotate(0, rotation, 0);
		if (!Accent.isPlaying)
		{
			GameObject weapon = Combat.GetWeapon(GameModel.ShrineWeapon);
			if (weapon != null)
				Instantiate(weapon, Vector3.up * 5, transform.rotation);
			// Unlock the weapon and whiteout the player.
			GameModel.UnlockWeapon(GameModel.ShrineWeapon);
			GameModel.ShrineWeapon = null;
			PlayerData.Whiteout();
			Destroy(gameObject);
		}
	}
}
