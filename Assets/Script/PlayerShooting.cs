using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField]
    PlayerSlashAttack playerSlashAttack;
    [SerializeField]
    GunShoot gunPoint1, gunPoint2;
    [SerializeField]
    SpriteRenderer gunPoint1Img, gunPoint2Img;
    private float timeBetweenShots;
    private float timeSinceLastShot = 0.0f;
    [SerializeField]
    AudioSource gunSound;
    bool shooting;
    float timeSinceLastShooting = 0f;   
    void Start()
    {
        timeBetweenShots = 1 / (800 / 60.0f);
        gunPoint1Img.enabled = false;
        gunPoint2Img.enabled = false;
        gunSound = GetComponent<AudioSource>();
    }

    public void GetShootInput(InputAction.CallbackContext context)
    {
        if (context.performed&&playerSlashAttack.slashState == PlayerSlashAttack.SlashState.Idle)
        {
            StartShooting();
        }
        if (context.canceled)
        {
            StopShooting();
        }
    }
    public void StartShooting()
    {
        shooting = true;
        gunPoint1Img.enabled = true;
        gunPoint2Img.enabled = true;
        
    }
    public void StopShooting()
    {
        shooting = false;
        gunPoint1Img.enabled = false;
        gunPoint2Img.enabled = false;
        gunSound.Stop();
    }
    void FixedUpdate()
    {
        timeSinceLastShot += Time.deltaTime;
        //Debug.Log("Shooting: "+shooting);
        if (shooting && timeSinceLastShot >= timeBetweenShots)
        {
            if (playerSlashAttack != null)
            {
                if (playerSlashAttack.slashState == PlayerSlashAttack.SlashState.Dashing 
                    || playerSlashAttack.slashState == PlayerSlashAttack.SlashState.Arrived 
                    || playerSlashAttack.slashState == PlayerSlashAttack.SlashState.Attacking)
                {
                    //Debug.Log("StopShooting");
                    StopShooting();
                    return;
                }
            }
            gunSound.Play();
            gunPoint1.Shoot();
            gunPoint2.Shoot();
            timeSinceLastShot = 0.0f;
        }
    }
}
