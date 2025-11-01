using System;
using UnityEngine;

[Serializable]
public class SettingsData
{
    // Locale
    public string localeCode = "en"; // "ja", "en", "zh-Hant"


    // Audio (0..1)
    public float masterVolume = 1f;
    public float musicVolume = 1f;
    public float sfxVolume = 1f;


    // Graphics (pending/apply pattern)
    public GraphicsSettingsData appliedGraphics = new GraphicsSettingsData();
    public GraphicsSettingsData pendingGraphics = new GraphicsSettingsData();
    public GraphicsSettingsData defaultLowGraphics = new GraphicsSettingsData();
    public GraphicsSettingsData defaultMediumGraphics = new GraphicsSettingsData();
    public GraphicsSettingsData defaultHighGraphics = new GraphicsSettingsData();

    // Input options (beyond rebinds)
    public bool invertLookY = false;
    public float lookSensitivity = 1.0f; // multiplier
    public float leftStickDeadzone = 0.125f; // 0..0.5 typical


    // Input System binding overrides JSON (per-ActionAsset)
    public string inputBindingOverridesJson = string.Empty;
}


[Serializable]
public class GraphicsSettingsData
{
    public FullScreenMode fullscreenMode = FullScreenMode.Windowed; // Display Mode
    //public Resolution resolution = new Resolution() { width = 1920, height = 1080};
    public int width = 1920;
    public int height = 1080;
    public int targetFramerate = 60; // Application.targetFrameRate
    public bool vSync = false; // QualitySettings.vSyncCount (0 or 1)


    // Brightness: logical 0..1; implementation-dependent
    public float brightness = 0.0f;

    public int textureResoluion = 2; // 0=Full, 1=Half, 2=Quarter (Unity default)

    // Anti-aliasing (MSAA) 0,2,4,8 (built-in) or URP MSAA sample count
    public int msaa = 2;


    // Anisotropic filtering
    public AnisotropicFiltering anisotropic = AnisotropicFiltering.Disable;


    // Quality level name (must exist in ProjectSettings/Quality)
    public string qualityLevelName = "Low";
}
