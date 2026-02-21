using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

// Attach this to a root UI object and drag references for each control group.
public class SettingsUIBinder : MonoBehaviour
{
    [HideInInspector]
    public bool settingOnChange { get; private set; } = false;
    public GameObject startMenuPanel;
    public GameObject SettingPanel;
    [Header("=== Audio ===")]
    public GameObject audioSubPanel; 
    public Slider masterSlider; // 0..1
    public Slider musicSlider;  // 0..1
    public Slider sfxSlider;    // 0..1

    [Header("=== Graphics (use Apply) ===")]
    public GameObject graphicsSubPanel; 
    public TMP_Dropdown displayModeDropdown; // options: FullScreenWindow, ExclusiveFullScreen, MaximizedWindow, Windowed
    public TMP_Dropdown resolutionDropDown;
    public Toggle vSyncToggle;
    public TMP_Dropdown framerateDropdown; // options e.g. 30, 60, 90, 120, 144, 165, 240
    public Slider brightnessSlider;     // 0..1
    public TMP_Dropdown textureResolutionDropDown;//Full, Half, Quarter
    public TMP_Dropdown msaaDropdown;       // 0,2,4,8
    public TMP_Dropdown anisotropicDropdown;// Disable, Enable, ForceEnable
    public TMP_Dropdown qualityDropdown;    // filled from QualitySettings.names
    public Button applyGraphicsButton;
    public Button discardGraphicsButton;

    [Header("=== Locale ===")]
    public GameObject  localeSubPanel; 
    public TMP_Dropdown languageDropdown; // options: Japanese, English, ÁcÅé¤¤¤å

    [Header("=== Input Tuning ===")]
    public GameObject gamePadSubPanel;
    public GameObject keyboradSubPanel; 
    public Toggle invertYToggle;
    public Slider sensitivitySlider; // 0.5 .. 3 (example)
    public Slider leftStickDeadzoneSlider; // 0..0.5

    [Header("=== Input Rebind (global) ===")]
    public InputActionAsset inputActions;
    public Button resetGamePadBindingsButton, resetKeyBoardBindingsButton;
    public static System.Action OnBindingReset;



    void Start()
    {
        var sm = SettingsManager.Instance;
        var data = sm.Data;
        GraphicsSettingsApplier.SetDeafaultGraphics(data);
        // Hook Audio
        if (masterSlider) { masterSlider.value = data.masterVolume; masterSlider.onValueChanged.AddListener(v => { data.masterVolume = v; AudioSettingsApplier.ApplyVolumes(data); sm.Save(); }); }
        if (musicSlider) { musicSlider.value = data.musicVolume; musicSlider.onValueChanged.AddListener(v => { data.musicVolume = v; AudioSettingsApplier.ApplyVolumes(data); sm.Save(); }); }
        if (sfxSlider) { sfxSlider.value = data.sfxVolume; sfxSlider.onValueChanged.AddListener(v => { data.sfxVolume = v; AudioSettingsApplier.ApplyVolumes(data); sm.Save(); }); }

        // Populate Graphics dropdowns
        InitGraphicsUI();

        // Hook Locale
        if (languageDropdown)
        {
            int idx = codeToLangIndex(data.localeCode);
            languageDropdown.value = idx;
            languageDropdown.onValueChanged.AddListener(i => {
                string code = indexToLocaleCode(i);
                data.localeCode = code;
                LocaleApplier.ApplyLocale(code);
                sm.Save();
            });
        }

        // Input tuning
        if (invertYToggle) { invertYToggle.isOn = data.invertLookY; invertYToggle.onValueChanged.AddListener(v => { data.invertLookY = v; InputTuningApplier.ApplyInputTuning(data); sm.Save(); }); }
        if (sensitivitySlider) { sensitivitySlider.minValue = 0.5f; sensitivitySlider.maxValue = 3f; sensitivitySlider.value = data.lookSensitivity; sensitivitySlider.onValueChanged.AddListener(v => { data.lookSensitivity = v; InputTuningApplier.ApplyInputTuning(data); sm.Save(); }); }
        if (leftStickDeadzoneSlider) { leftStickDeadzoneSlider.minValue = 0f; leftStickDeadzoneSlider.maxValue = 0.5f; leftStickDeadzoneSlider.value = data.leftStickDeadzone; leftStickDeadzoneSlider.onValueChanged.AddListener(v => { data.leftStickDeadzone = v; InputTuningApplier.ApplyInputTuning(data); sm.Save(); }); }

        // Reset bindings
        if (resetGamePadBindingsButton)
        {
            resetGamePadBindingsButton.onClick.AddListener(() => {
                if (inputActions == null) inputActions = sm.inputActions;
                if (inputActions != null)
                {
                    string targetScheme = "GamePad";
                    var mask = InputBinding.MaskByGroup(targetScheme);

                    foreach (var action in inputActions)
                    {
                        action.RemoveBindingOverride(mask);
                    }
                    sm.Data.inputBindingOverridesJson = inputActions.SaveBindingOverridesAsJson();
                    sm.Save();
                    OnBindingReset?.Invoke();
                }
            });
        }
        if (resetKeyBoardBindingsButton)
        {
            resetKeyBoardBindingsButton.onClick.AddListener(() => {
                if (inputActions == null) inputActions = sm.inputActions;
                if (inputActions != null)
                {
                    string targetScheme = "Keyboard";
                    var mask = InputBinding.MaskByGroup(targetScheme);
                    foreach (var action in inputActions)
                    {
                        action.RemoveBindingOverride(mask);
                    }
                    sm.Data.inputBindingOverridesJson = inputActions.SaveBindingOverridesAsJson();
                    sm.Save();
                    OnBindingReset?.Invoke();
                }
            });
        }
    }


