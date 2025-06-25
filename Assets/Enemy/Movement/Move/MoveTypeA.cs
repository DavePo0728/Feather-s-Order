using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MoveTypeA : IMoveBehaviour
{
    Vector3 _nextPos;
    public Tweener onMoveA;
    float waitTime;
    bool tweenPlaying=false;
    public float randomMoveRadius; 
    public void Move(EnemyMove enemyMove)
    {
        waitTime = enemyMove.pointWaitTime;
        randomMoveRadius = enemyMove.randomMoveRadius;
        //Debug.Log($"MovingRadius: {randomMoveRadius}");
        if (enemyMove.gameObject != null)
        {
            _nextPos = CurvePathGenerator.pathInstance.GetLandingPosZ(enemyMove.originPos, randomMoveRadius);
            //Debug.Log("EnemyName: "+enemyMove.name+" NextPos: " + _nextPos);
            onMoveA = enemyMove.transform.DOMove(_nextPos, enemyMove.moveTime).SetEase(Ease.Linear).SetDelay(waitTime).OnPause(() => { tweenPlaying = false; }).OnPlay(() => { tweenPlaying = true;});
            if(!tweenPlaying)
            onMoveA.Play();
            onMoveA.OnComplete(() => { tweenPlaying = false; Move(enemyMove); /*Debug.Log("TweenComplete");*/ });
            if (enemyMove.isLeave)
            {
                StopMove();
                enemyMove.CallLeave();
            }
            //onMoveA.OnKill(() => { tweenPlaying = false; Debug.Log("TweenKill"); });
        }
    }
    public bool CheckMoveStatus()
    {
        if (onMoveA == null)
        {
            return false;
        }
        else
        {
            //Debug.Log("MoveTypeA CheckMoveStatus: " + tweenPlaying);
            return tweenPlaying;
        }
    }
    public void StopMove()
    {
        if(onMoveA != null )
        {
            if (tweenPlaying)
            {
                onMoveA.Kill();
            }
        }
            
    }
    public void ParalyzePause()
    {
        onMoveA.Pause();
    }
    public void ParalyzeRecover()
    {
        onMoveA.Play();
    }
}
