using UnityEngine;
using UnityEngine.Audio;


// Put this in the first scene to provide AudioMixer & optional brightness overlay/volume.
public class AudioAndGraphicsBootstrap : MonoBehaviour
{
    public AudioMixer mixer; // assign your project's AudioMixer
    //public CanvasGroup brightnessOverlay; // optional overlay for brightness if no URP
#if UNITY_RENDER_PIPELINE_UNIVERSAL
public UnityEngine.Rendering.Volume globalVolume; // optional URP Volume for brightness
#endif
    void Awake()
    {
        AudioSettingsApplier.Mixer = mixer;
        //GraphicsSettingsApplier.brightnessOverlay = brightnessOverlay;
#if UNITY_RENDER_PIPELINE_UNIVERSAL
GraphicsSettingsApplier.globalVolume = globalVolume;
#endif
        // Re-apply in case SettingsManager loaded earlier without these refs
        if (SettingsManager.Instance != null)
        {
            AudioSettingsApplier.ApplyVolumes(SettingsManager.Instance.Data);
            //GraphicsSettingsApplier.ApplyGraphics(SettingsManager.Instance.Data.appliedGraphics);
        }
    }
}