using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StartEndPosition
{
	public Vector2 start;
	public Vector2 end;

	public StartEndPosition(Vector2 start, Vector2 end)
	{
		this.start = start;
		this.end = end;
	}
}

public class Test : MonoBehaviour
{
	public Transform target;                      // 參考基準物件
	public Transform player;                      // 要移動的物件
	public List<StartEndPosition> startEndPositions = new(); // 起點與終點清單
	public float duration = 2.0f;                 // 單段移動時間
	public AnimationCurve speedCurve;             // 速度控制曲線
	public bool loop = false;                     // 是否重複執行
	public float waitTime = 0f;                   // 每次移動後的間隔時間，預設為 0

	private bool isMoving = false;

	private void Start()
	{
		StartCoroutine(MovePlayer());
	}
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.W) && !isMoving)
		{
			StartCoroutine(MovePlayer());
		}

		if (Input.GetKeyDown(KeyCode.S))
		{
			loop = false;
		}
	}

	private IEnumerator MovePlayer()
	{
		isMoving = true;

		do
		{
			foreach (var pos in startEndPositions)
			{
				Vector3 endPosition = new Vector3(
					target.position.x + pos.end.x, target.position.y + pos.end.y,0);

				yield return StartCoroutine(MoveToPositionWithCurve(endPosition));

				// 等待間隔時間
				yield return new WaitForSeconds(waitTime);
			}
		} while (loop);

		isMoving = false;
	}

	private IEnumerator MoveToPositionWithCurve(Vector3 targetPosition)
	{
		Vector3 startPosition = player.position;
		float elapsedTime = 0f;

		while (elapsedTime < duration)
		{
			float t = elapsedTime / duration;
			float curvedT = speedCurve.Evaluate(t);
			player.position = Vector3.Lerp(startPosition, targetPosition, curvedT);

			elapsedTime += Time.deltaTime;
			yield return null;
		}

		player.position = targetPosition;
	}
}
