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
    float currentCorruptionValue;
    public float maxCorruptionValue;
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
    [SerializeField] AudioClip SlashHITClip; // 敵人近戰受擊音效

    [Header("UI")]
    [SerializeField]
    Canvas canvas;
    Image hpImage;
    Image corruptionImage;

    private void Awake()
    {
        enemyMove = gameObject.GetComponent<EnemyMove>();
        canvas = transform.Find("StatusCanvas").GetComponent<Canvas>();
        audioSource = GetComponent<AudioSource>();
        shieldEffect = transform.Find("MagicShieldBlue").gameObject;
        shieldExplosionEffect = transform.Find("TargetHitExplosion").gameObject;
        corruptEffect = transform.Find("CorruptionEffect").gameObject;
        slashHitEffectYellowObject = transform.Find("SwordHitMagicYellow").gameObject;
        slashHitEffectRedObject = transform.Find("SwordHitRedCritical").gameObject;
        slashHitEffectYellow = slashHitEffectYellowObject.GetComponent<ParticleSystem>();
        slashHitEffectRed = slashHitEffectRedObject.GetComponent<ParticleSystem>();
        canvas.worldCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        hpImage = canvas.transform.GetChild(0).GetChild(0).Find("HPBar").GetComponent<Image>();
        DeathExplosion = Resources.Load<GameObject>("ShadowExplosion2");
        scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
        shieldHitAudioClip = Resources.Load<AudioClip>("Sound/ShieldHitSound");
        shieldBreakAudioClip = Resources.Load<AudioClip>("Sound/ShieldBreakSound");
        hitimpactAudioClip = Resources.Load<AudioClip>("Sound/HitImpactSound");
        SlashHITClip = Resources.Load<AudioClip>("Sound/slashHit");
        corruptionImage = canvas.transform.GetChild(0).GetChild(1).Find("CorruptionBar").GetComponent<Image>();
        corruptionCleanseObject = transform.Find("CorruptionCleanse").gameObject;
        corruptionCleanseParticle =corruptionCleanseObject.GetComponent<ParticleSystem>();
        chainEffectObject = transform.Find("ChainEffect").gameObject;
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
            currentCorruptionValue = 0;
        }
        else 
        {
            corruption = false;
            corruptEffect.SetActive(false);
            currentCorruptionValue = 0;
        }
        corruptionDamageModifier = 0.5f;
        currentHp = maxHp;
        UpdateUI();
        UpdateCorruptionUI();

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
        canvas.gameObject.SetActive(false); // 先隱藏UI
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
            if (canvas.gameObject.activeSelf == false)
                canvas.gameObject.SetActive(true); // 顯示UI
            if (corruption == true)      //如果污穢未被解除
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
            if (canvas.gameObject.activeSelf == false)
                canvas.gameObject.SetActive(true); // 顯示UI
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
        if (haveshield)
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
        }
        else
        {
            if (corrupted)
            {
                if (canvas.gameObject.activeSelf == false)
                    canvas.gameObject.SetActive(true); // 顯示UI
                if (corruption == false)
                {
                    PlayhitimpactAudio();
                    currentHp -= damage;
                    UpdateUI();
                    //Debug.Log("Source :" + gameObject.name + " " + "CorruptionClean");
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
                if (canvas.gameObject.activeSelf == false)
                    canvas.gameObject.SetActive(true); // 顯示UI
                PlayhitimpactAudio();
                currentHp -= damage;
                UpdateUI();
                //Debug.Log("Source :" + gameObject.name + " " + "NoCSorruption");
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
                    PlayerMissileMove playerMissileMove = other.GetComponent<PlayerMissileMove>();
                    CleanseCorruption(playerMissileMove.corruptionDamage);
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

                    CleanseCorruption(10);
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
            SlashHurt(40);
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

            PlaySlashHitAudio(); // <<<<< 新增！播放近戰打擊音效
        }
        else if (hitCounter >= 3)
        {
            slashHitEffectRedObject.SetActive(true);
            slashHitEffectRed.Play();
            hitCounter = 0;
            ShieldHurt(damage);
            Invoke("SetSlashDetectBool", 0.3f);

            PlaySlashHitAudio(); // <<<<< 新增！播放近戰打擊音效
        }
    }
    void SetSlashDetectBool()
    {
        slashDetectBool = false;
    }
    void CleanseCorruption(int corruptionDamage)
    {
        if (currentCorruptionValue < maxCorruptionValue)
        {
            currentCorruptionValue += corruptionDamage;
            UpdateCorruptionUI();
        }
        if (corruption)
        {
            if (currentCorruptionValue >= maxCorruptionValue)
            {
                corruption = false;
                corruptionCleanseObject.SetActive(true);
                chainEffectObject.SetActive(true);
                chainEffectParticle.Play();
                corruptionCleanseParticle.Play();
                corruptEffect.SetActive(false);
                enemyMove.Paralyze();
            }
        }
        else
        {
            enemyMove.Paralyze();
        }
    }
    public void CorruptionRecover()
    {
        corruption = true;
        currentCorruptionValue = 0;
        UpdateCorruptionUI();
        playerSlashAttack.ForceFallBack();
        corruptionCleanseObject.SetActive(false);
        chainEffectObject.SetActive(false);
        corruptEffect.SetActive(true);
    }
    void UpdateCorruptionUI()
    {
        float CorruptionAmount = currentCorruptionValue / maxCorruptionValue;
        corruptionImage.fillAmount = CorruptionAmount;
    }
    void UpdateUI()
    {
        float HpAmount = currentHp / maxHp;
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
    private void PlaySlashHitAudio()
    {
        if (audioSource != null && SlashHITClip != null)
        {
            audioSource.pitch = Random.Range(0.95f, 1.05f); // 在 AudioSource 上調音高
            audioSource.PlayOneShot(SlashHITClip); // 播放 Clip
            audioSource.pitch = 1f; // 播放後重置回正常，避免後面別的聲音也受影響
        }
    }

}