using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MoveTypeA : IMoveBehaviour
{
    Vector3 _nextPos;
    public Tweener onMoveA;
    float waitTime;
    public void Move(EnemyMove enemyMove)
    {
        waitTime = enemyMove.pointWaitTime;
        //Debug.Log("Enter Move Type A");
        if (enemyMove.gameObject != null)
        {
            _nextPos = CurvePathGenerator.pathInstance.GetLandingPosZ(enemyMove.originPos, 10);
            onMoveA = enemyMove.transform.DOMove(_nextPos, enemyMove.moveTime).SetEase(Ease.Linear).SetDelay(waitTime)/*.OnStart(() => { Debug.Log("MoveATweenStart"); })*/;
            onMoveA.Play();
            onMoveA.OnComplete(() => { Move(enemyMove); });
            if (enemyMove.isLeave)
            {
                StopMove();
                enemyMove.CallLeave();
            }
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
