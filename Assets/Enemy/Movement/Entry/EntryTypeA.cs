using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
public class EntryTypeA : IEntryBehaviour
{
    Tweener enterTweener;
    public void Enter(EnemyMove enemyMove)
    {
        //Debug.Log(enemyMove.endPoint);
        CurvePathGenerator.pathInstance.SetPosition(enemyMove.startPoint, enemyMove.endPoint, enemyMove.curveHeight);
        enemyMove.path = CurvePathGenerator.pathInstance.GetPath();
        enterTweener = enemyMove.transform.DOPath(enemyMove.path, enemyMove.entryTime).SetEase(Ease.InOutSine)/*.OnStart(() => { Debug.Log("EntryATweenStart"); })*/;
        enterTweener.Play();
        enterTweener.OnComplete(() =>{ enemyMove.CallMove(); /*Debug.Log("EntryTweenFinish");*/ });
    }
    public void StopEnter()
    {
        if (enterTweener !=null)
        {
            if(enterTweener.IsPlaying())
            enterTweener.Kill();
        }
    }
    public bool CheckEntryStatus()
    {
        if (enterTweener == null)
        {
            return false;
        }
        else
        {
            return enterTweener.IsPlaying();
        }
    }
    public void ParalyzePause()
    {
        enterTweener.Pause();
    }
    public void ParalyzeRecover()
    {
        enterTweener.Play();
    }
}

