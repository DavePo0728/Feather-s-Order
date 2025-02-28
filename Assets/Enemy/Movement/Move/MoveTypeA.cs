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
            onMoveA = enemyMove.transform.DOMove(_nextPos, 0.5f).SetEase(Ease.Linear);

            onMoveA.OnComplete(() => { Move(enemyMove); });
            if (enemyMove.isLeave)
            {
                StopMove();
                enemyMove.CallLeave();
            }
        }
    }
    public void StopMove()
    {
        if(onMoveA != null&&onMoveA.IsPlaying())
            onMoveA.Kill();
    }
    
}
