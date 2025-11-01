using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.Localization.SmartFormat.Core.Parsing;

public class PointerSoundEffect : MonoBehaviour, IPointerEnterHandler, /*IPointerExitHandler,*/ ISelectHandler, /*IDeselectHandler,*/ IPointerDownHandler,ISubmitHandler
{
    [SerializeField]
    AudioSource audioSource;
    [SerializeField]
    private AudioClip pointerEnterSound,pointerSelectSound,pointerCancelSound;
    void Awake()
    {
        if (audioSource == null)
        {
            //audioSource = gameObject.AddComponent<AudioSource>();
            audioSource = GetComponent<AudioSource>();
        }
        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
        }
    }
    public void OnPointerEnter(PointerEventData e) { audioSource.PlayOneShot(pointerEnterSound); }
    //public void OnPointerExit(PointerEventData e) { audioSource.PlayOneShot(pointerEnterSound); }

    public void OnSelect(BaseEventData e) { audioSource.PlayOneShot(pointerEnterSound); }
    //public void OnDeselect(BaseEventData e) { audioSource.PlayOneShot(pointerCancelSound); }

    public void OnSubmit(BaseEventData e) { audioSource.PlayOneShot(pointerSelectSound); }

    public void OnPointerDown(PointerEventData e) { audioSource.PlayOneShot(pointerSelectSound); }

}
