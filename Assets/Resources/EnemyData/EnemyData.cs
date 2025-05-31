using PathCreation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New EnemyData", menuName = "EnemyData/New EnemyData")]
public class EnemyData : ScriptableObject
{
    public data data;
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
    [Tooltip("污穢值")]
    public float corruptionMaxValue;
    [Tooltip("束縛初始時間")]
    public float paralyzeTime;
    [Tooltip("束縛最大時間")]
    public float paralyzeMaxTime;
    [Tooltip("束縛時間增加量")]
    public float paralyzeAddTime;
    [Tooltip("束縛時間增加最大次數")]
    public int paralyzeMaxCount;
    [Tooltip("束縛時間增加係數")]
    public float paralyzeTimeStackMultiplier;
    [Tooltip("未攻擊污穢消退時間")]
    public float corruptionDecreaseTime;
    [Tooltip("未攻擊污穢消退速度")]
    public float corruptionDecreaseSpeed;
}