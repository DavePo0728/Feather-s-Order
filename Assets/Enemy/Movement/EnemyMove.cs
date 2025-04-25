using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using System;

public class EnemyMove : MonoBehaviour
{
    IEntryBehaviour entryBehavior;
    IMoveBehaviour moveBehavior;
    ILeaveBehaviour leaveBehavior;
    EnemyHp enemyHp;
    public float entryTime;
    public float moveTime;
    public float leaveTime;
    public float lifeTime;
    bool paralyzing = false;   //是否癱瘓中
    [SerializeField]
    float currentParalyzeTime = 0;  //目前癱瘓時間
    public float initialParalyzeTime;   //初始癱瘓時間
    public float maxParalyzeTime;   //最大癱瘓時間
    public int paralyzeMaxCount;    //最大癱瘓次數
    [SerializeField]
    int currentParalyzeCount = 0;   //目前癱瘓次數
    public float paralyzeAddTime;   //增加癱瘓時間
    public float paralyzeTimeStackMultiplier;  //癱瘓時間堆疊倍率
    protected float angle;
    public Vector3 startPoint;
    public Vector3 endPoint;
    public Vector3 leavePoint;
    public float curveHeight;
    public float stayTime;
    public float pointWaitTime;
    public Vector3 originPos;
    public bool isLeave = false;
    
    //[HideInInspector]
    public Vector3[] path;
    public Vector3[] moveB_PathList;
    public Vector3[] moveC_PathList;
    [SerializeField]
    bool isDebug;
    [SerializeField]
    float DebugMoveTime;
    [SerializeField]
    bool isDebugHaveGun;
    public bool isMove =false;

    public List<GameObject> gunList;
    GameObject activeGun, activeGun1;
    EnemyData testData;
    GunData testGunData;
    //public bool canShoot = false;
    public enum BulletType
    {
        Black,
        Red,
        Purple,
        BlackRed,
    }
    public BulletType bulletType;
    //[SerializeField]
    //protected float rotationSpeed;
    //[SerializeField]
    //protected float radius;

