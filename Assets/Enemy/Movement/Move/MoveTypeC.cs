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
            for (int i = 0; i < enemyMove.moveC_PathList.Length; i++)
            {
                onMoveC = enemyMove.transform.DOMove(enemyMove.moveC_PathList[i], enemyMove.stayTime).SetEase(Ease.Linear).SetDelay(pointWaitTime);
            }
            //onMoveC = enemyMove.transform.DOPath(enemyMove.moveC_PathList, enemyMove.stayTime).SetEase(Ease.Linear);

            onMoveC.OnComplete(() => { enemyMove.CallLeave(); });
        }
    }
    public void StopMove()
    {
        if (onMoveC != null && onMoveC.IsPlaying())
            onMoveC.Kill();
    }
}
