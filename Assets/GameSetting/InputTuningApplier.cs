#if ENABLE_INPUT_SYSTEM
using UnityEngine;
using UnityEngine.InputSystem;


public static class InputTuningApplier
{
    // Apply Y invert, sensitivity, and deadzone via processors override on specific actions.
    // Assumes your Action names: "Look" (Vector2 stick/mouse), "Move" (Vector2), etc.


    public static string lookActionName = "Look"; // change if needed


    public static void ApplyInputTuning(SettingsData data)
    {
        var asset = SettingsManager.Instance?.inputActions;
        if (asset == null) return;


        var look = asset.FindAction(lookActionName, throwIfNotFound: false);
        if (look != null)
        {
            // We'll override processors on each binding of Look that targets a gamepad stick.
            for (int i = 0; i < look.bindings.Count; i++)
            {
                var b = look.bindings[i];
                if (!b.isComposite && b.groups != null && b.groups.Contains("Gamepad"))
                {
                    // Build processors string
                    // stickDeadzone(min=deadzone)
                    // scaleVector2(x=sensitivity, y= +/- sensitivity)
                    string processors = $"stickDeadzone(min={data.leftStickDeadzone}),scaleVector2(x={data.lookSensitivity},y={(data.invertLookY ? -data.lookSensitivity : data.lookSensitivity)})";
                    look.ApplyBindingOverride(i, new InputBinding { overrideProcessors = processors });
                }
            }
        }


        // Activate changes immediately
        asset.Enable();


        // Save overrides back into SettingsData
        SettingsManager.Instance.Data.inputBindingOverridesJson = asset.SaveBindingOverridesAsJson();
        SettingsManager.Instance.Save();
    }
}
#endif
