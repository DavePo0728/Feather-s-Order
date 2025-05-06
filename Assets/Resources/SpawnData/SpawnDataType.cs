#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text;

public static class SpawnDataEnumGenerator
{
	[MenuItem("Tools/Generate Enums/Generate All Data Enums")]
	public static void GenerateAllEnums()
	{
		GenerateEnum<SpawnDataList>("SpawnDataType", "spawnDatas");
		GenerateEnum<EnemyDataList>("EnemyDataType", "enemyDatas");
		Debug.Log("所有 Enum 皆已產生。");
	}

	private static void GenerateEnum<T>(string enumName, string fieldName) where T : ScriptableObject
	{
		string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
		if (guids.Length == 0)
		{
			Debug.LogError($"找不到 {typeof(T).Name} 資產。");
			return;
		}

		string path = AssetDatabase.GUIDToAssetPath(guids[0]);
		T dataList = AssetDatabase.LoadAssetAtPath<T>(path);

		var so = new SerializedObject(dataList);
		var prop = so.FindProperty(fieldName);

		if (prop == null || prop.arraySize == 0)
		{
			Debug.LogError($"{typeof(T).Name} 的 {fieldName} 為空或未設置。");
			return;
		}

		StringBuilder sb = new StringBuilder();
		sb.AppendLine("// 自動產生的 enum，請勿手動修改");
		sb.AppendLine($"public enum {enumName}");
		sb.AppendLine("{");

		for (int i = 0; i < prop.arraySize; i++)
		{
			var element = prop.GetArrayElementAtIndex(i);
			var objRef = element.objectReferenceValue;
			if (objRef != null)
			{
				string safeName = objRef.name.Replace(" ", "_").Replace("-", "_");
				sb.AppendLine($"\t{safeName},");
			}
		}

		sb.AppendLine("}");

		string enumPath = $"Assets/Scripts/Generated/{enumName}.cs";
		Directory.CreateDirectory(Path.GetDirectoryName(enumPath));
		File.WriteAllText(enumPath, sb.ToString());

		AssetDatabase.Refresh();
		Debug.Log($"{enumName} enum 已產生。");
	}
}
#endif
