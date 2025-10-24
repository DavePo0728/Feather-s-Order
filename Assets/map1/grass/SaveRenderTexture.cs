using UnityEngine;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SaveRenderTexture : MonoBehaviour
{
	public RenderTexture renderTexture;
	public string fileName = "RenderOutput.png";
	public bool saveAsJPG = false;

#if UNITY_EDITOR
	public DefaultAsset folderAsset; // 拖資料夾進 Inspector
#endif

	void OnGUI()
	{
		if (GUILayout.Button("Save RenderTexture"))
		{
#if UNITY_EDITOR
			string folderPath = GetFolderPath(folderAsset);
			if (string.IsNullOrEmpty(folderPath))
			{
				Debug.LogError("❌ 請指定有效的資料夾（拖 Project 資料夾）");
				return;
			}

			SaveRTToFile(renderTexture, folderPath, fileName, saveAsJPG);
#else
            Debug.LogWarning("❌ 這個存檔功能只能在 Editor 模式使用！");
#endif
		}
	}

#if UNITY_EDITOR
	private string GetFolderPath(DefaultAsset folder)
	{
		if (folder == null)
			return null;

		string path = AssetDatabase.GetAssetPath(folder);
		if (AssetDatabase.IsValidFolder(path))
			return Path.GetFullPath(path);
		return null;
	}
#endif

	private void SaveRTToFile(RenderTexture rt, string folder, string name, bool asJPG)
	{
		if (rt == null)
		{
			Debug.LogError("❌ RenderTexture 是空的！");
			return;
		}

		if (!Directory.Exists(folder))
			Directory.CreateDirectory(folder);

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
	}
}
