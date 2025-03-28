using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class NearLockTweenAnim : MonoBehaviour
{
    Image nearLockImage;
    Tween nearLockScaleTween, nearLockAlphaTween;

    // Start is called before the first frame update
    void Start()
    {
        nearLockImage = GetComponent<Image>();
        nearLockScaleTween = transform.DOScale(1, 0.2f).SetAutoKill(false);
        nearLockAlphaTween = nearLockImage.DOColor(Color.white, 0.2f).SetAutoKill(false);
        nearLockScaleTween.Play();
        nearLockAlphaTween.Play();
    }
    private void OnEnable()
    {
            nearLockScaleTween.Play();
            nearLockAlphaTween.Play();
    }
    public void ReActive()
    {
        nearLockScaleTween.Restart();
        nearLockAlphaTween.Restart();
    }
    private void OnDisable()
    {
        nearLockScaleTween.Rewind();
        nearLockAlphaTween.Rewind();
    }
}
