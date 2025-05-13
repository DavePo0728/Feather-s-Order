using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpreadShot : MonoBehaviour
{
    [SerializeField]
    GameObject player;
    public float bulletAmount;
    GameObject bullet;

    public float shootingCoolDown;
    float shootWaveCount = 0;
    public float MaxShootWave;
    //float shootCount;
    bool canShoot = false;
    public enum BulletType
    {
        Black,
        Red,
        Purple,
        BlackRed,
    }
    public BulletType bulletType;
    void OnEnable()
    {
        if (canShoot == false)
            canShoot = true;
    }
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        canShoot = true;
    }

    void Update()
    {
        AimPlayer();
        if (canShoot && shootWaveCount < MaxShootWave)
        {
            StartCoroutine(ShootRoutine());
        }
    }
    IEnumerator ShootRoutine()
    {
        canShoot = false;
        SpreadShotMode(bulletAmount);
        MaxShootWave++;
        yield return new WaitForSeconds(shootingCoolDown);// Wait for cooldown
        canShoot = true;// Enable shooting again
                        //Debug.Log("ShootRoutine");
    }
    void ShootBlackBullet()
    {
        bullet = BulletPool.poolInstance.GetBlackBulletPooledObject();
        if (bullet != null)
        {
            bullet.transform.position = transform.position;
            bullet.transform.rotation = transform.rotation;
            //Debug.Log("rotation:" + transform.localRotation.x + "BulletRotation:" + bullet.transform.rotation);
            bullet.SetActive(true);
            BlackBulletMove bulletMove = bullet.GetComponent<BlackBulletMove>();
            bulletMove.FlatSpeard();
            bulletMove.Initial();
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
            bulletMove.FlatSpeard();
            bulletMove.Initial();
        }

    }
    public void SpreadShotMode(float amount)
    {
        for (int i = 0; i <= amount; i++)
        {
            bullet = BulletPool.poolInstance.GetRedBulletPooledObject();
            if (bullet != null)
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
                        //ShootPurpleBullet();
                        break;
                    case BulletType.BlackRed:
                        //ShootBlackRedBullet();
                        break;
                }
            }
        }
    }
    void AimPlayer()
    {
        transform.LookAt(player.transform);
    }
}
