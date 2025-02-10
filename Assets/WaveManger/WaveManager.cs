using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [SerializeField]
    List<GameObject> enemyList;
    [SerializeField]
    [Space(10)]
    List<GameObject> _12SpawnPointList, _6SpawnPointList, _3SpawnPointList, _3VerticalSpawnPointList;
    [SerializeField]
    [Space(10)]
    List<GameObject> _12EndPointList, _6EndPointList, _3EndPointList, _3VerticalEndPointList;
    [SerializeField]
    List<GameObject> spawnTestList,endTestList;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WaveSpawn());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void SpawnEnemy(GameObject enemy,GameObject spawnPoint, GameObject endPoint,IEntryBehaviour entryBehaviour,IMoveBehaviour moveBehaviour ,float curveHeight)
    {
        GameObject temp = Instantiate(enemy, spawnPoint.transform.position, spawnPoint.transform.rotation);
        EnemyMove enemyMove = temp.GetComponent<EnemyMove>();
        enemyMove.endPoint = endPoint.transform;
        enemyMove.curveHeight = curveHeight;
        IEntryBehaviour entry = entryBehaviour;
        IMoveBehaviour move = moveBehaviour;
        enemyMove.SetBehaviours(entry,move);
    }    
    void SpawnEnemy(GameObject enemy,GameObject spawnPoint, GameObject endPoint,IEntryBehaviour entryBehaviour)
    {
        GameObject temp = Instantiate(enemy, spawnPoint.transform.position, spawnPoint.transform.rotation);
        EnemyMove move = temp.GetComponent<EnemyMove>();
        move.endPoint = endPoint.transform;
        IEntryBehaviour entry = entryBehaviour;
        //move.SetBehaviours(entry);
    }
    IEnumerator WaveSpawn()
    {
        SpawnEnemy(enemyList[0], spawnTestList[0], endTestList[0],new EntryTypeA(),new MoveTypeA(),Random.Range(-100,100));
        SpawnEnemy(enemyList[0], spawnTestList[1], endTestList[2],new EntryTypeA(),new MoveTypeA(), Random.Range(-100,100));
        SpawnEnemy(enemyList[0], spawnTestList[2], endTestList[1],new EntryTypeA(),new MoveTypeA(), Random.Range(-100,100));
        SpawnEnemy(enemyList[0], spawnTestList[3], endTestList[3],new EntryTypeA(),new MoveTypeA(), Random.Range(-100,100));

        yield return new WaitForSeconds(1f);
        //yield return new WaitForSeconds(3f);
        //SpawnEnemy(enemyList[0], _6SpawnPointList[0], _6EndPointList[0]);
        //SpawnEnemy(enemyList[1], _6SpawnPointList[1], _6EndPointList[1]);
        //SpawnEnemy(enemyList[0], _6SpawnPointList[2], _6EndPointList[2]);
        //SpawnEnemy(enemyList[0], _6SpawnPointList[3], _6EndPointList[3]);
        //SpawnEnemy(enemyList[1], _6SpawnPointList[4], _6EndPointList[4]);
        //SpawnEnemy(enemyList[0], _6SpawnPointList[5], _6EndPointList[5]);
        //yield return new WaitForSeconds(12f);
        //SpawnEnemy(enemyList[0], _6SpawnPointList[0], _6EndPointList[0]);
        //SpawnEnemy(enemyList[0], _6SpawnPointList[1], _6EndPointList[1]);
        //SpawnEnemy(enemyList[0], _6SpawnPointList[2], _6EndPointList[2]);
        //SpawnEnemy(enemyList[1], _6SpawnPointList[4], _6EndPointList[4]);
        //yield return new WaitForSeconds(12f);
        /////右邊垂直
        //SpawnEnemy(enemyList[0], _3VerticalSpawnPointList[3], _3VerticalEndPointList[6]);
        //SpawnEnemy(enemyList[1], _3VerticalSpawnPointList[4], _3VerticalEndPointList[7]);
        //SpawnEnemy(enemyList[0], _3VerticalSpawnPointList[5], _3VerticalEndPointList[8]);
        //yield return new WaitForSeconds(12f);
        //SpawnEnemy(enemyList[0], _6SpawnPointList[0], _6EndPointList[0]);
        //SpawnEnemy(enemyList[1], _6SpawnPointList[1], _6EndPointList[1]);
        //SpawnEnemy(enemyList[0], _6SpawnPointList[2], _6EndPointList[2]);
        //SpawnEnemy(enemyList[0], _6SpawnPointList[3], _6EndPointList[3]);
        //SpawnEnemy(enemyList[1], _6SpawnPointList[4], _6EndPointList[4]);
        //SpawnEnemy(enemyList[0], _6SpawnPointList[5], _6EndPointList[5]);
        //yield return new WaitForSeconds(12f);
        //SpawnEnemy(enemyList[1], _3VerticalSpawnPointList[4], _3VerticalEndPointList[7]);
        //SpawnEnemy(enemyList[1], _3VerticalSpawnPointList[1], _3VerticalEndPointList[1]);
        //yield return new WaitForSeconds(12f);
        //SpawnEnemy(enemyList[0], _3VerticalSpawnPointList[3], _3VerticalEndPointList[3]);
        //SpawnEnemy(enemyList[1], _3VerticalSpawnPointList[4], _3VerticalEndPointList[4]);    ///中間垂直
        //SpawnEnemy(enemyList[0], _3VerticalSpawnPointList[5], _3VerticalEndPointList[5]);
        //yield return new WaitForSeconds(12f);
        /////左邊垂直
        //SpawnEnemy(enemyList[0], _3VerticalSpawnPointList[0], _3VerticalEndPointList[0]);
        //SpawnEnemy(enemyList[1], _3VerticalSpawnPointList[1], _3VerticalEndPointList[1]);
        //SpawnEnemy(enemyList[0], _3VerticalSpawnPointList[2], _3VerticalEndPointList[2]);
        //yield return new WaitForSeconds(12f);
        //SpawnEnemy(enemyList[1], _6SpawnPointList[0], _6EndPointList[0]);
        //SpawnEnemy(enemyList[0], _6SpawnPointList[1], _6EndPointList[1]);
        //SpawnEnemy(enemyList[1], _6SpawnPointList[2], _6EndPointList[2]);
        //SpawnEnemy(enemyList[1], _6SpawnPointList[3], _6EndPointList[3]);
        //SpawnEnemy(enemyList[0], _6SpawnPointList[4], _6EndPointList[4]);
        //SpawnEnemy(enemyList[1], _6SpawnPointList[5], _6EndPointList[5]);
    }
}
