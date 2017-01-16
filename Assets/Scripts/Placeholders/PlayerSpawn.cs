using UnityEngine;
using System.Collections;

public class PlayerSpawn : Placeholder
{
	public GameObject Player;
	protected override void SetDesiredValues()
	{
		base.SetDesiredValues();
		if (GameObject.FindGameObjectWithTag("Player") == null)
			DesiredObject = Player;
	}
}
