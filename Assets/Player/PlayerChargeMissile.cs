using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerChargeMissile : MonoBehaviour
{
    public Animator ChargeEffect;
    public Transform shootposition;
    public bool canShoot;
    public void GetChargeShootInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            ChargeEffect.SetTrigger("Charge");
            ChargeEffect.SetBool("back", false);
        }
        if (context.canceled)
        {
            if (canShoot == true)
            {
                ChargeEffect.SetTrigger("shoot");
            }
            else
            {
                ChargeEffect.SetBool("back", true);
            }

        }
    }

    public void ChargeComplete()
    {
        canShoot = true;
        print(canShoot);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
