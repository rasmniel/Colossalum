using UnityEngine;
using System.Collections;

public class FightGenerator : MonoBehaviour
{
	public GameObject VictoryEffect;
	public GameObject[] Weapons;

	private static GameObject victory;
	private GameObject chosenFight;
	private static int mobCount;

	void Start()
	{
		victory = VictoryEffect;
		// Force shrine weapon in arena.
		GameModel.PlayerWeapon = GameModel.ShrineWeapon;
		pickFight();
		Invoke("countMobs", 0.5f);
	}

	public static void OneKill()
	{
		mobCount--;
		if (mobCount <= 0)
			Instantiate(victory, Vector3.zero, Quaternion.identity);
	}

	private void pickFight()
	{
		string shrineWeapon = GameModel.ShrineWeapon;
		foreach (GameObject weapon in Weapons)
		{
			if (weapon.name == shrineWeapon)
			{
				GameObject layout = weapon.GetComponent<Weapon>().Layout;
				chosenFight = Instantiate(layout, Vector3.zero, Quaternion.identity) as GameObject;
			}
		}
	}

	private void countMobs()
	{
		mobCount = 0;
		foreach (Transform t in chosenFight.transform)
			if (t.CompareTag("Mob"))
				mobCount++;
	}
}
