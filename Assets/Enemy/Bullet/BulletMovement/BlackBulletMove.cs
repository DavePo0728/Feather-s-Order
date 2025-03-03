using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackBulletMove : BulletBase
{
    [SerializeField]
    private float spreadSpeed;
    //protected Rigidbody bulletRigidbody;
    [SerializeField]
    float minSpread, MaxSpread;
    public bool homing = false;
    bool homingMove = false;
    float homingTime;
    float maxRotateAngle;
    float homingTimer;
    Tweener speard;
    private bool initialMove = false;
    private bool moveToPlayer = false;
    private Vector3 destination;
    Vector3 direction;
    GameObject player;


    private void Awake()
    {
        //bulletRigidbody = GetComponent<Rigidbody>();
        bulletData = Resources.Load<EnemyBulletData>("BulletData/BlackBullet");
        BulletCollider = GetComponent<Collider>();
        bulletBody = transform.GetChild(0).gameObject;
        //Initial();
    }
    public void Initial()
    {
        BulletCollider.enabled = true;
        speed = bulletData.speed;
        BulletlifeTime = bulletData.lifeTime;
        initialMove = false;
        moveToPlayer = false;
        StartCoroutine(CountDownInactive(BulletlifeTime));
        moveToPlayer = true;
        speed = bulletData.speed;
        hitEffect= transform.GetChild(1).gameObject;
        hitEffect.gameObject.SetActive(false);
        bulletBody.SetActive(true);
    }
    public void NoMoveInitial()
    {
        BulletCollider.enabled = true;
        initialMove = false;
        moveToPlayer = false;
        StartCoroutine(CountDownInactive(BulletlifeTime));
        speed = bulletData.speed;
    }
    public void HomingInitial()
    {
        BulletCollider.enabled = true;
        BulletCollider.enabled = true;
        //hitEffect.SetActive(false);
        bulletBody.SetActive(true);
        BulletlifeTime = bulletData.lifeTime;
        speed = bulletData.speed;
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
    private void OnDisable()
    {
        //fireWork = false;
        homing = false;
        homingMove = false;
        moveToPlayer = false;
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
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, Random.Range(transform.rotation.eulerAngles.y-15f, transform.rotation.eulerAngles.y + 15f), transform.eulerAngles.z);
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
    void FixedUpdate()
    {
        if (initialMove)
        {
            var step = spreadSpeed * Time.deltaTime; // calculate distance to move
            transform.position = Vector3.MoveTowards(transform.position, destination, step);
            if (Vector3.Distance(transform.position, destination) < 0.001f)
            {
                initialMove = false;
                moveToPlayer = true;
            }
        }
        //if (speed != bulletData.speed)
        //{
        //    speed = bulletData.speed;
        //    // bulletData.DataUpdate = false;
        //}
        if (moveToPlayer)
        {
            transform.Translate(Vector3.forward * speed);
        }
        //Debug.Log("Bullet"+ speed);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            moveToPlayer = false;
            bulletBody.SetActive(false);
            hitEffect.gameObject.SetActive(true);
            BulletCollider.enabled = false;
            hitParticle.Play();
        }
    }
    //public void Speard()
    //{
    //    destination = transform.position + new Vector3(Random.Range(-10f,10f), Random.Range(-10f,10f), Random.Range(-10f,10f));
    //    initialMove = true;
    //}

}
