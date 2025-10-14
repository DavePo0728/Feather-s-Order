using UnityEngine;
using UnityEngine.Localization.Settings;

public static class LocaleApplier
{
    public static void ApplyLocale(string code)
    {
        try
        {
            var locs = LocalizationSettings.AvailableLocales.Locales;
            foreach (var loc in locs)
            {
                if (loc.Identifier.Code.Equals(code, System.StringComparison.OrdinalIgnoreCase))
                {
                    LocalizationSettings.SelectedLocale = loc;
                    break;
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Localization switch failed: {e}");
        }
    }
}
