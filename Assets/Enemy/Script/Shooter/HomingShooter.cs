using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingShooter : MonoBehaviour
{
    [SerializeField]
    public int bulletAmount;
    public float shootingCoolDown;
    bool canShoot = false;
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
        if (canShoot)
        {
            StartCoroutine(HomingShot(bulletAmount));
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
                bullet.transform.position = transform.position;
                bullet.transform.rotation = transform.rotation;
                bullet.SetActive(true);
                RedBulletMove bulletMove = bullet.GetComponent<RedBulletMove>();
                bulletMove.homing = true;
                bulletMove.HomingInitial();
                bulletMove.Speard(); 
            }
        }
        yield return new WaitForSeconds(shootingCoolDown);// Wait for cooldown
        canShoot = true;
    }
}
