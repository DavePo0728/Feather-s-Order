using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New EnemyDataList", menuName = "EnemyData/New EnemyDataList")]

public class EnemyDataList: ScriptableObject
{
   public EnemyData[] enemyDatas;
}
