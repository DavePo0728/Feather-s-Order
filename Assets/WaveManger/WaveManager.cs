using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using UnityEngine.InputSystem;

public class WaveManager : MonoBehaviour
{
    [SerializeField]
    List<GameObject> enemyList;
    [Space(10)]
    //List<GameObject> _12SpawnPointList, _6SpawnPointList, _3SpawnPointList, _3VerticalSpawnPointList;
    //List<GameObject> _12EndPointList, _6EndPointList, _3EndPointList, _3VerticalEndPointList;
    //List<GameObject> spawnTestList,endTestList,LeaveTestList;
    [SerializeField]
    List<PathCreator> pathList;
    public enum BulletType
    {
        Black,
        Red,
        Purple,
        BlackRed,
    }
    //public BulletType bulletType;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WaveSpawn());
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            StartCoroutine(SpawnGroup1_1());
        }
    }
    //spawn A
    void SpawnEnemy(GameObject enemy, float hp, bool haveShield,float shieldHp,
        Vector3 spawnPoint, Vector3 endPoint, Vector3 leavePoint, 
        IEntryBehaviour entryBehaviour,IMoveBehaviour moveBehaviour , ILeaveBehaviour leaveBehaviour,float curveHeight,
        int gunIndex,float rpm,float shootingCoolDown, float bulletAmount, float spinSpeed,BulletType bulletType,float MaxShootWave)
    {
        GameObject temp = Instantiate(enemy, spawnPoint, Quaternion.identity);
        EnemyMove enemyMove = temp.GetComponent<EnemyMove>();
        enemyHp enemyHp = temp.GetComponent<enemyHp>();
        enemyHp.Maxhp = hp;
        enemyHp.haveshield = haveShield;
        enemyHp.maxShieldHp = shieldHp;
        enemyMove.endPoint = endPoint;
        enemyMove.leavePoint = leavePoint;
        enemyMove.curveHeight = curveHeight;
        //Debug.Log(gunIndex);
        IEntryBehaviour entry = entryBehaviour;
        IMoveBehaviour move = moveBehaviour;
        ILeaveBehaviour leave = leaveBehaviour;
        enemyMove.SetBehaviours(entry,move,leave);
        enemyMove.ActiveGun(gunIndex, rpm, bulletAmount, spinSpeed, shootingCoolDown, (EnemyMove.BulletType)bulletType,MaxShootWave);
    }    
    //spawn B
    void SpawnEnemy(GameObject enemy, float hp, bool haveShield, float shieldHp,
        Vector3 spawnPoint, Vector3 endPoint, Vector3 leavePoint, 
        IEntryBehaviour entryBehaviour, IMoveBehaviour moveBehaviour, ILeaveBehaviour leaveBehaviour, float curveHeight, PathCreator CurvePath,
        int gunIndex, float rpm, float shootingCoolDown, float bulletAmount, float spinSpeed, BulletType bulletType, float MaxShootWave)
    {
        GameObject temp = Instantiate(enemy, spawnPoint, Quaternion.identity);
        EnemyMove enemyMove = temp.GetComponent<EnemyMove>();
        enemyHp enemyHp = temp.GetComponent<enemyHp>();
        enemyHp.Maxhp = hp;
        enemyHp.haveshield = haveShield;
        enemyHp.maxShieldHp = shieldHp;
        enemyMove.endPoint = endPoint;
        enemyMove.leavePoint = leavePoint;
        enemyMove.curveHeight = curveHeight;
        enemyMove.moveB_PathList = CurvePathGenerator.pathInstance.GetCurvePath(CurvePath);
        enemyMove.ActiveGun(gunIndex, rpm, bulletAmount, spinSpeed,shootingCoolDown,(EnemyMove.BulletType)bulletType, MaxShootWave);
        IEntryBehaviour entry = entryBehaviour;
        IMoveBehaviour move = moveBehaviour;
        ILeaveBehaviour leave = leaveBehaviour;
        enemyMove.SetBehaviours(entry, move, leave);
    }
    //spawn C
    void SpawnEnemy(GameObject enemy,float hp, bool haveShield, float shieldHp,
        Vector3 spawnPoint, Vector3 endPoint, Vector3 leavePoint, 
        IEntryBehaviour entryBehaviour, IMoveBehaviour moveBehaviour, ILeaveBehaviour leaveBehaviour, 
        float curveHeight,int pathListLength,float pointWaitTime,
        int gunIndex, float rpm, float shootingCoolDown, float bulletAmount,float spinSpeed, BulletType bulletType, float MaxShootWave)
    {
        GameObject temp = Instantiate(enemy, spawnPoint, Quaternion.identity);
        EnemyMove enemyMove = temp.GetComponent<EnemyMove>();
        enemyHp enemyHp = temp.GetComponent<enemyHp>();
        enemyHp.Maxhp = hp;
        enemyHp.haveshield = haveShield;
        enemyHp.maxShieldHp = shieldHp;
        enemyMove.endPoint = endPoint;
        enemyMove.leavePoint = leavePoint;
        enemyMove.curveHeight = curveHeight;
        enemyMove.moveC_PathList = Vector3PointGenerator.instance.GetMoveCPathList(pathListLength);
        //enemyMove.stayTime = (float)pathListLength;
        enemyMove.pointWaitTime = pointWaitTime;
        enemyMove.ActiveGun(gunIndex, rpm, bulletAmount, spinSpeed,shootingCoolDown, (EnemyMove.BulletType)bulletType, MaxShootWave);
        IEntryBehaviour entry = entryBehaviour;
        IMoveBehaviour move = moveBehaviour;
        ILeaveBehaviour leave = leaveBehaviour;
        enemyMove.SetBehaviours(entry, move, leave);
    }

    /*A_01: done
    SpawnEnemy(enemyList[0],5,false,0, spawnTestList[0], endTestList[0], LeaveTestList[Random.Range(0, 2)],
    new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f),1,300,1.5f,3,0,BulletType.Red);
    */
    /*A_01S: done
     SpawnEnemy(enemyList[0],5,true,5, spawnTestList[0], endTestList[0], LeaveTestList[Random.Range(0, 2)],
     new EntryTypeA(), new MoveTypeB(), new LeaveTypeA(), Random.Range(-100f, 100f),3,300,1f,6,0,BulletType.Black);
     */
    /*A_02: done
     SpawnEnemy(enemyList[0],5,false,0, spawnTestList[0], endTestList[0], LeaveTestList[Random.Range(0, 2)],
     new EntryTypeA(), new MoveTypeB(), new LeaveTypeA(), Random.Range(-100f, 100f),pathList[0],4,300,2.5f,4,0,BulletType.purple);
     */
    /*A_02S: done
     SpawnEnemy(enemyList[0],5,true,5, spawnTestList[0], endTestList[0], LeaveTestList[Random.Range(0, 2)],
     new EntryTypeA(), new MoveTypeC(), new LeaveTypeA(), Random.Range(-100f, 100f),pathList[0],4,300,2.5f,4,0,BulletType.purple);
     */
    /*A_03: done
     SpawnEnemy(enemyList[0],5,false,0, spawnTestList[0], endTestList[0], LeaveTestList[Random.Range(0, 2)],
     new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), Random.Range(-100f, 100f),1,300,1.5f,3,0,BulletType.Red);
     */
    /*A_04: done
      SpawnEnemy(enemyList[0],5,false,0, spawnTestList[0], endTestList[0], LeaveTestList[Random.Range(0, 2)],
     new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), Random.Range(-100f, 100f),1,300,1.5f,3,0,BulletType.Red);
     */
    /*A_05:
     SpawnEnemy(enemyList[0],5,false,0, spawnTestList[0], endTestList[0], LeaveTestList[Random.Range(0, 2)],
     new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f),1,300,1.5f,3,0,BulletType.Red);
    */
    /*B_01:
    SpawnEnemy(enemyList[1],20,false,0, spawnTestList[0], endTestList[0], LeaveTestList[Random.Range(0, 2)],
    new EntryTypeA(), new MoveTypeC(), new LeaveTypeA(), Random.Range(-100f, 100f),pathList[0],5,300,2.5f,4,2,BulletType.red);
    */

    /*SpawnEnemy(GameObject enemy, float hp, bool haveShield,float shieldHp,
    Vector3 spawnPoint, Vector3 endPoint, Vector3 leavePoint,
    IEntryBehaviour entryBehaviour,IMoveBehaviour moveBehaviour, ILeaveBehaviour leaveBehaviour,float curveHeight,
    int gunIndex,float rpm,float shootingCoolDown, float bulletAmount, float spinSpeed, BulletType bulletType)
        */
    IEnumerator SpawnGroup1_1()
    {
        SpawnEnemy(enemyList[0], 5, false, 0, Vector3PointGenerator.instance.GetPoint(2,1,1), Vector3PointGenerator.instance.GetPoint(2, 1, 5), Vector3PointGenerator.instance.GetPoint(19, 5, 14),
     new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), 1, 1, 300, 1.5f, 3, 0, BulletType.Red,2);
        yield return new WaitForSeconds(0.1f);
        SpawnEnemy(enemyList[0], 5, false, 0, Vector3PointGenerator.instance.GetPoint(3, 2, 1), Vector3PointGenerator.instance.GetPoint(3, 2, 5), Vector3PointGenerator.instance.GetPoint(19, 6, 14),
     new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), 1, 1, 300, 1.5f, 3, 0, BulletType.Red,2);
        yield return new WaitForSeconds(0.1f);
        SpawnEnemy(enemyList[0], 5, false, 0, Vector3PointGenerator.instance.GetPoint(4, 1, 1), Vector3PointGenerator.instance.GetPoint(4, 1, 5), Vector3PointGenerator.instance.GetPoint(19, 5, 14),
     new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), Random.Range(-100f, 100f), 1, 300, 1.5f, 3, 0, BulletType.Red,2);
        yield return new WaitForSeconds(0.1f);
        SpawnEnemy(enemyList[0], 5, false, 0, Vector3PointGenerator.instance.GetPoint(6, 1, 1), Vector3PointGenerator.instance.GetPoint(6, 1, 5), Vector3PointGenerator.instance.GetPoint(19, 5, 14),
     new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), Random.Range(-100f, 100f), 1, 300, 1.5f, 3, 0, BulletType.Red,2);
        yield return new WaitForSeconds(0.1f);
        SpawnEnemy(enemyList[0], 5, false, 0, Vector3PointGenerator.instance.GetPoint(7, 2, 1), Vector3PointGenerator.instance.GetPoint(7, 2, 5), Vector3PointGenerator.instance.GetPoint(19, 6, 14),
     new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), Random.Range(-100f, 100f), 1, 300, 1.5f, 3, 0, BulletType.Red,2);
        yield return new WaitForSeconds(0.1f);
        SpawnEnemy(enemyList[0], 5, false, 0, Vector3PointGenerator.instance.GetPoint(8, 2, 1), Vector3PointGenerator.instance.GetPoint(8, 1, 5), Vector3PointGenerator.instance.GetPoint(19, 5, 14),
     new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), Random.Range(-100f, 100f), 1, 300, 1.5f, 3, 0, BulletType.Red,2);
        yield return new WaitForSeconds(0.1f);
    }
    IEnumerator SpawnGroup1_2()
    {
        yield return null;
    }
    IEnumerator WaveSpawn()
    {
        StartCoroutine(SpawnGroup1_1());
        yield return new WaitForSeconds(10f);
    }
}
