using System.Collections;
using UnityEngine;

public class UIShaker : MonoBehaviour
{
    [Header("Shake Settings")]
    public float shakeStrength = 10f;
    public float shakeDuration = 0.2f;

    [Header("Flash Settings (GameObject Swap)")]
    public GameObject normalObject; // 原本的 UI
    public GameObject glowObject;   // 發光版 UI
    public float glowDuration = 0.2f;

    RectTransform rect;
    Vector3 originalPos;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        originalPos = rect.anchoredPosition;

        // 確保初始狀態正確
        if (normalObject != null) normalObject.SetActive(true);
        if (glowObject != null) glowObject.SetActive(false);
    }

    public void Trigger()
    {
        Shake();
        SwapObject();
    }

    public void Shake()
    {
        StopCoroutine(nameof(ShakeCoroutine));
        StartCoroutine(ShakeCoroutine());
    }

    public void SwapObject()
    {
        if (normalObject == null || glowObject == null) return;
        StopCoroutine(nameof(SwapObjectCoroutine));
        StartCoroutine(SwapObjectCoroutine());
    }

    IEnumerator ShakeCoroutine()
    {
        float timer = 0f;
        while (timer < shakeDuration)
        {
            float offsetX = Random.Range(-1f, 1f) * shakeStrength;
            float offsetY = Random.Range(-1f, 1f) * shakeStrength;
            rect.anchoredPosition = originalPos + new Vector3(offsetX, offsetY, 0f);
            timer += Time.deltaTime;
            yield return null;
        }
        rect.anchoredPosition = originalPos;
    }

    IEnumerator SwapObjectCoroutine()
    {
        normalObject.SetActive(false);
        glowObject.SetActive(true);

        yield return new WaitForSeconds(glowDuration);

        glowObject.SetActive(false);
        normalObject.SetActive(true);
    }
}