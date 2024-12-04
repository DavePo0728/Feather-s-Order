using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer gunPoint1, gunPoint2,gunPoint3;
    private float timeBetweenShots;
    private float timeSinceLastShot = 0.0f;
    [SerializeField]
    AudioSource gunSound;
    bool shooting;
    [SerializeField]
    PlayerAim playerAim;
    GameObject lockedEnemy;
    PlayerBulletMove bulletMove;
    // Start is called before the first frame update
    void Start()
    {
        timeBetweenShots = 1 / (800 / 60.0f);
        gunPoint1.enabled = false;
        gunPoint2.enabled = false;
        gunSound = transform.parent.parent.parent.GetComponent<AudioSource>();
        
        //playerAim = GetComponent<PlayerAim>();
    }
    public void GetShootInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            shooting = true;
            gunPoint1.enabled = true;
            gunPoint2.enabled = true;
            gunSound.Play();
        }
        if (context.canceled)
        {
            shooting = false;
            gunPoint1.enabled = false;
            gunPoint2.enabled = false;
            gunSound.Stop();
        }
    }
    
    // Update is called once per frame
    void FixedUpdate()
    {
        timeSinceLastShot += Time.deltaTime;
        if ( shooting && timeSinceLastShot >= timeBetweenShots)
        {
            GameObject bullet = BulletPool.poolInstance.GetPlayerPooledObject();
            if (bullet != null)
            {
                bullet.transform.position = transform.position;
                bullet.transform.rotation = transform.rotation;
                if (playerAim._lockedEnemy != null)
                {
                    bulletMove = bullet.GetComponent<PlayerBulletMove>();
                    bulletMove.SetLockedEnemy(playerAim._lockedEnemy);
                    //Debug.Log(playerAim._lockedEnemy.name);
                }
                bullet.SetActive(true);
            }
            timeSinceLastShot = 0.0f;
        }

    }
}
