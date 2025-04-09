using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip sceneBGM;
    public float fadeDuration = 1f;
    // Start is called before the first frame update
    void Start()
    {
        audioSource.DOFade(1f, fadeDuration);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FadeOutMusic()
    {
        // ²H¥X
        if (audioSource.isPlaying)
            audioSource.DOFade(0f, fadeDuration);
    }
}
