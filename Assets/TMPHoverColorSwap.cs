using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Localization.Components;

public class TMPHoverColorSwap : MonoBehaviour,IPointerEnterHandler, IPointerExitHandler,ISelectHandler, IDeselectHandler,IPointerDownHandler, IPointerUpHandler
{
    [Header("=== Setting Description ===")]
    public TMP_Text settingDescriptionText;
    public LocalizeStringEvent settingDescriptionLocalize;
    public int settingDescriptionNum;

    [Header("Target")]
    public TMP_Text target; // 留空會自動找子物件的 TMP 文字
    public EventSystem eventSystem;
    public Selectable targetSelectable;

    [Header("Colors")]
    public Color normal = Color.white;
    public Color hover = new Color(0.95f, 0.95f, 1f, 1f);
    public Color pressed = new Color(0.85f, 0.85f, 1f, 1f);
    public Color disabled = new Color(1f, 1f, 1f, 0.5f);

    [Header("Options")]
    [Tooltip("鍵盤/搖桿選取（Navigation停留）時是否也套用 hover 顏色")] public bool reactToSelection = true;

    Selectable selectable;
    bool pointerOver, selected, pointerDown;

    void Reset()
    {
        target = GetComponentInChildren<TMP_Text>(true);
        selectable = GetComponent<Selectable>();
    }

    void Awake()
    {
        if (target == null) target = GetComponentInChildren<TMP_Text>(true);
        if (selectable == null) selectable = GetComponent<Selectable>();
    }

    void OnEnable() => UpdateColor();

    void OnDisable()
    {
        pointerOver = selected = pointerDown = false;
        SetColor(normal);
    }

    public void OnPointerEnter(PointerEventData e) { pointerOver = true; UpdateColor(); UpdateDescriptionUI(settingDescriptionNum); }
    public void OnPointerExit(PointerEventData e) { pointerOver = false; pointerDown = false; UpdateColor(); }

    public void OnSelect(BaseEventData e) { if (!reactToSelection) return; selected = true; UpdateColor(); UpdateDescriptionUI(settingDescriptionNum); }
    public void OnDeselect(BaseEventData e) { if (!reactToSelection) return; selected = false; UpdateColor(); }

    public void OnPointerDown(PointerEventData e) { pointerDown = true; UpdateColor(); UpdateDescriptionUI(settingDescriptionNum); }
    public void OnPointerUp(PointerEventData e) { pointerDown = false; UpdateColor(); }

    void UpdateColor()
    {
        if (target == null) return;
        if (selectable != null && !selectable.interactable) { SetColor(disabled); return; }
        if (pointerDown) { SetColor(pressed); return; }
        bool active = pointerOver || (reactToSelection && selected);
        SetColor(active ? hover : normal);
    }

    void SetColor(Color c)
    {
        if (target != null && target.color != c) target.color = c;
    }
    public void UpdateDescriptionUI(int num)
    {
        if(gameObject.tag != "SettingUI") return;
        //if (settingDescriptionText.gameObject.activeInHierarchy == false) return;
        switch (num)
        {
            case 0:
                //settingDescriptionText.text = "Change the game language.";
                settingDescriptionLocalize.SetEntry("Language Description");
                break;
            case 1:
                //settingDescriptionText.text = "Adjust master, music, and SFX volume levels.";
                settingDescriptionLocalize.SetEntry("Audio Description");
                break;
            case 2:
                //settingDescriptionText.text = "Adjust game display settings.";
                settingDescriptionLocalize.SetEntry("Video Description");
                break;
            case 3:
                //settingDescriptionText.text = "Configure gamepad input settings.";
                settingDescriptionLocalize.SetEntry("Controller Description");
                break;
            case 4:
                //settingDescriptionText.text = "Configure keyboard input settings.";
                settingDescriptionLocalize.SetEntry("Keyboard Description");
                break;
            case 5:
                //settingDescriptionText.text = "Return to Main Menu";
                settingDescriptionLocalize.SetEntry("Return Description");
                break;
        }
    }
}
