using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SpawnDataEditor))]
public class SpawnDataEditorEditor : Editor
{
	private bool showGroup1 = false; // 改名字比較直覺
	public override void OnInspectorGUI()
    {

		serializedObject.Update();
		//serializedObject.Update();
		SpawnDataEditor SDEditor = (SpawnDataEditor)target;

		
		
		SerializedProperty iterator = serializedObject.GetIterator();
        SerializedProperty iterator2 = serializedObject.GetIterator();
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
						SDEditor.GotoTargetPosition(SDEditor.StartTarget,SDEditor.spawnData.spawnPosition);
					}
					GUILayout.BeginHorizontal();
					GUILayout.FlexibleSpace(); // 讓上鍵居中
					if (GUILayout.Button("←", GUILayout.Width(30), GUILayout.Height(30)))
					{
						SDEditor.spawnData.spawnPosition = SDEditor.MicroPos(SDEditor.spawnData.spawnPosition,new Vector3(-1f,0,0));
						SDEditor.GotoTargetPosition(SDEditor.StartTarget,SDEditor.spawnData.spawnPosition);
					}
					//GUILayout.FlexibleSpace();
					GUILayout.EndHorizontal();

					// 中（左與右）
					GUILayout.BeginHorizontal();
					
					if (GUILayout.Button("↑", GUILayout.Width(30), GUILayout.Height(30)))
					{
						SDEditor.spawnData.spawnPosition =  SDEditor.MicroPos(SDEditor.spawnData.spawnPosition, new Vector3(0, -1f, 0));
						SDEditor.GotoTargetPosition(SDEditor.StartTarget,SDEditor.spawnData.spawnPosition);
					}

