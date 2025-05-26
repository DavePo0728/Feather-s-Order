using PathCreationEditor;
using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEditorInternal.VR;
using UnityEngine;
using static UnityEngine.Random;

[CustomEditor(typeof(SpawnDataEditor))]
public class SpawnDataEditorEditor : Editor
{
	private bool showGroup1 = false; // 改名字比較直覺
	private SerializedProperty endTargetListProp;
	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("spawnEditDataType"));
		//serializedObject.Update();
		SpawnDataEditor SDEditor = (SpawnDataEditor)target;
		SerializedProperty iterator = serializedObject.GetIterator();
		SerializedProperty iterator2 = serializedObject.GetIterator();
		SerializedProperty iterator3 = serializedObject.GetIterator();
		iterator.NextVisible(true); // 跳過 script 欄位
		while (iterator.NextVisible(false))
		{
			EditorGUILayout.PropertyField(iterator, true);

			// 找到 spawnPosition 屬性後插入按鈕
			if (iterator.name == "spawnData")
			{
				SerializedProperty spawnPosProp = iterator.FindPropertyRelative("spawnPosition");

				if (spawnPosProp != null)
				{
					EditorGUILayout.PropertyField(spawnPosProp);

					// 插入按鈕在 spawnPosition 下
					GUILayout.BeginHorizontal();
					if (GUILayout.Button("EditPos"))
					{
						SDEditor.GotoTargetPosition(SDEditor.StartTarget, SDEditor.spawnData.spawnPosition);
					}


					GUILayout.BeginHorizontal();
					GUILayout.FlexibleSpace(); // 讓上鍵居中
					if (GUILayout.Button("X↺", GUILayout.Width(30), GUILayout.Height(30)))
					{
						SDEditor.spawnData.spawnPosition = SDEditor.ReversalPosX(SDEditor.spawnData.spawnPosition);
						SDEditor.GotoTargetPosition(SDEditor.StartTarget, SDEditor.spawnData.spawnPosition);
					}
					if (GUILayout.Button("Y↺", GUILayout.Width(30), GUILayout.Height(30)))
					{
						SDEditor.spawnData.spawnPosition = SDEditor.ReversalPosY(SDEditor.spawnData.spawnPosition);
						SDEditor.GotoTargetPosition(SDEditor.StartTarget, SDEditor.spawnData.spawnPosition);
					}
					if (GUILayout.Button("←", GUILayout.Width(30), GUILayout.Height(30)))
					{
						SDEditor.spawnData.spawnPosition = SDEditor.MicroPos(SDEditor.spawnData.spawnPosition, new Vector3(-1f, 0, 0));
						SDEditor.GotoTargetPosition(SDEditor.StartTarget, SDEditor.spawnData.spawnPosition);
					}
					//GUILayout.FlexibleSpace();
					GUILayout.EndHorizontal();

					// 中（左與右）
					GUILayout.BeginHorizontal();

					if (GUILayout.Button("↑", GUILayout.Width(30), GUILayout.Height(30)))
					{
						SDEditor.spawnData.spawnPosition = SDEditor.MicroPos(SDEditor.spawnData.spawnPosition, new Vector3(0, -1f, 0));
						SDEditor.GotoTargetPosition(SDEditor.StartTarget, SDEditor.spawnData.spawnPosition);
					}

					// 下
					GUILayout.BeginHorizontal();
					//GUILayout.FlexibleSpace(); // 讓下鍵居中
					if (GUILayout.Button("↓", GUILayout.Width(30), GUILayout.Height(30)))
					{
						SDEditor.spawnData.spawnPosition = SDEditor.MicroPos(SDEditor.spawnData.spawnPosition, new Vector3(0, 1, 0));
						SDEditor.GotoTargetPosition(SDEditor.StartTarget, SDEditor.spawnData.spawnPosition);
					}
					//GUILayout.FlexibleSpace();
					GUILayout.EndHorizontal();
					//GUILayout.Space(10);
					if (GUILayout.Button("→", GUILayout.Width(30), GUILayout.Height(30)))
					{
						SDEditor.spawnData.spawnPosition = SDEditor.MicroPos(SDEditor.spawnData.spawnPosition, new Vector3(1, 0, 0));
						SDEditor.GotoTargetPosition(SDEditor.StartTarget, SDEditor.spawnData.spawnPosition);
					}
					GUILayout.Space(20);
					if (GUILayout.Button("↺", GUILayout.Width(30), GUILayout.Height(30)))
					{
						SDEditor.spawnData.spawnPosition = SDEditor.ReversalPosZ(SDEditor.spawnData.spawnPosition);
						SDEditor.GotoTargetPosition(SDEditor.StartTarget, SDEditor.spawnData.spawnPosition);
					}
					if (GUILayout.Button("←", GUILayout.Width(30), GUILayout.Height(30)))
					{
						SDEditor.spawnData.spawnPosition = SDEditor.MicroPos(SDEditor.spawnData.spawnPosition, new Vector3(0, 0, -1));
						SDEditor.GotoTargetPosition(SDEditor.StartTarget, SDEditor.spawnData.spawnPosition);
					}
					if (GUILayout.Button("→", GUILayout.Width(30), GUILayout.Height(30)))
					{
						SDEditor.spawnData.spawnPosition = SDEditor.MicroPos(SDEditor.spawnData.spawnPosition, new Vector3(0, 0, 1f));
						SDEditor.GotoTargetPosition(SDEditor.StartTarget, SDEditor.spawnData.spawnPosition);
					}
					GUILayout.EndHorizontal();
					GUILayout.EndHorizontal();
				}
				SerializedProperty endPosition = iterator.FindPropertyRelative("endPosition");
				if (SDEditor.spawnEditDataType == SpawnEditDataType.TypeA)
				{

					if (endPosition != null)
					{
						EditorGUILayout.PropertyField(endPosition);

						// 插入按鈕在 spawnPosition 下
						GUILayout.BeginHorizontal();
						if (GUILayout.Button("EditPos"))
						{
							SDEditor.GotoTargetPosition(SDEditor.EndTarget, SDEditor.spawnData.endPosition);
						}
						GUILayout.BeginHorizontal();
						GUILayout.FlexibleSpace(); // 讓上鍵居中
						if (GUILayout.Button("X↺", GUILayout.Width(30), GUILayout.Height(30)))
						{
							SDEditor.spawnData.endPosition = SDEditor.ReversalPosX(SDEditor.spawnData.endPosition);
							SDEditor.GotoTargetPosition(SDEditor.EndTarget, SDEditor.spawnData.endPosition);
						}
						if (GUILayout.Button("Y↺", GUILayout.Width(30), GUILayout.Height(30)))
						{
							SDEditor.spawnData.endPosition = SDEditor.ReversalPosY(SDEditor.spawnData.endPosition);
							SDEditor.GotoTargetPosition(SDEditor.EndTarget, SDEditor.spawnData.endPosition);
						}
						if (GUILayout.Button("←", GUILayout.Width(30), GUILayout.Height(30)))
						{
							SDEditor.spawnData.endPosition = SDEditor.MicroPos(SDEditor.spawnData.endPosition, new Vector3(-1f, 0, 0));
							SDEditor.GotoTargetPosition(SDEditor.EndTarget, SDEditor.spawnData.endPosition);
						}
						//GUILayout.FlexibleSpace();
						GUILayout.EndHorizontal();

						// 中（左與右）
						GUILayout.BeginHorizontal();

						if (GUILayout.Button("↑", GUILayout.Width(30), GUILayout.Height(30)))
						{
							SDEditor.spawnData.endPosition = SDEditor.MicroPos(SDEditor.spawnData.endPosition, new Vector3(0, -1f, 0));
							SDEditor.GotoTargetPosition(SDEditor.EndTarget, SDEditor.spawnData.endPosition);
						}

						// 下
						GUILayout.BeginHorizontal();
						//GUILayout.FlexibleSpace(); // 讓下鍵居中
						if (GUILayout.Button("↓", GUILayout.Width(30), GUILayout.Height(30)))
						{
							SDEditor.spawnData.endPosition = SDEditor.MicroPos(SDEditor.spawnData.endPosition, new Vector3(0, 1, 0));
							SDEditor.GotoTargetPosition(SDEditor.EndTarget, SDEditor.spawnData.endPosition);
						}
						//GUILayout.FlexibleSpace();
						GUILayout.EndHorizontal();
						//GUILayout.Space(10);
						if (GUILayout.Button("→", GUILayout.Width(30), GUILayout.Height(30)))
						{
							SDEditor.spawnData.endPosition = SDEditor.MicroPos(SDEditor.spawnData.endPosition, new Vector3(1, 0, 0));
							SDEditor.GotoTargetPosition(SDEditor.EndTarget, SDEditor.spawnData.endPosition);
						}
						GUILayout.Space(20);
						if (GUILayout.Button("↺", GUILayout.Width(30), GUILayout.Height(30)))
						{
							SDEditor.spawnData.endPosition = SDEditor.ReversalPosZ(SDEditor.spawnData.endPosition);
							SDEditor.GotoTargetPosition(SDEditor.EndTarget, SDEditor.spawnData.endPosition);
						}
						if (GUILayout.Button("←", GUILayout.Width(30), GUILayout.Height(30)))
						{
							SDEditor.spawnData.endPosition = SDEditor.MicroPos(SDEditor.spawnData.endPosition, new Vector3(0, 0, -1));
							SDEditor.GotoTargetPosition(SDEditor.EndTarget, SDEditor.spawnData.endPosition);
						}
						if (GUILayout.Button("→", GUILayout.Width(30), GUILayout.Height(30)))
						{
							SDEditor.spawnData.endPosition = SDEditor.MicroPos(SDEditor.spawnData.endPosition, new Vector3(0, 0, 1f));
							SDEditor.GotoTargetPosition(SDEditor.EndTarget, SDEditor.spawnData.endPosition);
						}
						GUILayout.EndHorizontal();
						GUILayout.EndHorizontal();
					}

					SDEditor.SetLineToPath();
					SDEditor.TargetWaySetActive();

				}
				if (SDEditor.spawnEditDataType == SpawnEditDataType.TypeC)
				{

					reorderableList.DoLayoutList();

					SDEditor.SetLineToPath();
					SDEditor.TargetWaySetActive();
				}
				SerializedProperty LeavePositon = iterator.FindPropertyRelative("LeavePositon");
				if (LeavePositon != null)
				{
					EditorGUILayout.PropertyField(LeavePositon);

					// 插入按鈕在 spawnPosition 下
					GUILayout.BeginHorizontal();
					if (GUILayout.Button("EditPos"))
					{
						SDEditor.GotoTargetPosition(SDEditor.LeaveTarget, SDEditor.spawnData.LeavePositon);
					}
					GUILayout.BeginHorizontal();
					GUILayout.FlexibleSpace(); // 讓上鍵居中
					if (GUILayout.Button("X↺", GUILayout.Width(30), GUILayout.Height(30)))
					{
						SDEditor.spawnData.LeavePositon = SDEditor.ReversalPosX(SDEditor.spawnData.LeavePositon);
						SDEditor.GotoTargetPosition(SDEditor.LeaveTarget, SDEditor.spawnData.LeavePositon);
					}
					if (GUILayout.Button("Y↺", GUILayout.Width(30), GUILayout.Height(30)))
					{
						SDEditor.spawnData.LeavePositon = SDEditor.ReversalPosY(SDEditor.spawnData.LeavePositon);
						SDEditor.GotoTargetPosition(SDEditor.LeaveTarget, SDEditor.spawnData.LeavePositon);
					}
					if (GUILayout.Button("←", GUILayout.Width(30), GUILayout.Height(30)))
					{
						SDEditor.spawnData.LeavePositon = SDEditor.MicroPos(SDEditor.spawnData.LeavePositon, new Vector3(-1f, 0, 0));
						SDEditor.GotoTargetPosition(SDEditor.LeaveTarget, SDEditor.spawnData.LeavePositon);
					}
					//GUILayout.FlexibleSpace();
					GUILayout.EndHorizontal();

					// 中（左與右）
					GUILayout.BeginHorizontal();

					if (GUILayout.Button("↑", GUILayout.Width(30), GUILayout.Height(30)))
					{
						SDEditor.spawnData.LeavePositon = SDEditor.spawnData.endPosition = SDEditor.MicroPos(SDEditor.spawnData.LeavePositon, new Vector3(0, -1f, 0));
						SDEditor.GotoTargetPosition(SDEditor.LeaveTarget, SDEditor.spawnData.LeavePositon);
					}

					// 下
					GUILayout.BeginHorizontal();
					//GUILayout.FlexibleSpace(); // 讓下鍵居中
					if (GUILayout.Button("↓", GUILayout.Width(30), GUILayout.Height(30)))
					{
						SDEditor.spawnData.LeavePositon = SDEditor.MicroPos(SDEditor.spawnData.LeavePositon, new Vector3(0, 1, 0));
						SDEditor.GotoTargetPosition(SDEditor.LeaveTarget, SDEditor.spawnData.LeavePositon);
					}
					//GUILayout.FlexibleSpace();
					GUILayout.EndHorizontal();
					//GUILayout.Space(10);
					if (GUILayout.Button("→", GUILayout.Width(30), GUILayout.Height(30)))
					{
						SDEditor.spawnData.LeavePositon = SDEditor.MicroPos(SDEditor.spawnData.LeavePositon, new Vector3(1, 0, 0));
						SDEditor.GotoTargetPosition(SDEditor.LeaveTarget, SDEditor.spawnData.LeavePositon);
					}
					GUILayout.Space(20);
					if (GUILayout.Button("↺", GUILayout.Width(30), GUILayout.Height(30)))
					{
						SDEditor.spawnData.LeavePositon = SDEditor.ReversalPosZ(SDEditor.spawnData.LeavePositon);
						SDEditor.GotoTargetPosition(SDEditor.LeaveTarget, SDEditor.spawnData.LeavePositon);
					}
					if (GUILayout.Button("←", GUILayout.Width(30), GUILayout.Height(30)))
					{
						SDEditor.spawnData.LeavePositon = SDEditor.MicroPos(SDEditor.spawnData.LeavePositon, new Vector3(0, 0, -1));
						SDEditor.GotoTargetPosition(SDEditor.LeaveTarget, SDEditor.spawnData.LeavePositon);
					}
					if (GUILayout.Button("→", GUILayout.Width(30), GUILayout.Height(30)))
					{
						SDEditor.spawnData.LeavePositon = SDEditor.MicroPos(SDEditor.spawnData.LeavePositon, new Vector3(0, 0, 1f));
						SDEditor.GotoTargetPosition(SDEditor.LeaveTarget, SDEditor.spawnData.LeavePositon);
					}
					GUILayout.EndHorizontal();
					GUILayout.EndHorizontal();
				}
				// 略過 spawnData 的其餘內部欄位，防止重複顯示
				break;
			}
		}
		//if (iterator.name == "移動所有")
		//{
		//	float spacing = 2f;
		//	float btnW = 30f, btnH = 30f;
		//	float startX = rect.x + 20;
		//	float startY = rect.y + lineHeight + spacing + 35f;
		//	if (GUI.Button(new Rect(startX, startY, 60, btnH), "EditPos"))
		//	{
		//		//SDEditor.GotoTargetPosition(SDEditor.EndTargePath[index], SDEditor.EndTargetList[index]);
		//	}

		//	if (GUI.Button(new Rect(startX + 70, startY, btnW, btnH), "←"))
		//	{
				
		//	}
		//	if (GUI.Button(new Rect(startX, startY - btnH - spacing, btnW, btnH), "X↺"))
		//	{

		//	}
		//	if (GUI.Button(new Rect(startX + 105, startY - btnH - spacing, btnW, btnH), "↑"))
		//	{

		//	}



		//	if (GUI.Button(new Rect(startX + 105, startY + btnH + spacing, btnW, btnH), "↓"))
		//	{

		//	}
		//	if (GUI.Button(new Rect(startX, startY + btnH + spacing, btnW, btnH), "Y↺"))
		//	{

		//	}
		//	if (GUI.Button(new Rect(startX + 140, startY, btnW, btnH), "→"))
		//	{

		//	}

		//	if (GUI.Button(new Rect(startX + 180, startY, btnW, btnH), "Z←"))
		//	{

		//	}
		//	if (GUI.Button(new Rect(startX + 215, startY, btnW, btnH), "↺"))
		//	{

		//	}
		//	if (GUI.Button(new Rect(startX + 250, startY, btnW, btnH), "Z→"))
		//	{

		//	}
		//}
			showGroup1 = EditorGUILayout.Foldout(showGroup1, "資料", true);
		if (showGroup1)
		{
			EditorGUILayout.PropertyField(serializedObject.FindProperty("Center"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("Up"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("Down"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("Left"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("Right"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("UpLeft"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("UpRight"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("DownLeft"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("DownRight"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("StartTarget"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("EndTarget"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("LeaveTarget"));
			//EditorGUILayout.PropertyField(serializedObject.FindProperty("StartPoint"));
			//EditorGUILayout.PropertyField(serializedObject.FindProperty("EndPoint"));
			//EditorGUILayout.PropertyField(serializedObject.FindProperty("LeavePoint"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("SpawnDataList"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("Target_Path"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("EndTargePath"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("PathList"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("LineRenderer"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("PathData"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("Target_End"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("Target_PathList"));
			
		}
		EditorGUILayout.PropertyField(serializedObject.FindProperty("NewDataName"));
		// 原本的功能保留
		if (GUILayout.Button("新增 生成資料"))
		{
			((SpawnDataEditor)target).CreateSpawnDataAsset();
		}
		EditorGUILayout.PropertyField(serializedObject.FindProperty("CustomPathData"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("SpawnData"));
		if (GUILayout.Button("讀取資料"))
		{
			((SpawnDataEditor)target).LoadingMyData();

		}
		GUILayout.Space(100);
		if (GUILayout.Button("覆蓋資料"))
		{
			((SpawnDataEditor)target).OverwriteData();

		}
		serializedObject.ApplyModifiedProperties();
	}
	private ReorderableList reorderableList;

	private void OnEnable()
	{
		SpawnDataEditor SDEditor = (SpawnDataEditor)target;
		endTargetListProp = serializedObject.FindProperty("EndTargetList");

		reorderableList = new ReorderableList(serializedObject, endTargetListProp, true, true, true, true);

		reorderableList.drawHeaderCallback = (Rect rect) =>
		{
			EditorGUI.LabelField(rect, "End Target List");
		};

		reorderableList.onAddCallback = (ReorderableList list) =>
		{
			serializedObject.Update();
			endTargetListProp.arraySize++;
			serializedObject.ApplyModifiedProperties();

			list.index = endTargetListProp.arraySize - 1;

			SDEditor.SyncTargetPathsWithEndTargePath();
			for (int i = 0; i < SDEditor.EndTargePath.Count; i++)
			{
				SDEditor.GotoTargetPosition(SDEditor.EndTargePath[i], SDEditor.EndTargetList[i]);
			}
		};

		reorderableList.onRemoveCallback = (ReorderableList list) =>
		{
			if (list.index >= 0)
			{
				serializedObject.Update();
				endTargetListProp.DeleteArrayElementAtIndex(list.index);
				serializedObject.ApplyModifiedProperties();

				list.index = Mathf.Clamp(list.index, 0, endTargetListProp.arraySize - 1);

				SDEditor.SyncTargetPathsWithEndTargePath();
				for (int i = 0; i < SDEditor.EndTargePath.Count; i++)
				{
					SDEditor.GotoTargetPosition(SDEditor.EndTargePath[i], SDEditor.EndTargetList[i]);
				}
			}
		};

		reorderableList.onReorderCallback = (ReorderableList list) =>
		{
			serializedObject.ApplyModifiedProperties();

			SDEditor.SyncTargetPathsWithEndTargePath();
			for (int i = 0; i < SDEditor.EndTargePath.Count; i++)
			{
				SDEditor.GotoTargetPosition(SDEditor.EndTargePath[i], SDEditor.EndTargetList[i]);
			}
		};

		reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
		{
			SerializedProperty element = endTargetListProp.GetArrayElementAtIndex(index);
			float lineHeight = EditorGUIUtility.singleLineHeight;

			EditorGUI.PropertyField(
				new Rect(rect.x, rect.y, rect.width, lineHeight),
				element,
				new GUIContent($"Target {index}")
			);

			if (index == reorderableList.index)
			{
				float spacing = 2f;
				float btnW = 30f, btnH = 30f;
				float startX = rect.x + 20;
				float startY = rect.y + lineHeight + spacing + 35f;

				if (GUI.Button(new Rect(startX, startY, 60, btnH), "EditPos"))
				{
					SDEditor.GotoTargetPosition(SDEditor.EndTargePath[index], SDEditor.EndTargetList[index]);
				}

				if (GUI.Button(new Rect(startX + 70, startY, btnW, btnH), "←"))
				{
					SDEditor.EndTargetList[index] = SDEditor.MicroPos(SDEditor.EndTargetList[index], new Vector3(-1f, 0, 0));
					SDEditor.GotoTargetPosition(SDEditor.EndTargePath[index], SDEditor.EndTargetList[index]);
					SDEditor.SetWaypoints(index);
				}
				if (GUI.Button(new Rect(startX , startY - btnH - spacing, btnW, btnH), "X↺"))
				{
					SDEditor.EndTargetList[index] = SDEditor.ReversalPosX(SDEditor.EndTargetList[index]);
					SDEditor.GotoTargetPosition(SDEditor.EndTargePath[index], SDEditor.EndTargetList[index]);
					SDEditor.SetWaypoints(index);
				}
				if (GUI.Button(new Rect(startX + 105, startY - btnH - spacing, btnW, btnH), "↑"))
				{
					SDEditor.EndTargetList[index] = SDEditor.MicroPos(SDEditor.EndTargetList[index], new Vector3(0, -1f, 0));
					SDEditor.GotoTargetPosition(SDEditor.EndTargePath[index], SDEditor.EndTargetList[index]);
					SDEditor.SetWaypoints(index);
				}

				

				if (GUI.Button(new Rect(startX + 105, startY + btnH + spacing, btnW, btnH), "↓"))
				{
					SDEditor.EndTargetList[index] = SDEditor.MicroPos(SDEditor.EndTargetList[index], new Vector3(0, 1f, 0));
					SDEditor.GotoTargetPosition(SDEditor.EndTargePath[index], SDEditor.EndTargetList[index]);
					SDEditor.SetWaypoints(index);
				}
				if (GUI.Button(new Rect(startX , startY + btnH + spacing, btnW, btnH), "Y↺"))
				{
					SDEditor.EndTargetList[index] = SDEditor.ReversalPosY(SDEditor.EndTargetList[index]);
					SDEditor.GotoTargetPosition(SDEditor.EndTargePath[index], SDEditor.EndTargetList[index]);
					SDEditor.SetWaypoints(index);
				}
				if (GUI.Button(new Rect(startX + 140, startY, btnW, btnH), "→"))
				{
					SDEditor.EndTargetList[index] = SDEditor.MicroPos(SDEditor.EndTargetList[index], new Vector3(1f, 0, 0));
					SDEditor.GotoTargetPosition(SDEditor.EndTargePath[index], SDEditor.EndTargetList[index]);
					SDEditor.SetWaypoints(index);
				}

				if (GUI.Button(new Rect(startX + 180, startY, btnW, btnH), "Z←"))
				{
					SDEditor.EndTargetList[index] = SDEditor.MicroPos(SDEditor.EndTargetList[index], new Vector3(0, 0, -1f));
					SDEditor.GotoTargetPosition(SDEditor.EndTargePath[index], SDEditor.EndTargetList[index]);
					SDEditor.SetWaypoints(index);
				}
				if (GUI.Button(new Rect(startX + 215, startY, btnW, btnH), "↺"))
				{
					SDEditor.EndTargetList[index] = SDEditor.ReversalPosZ(SDEditor.EndTargetList[index]);
					SDEditor.GotoTargetPosition(SDEditor.EndTargePath[index], SDEditor.EndTargetList[index]);
					SDEditor.SetWaypoints(index);
				}
				if (GUI.Button(new Rect(startX + 250, startY, btnW, btnH), "Z→"))
				{
					SDEditor.EndTargetList[index] = SDEditor.MicroPos(SDEditor.EndTargetList[index], new Vector3(0, 0, 1f));
					SDEditor.GotoTargetPosition(SDEditor.EndTargePath[index], SDEditor.EndTargetList[index]);
					SDEditor.SetWaypoints(index);
				}
			}
		};

		reorderableList.elementHeightCallback = (int index) =>
		{
			return (index == reorderableList.index)
				? EditorGUIUtility.singleLineHeight + 100f
				: EditorGUIUtility.singleLineHeight;
		};
	}
}
