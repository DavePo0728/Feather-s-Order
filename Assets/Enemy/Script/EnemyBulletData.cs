using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New BulletData", menuName = "BulletData/New BulletData")]
public class EnemyBulletData : ScriptableObject
{
    public float speed;
    public bool DataUpdate;
    private EnemyBulletData()
    {
        DataUpdate = false;
    }
    // Start is called before the first frame update
}
