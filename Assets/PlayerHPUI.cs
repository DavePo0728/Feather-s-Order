using UnityEngine;

public class PlayerHPUI : MonoBehaviour
{
    [Header("HP設定")]
    public float maxHP = 100f;
    public float currentHP;
    public float delaySpeed = 0.5f; // 虛血條下降速度

    [Header("血條物件")]
    public RectTransform currentHPRect;  // 主血條
    public RectTransform delayHPRect;    // 虛血條
    public RectTransform hpBarRoot;       // 包含背景的血條整體

    [Header("震動設定")]
    public float shakeDuration = 0.2f;
    public float shakeStrength = 5f;
    private Vector3 originalPos;
    private float shakeTimer = 0f;

    private float originalWidth; // 原本血條的寬度

    void Start()
    {
        currentHP = maxHP;
        originalPos = hpBarRoot.localPosition;
        originalWidth = currentHPRect.sizeDelta.x; // 記錄血條原本長度
    }

    void Update()
    {
        UpdateHPUI();
        HandleShake();
    }

    void UpdateHPUI()
    {
        float currentFill = Mathf.Clamp01(currentHP / maxHP);

        // 直接改變主血條寬度
        Vector2 size = currentHPRect.sizeDelta;
        size.x = originalWidth * currentFill;
        currentHPRect.sizeDelta = size;

        // 虛血條慢慢追上
        if (delayHPRect.sizeDelta.x > size.x)
        {
            Vector2 delaySize = delayHPRect.sizeDelta;
            delaySize.x -= delaySpeed * Time.deltaTime * originalWidth;
            if (delaySize.x < size.x)
                delaySize.x = size.x;
            delayHPRect.sizeDelta = delaySize;
        }
    }

    void HandleShake()
    {
        if (shakeTimer > 0f)
        {
            Vector3 randomOffset = Random.insideUnitCircle * shakeStrength;
            hpBarRoot.localPosition = originalPos + randomOffset;
            shakeTimer -= Time.deltaTime;
        }
        else
        {
            hpBarRoot.localPosition = originalPos;
        }
    }

    // 外部呼叫扣血
    public void TakeDamage(float damage)
    {
        currentHP = Mathf.Max(0, currentHP - damage);

        // 啟動震動
        shakeTimer = shakeDuration;
    }
}
