using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimDetect : MonoBehaviour
{
    [SerializeField]
    PlayerAim playerAim;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy" && playerAim.lockedEnemy == this.gameObject)
        {
            playerAim.lockedEnemy = other.gameObject;
            playerAim.isLocked = true;
            //print("Locked");
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == playerAim.lockedEnemy)
        {
            playerAim.lockedEnemy = this.gameObject;
            playerAim.isLocked = false;
            //print("Unlocked");
        }
    }
}
