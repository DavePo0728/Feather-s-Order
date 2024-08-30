using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SimpleShoot : MonoBehaviour
{
    [SerializeField]
    shootingType shooting_Type;
    [SerializeField] 
    float fps;
    [SerializeField]
    GameObject shootingPointPrefab;
    [SerializeField]
    float radius;
    [SerializeField]
    int objectToSpawnAmount;
    private float timeBetweenShots;
    private float timeSinceLastShot = 0.0f;
    private enum shootingType { shotGun,hRound,vRound}
    // Start is called before the first frame update
    void Start()
    {
        timeBetweenShots = 1 / (fps / 60.0f);
        switch (shooting_Type)
        {
            case shootingType.shotGun:
                ShotGunSpawn();
                break;
            case shootingType.hRound:
                SpawnHorizontalRoundShootingPoint();
                break;
            case shootingType.vRound:
                SpawnVerticalRoundShootingPoint();
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void ShotGunSpawn()
    {
        Instantiate(shootingPointPrefab,transform);
    }
    void SpawnHorizontalRoundShootingPoint()
    {
        float angle = 360 / objectToSpawnAmount;
        for (int i = 0; i <= objectToSpawnAmount; i++)
        {
            float radians = (angle * i) * Mathf.Deg2Rad;
            float x = transform.position.x + Mathf.Cos(radians) * radius;
            float z = transform.position.z + Mathf.Sin(radians) * radius;
            Vector3 spawnPosition = new Vector3(x, transform.position.y, z);
            Instantiate(shootingPointPrefab, spawnPosition, Quaternion.Euler(0,0,0), transform);
        }
    }void SpawnVerticalRoundShootingPoint()
    {
        float angle = 360 / objectToSpawnAmount;
        for (int i = 0; i <= objectToSpawnAmount; i++)
        {
            float radians = (angle * i) * Mathf.Deg2Rad;
            float x = transform.position.x + Mathf.Cos(radians) * radius;
            float y = transform.position.y + Mathf.Sin(radians) * radius;
            Vector3 spawnPosition = new Vector3(x,y, transform.position.z);
            Instantiate(shootingPointPrefab, spawnPosition, Quaternion.Euler(0,0,0), transform);
        }
    }
}
