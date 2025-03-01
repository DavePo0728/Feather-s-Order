using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class EnemyMove : MonoBehaviour
{
    IEntryBehaviour entryBehavior;
    IMoveBehaviour moveBehavior;
    ILeaveBehaviour leaveBehavior;
    //[SerializeField]
    public float moveSpeed;
    [SerializeField]
    float lifeTime;
    protected float angle;
    public Transform startPoint;
    public Transform endPoint;
    public Transform leavePoint;
    public float curveHeight;
    public float enterTime;
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
    public bool isMove =false;

    public List<GameObject> gunList;

    public bool canShoot = false;
    //[SerializeField]
    //protected float rotationSpeed;
    //[SerializeField]
    //protected float radius;

    public void SetBehaviours(IEntryBehaviour entry,IMoveBehaviour move,ILeaveBehaviour leave)
    {
        entryBehavior = entry;
        moveBehavior = move;
        leaveBehavior = leave;
    }
    private void Awake()
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log(transform.GetChild(3).childCount);
        startPoint = gameObject.transform;
        StartCoroutine(TimeToLeave());
        //UpdateDebugLine();
        entryBehavior.Enter(this);
        for (int i = 0; i < transform.GetChild(3).childCount; i++)
        {
            gunList.Add(transform.GetChild(3).GetChild(i).gameObject);
        }
    }
    void FixedUpdate()
    {
        //if (!isDebug&&!isEnter)
        //{
        //    isEnter = true;
        //    entryBehavior.Enter(this);
        //}

        //UpdateDebugLine();
        //print(isLeave);
    }
    public void CallMove(int gunNum)
    {
        originPos = transform.position;
        moveBehavior.Move(this);
        isMove = true;
        switch(gunNum)
        {
            case 0:
                SimpleShoot simpleShoot = gunList[0].GetComponent<SimpleShoot>();
                gunList[0].SetActive(true);
                break;
            case 1:
                
                break;
            case 2:
                
                break;
            case 3:

                break;
            case 4:
                
                break;
            case 5:
                
                break;
        }
    }
    IEnumerator TimeToLeave()
    {
        yield return new WaitForSeconds(lifeTime);
        isLeave = true;
        //gun.SetActive(false);
    }
    public void CallLeave()
    {
        leaveBehavior.Leave(this);
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
