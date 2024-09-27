using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [SerializeField]
    Wave wave;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void SpawnEnemy()
    {
        if(wave._spawnPointList.Count == 0)     //only one spawnPoint
        Instantiate(wave._enemyToSpawnList[0]._enemy, wave._spawnPointList[0].transform);

        if (wave._spawnPointList.Count == 1)    //two spawnPoint
        {

        }
    }
}
