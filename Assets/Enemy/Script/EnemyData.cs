using PathCreation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New EnemyData", menuName = "EnemyData/New EnemyData")]
public class EnemyData : ScriptableObject
{
    public data data;
}
public enum BulletType
{
    Black,
    Red,
    Purple,
    BlackRed,
}
[System.Serializable]
    public class data
{
    [Header("Basic Data")]
    public GameObject enemy;
    public float hp;
    public bool haveShield;
    public float shieldHp;
    public bool corrupted;
    [Header("Spawn Data")]
    public Vector3 spawnPosition;
    public Vector3 endPosition;
    public Vector3 LeavePositon;
    public float curveHeight;
    public int gunIndex;
    public float rpm;
    public float shootingCoolDown;
    public float bulletAmount;
    public float spinSpeed;
    public BulletType bulletType;
    public float MaxShootWave;
    [Header("typeB")]
    public PathCreator CurvePath;
    [Header("typeC")]
    public int pathListLength;
    public float pointWaitTime;
    public int pathListIndex;
}