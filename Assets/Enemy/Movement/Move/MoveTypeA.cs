using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MoveTypeA : IMoveBehaviour
{
    Vector3 _NextPos;
    Tweener onMove;
    public void Move(EnemyMove enemyMove)
    {
        NextPos(enemyMove);
        onMove = enemyMove.transform.DOMove(_NextPos, 1f).SetEase(Ease.Linear);
        onMove.OnComplete(() => { NextPos(enemyMove); });
    }
    void NextPos(EnemyMove enemyMove)
    {
        _NextPos = CurvePathGenerator.pathInstance.GetLandingPosZ(enemyMove.transform, 5);
        onMove.Restart();
    }
    public void KillMove()
    {
        onMove.Kill();
    }
}
