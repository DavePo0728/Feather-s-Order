using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MoveTypeD : IMoveBehaviour
{
    public Tweener onMoveD;
    public void Move(EnemyMove enemyMove)
    {
        onMoveD = enemyMove.transform.DOMove(enemyMove.leavePoint.position, 3f) .SetEase(Ease.Linear);
        onMoveD.OnComplete(() => { enemyMove.CallLeave(); });
    }
    public void StopMove()
    {
        if (onMoveD != null && onMoveD.IsPlaying())
            onMoveD.Kill();
    }
}
