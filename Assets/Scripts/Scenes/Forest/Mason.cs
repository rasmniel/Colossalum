using UnityEngine;
using System.Collections;

public class Mason : MonoBehaviour
{
	public GameObject Wall, Cornerstone;

	void Start()
	{
		for (int i = 0; i < 4; i++)
			placeWall(i);
		Destroy(gameObject);
	}

	private void placeWall(int direction)
	{
		if (hasNeighborAt(direction))
		{
			// Turn the wall towards either of the four corners.
			Instantiate(Wall, transform.position, yRotation(direction));
			int diagonal = direction + 1;
			diagonal = diagonal < 4 ? diagonal : diagonal - 4;
			if (!hasNeighborAt(direction, diagonal))
				Instantiate(Cornerstone, transform.position, yRotation(direction));
		}
	}

	private Quaternion yRotation(int direction)
	{
		float rotation = direction * 90; // Degrees.
		return Quaternion.Euler(0, rotation, 0);
	}

	#region Has neighbor booleans

	private readonly Vector3[] AdjecentSpaces = new Vector3[] {
		Vector3.forward * WorldBuilder.TileSide, // North.
		Vector3.right * WorldBuilder.TileSide, // East.
		Vector3.forward * -WorldBuilder.TileSide, // South.
		Vector3.right * -WorldBuilder.TileSide // West.
	};
	private bool hasNeighborAt(int direction)
	{
		Vector3 neighbor = transform.position + AdjecentSpaces[direction];
		return hasNeighborAt(neighbor);
	}

	private bool hasNeighborAt(int direction1, int direction2)
	{
		Vector3 neighbor = transform.position + AdjecentSpaces[direction1] + AdjecentSpaces[direction2];
		return hasNeighborAt(neighbor);
	}

	private bool hasNeighborAt(Vector3 position)
	{
		Collider[] contact = Physics.OverlapSphere(position, 1);
		if (contact.Length != 0)
			return true;
		return false;
	}

	#endregion
}