					// 下
					GUILayout.BeginHorizontal();
					//GUILayout.FlexibleSpace(); // 讓下鍵居中
					if (GUILayout.Button("↓", GUILayout.Width(30), GUILayout.Height(30)))
					{
						SDEditor.spawnData.spawnPosition =  SDEditor.MicroPos(SDEditor.spawnData.spawnPosition, new Vector3(0, 1, 0));
						SDEditor.GotoTargetPosition(SDEditor.StartTarget,SDEditor.spawnData.spawnPosition);
					}
					//GUILayout.FlexibleSpace();
					GUILayout.EndHorizontal();
					//GUILayout.Space(10);
					if (GUILayout.Button("→", GUILayout.Width(30), GUILayout.Height(30)))
					{
						SDEditor.spawnData.spawnPosition =  SDEditor.MicroPos(SDEditor.spawnData.spawnPosition, new Vector3(1, 0, 0));
						SDEditor.GotoTargetPosition(SDEditor.StartTarget,SDEditor.spawnData.spawnPosition);
					}
					GUILayout.Space(50);
					if (GUILayout.Button("←", GUILayout.Width(30), GUILayout.Height(30)))
					{
						SDEditor.spawnData.spawnPosition = SDEditor.MicroPos(SDEditor.spawnData.spawnPosition, new Vector3(0, 0, -1));
						SDEditor.GotoTargetPosition(SDEditor.StartTarget,SDEditor.spawnData.spawnPosition);
					}
					if (GUILayout.Button("→", GUILayout.Width(30), GUILayout.Height(30)))
					{
						SDEditor.spawnData.spawnPosition = SDEditor.MicroPos(SDEditor.spawnData.spawnPosition, new Vector3(0, 0, 1f));
						SDEditor.GotoTargetPosition(SDEditor.StartTarget,SDEditor.spawnData.spawnPosition);
					}
					GUILayout.EndHorizontal();
					GUILayout.EndHorizontal();
                }
                SerializedProperty endPosition = iterator.FindPropertyRelative("endPosition");
                if (endPosition != null)
                {
                    EditorGUILayout.PropertyField(endPosition);

                    // 插入按鈕在 spawnPosition 下
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("EditPos"))
                    {
						SDEditor.GotoTargetPosition(SDEditor.EndTarget,SDEditor.spawnData.endPosition);
					}
					GUILayout.BeginHorizontal();
					GUILayout.FlexibleSpace(); // 讓上鍵居中
					if (GUILayout.Button("←", GUILayout.Width(30), GUILayout.Height(30)))
					{
						SDEditor.spawnData.endPosition = SDEditor.MicroPos(SDEditor.spawnData.endPosition, new Vector3(-1f, 0, 0));
						SDEditor.GotoTargetPosition(SDEditor.EndTarget,SDEditor.spawnData.endPosition);
					}
					//GUILayout.FlexibleSpace();
					GUILayout.EndHorizontal();

					// 中（左與右）
					GUILayout.BeginHorizontal();

					if (GUILayout.Button("↑", GUILayout.Width(30), GUILayout.Height(30)))
					{
						SDEditor.spawnData.endPosition =  SDEditor.MicroPos(SDEditor.spawnData.endPosition, new Vector3(0, -1f, 0));
						SDEditor.GotoTargetPosition(SDEditor.EndTarget,SDEditor.spawnData.endPosition);
					}

					// 下
					GUILayout.BeginHorizontal();
					//GUILayout.FlexibleSpace(); // 讓下鍵居中
					if (GUILayout.Button("↓", GUILayout.Width(30), GUILayout.Height(30)))
					{
						SDEditor.spawnData.endPosition =  SDEditor.MicroPos(SDEditor.spawnData.endPosition, new Vector3(0, 1, 0));
						SDEditor.GotoTargetPosition(SDEditor.EndTarget,SDEditor.spawnData.endPosition);
					}
					//GUILayout.FlexibleSpace();
					GUILayout.EndHorizontal();
					//GUILayout.Space(10);
					if (GUILayout.Button("→", GUILayout.Width(30), GUILayout.Height(30)))
					{
						SDEditor.spawnData.endPosition = SDEditor.MicroPos(SDEditor.spawnData.endPosition, new Vector3(1, 0, 0));
						SDEditor.GotoTargetPosition(SDEditor.EndTarget,SDEditor.spawnData.endPosition);
					}
					GUILayout.Space(50);
					if (GUILayout.Button("←", GUILayout.Width(30), GUILayout.Height(30)))
					{
						SDEditor.spawnData.endPosition = SDEditor.MicroPos(SDEditor.spawnData.endPosition, new Vector3(0, 0, -1));
						SDEditor.GotoTargetPosition(SDEditor.EndTarget,SDEditor.spawnData.endPosition);
					}
					if (GUILayout.Button("→", GUILayout.Width(30), GUILayout.Height(30)))
					{
						SDEditor.spawnData.endPosition = SDEditor.MicroPos(SDEditor.spawnData.endPosition, new Vector3(0, 0, 1f));
						SDEditor.GotoTargetPosition(SDEditor.EndTarget,SDEditor.spawnData.endPosition);
					}
					GUILayout.EndHorizontal();
					GUILayout.EndHorizontal();
				}
				SerializedProperty LeavePositon = iterator.FindPropertyRelative("LeavePositon");
				if (LeavePositon != null)
				{
					EditorGUILayout.PropertyField(LeavePositon);

					// 插入按鈕在 spawnPosition 下
					GUILayout.BeginHorizontal();
					if (GUILayout.Button("EditPos"))
					{
						SDEditor.GotoTargetPosition(SDEditor.LeaveTarget,SDEditor.spawnData.LeavePositon);
					}
					GUILayout.BeginHorizontal();
					GUILayout.FlexibleSpace(); // 讓上鍵居中
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
					GUILayout.Space(50);
					if (GUILayout.Button("←", GUILayout.Width(30), GUILayout.Height(30)))
					{
						SDEditor.spawnData.LeavePositon = SDEditor.MicroPos(SDEditor.spawnData.LeavePositon, new Vector3(0, 0, -1));
						SDEditor.GotoTargetPosition(SDEditor.LeaveTarget, SDEditor.spawnData.LeavePositon);
					}
					if (GUILayout.Button("→", GUILayout.Width(30), GUILayout.Height(30)))
					{
						SDEditor.spawnData.LeavePositon = SDEditor.MicroPos(SDEditor.spawnData.LeavePositon, new Vector3(0, 0, 1f));
						SDEditor.GotoTargetPosition(SDEditor.LeaveTarget,SDEditor.spawnData.LeavePositon);
					}
					GUILayout.EndHorizontal();
					GUILayout.EndHorizontal();
				}
				// 略過 spawnData 的其餘內部欄位，防止重複顯示
				break;
            }
		}
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
			EditorGUILayout.PropertyField(serializedObject.FindProperty("StartPoint"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("EndPoint"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("LeavePoint"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("SpawnDataList"));
			
		}
		EditorGUILayout.PropertyField(serializedObject.FindProperty("NewDataName"));
		// 原本的功能保留
		if (GUILayout.Button("新增 生成資料"))
		{
			((SpawnDataEditor)target).CreateSpawnDataAsset();
		}

		serializedObject.ApplyModifiedProperties();
	}
}
