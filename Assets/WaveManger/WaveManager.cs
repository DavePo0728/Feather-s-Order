using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [SerializeField]
    List<GameObject> enemyList;
    [SerializeField]
    [Space(10)]
    List<GameObject> _12SpawnPointList, _6SpawnPointList, _3SpawnPointList;
    [SerializeField]
    [Space(10)]
    List<GameObject> _12EndPointList, _6EndPointList, _3EndPointList;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WaveSpawn());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void SpawnEnemy(GameObject enemy,GameObject spawnPoint, GameObject endPoint)
    {
            GameObject temp = Instantiate(enemy, spawnPoint.transform.position, spawnPoint.transform.rotation);
            EnemyMoveA move = temp.GetComponent<EnemyMoveA>();
            move.endPoint = endPoint;
    }
    IEnumerator WaveSpawn()
    {
            //Debug.Log(i);
        SpawnEnemy(enemyList[0], _6SpawnPointList[0], _6EndPointList[0]);
        SpawnEnemy(enemyList[0], _6SpawnPointList[1], _6EndPointList[1]);
        SpawnEnemy(enemyList[0], _6SpawnPointList[2], _6EndPointList[2]);
        SpawnEnemy(enemyList[0], _6SpawnPointList[3], _6EndPointList[3]);
        SpawnEnemy(enemyList[0], _6SpawnPointList[4], _6EndPointList[4]);
        SpawnEnemy(enemyList[0], _6SpawnPointList[5], _6EndPointList[5]);
        yield return new WaitForSeconds(8f);
        SpawnEnemy(enemyList[0], _6SpawnPointList[0], _6EndPointList[0]);
        SpawnEnemy(enemyList[0], _6SpawnPointList[1], _6EndPointList[1]);
        SpawnEnemy(enemyList[0], _6SpawnPointList[2], _6EndPointList[2]);
        SpawnEnemy(enemyList[0], _6SpawnPointList[3], _6EndPointList[3]);
        SpawnEnemy(enemyList[0], _6SpawnPointList[4], _6EndPointList[4]);
        SpawnEnemy(enemyList[0], _6SpawnPointList[5], _6EndPointList[5]);
        yield return new WaitForSeconds(8f);
        SpawnEnemy(enemyList[0], _6SpawnPointList[0], _6EndPointList[0]);
        SpawnEnemy(enemyList[1], _6SpawnPointList[1], _6EndPointList[1]);
        SpawnEnemy(enemyList[0], _6SpawnPointList[2], _6EndPointList[2]);
        SpawnEnemy(enemyList[0], _6SpawnPointList[3], _6EndPointList[3]);
        SpawnEnemy(enemyList[0], _6SpawnPointList[4], _6EndPointList[4]);
        SpawnEnemy(enemyList[0], _6SpawnPointList[5], _6EndPointList[5]);
        yield return new WaitForSeconds(8f);
        SpawnEnemy(enemyList[0], _6SpawnPointList[0], _6EndPointList[0]);
        SpawnEnemy(enemyList[0], _6SpawnPointList[1], _6EndPointList[1]);
        SpawnEnemy(enemyList[0], _6SpawnPointList[2], _6EndPointList[2]);
        SpawnEnemy(enemyList[0], _6SpawnPointList[3], _6EndPointList[3]);
        SpawnEnemy(enemyList[1], _6SpawnPointList[4], _6EndPointList[4]);
        SpawnEnemy(enemyList[0], _6SpawnPointList[5], _6EndPointList[5]);
        yield return new WaitForSeconds(8f);
        SpawnEnemy(enemyList[0], _3SpawnPointList[0], _3EndPointList[0]);
        SpawnEnemy(enemyList[0], _3SpawnPointList[1], _3EndPointList[1]);
        SpawnEnemy(enemyList[0], _3SpawnPointList[2], _3EndPointList[2]);
        yield return new WaitForSeconds(8f);
        SpawnEnemy(enemyList[0], _3SpawnPointList[0], _3EndPointList[0]);
        SpawnEnemy(enemyList[0], _3SpawnPointList[1], _3EndPointList[1]);
        SpawnEnemy(enemyList[0], _3SpawnPointList[2], _3EndPointList[2]);
        yield return new WaitForSeconds(8f);
        SpawnEnemy(enemyList[0], _12SpawnPointList[0], _12EndPointList[0]);
        SpawnEnemy(enemyList[0], _12SpawnPointList[1], _12EndPointList[1]);
        SpawnEnemy(enemyList[0], _12SpawnPointList[2], _12EndPointList[2]);
        SpawnEnemy(enemyList[0], _12SpawnPointList[3], _12EndPointList[3]);
        SpawnEnemy(enemyList[0], _12SpawnPointList[4], _12EndPointList[4]);
        SpawnEnemy(enemyList[0], _12SpawnPointList[5], _12EndPointList[5]);
        SpawnEnemy(enemyList[0], _12SpawnPointList[6], _12EndPointList[6]);
        SpawnEnemy(enemyList[0], _12SpawnPointList[7], _12EndPointList[7]);
        SpawnEnemy(enemyList[0], _12SpawnPointList[8], _12EndPointList[8]);
        SpawnEnemy(enemyList[0], _12SpawnPointList[9], _12EndPointList[9]);
        SpawnEnemy(enemyList[0], _12SpawnPointList[10], _12EndPointList[10]);
        SpawnEnemy(enemyList[0], _12SpawnPointList[11], _12EndPointList[11]);
        SpawnEnemy(enemyList[0], _3SpawnPointList[0], _3EndPointList[0]);
        SpawnEnemy(enemyList[1], _3SpawnPointList[1], _3EndPointList[1]);
        SpawnEnemy(enemyList[0], _3SpawnPointList[2], _3EndPointList[2]);
        yield return new WaitForSeconds(8f);
        SpawnEnemy(enemyList[0], _6SpawnPointList[0], _6EndPointList[0]);
        SpawnEnemy(enemyList[0], _6SpawnPointList[1], _6EndPointList[1]);
        SpawnEnemy(enemyList[0], _6SpawnPointList[2], _6EndPointList[2]);
        SpawnEnemy(enemyList[0], _6SpawnPointList[3], _6EndPointList[3]);
        SpawnEnemy(enemyList[1], _6SpawnPointList[4], _6EndPointList[4]);
        SpawnEnemy(enemyList[0], _6SpawnPointList[5], _6EndPointList[5]);
    }
}
