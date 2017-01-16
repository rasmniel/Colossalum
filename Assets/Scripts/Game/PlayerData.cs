using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerData : MonoBehaviour
{
	public GameObject 
		HitsTakenObj,
		KillsObj;
	public bool HideCursor = true,
		ShowHits = true,
		ShowKills = true;

	void Start()
	{
		blackout = GameObject.Find("Blackout").GetComponent<Image>();
		if (ShowKills)
			killsUI();
		if (ShowHits)
			hitsTakenUI();
		loadScore();
		Cursor.lockState = CursorLockMode.Confined;
		if (HideCursor) Cursor.visible = false;
		Invoke("GetScars", 0.5f);
	}

	void Update()
	{
		doInfusion();
		doWhiteout();
	}

	void GetScars()
	{
		PlayerData.scar();
	}

	private static GameObject[] playerScars;
	public static void SetScars(GameObject[] scars)
	{
		playerScars = scars;
	}

	private static GameObject levelTattoo;
	public static void SetTattoo(GameObject tattoo)
	{
		levelTattoo = tattoo;
		updateTattoo();
	}

	private static void updateTattoo()
	{
		if (levelTattoo == null) return;
		int level = GameModel.Tiles - GameModel.MinTiles;
		Transform tattoo = levelTattoo.transform;
		for (int i = 0; i < tattoo.childCount; i++)
			tattoo.GetChild(i).gameObject.SetActive(i <= level);
	}

	#region Player actions.

	public static void PlayerKill()
	{
		GameModel.Kills++;
		if (killsCounter != null)
			incrementDisplay(killsCounter);
		// If in the arena, count one kill towards objective.
		if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Colossalum"))
			FightGenerator.OneKill();
	}

	private const int Lives = 10;
	public static void PlayerWasHit()
	{
		GameModel.HitsTaken++;
		if (hitsTakenCounter != null)
			incrementDisplay(hitsTakenCounter);
		if (GameModel.HitsTaken >= Lives)
			playerDeath();
		scar();
	}

	public static void PlayerHeal(int healing)
	{
		for (int i = 0; i < healing; i++)
		{
			if (GameModel.HitsTaken > 0)
			{
				GameModel.HitsTaken--;
				decrementDisplay(hitsTakenCounter);
			}
		}
		healScars();
	}

	private static int _infusionLevel;
	public const int InfusionValue = 100;
	public static void PlayerInfuse()
	{
		_infusionLevel += InfusionValue;
	}

	public static int GetInfusion()
	{
		return _infusionLevel;
	}

	private void doInfusion()
	{
		if (_infusionLevel > 0)
			_infusionLevel--;
	}

	#endregion

	private static Image whiteout;
	public static void Whiteout()
	{
		whiteout = GameObject.Find("Whiteout").GetComponent<Image>();
	}

	private const float
		WhiteoutSpeed = 0.01f,
		AlphaCap = 2;
	private void doWhiteout()
	{
		if (whiteout != null)
		{
			Color c = whiteout.color;
			c.a = c.a + WhiteoutSpeed;
			whiteout.color = c;
			if (c.a >= AlphaCap)
			{
				SceneManager.LoadScene("Hub");
				if (!PlayerController.Alive)
					respawnPlayer();
			}
		}
	}

	private static Image blackout;
	public static void Blackout(bool doBlackout)
	{
		Color c = blackout.color;
		c.a = doBlackout ? AlphaCap : 0;
		blackout.color = c;
	}

	private static void scar()
	{
		// Is the player alive and has taken damage?
		int damage = GameModel.HitsTaken;
		if (damage <= 0) return;
		int hp = Lives - damage;
		if (hp <= 0) return;

		// Determine which scar will be activated next.
		int index = 0;
		if (damage == 1)
			index = 0;
		else if (damage == Lives - 1)
			index = playerScars.Length - 1;
		else if (damage % 2 == 0)
			index = (damage / 2) - 1;
		else if (Random.value < 0.5f)
			return; // If any...
		else
			index = damage / 2;

		// Activate the scar tissue!
		for (int i = 0; i <= index; i++)
			playerScars[i].SetActive(true);
	}

	private static void healScars()
	{
		// If health has risen, heal old scars.
		for (int i = GameModel.HitsTaken / 2; i < playerScars.Length; i++)
			playerScars[i].SetActive(false);
	}

	private static void playerDeath()
	{
		// Player drops head.
		GameObject head = GameObject.FindGameObjectWithTag("PlayerHead");
		head.layer = LayerMask.NameToLayer("Env");
		head.transform.parent = null;
		Rigidbody body = head.GetComponent<Rigidbody>();
		body.useGravity = true;
		body.isKinematic = false;
		body.AddForce(Vector3.forward);
		// End it!
		PlayerController.Alive = false;
		Whiteout();
	}

	private static void respawnPlayer()
	{
		PlayerController.Alive = true;
		GameModel.Tiles = 0;
		GameModel.Kills = 0;
		resetDisplay(killsCounter);
		GameModel.ShrineWeapon = string.Empty;
		GameModel.PlayerWeapon = string.Empty;
		GameModel.ResetInventory();
		GameModel.HitsTaken = 0;
		resetDisplay(hitsTakenCounter);
	}

	private static Text killsCounter;
	private void killsUI()
	{
		GameObject txtObj = Instantiate(KillsObj, Vector3.zero, Quaternion.identity) as GameObject;
		txtObj.transform.SetParent(transform, false);
		killsCounter = txtObj.GetComponent<Text>();
		killsCounter.rectTransform.anchorMin = new Vector2(0, 0);
		killsCounter.rectTransform.anchorMax = new Vector2(0, 0);
		killsCounter.rectTransform.anchoredPosition = new Vector2(150, 50);
	}

	private static Text hitsTakenCounter;
	private void hitsTakenUI()
	{
		GameObject txtObj = Instantiate(HitsTakenObj, Vector3.zero, Quaternion.identity) as GameObject;
		txtObj.transform.SetParent(transform, false);
		hitsTakenCounter = txtObj.GetComponent<Text>();
		hitsTakenCounter.rectTransform.anchorMin = new Vector2(0, 1);
		hitsTakenCounter.rectTransform.anchorMax = new Vector2(0, 1);
		hitsTakenCounter.rectTransform.anchoredPosition = new Vector2(150, -50);
	}

	private static void loadScore()
	{
		for (int i = 0; i < GameModel.Kills; i++)
			incrementDisplay(killsCounter);
		for (int i = 0; i < GameModel.HitsTaken; i++)
			incrementDisplay(hitsTakenCounter);
	}

	private const string TickString = "| ";
	private static void incrementDisplay(Text display)
	{
		if (display != null)
			display.text += TickString;
	}

	private static void decrementDisplay(Text display)
	{
		if (display != null)
			display.text = display.text.Substring(TickString.Length);
	}

	private static void resetDisplay(Text display)
	{
		if (display != null)
			display.text = string.Empty;
	}
}
