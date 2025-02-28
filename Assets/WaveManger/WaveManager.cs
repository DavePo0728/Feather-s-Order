using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using PathCreation;

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
    List<GameObject> spawnTestList,endTestList,LeaveTestList;
    [SerializeField]
    List<PathCreator> pathList;
    [SerializeField]
    List<GameObject> gunList;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WaveSpawn());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnEnemy(GameObject enemy,GameObject spawnPoint, GameObject endPoint,GameObject leavePoint, GameObject gun, IEntryBehaviour entryBehaviour,IMoveBehaviour moveBehaviour , ILeaveBehaviour leaveBehaviour,float curveHeight)
    {
        GameObject temp = Instantiate(enemy, spawnPoint.transform.position, spawnPoint.transform.rotation);
        EnemyMove enemyMove = temp.GetComponent<EnemyMove>();
        enemyMove.endPoint = endPoint.transform;
        enemyMove.leavePoint = leavePoint.transform;
        enemyMove.curveHeight = curveHeight;
        enemyMove.gun = Instantiate(gun, temp.transform);
        enemyMove.gun.SetActive(false);
        IEntryBehaviour entry = entryBehaviour;
        IMoveBehaviour move = moveBehaviour;
        ILeaveBehaviour leave = leaveBehaviour;
        enemyMove.SetBehaviours(entry,move,leave);
    }    

    void SpawnEnemy(GameObject enemy,GameObject spawnPoint, GameObject endPoint, GameObject leavePoint,GameObject gun, IEntryBehaviour entryBehaviour, IMoveBehaviour moveBehaviour, ILeaveBehaviour leaveBehaviour, float curveHeight, PathCreator CurvePath)
    {
        GameObject temp = Instantiate(enemy, spawnPoint.transform.position, spawnPoint.transform.rotation);
        EnemyMove enemyMove = temp.GetComponent<EnemyMove>();
        enemyMove.endPoint = endPoint.transform;
        enemyMove.leavePoint = leavePoint.transform;
        enemyMove.curveHeight = curveHeight;
        enemyMove.moveB_PathList = CurvePathGenerator.pathInstance.GetCurvePath(CurvePath);
        enemyMove.gun=Instantiate(gun, temp.transform);
        enemyMove.gun.SetActive(false);
        IEntryBehaviour entry = entryBehaviour;
        IMoveBehaviour move = moveBehaviour;
        ILeaveBehaviour leave = leaveBehaviour;
        enemyMove.SetBehaviours(entry, move, leave);
    }

    void SpawnEnemy(GameObject enemy,GameObject spawnPoint, GameObject endPoint, GameObject leavePoint, GameObject gun, IEntryBehaviour entryBehaviour, IMoveBehaviour moveBehaviour, ILeaveBehaviour leaveBehaviour, float curveHeight,int pathListLength,float pointWaitTime)
    {
        GameObject temp = Instantiate(enemy, spawnPoint.transform.position, spawnPoint.transform.rotation);
        EnemyMove enemyMove = temp.GetComponent<EnemyMove>();
        enemyMove.endPoint = endPoint.transform;
        enemyMove.leavePoint = leavePoint.transform;
        enemyMove.curveHeight = curveHeight;
        enemyMove.moveC_PathList = Vector3PointGenerator.instance.GetMoveCPathList(pathListLength);
        enemyMove.stayTime = (float)pathListLength;
        enemyMove.pointWaitTime = pointWaitTime;
        enemyMove.gun = Instantiate(gun, temp.transform);
        enemyMove.gun.SetActive(false);
        IEntryBehaviour entry = entryBehaviour;
        IMoveBehaviour move = moveBehaviour;
        ILeaveBehaviour leave = leaveBehaviour;
        enemyMove.SetBehaviours(entry, move, leave);
    }
    IEnumerator WaveSpawn()
    {
        yield return new WaitForSeconds(3f);
        SpawnEnemy(enemyList[0], spawnTestList[0], endTestList[0], LeaveTestList[Random.Range(0, 2)], gunList[0], new EntryTypeA(),new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f,100f));
        yield return new WaitForSeconds(0.1f);
        SpawnEnemy(enemyList[0], spawnTestList[1], endTestList[2], LeaveTestList[Random.Range(0, 2)], gunList[0], new EntryTypeA(),new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f,100f));
        yield return new WaitForSeconds(0.1f);
        SpawnEnemy(enemyList[0], spawnTestList[2], endTestList[1], LeaveTestList[Random.Range(0, 2)], gunList[0], new EntryTypeA(),new MoveTypeC(), new LeaveTypeA(), Random.Range(-100f,100f),10,2f);
        yield return new WaitForSeconds(0.1f);
        SpawnEnemy(enemyList[0], spawnTestList[3], endTestList[3], LeaveTestList[Random.Range(0, 2)], gunList[0], new EntryTypeA(),new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f,100f));
        yield return new WaitForSeconds(0.1f);
        SpawnEnemy(enemyList[1], spawnTestList[0], endTestList[4], LeaveTestList[Random.Range(0, 2)], gunList[0], new EntryTypeA(),new MoveTypeB(), new LeaveTypeA(), Random.Range(-100f, 100f), pathList[0]);
        yield return new WaitForSeconds(10f);

        SpawnEnemy(enemyList[0], spawnTestList[0], endTestList[0], LeaveTestList[Random.Range(0, 2)], gunList[0], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        yield return new WaitForSeconds(0.1f);
        SpawnEnemy(enemyList[0], spawnTestList[1], endTestList[2], LeaveTestList[Random.Range(0, 2)], gunList[0], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        yield return new WaitForSeconds(0.1f);
        SpawnEnemy(enemyList[0], spawnTestList[3], endTestList[1], LeaveTestList[Random.Range(0, 2)], gunList[0], new EntryTypeA(), new MoveTypeC(), new LeaveTypeA(), Random.Range(-100f, 100f),10,2f);
        yield return new WaitForSeconds(0.1f);
        SpawnEnemy(enemyList[0], spawnTestList[2], endTestList[3], LeaveTestList[Random.Range(0, 2)], gunList[0], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        yield return new WaitForSeconds(0.1f);
        SpawnEnemy(enemyList[1], spawnTestList[0], endTestList[4], LeaveTestList[Random.Range(0, 2)], gunList[0], new EntryTypeA(), new MoveTypeB(), new LeaveTypeA(), Random.Range(-100f, 100f), pathList[0]);

        yield return new WaitForSeconds(10f);

        SpawnEnemy(enemyList[0], spawnTestList[0], endTestList[0], LeaveTestList[Random.Range(0, 2)], gunList[0], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        yield return new WaitForSeconds(0.1f);
        SpawnEnemy(enemyList[0], spawnTestList[2], endTestList[3], LeaveTestList[Random.Range(0, 2)], gunList[0], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        yield return new WaitForSeconds(0.1f);
        SpawnEnemy(enemyList[0], spawnTestList[1], endTestList[1], LeaveTestList[Random.Range(0, 2)], gunList[0], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        yield return new WaitForSeconds(0.1f);
        SpawnEnemy(enemyList[0], spawnTestList[3], endTestList[2], LeaveTestList[Random.Range(0, 2)], gunList[0], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        yield return new WaitForSeconds(0.1f);
        SpawnEnemy(enemyList[0], spawnTestList[3], endTestList[2], LeaveTestList[Random.Range(0, 2)], gunList[0], new EntryTypeA(), new MoveTypeC(), new LeaveTypeA(), Random.Range(-100f, 100f),10,2f);

        yield return new WaitForSeconds(10f);

        SpawnEnemy(enemyList[0], spawnTestList[0], endTestList[0], LeaveTestList[Random.Range(0, 2)], gunList[0], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        yield return new WaitForSeconds(0.1f);
        SpawnEnemy(enemyList[0], spawnTestList[2], endTestList[1], LeaveTestList[Random.Range(0, 2)], gunList[0], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        yield return new WaitForSeconds(0.1f);
        SpawnEnemy(enemyList[0], spawnTestList[3], endTestList[3], LeaveTestList[Random.Range(0, 2)], gunList[0], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        yield return new WaitForSeconds(0.1f);
        SpawnEnemy(enemyList[0], spawnTestList[1], endTestList[2], LeaveTestList[Random.Range(0, 2)], gunList[0], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        yield return new WaitForSeconds(0.1f);
        SpawnEnemy(enemyList[1], spawnTestList[0], endTestList[4], LeaveTestList[Random.Range(0, 2)], gunList[0], new EntryTypeA(), new MoveTypeB(), new LeaveTypeA(), Random.Range(-100f, 100f), pathList[0]);

        //yield return new WaitForSeconds(10f);

        //SpawnEnemy(enemyList[0], spawnTestList[0], endTestList[0], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        //yield return new WaitForSeconds(0.1f);
        //SpawnEnemy(enemyList[0], spawnTestList[3], endTestList[3], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        //yield return new WaitForSeconds(0.1f);
        //SpawnEnemy(enemyList[0], spawnTestList[2], endTestList[1], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        //yield return new WaitForSeconds(0.1f);
        //SpawnEnemy(enemyList[0], spawnTestList[1], endTestList[2], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        //yield return new WaitForSeconds(10f);

        //SpawnEnemy(enemyList[0], spawnTestList[0], endTestList[0], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        //yield return new WaitForSeconds(0.1f);
        //SpawnEnemy(enemyList[0], spawnTestList[3], endTestList[1], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        //yield return new WaitForSeconds(0.1f);
        //SpawnEnemy(enemyList[0], spawnTestList[1], endTestList[2], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        //yield return new WaitForSeconds(0.1f);
        //SpawnEnemy(enemyList[0], spawnTestList[2], endTestList[3], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        //yield return new WaitForSeconds(0.1f);
        //SpawnEnemy(enemyList[1], spawnTestList[0], endTestList[4], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeB(), new LeaveTypeA(), Random.Range(-100f, 100f), pathList[0]);
        //yield return new WaitForSeconds(10f);

        //SpawnEnemy(enemyList[0], spawnTestList[0], endTestList[0], LeaveTestList[Random.Range(0, 2)],new EntryTypeA(),new MoveTypeA(), new LeaveTypeA(),Random.Range(-100f,100f));
        //yield return new WaitForSeconds(0.1f);
        //SpawnEnemy(enemyList[0], spawnTestList[1], endTestList[2], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(),new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f,100f));
        //yield return new WaitForSeconds(0.1f);
        //SpawnEnemy(enemyList[0], spawnTestList[2], endTestList[1], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(),new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f,100f));
        //yield return new WaitForSeconds(0.1f);
        //SpawnEnemy(enemyList[0], spawnTestList[3], endTestList[3], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(),new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f,100f));
        //yield return new WaitForSeconds(0.1f);
        //SpawnEnemy(enemyList[1], spawnTestList[0], endTestList[4], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeB(), new LeaveTypeA(), Random.Range(-100f, 100f), pathList[0]);
        //yield return new WaitForSeconds(10f);

        //SpawnEnemy(enemyList[0], spawnTestList[0], endTestList[0], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        //yield return new WaitForSeconds(0.1f);
        //SpawnEnemy(enemyList[0], spawnTestList[1], endTestList[2], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        //yield return new WaitForSeconds(0.1f);
        //SpawnEnemy(enemyList[0], spawnTestList[3], endTestList[1], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        //yield return new WaitForSeconds(0.1f);
        //SpawnEnemy(enemyList[0], spawnTestList[2], endTestList[3], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        //yield return new WaitForSeconds(0.1f);
        //SpawnEnemy(enemyList[1], spawnTestList[0], endTestList[4], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeB(), new LeaveTypeA(), Random.Range(-100f, 100f), pathList[0]);
        //yield return new WaitForSeconds(10f);

        //SpawnEnemy(enemyList[0], spawnTestList[0], endTestList[0], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        //yield return new WaitForSeconds(0.1f);
        //SpawnEnemy(enemyList[0], spawnTestList[2], endTestList[3], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        //yield return new WaitForSeconds(0.1f);
        //SpawnEnemy(enemyList[0], spawnTestList[1], endTestList[1], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        //yield return new WaitForSeconds(0.1f);
        //SpawnEnemy(enemyList[0], spawnTestList[3], endTestList[2], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        //yield return new WaitForSeconds(10f);

        //SpawnEnemy(enemyList[0], spawnTestList[0], endTestList[0], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        //yield return new WaitForSeconds(0.1f);
        //SpawnEnemy(enemyList[0], spawnTestList[2], endTestList[1], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        //yield return new WaitForSeconds(0.1f);
        //SpawnEnemy(enemyList[0], spawnTestList[3], endTestList[3], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        //yield return new WaitForSeconds(0.1f);
        //SpawnEnemy(enemyList[0], spawnTestList[1], endTestList[2], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        //yield return new WaitForSeconds(0.1f);
        //SpawnEnemy(enemyList[1], spawnTestList[0], endTestList[4], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeB(), new LeaveTypeA(), Random.Range(-100f, 100f), pathList[0]);
        //yield return new WaitForSeconds(10f);

        //SpawnEnemy(enemyList[0], spawnTestList[0], endTestList[0], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        //yield return new WaitForSeconds(0.1f);
        //SpawnEnemy(enemyList[0], spawnTestList[3], endTestList[3], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        //yield return new WaitForSeconds(0.1f);
        //SpawnEnemy(enemyList[0], spawnTestList[2], endTestList[1], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        //yield return new WaitForSeconds(0.1f);
        //SpawnEnemy(enemyList[0], spawnTestList[1], endTestList[2], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        //yield return new WaitForSeconds(0.1f);
        //SpawnEnemy(enemyList[1], spawnTestList[0], endTestList[4], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeB(), new LeaveTypeA(), Random.Range(-100f, 100f), pathList[0]);
        //yield return new WaitForSeconds(10f);

        //SpawnEnemy(enemyList[0], spawnTestList[0], endTestList[0], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        //yield return new WaitForSeconds(0.1f);
        //SpawnEnemy(enemyList[0], spawnTestList[3], endTestList[1], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        //yield return new WaitForSeconds(0.1f);
        //SpawnEnemy(enemyList[0], spawnTestList[1], endTestList[2], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        //yield return new WaitForSeconds(0.1f);
        //SpawnEnemy(enemyList[0], spawnTestList[2], endTestList[3], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        //yield return new WaitForSeconds(10f);

        //SpawnEnemy(enemyList[0], spawnTestList[0], endTestList[0], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        //SpawnEnemy(enemyList[0], spawnTestList[1], endTestList[2], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        //SpawnEnemy(enemyList[0], spawnTestList[2], endTestList[1], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        //SpawnEnemy(enemyList[0], spawnTestList[3], endTestList[3], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        //yield return new WaitForSeconds(0.1f);
        //SpawnEnemy(enemyList[1], spawnTestList[0], endTestList[4], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeB(), new LeaveTypeA(), Random.Range(-100f, 100f), pathList[0]);
        //yield return new WaitForSeconds(10f);

        //SpawnEnemy(enemyList[0], spawnTestList[0], endTestList[0], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        //SpawnEnemy(enemyList[0], spawnTestList[1], endTestList[2], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        //SpawnEnemy(enemyList[0], spawnTestList[3], endTestList[1], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        //SpawnEnemy(enemyList[0], spawnTestList[2], endTestList[3], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        //yield return new WaitForSeconds(10f);

        //SpawnEnemy(enemyList[0], spawnTestList[0], endTestList[0], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        //SpawnEnemy(enemyList[0], spawnTestList[2], endTestList[3], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        //SpawnEnemy(enemyList[0], spawnTestList[1], endTestList[1], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        //SpawnEnemy(enemyList[0], spawnTestList[3], endTestList[2], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        //yield return new WaitForSeconds(0.1f);
        //SpawnEnemy(enemyList[1], spawnTestList[0], endTestList[4], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeB(), new LeaveTypeA(), Random.Range(-100f, 100f), pathList[0]);
        //yield return new WaitForSeconds(10f);

        //SpawnEnemy(enemyList[0], spawnTestList[0], endTestList[0], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        //SpawnEnemy(enemyList[0], spawnTestList[2], endTestList[1], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        //SpawnEnemy(enemyList[0], spawnTestList[3], endTestList[3], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        //SpawnEnemy(enemyList[0], spawnTestList[1], endTestList[2], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        //yield return new WaitForSeconds(10f);

        //SpawnEnemy(enemyList[0], spawnTestList[0], endTestList[0], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        //SpawnEnemy(enemyList[0], spawnTestList[3], endTestList[3], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        //SpawnEnemy(enemyList[0], spawnTestList[2], endTestList[1], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        //SpawnEnemy(enemyList[0], spawnTestList[1], endTestList[2], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        //yield return new WaitForSeconds(0.1f);
        //SpawnEnemy(enemyList[1], spawnTestList[0], endTestList[4], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeB(), new LeaveTypeA(), Random.Range(-100f, 100f), pathList[0]);
        //yield return new WaitForSeconds(10f);

        //SpawnEnemy(enemyList[0], spawnTestList[0], endTestList[0], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        //SpawnEnemy(enemyList[0], spawnTestList[3], endTestList[1], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        //SpawnEnemy(enemyList[0], spawnTestList[1], endTestList[2], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        //SpawnEnemy(enemyList[0], spawnTestList[2], endTestList[3], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));

        //yield return new WaitForSeconds(10f);

        //SpawnEnemy(enemyList[0], spawnTestList[0], endTestList[0], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        //SpawnEnemy(enemyList[0], spawnTestList[1], endTestList[2], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        //SpawnEnemy(enemyList[0], spawnTestList[2], endTestList[1], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        //SpawnEnemy(enemyList[0], spawnTestList[3], endTestList[3], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        //yield return new WaitForSeconds(10f);

        //SpawnEnemy(enemyList[0], spawnTestList[0], endTestList[0], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        //SpawnEnemy(enemyList[0], spawnTestList[1], endTestList[2], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        //SpawnEnemy(enemyList[0], spawnTestList[3], endTestList[1], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        //SpawnEnemy(enemyList[0], spawnTestList[2], endTestList[3], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        //yield return new WaitForSeconds(10f);

        //SpawnEnemy(enemyList[0], spawnTestList[0], endTestList[0], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        //SpawnEnemy(enemyList[0], spawnTestList[2], endTestList[3], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        //SpawnEnemy(enemyList[0], spawnTestList[1], endTestList[1], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        //SpawnEnemy(enemyList[0], spawnTestList[3], endTestList[2], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        //yield return new WaitForSeconds(10f);

        //SpawnEnemy(enemyList[0], spawnTestList[0], endTestList[0], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        //SpawnEnemy(enemyList[0], spawnTestList[2], endTestList[1], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        //SpawnEnemy(enemyList[0], spawnTestList[3], endTestList[3], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        //SpawnEnemy(enemyList[0], spawnTestList[1], endTestList[2], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        //yield return new WaitForSeconds(10f);

        //SpawnEnemy(enemyList[0], spawnTestList[0], endTestList[0], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        //SpawnEnemy(enemyList[0], spawnTestList[3], endTestList[3], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        //SpawnEnemy(enemyList[0], spawnTestList[2], endTestList[1], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        //SpawnEnemy(enemyList[0], spawnTestList[1], endTestList[2], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        //yield return new WaitForSeconds(10f);

        //SpawnEnemy(enemyList[0], spawnTestList[0], endTestList[0], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        //SpawnEnemy(enemyList[0], spawnTestList[3], endTestList[1], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        //SpawnEnemy(enemyList[0], spawnTestList[1], endTestList[2], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
        //SpawnEnemy(enemyList[0], spawnTestList[2], endTestList[3], LeaveTestList[Random.Range(0, 2)], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f));
    }
}
