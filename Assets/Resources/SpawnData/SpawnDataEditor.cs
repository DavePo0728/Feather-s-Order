using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpawnRegion
{
	Center,
	Up,
	Down,
	Left,
	Right,
	UpLeft,
	UpRight,
	DownLeft,
	DownRight
}

public class SpawnDataEditor : MonoBehaviour
{
	public SpData spawnData;

	private const int gridSize = 8;

	public Transform Target;
	public void PrintSpawnRegion()
	{
		if (spawnData == null)
		{
			Debug.LogWarning("spawnData 未設定");
			return;
		}

		SpawnRegion region = GetSpawnRegion(spawnData.spawnPosition);
		Debug.Log("Spawn Position 位於區域: " + region + "座標"+new Vector2(spawnData.spawnPosition.x, spawnData.spawnPosition.y));
	}

	private SpawnRegion GetSpawnRegion(Vector3 pos)
	{
		float x = pos.x;
		float y = pos.y;

		if (x >= 16 && x <= 32 && y >= 16 && y <= 32) return SpawnRegion.Center;
		if (x >= 16 && x <= 32 && y >= 0 && y <= 16) return SpawnRegion.Up;
		if (x >= 16 && x <= 32 && y >= 16 && y <= 32) return SpawnRegion.Down;

		if (x >= 0 && x <= 16 && y >= 8 && y <= 16) return SpawnRegion.Left;
		if (x >= 32 && x <= 48 && y >= 8 && y <= 16) return SpawnRegion.Right;

		if (x >= 0 && x <= 16 && y >= 0 && y <= 8) return SpawnRegion.UpLeft;
		if (x >= 32 && x <= 48 && y >= 0 && y <= 8) return SpawnRegion.UpRight;

		if (x >= 0 && x <= 16 && y >= 16 && y <= 24) return SpawnRegion.DownLeft;
		if (x >= 32 && x <= 48 && y >= 16 && y <= 24) return SpawnRegion.DownRight;

		return SpawnRegion.Center; // 預設 fallback
	}
	public string GetRegionName(Vector3 pos)
	{
		return GetSpawnRegion(pos).ToString();
	}
}
