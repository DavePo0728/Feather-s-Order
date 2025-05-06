//using UnityEditor;
//using UnityEngine;

//[CustomPropertyDrawer(typeof(SpawnGroupData))]
//public class SpawnGroupDataDrawer : PropertyDrawer
//{
//	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//	{
//		EditorGUI.BeginProperty(position, label, property);

//		SerializedProperty spawnDataTypeProp = property.FindPropertyRelative("_spawnDataType");

//		EditorGUI.BeginChangeCheck();
//		EditorGUI.PropertyField(position, spawnDataTypeProp, new GUIContent("Spawn Data Type"));
//		if (EditorGUI.EndChangeCheck())
//		{
//			// 印出改變訊息
//			Debug.Log("spawnDataType 被改變了");
//		}

//		EditorGUI.EndProperty();
//	}
//}
