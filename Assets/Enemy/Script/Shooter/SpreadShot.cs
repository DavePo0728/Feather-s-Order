using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpreadShot : MonoBehaviour
{
    [SerializeField]
    int bulletAmount;
    GameObject bullet;

    float shootingCoolDown = 1.5f;
    float shootCount;
    bool canShoot = false;
    void Start()
    {
        canShoot = true;
    }

    void Update()
    {
        if (canShoot)
        {
            StartCoroutine(ShootRoutine());
        }
    }
    IEnumerator ShootRoutine()
    {
        canShoot = false;
        SpreadShotMode(bulletAmount);
        yield return new WaitForSeconds(shootingCoolDown);// Wait for cooldown
        canShoot = true;// Enable shooting again
        //Debug.Log("ShootRoutine");
    }
    IEnumerator ShootGoldBullet()
    {
        canShoot = false;
        //Instantiate(goldBullet, transform.position, transform.rotation);
        yield return new WaitForSeconds(shootingCoolDown);
        canShoot = true;
    }
    public void SpreadShotMode(int amount)
    {

        for (int i = 0; i <= amount; i++)
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
                //Debug.Log("ShotGunMode");
            }
        }
    }
}
