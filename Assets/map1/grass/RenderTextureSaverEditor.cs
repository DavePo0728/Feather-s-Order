using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;
#if UNITY_EDITOR
[CustomEditor(typeof(SaveRenderTexture))]
public class SaveRenderTextureEditor : Editor
{
	private DefaultAsset folderAsset; // 指定資料夾

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		GUILayout.Space(10);
		GUILayout.Label("🖼️ RenderTexture Saver", EditorStyles.boldLabel);

		// 資料夾拖入
		folderAsset = (DefaultAsset)EditorGUILayout.ObjectField("Save Folder", folderAsset, typeof(DefaultAsset), false);

		// 儲存按鈕
		if (GUILayout.Button("💾 儲存 RenderTexture 到圖檔", GUILayout.Height(30)))
		{
			SaveRenderTexture saver = (SaveRenderTexture)target;

			if (saver.renderTexture == null)
			{
				Debug.LogError("❌ 請指定 RenderTexture");
				return;
			}

			string folderPath = GetFolderPath(folderAsset);
			if (string.IsNullOrEmpty(folderPath))
			{
				Debug.LogError("❌ 請拖入有效資料夾 (DefaultAsset)");
				return;
			}

			SaveRTToFile(saver.renderTexture, folderPath, saver.fileName, saver.saveAsJPG);
		}
	}

	private string GetFolderPath(DefaultAsset folder)
	{
		if (folder == null) return null;

		string path = AssetDatabase.GetAssetPath(folder);
		if (AssetDatabase.IsValidFolder(path))
			return Path.GetFullPath(path);

		return null;
	}

	private void SaveRTToFile(RenderTexture rt, string folder, string name, bool asJPG)
	{
		RenderTexture currentRT = RenderTexture.active;
		RenderTexture.active = rt;

		Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);
		tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
		tex.Apply();

		RenderTexture.active = currentRT;

		byte[] bytes = asJPG ? tex.EncodeToJPG(100) : tex.EncodeToPNG();
		string fullPath = Path.Combine(folder, name);
		File.WriteAllBytes(fullPath, bytes);

		Debug.Log($"✅ RenderTexture 已儲存到: {fullPath}");

		Object.DestroyImmediate(tex);
		AssetDatabase.Refresh();
	}
}
#endif