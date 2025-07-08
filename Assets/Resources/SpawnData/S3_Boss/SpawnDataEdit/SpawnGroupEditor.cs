#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;

using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(SpawnGroup))]
public class SpawnGroupEditor : Editor
{
	private ReorderableList reorderableList;

	private void OnEnable()
	{
		SerializedProperty listProp = serializedObject.FindProperty("spawnGroupDataList");

		reorderableList = new ReorderableList(serializedObject, listProp, true, true, true, true);

		reorderableList.drawHeaderCallback = (Rect rect) =>
		{
			EditorGUI.LabelField(rect, "Spawn Group Data List");
		};

		reorderableList.elementHeightCallback = (int index) =>
		{
			SerializedProperty element = listProp.GetArrayElementAtIndex(index);
			float baseHeight = EditorGUIUtility.singleLineHeight + 4;
			float lines = element.isExpanded ? 9 : 1;
			float graphHeight = element.isExpanded ? 220f : 0f; // 折線圖預留高度
			return baseHeight * lines + graphHeight;
		};

		reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
		{
			SerializedProperty element = listProp.GetArrayElementAtIndex(index);
			SpawnGroup group = (SpawnGroup)serializedObject.targetObject;

			float lineHeight = EditorGUIUtility.singleLineHeight;
			float y = rect.y + 2;

			element.isExpanded = EditorGUI.Foldout(
				new Rect(rect.x, y, rect.width, lineHeight),
				element.isExpanded,
				$"SpawnGroupData {index}",
				true
			);

			if (element.isExpanded)
			{
				y += lineHeight + 2;

				EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, lineHeight), element.FindPropertyRelative("enemyData"));
				y += lineHeight + 2;
				EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, lineHeight), element.FindPropertyRelative("spawnData"));
				y += lineHeight + 2;
				EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, lineHeight), element.FindPropertyRelative("gunData"));
				y += lineHeight + 2;
				EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, lineHeight), element.FindPropertyRelative("entryType"));
				y += lineHeight + 2;
				EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, lineHeight), element.FindPropertyRelative("moveType"));
				y += lineHeight + 2;
				EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, lineHeight), element.FindPropertyRelative("leaveType"));
				y += lineHeight + 2;
				EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, lineHeight), element.FindPropertyRelative("delayTime"));
				y += lineHeight + 2;
				EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, lineHeight), element.FindPropertyRelative("spawnType"));
				y += lineHeight + 6;

				// Draw graph
				int spawnIndex = group.spawnGroupDataList[index].spawnData;
				if (group.spawnDataList != null && group.spawnDataList.spawnDatas.Length > spawnIndex)
				{
					SpawnData spawnData = group.spawnDataList.spawnDatas[spawnIndex];
					if (spawnData != null)
					{
						var spData = spawnData.data;
						List<Vector3> points = new List<Vector3>();

						points.Add(spData.spawnPosition);
						points.Add(spData.endPosition);

						int pathIndex = spData.customPathNum;
						if (group.customPathDataList != null &&
							group.customPathDataList.customPathDataList.Count > pathIndex)
						{
							var path = group.customPathDataList.customPathDataList[pathIndex];
							for (int j = 0; j < Mathf.Min(path.pathX.Count, path.pathY.Count, path.pathZ.Count); j++)
							{
								points.Add(new Vector3(path.pathX[j], path.pathY[j], path.pathZ[j]));
							}
						}

						points.Add(spData.LeavePositon);

						PolylineGraphUtility.DrawPolylineGraph(points, v => new Vector2(v.x, v.y), "XY 折線圖 (X-Y)", new Vector2(0, 0), new Vector2(47, 23));
						PolylineGraphUtility.DrawPolylineGraph(points, v => new Vector2(v.z, v.y), "YZ 折線圖 (Y-Z)", new Vector2(0, 0), new Vector2(14, 23));
					}
				}
			}
		};
	}

	private void DrawSingleLine(SerializedProperty prop, Rect rect, ref float y)
	{
		EditorGUI.PropertyField(new Rect(rect.x + 10, y, rect.width - 20, EditorGUIUtility.singleLineHeight), prop);
		y += EditorGUIUtility.singleLineHeight + 2;
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		EditorGUILayout.PropertyField(serializedObject.FindProperty("spawnDataList"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("customPathDataList"));

		reorderableList.DoLayoutList();

		serializedObject.ApplyModifiedProperties();
	}

	private void DrawGraphsForData(SpawnGroup group, int index, float xOffset, ref float y)
	{
		if (group.spawnGroupDataList.Count <= index)
			return;

		int spawnIndex = group.spawnGroupDataList[index].spawnData;
		if (group.spawnDataList == null || group.spawnDataList.spawnDatas.Length <= spawnIndex)
			return;

		var spawnData = group.spawnDataList.spawnDatas[spawnIndex];
		if (spawnData == null) return;

		var spData = spawnData.data;
		List<Vector3> points = new List<Vector3>
		{
			spData.spawnPosition,
			spData.endPosition
		};

		int pathIndex = spData.customPathNum;
		if (group.customPathDataList != null && group.customPathDataList.customPathDataList.Count > pathIndex)
		{
			var path = group.customPathDataList.customPathDataList[pathIndex];
			for (int j = 0; j < Mathf.Min(path.pathX.Count, path.pathY.Count, path.pathZ.Count); j++)
			{
				points.Add(new Vector3(path.pathX[j], path.pathY[j], path.pathZ[j]));
			}
		}

		points.Add(spData.LeavePositon);

		// 畫 XY 和 YZ 折線圖（這裡直接在 Layout 中畫）
		GUILayout.Space(5);
		GUILayout.BeginHorizontal();
		GUILayout.Space(xOffset);
		GUILayout.BeginVertical();

		PolylineGraphUtility.DrawPolylineGraph(points, v => new Vector2(v.x, v.y), "XY 折線圖 (X-Y)", new Vector2(0, 0), new Vector2(47, 23));
		PolylineGraphUtility.DrawPolylineGraph(points, v => new Vector2(v.z, v.y), "YZ 折線圖 (Y-Z)", new Vector2(0, 0), new Vector2(14, 23));

		GUILayout.EndVertical();
		GUILayout.EndHorizontal();

		y += 220; // 估算高度調整（你可依圖大小微調）
	}
	private void DrawInlineGraph(SpawnGroup group, int index, Rect rect, System.Func<Vector3, Vector2> selector, string title)
	{
		int spawnIndex = group.spawnGroupDataList[index].spawnData;
		if (group.spawnDataList == null || group.spawnDataList.spawnDatas.Length <= spawnIndex)
			return;

		var spawnData = group.spawnDataList.spawnDatas[spawnIndex];
		if (spawnData == null) return;

		var spData = spawnData.data;
		List<Vector3> points = new List<Vector3>
	{
		spData.spawnPosition,
		spData.endPosition
	};

		int pathIndex = spData.customPathNum;
		if (group.customPathDataList != null && group.customPathDataList.customPathDataList.Count > pathIndex)
		{
			var path = group.customPathDataList.customPathDataList[pathIndex];
			for (int j = 0; j < Mathf.Min(path.pathX.Count, path.pathY.Count, path.pathZ.Count); j++)
			{
				points.Add(new Vector3(path.pathX[j], path.pathY[j], path.pathZ[j]));
			}
		}

		points.Add(spData.LeavePositon);

		EditorGUI.DrawRect(rect, new Color(0.1f, 0.1f, 0.1f)); // 背景
		GUI.Label(new Rect(rect.x, rect.y - 16, rect.width, 16), title, EditorStyles.boldLabel);

		if (points.Count < 2) return;

		Vector2 min = selector(points[0]);
		Vector2 max = selector(points[0]);
		foreach (var pt in points)
		{
			Vector2 p = selector(pt);
			min = Vector2.Min(min, p);
			max = Vector2.Max(max, p);
		}
		Vector2 size = max - min;
		if (size == Vector2.zero) size = Vector2.one;

		Handles.BeginGUI();
		for (int i = 0; i < points.Count - 1; i++)
		{
			Vector2 a = selector(points[i]);
			Vector2 b = selector(points[i + 1]);

			Vector2 normA = new Vector2((a.x - min.x) / size.x, (a.y - min.y) / size.y);
			Vector2 normB = new Vector2((b.x - min.x) / size.x, (b.y - min.y) / size.y);

			Vector2 guiA = new Vector2(rect.x + normA.x * rect.width, rect.y + (1 - normA.y) * rect.height);
			Vector2 guiB = new Vector2(rect.x + normB.x * rect.width, rect.y + (1 - normB.y) * rect.height);

			Handles.color = Color.cyan;
			Handles.DrawLine(guiA, guiB);
		}

		foreach (var pt in points)
		{
			Vector2 p = selector(pt);
			Vector2 norm = new Vector2((p.x - min.x) / size.x, (p.y - min.y) / size.y);
			Vector2 guiP = new Vector2(rect.x + norm.x * rect.width, rect.y + (1 - norm.y) * rect.height);

			Handles.color = Color.yellow;
			Handles.DrawSolidDisc(guiP, Vector3.forward, 2);
		}
		Handles.EndGUI();
	}
}
#endif