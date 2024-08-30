using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootShotGun : MonoBehaviour
{
    [SerializeField]
    int bulletAmount;
    GameObject bullet;

    float shootingCoolDown = 1.5f;
    bool canShoot = false;
    void Start()
    {
        canShoot = true;
    }
    IEnumerator ShootRoutine()
    {
        canShoot = false;
        ShotGunMode(bulletAmount);
        yield return new WaitForSeconds(shootingCoolDown);// Wait for cooldown
        canShoot = true;// Enable shooting again
    }
    void Update()
    {
        if (canShoot)
        {
            StartCoroutine(ShootRoutine());
        }
    }
    public void ShotGunMode(int amount)
    {
        for (int i = 0; i <= amount; i++)
        {
            bullet = BulletPool.poolInstance.GetEnemyPooledObject();
            if (bullet != null)
            {
                bullet.transform.position = transform.position;
                bullet.transform.rotation = transform.rotation;
                bullet.SetActive(true);
                EnemyBulletMove bulletMove = bullet.GetComponent<EnemyBulletMove>();
                bulletMove.NoMoveInitial();
                bulletMove.Speard();
            }
        }
    }
}
