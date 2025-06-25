using UnityEngine;

public class AutoDisable : MonoBehaviour
{
	[Tooltip("經過這麼多秒後關閉這個物件")]
	public float delay = 0.3f;

	void Start()
	{
		Invoke(nameof(DisableSelf), delay);
	}

	void DisableSelf()
	{
		gameObject.SetActive(false);
	}
}

