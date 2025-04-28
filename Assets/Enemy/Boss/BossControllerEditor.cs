//using UnityEditor;
//using UnityEngine;

//[CustomEditor(typeof(BossController))]
//public class BossControllerEditor : Editor
//{
//	SerializedProperty trigger1, trigger2, trigger3, trigger4;
//	SerializedProperty specialTrigger, newTrigger, outTrigger;
//	SerializedProperty targetGroup1, targetGroup2, targetGroup3, targetGroup4;
//	SerializedProperty downList, target;
//	SerializedProperty rotationGroup1, rotationGroup2, rotationGroup3;
//	SerializedProperty maxRotation1, maxRotation2, maxRotation3;
//	SerializedProperty Look_Target, EyesGroup;

//	// 觸手葉片與花瓣用的
//	SerializedProperty tentacleLeftUptrigger, tentacleRightUptrigger, tentacleLeftDowntrigger, tentacleRightDowntrigger;
//	SerializedProperty tentacleflowerLeftUptrigger, tentacleflowerRightUptrigger, tentacleflowerLeftDowntrigger, tentacleflowerRightDowntrigger;

//	SerializedProperty tentacleLeftUpGroup1, tentacleLeftUpGroup2, tentacleLeftUpGroup3, tentacleLeftUpGroup4, tentacleLeftUpGroup5;
//	SerializedProperty tentacleRightUpGroup1, tentacleRightUpGroup2, tentacleRightUpGroup3, tentacleRightUpGroup4, tentacleRightUpGroup5;
//	SerializedProperty tentacleLeftDownGroup1, tentacleLeftDownGroup2, tentacleLeftDownGroup3, tentacleLeftDownGroup4, tentacleLeftDownGroup5;
//	SerializedProperty tentacleRightDownGroup1, tentacleRightDownGroup2, tentacleRightDownGroup3, tentacleRightDownGroup4, tentacleRightDownGroup5;

//	SerializedProperty maxLeafRotation1, maxLeafRotation2, maxLeafRotation3, maxLeafRotation4, maxLeafRotation5;

//	SerializedProperty tentacleflowerLeftUpGroup1, tentacleflowerLeftUpGroup2, tentacleflowerLeftUpGroup3;
//	SerializedProperty tentacleflowerRightUpGroup1, tentacleflowerRightUpGroup2, tentacleflowerRightUpGroup3;
//	SerializedProperty tentacleflowerLeftDownGroup1, tentacleflowerLeftDownGroup2, tentacleflowerLeftDownGroup3;
//	SerializedProperty tentacleflowerRightDownGroup1, tentacleflowerRightDownGroup2, tentacleflowerRightDownGroup3;

//	SerializedProperty maxFlowerRotation1, maxFlowerRotation2, maxFlowerRotation3;
//	void OnEnable()
//	{
//		trigger1 = serializedObject.FindProperty("trigger1");
//		trigger2 = serializedObject.FindProperty("trigger2");
//		trigger3 = serializedObject.FindProperty("trigger3");
//		trigger4 = serializedObject.FindProperty("trigger4");
//		specialTrigger = serializedObject.FindProperty("specialTrigger");
//		newTrigger = serializedObject.FindProperty("newTrigger");
//		outTrigger = serializedObject.FindProperty("outTrigger");

//		targetGroup1 = serializedObject.FindProperty("targetGroup1");
//		targetGroup2 = serializedObject.FindProperty("targetGroup2");
//		targetGroup3 = serializedObject.FindProperty("targetGroup3");
//		targetGroup4 = serializedObject.FindProperty("targetGroup4");

//		downList = serializedObject.FindProperty("downList");
//		target = serializedObject.FindProperty("target");

//		rotationGroup1 = serializedObject.FindProperty("rotationGroup1");
//		rotationGroup2 = serializedObject.FindProperty("rotationGroup2");
//		rotationGroup3 = serializedObject.FindProperty("rotationGroup3");

//		maxRotation1 = serializedObject.FindProperty("maxRotation1");
//		maxRotation2 = serializedObject.FindProperty("maxRotation2");
//		maxRotation3 = serializedObject.FindProperty("maxRotation3");

