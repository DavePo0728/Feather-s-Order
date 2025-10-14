using UnityEngine;
using UnityEngine.Audio;


public static class AudioSettingsApplier
{
    // Connect your AudioMixer with exposed parameters.
    public static AudioMixer Mixer;


    // Set these to match your exposed param names
    public static string MasterParam = "MasterVolume";
    public static string MusicParam = "MusicVolume";
    public static string SfxParam = "SFXVolume";


    public static void ApplyVolumes(SettingsData data)
    {
        if (Mixer == null) { Debug.LogWarning("Audio Mixer not assigned on a scene AudioBootstrap."); return; }
        SetDb(MasterParam, data.masterVolume);
        SetDb(MusicParam, data.musicVolume);
        SetDb(SfxParam, data.sfxVolume);
    }


    static void SetDb(string param, float linear01)
    {
        // Map 0..1 slider to decibels. 0 => -80 dB (mute); 1 => 0 dB
        float dB = (linear01 <= 0.0001f) ? -80f : Mathf.Log10(Mathf.Clamp(linear01, 0.0001f, 1f)) * 20f;
        Mixer.SetFloat(param, dB);
    }
}