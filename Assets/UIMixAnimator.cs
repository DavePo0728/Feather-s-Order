using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIMixAnimator : MonoBehaviour
{
	[Header("動畫設定")]
	public float duration = 2.0f;         // 淡入動畫時間
	public float pauseTime = 1.0f;        // 每次動畫間的間隔秒數

	[Header("UI Image 目標")]
	public Image targetImage;

	private Material runtimeMaterial;

	void Start()
	{
		if (targetImage == null || targetImage.material == null)
		{
			Debug.LogError("請指定 UI Image 並確認其材質存在。");
			enabled = false;
			return;
		}

		// 建立獨立實例以避免影響其他 UI
		runtimeMaterial = Instantiate(targetImage.material);
		targetImage.material = runtimeMaterial;

		StartCoroutine(LoopMixAnimation());
	}

	IEnumerator LoopMixAnimation()
	{
		float startValue = -1f;
		float endValue = 0.8f;

		while (true)
		{
			float elapsed = 0f;

			while (elapsed < duration)
			{
				elapsed += Time.deltaTime;
				float t = Mathf.Clamp01(elapsed / duration);
				float mixValue = Mathf.Lerp(startValue, endValue, t);
				runtimeMaterial.SetFloat("_Mix", mixValue);
				yield return null;
			}

			runtimeMaterial.SetFloat("_Mix", endValue);
			yield return new WaitForSeconds(pauseTime);

			// 也可以選擇反向（0.8 -> -1），這裡我們重設即可重播
		}
	}
}
