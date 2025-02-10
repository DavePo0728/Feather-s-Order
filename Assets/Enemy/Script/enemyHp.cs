using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyHp : MonoBehaviour
{
    [SerializeField]
    float Maxhp;
    float currentHp;
    [SerializeField]
    GameObject shieldEffect, shieldExplosionEffect;
    public bool haveshield;
    [SerializeField]
    float currentShieldHp;
    [SerializeField]
    float maxShieldHp;
    float shieldDamageMultiplier;
    GameObject DeathExplosion;
    ScoreManager scoreManager;
    [SerializeField]
    AudioSource shieldHitAudioSource; // 擊中盾音效播放器
    [SerializeField]
    AudioSource shieldBreakAudioSource; // 破盾音效播放器
    [SerializeField]
    AudioSource hitimpact; // 擊中敵人聲
    [SerializeField]
    GameObject soundManager;

    // Start is called before the first frame update
    void Start()
    {
        
        currentHp = Maxhp;
        DeathExplosion = Resources.Load<GameObject>("ShadowExplosion2");
        scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();

        if (haveshield)
        {
            soundManager = GameObject.FindGameObjectWithTag("SoundManager");
            shieldHitAudioSource = soundManager.transform.GetChild(3).GetComponent<AudioSource>();
            shieldBreakAudioSource = soundManager.transform.GetChild(4).GetComponent<AudioSource>();
            hitimpact = soundManager.transform.GetChild(5).GetComponent<AudioSource>(); 
            shieldEffect = transform.GetChild(2).gameObject;
            shieldExplosionEffect = transform.GetChild(3).gameObject;
            currentShieldHp = maxShieldHp;
            shieldEffect.SetActive(true);
        }
        else
        {
            currentShieldHp = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Hurt(float damage)
    {
        shieldDamageMultiplier = 0.5f;
        if (currentShieldHp > 0) // 擊中盾的音效
        {
            PlayShieldHitSound(); // 播放擊中盾音效
            currentShieldHp -= damage * shieldDamageMultiplier;
            
        }
        else
        {
            if (shieldEffect != null&&haveshield) // 破盾的音效
            {
                haveshield = false;
                shieldEffect.SetActive(false);
                shieldExplosionEffect.SetActive(true);
                
                PlayShieldBreakSound(); // 播放破盾音效
            }
            if (currentShieldHp > 0)
            {
                currentShieldHp -= damage;
            }
            else
            {
                if (currentHp <= 0)
                {
                    DeathEffect();
                    scoreManager.AddScore();
                }
                else
                {
                    Playhitimpact();
                    currentHp -= damage;
                }
            }
        }
    }

    public void ShieldHurt(float damage)
    {
        shieldDamageMultiplier = 1.5f;
        if (currentShieldHp > 0)
        {
            currentShieldHp -= damage * shieldDamageMultiplier;
            
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
            if (currentShieldHp > 0)
            {
                currentShieldHp -= damage;
            }
            else
            {
                if (currentHp <= 0)
                {
                    DeathEffect();
                    scoreManager.AddScore();
                }
                else
                {
                    currentHp -= damage;
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
            Hurt(1);
            // Debug.Log("hit");
        }
        if (other.tag == "ChargeBullet")
        {
            ShieldHurt(10);
        }
    }

    private void PlayShieldHitSound()
    {
        //soundManager = GameObject.Find("SoundManager").GetComponent<GameObject>();
        //shieldHitAudioSource = soundManager.transform.GetChild(3).GetComponent<AudioSource>();
        if (shieldHitAudioSource != null && shieldHitAudioSource.clip != null)
        {
            shieldHitAudioSource.Play();
        }
    }

    private void PlayShieldBreakSound()
    {
        if (shieldBreakAudioSource != null && shieldBreakAudioSource.clip != null)
        {
            shieldBreakAudioSource.Play();
        }
    }

    private void Playhitimpact()
    {
        if (hitimpact != null && hitimpact.clip != null)
        {
            hitimpact.Play();
        }
    }
    
}