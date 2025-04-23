using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class AlphaBreathingWithYOffset : MonoBehaviour
{
    public float breathSpeed = 1f;
    public float minAlpha = 0.2f;
    public float maxAlpha = 1f;
    public float offsetSpeed = 0.5f;

    [Header("擦彈時粒子特效")]
    public ParticleSystem targetParticleSystem;
    public float flashLifetime = 2f;        // 擦彈時提升的 Start Lifetime
    public float flashDuration = 0.15f;     // 閃爍持續時間

    private Material mat;
    private Color originalColor;
    private float offsetValue = 0f;
    private float originalLifetime;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
        originalColor = mat.color;

        if (targetParticleSystem != null)
            originalLifetime = targetParticleSystem.main.startLifetime.constant;
    }

    void Update()
    {
        // Alpha 呼吸效果
        float t = (Mathf.Sin(Time.time * breathSpeed * Mathf.PI * 2f) + 1f) / 2f;
        float alpha = Mathf.Lerp(minAlpha, maxAlpha, t);
        Color newColor = originalColor;
        newColor.a = alpha;
        mat.color = newColor;

        // Y 軸 Offset 流動
        offsetValue += Time.deltaTime * offsetSpeed;
        if (offsetValue > 1f) offsetValue -= 1f;
        mat.SetTextureOffset("_MainTex", new Vector2(0f, offsetValue));
    }

    public void FlashLifetime()
    {
        if (targetParticleSystem == null) return;
        StopAllCoroutines();
        StartCoroutine(FlashLifetimeCoroutine());
    }

    IEnumerator FlashLifetimeCoroutine()
    {
        var main = targetParticleSystem.main;

        // 提高粒子 Lifetime
        main.startLifetime = flashLifetime;

        yield return new WaitForSeconds(flashDuration);

        // 回復原本的 Lifetime
        main.startLifetime = originalLifetime;
    }
}
