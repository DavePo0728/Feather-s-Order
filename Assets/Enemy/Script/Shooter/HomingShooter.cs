using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingShooter : MonoBehaviour
{
    [SerializeField]
    public int bulletAmount;
    public float shootingCoolDown;
    bool canShoot = false;
    float shootWaveCount = 0;
    public float MaxShootWave;
    GameObject bullet;
    [SerializeField]
    List<GameObject> pointList = new List<GameObject>();
    public enum BulletType
    {
        Black,
        Red,
        Purple,
        BlackRed,
    }
    public BulletType bulletType;
    // Start is called before the first frame update
    void Start()
    {
        canShoot = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (canShoot&&shootWaveCount<MaxShootWave)
        {
            StartCoroutine(HomingShot(bulletAmount));
        }
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
            bulletMove.HomingInitial();
            
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
            bulletMove.HomingInitial();
            
        }

    }
    void ShootHomingPurpleBullet(GameObject point)
    {
        bullet = BulletPool.poolInstance.GetPurpleBulletPooledObject();
        if (bullet != null)
        {
            bullet.transform.position = transform.position;
            bullet.transform.rotation = transform.rotation;
            bullet.SetActive(true);
            HighSpeedVioletBulletMove purpleBulletMove = bullet.GetComponent<HighSpeedVioletBulletMove>();
            purpleBulletMove.homingTime = 1f;
            purpleBulletMove.SetHomingShooterTransform(transform);
            purpleBulletMove.HomingInitial(point); 
        }
    }
    IEnumerator HomingShot(int bulletAmount)
    {
        for (int i = 0; i < bulletAmount; i++)
        {
            canShoot = false;
            GameObject bullet = BulletPool.poolInstance.GetRedBulletPooledObject();
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
                        ShootHomingPurpleBullet(pointList[i]);
                        break;
                    case BulletType.BlackRed:
                        //ShootBlackRedBullet();
                        break;
                }
            }
        }
        shootWaveCount++;
        yield return new WaitForSeconds(shootingCoolDown);// Wait for cooldown
        canShoot = true;
    }
}
