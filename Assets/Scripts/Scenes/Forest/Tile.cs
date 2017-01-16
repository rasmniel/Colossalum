using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour
{
	public bool[] Transitions = new bool[4];
	private bool[] _rotatedTransitions;
	
	void Start()
	{
		if (Transitions.Length != 4)
			throw new UnityException("Length of transition array must be equal to 4, representing: North(0), East(1), South(2), West(3)");
	}

	public void SetRotation(int direction)
	{
		if (direction == 0)
			_rotatedTransitions = null;
		else
		{
			_rotatedTransitions = new bool[4];
			for (int i = 0; i < 4; i++)
			{
				int newDirection = direction + i;
				newDirection = newDirection < 4 ? newDirection : newDirection - 4;
				_rotatedTransitions[i] = Transitions[newDirection];
			}
		}
	}

	public bool[] ActualTransitions()
	{	
		return _rotatedTransitions ?? Transitions;
	}
}
