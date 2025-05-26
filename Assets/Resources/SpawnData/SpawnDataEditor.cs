#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;
using Tayx.Graphy.Utils.NumString;
using UnityEditorInternal.VR;

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

	public Transform Target_End;
	public Transform Target_PathList;

	public CustomPathData CustomPathData;
	public SpawnData SpawnData;

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

	public void TargetWaySetActive()
	{
		switch (spawnEditDataType)
		{
			case SpawnEditDataType.TypeA:
				Target_End.gameObject.SetActive(true);
				Target_PathList.gameObject.SetActive(false);
				break;
			case SpawnEditDataType.TypeC:
				Target_End.gameObject.SetActive(false);
				Target_PathList.gameObject.SetActive(true);
				break;
		}
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
	public Vector3 ReversalPosX(Vector3 target)
	{
		Vector3 max = new Vector3(47, 23, 14);

		target = new Vector3(max.x - target.x,target.y, target.z);
		return target;
	}
	public Vector3 ReversalPosY(Vector3 target)
	{
		Vector3 max = new Vector3(47, 23, 14);

		target = new Vector3(target.x, max.y - target.y, target.z);
		return target;
	}
	public Vector3 ReversalPosZ(Vector3 target)
	{
		Vector3 max = new Vector3(47, 23, 14);
		target = new Vector3(target.x,target.y, max.z-target.z);
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
					customPathNum = spawnData.customPathNum,
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
		Vector2 originUI = new Vector2(960, -480) + new Vector2(-24 * 40, 12 * 40);
		Vector2 cellSize = new Vector2(40, 40);

		float uiX = (gridPos.x - 24) * cellSize.x + originUI.x;
		float uiY = (12 - gridPos.y) * cellSize.y + originUI.y;
		float uiZ = (12 - gridPos.z) * 128f;

		Vector3 localUIPos = new Vector3(uiX, uiY, uiZ);

		// 將 local UI 座標轉為世界座標
		return canvas.transform.TransformPoint(localUIPos);
	}

	public void LoadingMyData()
	{
		LoadingData(SpawnData, CustomPathData);
	}
	public void LoadingData(SpawnData sp, CustomPathData cpd)
	{
		// 1. 重新建立 EndTargetList（包含 endPosition + 自定義路徑）
		EndTargetList.Clear();
		EndTargetList.Add(sp.data.endPosition); // 第一點是原本 endPosition

		// 2. 根據 pathX/Y/Z 長度，建立剩餘點位
		int customCount = Mathf.Min(cpd.pathX.Count, cpd.pathY.Count, cpd.pathZ.Count);
		for (int i = 0; i < customCount; i++)
		{
			Vector3 pos = new Vector3(
				cpd.pathX[i].ToFloat(),
				cpd.pathY[i].ToFloat(),
				cpd.pathZ[i].ToFloat()
			);
			EndTargetList.Add(pos);
		}

		// 3. 載入其他資料
		spawnData.spawnPosition = sp.data.spawnPosition;
		spawnData.LeavePositon = sp.data.LeavePositon;
		SpawnData.data.customPathNum = sp.data.customPathNum;
		SpawnData.data.curveHeight = sp.data.curveHeight;
		SpawnData.data.customPathNum = sp.data.customPathNum;

		// 4. 同步路徑
		SyncTargetPathsWithEndTargePath();
		for (int i = 0; i < EndTargePath.Count; i++)
		{
			GotoTargetPosition(EndTargePath[i], EndTargetList[i]);
		}
		GotoTargetPosition(StartTarget, spawnData.spawnPosition);
		GotoTargetPosition(LeaveTarget, spawnData.LeavePositon);
	}
	public void OverwriteData()
	{
		if (SpawnData == null || CustomPathData == null)
		{
			Debug.LogWarning("SpawnData 或 CustomPathData 為空，無法覆蓋");
			return;
		}

		// 1. 覆蓋 SpawnData 資料
		SpData newSpData = new SpData
		{
			spawnPosition = spawnData.spawnPosition,
			endPosition = EndTargetList.Count > 0 ? EndTargetList[0] : spawnData.endPosition,
			LeavePositon = spawnData.LeavePositon,
			curveHeight = spawnData.curveHeight,
			pathNum = spawnData.pathNum,
			customPathNum = spawnData.customPathNum,
			pointWaitTime = spawnData.pointWaitTime
		};
		SpawnData.data = newSpData;
		EditorUtility.SetDirty(SpawnData);

		// 2. 覆蓋 CustomPathData 資料
		CustomPathData.pathX.Clear();
		CustomPathData.pathY.Clear();
		CustomPathData.pathZ.Clear();

		for (int i = 1; i < EndTargetList.Count; i++) // 從第 1 個 endPoint 開始
		{
			Vector3 pos = EndTargetList[i];
			CustomPathData.pathX.Add(Mathf.RoundToInt(pos.x));
			CustomPathData.pathY.Add(Mathf.RoundToInt(pos.y));
			CustomPathData.pathZ.Add(Mathf.RoundToInt(pos.z));
		}
		EditorUtility.SetDirty(CustomPathData);

		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();

		Debug.Log("成功覆蓋 SpawnData 與 CustomPathData");
	}

}
