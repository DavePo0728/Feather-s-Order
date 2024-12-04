using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyHp : MonoBehaviour
{
    float Maxhp = 5;
    float currentHp;
    GameObject shieldEffect,shieldExplosionEffect;
    public bool haveshield;
    [SerializeField]
    float currentShieldHp;
    [SerializeField]
    float maxShieldHp;
    float shieldDamageMultiplier;
    GameObject DeathExplosion;
    ScoreManager scoreManager;
    // Start is called before the first frame update
    void Start()
    {
        currentHp = Maxhp;
        DeathExplosion = Resources.Load<GameObject>("ShadowExplosion2");
        scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
        shieldEffect = transform.GetChild(1).gameObject;
        shieldExplosionEffect = transform.GetChild(2).gameObject;
        if (haveshield)
        {
            currentShieldHp = maxShieldHp;
            shieldEffect.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Hurt(float damage)
    {
        shieldDamageMultiplier = 0.5f;
        if (currentShieldHp > 0)
        {
            currentShieldHp-= damage*shieldDamageMultiplier;
        }
        else
        {
            if (currentShieldHp > 0)
            {
                currentShieldHp -= damage;
            }
            else
            {
                shieldEffect.SetActive(false);
                shieldExplosionEffect.SetActive(true);
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
    public void ShieldHurt(float damage)
    {
        shieldDamageMultiplier = 1.5f;
        if (currentShieldHp > 0)
        {
            currentShieldHp -= damage * shieldDamageMultiplier;
        }
        else
        {
            if (currentShieldHp > 0)
            {
                currentShieldHp-= damage;
            }
            else
            {
                shieldEffect.SetActive(false);
                shieldExplosionEffect.SetActive(true);
                if (currentHp <= 0)
                {
                    DeathEffect();
                    scoreManager.AddScore();
                }
                else
                {
                    currentHp-=damage;
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
            //Debug.Log("hit");
        }
        if(other.tag == "ChargeBullet")
        {
            ShieldHurt(5);
        }
    }
}
