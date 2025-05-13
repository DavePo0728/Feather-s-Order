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
    // Start is called before the first frame update
    void Start()
    {
        canShoot = true;
    }
    void OnEnable()
    {
        if (canShoot == false)
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
    void ShootHomingPurpleBullet(GameObject point)
    {
        bullet = BulletPool.poolInstance.GetPurpleBulletPooledObject();
        if (bullet != null)
        {
            bullet.transform.position = transform.position;
            bullet.transform.rotation = transform.rotation;
            bullet.SetActive(true);
            //HighSpeedVioletBulletMove purpleBulletMove = bullet.GetComponent<HighSpeedVioletBulletMove>();
            //purpleBulletMove.homingTime = 1f;
            //purpleBulletMove.SetHomingShooterTransform(transform);
            //purpleBulletMove.HomingInitial(point); 
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
                ShootHomingPurpleBullet(pointList[i]);
            }
        }
        shootWaveCount++;
        yield return new WaitForSeconds(shootingCoolDown);// Wait for cooldown
        canShoot = true;
    }
}
