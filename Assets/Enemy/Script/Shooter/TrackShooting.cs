using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackShooting : MonoBehaviour
{
    [SerializeField]
    GameObject player;
    GameObject bullet;
    bool canShoot = false;
    public float shootingCoolDown; 
    private float timeBetweenShots;
    private float timeSinceLastShot = 0.0f;
    public float rpm;
    public float maxShots; 
    int shotCount = 0; // 璸计竟
    float shootWaveCount=0;
    public float maxShootWave;
    public enum BulletType
    {
        Black,
        Red,
        Purple,
        BlackRed,
    }
    public BulletType bulletType;
    // Start is called before the first frame update
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        timeBetweenShots = 1 / (rpm / 60.0f);
        canShoot = true;
        AimPlayer();
    }
    void OnEnable()
    {
        
        if (canShoot == false)
            canShoot = true;
    }
    // Update is called once per frame
    void Update()
    {
        //Debug.Log(canShoot && shootWaveCount < MaxShootWave);
        //if (canShoot&&shootWaveCount<maxShootWave)
        //{
        //    timeSinceLastShot += Time.deltaTime;
        //    if (timeSinceLastShot >= timeBetweenShots)
        //    {
                AimPlayer();
        //        switch (bulletType)
        //        {
        //            case BulletType.Black:
        //                ShootBlackBullet();
        //                break;
        //            case BulletType.Red:
        //                ShootRedBullet();
        //                break;
        //            case BulletType.Purple:
        //                ShootPurpleBullet();
        //                break;
        //            case BulletType.BlackRed:
        //                //ShootBlackRedBullet();
        //                break;
        //        }
                
        //    }
            
        //}
    }
    //void ShootBlackRedBullet()
    //{
    //    bullet = Instantiate(blackRedBulletPrefab, transform.position, transform.rotation);
    //}
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
            timeSinceLastShot = 0.0f;
            shotCount++; // 糤璸计竟
            if (shotCount >= maxShots)
            {
                StartCoroutine(ShootRoutine());
            }
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
            bulletMove.Initial();
            timeSinceLastShot = 0.0f;
            shotCount++; // 糤璸计竟
            if (shotCount >= maxShots)
            {
                StartCoroutine(ShootRoutine());
            }
        }

    }
    void ShootBullet()
    {
        bullet = BulletPool.poolInstance.GetBlackBulletPooledObject();
        if (bullet != null)
        {
            bullet.transform.position = transform.position;
            bullet.transform.rotation = transform.rotation;

            bullet.SetActive(true);
            BlackBulletMove bulletMove = bullet.GetComponent<BlackBulletMove>();
            bulletMove.Initial();
            timeSinceLastShot = 0.0f;

            shotCount++; // 糤璸计竟
            if (shotCount >= maxShots)
            {
                StartCoroutine(ShootRoutine());
            }
        }
    }

    IEnumerator ShootRoutine()
    {
        canShoot = false;
        shotCount = 0; // 竚璸计竟
        shootWaveCount++;
        yield return new WaitForSeconds(shootingCoolDown);
        canShoot = true;
    }

    void AimPlayer()
    {
        transform.LookAt(player.transform);
    }
}
