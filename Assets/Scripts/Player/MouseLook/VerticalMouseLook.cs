using UnityEngine;

public class VerticalMouseLook : MouseLookBehaviour
{
	public float MaxX, MinX;
	private float _xRotation = 0;
	private static bool _inverted = false;
	protected override void RotateCamera()
	{
		if (transform.parent == null) return;
		if (Input.GetKeyDown(KeyCode.I)) _inverted = !_inverted;
		float rotation = getInput('Y') * Sensitivity;
		if (_inverted) rotation = -rotation;
		_xRotation += rotation;
		_xRotation = clampAngle(_xRotation, MinX, MaxX);
		transform.rotation = Quaternion.Euler(_xRotation, transform.parent.eulerAngles.y, 0.0f);
	}
}
