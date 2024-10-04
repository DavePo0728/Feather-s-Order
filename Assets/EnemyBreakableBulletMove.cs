using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBreakableBulletMove : EnemyBulletMove
{
    private void Awake()
    {
        bulletRigidbody = GetComponent<Rigidbody>();
        bulletData = Resources.Load<EnemyBulletData>("BulletData/NormalBullet");
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
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
        if (speed != bulletData.speed)
        {
            speed = bulletData.speed;
            // bulletData.DataUpdate = false;
        }
        if (moveToPlayer)
        {
            transform.Translate(-Vector3.forward * speed);
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
}
