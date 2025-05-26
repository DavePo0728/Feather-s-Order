using UnityEngine;
using System.Collections;

using ASP; // 確保這是你 ASP 的命名空間

public class EnterExitSchedule : MonoBehaviour
{
	private ASPCharacterPanel aspPanel;
	private Coroutine fadeCoroutine;
	public GameObject Bgshader;
	private void Awake()
	{
		aspPanel = GetComponent<ASPCharacterPanel>();
		if (aspPanel == null)
		{
			Debug.LogError("ASPCharacterPanel not found on the GameObject.");
		}
	}

	public void FadeIn(float duration)
	{
		StartFade(1f, 0f, duration); // 淡入：從1（透明）到0（實體）
	}

	public void FadeOut(float duration)
	{
		StartFade(0f, 1f, duration); // 淡出：從0（實體）到1（透明）
	}

	private void StartFade(float from, float to, float duration)
	{
		if (fadeCoroutine != null)
		{
			StopCoroutine(fadeCoroutine);
		}
		fadeCoroutine = StartCoroutine(FadeDithering(from, to, duration));
		
	}

	private IEnumerator FadeDithering(float start, float end, float duration)
	{
		if (end == 0)
		{
			Bgshader.SetActive(false);
		}
		float timeElapsed = 0f;
		while (timeElapsed < duration)
		{
			float t = timeElapsed / duration;
			float current = Mathf.Lerp(start, end, t);
			aspPanel.SetDitheringValueToAllMaterials(current);
			timeElapsed += Time.deltaTime;
			yield return null;
		}
		if (start == 0)
		{
			Bgshader.SetActive(true);
		}

		// 保證最終值精準
		aspPanel.SetDitheringValueToAllMaterials(end);
	}
}

