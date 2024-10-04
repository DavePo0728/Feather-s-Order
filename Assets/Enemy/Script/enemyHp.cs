using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyHp : MonoBehaviour
{
    float Maxhp = 10;
    float currentHp;
    GameObject DeathExplosion;
    ScoreManager scoreManager;
    // Start is called before the first frame update
    void Start()
    {
        currentHp = Maxhp;
        DeathExplosion = Resources.Load<GameObject>("ShadowExplosion2");
        scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Hurt()
    {
        if (currentHp <= 0)
        {
            DeathEffect();
            scoreManager.AddScore();
        }
        else
        {
            currentHp--;
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
            Hurt();
        }
    }
}