//		Look_Target = serializedObject.FindProperty("Look_Target");
//		EyesGroup = serializedObject.FindProperty("EyesGroup");

//		// 葉片 花瓣 滑條
//		tentacleLeftUptrigger = serializedObject.FindProperty("tentacleLeftUptrigger");
//		tentacleRightUptrigger = serializedObject.FindProperty("tentacleRightUptrigger");
//		tentacleLeftDowntrigger = serializedObject.FindProperty("tentacleLeftDowntrigger");
//		tentacleRightDowntrigger = serializedObject.FindProperty("tentacleRightDowntrigger");

//		tentacleflowerLeftUptrigger = serializedObject.FindProperty("tentacleflowerLeftUptrigger");
//		tentacleflowerRightUptrigger = serializedObject.FindProperty("tentacleflowerRightUptrigger");
//		tentacleflowerLeftDowntrigger = serializedObject.FindProperty("tentacleflowerLeftDowntrigger");
//		tentacleflowerRightDowntrigger = serializedObject.FindProperty("tentacleflowerRightDowntrigger");

//		// 葉片 List
//		tentacleLeftUpGroup1 = serializedObject.FindProperty("tentacleLeftUpGroup1");
//		tentacleLeftUpGroup2 = serializedObject.FindProperty("tentacleLeftUpGroup2");
//		tentacleLeftUpGroup3 = serializedObject.FindProperty("tentacleLeftUpGroup3");
//		tentacleLeftUpGroup4 = serializedObject.FindProperty("tentacleLeftUpGroup4");
//		tentacleLeftUpGroup5 = serializedObject.FindProperty("tentacleLeftUpGroup5");

//		tentacleRightUpGroup1 = serializedObject.FindProperty("tentacleRightUpGroup1");
//		tentacleRightUpGroup2 = serializedObject.FindProperty("tentacleRightUpGroup2");
//		tentacleRightUpGroup3 = serializedObject.FindProperty("tentacleRightUpGroup3");
//		tentacleRightUpGroup4 = serializedObject.FindProperty("tentacleRightUpGroup4");
//		tentacleRightUpGroup5 = serializedObject.FindProperty("tentacleRightUpGroup5");

//		tentacleLeftDownGroup1 = serializedObject.FindProperty("tentacleLeftDownGroup1");
//		tentacleLeftDownGroup2 = serializedObject.FindProperty("tentacleLeftDownGroup2");
//		tentacleLeftDownGroup3 = serializedObject.FindProperty("tentacleLeftDownGroup3");
//		tentacleLeftDownGroup4 = serializedObject.FindProperty("tentacleLeftDownGroup4");
//		tentacleLeftDownGroup5 = serializedObject.FindProperty("tentacleLeftDownGroup5");

//		tentacleRightDownGroup1 = serializedObject.FindProperty("tentacleRightDownGroup1");
//		tentacleRightDownGroup2 = serializedObject.FindProperty("tentacleRightDownGroup2");
//		tentacleRightDownGroup3 = serializedObject.FindProperty("tentacleRightDownGroup3");
//		tentacleRightDownGroup4 = serializedObject.FindProperty("tentacleRightDownGroup4");
//		tentacleRightDownGroup5 = serializedObject.FindProperty("tentacleRightDownGroup5");

//		maxLeafRotation1 = serializedObject.FindProperty("maxLeafRotation1");
//		maxLeafRotation2 = serializedObject.FindProperty("maxLeafRotation2");
//		maxLeafRotation3 = serializedObject.FindProperty("maxLeafRotation3");
//		maxLeafRotation4 = serializedObject.FindProperty("maxLeafRotation4");
//		maxLeafRotation5 = serializedObject.FindProperty("maxLeafRotation5");

//		// 花瓣 List
//		tentacleflowerLeftUpGroup1 = serializedObject.FindProperty("tentacleflowerLeftUpGroup1");
//		tentacleflowerLeftUpGroup2 = serializedObject.FindProperty("tentacleflowerLeftUpGroup2");
//		tentacleflowerLeftUpGroup3 = serializedObject.FindProperty("tentacleflowerLeftUpGroup3");

