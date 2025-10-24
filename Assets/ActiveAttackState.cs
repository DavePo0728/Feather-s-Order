using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveAttackState : MonoBehaviour
{
    [SerializeField]
    PlayerSlashAttack playerSlashAttack;
    public void EnterState()
    {
        playerSlashAttack.SetAttack();
    }
}
