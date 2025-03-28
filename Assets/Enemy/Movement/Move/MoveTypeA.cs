using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MoveTypeA : IMoveBehaviour
{
    Vector3 _nextPos;
    public Tweener onMoveA;
    public void Move(EnemyMove enemyMove)
    {
        //Debug.Log("Enter Move Type A");
        if (enemyMove.gameObject != null)
        {
            _nextPos = CurvePathGenerator.pathInstance.GetLandingPosZ(enemyMove.originPos, 10);
            onMoveA = enemyMove.transform.DOMove(_nextPos, enemyMove.moveTime).SetEase(Ease.Linear)/*.OnStart(() => { Debug.Log("MoveATweenStart"); })*/;
            onMoveA.Play();
            onMoveA.OnComplete(() => { Move(enemyMove);/*Debug.Log("MoveATweenFinish");*/ });
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
