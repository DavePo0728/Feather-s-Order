using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

// Attach to a bootstrap GameObject that exists across scenes.
public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    [Header("Input System (optional but recommended)")]
#if ENABLE_INPUT_SYSTEM
    [Tooltip("Reference to your game's InputActionAsset to load/apply binding overrides.")]
    public InputActionAsset inputActions;
#endif

    public SettingsData Data { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        Data = SettingsStore.LoadOrCreate();

#if ENABLE_INPUT_SYSTEM
        // Apply saved binding overrides to the whole asset
        if (inputActions != null && !string.IsNullOrEmpty(Data.inputBindingOverridesJson))
        {
            try { inputActions.LoadBindingOverridesFromJson(Data.inputBindingOverridesJson); }
            catch (System.Exception e) { Debug.LogWarning($"LoadBindingOverrides failed: {e}"); }
        }
#endif
        // Apply non-graphics immediately
        AudioSettingsApplier.ApplyVolumes(Data);
        InputTuningApplier.ApplyInputTuning(Data);
        LocaleApplier.ApplyLocale(Data.localeCode);

        // Apply last applied graphics (not pending)
        GraphicsSettingsApplier.ApplyGraphics(Data.appliedGraphics);
    }

    public void Save() => SettingsStore.Save(Data);
}
