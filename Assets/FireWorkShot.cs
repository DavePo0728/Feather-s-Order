using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireWorkShot : MonoBehaviour
{
    [SerializeField]
    int bulletCount;
    GameObject bullet;

    float shootingCoolDown = 1.5f;
    [SerializeField]
    float offset;
    bool canShoot = false;
    [SerializeField]
    float range;
    // Start is called before the first frame update
    void Start()
    {
        canShoot = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (canShoot)
        {
            //StartCoroutine(ShootDandelion());
            StartCoroutine(ShootSphere());
        }
    }
    IEnumerator ShootDandelion()
    {
        canShoot = false;
        for (int i = 0; i < bulletCount; i++)
        {
            bullet = BulletPool.poolInstance.GetRedBulletPooledObject();
            if (bullet != null)
            {

                Vector3 direction = Random.onUnitSphere * range;
                bullet.transform.position = transform.position;
                bullet.transform.rotation = transform.rotation;
                bullet.SetActive(true);
                RedBulletMove bulletMove = bullet.GetComponent<RedBulletMove>();
                bulletMove.SphereSpread(direction);
                bulletMove.FireWorkInitial();
                //Debug.Log("ShotGunMode");
            }
        }
        yield return new WaitForSeconds(shootingCoolDown);
        canShoot = true;

    }
    IEnumerator ShootSphere()
    {
        canShoot = false;
        for (int i = 0; i < bulletCount; i++)
        {
            bullet = BulletPool.poolInstance.GetRedBulletPooledObject();
            if (bullet != null)
            {
                // 這邊用 (i + 0.5) 作為偏移，讓分佈更均勻
                float iFloat = i + offset;
                float theta = Mathf.Acos(1 - 2 * iFloat / bulletCount);          // 極角
                float phi = Mathf.PI * (1 + Mathf.Sqrt(5)) * iFloat;                // 方位角

                // 將球面座標轉換成 Cartesian 座標
                float x = Mathf.Sin(theta) * Mathf.Cos(phi);
                float y = Mathf.Sin(theta) * Mathf.Sin(phi);
                float z = Mathf.Cos(theta);

                Vector3 direction = new Vector3(x, y, z);
                bullet.transform.position = transform.position;
                bullet.transform.rotation = Quaternion.LookRotation(direction);
                bullet.SetActive(true);
                RedBulletMove bulletMove = bullet.GetComponent<RedBulletMove>();
                bulletMove.SphereSpread(direction);
                bulletMove.FireWorkInitial();
            }
        }
        yield return new WaitForSeconds (shootingCoolDown);
        canShoot = true;
    }
}
