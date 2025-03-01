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
    int shotCount = 0; // 計數器
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
        player = GameObject.FindGameObjectWithTag("Player");
        timeBetweenShots = 1 / (rpm / 60.0f);
        canShoot = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (canShoot)
        {
            timeSinceLastShot += Time.deltaTime;
            if (timeSinceLastShot >= timeBetweenShots)
            {
                AimPlayer();
                ShootBullet();
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
        yield return new WaitForSeconds(shootingCoolDown);
        canShoot = true;
    }

    void AimPlayer()
    {
        transform.LookAt(player.transform);
    }
}
