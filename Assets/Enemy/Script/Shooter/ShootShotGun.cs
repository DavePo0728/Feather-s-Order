using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootShotGun : MonoBehaviour
{
    [SerializeField]
    public float bulletAmount;
    GameObject bullet;
    public float shootingCoolDown = 1.5f;
    float shootCount;
    bool canShoot = false;
    public float MaxShootWave;
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
        canShoot = true;
    }
    
    void Update()
    {
        if (canShoot)
        {
            StartCoroutine(ShootRoutine());
            shootCount++;
        }
    }
    IEnumerator ShootRoutine()
    {
        canShoot = false;
        ShotGunMode(bulletAmount);
        yield return new WaitForSeconds(shootingCoolDown);// Wait for cooldown
        canShoot = true;// Enable shooting again
        //Debug.Log("ShootRoutine");
    }
    public void ShotGunMode(float amount)
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
                bulletMove.NoMoveInitial();
                bulletMove.Speard();
                //Debug.Log("ShotGunMode");
            }
        }
    }
}
