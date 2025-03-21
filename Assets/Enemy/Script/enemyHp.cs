using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class enemyHp : MonoBehaviour
{
    
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
    bool corruption;
    float CorruptionDamageModifier;

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


    [Header("UI")]
    [SerializeField]
    Canvas canvas;
    Image hpImage;

    private void Awake()
    {
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
        CorruptionDamageModifier = 0.5f;
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
            Debug.Log("Source :"+gameObject.name+" "+"ShieldBlock");
            //播抵消特效
            return;
        }
        if (corrupted)      //如果有污穢
        {
            if(corruption == true)      //如果污穢未被解除
            {
                Playhitimpact();
                currentHp -= damage * CorruptionDamageModifier;
                UpdateUI();
                Debug.Log("Source :" + gameObject.name + " " + "CorruptionDamage:"+ damage * CorruptionDamageModifier);
                if (currentHp <= 0)
                {
                    DeathEffect();
                    scoreManager.AddScore();
                }
            }
            else       //如果污穢被解除
            {
                    Playhitimpact();
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
                Playhitimpact();
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
                    Playhitimpact();
                    currentHp -= damage;
                    UpdateUI();
                    Debug.Log("Source :" + gameObject.name + " " + "CorruptionClean");
                    if (currentHp <= 0)
                    {
                        DeathEffect();
                        scoreManager.AddScore();
                    }

                }
                else
                {
                    Playhitimpact();
                    currentHp -= damage * CorruptionDamageModifier;
                    UpdateUI();
                    Debug.Log("Source :" + gameObject.name + " " + "CorruptionNotClean"+ damage * CorruptionDamageModifier);
                    if (currentHp <= 0)
                    {
                        DeathEffect();
                        scoreManager.AddScore();
                    }
                }
            }
            else
            {
                Playhitimpact();
                currentHp -= damage;
                UpdateUI();
                Debug.Log("Source :" + gameObject.name + " " + "NoCSorruption");
                if (currentHp <= 0)
                {
                    DeathEffect();
                    scoreManager.AddScore();
                }
            }
        }   
    } 

    public void DeathEffect()
    {
        GameObject effect = Instantiate(DeathExplosion, transform.position, Quaternion.identity);
        Destroy(effect, 1.5f);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
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
                    ShootHurt(1);
                }
                else
                {
                    ShootHurt(1);
                }
            }

            //Debug.Log("hit");
        }
        if(other.tag == "SlashCollider"&&slashDetectBool==false)
        {
            SlashHurt();
        }
    }
    void SlashHurt()
    {
        slashDetectBool = true;
        if (hitCounter < 3)
        {
            slashHitEffectYellowObject.SetActive(true);
            slashHitEffectYellow.Play();
            hitCounter++;
            ShieldHurt(10);
            Invoke("SetSlashDetectBool", 0.2f);
        }
        else if (hitCounter >= 3)
        {
            slashHitEffectRedObject.SetActive(true);
            slashHitEffectRed.Play();
            hitCounter = 0;
            ShieldHurt(10);
            Invoke("SetSlashDetectBool", 0.3f);
        }
    }
    void SetSlashDetectBool()
    {
        slashDetectBool = false;
    }
    IEnumerator CleanseCorruption()
    {
        corruption = false;
        corruptEffect.SetActive(false);
        yield return new WaitForSeconds(10); 
        corruptEffect.SetActive(true);
        corruption = true;
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

    private void Playhitimpact()
    {
        if (audioSource != null && hitimpactAudioClip != null)
        {
            audioSource.PlayOneShot(hitimpactAudioClip);
        }
    }

}