//		tentacleflowerRightUpGroup1 = serializedObject.FindProperty("tentacleflowerRightUpGroup1");
//		tentacleflowerRightUpGroup2 = serializedObject.FindProperty("tentacleflowerRightUpGroup2");
//		tentacleflowerRightUpGroup3 = serializedObject.FindProperty("tentacleflowerRightUpGroup3");

//		tentacleflowerLeftDownGroup1 = serializedObject.FindProperty("tentacleflowerLeftDownGroup1");
//		tentacleflowerLeftDownGroup2 = serializedObject.FindProperty("tentacleflowerLeftDownGroup2");
//		tentacleflowerLeftDownGroup3 = serializedObject.FindProperty("tentacleflowerLeftDownGroup3");

//		tentacleflowerRightDownGroup1 = serializedObject.FindProperty("tentacleflowerRightDownGroup1");
//		tentacleflowerRightDownGroup2 = serializedObject.FindProperty("tentacleflowerRightDownGroup2");
//		tentacleflowerRightDownGroup3 = serializedObject.FindProperty("tentacleflowerRightDownGroup3");

//		maxFlowerRotation1 = serializedObject.FindProperty("maxFlowerRotation1");
//		maxFlowerRotation2 = serializedObject.FindProperty("maxFlowerRotation2");
//		maxFlowerRotation3 = serializedObject.FindProperty("maxFlowerRotation3");

//		maxRotation1 = serializedObject.FindProperty("maxRotation1");
//		maxRotation2 = serializedObject.FindProperty("maxRotation2");
//		maxRotation3 = serializedObject.FindProperty("maxRotation3");
	
//	}

//	public override void OnInspectorGUI()
//	{
//		serializedObject.Update();

//		GUILayout.Label("中央眼睛與花瓣基本控制", EditorStyles.boldLabel);
//		EditorGUILayout.PropertyField(trigger1, new GUIContent("右下眼睛"));
//		EditorGUILayout.PropertyField(trigger2, new GUIContent("左下眼睛"));
//		EditorGUILayout.PropertyField(trigger3, new GUIContent("右上眼睛"));
//		EditorGUILayout.PropertyField(trigger4, new GUIContent("左上眼睛"));
//		EditorGUILayout.PropertyField(specialTrigger, new GUIContent("中央眼睛"));
//		EditorGUILayout.PropertyField(newTrigger, new GUIContent("眼睛隱藏"));
//		EditorGUILayout.PropertyField(outTrigger, new GUIContent("花瓣開合"));

//		GUILayout.Space(10);

//		DrawTentacleSection("左上葉片與花瓣控制", tentacleLeftUptrigger, tentacleflowerLeftUptrigger,
//			tentacleLeftUpGroup1, tentacleLeftUpGroup2, tentacleLeftUpGroup3, tentacleLeftUpGroup4, tentacleLeftUpGroup5,
//			tentacleflowerLeftUpGroup1, tentacleflowerLeftUpGroup2, tentacleflowerLeftUpGroup3);

//		DrawTentacleSection("右上葉片與花瓣控制", tentacleRightUptrigger, tentacleflowerRightUptrigger,
//			tentacleRightUpGroup1, tentacleRightUpGroup2, tentacleRightUpGroup3, tentacleRightUpGroup4, tentacleRightUpGroup5,
//			tentacleflowerRightUpGroup1, tentacleflowerRightUpGroup2, tentacleflowerRightUpGroup3);

//		DrawTentacleSection("左下葉片與花瓣控制", tentacleLeftDowntrigger, tentacleflowerLeftDowntrigger,
//			tentacleLeftDownGroup1, tentacleLeftDownGroup2, tentacleLeftDownGroup3, tentacleLeftDownGroup4, tentacleLeftDownGroup5,
//			tentacleflowerLeftDownGroup1, tentacleflowerLeftDownGroup2, tentacleflowerLeftDownGroup3);