    void InitGraphicsUI()
    {
        var sm = SettingsManager.Instance; 
        var data = sm.Data; 
        var g = data.pendingGraphics;
        // Initialize pending with last applied at first launch
        if (g == null || (g.qualityLevelName == null)) data.pendingGraphics = JsonUtility.FromJson<GraphicsSettingsData>(JsonUtility.ToJson(data.appliedGraphics));
        g = data.pendingGraphics;
        if (displayModeDropdown)
        {
            displayModeDropdown.value = (int)g.fullscreenMode;
            displayModeDropdown.onValueChanged.AddListener(i => { g.fullscreenMode = (FullScreenMode)i; settingOnChange = true; });
        }
        if(resolutionDropDown)
        {
            resolutionDropDown.ClearOptions();
            var options = new List<string>();
            int currentIdx = 0;
            var resolutions = Screen.resolutions;
            for (int i = 0; i < resolutions.Length; i++)
            {
                var r = resolutions[i];
                string s = $"{r.width} x {r.height}";
                options.Add(s);
                if (r.width == g.width && r.height == g.height)
                {
                    currentIdx = i;
                    
                }
            }
            resolutionDropDown.AddOptions(options);
            resolutionDropDown.value = currentIdx;
            resolutionDropDown.onValueChanged.AddListener(i => {
                var r = resolutions[Mathf.Clamp(i, 0, resolutions.Length - 1)];
                g.width = r.width;
                g.height = r.height;
                settingOnChange = true;
            });
        }
        if (vSyncToggle) 
        { 
            vSyncToggle.isOn = g.vSync; 
            vSyncToggle.onValueChanged.AddListener(v => { g.vSync = v; settingOnChange = true; qualityDropdown.value = 3; }); 
        }

        if (framerateDropdown)
        {
            int[] rates = new[] { 30, 60, 90, 120, 144};
            int idx = System.Array.IndexOf(rates, g.targetFramerate);
            framerateDropdown.value = Mathf.Max(0, idx);
            framerateDropdown.onValueChanged.AddListener(i => { g.targetFramerate = rates[Mathf.Clamp(i, 0, rates.Length - 1)]; settingOnChange = true; });
        }

        if (brightnessSlider) 
        {
            brightnessSlider.value = g.brightness; 
            brightnessSlider.onValueChanged.AddListener(v => { g.brightness = v; settingOnChange = true; }); 
        }

        if (textureResolutionDropDown)
        {
            textureResolutionDropDown.value = 0; // Full by default
            textureResolutionDropDown.onValueChanged.AddListener(i => { g.textureResoluion = i; settingOnChange = true; qualityDropdown.value = 3; });
        }

        if (msaaDropdown)
        {
            int[] msaaOptions = new[] { 0, 2, 4, 8 };
            int idx = System.Array.IndexOf(msaaOptions, g.msaa);
            msaaDropdown.value = Mathf.Max(0, idx);
            msaaDropdown.onValueChanged.AddListener(i => { g.msaa = msaaOptions[Mathf.Clamp(i, 0, msaaOptions.Length - 1)]; settingOnChange = true; qualityDropdown.value = 3; });
        }

        if (anisotropicDropdown)
        {
            anisotropicDropdown.value = (int)g.anisotropic;
            anisotropicDropdown.onValueChanged.AddListener(i => { g.anisotropic = (AnisotropicFiltering)i; settingOnChange = true; qualityDropdown.value = 3; });
        }

        if (qualityDropdown)
        {
            var names = QualitySettings.names;
            int idx = System.Array.IndexOf(names, g.qualityLevelName);
            if (idx < 0) idx = QualitySettings.GetQualityLevel();
            qualityDropdown.value = Mathf.Clamp(idx, 0, names.Length - 1);
            qualityDropdown.onValueChanged.AddListener(i => { var names2 = QualitySettings.names; g.qualityLevelName = names2[Mathf.Clamp(i, 0, names2.Length - 1)]; settingOnChange = true; TempSetQL(data); });
        }

        if (applyGraphicsButton)
        {
            applyGraphicsButton.onClick.AddListener(() =>
            {
                switch(qualityDropdown.value)
                {
                    case 0:
                        data.appliedGraphics = JsonUtility.FromJson<GraphicsSettingsData>(JsonUtility.ToJson(data.defaultLowGraphics));
                        break;
                    case 1:
                        data.appliedGraphics = JsonUtility.FromJson<GraphicsSettingsData>(JsonUtility.ToJson(data.defaultMediumGraphics));
                        break;
                    case 2:
                        data.appliedGraphics = JsonUtility.FromJson<GraphicsSettingsData>(JsonUtility.ToJson(data.defaultHighGraphics));
                        break;
                    case 3:
                        // custom, use as is
                        data.appliedGraphics = JsonUtility.FromJson<GraphicsSettingsData>(JsonUtility.ToJson(data.pendingGraphics));
                        break;
                    default:
                        // custom, do nothing
                        data.appliedGraphics = JsonUtility.FromJson<GraphicsSettingsData>(JsonUtility.ToJson(data.pendingGraphics));
                        break;
                }
                SettingsManager.Instance.Save();
                settingOnChange = false;
            });
        }
        if (discardGraphicsButton)
        {
            discardGraphicsButton.onClick.AddListener(() =>
            {
                GraphicsSettingsApplier.ApplyGraphics(data.appliedGraphics);
                SettingsManager.Instance.Save();
                UpdateUI();
                settingOnChange = false;
            });
        }
    }
    void TempSetQL(SettingsData data)
    {
        switch (qualityDropdown.value)
        {
            case 0:
                vSyncToggle.SetIsOnWithoutNotify(data.defaultLowGraphics.vSync);
                if(data.defaultLowGraphics.msaa == 2) { msaaDropdown.SetValueWithoutNotify(1); }
                anisotropicDropdown.SetValueWithoutNotify((int)data.defaultLowGraphics.anisotropic);
                textureResolutionDropDown.SetValueWithoutNotify(data.defaultLowGraphics.textureResoluion);
                break;
            case 1:
                vSyncToggle.SetIsOnWithoutNotify(data.defaultMediumGraphics.vSync);
                if (data.defaultMediumGraphics.msaa == 4) { msaaDropdown.SetValueWithoutNotify(2); }
                anisotropicDropdown.SetValueWithoutNotify((int)data.defaultMediumGraphics.anisotropic);
                textureResolutionDropDown.SetValueWithoutNotify(data.defaultMediumGraphics.textureResoluion);
                break;
            case 2:
                vSyncToggle.SetIsOnWithoutNotify(data.defaultHighGraphics.vSync);
                if (data.defaultHighGraphics.msaa == 8) { msaaDropdown.SetValueWithoutNotify(3); }
                anisotropicDropdown.SetValueWithoutNotify((int)data.defaultHighGraphics.anisotropic);
                textureResolutionDropDown.SetValueWithoutNotify(data.defaultHighGraphics.textureResoluion);
                break;
            case 3:
                // custom, use as is
                vSyncToggle.isOn = data.appliedGraphics.vSync;
                switch(data.appliedGraphics.msaa)
                {
                    case 0: msaaDropdown.value = 0; break;
                    case 2: msaaDropdown.value = 1; break;
                    case 4: msaaDropdown.value = 2; break;
                    case 8: msaaDropdown.value = 3; break;
                    default: msaaDropdown.value = 0; break;
                }
                anisotropicDropdown.value = (int)data.appliedGraphics.anisotropic;
                textureResolutionDropDown.value = data.defaultLowGraphics.textureResoluion;
                break;
            default:
                break;
        }
    }
    public void QualityToCustom()
    {
        qualityDropdown.value = 3;
    }
    public void UpdateUI()
    {
        var sm = SettingsManager.Instance;
        var data = sm.Data;
        var g = data.appliedGraphics;

        displayModeDropdown.value = (int)g.fullscreenMode;

        int currentIdx = 0;
        var resolutions = Screen.resolutions;
        for (int i = 0; i < resolutions.Length; i++)
        {
            var r = resolutions[i];
            string s = $"{r.width} x {r.height}";
            if (r.width == g.width && r.height == g.height)
            {
                currentIdx = i;
            }
        }
        resolutionDropDown.value = currentIdx;

        vSyncToggle.isOn = g.vSync;
        framerateDropdown.value = g.targetFramerate;
        brightnessSlider.value = g.brightness;
        msaaDropdown.value = g.msaa;
        anisotropicDropdown.value = (int)g.anisotropic;
    }
    
