using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using UnityEngine.InputSystem;
using System.Net;

public class WaveManager : MonoBehaviour
{
    [SerializeField]
    List<GameObject> enemyList;
    [Space(10)]
    [SerializeField]
    List<PathCreator> pathList;
    [SerializeField]
    List<CustomPathData> customPathDataList;

    [Header("UI")]
    [SerializeField]
    GameObject gameoverPanel;
    [SerializeField]
    GameObject gameOverText, GameClearText;
    [SerializeField]
    bool debug = false;
    [SerializeField]
    EnemyData[] enemyDatas;
    public enum BulletType
    {
        Black,
        Red,
        Purple,
        BlackRed,
    }
    private void Awake()
    {
        //customPathDataList = new List<CustomPathData>();
        //customPathDataList.Add(Resources.Load<CustomPathData>("PathData/PathData1"));

    }

    //public BulletType bulletType;
    // Start is called before the first frame update
    void Start()
    {
        if(!debug)
        StartCoroutine(WaveSpawn());
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.KeypadPlus))
        {
            StartCoroutine(WaveSpawn());
        }
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            //StartCoroutine(SpawnGroup1_1());
            StartCoroutine(TestSpawn());
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            StartCoroutine(SpawnGroup1_2());
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            StartCoroutine(SpawnGroup1_3());
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            StartCoroutine(SpawnGroup1_4());
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            StartCoroutine(SpawnGroup1_5());
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            StartCoroutine(SpawnGroup1_6());
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            StartCoroutine(SpawnGroup1_7());
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            StartCoroutine(SpawnGroup1_8());
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            StartCoroutine(SpawnGroup1_9());
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            StartCoroutine(SpawnGroup1_10());
        }
        if (Input.GetKeyDown(KeyCode.Minus))
        {
            StartCoroutine(SpawnGroup1_11());
        }
        if (Input.GetKeyDown(KeyCode.Equals))
        {
            StartCoroutine(SpawnGroup1_12());
        }
        if (Input.GetKeyDown(KeyCode.Backslash))
        {
            StartCoroutine(SpawnGroup1_13());
        }
    }
    //spawn A
    void SpawnEnemy(GameObject enemy, float hp,float lifeTime,bool corrupted, float corruptionStack,float paralyzeTime, bool haveShield,float shieldHp,
        Vector3 spawnPoint, Vector3 endPoint, Vector3 leavePoint, 
        IEntryBehaviour entryBehaviour,IMoveBehaviour moveABehaviour , ILeaveBehaviour leaveBehaviour,float curveHeight,
        int gunIndex,float rpm,float shootingCoolDown, float bulletAmount, float spinSpeed,BulletType bulletType,float MaxShootWave)
    {
        GameObject temp = Instantiate(enemy, spawnPoint, Quaternion.identity);
        EnemyMove enemyMove = temp.GetComponent<EnemyMove>();
        enemyHp enemyHp = temp.GetComponent<enemyHp>();
        enemyHp.maxHp = hp;
        enemyHp.corrupted = corrupted;
        enemyHp.haveshield = haveShield;
        enemyHp.maxShieldHp = shieldHp;
        enemyHp.MaxcorruptionStack = corruptionStack;
        enemyMove.lifeTime = lifeTime;
        enemyMove.paralyzeTime = paralyzeTime;
        enemyMove.endPoint = endPoint;
        enemyMove.leavePoint = leavePoint;
        enemyMove.curveHeight = curveHeight;
        //Debug.Log(gunIndex);
        IEntryBehaviour entry = entryBehaviour;
        IMoveBehaviour move = moveABehaviour;
        ILeaveBehaviour leave = leaveBehaviour;
        enemyMove.SetBehaviours(entry,move,leave);
        enemyMove.ActiveGun(gunIndex, rpm, bulletAmount, spinSpeed, shootingCoolDown, (EnemyMove.BulletType)bulletType,MaxShootWave);
    }
    //spawn B
    void SpawnEnemy(GameObject enemy, float hp, float lifeTime, bool corrupted, float corruptionStack, float paralyzeTime, bool haveShield, float shieldHp,
        Vector3 spawnPoint, Vector3 endPoint, Vector3 leavePoint, 
        IEntryBehaviour entryBehaviour, IMoveBehaviour moveBBehaviour, ILeaveBehaviour leaveBehaviour, float curveHeight, PathCreator CurvePath,
        int gunIndex, float rpm, float shootingCoolDown, float bulletAmount, float spinSpeed, BulletType bulletType, float MaxShootWave)
    {
        GameObject temp = Instantiate(enemy, spawnPoint, Quaternion.identity);
        EnemyMove enemyMove = temp.GetComponent<EnemyMove>();
        enemyHp enemyHp = temp.GetComponent<enemyHp>();
        enemyHp.maxHp = hp;
        enemyHp.corrupted = corrupted;
        enemyHp.haveshield = haveShield;
        enemyHp.maxShieldHp = shieldHp;
        enemyHp.MaxcorruptionStack = corruptionStack;
        enemyMove.lifeTime = lifeTime;
        enemyMove.paralyzeTime = paralyzeTime;
        enemyMove.endPoint = endPoint;
        enemyMove.leavePoint = leavePoint;
        enemyMove.curveHeight = curveHeight;
        enemyMove.moveB_PathList = CurvePathGenerator.pathInstance.GetCurvePath(CurvePath);
        enemyMove.ActiveGun(gunIndex, rpm, bulletAmount, spinSpeed,shootingCoolDown,(EnemyMove.BulletType)bulletType, MaxShootWave);
        IEntryBehaviour entry = entryBehaviour;
        IMoveBehaviour move = moveBBehaviour;
        ILeaveBehaviour leave = leaveBehaviour;
        enemyMove.SetBehaviours(entry, move, leave);
    }
    //spawn C
    void SpawnEnemy(GameObject enemy,float hp,float lifeTime, bool corrupted,float corruptionStack,float paralyzeTime, bool haveShield, float shieldHp,
        Vector3 spawnPoint, Vector3 endPoint, Vector3 leavePoint, 
        IEntryBehaviour entryBehaviour, IMoveBehaviour moveCBehaviour, ILeaveBehaviour leaveBehaviour, 
        float curveHeight,int pathListLength,float pointWaitTime,int pathListIndex,
        int gunIndex, float rpm, float shootingCoolDown, float bulletAmount,float spinSpeed, BulletType bulletType, float MaxShootWave)
    {
        GameObject temp = Instantiate(enemy, spawnPoint, Quaternion.identity);
        EnemyMove enemyMove = temp.GetComponent<EnemyMove>();
        enemyHp enemyHp = temp.GetComponent<enemyHp>();
        enemyHp.maxHp = hp;
        enemyHp.corrupted = corrupted;
        enemyHp.haveshield = haveShield;
        enemyHp.maxShieldHp = shieldHp;
        enemyHp.MaxcorruptionStack = corruptionStack;
        enemyMove.lifeTime = lifeTime;
        enemyMove.paralyzeTime = paralyzeTime;
        enemyMove.endPoint = endPoint;
        enemyMove.leavePoint = leavePoint;
        enemyMove.curveHeight = curveHeight;
        enemyMove.moveC_PathList = Vector3PointGenerator.instance.GetMoveCPathList(4, customPathDataList[pathListIndex]);
        //enemyMove.stayTime = (float)pathListLength;
        enemyMove.pointWaitTime = pointWaitTime;
        enemyMove.ActiveGun(gunIndex, rpm, bulletAmount, spinSpeed,shootingCoolDown, (EnemyMove.BulletType)bulletType, MaxShootWave);
        IEntryBehaviour entry = entryBehaviour;
        IMoveBehaviour move = moveCBehaviour;
        ILeaveBehaviour leave = leaveBehaviour;
        enemyMove.SetBehaviours(entry, move, leave);
    }
    void NewSpawn(EnemyData enemyData,IEntryBehaviour entryBehaviour, IMoveBehaviour moveABehaviour, ILeaveBehaviour leaveBehaviour)
    {
        Vector3 spawnPoint = Vector3PointGenerator.instance.GetPoint((int)enemyData.data.spawnPosition.x, (int)enemyData.data.spawnPosition.y, (int)enemyData.data.spawnPosition.z);
        Vector3 endPoint = Vector3PointGenerator.instance.GetPoint((int)enemyData.data.endPosition.x, (int)enemyData.data.endPosition.y, (int)enemyData.data.endPosition.z);
        Vector3 leavePoint = Vector3PointGenerator.instance.GetPoint((int)enemyData.data.LeavePositon.x, (int)enemyData.data.LeavePositon.y, (int)enemyData.data.LeavePositon.z);
        GameObject temp = Instantiate(enemyData.data.enemy, spawnPoint, Quaternion.identity);
        EnemyMove enemyMove = temp.GetComponent<EnemyMove>();
        enemyHp enemyHp = temp.GetComponent<enemyHp>();
        enemyHp.maxHp = enemyData.data.hp;
        enemyMove.lifeTime = enemyData.data.lifeTime;
        enemyMove.paralyzeTime = enemyData.data.paralyzeTime;
        enemyHp.corrupted = enemyData.data.corrupted;
        enemyHp.haveshield = enemyData.data.haveShield;
        enemyHp.maxShieldHp = enemyData.data.shieldHp;
        enemyHp.MaxcorruptionStack = enemyData.data.corruptionStack;
        enemyMove.endPoint = endPoint;
        enemyMove.leavePoint = leavePoint;
        if(enemyData.data.curveHeight == 0)
        {
            enemyData.data.curveHeight = Random.Range(-100f, 100f);
        }
        else
        {
            enemyMove.curveHeight = enemyData.data.curveHeight;
        }
        IEntryBehaviour entry = entryBehaviour;
        IMoveBehaviour move = moveABehaviour;
        ILeaveBehaviour leave = leaveBehaviour;
        enemyMove.SetBehaviours(entry, move, leave);
        enemyMove.ActiveGun(enemyData.data.gunIndex, enemyData.data.rpm, enemyData.data.bulletAmount, enemyData.data.spinSpeed, enemyData.data.shootingCoolDown, (EnemyMove.BulletType)enemyData.data.bulletType, enemyData.data.MaxShootWave);
    }
    void NewSpawnB(EnemyData enemyData, IEntryBehaviour entryBehaviour, IMoveBehaviour moveABehaviour, ILeaveBehaviour leaveBehaviour)
    {
        Vector3 spawnPoint = Vector3PointGenerator.instance.GetPoint((int)enemyData.data.spawnPosition.x, (int)enemyData.data.spawnPosition.y, (int)enemyData.data.spawnPosition.z);
        Vector3 endPoint = Vector3PointGenerator.instance.GetPoint((int)enemyData.data.endPosition.x, (int)enemyData.data.endPosition.y, (int)enemyData.data.endPosition.z);
        Vector3 leavePoint = Vector3PointGenerator.instance.GetPoint((int)enemyData.data.LeavePositon.x, (int)enemyData.data.LeavePositon.y, (int)enemyData.data.LeavePositon.z);
        GameObject temp = Instantiate(enemyData.data.enemy, spawnPoint, Quaternion.identity);
        EnemyMove enemyMove = temp.GetComponent<EnemyMove>();
        enemyHp enemyHp = temp.GetComponent<enemyHp>();
        enemyHp.maxHp = enemyData.data.hp;
        enemyMove.lifeTime = enemyData.data.lifeTime;
        enemyMove.paralyzeTime = enemyData.data.paralyzeTime;
        enemyHp.corrupted = enemyData.data.corrupted;
        enemyHp.haveshield = enemyData.data.haveShield;
        enemyHp.maxShieldHp = enemyData.data.shieldHp;
        enemyHp.MaxcorruptionStack = enemyData.data.corruptionStack;
        enemyMove.endPoint = endPoint;
        enemyMove.leavePoint = leavePoint;
        if (enemyData.data.curveHeight == 0)
        {
            enemyData.data.curveHeight = Random.Range(-100f, 100f);
        }
        else
        {
            enemyMove.curveHeight = enemyData.data.curveHeight;
        }
        enemyMove.moveB_PathList = CurvePathGenerator.pathInstance.GetCurvePath(pathList[enemyData.data.pathNum]);
        IEntryBehaviour entry = entryBehaviour;
        IMoveBehaviour move = moveABehaviour;
        ILeaveBehaviour leave = leaveBehaviour;
        enemyMove.SetBehaviours(entry, move, leave);
        enemyMove.ActiveGun(enemyData.data.gunIndex, enemyData.data.rpm, enemyData.data.bulletAmount, enemyData.data.spinSpeed, enemyData.data.shootingCoolDown, (EnemyMove.BulletType)enemyData.data.bulletType, enemyData.data.MaxShootWave);
    }
    void NewSpawnC(EnemyData enemyData, IEntryBehaviour entryBehaviour, IMoveBehaviour moveABehaviour, ILeaveBehaviour leaveBehaviour)
    {
        Vector3 spawnPoint = Vector3PointGenerator.instance.GetPoint((int)enemyData.data.spawnPosition.x, (int)enemyData.data.spawnPosition.y, (int)enemyData.data.spawnPosition.z);
        Vector3 endPoint = Vector3PointGenerator.instance.GetPoint((int)enemyData.data.endPosition.x, (int)enemyData.data.endPosition.y, (int)enemyData.data.endPosition.z);
        Vector3 leavePoint = Vector3PointGenerator.instance.GetPoint((int)enemyData.data.LeavePositon.x, (int)enemyData.data.LeavePositon.y, (int)enemyData.data.LeavePositon.z);
        GameObject temp = Instantiate(enemyData.data.enemy, spawnPoint, Quaternion.identity);
        EnemyMove enemyMove = temp.GetComponent<EnemyMove>();
        enemyHp enemyHp = temp.GetComponent<enemyHp>();
        enemyHp.maxHp = enemyData.data.hp;
        enemyMove.lifeTime = enemyData.data.lifeTime;
        enemyMove.paralyzeTime = enemyData.data.paralyzeTime;
        enemyHp.corrupted = enemyData.data.corrupted;
        enemyHp.haveshield = enemyData.data.haveShield;
        enemyHp.maxShieldHp = enemyData.data.shieldHp;
        enemyHp.MaxcorruptionStack = enemyData.data.corruptionStack;
        enemyMove.endPoint = endPoint;
        enemyMove.leavePoint = leavePoint;
        if (enemyData.data.curveHeight == 0)
        {
            enemyData.data.curveHeight = Random.Range(-100f, 100f);
        }
        else
        {
            enemyMove.curveHeight = enemyData.data.curveHeight;
        }
        CustomPathData path = customPathDataList[enemyData.data.customPathNum];
        enemyMove.moveC_PathList = Vector3PointGenerator.instance.GetMoveCPathList(path.pathX.Count, path);
        enemyMove.pointWaitTime = enemyData.data.pointWaitTime;
        IEntryBehaviour entry = entryBehaviour;
        IMoveBehaviour move = moveABehaviour;
        ILeaveBehaviour leave = leaveBehaviour;
        enemyMove.SetBehaviours(entry, move, leave);
        enemyMove.ActiveGun(enemyData.data.gunIndex, enemyData.data.rpm, enemyData.data.bulletAmount, enemyData.data.spinSpeed, enemyData.data.shootingCoolDown, (EnemyMove.BulletType)enemyData.data.bulletType, enemyData.data.MaxShootWave);
    }
    /*A_01: done
    SpawnEnemy(enemyList[0],5,false,0, Vector3PointGenerator.instance.GetPoint(17, 3, 0), Vector3PointGenerator.instance.GetPoint(17, 3, 0), Vector3PointGenerator.instance.GetPoint(17, 3, 0),
    new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f),1,300,1.5f,3,0,BulletType.Red);
    */
    /*A_01S: done
     SpawnEnemy(enemyList[0],5,true,5, Vector3PointGenerator.instance.GetPoint(17, 3, 0), Vector3PointGenerator.instance.GetPoint(17, 3, 0), Vector3PointGenerator.instance.GetPoint(17, 3, 0),
     new EntryTypeA(), new MoveTypeB(), new LeaveTypeA(), Random.Range(-100f, 100f),3,300,1f,6,0,BulletType.Black);
     */
    /*A_02: done
     SpawnEnemy(enemyList[0],5,false,0, Vector3PointGenerator.instance.GetPoint(17, 3, 0), Vector3PointGenerator.instance.GetPoint(17, 3, 0), Vector3PointGenerator.instance.GetPoint(17, 3, 0),
     new EntryTypeA(), new MoveTypeB(), new LeaveTypeA(), Random.Range(-100f, 100f),pathList[0],4,300,2.5f,4,0,BulletType.purple);
     */
    /*A_02S: done
     SpawnEnemy(enemyList[0],5,true,5, Vector3PointGenerator.instance.GetPoint(17, 3, 0), Vector3PointGenerator.instance.GetPoint(17, 3, 0), Vector3PointGenerator.instance.GetPoint(17, 3, 0),
     new EntryTypeA(), new MoveTypeC(), new LeaveTypeA(), Random.Range(-100f, 100f),pathList[0],4,300,2.5f,4,0,BulletType.purple);
     */
    /*A_03: done
     SpawnEnemy(enemyList[0],5,false,0, Vector3PointGenerator.instance.GetPoint(17, 3, 0), Vector3PointGenerator.instance.GetPoint(17, 3, 0), Vector3PointGenerator.instance.GetPoint(17, 3, 0),
     new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), Random.Range(-100f, 100f),1,300,1.5f,3,0,BulletType.Red);
     */
    /*A_04: done
      SpawnEnemy(enemyList[0],5,false,0, Vector3PointGenerator.instance.GetPoint(17, 3, 0), Vector3PointGenerator.instance.GetPoint(17, 3, 0), Vector3PointGenerator.instance.GetPoint(17, 3, 0),
     new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), Random.Range(-100f, 100f),1,300,1.5f,3,0,BulletType.Red);
     */
    /*A_05:
     SpawnEnemy(enemyList[0],5,false,0, Vector3PointGenerator.instance.GetPoint(17, 3, 0), Vector3PointGenerator.instance.GetPoint(17, 3, 0), Vector3PointGenerator.instance.GetPoint(17, 3, 0),
     new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f),1,300,1.5f,3,0,BulletType.Red);
    */
    /*B_01:
    SpawnEnemy(enemyList[1],20,false,0, Vector3PointGenerator.instance.GetPoint(17, 3, 0), Vector3PointGenerator.instance.GetPoint(17, 3, 0), Vector3PointGenerator.instance.GetPoint(17, 3, 0),
    new EntryTypeA(), new MoveTypeC(), new LeaveTypeA(), Random.Range(-100f, 100f),pathList[0],5,300,2.5f,4,2,BulletType.red);
    */

    /*SpawnEnemy(GameObject enemy, float hp, bool haveShield,float shieldHp,
    Vector3 spawnPoint, Vector3 endPoint, Vector3 leavePoint,
    IEntryBehaviour entryBehaviour,IMoveBehaviour moveBehaviour, ILeaveBehaviour leaveBehaviour,float curveHeight,
    int gunIndex,float rpm,float shootingCoolDown, float bulletAmount, float spinSpeed, BulletType bulletType)
        */
    IEnumerator TestSpawn()
    {
        NewSpawn(enemyDatas[0],new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());   ///MoveTypeA  Or   MovetypeD
        yield return new WaitForSeconds(0.1f);
        NewSpawnB(enemyDatas[1], new EntryTypeA(), new MoveTypeB(), new LeaveTypeA());   //MoveTypeB
        yield return new WaitForSeconds(0.1f);
        NewSpawnC(enemyDatas[2], new EntryTypeA(), new MoveTypeC(), new LeaveTypeA());   //MoveTypeC
    }
    IEnumerator SpawnGroup1_1()
    {
        NewSpawn(enemyDatas[0], new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.1f);
        NewSpawn(enemyDatas[0], new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.1f);
        NewSpawn(enemyDatas[0], new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.1f);
        NewSpawn(enemyDatas[0], new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.1f);
        NewSpawn(enemyDatas[0], new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.1f);
        NewSpawn(enemyDatas[0], new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.1f);

     //   SpawnEnemy(enemyList[0], 5,10,false, 0,0, false, 0, Vector3PointGenerator.instance.GetPoint(0,1,1), Vector3PointGenerator.instance.GetPoint(2, 1, 5), Vector3PointGenerator.instance.GetPoint(19, 5, 14),
     //new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), Random.Range(-100f, 100f), 1, 300, 1.5f, 3, 0, BulletType.Red,2);
     //   yield return new WaitForSeconds(0.1f);
     //   SpawnEnemy(enemyList[0], 5,10, false, 0,0, false, 0, Vector3PointGenerator.instance.GetPoint(0, 2, 1), Vector3PointGenerator.instance.GetPoint(3, 2, 5), Vector3PointGenerator.instance.GetPoint(19, 6, 14),
     //new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), Random.Range(-100f, 100f), 1, 300, 1.5f, 3, 0, BulletType.Red,2);
     //   yield return new WaitForSeconds(0.1f);
     //   SpawnEnemy(enemyList[0], 5, 10, false, 0, 0, false, 0, Vector3PointGenerator.instance.GetPoint(0, 1, 1), Vector3PointGenerator.instance.GetPoint(4, 1, 5), Vector3PointGenerator.instance.GetPoint(19, 5, 14),
     //new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), Random.Range(-100f, 100f), 1, 300, 1.5f, 3, 0, BulletType.Red,2);
     //   yield return new WaitForSeconds(0.1f);
     //   SpawnEnemy(enemyList[0], 5, 10, false, 0, 0, false, 0, Vector3PointGenerator.instance.GetPoint(0, 1, 1), Vector3PointGenerator.instance.GetPoint(6, 1, 5), Vector3PointGenerator.instance.GetPoint(19, 5, 14),
     //new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), Random.Range(-100f, 100f), 1, 300, 1.5f, 3, 0, BulletType.Red,2);
     //   yield return new WaitForSeconds(0.1f);
     //   SpawnEnemy(enemyList[0], 5, 10, false, 0, 0, false, 0, Vector3PointGenerator.instance.GetPoint(0, 2, 1), Vector3PointGenerator.instance.GetPoint(7, 2, 5), Vector3PointGenerator.instance.GetPoint(19, 6, 14),
     //new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), Random.Range(-100f, 100f), 1, 300, 1.5f, 3, 0, BulletType.Red,2);
     //   yield return new WaitForSeconds(0.1f);
     //   SpawnEnemy(enemyList[0], 5, 10, false, 0, 0, false, 0, Vector3PointGenerator.instance.GetPoint(0, 1, 1), Vector3PointGenerator.instance.GetPoint(8, 1, 5), Vector3PointGenerator.instance.GetPoint(19, 5, 14),
     //new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), Random.Range(-100f, 100f), 1, 300, 1.5f, 3, 0, BulletType.Red,2);
    }
    IEnumerator SpawnGroup1_2()
    {
        NewSpawn(enemyDatas[0], new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.1f);

        //   SpawnEnemy(enemyList[0], 5, 10, false,0, 0, false, 0, Vector3PointGenerator.instance.GetPoint(19, 1, 1), Vector3PointGenerator.instance.GetPoint(11, 1, 5), Vector3PointGenerator.instance.GetPoint(0, 5, 14),
        //new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), Random.Range(-100f, 100f), 1, 300, 1.5f, 3, 0, BulletType.Red, 2);
        //   yield return new WaitForSeconds(0.1f);
        //   SpawnEnemy(enemyList[0], 5, 10, false, 0, 0, false, 0, Vector3PointGenerator.instance.GetPoint(19, 2, 1), Vector3PointGenerator.instance.GetPoint(12, 2, 5), Vector3PointGenerator.instance.GetPoint(0, 6, 14),
        //new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), Random.Range(-100f, 100f), 1, 300, 1.5f, 3, 0, BulletType.Red, 2);
        //   yield return new WaitForSeconds(0.1f);
        //   SpawnEnemy(enemyList[0], 5, 10, false, 0, 0, false, 0, Vector3PointGenerator.instance.GetPoint(19, 1, 1), Vector3PointGenerator.instance.GetPoint(13, 1, 5), Vector3PointGenerator.instance.GetPoint(0, 5, 14),
        //new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), Random.Range(-100f, 100f), 1, 300, 1.5f, 3, 0, BulletType.Red, 2);
        //   yield return new WaitForSeconds(0.1f);
        //   SpawnEnemy(enemyList[0], 5, 10, false, 0, 0, false, 0, Vector3PointGenerator.instance.GetPoint(19, 1, 1), Vector3PointGenerator.instance.GetPoint(15, 1, 5), Vector3PointGenerator.instance.GetPoint(0, 5, 14),
        //new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), Random.Range(-100f, 100f), 1, 300, 1.5f, 3, 0, BulletType.Red, 2);
        //   yield return new WaitForSeconds(0.1f);
        //   SpawnEnemy(enemyList[0], 5, 10, false, 0, 0, false, 0, Vector3PointGenerator.instance.GetPoint(19, 2, 1), Vector3PointGenerator.instance.GetPoint(16, 2, 5), Vector3PointGenerator.instance.GetPoint(0, 6, 14),
        //new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), Random.Range(-100f, 100f), 1, 300, 1.5f, 3, 0, BulletType.Red, 2);
        //   yield return new WaitForSeconds(0.1f);
        //   SpawnEnemy(enemyList[0], 5, 10, false, 0, 0, false, 0, Vector3PointGenerator.instance.GetPoint(19, 2, 1), Vector3PointGenerator.instance.GetPoint(17, 1, 5), Vector3PointGenerator.instance.GetPoint(0, 5, 14),
        //new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), Random.Range(-100f, 100f), 1, 300, 1.5f, 3, 0, BulletType.Red, 2);
    }
    IEnumerator SpawnGroup1_3()
    {
        NewSpawn(enemyDatas[0], new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.1f);
        //   SpawnEnemy(enemyList[0], 5, 10, false,0, 0, false, 0, Vector3PointGenerator.instance.GetPoint(5, 2, 0), Vector3PointGenerator.instance.GetPoint(5, 5, 4), Vector3PointGenerator.instance.GetPoint(0, 5, 14),
        //new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), Random.Range(-100f, 100f), 1, 300, 1.5f, 3, 0, BulletType.Red, 0);
        //   yield return new WaitForSeconds(0.1f);
        //   SpawnEnemy(enemyList[0], 5, 10, false, 0, 0, false, 0, Vector3PointGenerator.instance.GetPoint(6, 3, 0), Vector3PointGenerator.instance.GetPoint(6, 6, 4), Vector3PointGenerator.instance.GetPoint(0, 6, 14),
        //new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), Random.Range(-100f, 100f), 1, 300, 1.5f, 3, 0, BulletType.Red, 0);
        //   yield return new WaitForSeconds(0.1f);
        //   SpawnEnemy(enemyList[0], 5, 10, false, 0, 0, false, 0, Vector3PointGenerator.instance.GetPoint(7, 2, 0), Vector3PointGenerator.instance.GetPoint(7, 5, 4), Vector3PointGenerator.instance.GetPoint(0, 5, 14),
        //new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), Random.Range(-100f, 100f), 1, 300, 1.5f, 3, 0, BulletType.Red, 0);
        //   yield return new WaitForSeconds(0.1f);
        //   SpawnEnemy(enemyList[0], 5, 10, false, 0, 0, false, 0, Vector3PointGenerator.instance.GetPoint(9, 2, 0), Vector3PointGenerator.instance.GetPoint(9, 5, 4), Vector3PointGenerator.instance.GetPoint(0, 5, 14),
        //new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), Random.Range(-100f, 100f), 1, 300, 1.5f, 3, 0, BulletType.Red, 0);
        //   yield return new WaitForSeconds(0.1f);
        //   SpawnEnemy(enemyList[0], 5, 10, false, 0, 0, false, 0, Vector3PointGenerator.instance.GetPoint(10, 3, 0), Vector3PointGenerator.instance.GetPoint(10, 6, 4), Vector3PointGenerator.instance.GetPoint(0, 6, 14),
        //new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), Random.Range(-100f, 100f), 1, 300, 1.5f, 3, 0, BulletType.Red, 0);
        //   yield return new WaitForSeconds(0.1f);
        //   SpawnEnemy(enemyList[0], 5, 10, false, 0, 0, false, 0, Vector3PointGenerator.instance.GetPoint(11, 2, 0), Vector3PointGenerator.instance.GetPoint(11, 5, 4), Vector3PointGenerator.instance.GetPoint(0, 5, 14),
        //new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), Random.Range(-100f, 100f), 1, 300, 1.5f, 3, 0, BulletType.Red, 0);
        //   yield return new WaitForSeconds(0.1f);        
        //   SpawnEnemy(enemyList[0], 5, 10, false, 0, 0, false, 0, Vector3PointGenerator.instance.GetPoint(13, 2, 0), Vector3PointGenerator.instance.GetPoint(13, 5, 4), Vector3PointGenerator.instance.GetPoint(0, 5, 14),
        //new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), Random.Range(-100f, 100f), 1, 300, 1.5f, 3, 0, BulletType.Red, 0);
        //   yield return new WaitForSeconds(0.1f);
        //   SpawnEnemy(enemyList[0], 5, 10, false, 0, 0, false, 0, Vector3PointGenerator.instance.GetPoint(14, 3, 0), Vector3PointGenerator.instance.GetPoint(14, 6, 4), Vector3PointGenerator.instance.GetPoint(0, 6, 14),
        //new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), Random.Range(-100f, 100f), 1, 300, 1.5f, 3, 0, BulletType.Red, 0);
        //   yield return new WaitForSeconds(0.1f);
        //   SpawnEnemy(enemyList[0], 5, 10, false, 0, 0, false, 0, Vector3PointGenerator.instance.GetPoint(15, 2, 0), Vector3PointGenerator.instance.GetPoint(15, 5, 4), Vector3PointGenerator.instance.GetPoint(0, 5, 14),
        //new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), Random.Range(-100f, 100f), 1, 300, 1.5f, 3, 0, BulletType.Red, 0);
    }
    IEnumerator SpawnGroup1_4()
    {
        NewSpawn(enemyDatas[0], new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.1f);
        //   SpawnEnemy(enemyList[0], 5, 10, false, 0, 0, false, 0, Vector3PointGenerator.instance.GetPoint(0, 0, 14), Vector3PointGenerator.instance.GetPoint(3, 3, 5), Vector3PointGenerator.instance.GetPoint(19, 5, 0),
        //new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), Random.Range(-100f, 100f), 1, 300, 1.5f, 3, 0, BulletType.Red,2);
        //   yield return new WaitForSeconds(0.1f);
        //   SpawnEnemy(enemyList[0], 5, 10, true,1,3, false, 0, Vector3PointGenerator.instance.GetPoint(0, 1, 14), Vector3PointGenerator.instance.GetPoint(4, 4, 5), Vector3PointGenerator.instance.GetPoint(19, 6, 0),
        //new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), Random.Range(-100f, 100f), 1, 300, 1.5f, 3, 0, BulletType.Red, 2);
        //   yield return new WaitForSeconds(0.1f);
        //   SpawnEnemy(enemyList[0], 5, 10, false, 0, 0, false, 0, Vector3PointGenerator.instance.GetPoint(0, 2, 14), Vector3PointGenerator.instance.GetPoint(3, 5, 5), Vector3PointGenerator.instance.GetPoint(19, 7, 0),
        //new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), Random.Range(-100f, 100f), 1, 300, 1.5f, 3, 0, BulletType.Red, 2);
    }
    IEnumerator SpawnGroup1_5()
    {
        NewSpawn(enemyDatas[0], new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.1f);
        //   SpawnEnemy(enemyList[0], 5, 10, false, 0, 0, false, 0, Vector3PointGenerator.instance.GetPoint(19, 9, 14), Vector3PointGenerator.instance.GetPoint(16, 5, 5), Vector3PointGenerator.instance.GetPoint(0, 3, 0),
        //new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), Random.Range(-100f, 100f), 1, 300, 1.5f, 3, 0, BulletType.Red, 2);
        //   yield return new WaitForSeconds(0.1f);
        //   SpawnEnemy(enemyList[0], 5, 10, true,1,3, false, 0, Vector3PointGenerator.instance.GetPoint(19, 10, 14), Vector3PointGenerator.instance.GetPoint(15, 6, 5), Vector3PointGenerator.instance.GetPoint(0, 4, 0),
        //new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), Random.Range(-100f, 100f), 1, 300, 1.5f, 3, 0, BulletType.Red, 2);
        //   yield return new WaitForSeconds(0.1f);
        //   SpawnEnemy(enemyList[0], 5, 10, false, 0, 0, false, 0, Vector3PointGenerator.instance.GetPoint(19, 11, 14), Vector3PointGenerator.instance.GetPoint(16, 7, 5), Vector3PointGenerator.instance.GetPoint(0, 5, 0),
        //new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), Random.Range(-100f, 100f), 1, 300, 1.5f, 3, 0, BulletType.Red, 2);
    }
    IEnumerator SpawnGroup1_6()
    {
        NewSpawn(enemyDatas[0], new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.1f);
        //   SpawnEnemy(enemyList[0], 5, 10, false, 0, 0, false, 0, Vector3PointGenerator.instance.GetPoint(0, 3, 0), Vector3PointGenerator.instance.GetPoint(3, 3, 7), Vector3PointGenerator.instance.GetPoint(19, 3, 14),
        //new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), Random.Range(-100f, 100f), 1, 300, 1.5f, 3, 0, BulletType.Red, 2);
        //   yield return new WaitForSeconds(0.1f);
        //   SpawnEnemy(enemyList[0], 5, 10, false, 0, 0, false, 0, Vector3PointGenerator.instance.GetPoint(2, 3, 0), Vector3PointGenerator.instance.GetPoint(5, 3, 7), Vector3PointGenerator.instance.GetPoint(19, 3, 14),
        //new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), Random.Range(-100f, 100f), 1, 300, 1.5f, 3, 0, BulletType.Red, 2);
        //   yield return new WaitForSeconds(0.1f);
        //   SpawnEnemy(enemyList[0], 5, 10, false, 0, 0, false, 0, Vector3PointGenerator.instance.GetPoint(0, 3, 0), Vector3PointGenerator.instance.GetPoint(3, 3, 7), Vector3PointGenerator.instance.GetPoint(19, 3, 14),
        //new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), Random.Range(-100f, 100f), 1, 300, 1.5f, 3, 0, BulletType.Red, 2);
    }
    IEnumerator SpawnGroup1_7()
    {
        NewSpawn(enemyDatas[0], new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.1f);
        //        SpawnEnemy(enemyList[0], 5, 10, false, 0, 0, false, 0, Vector3PointGenerator.instance.GetPoint(17, 3, 0), Vector3PointGenerator.instance.GetPoint(14, 3, 5), Vector3PointGenerator.instance.GetPoint(0, 5, 14),
        //new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), Random.Range(-100f, 100f), 1, 300, 1.5f, 3, 0, BulletType.Red, 2);
        //        yield return new WaitForSeconds(0.1f);
        //        SpawnEnemy(enemyList[0], 5, 10, false, 0, 0, false, 0, Vector3PointGenerator.instance.GetPoint(19, 3, 0), Vector3PointGenerator.instance.GetPoint(16, 3, 5), Vector3PointGenerator.instance.GetPoint(0, 5, 14),
        //     new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), Random.Range(-100f, 100f), 1, 300, 1.5f, 3, 0, BulletType.Red, 2);
    }
    IEnumerator SpawnGroup1_8()
    {
        NewSpawn(enemyDatas[0], new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.1f);
        //SpawnEnemy(enemyList[0], 5, 10, false, 0, 0, false, 0, Vector3PointGenerator.instance.GetPoint(0, 8, 14), Vector3PointGenerator.instance.GetPoint(4, 3, 4), Vector3PointGenerator.instance.GetPoint(0, 3, 0),
        //new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f), 1, 300, 1.5f, 3, 0, BulletType.Red,2);
        //yield return new WaitForSeconds(0.1f);
        //SpawnEnemy(enemyList[0], 5, 10, false, 0, 0, true, 5, Vector3PointGenerator.instance.GetPoint(2, 8, 14), Vector3PointGenerator.instance.GetPoint(6, 3, 4), Vector3PointGenerator.instance.GetPoint(0, 3, 0),
        //new EntryTypeA(), new MoveTypeB(), new LeaveTypeA(), Random.Range(-100f, 100f), 3, 300, 1f, 6, 0, BulletType.Black,3);
        //yield return new WaitForSeconds(0.1f);
        //SpawnEnemy(enemyList[0],5, 10, false, 0, 0, false,0, Vector3PointGenerator.instance.GetPoint(4, 8, 14), Vector3PointGenerator.instance.GetPoint(8, 3, 4), Vector3PointGenerator.instance.GetPoint(0, 3, 0),
        //new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f),1,300,1.5f,3,0,BulletType.Red,2);

    }
    IEnumerator SpawnGroup1_9()
    {
        NewSpawn(enemyDatas[0], new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.1f);
        //SpawnEnemy(enemyList[0], 5, 10, false, 0, 0, false, 0, Vector3PointGenerator.instance.GetPoint(15, 3, 14), Vector3PointGenerator.instance.GetPoint(12, 5, 4), Vector3PointGenerator.instance.GetPoint(0, 5, 5),
        //new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f), 1, 300, 1.5f, 3, 0, BulletType.Red, 2);
        //yield return new WaitForSeconds(0.1f);
        //SpawnEnemy(enemyList[0], 5, 10, true,1,3, true, 5, Vector3PointGenerator.instance.GetPoint(17, 3, 14), Vector3PointGenerator.instance.GetPoint(14, 5, 4), Vector3PointGenerator.instance.GetPoint(0, 5, 5),
        //new EntryTypeA(), new MoveTypeB(), new LeaveTypeA(), Random.Range(-100f, 100f), 3, 300, 1f, 6, 0, BulletType.Black,3);
        //yield return new WaitForSeconds(0.1f);
        //SpawnEnemy(enemyList[0], 5, 10, false, 0, 0, false, 0, Vector3PointGenerator.instance.GetPoint(19, 3, 14), Vector3PointGenerator.instance.GetPoint(16, 5, 4), Vector3PointGenerator.instance.GetPoint(0, 5, 5),
        //new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f), 1, 300, 1.5f, 3, 0, BulletType.Red, 2);
        //yield return new WaitForSeconds(0.1f);

    }
    IEnumerator SpawnGroup1_10()
    {
        NewSpawn(enemyDatas[0], new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.1f);
        //SpawnEnemy(enemyList[0], 5, 10, false, 0, 0, false, 0, Vector3PointGenerator.instance.GetPoint(8, 5, 0), Vector3PointGenerator.instance.GetPoint(8, 5, 5), Vector3PointGenerator.instance.GetPoint(8, 5, 0),
        //new EntryTypeA(), new MoveTypeB(), new LeaveTypeA(), Random.Range(-100f, 100f), pathList[0], 4, 300, 2.5f, 4, 0, BulletType.Purple,5);
        //yield return new WaitForSeconds(0.1f);
        //SpawnEnemy(enemyList[0], 5, 10, false, 0, 0, false, 0, Vector3PointGenerator.instance.GetPoint(10, 5, 0), Vector3PointGenerator.instance.GetPoint(10, 5, 5), Vector3PointGenerator.instance.GetPoint(10, 5, 0),
        //new EntryTypeA(), new MoveTypeB(), new LeaveTypeA(), Random.Range(-100f, 100f), pathList[0], 4, 300, 2.5f, 4, 0, BulletType.Purple,5);
        //yield return new WaitForSeconds(0.1f);
        //SpawnEnemy(enemyList[0], 5, 10, false, 0, 0, false, 0, Vector3PointGenerator.instance.GetPoint(12, 5, 0), Vector3PointGenerator.instance.GetPoint(12, 5, 5), Vector3PointGenerator.instance.GetPoint(12, 5, 0),
        //new EntryTypeA(), new MoveTypeB(), new LeaveTypeA(), Random.Range(-100f, 100f), pathList[0], 4, 300, 2.5f, 4, 0, BulletType.Purple,5);
    }
    IEnumerator SpawnGroup1_11()
    {
        NewSpawn(enemyDatas[0], new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.1f);
        //SpawnEnemy(enemyList[0], 5, 10, false, 0, 0, true, 5, Vector3PointGenerator.instance.GetPoint(15, 2, 14), Vector3PointGenerator.instance.GetPoint(8, 5, 5), Vector3PointGenerator.instance.GetPoint(0, 5, 0),
        //new EntryTypeA(), new MoveTypeB(), new LeaveTypeA(), Random.Range(-100f, 100f), pathList[0], 4, 300, 2.5f, 4, 0, BulletType.Purple,5);
        //yield return new WaitForSeconds(0.1f);
        //SpawnEnemy(enemyList[0], 5, 10, true, 3, 0, true, 5, Vector3PointGenerator.instance.GetPoint(17, 2, 14), Vector3PointGenerator.instance.GetPoint(9, 5, 5), Vector3PointGenerator.instance.GetPoint(2, 5, 0),
        //new EntryTypeA(), new MoveTypeB(), new LeaveTypeA(), Random.Range(-100f, 100f), pathList[0], 4, 300, 2.5f, 4, 0, BulletType.Purple,5);
        //yield return new WaitForSeconds(0.1f);
        //SpawnEnemy(enemyList[0], 5, 10, false, 0, 0, true, 5, Vector3PointGenerator.instance.GetPoint(19, 2, 14), Vector3PointGenerator.instance.GetPoint(12, 5, 5), Vector3PointGenerator.instance.GetPoint(4, 5, 0),
        //new EntryTypeA(), new MoveTypeB(), new LeaveTypeA(), Random.Range(-100f, 100f), pathList[0], 4, 300, 2.5f, 4, 0, BulletType.Purple,5);
    }
    IEnumerator SpawnGroup1_12()
    {
        NewSpawn(enemyDatas[0], new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.1f);
        //SpawnEnemy(enemyList[0], 5, 10, true,1,3, false, 0, Vector3PointGenerator.instance.GetPoint(2, 1, 14), Vector3PointGenerator.instance.GetPoint(10, 3, 5), Vector3PointGenerator.instance.GetPoint(19, 6, 0),
        //new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f), 1, 300, 1.5f, 3, 0, BulletType.Red,4);
        //yield return new WaitForSeconds(0.1f);
        //SpawnEnemy(enemyList[0], 5, 10, true,1,3, false, 0, Vector3PointGenerator.instance.GetPoint(4, 3, 14), Vector3PointGenerator.instance.GetPoint(12, 5, 5), Vector3PointGenerator.instance.GetPoint(19, 6, 0),
        //new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f), 1, 300, 1.5f, 3, 0, BulletType.Red,4);
        //yield return new WaitForSeconds(0.1f);
        //SpawnEnemy(enemyList[0], 5, 10, true,1, 3,false, 0, Vector3PointGenerator.instance.GetPoint(2, 5, 14), Vector3PointGenerator.instance.GetPoint(10, 7, 5), Vector3PointGenerator.instance.GetPoint(19, 6, 0),
        //new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f), 1, 300, 1.5f, 3, 0, BulletType.Red,4);
        //yield return new WaitForSeconds(0.1f);
        //SpawnEnemy(enemyList[0], 5, 10, true,1, 3,false, 0, Vector3PointGenerator.instance.GetPoint(0, 3, 14), Vector3PointGenerator.instance.GetPoint(8, 5, 5), Vector3PointGenerator.instance.GetPoint(19, 6, 0),
        //new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f), 1, 300, 1.5f, 3, 0, BulletType.Red,4);
        //yield return new WaitForSeconds(0.1f);
        //SpawnEnemy(enemyList[1], 20, 10, false,0,0, false, 0, Vector3PointGenerator.instance.GetPoint(2, 3, 14), Vector3PointGenerator.instance.GetPoint(10, 5, 5), Vector3PointGenerator.instance.GetPoint(19, 6, 0),
        //new EntryTypeA(), new MoveTypeC(), new LeaveTypeA(), Random.Range(-100f, 100f),4,3,0, 5, 300, 2.5f, 4, 2, BulletType.Red,0);
    }
    IEnumerator SpawnGroup1_13()
    {
        NewSpawn(enemyDatas[0], new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.1f);

        //SpawnEnemy(enemyList[0], 5, 10, true,1,3, false, 0, Vector3PointGenerator.instance.GetPoint(6, 3, 0), Vector3PointGenerator.instance.GetPoint(6, 5, 4), Vector3PointGenerator.instance.GetPoint(0, 1, 14),
        //new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f), 1, 300, 1.5f, 3, 0, BulletType.Red,3);
        //yield return new WaitForSeconds(0.1f);
        //SpawnEnemy(enemyList[0], 5, 10, false,0,0, true, 5, Vector3PointGenerator.instance.GetPoint(7, 2, 0), Vector3PointGenerator.instance.GetPoint(7, 4, 3), Vector3PointGenerator.instance.GetPoint(0, 1, 14),
        //new EntryTypeA(), new MoveTypeC(), new LeaveTypeA(), Random.Range(-100f, 100f), pathList[0], 4, 300, 2.5f, 4, 0, BulletType.Purple, 2);
        //yield return new WaitForSeconds(0.1f);
        //SpawnEnemy(enemyList[0], 5, 10, false, 0, 0, false, 0, Vector3PointGenerator.instance.GetPoint(8, 3, 0), Vector3PointGenerator.instance.GetPoint(8, 5, 4), Vector3PointGenerator.instance.GetPoint(0, 1, 14),
        //new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f), 1, 300, 1.5f, 3, 0, BulletType.Red,3);
        //yield return new WaitForSeconds(0.1f);
        //SpawnEnemy(enemyList[0], 5, 10, false, 0, 0, true, 5, Vector3PointGenerator.instance.GetPoint(9, 4, 0), Vector3PointGenerator.instance.GetPoint(9, 6, 6), Vector3PointGenerator.instance.GetPoint(0, 1, 14),
        //new EntryTypeA(), new MoveTypeC(), new LeaveTypeA(), Random.Range(-100f, 100f), pathList[0], 4, 300, 2.5f, 4, 0, BulletType.Purple, 3);
        //yield return new WaitForSeconds(0.1f);
        //SpawnEnemy(enemyList[0], 5, 10, false, 0, 0, false, 0, Vector3PointGenerator.instance.GetPoint(10, 3, 0), Vector3PointGenerator.instance.GetPoint(10, 5, 4), Vector3PointGenerator.instance.GetPoint(0, 1, 14),
        //new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f), 1, 300, 1.5f, 3, 0, BulletType.Red,3);
        //yield return new WaitForSeconds(0.1f);
        //SpawnEnemy(enemyList[0], 5, 10, false, 0, 0, true, 5, Vector3PointGenerator.instance.GetPoint(11, 2, 0), Vector3PointGenerator.instance.GetPoint(11, 4, 3), Vector3PointGenerator.instance.GetPoint(0, 1, 14),
        //new EntryTypeA(), new MoveTypeC(), new LeaveTypeA(), Random.Range(-100f, 100f), pathList[0], 4, 300, 2.5f, 4, 0, BulletType.Purple, 3);
        //yield return new WaitForSeconds(0.1f);
        //SpawnEnemy(enemyList[0], 5, 10, true,1,3, false, 0, Vector3PointGenerator.instance.GetPoint(12, 3, 0), Vector3PointGenerator.instance.GetPoint(12, 5, 4), Vector3PointGenerator.instance.GetPoint(0, 1, 14),
        //new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f), 1, 300, 1.5f, 3, 0, BulletType.Red,3);
        //yield return new WaitForSeconds(0.1f);
    }
    IEnumerator WaveSpawn()
    {
        yield return new WaitForSeconds(4f);
        StartCoroutine(SpawnGroup1_1());

        yield return new WaitForSeconds(5f);
        StartCoroutine(SpawnGroup1_2());

        yield return new WaitForSeconds(5f);
        StartCoroutine(SpawnGroup1_3());

        yield return new WaitForSeconds(6f);
        StartCoroutine(SpawnGroup1_4());
        StartCoroutine(SpawnGroup1_1());
        yield return new WaitForSeconds(4f);
        StartCoroutine(SpawnGroup1_5());
        StartCoroutine(SpawnGroup1_2());
        yield return new WaitForSeconds(4f);
        StartCoroutine(SpawnGroup1_3());
        yield return new WaitForSeconds(6f);
        StartCoroutine(SpawnGroup1_6());
        yield return new WaitForSeconds(3f);
        StartCoroutine(SpawnGroup1_1());
        yield return new WaitForSeconds(4f);
        StartCoroutine (SpawnGroup1_2());
        StartCoroutine (SpawnGroup1_4());
        yield return new WaitForSeconds(4f);
        StartCoroutine(SpawnGroup1_7());
        yield return new WaitForSeconds(3f);
        StartCoroutine(SpawnGroup1_8());
        StartCoroutine(SpawnGroup1_2());
        yield return new WaitForSeconds(5f);
        StartCoroutine(SpawnGroup1_9());
        yield return new WaitForSeconds(3f);
        StartCoroutine(SpawnGroup1_10());
        yield return new WaitForSeconds(5f);
        StartCoroutine(SpawnGroup1_11());
        yield return new WaitForSeconds(5f);
        StartCoroutine(SpawnGroup1_4());
        StartCoroutine(SpawnGroup1_1());
        yield return new WaitForSeconds(4f);
        StartCoroutine(SpawnGroup1_12());
        StartCoroutine(SpawnGroup1_5());
        yield return new WaitForSeconds(6f);
        StartCoroutine(SpawnGroup1_13());
        StartCoroutine(SpawnGroup1_7());
        yield return new WaitForSeconds(13f);
        //wave 2
        yield return new WaitForSeconds(4f);
        StartCoroutine(SpawnGroup1_1());
        yield return new WaitForSeconds(5f);
        StartCoroutine(SpawnGroup1_2());
        yield return new WaitForSeconds(5f);
        StartCoroutine(SpawnGroup1_3());
        yield return new WaitForSeconds(6f);
        StartCoroutine(SpawnGroup1_4());
        StartCoroutine(SpawnGroup1_1());
        yield return new WaitForSeconds(4f);
        StartCoroutine(SpawnGroup1_5());
        StartCoroutine(SpawnGroup1_2());
        yield return new WaitForSeconds(4f);
        StartCoroutine(SpawnGroup1_3());
        yield return new WaitForSeconds(6f);
        StartCoroutine(SpawnGroup1_6());
        yield return new WaitForSeconds(3f);
        StartCoroutine(SpawnGroup1_1());
        yield return new WaitForSeconds(4f);
        StartCoroutine(SpawnGroup1_2());
        StartCoroutine(SpawnGroup1_4());
        yield return new WaitForSeconds(4f);
        StartCoroutine(SpawnGroup1_7());
        yield return new WaitForSeconds(3f);
        StartCoroutine(SpawnGroup1_8());
        StartCoroutine(SpawnGroup1_2());
        yield return new WaitForSeconds(5f);
        StartCoroutine(SpawnGroup1_9());
        yield return new WaitForSeconds(3f);
        StartCoroutine(SpawnGroup1_10());
        yield return new WaitForSeconds(5f);
        StartCoroutine(SpawnGroup1_11());
        yield return new WaitForSeconds(5f);
        StartCoroutine(SpawnGroup1_4());
        StartCoroutine(SpawnGroup1_1());
        yield return new WaitForSeconds(4f);
        StartCoroutine(SpawnGroup1_12());
        StartCoroutine(SpawnGroup1_5());
        yield return new WaitForSeconds(6f);
        StartCoroutine(SpawnGroup1_13());
        StartCoroutine(SpawnGroup1_7());
        yield return new WaitForSeconds(11f);
        GameFinish();
    }
    public void GameFinish()
    {
        gameoverPanel.SetActive(true);
        gameOverText.SetActive(false);
        GameClearText.SetActive(true);
        Time.timeScale = 0.0f;
    }
}
