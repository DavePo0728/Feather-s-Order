using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Net;

public class EntryTypeA : IEntryBehaviour
{

    public void Enter(EnemyMove enemyMove)
    {
        //Debug.Log("Enter Type A");
        CurvePathGenerator.pathInstance.SetPosition(enemyMove.startPoint, enemyMove.endPoint, 100);
        enemyMove.path = CurvePathGenerator.pathInstance.GetPath();
        Tweener moveTweener = enemyMove.transform.DOPath(enemyMove.path, enemyMove.enterTime).SetEase(Ease.InOutSine);
        moveTweener.OnComplete(() =>{ enemyMove.CallMove(); });
    }
}

