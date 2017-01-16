using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
	public GameObject[] Scars;
	public GameObject Tattoo;

	private CharacterController control;
	public static bool Alive = true;

	void Start()
	{
		hideScars();
		PlayerData.SetTattoo(Tattoo);
		control = GetComponent<CharacterController>();
	}

	void Update()
	{
		if (Alive)
		{
			lingerInvincibility();
			sprint();
			move();
//			devResetLevel();
		}
		else
			gameObject.SetActive(false);
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Harm") && _invincibility <= 0)
		{
			PlayerData.PlayerWasHit();
			_invincibility = InvincibilityPeriod;
		}
	}

	private void hideScars()
	{
		PlayerData.SetScars(Scars);
		foreach (GameObject scar in Scars)
			scar.SetActive(false);
	}

	private const float InvincibilityPeriod = 5.0f;
	private float _invincibility = 0;
	private void lingerInvincibility()
	{
		if (_invincibility > 0) _invincibility--;
	}

	private bool
		_allowSprinting = true,
		_sprinting = false,
		_fatigued = false;
	private int _fatigue;
	public int FatigueLimit = 250;
	private void sprint()
	{
		// Determine whether the player is infused by crystal.
		int infusion = PlayerData.GetInfusion();
		// Debug.Log("Fatigue: " + _fatigue + ", Infusion: " + infusion);
		if (infusion > PlayerData.InfusionValue)
		{
			// If at least two has been consumed at once, sprint freely.
			_sprinting = true;
			_fatigued = false;
			_fatigue = 0;
			return;
		}
		else if (infusion > 0)
		{
			// If any crystals are active, relieve fatigue.
			_fatigued = false;
			_fatigue = 0;
		}
		// Assess whether sprinting is possible.
		bool doSprint = Input.GetAxis("Sprint") > 0;
		if (_allowSprinting) _sprinting = doSprint;
		if (doSprint) _allowSprinting = false;
		else _allowSprinting = true;

		// Is the player fatigued?
		if (_fatigue >= FatigueLimit)
		{
			_sprinting = false;
			_fatigued = true;
		}
		// If not fatigued and still sprinting, tire out.
		else if (_sprinting && !_fatigued)
			_fatigue++;
		else _sprinting = false;

		// Recover from fatigue.
		if (_fatigue > 0 && (_fatigued || !doSprint))
		{
			_fatigue -= 2;
			if (_fatigue <= 0)
				_fatigued = false;
		}
	}
		
	private void move()
	{
		// Get input.
		float vMove = Input.GetAxis("Vertical");
		float hMove = Input.GetAxis("Horizontal");

		// Determine direction.
		Vector3 movement = new Vector3(hMove, 0, vMove);
		movement = transform.TransformDirection(movement);

		// Adjust movement speed.
		movement = speed(movement);

		// Jump?
		movement = jump(movement);

		// Move!
		control.Move(movement * Time.deltaTime);
	}

	public float
		FatiguedSpeed = 3.0f,
		SprintSpeed = 9.0f,
		WalkSpeed = 5.0f;
	private Vector3 speed(Vector3 movement)
	{
		if (_fatigued)
			return movement * FatiguedSpeed;
		else if (_sprinting)
			return movement * SprintSpeed;
		return movement * WalkSpeed;
	}

	public float
		JumpPower = 7.0f,
		Gravity = 6.0f;
	public int JumpApex = 6;
	private bool
		_jumping = false,
		_allowJump = true;
	private int _takeOffPower;
	private Vector3 jump(Vector3 movement)
	{
		// Determine action.
		bool input = Input.GetAxis("Jump") > 0;
		if (control.isGrounded)
		{
			// Jump.
			if (_allowJump) _jumping = input;
			if (input) _allowJump = false;
		}
		else
		{
			// Fall.
			if (!input) _allowJump = true;
			movement.y -= Gravity;
		}
		// Act on the jumping condition.
		if (_takeOffPower > 0)
		{
			// Calculate the jump force and add it to the movement vector.
			if (_jumping && input)
			{
				float jumpForce = JumpPower - (JumpPower / _takeOffPower--);
				movement.y = jumpForce;
			}
			else _takeOffPower = JumpApex;
		}
		else
		{
			// Reset the character to full jumping potential.
			_takeOffPower = JumpApex;
			_jumping = false;
		}
		return movement;
	}

	private void devResetLevel()
	{
		if (Input.GetAxis("Reset") != 0)
			SceneManager.LoadScene(0);
	}
}
