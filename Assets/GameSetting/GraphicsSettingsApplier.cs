using UnityEngine;
//#if UNITY_RENDER_PIPELINE_UNIVERSAL
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
//#endif

public static class GraphicsSettingsApplier
{
//#if UNITY_RENDER_PIPELINE_UNIVERSAL
    public static Volume globalVolume; // assign a Volume with Color Adjustments

    //#endif
    public static void ApplyGraphics(GraphicsSettingsData g)
    {
        
        // VSync and target frame rate
        QualitySettings.vSyncCount = g.vSync ? 1 : 0;
        if (g.width <= 0 || g.height <= 0) return;
        Screen.SetResolution(g.width, g.height, g.fullscreenMode);
        Application.targetFrameRate = g.targetFramerate;

        // MSAA
        //#if UNITY_RENDER_PIPELINE_UNIVERSAL
        var urpAsset = QualitySettings.renderPipeline as UniversalRenderPipelineAsset;
        if (urpAsset != null)
        {
            urpAsset.msaaSampleCount = Mathf.Clamp(g.msaa, 0, 8);
        }
//        else
//#endif
//        {
//            QualitySettings.antiAliasing = Mathf.Clamp(g.msaa, 0, 8);
//        }

        //texture resolution

        QualitySettings.globalTextureMipmapLimit = g.textureResoluion;

        // Anisotropic
        QualitySettings.anisotropicFiltering = g.anisotropic;

        globalVolume = globalVolume ? globalVolume : GameObject.FindObjectOfType<Volume>();
        if (globalVolume != null && globalVolume.profile.TryGet<LiftGammaGain>(out var gain))
        {

            gain.gain.Override(new Vector4(0,0,0,g.brightness));
            //Debug.Log($"Set brightness EV {g.brightness}");
        }
    }
    public static void SetDeafaultGraphics(SettingsData settings)
    {

        settings.defaultLowGraphics = new GraphicsSettingsData()
        {
            width = settings.appliedGraphics.width,
            height = settings.appliedGraphics.height,
            fullscreenMode = settings.appliedGraphics.fullscreenMode,
            targetFramerate = settings.appliedGraphics.targetFramerate,
            vSync = false,
            brightness = settings.appliedGraphics.brightness,
            textureResoluion = 2,
            msaa = 2,
            anisotropic = AnisotropicFiltering.Disable,
            qualityLevelName = "Low"
        };
        settings.defaultMediumGraphics = new GraphicsSettingsData()
        {
            width = settings.appliedGraphics.width,
            height = settings.appliedGraphics.height,
            fullscreenMode = settings.appliedGraphics.fullscreenMode,
            targetFramerate = settings.appliedGraphics.targetFramerate,
            vSync = true,
            brightness = settings.appliedGraphics.brightness,
            textureResoluion = 1,
            msaa = 4,
            anisotropic = AnisotropicFiltering.Enable,
            qualityLevelName = "Medium"
        };
        settings.defaultHighGraphics = new GraphicsSettingsData()
        {
            width = settings.appliedGraphics.width,
            height = settings.appliedGraphics.height,
            fullscreenMode = settings.appliedGraphics.fullscreenMode,
            targetFramerate = settings.appliedGraphics.targetFramerate,
            vSync = true,
            brightness = settings.appliedGraphics.brightness,
            textureResoluion = 0,
            msaa = 8,
            anisotropic = AnisotropicFiltering.ForceEnable,
            qualityLevelName = "High"
        };
        SettingsManager.Instance.Save();
    }
    public static void TrySetQualityByName(string name)
    {
        var names = QualitySettings.names;
        int idx = System.Array.IndexOf(names, name);
        if (idx >= 0) 
        QualitySettings.SetQualityLevel(idx, true);
        Debug.Log($"anisotropicFiltering: {QualitySettings.anisotropicFiltering}");
    }
}