    public void SetBehaviours(IEntryBehaviour entry,IMoveBehaviour move,ILeaveBehaviour leave)
    {
        entryBehavior = entry;
        moveBehavior = move;
        leaveBehavior = leave;
        //Debug.Log(this.name+" SetBehaviours "+"Entry: "+ entry+" Move: "+ move+" Leave: "+leave);
    }
    private void Awake()
    {
        enemyHp = GetComponent<EnemyHp>();
        if (entryTime+moveTime+leaveTime>lifeTime)
        {
            Debug.LogError("you are idoit sandwich!!!!");
        }
        if (!isDebug)
        {
            //testData = Resources.Load<EnemyData>("EnemyData/Test");
            gunList = new List<GameObject>();
            for (int i = 0; i < transform.GetChild(4).childCount; i++)
            {
                gunList.Add(transform.GetChild(4).GetChild(i).gameObject);
            }
        }
        else if (isDebugHaveGun)
        {
            //testData = Resources.Load<EnemyData>("EnemyData/Test");
            testGunData = Resources.Load<GunData>("GunData/Test");
            gunList = new List<GameObject>();
            for (int i = 0; i < transform.GetChild(4).childCount; i++)
            {
                gunList.Add(transform.GetChild(4).GetChild(i).gameObject);
            }
            ActiveGun(testGunData.data.gunIndex, testGunData.data.rpm, testGunData.data.bulletAmount, testGunData.data.spinSpeed, testGunData.data.shootingCoolDown, (EnemyMove.BulletType)testGunData.data.bulletType, testGunData.data.MaxShootWave);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        if (!isDebug)
        {
            startPoint = gameObject.transform.position;
            StartCoroutine(TimeToLeave());
            entryBehavior.Enter(this);
        }
        else
        {
            moveTime = DebugMoveTime;
            if (activeGun != null)
                activeGun.SetActive(true);
            originPos = transform.position;
            moveBehavior = new MoveTypeA();
            moveBehavior.Move(this);
            isMove = true;
        }
    }
    void FixedUpdate()
    {
        if (paralyzing)
        {
            currentParalyzeTime -= Time.deltaTime;
            if (currentParalyzeTime < 0)
            {
                recoverParalyze();
                paralyzing = false;
                currentParalyzeTime = 0;
                currentParalyzeCount = 0;
            }
        }
    }
    public void CallMove()
    {
        if(activeGun != null)
            activeGun.SetActive(true);
        originPos = transform.position;
        moveBehavior.Move(this);
        isMove = true;
    }
    public void ActiveGun(int gunIndex, float rpm, float bulletAmount, float spinSpeed,float shootingCoolDown,BulletType bulletType,float MaxShootWave)
    {
        if (gunIndex < 0 || gunIndex >= gunList.Count)
        {
            Debug.LogError($"Invalid gunIndex: {gunIndex}. It must be between 0 and {gunList.Count - 1}.");
            return;
        }
        
        switch (gunIndex)
        {
            
            case 0: //strightShooting
                SimpleShoot simpleShoot = gunList[gunIndex].GetComponent<SimpleShoot>();
                simpleShoot.rpm = rpm;
                simpleShoot.maxShots = bulletAmount;
                simpleShoot.shootingCoolDown = shootingCoolDown;
                simpleShoot.bulletType = (SimpleShoot.BulletType)bulletType;
                simpleShoot.maxShootWave = MaxShootWave;

                break;
            case 1: //trackShooting
                TrackShooting track = gunList[gunIndex].GetComponent<TrackShooting>();
                track.rpm = rpm;
                track.maxShots = bulletAmount;
                track.shootingCoolDown = shootingCoolDown;
                track.bulletType = (TrackShooting.BulletType)bulletType;
                track.maxShootWave = MaxShootWave;
                break;
            case 2: //shotGun
                ShootShotGun shootShotGun = gunList[gunIndex].GetComponent<ShootShotGun>();
                shootShotGun.bulletAmount = (int)bulletAmount;
                shootShotGun.shootingCoolDown = shootingCoolDown;
                shootShotGun.bulletType = (ShootShotGun.BulletType)bulletType;
                break;
            case 3: //SpreadShot
                SpreadShot spreadShot = gunList[gunIndex].GetComponent<SpreadShot>();
                spreadShot.bulletAmount = (int)bulletAmount;
                spreadShot.shootingCoolDown = shootingCoolDown;
                spreadShot.bulletType = (SpreadShot.BulletType)bulletType;
                spreadShot.MaxShootWave = MaxShootWave;
                break;
            case 4: //HomingShooter
                HomingShooter homingShooter = gunList[gunIndex].GetComponent<HomingShooter>();
                homingShooter.bulletAmount = (int)bulletAmount;
                homingShooter.shootingCoolDown = shootingCoolDown;
                homingShooter.bulletType = (HomingShooter.BulletType)bulletType;
                homingShooter.MaxShootWave = MaxShootWave;
                break;
            case 5: //FourWayGunSpin
                FourWayGunSpin fourWayGunSpin = gunList[gunIndex].GetComponent<FourWayGunSpin>();
                fourWayGunSpin.speed = spinSpeed;
                for (int i = 0; i < gunList[gunIndex].transform.childCount; i++)
                {
                    simpleShoot = gunList[gunIndex].transform.GetChild(i).GetComponent<SimpleShoot>();
                    simpleShoot.rpm = rpm;
                    simpleShoot.maxShots = bulletAmount;
                    simpleShoot.shootingCoolDown = shootingCoolDown;
                    simpleShoot.bulletType = (SimpleShoot.BulletType)bulletType;
                }
                break;
        }
        activeGun = gunList[gunIndex];
    }
    public void Paralyze()
    {
        if (currentParalyzeCount < paralyzeMaxCount)
        {
            currentParalyzeCount++;
            if (paralyzing == false)
            {
                paralyzing = true;
                currentParalyzeTime = initialParalyzeTime;
                if (activeGun != null && activeGun.activeSelf == true)
                    activeGun.SetActive(false);
                if (entryBehavior != null && entryBehavior.CheckEntryStatus())
                {
                    entryBehavior.ParalyzePause();
                }
                if (moveBehavior != null && moveBehavior.CheckMoveStatus())
                {
                    moveBehavior.ParalyzePause();
                }
                if (leaveBehavior != null && leaveBehavior.CheckLeaveStatus())
                {
                    leaveBehavior.ParalyzePause();
                }
            }
            else
            {
                if (currentParalyzeTime < maxParalyzeTime)
                {
                    currentParalyzeTime += paralyzeAddTime*(paralyzeTimeStackMultiplier/currentParalyzeCount);
                    //Debug.Log("currentParalyzeTime: " + currentParalyzeTime);
                    if (currentParalyzeTime > maxParalyzeTime)
                    {
                        currentParalyzeTime = maxParalyzeTime;
                    }
                }
            }
        }
    }
    void recoverParalyze()
    {
        enemyHp.CorruptionRecover();
        if (activeGun != null && activeGun.activeSelf == false)
            activeGun.SetActive(true);
        if (entryBehavior != null && entryBehavior.CheckEntryStatus() == false)
        {
            entryBehavior.ParalyzeRecover();
        }
        if (moveBehavior != null && moveBehavior.CheckMoveStatus() == false)
        {
            //Debug.Log("MoveBehaviorRecover");
            moveBehavior.ParalyzeRecover();
        }
        if (leaveBehavior != null && leaveBehavior.CheckLeaveStatus() == false)
        {
            leaveBehavior.ParalyzeRecover();
        }
    }
    IEnumerator TimeToLeave()
    {
        yield return new WaitForSeconds(lifeTime);
        isLeave = true;
        activeGun.SetActive(false);
        //foreach (GameObject gun in gunList)
        //{
        //    if(gun.activeSelf)
        //    {
        //       gun.SetActive(false);
        //    }
        //}
    }
    public void CallLeave()
    {
        leaveBehavior.Leave(this);
        foreach (GameObject gun in gunList)
        {
            if (gun.activeSelf)
            {
                gun.SetActive(false);
            }
        }
    }
    public void DestroyNow()
    {
        Destroy(gameObject);
    }
    private void OnDestroy()
    {
        if(moveBehavior != null)
            moveBehavior.StopMove();
        if (leaveBehavior != null)
            leaveBehavior.StopLeave();
        if (entryBehavior != null)
            entryBehavior.StopEnter();
    }
    void UpdateDebugLine()
    {
        
        CurvePathGenerator.pathInstance.DrawDebugLine(this.gameObject);
    }
    void OnDrawGizmos()
    {
        if (isMove)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(originPos, 10);
            //Debug.Log("Draw");
        }
    }
}
