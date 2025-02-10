using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MoveTypeA : IMoveBehaviour
{
    Vector3 _nextPos;
    Tweener onMove;
    public void Move(EnemyMove enemyMove)
    {

        
        _nextPos = CurvePathGenerator.pathInstance.GetLandingPosZ(enemyMove.originPos, 10);
        onMove = enemyMove.transform.DOMove(_nextPos, 0.5f).SetEase(Ease.Linear);
        onMove.OnComplete(() => { Move(enemyMove); });
    }
    public void KillMove()
    {
        onMove.Kill();
    }
}
