using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimDetect : MonoBehaviour
{
    [SerializeField]
    PlayerAim playerAim;
    [SerializeField]
    GameObject AimObject;
    private void Awake()
    {
        playerAim.lockedEnemy = AimObject;
    }
    private void Update()
    {
        transform.LookAt(AimObject.transform);
    }
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.gameObject.tag);
        if (other.gameObject.tag == "Enemy")
        {
            playerAim.lockedEnemy = other.gameObject;
            playerAim.isLocked = true;
            playerAim.CallFarReactive();
            playerAim.CallNearReactive();
            //print("Locked");
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == playerAim.lockedEnemy)
        {
            playerAim.lockedEnemy = AimObject;
            playerAim.isLocked = false;
            //print("Unlocked");
        }
    }
}