    public void ActiveSettingPanel()
    {
        SettingPanel.SetActive(true);
    }
    public void DeactiveSettingPanel()
    {
        SettingPanel.SetActive(false);
    }
    public void ActiveStartMenuPanel()
    {
        startMenuPanel.SetActive(true);
    }
    public void OnClickLanguageTitle()
    {
        localeSubPanel.SetActive(true);
        audioSubPanel.SetActive(false);
        graphicsSubPanel.SetActive(false);
        gamePadSubPanel.SetActive(false);
        keyboradSubPanel.SetActive(false);
    }
    public void OnClickVideoTitle()
    {
        localeSubPanel.SetActive(false);
        audioSubPanel.SetActive(false);
        graphicsSubPanel.SetActive(true);
        gamePadSubPanel.SetActive(false);
        keyboradSubPanel.SetActive(false);
    }
    public void OnClickAudioTitle()
    {
        localeSubPanel.SetActive(false);
        audioSubPanel.SetActive(true);
        graphicsSubPanel.SetActive(false);
        gamePadSubPanel.SetActive(false);
        keyboradSubPanel.SetActive(false);
    }
    public void OnClickGamePadTitle()
    {
        localeSubPanel.SetActive(false);
        audioSubPanel.SetActive(false);
        graphicsSubPanel.SetActive(false);
        gamePadSubPanel.SetActive(true);
        keyboradSubPanel.SetActive(false);
    }
    public void OnClickKeyboardTitle()
    {
        localeSubPanel.SetActive(false);
        audioSubPanel.SetActive(false);
        graphicsSubPanel.SetActive(false);
        gamePadSubPanel.SetActive(false);
        keyboradSubPanel.SetActive(true);
    }
    static int codeToLangIndex(string code)
    {
        switch (code)
        {
            case "en": return 0;
            case "zh-Hant": return 1;
            case "ja": return 2;
            default: return 0;
        }
    }
    static string indexToLocaleCode(int idx)
    {
        switch (idx)
        {
            case 0: return "en";
            case 1: return "zh-Hant";
            case 2: return "ja";
            default: return "en";
        }
    }
}
