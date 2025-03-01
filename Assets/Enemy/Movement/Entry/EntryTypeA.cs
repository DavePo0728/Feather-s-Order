using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EntryTypeA : IEntryBehaviour
{
    Tweener enterTweener;
    public void Enter(EnemyMove enemyMove)
    {
        //Debug.Log("Enter Type A");
        CurvePathGenerator.pathInstance.SetPosition(enemyMove.startPoint, enemyMove.endPoint, enemyMove.curveHeight);
        enemyMove.path = CurvePathGenerator.pathInstance.GetPath();
        enterTweener = enemyMove.transform.DOPath(enemyMove.path, enemyMove.enterTime).SetEase(Ease.InOutSine);
        //enterTweener.OnComplete(() =>{ enemyMove.CallMove(); });
    }
    public void StopEnter()
    {
        if (enterTweener !=null&&enterTweener.IsPlaying())
        {
            enterTweener.Kill();
        }
    }
}

