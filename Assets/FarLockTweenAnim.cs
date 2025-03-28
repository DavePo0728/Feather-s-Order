using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class FarLockTweenAnim : MonoBehaviour
{
    Image farLockImage;
    Tween farLockScaleTween, farLockRotateTween,farLockAlphaTween;
    // Start is called before the first frame update
    void Start()
    {
        farLockImage = gameObject.GetComponent<Image>();
        farLockScaleTween = transform.DOScale(1, 0.2f).SetAutoKill(false);
        farLockRotateTween = transform.DORotate(new Vector3(0,0,-360f),0.2f, RotateMode.FastBeyond360).SetAutoKill(false);
        farLockAlphaTween = farLockImage.DOColor(Color.white, 0.2f).SetAutoKill(false);
        farLockScaleTween.Play();
        farLockRotateTween.Play();
        farLockAlphaTween.Play();
    }
    private void OnEnable()
    {
            farLockScaleTween.Play();
            farLockAlphaTween.Play();
            farLockRotateTween.Play();
    }
    public void ReActive()
    {
        farLockScaleTween.Restart();
        farLockAlphaTween.Restart();
        farLockRotateTween.Restart();
    }
    private void OnDisable()
    {
        farLockScaleTween.Rewind();
        farLockAlphaTween.Rewind();
        farLockRotateTween.Rewind();
    }
    public void Refresh()
    {
        
    }
}
