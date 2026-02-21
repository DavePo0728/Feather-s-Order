#if ENABLE_INPUT_SYSTEM
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using TMPro;
using static System.Collections.Specialized.BitVector32;

// Put this on each "binding row" (label + button). Supports Keyboard/Mouse and Gamepad.
public class RebindUI : MonoBehaviour
{
    [Header("Action & Binding")]
    public InputActionReference actionRef; // e.g., Player/Jump
    [Tooltip("Optional: binding id (GUID). If empty, we pick the first binding from the chosen control scheme.")]
    public string bindingId;

    [Header("Scheme Filter")] // pick one row for Keyboard&Mouse, one for Gamepad
    public string controlSchemeGroup; // must match your binding 'groups' (e.g., "Keyboard&Mouse" or "Gamepad")

    [Header("UI")]
    public TMP_Text bindingNameLabel; // shows current binding path nicely
    public Button rebindButton;
    //public Button clearButton; // optional

    [Header("Composite Support")] // NEW: allow rebinding a specific part of a composite
    public bool isCompositePart = false;
    [Tooltip("Part name inside the composite (e.g., Up/Down/Left/Right for 2DVector; Positive/Negative for 1DAxis)")]
    public string compositePartName = "";

    InputAction _action;
    public int _bindingIndex;
    public GameObject waitingForInputIcon;
    void OnEnable()
    {
        _action = actionRef?.action;
        ResolveBinding();
        RefreshLabel();
        if (rebindButton) rebindButton.onClick.AddListener(StartInteractiveRebind);
        SettingsUIBinder.OnBindingReset += RefreshLabel;
        //if (clearButton) clearButton.onClick.AddListener(ClearBinding);
    }
    void OnDisable()
    {
        if (rebindButton) rebindButton.onClick.RemoveListener(StartInteractiveRebind);
        SettingsUIBinder.OnBindingReset -= RefreshLabel;
        // if (clearButton) clearButton.onClick.RemoveListener(ClearBinding);
    }

    void ResolveBinding()
    {
        //_bindingIndex = 0;
        if (_action == null) {
            Debug.Log("RebindUI: No action assigned.");
            return;
        }

        if (string.IsNullOrEmpty(bindingId))
        {
            bindingId = actionRef.action.bindings[_bindingIndex].id.ToString();
        }


        if (!string.IsNullOrEmpty(bindingId))
        {
            var guid = new System.Guid(bindingId);
            //Debug.Log($"Looking for binding with id {guid} in action {_action.name}");
            for (int i = 0; i < _action.bindings.Count; i++)
            {
                if (_action.bindings[i].id == guid) { _bindingIndex = i; break; }
            }
            return;
        }


        // Search by control-scheme group
        if (isCompositePart)
        {
            // find the composite header for this group, then the child part with given name
            for (int i = 0; i < _action.bindings.Count; i++)
            {
                var b = _action.bindings[i];
                if (b.isComposite && (string.IsNullOrEmpty(controlSchemeGroup) || (b.groups?.Contains(controlSchemeGroup) ?? false)))
                {
                    // iterate its children
                    int j = i + 1;
                    for (; j < _action.bindings.Count && _action.bindings[j].isPartOfComposite; j++)
                    {
                        var part = _action.bindings[j];
                        if (string.Equals(part.name, compositePartName, System.StringComparison.OrdinalIgnoreCase))
                        {
                            _bindingIndex = j; return;
                        }
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < _action.bindings.Count; i++)
            {
                var b = _action.bindings[i];
                if (!b.isComposite && !b.isPartOfComposite && (string.IsNullOrEmpty(controlSchemeGroup) || (b.groups?.Contains(controlSchemeGroup) ?? false)))
                { _bindingIndex = i; break; }
            }
        }
    }

    public void RefreshLabel()
    {
        if (bindingNameLabel == null || _action == null || _bindingIndex < 0) return;
        string human = InputControlPath.ToHumanReadableString(
            _action.bindings[_bindingIndex].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice);
        if (string.IsNullOrEmpty(human)) human = "Unbound";
        bindingNameLabel.text = human;
    }

    public void StartInteractiveRebind()
    {
        if (_action == null || _bindingIndex < 0) return;
        _action.Disable();
        waitingForInputIcon.SetActive(true);
        var rebind = _action.PerformInteractiveRebinding(_bindingIndex)
            .WithCancelingThrough("<Keyboard>/escape")
            .WithTimeout(10f)
            .OnMatchWaitForAnother(0.1f)
            .OnCancel(op => { op.Dispose(); _action.Enable(); waitingForInputIcon.SetActive(false); })
            .OnComplete(op =>
            {
                op.Dispose();
                _action.Enable();
                SaveAllOverrides();
                RefreshLabel();
                waitingForInputIcon.SetActive(false);
                Debug.Log($"Rebind complete the rebind key is{_action.bindings[0].effectivePath}");
            });

        // Optional: exclude mouse if rebinding keyboard-only, etc.
        if (controlSchemeGroup == "Gamepad")
        {
            rebind.WithControlsHavingToMatchPath("<Gamepad>");
        }
        else if (controlSchemeGroup == "Keyboard")
        {
            rebind.WithControlsHavingToMatchPath("<Keyboard>").WithControlsExcluding("<Mouse>/delta");
        }

        rebind.Start();
    }

    static void SaveAllOverrides()
    {
        var sm = SettingsManager.Instance;
        if (sm == null || sm.inputActions == null) return;
        sm.Data.inputBindingOverridesJson = sm.inputActions.SaveBindingOverridesAsJson();
        sm.Save();
    }
}
#endif
