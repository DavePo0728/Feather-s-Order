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
    GameObject gun;
    public bool isMove =false;
    EnemyData testData;

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
        if (gun != null)
        {
            if (gun.activeSelf == false)
                gun.SetActive(true);
        }
        originPos = transform.position;
        moveBehavior.Move(this);
        isMove = true;
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
                if (gun != null)
                {
                    if (gun.activeSelf == true)
                        gun.SetActive(false);
                }
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
        if (gun != null)
        {
            if (gun.activeSelf == false)
                gun.SetActive(true);
        }
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
        if (gun != null)
        {
            if (gun.activeSelf == true)
                gun.SetActive(false);
        }
    }
    public void CallLeave()
    {
        leaveBehavior.Leave(this);
        if (gun != null)
        {
            if (gun.activeSelf == true)
                gun.SetActive(false);
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
