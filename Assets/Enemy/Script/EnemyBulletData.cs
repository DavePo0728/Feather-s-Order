using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New BulletData", menuName = "BulletData/New BulletData")]
public class EnemyBulletData : ScriptableObject
{
    public float speed;
    public float lifeTime;
}
