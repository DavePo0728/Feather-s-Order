using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using Cinemachine;

public class PlayerHP : MonoBehaviour
{
    ScenesManager scenesManager;
    public BulletGraze bulletGraze;
    Rigidbody playerRigidbody;
    [SerializeField] int maxHp;
    [SerializeField] int playerHp;

    [Header("UI")]
    [SerializeField] Image hpBarImage;         // 主血條
    [SerializeField] Image delayHpBarImage;     // 虛血條
    [SerializeField] TMP_Text hpText;
    [SerializeField] GameObject gameOverUI;
    [SerializeField] GameObject hpDamageImage;
    [SerializeField] RectTransform hpBarRoot;   // 整個血條容器
    [SerializeField] CinemachineImpulseSource impulseSource;

    [Header("背景物件切換")]
    [SerializeField] GameObject normalBackground;
    [SerializeField] GameObject hurtFlashBackground;
    [SerializeField] GameObject healFlashBackground;
    [SerializeField] float flashDuration = 0.1f;

    [Header("虛血條設定")]
    [SerializeField] float delaySpeed = 0.5f;

    [Header("血條震動設定")]
    [SerializeField] float hurtShakeStrength = 5f;
    [SerializeField] float healShakeStrength = 2f;
    [SerializeField] float hurtShakeDuration = 0.3f;
    [SerializeField] float healShakeDuration = 0.15f;

    [Header("Debug")]
    [SerializeField] bool debug;

    AudioSource hurtAudioSource;
    AudioClip hurtClip1, hurtClip2;
    bool isMuteki = false;

    float shakeTimer = 0f;
    float currentShakeStrength = 0f;
    Vector3 originalHpBarPos;
    float flashTimer = 0f;
    GameObject currentFlashingBackground;

    private void Awake()
    {
        Time.timeScale = 1;
        hurtAudioSource = GetComponent<AudioSource>();
        hurtClip1 = Resources.Load<AudioClip>("Sound/PlayerGetHit01");
        hurtClip2 = Resources.Load<AudioClip>("Sound/PlayerGetHit02");
        scenesManager = GameObject.Find("SceneManager").GetComponent<ScenesManager>();
    }

    void Start()
    {
        if (debug)
        {
            maxHp = 9999;
            playerHp = maxHp;
        }
        else
        {
            maxHp = 100;
            playerHp = maxHp;
        }
        UpdateHpUI();
        playerRigidbody = GetComponent<Rigidbody>();

        if (hpBarRoot != null)
            originalHpBarPos = hpBarRoot.localPosition;

        // 初始狀態
        if (normalBackground != null) normalBackground.SetActive(true);
        if (hurtFlashBackground != null) hurtFlashBackground.SetActive(false);
        if (healFlashBackground != null) healFlashBackground.SetActive(false);
    }

    void Update()
    {
        HandleHpBarShake();
        HandleHpBarFlash();
        HandleDelayHP();
    }

    public void getHit(int damage)
    {
        int temp = Random.Range(0, 2);
        switch (temp)
        {
            case 0:
                hurtAudioSource.PlayOneShot(hurtClip1);
                break;
            case 1:
                hurtAudioSource.PlayOneShot(hurtClip2);
                break;
        }
        Vibrate(0.5f, 0.5f, 0.1f);
        hpDamageImage.SetActive(true);
        Invoke("InactiveFlashImage", 0.02f);

        shakeTimer = hurtShakeDuration;
        currentShakeStrength = hurtShakeStrength;

        playerHp -= damage;
        UpdateHpUI();
        bulletGraze.UpdateGrazeEnergyOutside(10);
        StartCoroutine(MuTeKiTime(0.1f));

        TriggerHurtFlash();
    }

    void InactiveFlashImage()
    {
        hpDamageImage.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "EnemyBullet")
        {
            if (!isMuteki)
                getHit(10);
        }
        if (other.tag == "Block")
        {
            if (!isMuteki)
                getHit(5);
        }
    }

    private void UpdateHpUI()
    {
        float hpAmount = (float)playerHp / (float)maxHp;

        hpBarImage.fillAmount = hpAmount;
        hpText.text = playerHp.ToString();

        if (playerHp <= 0)
        {
            StopVibration();
            GameOver();
        }
    }

    public void Heal(int healAmount)
    {
        playerHp += healAmount;
        if (playerHp > maxHp)
        {
            playerHp = maxHp;
        }
        UpdateHpUI();

        shakeTimer = healShakeDuration;
        currentShakeStrength = healShakeStrength;

        TriggerHealFlash();
    }

    void GameOver()
    {
        scenesManager.isGameOver = true;
        gameOverUI.SetActive(true);
        Time.timeScale = 0;
    }

    void HandleDelayHP()
    {
        float hpAmount = (float)playerHp / (float)maxHp;

        if (delayHpBarImage.fillAmount > hpAmount)
        {
            delayHpBarImage.fillAmount -= delaySpeed * Time.deltaTime;
            if (delayHpBarImage.fillAmount < hpAmount)
            {
                delayHpBarImage.fillAmount = hpAmount;
            }
        }
    }

    IEnumerator MuTeKiTime(float mutekiTime)
    {
        isMuteki = true;
        yield return new WaitForSeconds(mutekiTime);
        isMuteki = false;
    }

    void Vibrate(float lowFrequency, float highFrequency, float duration)
    {
        if (Gamepad.current != null)
        {
            Gamepad.current.SetMotorSpeeds(lowFrequency, highFrequency);
            Invoke(nameof(StopVibration), duration);
        }
    }

    void StopVibration()
    {
        if (Gamepad.current != null)
        {
            Gamepad.current.SetMotorSpeeds(0f, 0f);
        }
    }

    void Shake(float intensity)
    {
        impulseSource.GenerateImpulseWithForce(intensity);
    }

    void TriggerHurtFlash()
    {
        FlashBackground(hurtFlashBackground);
    }

    void TriggerHealFlash()
    {
        FlashBackground(healFlashBackground);
    }

    void FlashBackground(GameObject flashTarget)
    {
        if (normalBackground != null)
            normalBackground.SetActive(false);

        if (hurtFlashBackground != null)
            hurtFlashBackground.SetActive(false);
        if (healFlashBackground != null)
            healFlashBackground.SetActive(false);

        if (flashTarget != null)
            flashTarget.SetActive(true);

        currentFlashingBackground = flashTarget;
        flashTimer = flashDuration;
    }

    void HandleHpBarShake()
    {
        if (shakeTimer > 0f)
        {
            Vector3 randomOffset = Random.insideUnitCircle * currentShakeStrength;
            hpBarRoot.localPosition = originalHpBarPos + randomOffset;
            shakeTimer -= Time.deltaTime;
        }
        else if (hpBarRoot.localPosition != originalHpBarPos)
        {
            hpBarRoot.localPosition = originalHpBarPos;
        }
    }

    void HandleHpBarFlash()
    {
        if (flashTimer > 0f)
        {
            flashTimer -= Time.deltaTime;
            if (flashTimer <= 0f)
            {
                if (normalBackground != null)
                    normalBackground.SetActive(true);

                if (hurtFlashBackground != null)
                    hurtFlashBackground.SetActive(false);
                if (healFlashBackground != null)
                    healFlashBackground.SetActive(false);

                currentFlashingBackground = null;
            }
        }
    }
}