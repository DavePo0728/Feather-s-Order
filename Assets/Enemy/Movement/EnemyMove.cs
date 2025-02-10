using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    IEntryBehaviour entryBehavior;
    IMoveBehaviour moveBehavior;
    //[SerializeField]
    public float moveSpeed;
    [SerializeField]
    protected float lifeTime;
    protected float angle;
    public Transform startPoint;
    public Transform endPoint;
    public float curveHeight;
    public float enterTime;
    public float moveCDTime;
    public Vector3 originPos;
    //[HideInInspector]
    public Vector3[] path;
    [SerializeField]
    bool isDebug;
    public bool isMove =false;
    //[SerializeField]
    //protected float rotationSpeed;
    //[SerializeField]
    //protected float radius;

    public void SetBehaviours(IEntryBehaviour entry,IMoveBehaviour move)
    {
        entryBehavior = entry;
        moveBehavior = move;
    }
    private void Awake()
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        startPoint = gameObject.transform;
        //StartCoroutine(TimeToDestroy());
        //UpdateDebugLine();
        entryBehavior.Enter(this);
    }
    void FixedUpdate()
    {
        //if (!isDebug&&!isEnter)
        //{
        //    isEnter = true;
        //    entryBehavior.Enter(this);
        //}

        //UpdateDebugLine();

    }
    public void CallMove()
    {
        originPos = transform.position;
        moveBehavior.Move(this);
        isMove = true;
        //Debug.Log("Call Move");
    }
    IEnumerator TimeToDestroy()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
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
