using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleShoot : MonoBehaviour
{

    GameObject bullet;
    [SerializeField]
    float bpm;
    private float timeBetweenShots;
    private float timeSinceLastShot = 0.0f;
    [SerializeField]
    //float bulletSpeed;

    // Start is called before the first frame update
    void Start()
    {
        timeBetweenShots = 1 / (bpm / 60.0f);
        //StartCoroutine(AimToPlayer());
    }
    public void SetGun(int rpm)
    {
        bpm = rpm;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        timeSinceLastShot += Time.deltaTime;
        if (timeSinceLastShot >= timeBetweenShots)
        {
            //ShootBlackBullet();
            ShootRedBullet();
        }
    }
    void ShootBlackBullet()
    {
        bullet = BulletPool.poolInstance.GetBlackBulletPooledObject();
        if (bullet != null)
        {
            bullet.transform.position = transform.position;
            bullet.transform.rotation = transform.rotation;
            bullet.SetActive(true);
            BlackBulletMove bulletMove = bullet.GetComponent<BlackBulletMove>();
            bulletMove.Initial();
            //bulletMove.speed = bulletSpeed;
            timeSinceLastShot = 0.0f;
        }
    }
    void ShootRedBullet()
    {
        bullet = BulletPool.poolInstance.GetRedBulletPooledObject();
        if (bullet != null)
        {
            bullet.transform.position = transform.position;
            bullet.transform.rotation = transform.rotation;
            bullet.SetActive(true);
            RedBulletMove bulletMove = bullet.GetComponent<RedBulletMove>();
            //bulletMove.speed = bulletSpeed;
            bulletMove.Initial();
            timeSinceLastShot = 0.0f;
        }
    }
    void ShootPurpleBullet()
    {
        bullet = BulletPool.poolInstance.GetPurpleBulletPooledObject();
        if (bullet != null)
        {
            bullet.transform.position = transform.position;
            bullet.transform.rotation = transform.rotation;
            bullet.SetActive(true);
            BlackBulletMove bulletMove = bullet.GetComponent<BlackBulletMove>();
            //bulletMove.speed = bulletSpeed;
            bulletMove.Initial();
            timeSinceLastShot = 0.0f;
        }
    }

}
