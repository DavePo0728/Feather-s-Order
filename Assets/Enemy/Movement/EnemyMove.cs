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
        UpdateDebugLine();
        entryBehavior.Enter(this);
    }
    void FixedUpdate()
    {
        //if (!isDebug&&!isEnter)
        //{
        //    isEnter = true;
        //    entryBehavior.Enter(this);
        //}

        UpdateDebugLine();
    }
    IEnumerator TimeToDestroy()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }
    void UpdateDebugLine()
    {
        CurvePathGenerator.pathInstance.SetPosition(startPoint, endPoint,100);
        path = CurvePathGenerator.pathInstance.GetPath();
        CurvePathGenerator.pathInstance.DrawDebugLine(this.gameObject);
    }
}
