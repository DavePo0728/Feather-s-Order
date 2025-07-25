using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunShoot : MonoBehaviour
{
    [SerializeField]
    PlayerAim playerAim;
    GameObject lockedEnemy;
    [SerializeField]
    GameObject EmptyAimObject;
    PlayerBulletMove bulletMove;
	[SerializeField]
	ParticleSystem Flash;
	// Start is called before the first frame update
	void Start()
    {
        //Debug.Log(gameObject.name);
	}

    // Update is called once per frame
    void Update()
    {
        if (playerAim.aimInput.x != 0)
        {
            transform.LookAt(EmptyAimObject.transform);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }

    }
    public void Shoot()
    {
        GameObject bullet = BulletPool.poolInstance.GetPlayerPooledObject();
        if (bullet != null)
        {
			transform.GetChild(1).GetComponent<ParticleSystem>().Play();
			bullet.transform.position = transform.position;
            bullet.transform.rotation = transform.rotation;
            if (playerAim._lockedEnemy != null&& !playerAim._lockedEnemy.CompareTag("AimPoint"))
            {
                bulletMove = bullet.GetComponent<PlayerBulletMove>();
                bulletMove.SetLockedEnemy(playerAim._lockedEnemy);
            }         
            bullet.SetActive(true);
        }
    }
}
