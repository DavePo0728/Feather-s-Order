using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSlash : MonoBehaviour
{
    [SerializeField]
    GameObject slashBullet;
    [SerializeField]
    Transform shootposition;
    [SerializeField]
    Animator ChargeEffect;
    bool canShoot;
    bool startCharge;

    [SerializeField]
    float maxChargeTime;
    float chargeTimer;
    [SerializeField]
    BulletGraze bulletGraze;
    public float slashCost;

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
            if (bulletGraze.CheckGrazeEnergy(slashCost))
            {
                startCharge = true;
                ChargeEffect.SetTrigger("Charge");
            }
        }
        if (context.canceled)
        {
            if (startCharge&&chargeTimer <= maxChargeTime)
            {
                ChargeEffect.SetTrigger("Return");
                startCharge = false;
            }
            if (canShoot == true)
            {
                shoot();
                ChargeEffect.SetTrigger("Shoot");
                chargeTimer = 0;
                canShoot = false;
                startCharge = false;
            }
        }
    }
    public void shoot()
	{
        bulletGraze.UpdateGrazeEnergyOutside(slashCost);
        Instantiate(slashBullet, transform.position, transform.rotation);
		canShoot = false;
	}
}
