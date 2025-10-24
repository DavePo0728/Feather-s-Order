using System.Collections.Generic;
using UnityEngine;

public class SceneSideShift : MonoBehaviour
{
	[Header("設定區塊與參照")]
	public List<Transform> blocks;       // 場景的 8 個區塊
	public Transform referencePoint;     // 距離參照（玩家或相機）

	[Header("偏移參數")]
	public float maxSideOffset = 3f;     // 最近的區塊最大偏移量
	public float farOffsetScale = 0.5f;  // 最遠區塊偏移倍率
	public float smoothSpeed = 5f;       // 平滑速度

	private Vector3[] originalPositions; // 原始位置
	private float inputDir = 0f;         // -1 = 左鍵, 1 = 右鍵, 0 = 無輸入

	void Start()
	{
		originalPositions = new Vector3[blocks.Count];
		for (int i = 0; i < blocks.Count; i++)
			originalPositions[i] = blocks[i].position;
	}

	void Update()
	{
		// 偵測左右輸入
		if (Input.GetKey(KeyCode.A)) inputDir = -1f;
		else if (Input.GetKey(KeyCode.D)) inputDir = 1f;
		else inputDir = 0f;

		// 找出最大距離（用來做比例）
		float maxDist = 0f;
		float[] dist = new float[blocks.Count];
		for (int i = 0; i < blocks.Count; i++)
		{
			dist[i] = Vector3.Distance(referencePoint.position, blocks[i].position);
			if (dist[i] > maxDist) maxDist = dist[i];
		}

		// 根據距離偏移（只動 X 軸）
		for (int i = 0; i < blocks.Count; i++)
		{
			float t = 1f - (dist[i] / maxDist); // 越近 t 越大
			float offset = Mathf.Lerp(maxSideOffset * farOffsetScale, maxSideOffset, t);

			Vector3 targetPos = originalPositions[i];

			if (inputDir != 0)
			{
				// 最近的往反方向，遠的往同方向
				float dir = inputDir;
				float side = (t > 0.5f) ? -1f : 1f;
				targetPos.x += dir * offset * side;
				Debug.Log($"Block {i} t={t:F2} offset={offset:F2} side={side}");
			}

			// 只插值 X 軸，保持原本的 Y、Z
			Vector3 current = blocks[i].position;
			current.x = Mathf.Lerp(current.x, targetPos.x, Time.deltaTime * smoothSpeed);
			blocks[i].position = current;
		}
	}
}

