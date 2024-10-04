using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBulletMove : MonoBehaviour
{
    protected EnemyBulletData bulletData;
    public float speed;
    [SerializeField]
    protected float spreadSpeed;
    protected float BulletlifeTime=5f;
    protected Rigidbody bulletRigidbody;
    [Header("Gizmo")]
    [SerializeField]
    protected float sphereRadius;
    protected bool initialMove = false;
    protected bool moveToPlayer = false;
    protected Vector3 destination;

    private void Awake()
    {
        bulletRigidbody = GetComponent<Rigidbody>();
        bulletData = Resources.Load<EnemyBulletData>("BulletData/NormalBullet");
    }
    public void Initial()
    {
        initialMove = false;
        moveToPlayer = false;
        StartCoroutine(CountDownInactive(BulletlifeTime));
        moveToPlayer = true;
        speed = bulletData.speed;
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
        if (speed !=bulletData.speed)
        {
            speed = bulletData.speed;
           // bulletData.DataUpdate = false;
        }
        if (moveToPlayer)
        {
            transform.Translate(-Vector3.forward * speed);
        }
        //Debug.Log("Bullet"+ speed);
    }
    protected IEnumerator CountDownInactive(float lifeTime)
    {
        yield return new WaitForSeconds(lifeTime);
        gameObject.SetActive(false);
    }
    public void Speard()
    {
        destination = transform.position + new Vector3(Random.Range(-2f,2f), Random.Range(-2f,2f), Random.Range(-2f,2f));
        initialMove = true;
    }
}
