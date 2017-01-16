using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider))]
public class SceneTransition : MonoBehaviour
{
	public string Tag;
	public string Scene;
	void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag(Tag))
		{
			SceneManager.LoadScene(Scene);
			OnSceneTransition(Scene);
		}
	}

	protected virtual void OnSceneTransition(string s)
	{
		// Override in child class to act when scene changes.
	}
}
