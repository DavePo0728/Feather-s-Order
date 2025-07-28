using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class GainWAnimator : MonoBehaviour
{
	public Volume volume;
	public float targetW1 = 1.25f;
	public float targetW2 = 0f;
	public float duration1 = 0.5f;
	public float duration2 = 1.0f;
	public float delayBetween = 0f;
	public float fadeDuration = 1.0f; // 可在 Inspector 設定

	private LiftGammaGain liftGammaGain;

	public ScenesManager scenesManager; // 這是用來載入教學的參考
										// 呼叫這個開始動畫並在完成後載入教學
	public void StartAnimation()
	{
		if (volume != null && volume.profile.TryGet(out liftGammaGain))
		{
			StartCoroutine(AnimationAndLoadTeaching());
		}
		else
		{
			Debug.LogWarning("找不到 LiftGammaGain");
		}
	}

	IEnumerator AnimationAndLoadTeaching()
	{
		Vector4 baseGain = liftGammaGain.gain.value;

		// 第一段：0 → targetW1
		yield return AnimateW(baseGain, 0f, targetW1, duration1);
		yield return new WaitForSeconds(delayBetween);

		// 第二段：targetW1 → targetW2
		yield return AnimateW(baseGain, targetW1, targetW2, duration2);
		yield return new WaitForSeconds(delayBetween);

		// 動畫結束後呼叫教學
		scenesManager.InvokeLoadTeaching();
	}

	IEnumerator AnimateW(Vector4 baseGain, float fromW, float toW, float duration)
	{
		float time = 0f;
		while (time < duration)
		{
			time += Time.deltaTime;
			float t = time / duration;
			float currentW = Mathf.Lerp(fromW, toW, t);
			liftGammaGain.gain.value = new Vector4(baseGain.x, baseGain.y, baseGain.z, currentW);
			yield return null;
		}
		liftGammaGain.gain.value = new Vector4(baseGain.x, baseGain.y, baseGain.z, toW);
	}

	// 這會在動畫結束 fadeDuration 秒後被呼叫
	void LoadTeaching()
	{
		Debug.Log("教學載入完成！");
		// 你可以在這裡加入實際邏輯
	}
}
