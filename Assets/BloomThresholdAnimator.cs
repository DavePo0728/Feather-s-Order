using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class BloomThresholdAnimator : MonoBehaviour
{
	public Volume volume;
	public float startValue = 1.2f;
	public float endValue = 1.1f;
	public float duration = 1.0f;      // 動畫時間
	public float waitTime = 0.5f;      // 循環間隔

	private Bloom bloom;

	void Start()
	{
		if (volume != null && volume.profile.TryGet(out bloom))
		{
			StartCoroutine(AnimateBloomThreshold());
		}
	}

	IEnumerator AnimateBloomThreshold()
	{
		while (true)
		{
			// 動畫從 startValue 到 endValue
			yield return AnimateValue(startValue, endValue);
			yield return new WaitForSeconds(waitTime);

			// 動畫從 endValue 回到 startValue
			yield return AnimateValue(endValue, startValue);
			yield return new WaitForSeconds(waitTime);
		}
	}

	IEnumerator AnimateValue(float from, float to)
	{
		float time = 0f;
		while (time < duration)
		{
			time += Time.deltaTime;
			float t = time / duration;
			float current = Mathf.Lerp(from, to, t);
			bloom.threshold.value = current;
			yield return null;
		}
		bloom.threshold.value = to;
	}
}
