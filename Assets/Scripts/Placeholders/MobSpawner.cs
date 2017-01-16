using UnityEngine;
using System.Collections;

public class MobSpawner : Placeholder
{
	public GameObject[] DesiredMobs;

	private static GameObject player;
	private const float
		ActiveDistance = 100.0f,
		MobSpawnChance = 0.5f;
	void Start()
	{
		SpawnChance = MobSpawnChance;
		SetDesiredValues();
		DestroyIndicator();
	}

	void Update()
	{
		if (player != null)
		{
			float distance = Vector3.Distance(player.transform.position, transform.position);
			if (distance < ActiveDistance)
				InstantiateDesiredObject();
		}
		else
			player = GameObject.FindGameObjectWithTag("Player");
	}

	protected override void SetDesiredValues()
	{
		Mob m;
		do do // ...
			m = getRandom(DesiredMobs).GetComponent<Mob>();
			while(m.Rarity < Random.value);
		while(m.TileCountToSpawn > GameModel.Tiles);
		DesiredObject = m.gameObject;
		DesiredRotation = Quaternion.Euler(0, Random.value * 360, 0);
	}
}
