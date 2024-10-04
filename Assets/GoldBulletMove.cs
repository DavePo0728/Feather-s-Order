using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldBulletMove : EnemyBulletMove
{
    [SerializeField]
    GameObject GoldBulletExplosion;
    float lifeTime = 10f;
    MeshRenderer meshRenderer;
    [HideInInspector]
    public bool bounceBack = false;
    bool goback=false;
    ScoreManager scoreManager;
    private void Awake()
    {
        bulletData = Resources.Load<EnemyBulletData>("BulletData/NormalBullet");
        
    }
    // Start is called before the first frame update
    void Start()
    {
        moveToPlayer = false;
        StartCoroutine(CountDownInactive(lifeTime));
        moveToPlayer = true;
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
        speed = bulletData.speed;
        scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (moveToPlayer)
        {
            transform.Translate(-Vector3.forward * speed);
        }
        if (bounceBack)
        {
            goback = true;
            if(gameObject.transform.position.z >= 300)
            {
                StartCoroutine(GoldBulletEffect());
            }
        }
        if (speed != bulletData.speed&&!goback)
        {
            speed = bulletData.speed;
        }
    }
    IEnumerator GoldBulletEffect()
    {
        Debug.Log("gold");
        bounceBack = false;
        speed = 0;
        meshRenderer.enabled = false;
        Collider[] destroyList = Physics.OverlapSphere(transform.position,sphereRadius);
        for(int i=0; i < destroyList.Length; i++)
        {
            Debug.Log(i+":"+destroyList[i].tag);
            switch (destroyList[i].tag)
            {
                case "EnemyBullet":
                    destroyList[i].gameObject.SetActive(false);
                    break;
                case "Enemy":
                    enemyHp _enemyHp = destroyList[i].gameObject.GetComponent<enemyHp>();
                    _enemyHp.DeathEffect();
                    scoreManager.AddScore();
                    break;
            }
        }
        GameObject temp = Instantiate(GoldBulletExplosion, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(2f);
        Destroy(temp);
        Destroy(gameObject);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (bounceBack)
        {
            if(other.tag == "Enemy")
            {
                Debug.Log("Hit"+ other.name);
                StartCoroutine(GoldBulletEffect());
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, sphereRadius);
    }
}
