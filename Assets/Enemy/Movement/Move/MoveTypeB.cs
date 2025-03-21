using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MoveTypeB : IMoveBehaviour
{
    
    public Tweener onMoveB;

    public void Move(EnemyMove enemyMove)
    {
        //Debug.Log("Enter Move Type B");
        if (enemyMove.gameObject != null)
        {
            onMoveB = enemyMove.transform.DOPath(enemyMove.moveB_PathList, enemyMove.stayTime).SetEase(Ease.Linear);
            onMoveB.Play();
            onMoveB.OnComplete(() => { Move(enemyMove); });
            if (enemyMove.isLeave)
            {
                StopMove();
                enemyMove.CallLeave();
            }
        }
    }
    public void StopMove()
    {
        if (onMoveB != null && onMoveB.IsPlaying())
            onMoveB.Kill();
    }
}
