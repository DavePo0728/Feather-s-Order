using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New EnemyData", menuName = "EnemyData/New EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("Basic Data")]
    public GameObject enemy;
    public float hp;
    public bool haveShield;
    public float shieldHp;
    [Header("Spawn Data")]
    public Vector3 spawnPosition;
    public Vector3 endPosition;
    public float speed;
}
