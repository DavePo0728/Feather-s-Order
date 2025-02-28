using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerChargeShooting : MonoBehaviour
{
    //[SerializeField]
    //float maxChargeTime;
    [SerializeField]
    GameObject ChargeBullet;
    //bool chargeShooting;
    public bool CanShoot;
    bool manualLean = false;
    public Animator ChargeEffect;
    public Transform shootposition;
    // Start is called before the first frame update

    public void GetChargeShootInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            print("«ö¤U"); 
            ChargeEffect.SetTrigger("Charge");
			ChargeEffect.SetBool("back", false);
            //print(123);

		}
        if (context.canceled)
        {
			if (CanShoot == true)
			{


				ChargeEffect.SetTrigger("shoot");
				

			}
		    else
            {
				ChargeEffect.SetBool("back", true);
			}

        }
    }

    public void GetAimLean(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            manualLean = true;
        }
        if (context.canceled)
        {
            manualLean = false;
        }
    }
    public void ChargeComplete()
    {
        CanShoot = true;
        print(CanShoot);
	}
    private void FixedUpdate()
    {
        transform.LookAt(shootposition);
    }
    public void shoot()
	{
	    Instantiate(ChargeBullet, transform.position, transform.rotation);
		CanShoot = false;
	}
}
