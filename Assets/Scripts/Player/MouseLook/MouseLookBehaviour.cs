using UnityEngine;

public abstract class MouseLookBehaviour : MonoBehaviour
{
	void Update()
	{
		if (PlayerController.Alive)
			RotateCamera();
	}

	[Range(-1, 1)]
	public float Sensitivity;
	protected abstract void RotateCamera();

	[Range(1, 10)]
	public float JoystickBoost = 2.0f;
	[Range(0, 1)]
	public float DeadZone = 0.35f;
	protected float getInput(char axis)
	{
		// Keep Unity's Input-Manager at 0 deadzone and 1 sensitivity.
		// Manage input axis for rotating camera.
		float joy = Input.GetAxis("Joy " + axis);
		float mouse = Input.GetAxis("Mouse " + axis);
		float input = 0;
		// Determine if input is higher or lower than 0.
		if (joy < 0 || mouse < 0)
		{
			// Choose input; mouse or joystick, and correct value.
			input = Mathf.Min(mouse, joy);
			bool isJoy = input == joy;
			input = input < -DeadZone ? input : 0;
			if (isJoy) input *= JoystickBoost;
		}
		else if (joy > 0 || mouse > 0)
		{
			input = Mathf.Max(mouse, joy);
			bool isJoy = input == joy;
			input = input > DeadZone ? input : 0;
			if (isJoy) input *= JoystickBoost;
		}
		return input;
	}

	protected const float
		MinY = -360.0f,
		MaxY = 360.0f;
	protected float clampAngle(float angle, float min, float max)
	{
		// Fit the angle of the rotation within a 360 degree circle.
		if (angle < MinY) angle += 360.0f;
		if (angle > MaxY) angle -= 360.0f;
		return Mathf.Clamp(angle, min, max);
	}
}
