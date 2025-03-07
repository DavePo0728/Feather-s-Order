using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighSpeedVioletBulletMove : BulletBase
{
    //bool initialMove = false;
    [SerializeField]
    bool moveToward = false;
    [SerializeField]
    float minSpread, MaxSpread;
    [SerializeField]
    private float spreadSpeed;
    private Vector3 destination;
    public bool homing = false;
    bool homingMove = false;
    public float homingTime;
    float maxRotateAngle;
    GameObject player;
    float homingTimer;
    Tweener speard;
    public bool fireWork = false;
    Vector3 direction;
    private Transform homingShooterTransform; // 存儲 HomingShooter 的 Transform

    private void Awake()
    {
        ///bulletRigidbody = GetComponent<Rigidbody>();
        bulletData = Resources.Load<EnemyBulletData>("BulletData/VioletBullet");
        player = GameObject.FindGameObjectWithTag("Player");
        BulletCollider = GetComponent<Collider>();
    }

    private void OnDisable()
    {
        fireWork = false;
        homing = false;
        homingMove = false;
        moveToward = false;
    }

    public void Initial()
    {
        speed = bulletData.speed;
        BulletlifeTime = bulletData.lifeTime;
        //initialMove = false;
        moveToward = false;
        StartCoroutine(CountDownInactive(BulletlifeTime));
        moveToward = true;
        //hitEffect.SetActive(false);
        //bulletBody.SetActive(true);
    }
    public void NoMoveInitial()
    {
        BulletlifeTime = bulletData.lifeTime;
        speed = bulletData.speed;
        spreadSpeed = 1f;
        minSpread = -5f;
        MaxSpread = 5f;
        //initialMove = false;
        moveToward = false;
        StartCoroutine(CountDownInactive(BulletlifeTime));
    }
    public void HomingInitial(GameObject point)
    {
        homingMove = false;
        homingTimer = 0;
        moveToward = false;
        speed = 0;
        BulletlifeTime = bulletData.lifeTime;
        StartCoroutine(CountDownInactive(BulletlifeTime));
        Tweener homing = transform.DOMove(point.transform.position, 0.1f);
        speed = bulletData.speed;
        homing.OnComplete(() => { homingMove = true; speed = bulletData.speed; });
    }
    public void FireWorkInitial()
    {
        BulletlifeTime = bulletData.lifeTime;
        speed = bulletData.speed;
        moveToward = false;
        fireWork = true;
        StartCoroutine(CountDownInactive(BulletlifeTime));
    }
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (homingMove)
        {
            homingTimer += Time.deltaTime;
            //Debug.Log(homingTimer);
        }

        if (homingMove && homingTimer <= homingTime)
        {
            Debug.Log("Homing");
            Vector3 targetDirection = (player.transform.position - transform.position).normalized;

            // 計算每秒最大旋轉角度
            float maxRotationAngle = 120f * Time.deltaTime;  

            // 計算目標旋轉
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

            // 旋轉到目標方向
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, maxRotationAngle);
            transform.Translate(transform.forward* speed);
            if (Vector3.Distance(transform.position, player.transform.position) < 10)
            {
                homingMove = false;
            }

        }
        else
        {
            moveToward = true;
        }
        if (moveToward)
        {
            transform.Translate(Vector3.forward * speed);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            gameObject.SetActive(false);
        }
    }

    public void Speard()
    {
        destination = transform.position + new Vector3(Random.Range(minSpread, MaxSpread), Random.Range(minSpread, MaxSpread), Random.Range(minSpread, MaxSpread));
        speard = transform.DOMove(destination, spreadSpeed);
        if (homing)
        {
            speard.OnComplete(() => { IsHoming(); });
        }
        else
        {
            speard.OnComplete(() => { moveToward = true; });
        }
    }

    public void IsHoming()
    {
        homingMove = true;
    }

    public void FlatSpeard()
    {
        transform.rotation = Quaternion.Euler(transform.rotation.x, Random.Range(135f, 225f), transform.rotation.z);
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

    public void SetHomingShooterTransform(Transform shooterTransform)
    {
        homingShooterTransform = shooterTransform;
    }
}
