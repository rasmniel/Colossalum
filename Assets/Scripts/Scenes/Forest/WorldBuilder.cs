using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class WorldBuilder : MonoBehaviour
{
	[Range(GameModel.MinTiles, GameModel.MaxTiles)]
	public int TileCount;
	public bool DisregardModel = false;
	public GameObject[] Tiles;
	public GameObject Mason;

	public const float TileSide = 100.0f;
	public const int WorldCorners = 4,
		North = 0,
		East = 1,
		South = 2,
		West = 3;

	// getTiles: X represents the tile type.
	// Y represents the copy index of that tile.
	private GameObject[,] getTiles;
	private int variations, copies, activeTiles;

	void Start()
	{
		PlayerData.Blackout(true);
		instantiateTiles();
		retainOrigin();
		buildMap();
		clearRemainders();
		// Recede from blackout when all mesh combiners have been executed.
		Invoke("Ready", 2);
	}

	void Ready()
	{
		PlayerData.Blackout(false);
	}

	private void instantiateTiles()
	{
		// Get amount of tiles needed from the game model, unless otherwise specified.
		if (DisregardModel)
		{
			GameModel.Tiles = TileCount;
			Debug.Log("Disregarding Model...");
		}
		TileCount = GameModel.Tiles;
		// Calculate copies needed of each tile.
		variations = Tiles.Length;
		copies = Mathf.CeilToInt(TileCount / Tiles.Length) + 5; // HACK! reduce this number when more tile-variety exists!
		getTiles = new GameObject[variations, copies];
		// Randomize list of tiles.
		Tiles = Tiles.OrderBy(x => Random.value).ToArray();
		// Instantiate tiles.
		for (int i = 0; i < variations; i++)
			for (int j = 0; j < copies; j++)
				getTiles[i, j] = Instantiate(Tiles[i], Vector3.zero, Quaternion.identity) as GameObject;
	}

	private void retainOrigin()
	{
		foreach (GameObject tile in getTiles)
			tile.SetActive(false);
		getTiles[0, 0].SetActive(true);
		activeTiles = 1;
	}

	private void buildMap()
	{
		// A ring has n * 8 tiles.
		int rings = Mathf.CeilToInt((float)TileCount / 8) + 1;
		// (+ 1): The extra ring is the walls around the tiles.
		int x, y;
		x = y = 0;
		for (int n = 1; n <= rings; n++)
		{
			int tiles = n * 8;
			bool evalX, inc;
			evalX = inc = true;
			for (int i = 0; i < tiles; i++)
			{
				if (evalX)
				{
					if (x < n && inc)
						x++;
					if (x > -n && !inc)
						x--;
					if (x == n || x == -n)
						evalX = false;
				}
				else
				{
					if (y < n && inc)
						y++;
					if (y > -n && !inc)
						y--;
					if (y == n || y == -n)
					{
						evalX = true;
						inc = !inc;
					}
				}
				Vector3 position = new Vector3(TileSide * x, 0, TileSide * y);
				if (activeTiles < TileCount)
					spawnRandomTileAt(position);
				else
					Instantiate(Mason, position, Quaternion.identity);
			}
		}
	}

	#region Tile methods.

	private void spawnRandomTileAt(Vector3 position)
	{
		if (activeTiles >= getTiles.Length)
			// Must not throw!
			throw new UnityException("Whoops - not enough tiles, breh!");
		
		GameObject tile;
		int direction;
		do
		{
			// Get a random tile that is not in use
			do tile = getRandomTile();
			while (tile.activeSelf);
			// Get appropriate tile direction according to other tiles around it.
			direction = getTileDirection(tile, position); // Choose(spawn) another tile, if this goes on for too long? HACK?
		} while (direction < 0);
		// Rotate and position the tile, so it fits it's neighbors.
		tile.transform.rotation = Quaternion.Euler(0, -90 * direction, 0); 
		tile.transform.position = position;
		// Activate tile.
		tile.SetActive(true);
		activeTiles++;
	}
	
	private GameObject getRandomTile()
	{
		return getTiles[Random.Range(0, variations), Random.Range(0, copies)];
	}

	private int getTileDirection(GameObject tile, Vector3 position)
	{
		Tile t = tile.GetComponent<Tile>();
		Tile[] adjecent = getTilesAround(position);
		// Attempt to fit the tile by.
		for (int i = 0; i < WorldCorners; i++)
		{
			// Rotate the tile for a new fit.
			t.SetRotation(i);
			// Evaulating it's available transitions compared to the adjecent tiles.
			if (doesTileMatch(t, adjecent[North], North) &&
			    doesTileMatch(t, adjecent[East], East) &&
			    doesTileMatch(t, adjecent[South], South) &&
			    doesTileMatch(t, adjecent[West], West))
				// If all transitions match, we've found a suitable direction!
				return i;
		}
		// If no suitable direction is found, reset rotation and return -1.
		t.SetRotation(0);
		return -1;
	}

	private Tile[] getTilesAround(Vector3 position)
	{
		return new Tile[] {
			getTileAt(position + Vector3.forward * TileSide), // North.
			getTileAt(position + Vector3.right * TileSide), // East.
			getTileAt(position - Vector3.forward * TileSide), //South.
			getTileAt(position - Vector3.right * TileSide), // West.
		};
	}

	private Tile getTileAt(Vector3 position)
	{
		Collider[] contact = Physics.OverlapSphere(position, 1);
		foreach (Collider c in contact)
		{
			Tile t = c.transform.parent.GetComponent<Tile>();
			if (t != null)
				return t;
		}
		return null;
	}

	private bool doesTileMatch(Tile t, Tile adjecent, int direction)
	{
		if (adjecent == null) return true; // try false?
		return t.ActualTransitions()[direction] == adjecent.ActualTransitions()[getOppositeDirection(direction)];
	}

	#endregion

	private int getOppositeDirection(int direction)
	{
		direction += (WorldCorners / 2);
		if (direction >= WorldCorners)
			direction -= WorldCorners;
		return direction;
	}

	private void clearRemainders()
	{
		foreach (GameObject tile in getTiles)
			if (!tile.activeSelf)
				Destroy(tile);
	}
}
