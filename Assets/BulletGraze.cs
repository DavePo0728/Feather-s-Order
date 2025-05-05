using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class BulletGraze : MonoBehaviour
{
    [Header("GrazeBulletData")]
    public bool canGraze;
    public float grazeCD;
    public float maxGrazeEnergy;
    public float currentGrazeEnergy;
    public float grazeEnergyGain;
    float grazeGapTimer;
    [SerializeField] float maxGrazeGapTime;

    [Header("UI 元件")]
    public Image realFill;       // 真實 MP 條（即時變化）
    public Image fakeFill;       // 虛血 MP 條（延遲跟上）
    [SerializeField] TMP_Text grazeEnergyText;
    [SerializeField] UIShaker mpUIShaker;

    [Header("虛血動畫設定")]
    public float fakeDelay = 0.3f;
    public float fakeSpeed = 0.5f;

    AudioSource grazeSound;
    AudioClip grazeClip;
    GameObject grazeEffect;
    ParticleSystem grazeEffectParticle;
    PlayerHP playerHP;
    Coroutine fakeCoroutine;

    AlphaBreathingWithYOffset shieldFlashEffect;

    public GrazeColor grazeColor;

	private void Awake()
    {
        grazeSound = GetComponent<AudioSource>();
        grazeClip = Resources.Load<AudioClip>("Sound/BulletGrazing");
        grazeEffect = transform.GetChild(0).gameObject;
        grazeEffectParticle = grazeEffect.GetComponent<ParticleSystem>();
        playerHP = GameObject.Find("HPCollider").GetComponent<PlayerHP>();
        shieldFlashEffect = GetComponentInChildren<AlphaBreathingWithYOffset>();
    }

    void Start()
    {
        canGraze = true;
        currentGrazeEnergy = 0;
        UpdateGrazeUI(); // 初始化顯示
    }

    void Update()
    {
        grazeGapTimer += Time.deltaTime;
        if (grazeGapTimer > maxGrazeGapTime)
        {
            grazeEffect.SetActive(false);
            grazeGapTimer = 0;
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            currentGrazeEnergy += maxGrazeEnergy;
            UpdateGrazeUI();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "BulletGrazeCollider")
        {
            RedBulletMove bullet = other.GetComponentInParent<RedBulletMove>();
            if (bullet != null) bullet.PlayGrazeEffect(other.transform);

            BlackBulletMove blackBullet = other.GetComponentInParent<BlackBulletMove>();
            if (blackBullet != null) blackBullet.PlayGrazeEffect(other.transform);

            if (canGraze)
            {
                grazeGapTimer = 0;
                grazeEffect.SetActive(true);
                if (!grazeEffectParticle.isPlaying)
                    grazeEffectParticle.Play();
				grazeColor.ApplyGrazeEffect();

				Vibrate(0.1f, 0.1f, 0.05f);
                grazeSound.PlayOneShot(grazeClip);

                if (shieldFlashEffect != null)
                    shieldFlashEffect.FlashLifetime();

                if (mpUIShaker != null)
                    mpUIShaker.Trigger();

                StartCoroutine(GrazeCD());

                currentGrazeEnergy += grazeEnergyGain;
                playerHP.Heal(1);
                if (currentGrazeEnergy > maxGrazeEnergy)
                    currentGrazeEnergy = maxGrazeEnergy;

                UpdateGrazeUI(); // 真實值直接更新，不觸發虛血動畫
            }
        }
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
            Gamepad.current.SetMotorSpeeds(0f, 0f);
    }

    public void UpdateGrazeEnergyOutside(float grazeEnergy)
    {
        currentGrazeEnergy -= grazeEnergy;
        if (currentGrazeEnergy < 0) currentGrazeEnergy = 0;
        UpdateGrazeUI();
        TriggerFakeMP(); // 虛血條延遲動畫
    }

    public bool CheckGrazeEnergy(float costEnergy)
    {
        if (currentGrazeEnergy >= costEnergy) return true;
        Debug.Log("Graze Energy Not Enough");
        return false;
    }

    void UpdateGrazeUI()
    {
        float ratio = currentGrazeEnergy / maxGrazeEnergy;
        realFill.fillAmount = ratio;
        fakeFill.fillAmount = Mathf.Max(fakeFill.fillAmount, ratio); // 虛血不應超過實際值
        grazeEnergyText.text = currentGrazeEnergy.ToString("0");
    }

    void TriggerFakeMP()
    {
        float target = currentGrazeEnergy / maxGrazeEnergy;
        if (fakeCoroutine != null) StopCoroutine(fakeCoroutine);
        fakeCoroutine = StartCoroutine(FakeMPRoutine(target));
    }

    IEnumerator FakeMPRoutine(float target)
    {
        yield return new WaitForSeconds(fakeDelay);
        while (fakeFill.fillAmount > target)
        {
            fakeFill.fillAmount = Mathf.MoveTowards(fakeFill.fillAmount, target, Time.deltaTime * fakeSpeed);
            yield return null;
        }
    }

    IEnumerator GrazeCD()
    {
        canGraze = false;
        yield return new WaitForSeconds(grazeCD);
        canGraze = true;
    }
}