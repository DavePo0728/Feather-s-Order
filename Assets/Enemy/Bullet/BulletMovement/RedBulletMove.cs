using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RedBulletMove : BulletBase
{
    [SerializeField] public Transform playerTransform;
    Rigidbody bulletRigidbody;
    //bool initialMove = false;
    [SerializeField]
    bool moveToPlayer = false;
    [SerializeField]
    float minSpread, MaxSpread;
    [SerializeField]
    private float spreadSpeed;
    private Vector3 destination;
    public bool homing =false;
    bool homingMove = false;
    float homingTime;
    float maxRotateAngle;
    GameObject player;
    float homingTimer;
    Tweener speard;
    public bool fireWork =false;
    Vector3 direction;
    public Collider bulletGrazeCollider;

    public GameObject grazeEffectPrefab;

    //擦彈特效
    public void PlayGrazeEffect(Transform grazePoint)
    {
        if (grazeEffectPrefab != null)
        {
            Vector3 spawnPos = grazePoint.position;

            //  若有玩家參考，加個偏移會更爽感
            if (playerTransform != null)
            {
                Vector3 dir = (playerTransform.position - grazePoint.position).normalized;
                spawnPos += dir * -0.5f; // 可微調距離
            }

            GameObject effect = Instantiate(grazeEffectPrefab, spawnPos, Quaternion.identity);
            effect.transform.localScale = Vector3.one;
            ParticleSystem ps = effect.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Play();
            }
            Destroy(effect, 1f);
        }
    }
    private void Awake()
    {
        bulletRigidbody = GetComponent<Rigidbody>();
        bulletData = Resources.Load<EnemyBulletData>("BulletData/RedBullet");
        bulletBody= transform.GetChild(0).gameObject;
        hitEffect = transform.GetChild(1).gameObject;
        hitParticle = hitEffect.GetComponent<ParticleSystem>();
        BulletCollider = GetComponent<Collider>();
        bulletGrazeCollider =transform.GetChild(0).GetComponent<Collider>();
    }
    private void OnDisable()
    {
        fireWork = false;
        homing = false;
        homingMove = false;
        moveToPlayer = false;
    }
    public void Initial()
    {
        speed = bulletData.speed;
        BulletlifeTime = bulletData.lifeTime;
        //initialMove = false;
        moveToPlayer = false;
        StartCoroutine(CountDownInactive(BulletlifeTime));
        moveToPlayer = true;
        hitEffect.SetActive(false);
        bulletBody.SetActive(true);
        BulletCollider.enabled = true;
        bulletGrazeCollider.enabled = true;
    }
    public void NoMoveInitial()
    {
        BulletlifeTime = bulletData.lifeTime;
        speed = bulletData.speed;
        spreadSpeed = 1f;
        minSpread = -5f;
        MaxSpread = 5f;
        //initialMove = false;
        moveToPlayer = false;
        StartCoroutine(CountDownInactive(BulletlifeTime));
        hitEffect.SetActive(false);
        bulletBody.SetActive(true);
        BulletCollider.enabled = true;
        bulletGrazeCollider.enabled = true;
    }
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (moveToPlayer)
        {
            transform.Translate(Vector3.forward * speed);
            //Debug.Log(speed);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            bulletGrazeCollider.enabled = false;
        }
        if (other.tag == "PlayerBullet")
        {
            speed = 0;
            other.gameObject.SetActive(false);
            moveToPlayer = false;
            bulletBody.SetActive(false);
            hitEffect.gameObject.SetActive(true);
            hitParticle.Play();
            BulletCollider.enabled = false;
            Debug.Log("PlayerBullet Hit");
		}
        if (other.tag == "ChargeBullet")
		{
            Debug.Log("ChargeBullet Hit");
			speed = 0;
			//other.gameObject.SetActive(false);
			moveToPlayer = false;
			bulletBody.SetActive(false);
			hitEffect.gameObject.SetActive(true);
			hitParticle.Play();
            BulletCollider.enabled = false;

        }
        if (other.tag == "HPCollider")
        {
            speed = 0;
            moveToPlayer = false;
            bulletBody.SetActive(false);
            hitEffect.gameObject.SetActive(true);
            hitParticle.Play();
            BulletCollider.enabled = false;
        }

    }
    public void Speard()
    {
        destination = transform.position + new Vector3(Random.Range(minSpread, MaxSpread), Random.Range(minSpread, MaxSpread), Random.Range(minSpread, MaxSpread));
        speard = transform.DOMove(destination, spreadSpeed);
        if (homing) {
            speard.OnComplete(() => { IsHoming(); });
        }
        else
        {
            speard.OnComplete(() => { moveToPlayer = true; });
        }
        //Debug.Log(destination);
        //initialMove = true;
    }
    public void IsHoming()
    {
        homingMove = true;
    }
    public void FlatSpeard()
    {
        //destination = transform.position + new Vector3(Random.Range(minSpread, MaxSpread),0, Random.Range(minSpread, MaxSpread));
        transform.rotation = Quaternion.Euler(transform.rotation.x, Random.Range(135f,225f), transform.rotation.z);
        //speard = transform.DOMove(destination, spreadSpeed);
        //speard.OnComplete(() => { moveToPlayer = true; });
        //Debug.Log(destination);
        //initialMove = true;
    }
    public void SphereSpread(Vector3 dir)
    {
        direction = dir.normalized;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, sphereRadius);
    }
    void Homing()
    {
        Debug.Log("Homing");
        Vector3 targetDirection = (player.transform.position - transform.position).normalized;

        // 計算每秒最大旋轉角度
        float maxRotationAngle = 15f * Time.deltaTime;

        // 計算目標旋轉
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

        // 旋轉到目標方向
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, maxRotationAngle);

    }
    
}
