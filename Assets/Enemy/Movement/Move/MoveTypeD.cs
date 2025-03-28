using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MoveTypeD : IMoveBehaviour
{
    public Tweener onMoveD;
    public void Move(EnemyMove enemyMove)
    {
        onMoveD = enemyMove.transform.DOMove(enemyMove.leavePoint, enemyMove.moveTime) .SetEase(Ease.Linear);
        onMoveD.Play();
        onMoveD.OnComplete(() => { enemyMove.DestroyNow(); });
    }
    public bool CheckMoveStatus()
    {
        if (onMoveD.IsPlaying())
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
        if (onMoveD != null && onMoveD.IsPlaying())
            onMoveD.Kill();
    }
    public void ParalyzePause()
    {
        onMoveD.Pause();
    }
    public void ParalyzeRecover()
    {
        onMoveD.Play();
    }
}
