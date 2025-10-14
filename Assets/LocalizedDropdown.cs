using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization;

public class LocalizedDropdown : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropdown;
    [SerializeField] private List<LocalizedString> localizedOptions;

    void Start()
    {
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
        UpdateDropdownOptions();
    }

    void OnLocaleChanged(Locale locale)
    {
        UpdateDropdownOptions();
    }

    void UpdateDropdownOptions()
    {
        dropdown.options.Clear();

        foreach (var localizedString in localizedOptions)
        {
            string localizedText = localizedString.GetLocalizedString();
            dropdown.options.Add(new TMP_Dropdown.OptionData(localizedText));
        }

        dropdown.RefreshShownValue();
    }

    void OnDestroy()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
    }
}
