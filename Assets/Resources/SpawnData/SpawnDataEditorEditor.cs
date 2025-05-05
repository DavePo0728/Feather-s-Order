using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SpawnDataEditor))]
public class SpawnDataEditorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //serializedObject.Update();

        SerializedProperty iterator = serializedObject.GetIterator();
        SerializedProperty iterator2 = serializedObject.GetIterator();
        iterator.NextVisible(true); // 跳過 script 欄位
		EditorGUILayout.PropertyField(serializedObject.FindProperty("Target"));
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
                        Debug.Log("EditPos clicked: " + spawnPosProp.vector3Value);
                    }
					GUILayout.BeginHorizontal();
					GUILayout.FlexibleSpace(); // 讓上鍵居中
					if (GUILayout.Button("←", GUILayout.Width(30), GUILayout.Height(30)))
					{
						Debug.Log("Move Left");
					}
					GUILayout.FlexibleSpace();
					GUILayout.EndHorizontal();

					// 中（左與右）
					GUILayout.BeginHorizontal();
					
					if (GUILayout.Button("↑", GUILayout.Width(30), GUILayout.Height(30)))
					{
						Debug.Log("Move Up");
					}

					// 下
					GUILayout.BeginHorizontal();
					GUILayout.FlexibleSpace(); // 讓下鍵居中
					if (GUILayout.Button("↓", GUILayout.Width(30), GUILayout.Height(30)))
					{
						Debug.Log("Move Down");
					}
					GUILayout.FlexibleSpace();
					GUILayout.EndHorizontal();
					GUILayout.Space(10);
					if (GUILayout.Button("→", GUILayout.Width(30), GUILayout.Height(30)))
					{
						Debug.Log("Move Right");
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
                        Debug.Log("EditPos clicked: " + endPosition.vector3Value);
                    }
					GUILayout.BeginHorizontal();
					GUILayout.FlexibleSpace(); // 讓上鍵居中
					if (GUILayout.Button("←", GUILayout.Width(30), GUILayout.Height(30)))
					{
						Debug.Log("Move Left");
					}
					GUILayout.FlexibleSpace();
					GUILayout.EndHorizontal();

					// 中（左與右）
					GUILayout.BeginHorizontal();

					if (GUILayout.Button("↑", GUILayout.Width(30), GUILayout.Height(30)))
					{
						Debug.Log("Move Up");
					}

					// 下
					GUILayout.BeginHorizontal();
					GUILayout.FlexibleSpace(); // 讓下鍵居中
					if (GUILayout.Button("↓", GUILayout.Width(30), GUILayout.Height(30)))
					{
						Debug.Log("Move Down");
					}
					GUILayout.FlexibleSpace();
					GUILayout.EndHorizontal();
					GUILayout.Space(10);
					if (GUILayout.Button("→", GUILayout.Width(30), GUILayout.Height(30)))
					{
						Debug.Log("Move Right");
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
						Debug.Log("EditPos clicked: " + LeavePositon.vector3Value);
					}
					GUILayout.BeginHorizontal();
					GUILayout.FlexibleSpace(); // 讓上鍵居中
					if (GUILayout.Button("←", GUILayout.Width(30), GUILayout.Height(30)))
					{
						Debug.Log("Move Left");
					}
					GUILayout.FlexibleSpace();
					GUILayout.EndHorizontal();

					// 中（左與右）
					GUILayout.BeginHorizontal();

					if (GUILayout.Button("↑", GUILayout.Width(30), GUILayout.Height(30)))
					{
						Debug.Log("Move Up");
					}

					// 下
					GUILayout.BeginHorizontal();
					GUILayout.FlexibleSpace(); // 讓下鍵居中
					if (GUILayout.Button("↓", GUILayout.Width(30), GUILayout.Height(30)))
					{
						Debug.Log("Move Down");
					}
					GUILayout.FlexibleSpace();
					GUILayout.EndHorizontal();
					GUILayout.Space(10);
					if (GUILayout.Button("→", GUILayout.Width(30), GUILayout.Height(30)))
					{
						Debug.Log("Move Right");
					}
					GUILayout.EndHorizontal();
					GUILayout.EndHorizontal();
				}
				// 略過 spawnData 的其餘內部欄位，防止重複顯示
				break;
            }
		}

        serializedObject.ApplyModifiedProperties();

        // 原本的功能保留
        if (GUILayout.Button("判斷 Spawn 區域"))
        {
            ((SpawnDataEditor)target).PrintSpawnRegion();
        }
    }
}
