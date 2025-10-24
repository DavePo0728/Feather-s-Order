using UnityEngine;
using System.Collections;

public class ProceduralMove : MonoBehaviour
{
	[Header("移動設定")]
	public Vector3 pointA = new Vector3(0, 0, 0);
	public Vector3 pointB = new Vector3(5, 3, 0);
	public float moveSpeed = 2f;
	public bool loop = true;

	[Header("高度控制")]
	public AnimationCurve heightCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

	[Header("動畫起始時間 (0~1)")]
	[Range(0f, 1f)]
	public float startTime = 0f; // 0~1

	void Start()
	{
		transform.position = pointA;
		StartCoroutine(MoveRoutine());
	}

	private IEnumerator MoveRoutine()
	{
		float duration = Vector3.Distance(pointA, pointB) / moveSpeed;

		// 動畫 t 從 startTime 開始
		float t = startTime;

		while (true)
		{
			t += Time.deltaTime / duration;

			if (t > 1f)
			{
				if (loop)
					t = 0f; // 循環回頭
				else
					t = 1f; // 停在最後
			}

			// 水平線性插值
			Vector3 pos = Vector3.Lerp(pointA, pointB, t);

			// 曲線高度映射到 A.y 與 B.y
			pos.y = Mathf.Lerp(pointA.y, pointB.y, heightCurve.Evaluate(t));

			transform.position = pos;
			yield return null;

			if (!loop && t >= 1f)
				yield break;
		}
	}
}
