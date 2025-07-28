using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTimeManager : MonoBehaviour
{
    public static BulletTimeManager instance;
    private float _normalTimeScale;
    private float _normalFixedDelta;
    private void Awake()
    {
        instance = this;
        _normalTimeScale = Time.timeScale;
        _normalFixedDelta = Time.fixedDeltaTime;
    }
    private void Update()
    {
        //Debug.Log("Current Time Scale: " + Time.timeScale);
        //foreach (var src in FindObjectsOfType<AudioSource>())
        //    Debug.Log("Audio Source Pitch: " + src.pitch);
    }
    public void DoBulletTime(float slowScale, float duration)
    {
        //StartCoroutine(BulletTimeCR(slowScale, duration));
        BulletTimeTween(slowScale, duration);
    }
    private IEnumerator BulletTimeCR(float slowScale, float duration)
    {
        Time.timeScale = slowScale;
        Time.fixedDeltaTime = _normalFixedDelta * slowScale;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = _normalTimeScale;
        Time.fixedDeltaTime = _normalFixedDelta;
    }
    void BulletTimeTween(float slowScale, float duration)
    {
        DOTween.To(() => Time.timeScale, x =>
        {
            Time.timeScale = x;
            Time.fixedDeltaTime = _normalFixedDelta * x;
            //foreach (var src in FindObjectsOfType<AudioSource>())
            //    src.pitch = x;
        }, slowScale, 0.1f).Play()
                .SetUpdate(UpdateType.Normal, true)
                .OnComplete(() =>
                {
                        DOTween.To(() => Time.timeScale, x =>
                        {
                            Time.timeScale = x;
                            Time.fixedDeltaTime = _normalFixedDelta * x;
                            //foreach (var src in FindObjectsOfType<AudioSource>())
                            //src.pitch = x;
                        }, _normalTimeScale, 0.1f).Play()
                        .OnComplete(() => { Debug.Log("Bullet Time Completed. Restoring normal time scale."); })
                        .SetUpdate(UpdateType.Normal, true)
                        .SetDelay(duration);
                });
    }
}
