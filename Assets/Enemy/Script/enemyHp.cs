using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHp : MonoBehaviour
{
    [SerializeField]
    EnemyMove enemyMove;
    [SerializeField]
    public float maxHp;
    [SerializeField]
    float currentHp;

    [Header("Corrupt Data")]
    [SerializeField]
    GameObject corruptEffect;
    [SerializeField]
    GameObject corruptionExplosionEffect;
    public bool corrupted;
    [SerializeField]
    bool corruption;
    public bool corruption_P => corruption;
    float corruptionDamageModifier;
    public float MaxcorruptionStack;
    float currentCorruptionStack;
    [SerializeField]
    GameObject corruptionCleanseObject,chainEffectObject;
    ParticleSystem corruptionCleanseParticle, chainEffectParticle;
    PlayerSlashAttack playerSlashAttack;
    [Header("Shield Data")]
    [SerializeField]
    GameObject shieldEffect;
    [SerializeField]
    GameObject shieldExplosionEffect;
    public bool haveshield;
    [SerializeField]
    float currentShieldHp;
    [SerializeField]
    public float maxShieldHp;
    float ShieldDamageModifier;
    [SerializeField]
    GameObject DeathExplosion;
    ScoreManager scoreManager;

    ParticleSystem slashHitEffectYellow, slashHitEffectRed;
    GameObject slashHitEffectYellowObject, slashHitEffectRedObject;
    int hitCounter;
    bool slashDetectBool =false;

    [Header("Audio")]
    [SerializeField]
    AudioClip shieldHitAudioClip; // 擊中盾音效
    [SerializeField]
    AudioClip shieldBreakAudioClip; // 破盾音效
    [SerializeField]
    AudioClip hitimpactAudioClip; // 擊中敵人聲
    AudioSource audioSource;
    [SerializeField] AudioClip deathAudioClip; // 敵人死亡音效

    [Header("UI")]
    [SerializeField]
    Canvas canvas;
    Image hpImage;

    private void Awake()
    {
        enemyMove = gameObject.GetComponent<EnemyMove>();
        canvas = transform.GetChild(7).GetComponent<Canvas>();
        audioSource = GetComponent<AudioSource>();
        shieldEffect = transform.GetChild(1).gameObject;
        shieldExplosionEffect = transform.GetChild(3).gameObject;
        corruptEffect = transform.GetChild(2).gameObject;
        slashHitEffectYellowObject = transform.GetChild(5).gameObject;
        slashHitEffectRedObject = transform.GetChild(6).gameObject;
        slashHitEffectYellow = slashHitEffectYellowObject.GetComponent<ParticleSystem>();
        slashHitEffectRed = slashHitEffectRedObject.GetComponent<ParticleSystem>();
        canvas = transform.GetChild(8).GetComponent<Canvas>();
        canvas.worldCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        hpImage = transform.GetChild(8).GetChild(0).GetChild(0).GetComponent<Image>();
        DeathExplosion = Resources.Load<GameObject>("ShadowExplosion2");
        scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
        shieldHitAudioClip = Resources.Load<AudioClip>("Sound/ShieldHitSound");
        shieldBreakAudioClip = Resources.Load<AudioClip>("Sound/ShieldBreakSound");
        hitimpactAudioClip = Resources.Load<AudioClip>("Sound/HitImpactSound");
        currentCorruptionStack = 0;
        corruptionCleanseObject = transform.GetChild(9).gameObject;
        corruptionCleanseParticle =corruptionCleanseObject.GetComponent<ParticleSystem>();
        chainEffectObject = transform.GetChild(10).gameObject;
        chainEffectParticle = chainEffectObject.GetComponent<ParticleSystem>();
        playerSlashAttack = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerSlashAttack>();
        deathAudioClip = Resources.Load<AudioClip>("Sound/EnemyDeathSound");
    }
    // Start is called before the first frame update
    void Start()
    {
        if (corrupted)
        {
            corruption = true;
            corruptEffect.SetActive(true);
        }
        else 
        {
            corruption = false;
            corruptEffect.SetActive(false);
        }
        corruptionDamageModifier = 0.5f;
        //ShieldDamageModifier = 1.5f;
        currentHp = maxHp;
        UpdateUI();
        //Debug.Log("haveShield" + haveshield);

        if (haveshield)
        {
            
            currentShieldHp = maxShieldHp;
            shieldEffect.SetActive(true);
        }
        else
        {
            shieldEffect.SetActive(false);
            currentShieldHp = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShootHurt(float damage)
    {
        if (haveshield)     //打到盾無效
        {
            PlayShieldHitSound();
            //Debug.Log("Source :"+gameObject.name+" "+"ShieldBlock");
            //播抵消特效
            return;
        }
        if (corrupted)      //如果有污穢
        {
            if(corruption == true)      //如果污穢未被解除
            {
                PlayhitimpactAudio();
                currentHp -= damage * corruptionDamageModifier;
                UpdateUI();
                //Debug.Log("Source :" + gameObject.name + " " + "CorruptionDamage:"+ damage * corruptionDamageModifier);
                if (currentHp <= 0)
                {
                    DeathEffect();
                    scoreManager.AddScore();
                }
            }
            else       //如果污穢被解除
            {
                    PlayhitimpactAudio();
                    currentHp -= damage;
                    UpdateUI();
                    if (currentHp <= 0)
                    {
                        DeathEffect();
                        scoreManager.AddScore();
                    }
            }
            
        }
        else       //沒有污穢
        {
                PlayhitimpactAudio();
                currentHp -= damage;
                UpdateUI();
                if (currentHp <= 0)
                {
                    DeathEffect();
                    scoreManager.AddScore();
                }
        }
    }

    public void ShieldHurt(float damage) //只有近攻會觸發這個
    {
        if (currentShieldHp > 0)
        {
            PlayShieldHitSound();
            currentShieldHp -= damage;
            UpdateUI();
            if (shieldEffect != null && currentShieldHp <= 0)
            {
                shieldEffect.SetActive(false);
                shieldExplosionEffect.SetActive(true);
                PlayShieldBreakSound();
                haveshield = false;
            }
        }
        else
        {
            if (corrupted)
            {
                if (corruption == false)
                {
                    PlayhitimpactAudio();
                    currentHp -= damage;
                    UpdateUI();
                    Debug.Log("Source :" + gameObject.name + " " + "CorruptionClean");
                    if (currentHp <= 0)
                    {
                        DeathEffect();
                        scoreManager.AddScore();
                        PlayerHP playerHP = GameObject.FindGameObjectWithTag("HPCollider").GetComponent<PlayerHP>();
                        playerHP.Heal(10);
                    }

                }
            }
            else
            {
                PlayhitimpactAudio();
                currentHp -= damage;
                UpdateUI();
                Debug.Log("Source :" + gameObject.name + " " + "NoCSorruption");
                if (currentHp <= 0)
                {
                    DeathEffect();
                    scoreManager.AddScore();
                    PlayerHP playerHP = GameObject.FindGameObjectWithTag("HPCollider").GetComponent<PlayerHP>();
                    playerHP.Heal(10);
                }
            }
        }   
    }

    public void DeathEffect()
    {
        GameObject sfxPlayer = new GameObject("DeathSFX");
        sfxPlayer.transform.position = transform.position;

        AudioSource sfxAudio = sfxPlayer.AddComponent<AudioSource>();
        sfxAudio.clip = deathAudioClip;

        //  音量控制（你可以這裡調整音量大小）
        sfxAudio.volume = 0.8f;

        //  空間感：讓聲音根據距離遠近衰減
        sfxAudio.spatialBlend = 1f;       // 3D 音效
        sfxAudio.minDistance = 50f;        // 在這距離內聲音不變
        sfxAudio.maxDistance = 300f;       // 超過這距離聲音最小

        //  混音群組（可選，如果你用 Audio Mixer）
        // sfxAudio.outputAudioMixerGroup = yourEnemySFXGroup;

        sfxAudio.Play();

        Destroy(sfxPlayer, deathAudioClip.length);

        GameObject effect = Instantiate(DeathExplosion, transform.position, Quaternion.identity);
        Destroy(effect, 1.5f);

        Destroy(gameObject); // 本體照常清除
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((other.tag=="PlayerMissile"))
        {
            if (currentShieldHp <= 0)
            {
                if (corrupted)
                {
                    StartCoroutine(CleanseCorruption());
                    ShootHurt(5);
                }
                else
                {
                    ShootHurt(5);
                }
            }
        }
        if (other.tag == "PlayerBullet")
        {
            ShootHurt(1);
        }
        if (other.tag == "ChargeBullet")
        {
            if(currentShieldHp <= 0)
            {
                if (corrupted)
                {
                    StartCoroutine(CleanseCorruption());
                    ShootHurt(20);
                }
                else
                {
                    ShootHurt(20);
                }
            }

            //Debug.Log("hit");
        }
        if(other.tag == "SlashCollider"&&slashDetectBool==false)
        {
            SlashHurt(20);
        }
    }
    void SlashHurt(float damage)
    {
        slashDetectBool = true;
        if (hitCounter < 3)
        {
            slashHitEffectYellowObject.SetActive(true);
            slashHitEffectYellow.Play();
            hitCounter++;
            ShieldHurt(damage);
            Invoke("SetSlashDetectBool", 0.2f);
        }
        else if (hitCounter >= 3)
        {
            slashHitEffectRedObject.SetActive(true);
            slashHitEffectRed.Play();
            hitCounter = 0;
            ShieldHurt(damage);
            Invoke("SetSlashDetectBool", 0.3f);
        }
    }
    void SetSlashDetectBool()
    {
        slashDetectBool = false;
    }
    IEnumerator CleanseCorruption()
    {
        currentCorruptionStack++;
        if (currentCorruptionStack>=MaxcorruptionStack)
        {
            corruption = false;
            corruptionCleanseObject.SetActive(true);
            chainEffectObject.SetActive(true);
            chainEffectParticle.Play();
            corruptionCleanseParticle.Play();
            corruptEffect.SetActive(false);
            StartCoroutine(enemyMove.Paralyze());
            yield return new WaitForSeconds(enemyMove.paralyzeTime);
            corruption = true;
            playerSlashAttack.ForceFallBack();
            corruptionCleanseObject.SetActive(false);
            chainEffectObject.SetActive(false);
            corruptEffect.SetActive(true);
            
        }
    }
    void UpdateUI()
    {
        float HpAmount = (float)currentHp / (float)maxHp;
        //Debug.Log(HpAmount);
        hpImage.fillAmount = HpAmount;
    }
    private void PlayShieldHitSound()
    {
        if (audioSource != null && shieldHitAudioClip != null)
        {
            audioSource.PlayOneShot(shieldHitAudioClip);
        }
    }

    private void PlayShieldBreakSound()
    {
        if (audioSource != null && shieldBreakAudioClip != null)
        {
            audioSource.PlayOneShot(shieldBreakAudioClip);
        }
    }

    private void PlayhitimpactAudio()
    {
        if (audioSource != null && hitimpactAudioClip != null)
        {
            audioSource.PlayOneShot(hitimpactAudioClip);
        }
    }

}