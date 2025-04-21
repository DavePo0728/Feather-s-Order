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
    public float entryTime;
    public float singleMoveTime;
    public float leaveTime;
    public float hp;
    public float lifeTime;
    public bool haveShield;
    public float shieldHp;
    public bool corrupted;
    public float corruptionStack;
    public float paralyzeTime;
}