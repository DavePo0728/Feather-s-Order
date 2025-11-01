using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SetsUiElementToSelectOnInteraction : MonoBehaviour, ICancelHandler
{
    [Header("Setup")]
    [SerializeField] private SettingsUIBinder settingsUIBinder;
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private Selectable elementToSelect;
    [SerializeField] private Selectable CancelElement;
    [SerializeField] private GameObject settingPanel;
    [SerializeField] private GameObject activePanel;

    [Header("Visualization")]
    [SerializeField] private bool showVisualization;
    [SerializeField] private Color navigationColour = Color.cyan;

    private void OnDrawGizmos()
    {
        if (!showVisualization)
            return;

        if (elementToSelect == null)
            return;

        Gizmos.color = navigationColour;
        Gizmos.DrawLine(gameObject.transform.position, elementToSelect.gameObject.transform.position);
    }

    public void OnCancel(BaseEventData e) {
        if(gameObject.tag == "SubPanelUI")
        {
            
            if(settingsUIBinder.settingOnChange == true)
            {
                ActiveSubPanel();
                JumpToElement();
            }
            else
            {
                settingPanel.SetActive(false);
                eventSystem.SetSelectedGameObject(CancelElement.gameObject);
            }
        }
        if(gameObject.tag == "ConfirmUI")
        {
            settingPanel.SetActive(false);
            eventSystem.SetSelectedGameObject(CancelElement.gameObject);
        }
        if(gameObject.tag == "SettingUI")
        {
            settingPanel.SetActive(false);
            eventSystem.SetSelectedGameObject(CancelElement.gameObject);
        }
    }
    public void JumpToElement()
    {
        if (eventSystem == null)
            Debug.Log("This item has no event system referenced yet.", this);

        if (elementToSelect == null)
            Debug.Log("This should jump where?", this);

        eventSystem.SetSelectedGameObject(elementToSelect.gameObject);
    }
    public void ActiveSubPanel()
    {
        activePanel.SetActive(true);
    }
    public void ConfirmFinish()
    {
        if (gameObject.tag == "ConfirmUI")
        {
            settingPanel.SetActive(false);
        }
        eventSystem.SetSelectedGameObject(elementToSelect.gameObject);
    }
}
