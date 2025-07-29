using UnityEngine;
using System.Collections;

public class SceneInitWatcher : MonoBehaviour
{
	public static bool Initialized { get; private set; } = false;

	private IEnumerator Start()
	{
		yield return null;


		yield return StartCoroutine(DoSceneInitialization());

		Initialized = true;
		Debug.Log("Scene initialization complete. Initialized: " + Initialized);
	}

	private IEnumerator DoSceneInitialization()
	{
		yield return new WaitForSeconds(0.2f); 
	}
}
