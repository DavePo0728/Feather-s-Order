using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleShoot : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    GameObject bullet;
    GameObject blackRedBulletPrefab;
    public float rpm;
    public float shootingCoolDown;
    private float timeBetweenShots;
    private float timeSinceLastShot = 0.0f;
    public float maxShots;
    int shotCount = 0;
    public bool canShoot = false;
    float shootWaveCount = 0;
    public float maxShootWave;
    public enum BulletType
    {
        Black,
        Red,
        Purple,
        BlackRed,
    }
    public BulletType bulletType;
    [SerializeField]
    //float bulletSpeed;

    // Start is called before the first frame update
    void Start()
    {
        timeBetweenShots = 1 / (rpm / 60.0f);
        blackRedBulletPrefab = Resources.Load<GameObject>("Prefabs/Enemy/Bullet/BlackRedBullet");
        canShoot = true;
        //StartCoroutine(AimToPlayer());
    }
    public void SetGun(int rpm)
    {
        this.rpm = rpm;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (canShoot && shootWaveCount < maxShootWave)
        {
            timeSinceLastShot += Time.deltaTime;
            if (timeSinceLastShot >= timeBetweenShots)
            {
                switch (bulletType)
                {
                    case BulletType.Black:
                        ShootBlackBullet();
                        break;
                    case BulletType.Red:
                        ShootRedBullet();
                        break;
                    case BulletType.Purple:
                        ShootPurpleBullet();
                        break;
                    case BulletType.BlackRed:
                        ShootBlackRedBullet();
                        break;
                }
            }
        }
    }
    void ShootBlackRedBullet()
    {
        bullet = Instantiate(blackRedBulletPrefab,transform.position,transform.rotation);
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
            timeSinceLastShot = 0.0f;
            shotCount++; // 增加計數器
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
            bulletMove.playerTransform = playerTransform; //  設定玩家位置
            bulletMove.Initial();

            timeSinceLastShot = 0.0f;
            shotCount++;
            if (shotCount >= maxShots)
            {
                StartCoroutine(ShootRoutine());
            }
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
            bulletMove.Initial();
            timeSinceLastShot = 0.0f;
            shotCount++; // 增加計數器
            if (shotCount >= maxShots)
            {
                StartCoroutine(ShootRoutine());
            }
        }
    }
    IEnumerator ShootRoutine()
    {
        canShoot = false;
        shotCount = 0; // 重置計數器
        shootWaveCount++;
        yield return new WaitForSeconds(shootingCoolDown);
        canShoot = true;
    }
}
