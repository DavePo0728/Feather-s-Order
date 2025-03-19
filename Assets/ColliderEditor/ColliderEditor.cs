using System.IO;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(SkinnedMeshRenderer), typeof(MeshCollider))]
public class ColliderEditor : MonoBehaviour
{
    [Header("碰撞體名稱(相同覆蓋)")]
    public string meshName = "NewMesh";
#if UNITY_EDITOR
    [Header("儲存路径（選擇文件夹）")]
    public DefaultAsset saveFolder; // 允许选择文件夹
#endif
    public void GenerateAndAssignMesh()
    {
        #if UNITY_EDITOR
        if (saveFolder == null)
        {
            Debug.LogError("请指定存储的文件夹！");
            return;
        }

        SkinnedMeshRenderer skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        MeshCollider meshCollider = GetComponent<MeshCollider>();

        if (skinnedMeshRenderer == null || meshCollider == null)
        {
            Debug.LogError("Missing SkinnedMeshRenderer or MeshCollider component!");
            return;
        }

        // 获取文件夹路径
        string folderPath = AssetDatabase.GetAssetPath(saveFolder);
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            Debug.LogError("指定路徑無效！");
            return;
        }

        string path = folderPath + "/" + meshName + ".asset";
        Mesh bakedMesh = new Mesh();
        skinnedMeshRenderer.BakeMesh(bakedMesh);
        bakedMesh.name = meshName;

        Mesh existingMesh = AssetDatabase.LoadAssetAtPath<Mesh>(path);
        if (existingMesh != null)
        {
            existingMesh.Clear();
            EditorUtility.CopySerialized(bakedMesh, existingMesh);
            Debug.Log("Updated existing Mesh: " + path);
        }
        else
        {
            AssetDatabase.CreateAsset(bakedMesh, path);
            Debug.Log("Created new Mesh: " + path);
        }

        AssetDatabase.SaveAssets();
        meshCollider.sharedMesh = AssetDatabase.LoadAssetAtPath<Mesh>(path);
        #endif
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(ColliderEditor))]
public class ColliderEditorInspector : Editor
{
    SerializedProperty meshNameProp;
    SerializedProperty saveFolderProp;

    private void OnEnable()
    {
        meshNameProp = serializedObject.FindProperty("meshName");
        saveFolderProp = serializedObject.FindProperty("saveFolder");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(meshNameProp, new GUIContent("碰撞体名称(相同覆盖)"));
        EditorGUILayout.PropertyField(saveFolderProp, new GUIContent("存储文件夹"));

        ColliderEditor script = (ColliderEditor)target;

        if (GUILayout.Button("生成网格"))
        {
            script.GenerateAndAssignMesh();
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif