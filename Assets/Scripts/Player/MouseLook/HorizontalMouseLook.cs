using UnityEngine;

public class HorizontalMouseLook : MouseLookBehaviour
{
	void Start()
	{
		_yRotation = transform.eulerAngles.y;
	}

	private float _yRotation;
	protected override void RotateCamera()
	{
		_yRotation += getInput('X') * Sensitivity;
		_yRotation = clampAngle(_yRotation, MinY, MaxY);
		transform.rotation = Quaternion.Euler(0.0f, _yRotation, 0.0f);
	}
}
