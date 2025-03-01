using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighSpeedVioletBulletMove : BulletBase
{
    //bool initialMove = false;
    [SerializeField]
    bool moveToPlayer = false;
    [SerializeField]
    float minSpread, MaxSpread;
    [SerializeField]
    private float spreadSpeed;
    private Vector3 destination;
    public bool homing = false;
    bool homingMove = false;
    float homingTime;
    float maxRotateAngle;
    GameObject player;
    float homingTimer;
    Tweener speard;
    public bool fireWork = false;
    Vector3 direction;

    private void Awake()
    {
        ///bulletRigidbody = GetComponent<Rigidbody>();
        bulletData = Resources.Load<EnemyBulletData>("BulletData/PurpleBullet");
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
        moveToPlayer = false;
        StartCoroutine(CountDownInactive(BulletlifeTime));
    }
    public void HomingInitial()
    {
        BulletlifeTime = bulletData.lifeTime;
        speed = bulletData.speed;
        spreadSpeed = 0.2f;
        minSpread = -10f;
        MaxSpread = 10f;
        if (homing)
        {
            homingTime = 2f;
            maxRotateAngle = 15f;
            homingTimer = 0;
        }
        destination = CurvePathGenerator.pathInstance.GetLandingPosZ(transform.position, 10);
        //initialMove = false;
        moveToPlayer = false;
        StartCoroutine(CountDownInactive(BulletlifeTime));
    }
    public void FireWorkInitial()
    {
        BulletlifeTime = bulletData.lifeTime;
        speed = bulletData.speed;
        moveToPlayer = false;
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
            homingTimer += Time.deltaTime;
        //if (initialMove)
        //{
        //var step = spreadSpeed * Time.deltaTime; // calculate distance to move
        //transform.position = Vector3.MoveTowards(transform.position, destination, step);
        //initialMove = false;
        if (homing)
        {
            IsHoming();
            if (homingMove && homingTimer <= homingTime)
            {
                Homing();
            }
            else
            {
                homingMove = false;
                moveToPlayer = true;
            }
        }
        else
        {
            moveToPlayer = true;
        }
        //}
        if (fireWork)
        {
            transform.Translate(transform.forward * speed);
            Debug.Log(direction);
        }

        if (moveToPlayer)
        {
            transform.Translate(Vector3.forward * speed);
            //Debug.Log(speed);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PlayerBullet")
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
        transform.rotation = Quaternion.Euler(0, Random.Range(135f, 225f), 0);
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
