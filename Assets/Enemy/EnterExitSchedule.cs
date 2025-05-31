using UnityEngine;
using System.Collections;

using ASP; // 確保這是你 ASP 的命名空間

public class EnterExitSchedule : MonoBehaviour
{
	private ASPCharacterPanel aspPanel;
	private Coroutine fadeCoroutine;
	public GameObject Bgshader;

	public float FadeInTime = 1.0f;
	public float FadeOutTime = 1.0f;

	public bool haveblueS = false;
	public GameObject blueSG;
	public bool haveRedS = false;
	public GameObject RedS;
	private void Awake()
	{
		aspPanel = GetComponent<ASPCharacterPanel>();
		if (aspPanel == null)
		{
			Debug.LogError("ASPCharacterPanel not found on the GameObject.");
		}
	}
	private void Start()
	{
		FadeIn();
		FadeOutTime = GetComponent<EnemyMove>().leaveTime;
		FadeInTime = GetComponent<EnemyMove>().entryTime;
	}
	public void FadeIn()
	{
		StartFade(1f, 0f,0f,75f,1f,0f, FadeInTime); 
	}

	public void FadeOut()
	{
		Bgshader.SetActive(false);
		StartFade(0f, 1f,75f,0f,0f,1f, FadeOutTime); 
	}

	private void StartFade(float from, float to, float start2, float end2, float start3, float end3, float duration)
	{
		if (fadeCoroutine != null)
		{
			StopCoroutine(fadeCoroutine);
		}
		fadeCoroutine = StartCoroutine(FadeDithering(from, to, start2, end2, start3, end3, duration));
		
	}

	private IEnumerator FadeDithering(float start1, float end1, float start2, float end2, float start3, float end3, float duration)
	{
		float timeElapsed = 0f;
		bool FadeIn = false;

		if (end1 == 1f)
		{
			FadeIn = true;
		}

		// 如果你需要改變 particleSystem 的透明度，需要透過 main module 修改 startColor
		ParticleSystem.MainModule blueMain = default;
		if (haveblueS && blueSG != null)
		{
			blueMain = blueSG.GetComponent<ParticleSystem>().main;
		}

		Material redMaterial = null;
		if (haveRedS && RedS != null)
		{
			redMaterial = RedS.GetComponent<Renderer>().material;
		}

		while (timeElapsed < duration)
		{
			float t = timeElapsed / duration;

			// 淡入顆粒的Dithering值
			float current = Mathf.Lerp(start1, end1, t);
			aspPanel.SetDitheringValueToAllMaterials(current);

			// blueSG 粒子的透明度
			if (haveblueS && blueSG != null)
			{
				Color color = blueMain.startColor.color;
				color.a = Mathf.Lerp(start2, end2, t);
				blueMain.startColor = color;
			}

			// RedS 材質的 Alpha 變化（假設 Shader Graph 有 "_alpha" 這個 float 屬性）
			if (haveRedS && redMaterial != null)
			{
				float alpha = Mathf.Lerp(start3, end3, t);
				redMaterial.SetFloat("_alpha", alpha);
			}

			timeElapsed += Time.deltaTime;

			if (!FadeIn && t >= 0.8f)
			{
				FadeIn = true;
				Bgshader.SetActive(true);
			}

			yield return null;
		}

		// 最後保證值是準確的
		aspPanel.SetDitheringValueToAllMaterials(end1);

		if (haveblueS && blueSG != null)
		{
			Color color = blueMain.startColor.color;
			color.a = end2;
			blueMain.startColor = color;
		}

		if (haveRedS && redMaterial != null)
		{
			redMaterial.SetFloat("_alpha", end3);
		}
	}

}

