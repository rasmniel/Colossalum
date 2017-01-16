using UnityEngine;
using System;
using System.Collections.Generic;

public class GameModel
{
	public static int Kills { get; set; }
	public static int HitsTaken { get; set; }

	public static List<string> UnlockedWeapons = new List<string>();
	public static void UnlockWeapon(string weapon)
	{
		UnlockedWeapons.Add(weapon);
	}

	public const int
		MinTiles = 4,
		MaxTiles = 9;
	private static int _tiles = 0;
	public static int Tiles
	{
		get { return _tiles; }
		set
		{
			if (value == 0) _tiles = value;
			else _tiles = Mathf.Clamp(value, MinTiles, MaxTiles);
		}
	}

	public static string[] Inventory = new string[Offhand.Capacity];
	public static void ResetInventory()
	{
		Inventory = new string[Offhand.Capacity];
	}

	private static string _playerWeapon;
	public static string PlayerWeapon
	{
		get { return _playerWeapon; } 
		set	{ _playerWeapon = removeCloneString(value); }
	}

	private static string _shrineWeapon;
	public static string ShrineWeapon
	{
		get { return _shrineWeapon; } 
		set	{ _shrineWeapon = removeCloneString(value); }
	}

	private static string cloneString = "(Clone)";
	private static string removeCloneString(string str)
	{
		if (str == null) return string.Empty;
		while (str.EndsWith(cloneString))
			str = str.Substring(0, str.Length - cloneString.Length).Trim();
		return str;
	}
}