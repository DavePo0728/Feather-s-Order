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
    public void Move(EnemyMove enemyMove)
    {
        waitTime = enemyMove.pointWaitTime;
        //Debug.Log("Enter Move Type A");
        if (enemyMove.gameObject != null)
        {
            _nextPos = CurvePathGenerator.pathInstance.GetLandingPosZ(enemyMove.originPos, 10);
            //Debug.Log("EnemyName: "+enemyMove.name+" NextPos: " + _nextPos);
            onMoveA = enemyMove.transform.DOMove(_nextPos, enemyMove.moveTime).SetEase(Ease.Linear).SetDelay(waitTime).OnStart(() => { tweenPlaying = true; /*Debug.Log("MoveATweenStart"); */});
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
        if (onMoveA.IsPlaying())
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public void StopMove()
    {
        if(onMoveA != null&&onMoveA.IsPlaying())
            onMoveA.Kill();
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
