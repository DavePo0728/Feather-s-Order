using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCleanseShot : MonoBehaviour
{
    [SerializeField]
    GameObject cleanseBullet;
    [SerializeField]
    Transform shootposition;
    [SerializeField]
    Animator chargeEffect;
    bool canShoot;
    bool startCharge;

    [SerializeField]
    float maxChargeTime;
    float chargeTimer;
    [SerializeField]
    BulletGraze bulletGraze;
    public float shootCost;

    private void Awake()
    {
        canShoot = false;
        chargeTimer = 0;
    }
    private void FixedUpdate()
    {
        if(startCharge)
        {
            chargeTimer += Time.deltaTime;
            //Debug.Log(chargeTimer);
        }
        else
        {
            
            chargeTimer = 0;
        }
        if (chargeTimer >= maxChargeTime)
        {
            canShoot = true;
        }
        transform.LookAt(shootposition);
    }
    
    public void GetSlashInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (bulletGraze.CheckGrazeEnergy(shootCost))
            {
                startCharge = true;
                chargeEffect.SetTrigger("Charge");
            }
        }
        if (context.canceled)
        {
            if (startCharge&&chargeTimer <= maxChargeTime)
            {
                chargeEffect.SetTrigger("Return");
                startCharge = false;
            }
            if (canShoot == true)
            {
                shoot();
                chargeEffect.SetTrigger("Shoot");
                chargeTimer = 0;
                canShoot = false;
                startCharge = false;
            }
        }
    }
    public void shoot()
	{
        bulletGraze.UpdateGrazeEnergyOutside(shootCost);
        Instantiate(cleanseBullet, transform.position, transform.rotation);
		canShoot = false;
	}
}
