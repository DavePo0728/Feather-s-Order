using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTypeE : IMoveBehaviour
{
    public Tweener onMoveE;
    float waitTime;
    public void Move(EnemyMove enemyMove)
    {
        waitTime = enemyMove.pointWaitTime;
        onMoveE = enemyMove.transform.DOMove(enemyMove.leavePoint, enemyMove.moveTime).SetEase(Ease.Linear).SetDelay(waitTime);
        onMoveE.Play();
        onMoveE.OnComplete(() => { enemyMove.DestroyNow(); });
    }
    public bool CheckMoveStatus()
    {
        if(onMoveE == null)
        {
            return false;

        }
        else
        {
            return onMoveE.IsPlaying();
        }
    }
    public void StopMove()
    {
        if (onMoveE != null && onMoveE.IsPlaying())
            onMoveE.Kill();
    }
    public void ParalyzePause()
    {
        onMoveE.Pause();
    }
    public void ParalyzeRecover()
    {
        onMoveE.Play();
    }
}
