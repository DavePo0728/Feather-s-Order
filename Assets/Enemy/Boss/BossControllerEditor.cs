using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BossController), true)]
[CanEditMultipleObjects]
public class BossControllerEditor : Editor
{
	private bool showController = true;
	private bool showGroup = true;
	private bool showEyeGroup = true; // 改名字比較直覺
	private bool showTentacle_Shooter = true;









	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		// 每個 Foldout 都獨立控制
		showGroup = EditorGUILayout.Foldout(showGroup, "物件", true);
		if (showGroup)
		{
			EditorGUI.indentLevel++;
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("一般眼睛控制群組", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("targetGroup1"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("targetGroup2"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("targetGroup3"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("targetGroup4"));

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("中央眼睛群組", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("downList"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("target"));

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("眼睛花瓣旋轉控制", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("rotationGroup1"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("rotationGroup2"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("rotationGroup3"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("maxRotation1"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("maxRotation2"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("maxRotation3"));

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("眼睛朝向設定", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("Look_Target"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("EyesGroup"));

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("觸手葉片群組 (每個方向5個List)", EditorStyles.boldLabel);
			// 這邊 tentacle group 的一堆，照你的排版繼續列
			EditorGUILayout.PropertyField(serializedObject.FindProperty("tentacleLeftUpGroup1"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("tentacleLeftUpGroup2"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("tentacleLeftUpGroup3"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("tentacleLeftUpGroup4"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("tentacleLeftUpGroup5"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("tentacleRightUpGroup1"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("tentacleRightUpGroup2"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("tentacleRightUpGroup3"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("tentacleRightUpGroup4"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("tentacleRightUpGroup5"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("tentacleLeftDownGroup1"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("tentacleLeftDownGroup2"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("tentacleLeftDownGroup3"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("tentacleLeftDownGroup4"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("tentacleLeftDownGroup5"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("tentacleRightDownGroup1"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("tentacleRightDownGroup2"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("tentacleRightDownGroup3"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("tentacleRightDownGroup4"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("tentacleRightDownGroup5"));

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("觸手花瓣群組", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("tentacleflowerLeftUpGroup1"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("tentacleflowerLeftUpGroup2"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("tentacleflowerLeftUpGroup3"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("tentacleflowerRightUpGroup1"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("tentacleflowerRightUpGroup2"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("tentacleflowerRightUpGroup3"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("tentacleflowerLeftDownGroup1"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("tentacleflowerLeftDownGroup2"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("tentacleflowerLeftDownGroup3"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("tentacleflowerRightDownGroup1"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("tentacleflowerRightDownGroup2"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("tentacleflowerRightDownGroup3"));
			EditorGUI.indentLevel--;
		}

		showController = EditorGUILayout.Foldout(showController, "控制器", true);
		if (showController)
		{
			EditorGUI.indentLevel++;
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("一般控制滑條", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("trigger1"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("trigger2"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("trigger3"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("trigger4"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("specialTrigger"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("newTrigger"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("outTrigger"));

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("觸手葉子滑條", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("tentacleLeftUptrigger"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("tentacleRightUptrigger"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("tentacleLeftDowntrigger"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("tentacleRightDowntrigger"));

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("觸手花瓣滑條", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("tentacleflowerLeftUptrigger"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("tentacleflowerRightUptrigger"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("tentacleflowerLeftDowntrigger"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("tentacleflowerRightDowntrigger"));
			EditorGUI.indentLevel--;
		}

		showEyeGroup = EditorGUILayout.Foldout(showEyeGroup, "旋轉控制", true);
		if (showEyeGroup)
		{
			EditorGUI.indentLevel++;
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("觸手葉片最大旋轉", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("maxLeafRotation1"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("maxLeafRotation2"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("maxLeafRotation3"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("maxLeafRotation4"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("maxLeafRotation5"));

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("中央花瓣最大旋轉", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("maxRotation1"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("maxRotation2"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("maxRotation3"));

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("觸手花瓣最大旋轉", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("maxFlowerRotation1"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("maxFlowerRotation2"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("maxFlowerRotation3"));
			EditorGUI.indentLevel--;
		}
		showTentacle_Shooter = EditorGUILayout.Foldout(showTentacle_Shooter, "發射器", true);
		if (showTentacle_Shooter)
		{

			EditorGUILayout.PropertyField(serializedObject.FindProperty("movementSpeed"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("maxMoveDistance"));
			// 顯示 UpRightAim 和對應的偏移量
			EditorGUILayout.PropertyField(serializedObject.FindProperty("UpRightAim"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("Tentacle_ShootingUpRight"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("MainSight1"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("EyesTrigger1"), new GUIContent("追蹤倍率"));
			//EditorGUILayout.PropertyField(serializedObject.FindProperty("TentacleRotaRightUpX"), new GUIContent("旋轉偏移X"));
			//EditorGUILayout.PropertyField(serializedObject.FindProperty("TentacleRotaRightUpY"), new GUIContent("旋轉偏移Y"));
			EditorGUILayout.Space();

			// 顯示 UpLeftAim 和對應的偏移量
			EditorGUILayout.PropertyField(serializedObject.FindProperty("UpLeftAim"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("Tentacle_ShootingUpLeft"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("MainSight2"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("EyesTrigger2"), new GUIContent("追蹤倍率"));
			//EditorGUILayout.PropertyField(serializedObject.FindProperty("TentacleRotaLeftUpX"), new GUIContent("旋轉偏移X"));
			//EditorGUILayout.PropertyField(serializedObject.FindProperty("TentacleRotaLeftUpY"), new GUIContent("旋轉偏移Y"));
			EditorGUILayout.Space();

			// 顯示 DownRightAim 和對應的偏移量
			EditorGUILayout.PropertyField(serializedObject.FindProperty("DownRightAim"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("Tentacle_ShootingDownRight"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("MainSight3"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("EyesTrigger3"), new GUIContent("追蹤倍率")); 
			//EditorGUILayout.PropertyField(serializedObject.FindProperty("TentacleRotaRightDownX"), new GUIContent("旋轉偏移X"));
			//EditorGUILayout.PropertyField(serializedObject.FindProperty("TentacleRotaRightDownY"), new GUIContent("旋轉偏移Y"));
			EditorGUILayout.Space();

			// 顯示 DownLeftAim 和對應的偏移量
			EditorGUILayout.PropertyField(serializedObject.FindProperty("DownLeftAim"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("Tentacle_ShootingDownLeft"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("MainSight4"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("EyesTrigger4"), new GUIContent("追蹤倍率"));
			//EditorGUILayout.PropertyField(serializedObject.FindProperty("TentacleRotaLeftDownX"), new GUIContent("旋轉偏移X"));
			//EditorGUILayout.PropertyField(serializedObject.FindProperty("TentacleRotaLeftDownY"), new GUIContent("旋轉偏移Y"));
		}



		serializedObject.ApplyModifiedProperties();
	}
}