//		DrawTentacleSection("右下葉片與花瓣控制", tentacleRightDowntrigger, tentacleflowerRightDowntrigger,
//			tentacleRightDownGroup1, tentacleRightDownGroup2, tentacleRightDownGroup3, tentacleRightDownGroup4, tentacleRightDownGroup5,
//			tentacleflowerRightDownGroup1, tentacleflowerRightDownGroup2, tentacleflowerRightDownGroup3);

//		// --- 最大旋轉值 ---
//		GUILayout.Space(5);
//		GUILayout.Label("觸手最大旋轉值", EditorStyles.boldLabel);
//		EditorGUILayout.PropertyField(maxRotation1, new GUIContent("中央花瓣1_旋轉Max"));
//		EditorGUILayout.PropertyField(maxRotation2, new GUIContent("中央花瓣2_旋轉Max"));
//		EditorGUILayout.PropertyField(maxRotation3, new GUIContent("中央花瓣3_旋轉Max"));

//		EditorGUILayout.PropertyField(maxLeafRotation1, new GUIContent("觸手葉片1_旋轉Max"));
//		EditorGUILayout.PropertyField(maxLeafRotation2, new GUIContent("觸手葉片2_旋轉Max"));
//		EditorGUILayout.PropertyField(maxLeafRotation3, new GUIContent("觸手葉片3_旋轉Max"));
//		EditorGUILayout.PropertyField(maxLeafRotation4, new GUIContent("觸手葉片4_旋轉Max"));
//		EditorGUILayout.PropertyField(maxLeafRotation5, new GUIContent("觸手葉片5_旋轉Max"));

//		EditorGUILayout.PropertyField(maxFlowerRotation1, new GUIContent("觸手花瓣1_旋轉Max"));
//		EditorGUILayout.PropertyField(maxFlowerRotation2, new GUIContent("觸手花瓣2_旋轉Max"));
//		EditorGUILayout.PropertyField(maxFlowerRotation3, new GUIContent("觸手花瓣3_旋轉Max"));


//		GUILayout.Space(10);

//		GUILayout.Label("眼睛朝向設定", EditorStyles.boldLabel);
//		EditorGUILayout.PropertyField(Look_Target, new GUIContent("朝向目標"));
//		EditorGUILayout.PropertyField(target, new GUIContent("中央眼睛目標"));
//		EditorGUILayout.PropertyField(EyesGroup, true);

//		serializedObject.ApplyModifiedProperties();
//	}

//	private void DrawTentacleSection(string title, SerializedProperty leafTrigger, SerializedProperty flowerTrigger,
//		SerializedProperty group1, SerializedProperty group2, SerializedProperty group3, SerializedProperty group4, SerializedProperty group5,
//		SerializedProperty flowerGroup1, SerializedProperty flowerGroup2, SerializedProperty flowerGroup3)
//	{
//		GUILayout.Space(10);
//		GUILayout.Label(title, EditorStyles.boldLabel);
//		EditorGUILayout.PropertyField(leafTrigger, new GUIContent("葉片開合滑條"));
//		EditorGUILayout.PropertyField(group1, new GUIContent("葉片群組1"), true);
//		EditorGUILayout.PropertyField(group2, new GUIContent("葉片群組2"), true);
//		EditorGUILayout.PropertyField(group3, new GUIContent("葉片群組3"), true);
//		EditorGUILayout.PropertyField(group4, new GUIContent("葉片群組4"), true);
//		EditorGUILayout.PropertyField(group5, new GUIContent("葉片群組5"), true);

//		EditorGUILayout.PropertyField(flowerTrigger, new GUIContent("花瓣開合滑條"));
//		EditorGUILayout.PropertyField(flowerGroup1, new GUIContent("花瓣群組1"), true);
//		EditorGUILayout.PropertyField(flowerGroup2, new GUIContent("花瓣群組2"), true);
//		EditorGUILayout.PropertyField(flowerGroup3, new GUIContent("花瓣群組3"), true);
//	}
//}
