using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class TagPursuit : MonoBehaviour
{
	public string ToPursue = "Player";
	public float
		MovementSpeed = 100.0f,
		AggroRange = 20.0f,
		MeleeRange = 8.0f,
		CloseRange = 2.0f;
    
	private GameObject target;
	private CharacterController control;
	private bool _doPursuit = true;

	void Start()
	{
		control = GetComponent<CharacterController>();
	}

	void FixedUpdate()
	{
		pursue();
	}

	public void SetPursuit(bool pursue)
	{
		_doPursuit = pursue;
	}

	private float distanceFromTarget()
	{
		return Vector3.Distance(target.transform.position, transform.position);
	}

	private void pursue()
	{
		if (target == null)
			getTarget();
		else if (distanceFromTarget() < AggroRange)
			doPursuit();
		else
		{
			target = null;
			onTargetLost();
		}
	}

	private void getTarget()
	{
		GameObject tagged = GameObject.FindGameObjectWithTag(ToPursue);
		if (tagged != null)
		{
			float distance = Vector3.Distance(tagged.transform.position, transform.position);
			if (distance < AggroRange)
			{
				target = tagged;
				onAggro();
			}
		}
	}

	private bool _inMeleeRange, _inCloseRange;
	private void doPursuit()
	{
		// Require character controller to pursue.
		if (!control.enabled) return;

		// Pursue target.
		transform.LookAt(target.transform);
		float distance = Vector3.Distance(target.transform.position, transform.position);
		if (distance > CloseRange)
		{
			doMove();
			if (distance > MeleeRange && _inMeleeRange)
			{
				_inMeleeRange = false;
				onLeaveMelee();
			}
			if (_inCloseRange)
			{
				onExitCloseRange();
				_inCloseRange = false;
			}
		}
		else if (distance <= CloseRange)
		{
			onCloseRange();
			_inCloseRange = true;
		}
		if (distance < MeleeRange && !_inMeleeRange)
		{
			onEnterMelee();
			_inMeleeRange = true;
		}
	}

	private void doMove()
	{
		if (_doPursuit)
			control.SimpleMove(transform.forward * MovementSpeed * Time.fixedDeltaTime);
	}

	#region Events firing whenever target or pursuit is altered.

	public delegate void PursuitActionEvent();
	public event PursuitActionEvent
		TargetLostEvent,
		AggroEvent,
		EnterMeleeEvent,
		LeaveMeleeEvent,
		InCloseRange,
		ExitCloseRange;

	private void onTargetLost()
	{
		// This method is called, when pursuer loses target.
		if (TargetLostEvent != null)
			TargetLostEvent();
	}

	private void onAggro()
	{
		// This method is called, when the pursuer is lured by the target.
		if (AggroEvent != null)
			AggroEvent();
	}

	private void onEnterMelee()
	{
		// This method is called, when the pursuer reaches melee range of target.
		if (EnterMeleeEvent != null)
			EnterMeleeEvent();
	}

	private void onLeaveMelee()
	{
		// This method is called, when the pursuer exceeds melee range of target after having entered it.
		if (LeaveMeleeEvent != null)
			LeaveMeleeEvent();
	}

	private void onCloseRange()
	{
		// This method is called, when the pursuer enters close range of target.
		if (InCloseRange != null)
			InCloseRange();
	}

	private void onExitCloseRange()
	{
		// This method is called, when the pursuer leaves close range of target.
		if (ExitCloseRange != null)
			ExitCloseRange();
	}

	#endregion
}
