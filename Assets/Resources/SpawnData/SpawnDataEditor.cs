#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.IO;

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

	public RectTransform Target;


	public string NewDataName = "NewData";

	public DefaultAsset Center;
	public DefaultAsset Up;
	public DefaultAsset Down;
	public DefaultAsset Left;
	public DefaultAsset Right;
	public DefaultAsset UpLeft;
	public DefaultAsset UpRight;
	public DefaultAsset DownLeft;
	public DefaultAsset DownRight;
	public void PrintSpawnRegion()
	{
		if (spawnData == null)
		{
			Debug.LogWarning("spawnData 未設定");
			return;
		}

		SpawnRegion region = GetSpawnRegion(spawnData.spawnPosition);
		Debug.Log("Spawn Position 位於區域: " + region + "座標" + new Vector2(spawnData.spawnPosition.x, spawnData.spawnPosition.y));
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
	public void GotoTargetPosition(Vector3 gridPos)
	{
		Vector2 originUI = new Vector2(960, -480);
		Vector2 cellSize = new Vector2(40, 40);

		float uiX = (gridPos.x - 24) * cellSize.x + originUI.x;
		float uiY = (12 - gridPos.y) * cellSize.y + originUI.y;
		float uiZ = (12 - gridPos.z) * 128f; // 假設每層 z 軸跟 Y 一樣大小（你可自訂比例）

		Vector2 uiPos2D = new Vector2(uiX, uiY);
		float zPos = uiZ;

		if (Target != null)
		{
			Target.anchoredPosition = uiPos2D;

			Vector3 currentLocal = Target.localPosition;
			Target.localPosition = new Vector3(currentLocal.x, currentLocal.y, zPos);

			Debug.Log($"Moved Target to UI position: {uiPos2D} (Z: {zPos}) from Grid({gridPos.x}, {gridPos.y}, {gridPos.z})");
		}
		else
		{
			Debug.LogWarning("Target is not assigned.");
		}
	}

	public Vector3Int PosConversion(Vector3 pos)
	{
		// UI 中央點 (960, -480) 是網格 (24, 12, 0)
		Vector2 originUI = new Vector2(960, -480);
		Vector2 cellSize = new Vector2(40, 40);

		// 計算 X 與 Y 軸的格子位置
		Vector2 offset = new Vector2(pos.x - originUI.x, pos.y - originUI.y);
		int gridX = Mathf.RoundToInt(offset.x / cellSize.x) + 24;
		int gridY = Mathf.RoundToInt(offset.y / cellSize.y) + 12;

		// 假設 Z 軸本來就是格子單位，不需轉換（視需求可加轉換邏輯）
		int gridZ = Mathf.RoundToInt(pos.z); // 或視需要保留原始的 pos.z

		return new Vector3Int(gridX, gridY, gridZ);
	}


	public Vector3 MicroPos(Vector3 target, Vector3 offset)
	{
		target = target + offset;
		return target;
	}
	public void CreateSpawnDataAsset()
	{
		if (spawnData == null)
		{
			Debug.LogWarning("spawnData is null");
			return;
		}

		// 1. 取得區域
		SpawnRegion region = GetSpawnRegion(spawnData.spawnPosition);

		// 2. 找到對應資料夾路徑
		string folderPath = GetRegionFolderPath(region);
		if (string.IsNullOrEmpty(folderPath))
		{
			Debug.LogWarning("Folder path for region is null or empty");
			return;
		}

		// 3. 確保資料夾存在
		if (!Directory.Exists(folderPath))
		{
			Debug.LogWarning($"Folder does not exist: {folderPath}");
			return;
		}

		// 4. 建立新的 ScriptableObject 實例
		SpawnData newAsset = ScriptableObject.CreateInstance<SpawnData>();
		newAsset.data = spawnData;

		// 5. 處理命名避免覆蓋
		string baseName = string.IsNullOrEmpty(NewDataName) ? "NewData" : NewDataName;
		string assetPath = Path.Combine(folderPath, baseName + ".asset");
		assetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);

		// 6. 建立資源
		AssetDatabase.CreateAsset(newAsset, assetPath);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();

		Debug.Log($"Created new SpawnData asset at: {assetPath}");
	}
	private string GetRegionFolderPath(SpawnRegion region)
	{
		DefaultAsset folderAsset = null;
		switch (region)
		{
			case SpawnRegion.Center: folderAsset = Center; break;
			case SpawnRegion.Up: folderAsset = Up; break;
			case SpawnRegion.Down: folderAsset = Down; break;
			case SpawnRegion.Left: folderAsset = Left; break;
			case SpawnRegion.Right: folderAsset = Right; break;
			case SpawnRegion.UpLeft: folderAsset = UpLeft; break;
			case SpawnRegion.UpRight: folderAsset = UpRight; break;
			case SpawnRegion.DownLeft: folderAsset = DownLeft; break;
			case SpawnRegion.DownRight: folderAsset = DownRight; break;
		}

		if (folderAsset == null) return null;

		// 用 UnityEditor 的 API 取得資源路徑
#if UNITY_EDITOR
		return UnityEditor.AssetDatabase.GetAssetPath(folderAsset);
#else
    return null;
#endif
	}
}
