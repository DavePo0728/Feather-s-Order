using DG.Tweening;
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

    [SerializeField]
    private List<GameObject> enemiesInRange = new List<GameObject>();

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
        if (other.CompareTag("Enemy"))
        {
            enemiesInRange.Add(other.gameObject);
            UpdateClosestEnemy();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemiesInRange.Remove(other.gameObject);
            if (other.gameObject == playerAim.lockedEnemy)
            {
                UpdateClosestEnemy();
            }
        }
    }
    private void UpdateClosestEnemy()
    {
        if (enemiesInRange.Count == 0)
        {
            Unlock();
            return;
        }

        GameObject closest = enemiesInRange[0];
        float closestDistance = Vector3.Distance(transform.position, closest.transform.position);

        foreach (GameObject enemy in enemiesInRange)
        {
            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist < closestDistance)
            {
                closest = enemy;
                closestDistance = dist;
            }
        }

        LockOntoEnemy(closest);
    }
    public void ClearAimList()
    {
        enemiesInRange.Clear();
    }
    private void LockOntoEnemy(GameObject enemy)
    {
        playerAim.lockedEnemy = enemy;
        playerAim.isLocked = true;
        playerAim.CallFarReactive();
        playerAim.CallNearReactive();
    }

    private void Unlock()
    {
        playerAim.lockedEnemy = AimObject;
        playerAim.isLocked = false;
    }
}
