#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;

public enum SpawnEditDataType
{
	TypeA,
	TypeC
}

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
	public SpawnEditDataType spawnEditDataType;

	public List<RectTransform> EndTargePath;
	public GameObject Target_Path;
	public Transform PathList;
	public List<Vector3> EndTargetList;

	private const int gridSize = 8;

	public RectTransform StartTarget;
	public RectTransform EndTarget;
	public RectTransform LeaveTarget;
	//public Transform StartPoint;
	//public Transform EndPoint;
	//public Transform LeavePoint;

	public LineRenderer LineRenderer;

	public SpawnDataList SpawnDataList;

	public string NewDataName = "NewData";

	public Canvas canvas;
	public DefaultAsset Center;
	public DefaultAsset Up;
	public DefaultAsset Down;
	public DefaultAsset Left;
	public DefaultAsset Right;
	public DefaultAsset UpLeft;
	public DefaultAsset UpRight;
	public DefaultAsset DownLeft;
	public DefaultAsset DownRight;
	public DefaultAsset PathData;
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
	public void SetLineToPath()
	{
		// 確保 LineRenderer 已經初始化
		if (LineRenderer == null)
		{
			Debug.LogError("LineRenderer 沒有設置！");
			return;
		}

		// 每次重設線條資料
		LineRenderer.positionCount = 0;

		switch (spawnEditDataType)
		{
			case SpawnEditDataType.TypeA:
				LineRenderer.positionCount = 3;

				LineRenderer.SetPosition(0, ConvertGridToUI(spawnData.spawnPosition));
				LineRenderer.SetPosition(1, ConvertGridToUI(spawnData.endPosition));
				LineRenderer.SetPosition(2, ConvertGridToUI(spawnData.LeavePositon));
				break;

			case SpawnEditDataType.TypeC:
				int targetCount = EndTargetList.Count + 2;
				LineRenderer.positionCount = targetCount;

				LineRenderer.SetPosition(0, ConvertGridToUI(spawnData.spawnPosition));

				for (int i = 0; i < EndTargetList.Count; i++)
				{
					LineRenderer.SetPosition(i + 1, ConvertGridToUI(EndTargetList[i]));
				}

				LineRenderer.SetPosition(targetCount - 1, ConvertGridToUI(spawnData.LeavePositon));
				break;

			default:
				break;
		}
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
	public void GotoTargetPosition(RectTransform PointTarget, Vector3 gridPos)
	{
		Vector2 originUI = new Vector2(960, -480);
		Vector2 cellSize = new Vector2(40, 40);

		float uiX = (gridPos.x - 24) * cellSize.x + originUI.x;
		float uiY = (12 - gridPos.y) * cellSize.y + originUI.y;
		float uiZ = (12 - gridPos.z) * 128f; // 假設每層 z 軸跟 Y 一樣大小（你可自訂比例）

		Vector2 uiPos2D = new Vector2(uiX, uiY);
		float zPos = uiZ;

		if (PointTarget != null)
		{
			PointTarget.anchoredPosition = uiPos2D;

			Vector3 currentLocal = PointTarget.localPosition;
			PointTarget.localPosition = new Vector3(currentLocal.x, currentLocal.y, zPos);

			//Debug.Log($"Moved Target to UI position: {uiPos2D} (Z: {zPos}) from Grid({gridPos.x}, {gridPos.y}, {gridPos.z})");
		}
		else
		{
			Debug.LogWarning("Target is not assigned.");
		}
		SetLineToPath();
	}
	public void TextChange()
	{

	}
	public Vector3 MicroPos(Vector3 target, Vector3 offset)
	{
		target = target + offset;
		return target;
	}
	public void CreateSpawnDataAsset()
	{
		switch (spawnEditDataType)
		{
			case SpawnEditDataType.TypeA:

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
				if (SpawnDataList != null)
				{
					var tempList = new List<SpawnData>(SpawnDataList.spawnDatas ?? new SpawnData[0]);
					tempList.Add(newAsset);
					SpawnDataList.spawnDatas = tempList.ToArray();
					EditorUtility.SetDirty(SpawnDataList);
					AssetDatabase.SaveAssets();
					Debug.Log("SpawnData 新增至 SpawnDataList");
				}
				else
				{
					Debug.LogWarning("SpawnDataList 為空，無法加入資料");
				}
				break;
			case SpawnEditDataType.TypeC:
				if (spawnData == null)
				{
					Debug.LogWarning("spawnData is null");
					return;
				}

				// 1. 取得區域
				SpawnRegion regionC = GetSpawnRegion(spawnData.spawnPosition);

				// 2. 找到對應資料夾路徑
				string folderPathC = GetRegionFolderPath(regionC);
				if (string.IsNullOrEmpty(folderPathC))
				{
					Debug.LogWarning("Folder path for region is null or empty");
					return;
				}

				if (!Directory.Exists(folderPathC))
				{
					Debug.LogWarning($"Folder does not exist: {folderPathC}");
					return;
				}

				// 3. 準備新的 spawnData（會複製並修改原本的資料）
				SpData newSpData = new SpData
				{
					spawnPosition = spawnData.spawnPosition,
					endPosition = EndTargetList.Count > 0 ? EndTargetList[0] : spawnData.endPosition,
					LeavePositon = spawnData.LeavePositon,
					curveHeight = spawnData.curveHeight,
					pathNum = spawnData.pathNum,
					customPathNum = EndTargetList.Count - 1,
					pointWaitTime = spawnData.pointWaitTime
				};

				// 4. 建立新的 SpawnData 資產
				SpawnData newSpawnAsset = ScriptableObject.CreateInstance<SpawnData>();
				newSpawnAsset.data = newSpData;

				string baseNameC = string.IsNullOrEmpty(NewDataName) ? "NewData_TypeC" : NewDataName + "_TypeC";
				string assetPathC = Path.Combine(folderPathC, baseNameC + ".asset");
				assetPathC = AssetDatabase.GenerateUniqueAssetPath(assetPathC);

				AssetDatabase.CreateAsset(newSpawnAsset, assetPathC);

				// 5. 建立 CustomPathData
				CustomPathData customPath = ScriptableObject.CreateInstance<CustomPathData>();
				customPath.pathX = new List<int>();
				customPath.pathY = new List<int>();
				customPath.pathZ = new List<int>();

				for (int i = 1; i < EndTargetList.Count; i++) // 從第 1 項開始
				{
					Vector3 pos = EndTargetList[i];
					customPath.pathX.Add(Mathf.RoundToInt(pos.x));
					customPath.pathY.Add(Mathf.RoundToInt(pos.y));
					customPath.pathZ.Add(Mathf.RoundToInt(pos.z));
				}

				string pathDataFolder = AssetDatabase.GetAssetPath(PathData);
				string pathDataName = baseNameC + "_PathData.asset";
				string pathDataAssetPath = Path.Combine(pathDataFolder, pathDataName);
				pathDataAssetPath = AssetDatabase.GenerateUniqueAssetPath(pathDataAssetPath);
				AssetDatabase.CreateAsset(customPath, pathDataAssetPath);

				// 6. 更新 SpawnDataList
				if (SpawnDataList != null)
				{
					var tempList = new List<SpawnData>(SpawnDataList.spawnDatas ?? new SpawnData[0]);
					tempList.Add(newSpawnAsset);
					SpawnDataList.spawnDatas = tempList.ToArray();
					EditorUtility.SetDirty(SpawnDataList);
				}

				// 7. 儲存與刷新資源
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();

				Debug.Log($"Created TypeC SpawnData at: {assetPathC}");
				Debug.Log($"Created TypeC PathData at: {pathDataAssetPath}");
				break;
			default:
				break;
		}
		
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
	public void SyncTargetPathsWithEndTargePath()
	{
		if (Target_Path == null || PathList == null)
		{
			Debug.LogError("Target_Path is null 或 PathList 為 null");
			return;
		}

		// 清除現有子物件
		EndTargePath.Clear(); // 先清空 List
		while (PathList.childCount > 0)
		{
			Transform child = PathList.GetChild(0);
			if (Application.isEditor && !Application.isPlaying)
				DestroyImmediate(child.gameObject);
			else
				Destroy(child.gameObject);
		}

		// 依照 EndTargetList 的數量生成新的物件
		for (int i = 0; i < EndTargetList.Count; i++)
		{
			GameObject newObj = PrefabUtility.InstantiatePrefab(Target_Path) as GameObject;
			newObj.name = $"Target_Path_{i}";
			newObj.transform.SetParent(PathList, false);
			newObj.transform.localPosition = Vector3.zero;

			RectTransform rectTransform = newObj.GetComponent<RectTransform>();
			if (rectTransform != null)
			{
				EndTargePath.Add(rectTransform); // 添加到清單中
			}
			else
			{
				Debug.LogWarning("生成的物件缺少 RectTransform 組件");
			}
		}

		Debug.Log($"生成 Target_Path 數量：{EndTargePath.Count}");
	}
	public void SetWaypoints(int index)
	{
		Vector3 gridPos = EndTargetList[index];
		RectTransform PointTarget = EndTargePath[index];

		Vector2 originUI = new Vector2(960, -480);
		Vector2 cellSize = new Vector2(40, 40);

		float uiX = (gridPos.x - 24) * cellSize.x + originUI.x;
		float uiY = (12 - gridPos.y) * cellSize.y + originUI.y;
		float uiZ = (12 - gridPos.z) * 128f; // 假設每層 z 軸跟 Y 一樣大小（你可自訂比例）

		Vector2 uiPos2D = new Vector2(uiX, uiY);
		float zPos = uiZ;

		if (PointTarget != null)
		{
			PointTarget.anchoredPosition = uiPos2D;

			Vector3 currentLocal = PointTarget.localPosition;
			PointTarget.localPosition = new Vector3(currentLocal.x, currentLocal.y, zPos);

			//Debug.Log($"Moved Target to UI position: {uiPos2D} (Z: {zPos}) from Grid({gridPos.x}, {gridPos.y}, {gridPos.z})");
		}
		else
		{
			Debug.LogWarning("Target is not assigned.");
		}
		SetLineToPath();
	}
	private Vector3 ConvertGridToUI(Vector3 gridPos)
	{
		Vector2 originUI = new Vector2(960, -480)+new Vector2(-24*40,12*40);
		Vector2 cellSize = new Vector2(40, 40);

		float uiX = (gridPos.x - 24) * cellSize.x + originUI.x;
		float uiY = (12 - gridPos.y) * cellSize.y + originUI.y;
		float uiZ = (12 - gridPos.z) * 128f;

		Vector3 localUIPos = new Vector3(uiX, uiY, uiZ);

		// 將 local UI 座標轉為世界座標
		return canvas.transform.TransformPoint(localUIPos);
	}
}
