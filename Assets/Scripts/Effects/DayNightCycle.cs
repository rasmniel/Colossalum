using UnityEngine;
using System.Collections;

public class DayNightCycle : MonoBehaviour
{
	public GameObject Sun;
	public GameObject Moon;
	public Color DayColor;
	public Color NightColor;

	private Light lightSource, moonLight;
	private bool _day = true;
	void Start()
	{
		lightSource = Sun.GetComponent<Light>();
		lightSource.color = DayColor;
		moonLight = Moon.GetComponent<Light>();
		moonLight.color = NightColor;
		Moon.transform.parent = GameObject.FindGameObjectWithTag("Player").transform;
		Moon.transform.position = Moon.transform.parent.position;
		Moon.SetActive(false);
	}

	void Update()
	{
		rotate();
	}

	[Range(0, 1)]
	public float RotationSpeed = 1.0f;
	private const float
		NightTimeDegrees = 90.0f,
		DayTimeDegrees = 270.0f;
	private void rotate()
	{
		transform.Rotate(Vector3.forward * RotationSpeed);
		colorTransition();
	}

	private void colorTransition()
	{
		if (transform.eulerAngles.z > NightTimeDegrees && transform.eulerAngles.z < DayTimeDegrees)
		{
			doColorTransition(lightSource.color, NightColor, false);
			if (_day) dayNightToggle();
		}
		else if (transform.eulerAngles.z > DayTimeDegrees || transform.eulerAngles.z < NightTimeDegrees)
		{
			doColorTransition(lightSource.color, DayColor, true);
			if (!_day) dayNightToggle();
		}
	}

	private void doColorTransition(Color from, Color to, bool day)
	{
		Color c = Color.Lerp(from, to, Time.deltaTime * RotationSpeed);
		lightSource.color = c;
		moonLight.color = c;
	}

	private void dayNightToggle()
	{
		_day = !_day;
		lightSource.shadows = // Effectively toggle between soft and no shadows.
			lightSource.shadows == LightShadows.None ? LightShadows.Soft : LightShadows.None;
		Moon.SetActive(!_day);
	}
}
