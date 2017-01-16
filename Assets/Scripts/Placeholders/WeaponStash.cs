using UnityEngine;
using System.Collections;

public class WeaponStash : Placeholder
{
	protected override void SetDesiredValues()
	{
		foreach (string unlocked in GameModel.UnlockedWeapons)
			if (DesiredObject.name == unlocked)
				return;
		DesiredObject = null;
	}
}
