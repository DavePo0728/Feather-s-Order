using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class ScaleAnimator : MonoBehaviour
{
	public Vector3 startScale = Vector3.zero;
	public Vector3 endScale = Vector3.one;
	public float duration = 1f;

	public List<Rigidbody> rig = new List<Rigidbody>();
	private Coroutine scaleCoroutine;

	void Update()
	{
		// 按下空白鍵時觸發動畫
		if (Input.GetKeyDown(KeyCode.Space))
		{
			PlayScaleAnimation(startScale, endScale, duration);
			foreach (var rig in rig)
			{
				rig.useGravity = true;
			}
			print("13");
		}
	}

	public void PlayScaleAnimation(Vector3 from, Vector3 to, float time)
	{
		if (scaleCoroutine != null)
		{
			StopCoroutine(scaleCoroutine);
		}
		scaleCoroutine = StartCoroutine(ScaleOverTime(from, to, time));
	}

	private IEnumerator ScaleOverTime(Vector3 from, Vector3 to, float time)
	{
		float elapsed = 0f;
		transform.localScale = from;

		while (elapsed < time)
		{
			transform.localScale = Vector3.Lerp(from, to, elapsed / time);
			elapsed += Time.deltaTime;
			yield return null;
		}

		transform.localScale = to;
	}
}
