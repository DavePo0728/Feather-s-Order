using System.IO;
using UnityEngine;


public static class SettingsStore
{
    private static readonly string FilePath = Path.Combine(Application.persistentDataPath, "game_settings.json");


    public static SettingsData LoadOrCreate()
    {
        try
        {
            if (File.Exists(FilePath))
            {
                var json = File.ReadAllText(FilePath);
                //Debug.Log($"Loaded settings from {FilePath}:\n{json}");
                var data = JsonUtility.FromJson<SettingsData>(json);
                if (data != null) return data;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to LoadOrCreate settings: {e}");
        }
        return new SettingsData();
    }


    public static void Save(SettingsData data)
    {
        try
        {
            var json = JsonUtility.ToJson(data, prettyPrint: true);
            File.WriteAllText(FilePath, json);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save settings: {e}");
        }
    }
}