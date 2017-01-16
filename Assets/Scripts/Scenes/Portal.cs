using UnityEngine;
using System.Collections;

public class Portal : SceneTransition
{
	public Color
		FrameColor,
		LayerColor,
		CoreColor;

	private GameObject player;
	private ParticleSystem
		FrameSystem,
		LayerSystem,
		CoreSystem;

	void Start()
	{
		player = GameObject.FindGameObjectWithTag("Player");
		FrameSystem = transform.GetChild(0).GetComponent<ParticleSystem>();
		FrameSystem.startColor = FrameColor;
		LayerSystem = transform.GetChild(1).GetComponent<ParticleSystem>();
		LayerSystem.startColor = LayerColor;
		CoreSystem = transform.GetChild(2).GetComponent<ParticleSystem>();
		CoreSystem.startColor = CoreColor;
		setActive(false);
	}

	void Update()
	{
		setPortal();
		pan();
	}

	protected override void OnSceneTransition(string s)
	{
		GameModel.Tiles++;
		MonumentPlaceholder.NewLevel = true;
	}

	private const float Vicinity = 10.0f;
	private void setPortal()
	{
		float distance = Vector3.Distance(player.transform.position, transform.position);
		if (distance < Vicinity)
			setActive(true);
		else
			setActive(false);
	}

	private bool _isActive = true;
	private void setActive(bool active)
	{
		if (_isActive == active) return;
		ParticleSystem.EmissionModule frameModule = FrameSystem.emission;
		frameModule.enabled = active;
		ParticleSystem.EmissionModule layerModule = LayerSystem.emission;
		layerModule.enabled = active;
		ParticleSystem.EmissionModule coreModule = CoreSystem.emission;
		coreModule.enabled = active;
		_isActive = active;
	}

	private void pan()
	{
		transform.LookAt(player.transform.position);
	}
}
