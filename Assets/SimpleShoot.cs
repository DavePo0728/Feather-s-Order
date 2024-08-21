using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleShoot : MonoBehaviour
{
    [SerializeField]
    GameObject shootingPointPrefab;
    [SerializeField]
    float radius;
    [SerializeField]
    int objectToSpawnAmount;
    private float timeBetweenShots;
    private float timeSinceLastShot = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        timeBetweenShots = 1 / (700 / 60.0f);
        SpawnShootingPoint();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void SpawnShootingPoint()
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
    }
}
