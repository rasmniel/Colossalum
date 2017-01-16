using UnityEngine;
using System.Collections;

public class BreakablePlaceholder : Placeholder
{
	public GameObject[] DesiredBreakables;

	protected override void SetDesiredValues()
	{
		Breakable b;
		do do // ...
				b = getRandom(DesiredBreakables).GetComponent<Breakable>();
			while(b.Rarity < Random.value);
		while(b.TileCountToSpawn > GameModel.Tiles);
		DesiredObject = b.gameObject;
	}
}
