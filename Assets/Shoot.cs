using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    private float timeBetweenShots;
    private float timeSinceLastShot = 0.0f;
    GameObject bullet;



    // Start is called before the first frame update
    void Start()
    {
        timeBetweenShots = 1 / (700 / 60.0f);
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceLastShot += Time.deltaTime;
    }

    public void ShootBullet()
    {
        bullet = BulletPool.poolInstance.GetEnemyPooledObject();
        if (timeSinceLastShot >= timeBetweenShots)
        {
            if (bullet != null)
            {
                bullet.transform.position = transform.position;
                bullet.transform.rotation = transform.rotation;
                bullet.SetActive(true);
                EnemyBulletMove bulletMove = bullet.GetComponent<EnemyBulletMove>();
                bulletMove.Initial();
            }
            timeSinceLastShot = 0.0f;
        }
    }
    public void ShootBreakableBullet()
    {
        bullet = BulletPool.poolInstance.GetEnemyBreakablePooledObject();
        if (timeSinceLastShot >= timeBetweenShots)
        {
            if (bullet != null)
            {
                bullet.transform.position = transform.position;
                bullet.transform.rotation = transform.rotation;
                bullet.SetActive(true);
                EnemyBulletMove bulletMove = bullet.GetComponent<EnemyBulletMove>();
                bulletMove.Initial();
            }
            timeSinceLastShot = 0.0f;
        }
    }
}
