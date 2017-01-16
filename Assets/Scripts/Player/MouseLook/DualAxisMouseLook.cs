using UnityEngine;

public class DualAxisMouseLook : MouseLookBehaviour
{
	void Start()
	{
		_yRotation = transform.eulerAngles.y;
	}

	[Range(-360, 360)]
	public float
		MaxX, MinX;
	[Range(-1, 1)]
	public float
		SensitivityY;
	private float
		_xRotation = 0,
		_yRotation = 0;
	protected override void RotateCamera()
	{
		_xRotation += -Input.GetAxis("Mouse Y") * SensitivityY;
		_xRotation = clampAngle(_xRotation, MinX, MaxX);
		_yRotation += Input.GetAxis("Mouse X") * Sensitivity;
		_yRotation = clampAngle(_yRotation, MinY, MaxY);
		transform.rotation = Quaternion.Euler(_xRotation, _yRotation, 0.0f);
	}
}
