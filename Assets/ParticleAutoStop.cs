using UnityEngine;

public class ParticleAutoStop : MonoBehaviour
{
	private ParticleSystem particleSystem;

	void Start()
	{
		particleSystem = GetComponent<ParticleSystem>();
		if (particleSystem != null)
		{
			particleSystem.Play();
			Invoke("StopParticle", 0.2f);
		}
		else
		{
			Debug.LogWarning("未找到 ParticleSystem 元件");
		}
	}

	void StopParticle()
	{
		if (particleSystem != null)
		{
			particleSystem.Stop();
		}
	}
}
