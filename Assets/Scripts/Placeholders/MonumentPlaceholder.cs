using UnityEngine;
using System.Collections;

public class MonumentPlaceholder : Placeholder
{
	public GameObject Unique;
	public GameObject[] DesiredMonuments;

	public static bool NewLevel = true;

	private static float _uniqueSpawnChance;
	private static bool _hasSpawnedUnique = false;
	protected override void SetDesiredValues()
	{
		if (NewLevel) resetValues();
		if (Unique != null && !_hasSpawnedUnique)
		{
			if (Random.value < getUniqueSpawnChance())
			{
				DesiredObject = Unique;
				SpawnChance = 1;
				_hasSpawnedUnique = true;
				return;
			}
		}
		DesiredObject = getRandom(DesiredMonuments);
	}
		
	private static void resetValues()
	{
		_hasSpawnedUnique = false;
		_uniqueSpawnChance = 0;
		NewLevel = false;
	}

	/// <summary>
	/// NOTE: With this algorithm, it is NOT certain that a Unique will spawn!
	/// Guaranteed instantiation of the Unique is predicated on each single tile triggering this method.
	/// In other words, each tile needs exactly one monument to call this method for it.
	/// </summary>
	private static float getUniqueSpawnChance()
	{
		if (GameModel.Tiles == 0) return 1;
		// Get fraction of tile count.
		_uniqueSpawnChance += (100 / GameModel.Tiles);
		// Divide by 100 to convert to decimal point number.
		// Eg. 25 => 0.25
		return _uniqueSpawnChance / 100;
	}
}
