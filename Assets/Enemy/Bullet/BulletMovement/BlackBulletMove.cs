using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackBulletMove : BulletBase
{
    [SerializeField]
    private float spreadSpeed;
    //protected Rigidbody bulletRigidbody;

    private bool initialMove = false;
    private bool moveToPlayer = false;
    private Vector3 destination;

    private void Awake()
    {
        //bulletRigidbody = GetComponent<Rigidbody>();
        bulletData = Resources.Load<EnemyBulletData>("BulletData/BlackBullet");
        //Initial();
    }
    public void Initial()
    {
        speed = bulletData.speed;
        BulletlifeTime = bulletData.lifeTime;
        initialMove = false;
        moveToPlayer = false;
        StartCoroutine(CountDownInactive(BulletlifeTime));
        moveToPlayer = true;
        //speed = bulletData.speed;
        hitEffect.gameObject.SetActive(false);
        bulletBody.SetActive(true);
    }
    public void NoMoveInitial()
    {
        initialMove = false;
        moveToPlayer = false;
        StartCoroutine(CountDownInactive(BulletlifeTime));
        speed = bulletData.speed;
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
            hitParticle.Play();
        }
    }
    public void Speard()
    {
        destination = transform.position + new Vector3(Random.Range(-10f,10f), Random.Range(-10f,10f), Random.Range(-10f,10f));
        initialMove = true;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, sphereRadius);
    }
}
