using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTypeC : IMoveBehaviour
{
    public Tweener onMoveC;
    float pointWaitTime;
    public void Move(EnemyMove enemyMove)
    {
        pointWaitTime = enemyMove.pointWaitTime;
        if (enemyMove.gameObject != null)
        {
            //for (int i = 0; i < enemyMove.moveC_PathList.Length; i++)
            //{
            //    onMoveC = enemyMove.transform.DOMove(enemyMove.moveC_PathList[i], 2f).SetEase(Ease.Linear).SetDelay(pointWaitTime);
            //}
            onMoveC = enemyMove.transform.DOPath(enemyMove.moveC_PathList, 8).SetEase(Ease.Linear);
            onMoveC.Play();
            onMoveC.OnComplete(() => { enemyMove.CallLeave(); });
        }
    }
    public bool CheckMoveStatus()
    {
        if (onMoveC.IsPlaying())
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
        if (onMoveC != null && onMoveC.IsPlaying())
            onMoveC.Kill();
    }
    public void ParalyzePause()
    {
        onMoveC.Pause();
    }
    public void ParalyzeRecover()
    {
        onMoveC.Play();
    }
}
